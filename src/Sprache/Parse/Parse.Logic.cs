using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Attempt parsing only if the <paramref name="except"/> parser fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="except"></param>
        /// <returns></returns>
        public static Parser<T> Except<T, U>(this Parser<T> parser, Parser<U> except)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (except == null) throw new ArgumentNullException("except");

            // Could be more like: except.Then(s => s.Fail("..")).XOr(parser)
            return i =>
            {
                var r = except(i);
                if (r.WasSuccessful)
                    return Result.Failure<T>(i, "Excepted parser succeeded.", new[] { "other than the excepted input" });
                return parser(i);
            };
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in regex.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the opposite of the given parser.</returns>
        public static Parser<object> Not<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var result = parser(i);

                if (result.WasSuccessful)
                {
                    var msg = string.Format("`{0}' was not expected", string.Join(", ", result.Expectations));
                    return Result.Failure<object>(i, msg, new string[0]);
                }
                return Result.Success<object>(null, i);
            };
        }

        /// <summary>
        /// Construct a parser that indicates the given parser
        /// is optional. The returned parser will succeed on
        /// any input no matter whether the given parser
        /// succeeds or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IOption<T>> Optional<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var pr = parser(i);

                if (pr.WasSuccessful)
                    return Result.Success(new Some<T>(pr.Value), pr.Remainder);

                return Result.Success(new None<T>(), i);
            };
        }

        /// <summary>
        /// Parse first, if it succeeds, return first, otherwise try second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i =>
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    return second(i).IfFailure(sf => Result.Failure<T>(
                        fr.Remainder,
                        fr.Message,
                        fr.Expectations.Union(sf.Expectations)));
                }

                if (fr.Remainder == i)
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        /// <summary>
        /// Parse first, if it succeeds, return first, otherwise try second.
        /// Assumes that the first parsed character will determine the parser chosen (see Try).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<T> XOr<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i =>
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    if (fr.Remainder != i)
                        return fr;

                    return second(i).IfFailure(sf => Result.Failure<T>(
                        fr.Remainder,
                        fr.Message,
                        fr.Expectations.Union(sf.Expectations)));
                }

                if (fr.Remainder == i)
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
        }

        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// Second is a parser, not a lambda.
        /// For cases where the lambda is: a => second.Return(a)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<T> Then<T, U>(this Parser<T> first, Parser<U> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return first.Then(a => second.Return(a));
        }

        /// <summary>
        /// Succeed if the parsed value matches predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return i => parser(i).IfSuccess(s =>
                predicate(s.Value) ? s : Result.Failure<T>(i,
                    string.Format("Unexpected {0}.", s.Value),
                    new string[0]));
        }
    }
}