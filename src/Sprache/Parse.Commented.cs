using System;

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
    }
}
