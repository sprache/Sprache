using System;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Construct a parser that indicates that the given parser
        /// is optional. The returned parser will succeed on
        /// any input no matter whether the given parser
        /// succeeds or not.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>An optional version of the given parser.</returns>
        public static Parser<IOption<T>> Optional<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var pr = parser(i);

                if (pr.WasSuccessful)
                    return Result.Success(new Some<T>(pr.Value), pr.Remainder);

                return Result.Success(new None<T>(), i);
            };
        }

        /// <summary>
        /// Constructs the eXclusive version of the Optional{T} parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>An eXclusive optional version of the given parser.</returns>
        /// <seealso cref="XOr"/>
        public static Parser<IOption<T>> XOptional<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var result = parser(i);

                if (result.WasSuccessful)
                    return Result.Success(new Some<T>(result.Value), result.Remainder);

                if (result.Remainder.Equals(i))
                    return Result.Success(new None<T>(), i);

                return Result.Failure<IOption<T>>(result.Remainder, result.Message, result.Expectations);
            };
        }

        /// <summary>
        /// Construct a parser that indicates that the given parser is optional
        /// and non-consuming. The returned parser will succeed on
        /// any input no matter whether the given parser succeeds or not.
        /// In any case, it won't consume any input, like a positive look-ahead in regex.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>A non-consuming version of the given parser.</returns>
        public static Parser<IOption<T>> Preview<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var result = parser(i);

                if (result.WasSuccessful)
                    return Result.Success(new Some<T>(result.Value), i);

                return Result.Success(new None<T>(), i);
            };
        }
    }
}
