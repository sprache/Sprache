using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Represents a text span of the matched result.
        /// </summary>
        /// <typeparam name="T">Type of the matched result.</typeparam>
        private class TextSpan<T> : ITextSpan<T>
        {
            public T Value { get; set; }

            public Position Start { get; set; }

            public Position End { get; set; }

            public int Length { get; set; }
        }

        /// <summary>
        /// Constructs a parser that returns the <see cref="ITextSpan{T}"/> of the parsed value.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>A parser for the text span of the given parser.</returns>
        public static Parser<ITextSpan<T>> Span<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var r = parser(i);
                if (r.WasSuccessful)
                {
                    var span = new TextSpan<T>
                    {
                        Value = r.Value,
                        Start = Position.FromInput(i),
                        End = Position.FromInput(r.Remainder),
                        Length = r.Remainder.Position - i.Position,
                    };

                    return Result.Success(span, r.Remainder);
                }

                return Result.Failure<ITextSpan<T>>(r.Remainder, r.Message, r.Expectations);
            };
        }

        /// <summary>
        /// Represents a commented result with its leading and trailing comments.
        /// </summary>
        /// <typeparam name="T">Type of the matched result.</typeparam>
        private class CommentedValue<T> : ICommented<T>
        {
            public CommentedValue(T value)
            {
                LeadingComments = TrailingComments = EmptyStringList;
                Value = value;
            }

            public CommentedValue(IEnumerable<string> leading, T value, IEnumerable<string> trailing)
            {
                LeadingComments = leading ?? EmptyStringList;
                Value = value;
                TrailingComments = trailing ?? EmptyStringList;
            }

            public T Value { get; }

            public IEnumerable<string> LeadingComments { get; }

            public IEnumerable<string> TrailingComments { get; }
        }

        private static readonly string[] EmptyStringList = new string[0];

        private static readonly IComment DefaultCommentParser = new CommentParser();

        /// <summary>
        /// Constructs a parser that consumes a whitespace and all comments
        /// parsed by the commentParser.AnyComment parser, but parses only one trailing
        /// comment that starts exactly on the last line of the parsed value.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <param name="commentParser">The comment parser.</param>
        /// <returns>An extended Token() version of the given parser.</returns>
        public static Parser<ICommented<T>> Commented<T>(this Parser<T> parser, IComment commentParser = null)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            // consume any comment supported by the comment parser
            var comment = (commentParser ?? DefaultCommentParser).AnyComment;

            // parses any whitespace except for the new lines
            var whiteSpaceExceptForNewLine = WhiteSpace.Except(Chars("\r\n")).Many().Text();

            // returns true if the second span starts on the first span's last line
            bool IsSameLine(ITextSpan<T> first, ITextSpan<string> second) =>
                first.End.Line == second.Start.Line;

            // single comment span followed by a whitespace
            var commentSpan =
                from cs in comment.Span()
                from ws in whiteSpaceExceptForNewLine
                select cs;

            // add leading and trailing comments to the parser
            return
                from leadingWhiteSpace in WhiteSpace.Many()
                from leadingComments in comment.Token().Many()
                from valueSpan in parser.Span()
                from trailingWhiteSpace in whiteSpaceExceptForNewLine
                from trailingPreview in commentSpan.Many().Preview()
                let trailingCount = trailingPreview.GetOrElse(Enumerable.Empty<ITextSpan<string>>())
                    .Count(c => IsSameLine(valueSpan, c))
                from trailingComments in commentSpan.Repeat(trailingCount)
                select new CommentedValue<T>(leadingComments, valueSpan.Value, trailingComments.Select(c => c.Value));
        }
    }
}
