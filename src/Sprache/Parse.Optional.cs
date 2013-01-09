using System;

namespace Sprache
{
    partial class Parse
    {
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

                return Result.Success(new None<T>(), pr.Remainder);
            };
        }
    }
}
