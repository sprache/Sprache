using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    /// <summary>
    /// Parsers and combinators.
    /// </summary>
    public static partial class Parse
    {
        /// <summary>
        /// Parse end-of-input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<T> End<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i => parser(i).IfSuccess(s =>
                s.Remainder.AtEnd
                    ? s
                    : Result.Failure<T>(
                        s.Remainder,
                        string.Format("unexpected '{0}'", s.Remainder.Current),
                        new[] { "end of input" }));
        }

        /// <summary>
        /// Names part of the grammar for help with error messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Parser<T> Named<T>(this Parser<T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (name == null) throw new ArgumentNullException("name");

            return i => parser(i).IfFailure(f => f.Remainder == i ?
                Result.Failure<T>(f.Remainder, f.Message, new[] { name }) :
                f);
        }

        /// <summary>
        /// Construct a parser that will set the position to the position-aware
        /// T on succsessful match.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<T> Positioned<T>(this Parser<T> parser) where T : IPositionAware<T>
        {
            return i =>
            {
                var r = parser(i);

                if (r.WasSuccessful)
                {
                    return Result.Success(r.Value.SetPos(Position.FromInput(i), r.Remainder.Position - i.Position), r.Remainder);
                }
                return r;
            };
        }

        /// <summary>
        /// Refer to another parser indirectly. This allows circular compile-time dependency between parsers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Parser<T> Ref<T>(Func<Parser<T>> reference)
        {
            if (reference == null) throw new ArgumentNullException("reference");

            Parser<T> p = null;

            return i =>
            {
                if (p == null)
                    p = reference();

                if (i.Memos.ContainsKey(p))
                    throw new ParseException(i.Memos[p].ToString());

                i.Memos[p] = Result.Failure<T>(i,
                    "Left recursion in the grammar.",
                    new string[0]);
                var result = p(i);
                i.Memos[p] = result;
                return result;
            };
        }

        /// <summary>
        /// Succeed immediately and return value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }

        /// <summary>
        /// Version of Return with simpler inline syntax.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Parser<U> Return<T, U>(this Parser<T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            return parser.Select(t => value);
        }

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (convert == null) throw new ArgumentNullException("convert");

            return parser.Then(t => Return(convert(t)));
        }

        /// <summary>
        /// Monadic combinator Then, adapted for Linq comprehension syntax.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <param name="selector"></param>
        /// <param name="projector"></param>
        /// <returns></returns>
        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (selector == null) throw new ArgumentNullException("selector");
            if (projector == null) throw new ArgumentNullException("projector");

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        /// <summary>
        /// Convert a stream of characters to a string.
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static Parser<string> Text(this Parser<IEnumerable<char>> characters)
        {
            return characters.Select(chs => new string(chs.ToArray()));
        }

        /// <summary>
        /// Parse the token, embedded in any amount of whitespace characters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<T> Token<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            return parser.Contained(Parse.Characters.WhiteSpace.Many());
        }

    }
}
