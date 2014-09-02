namespace Sprache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Parse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="delimiter"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Parser<IEnumerable<T>> DelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (delimiter == null) throw new ArgumentNullException("delimiter");

            return from head in parser.Once()
                   from tail in
                       (from separator in delimiter
                        from item in parser
                        select item).Many()
                   select head.Concat(tail);
        }

        /// <summary>
        /// Fails on the first itemParser failure, if it reads at least one character.
        /// </summary>
        /// <param name="itemParser"></param>
        /// <param name="delimiter"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Parser<IEnumerable<T>> XDelimitedBy<T, U>(this Parser<T> itemParser, Parser<U> delimiter)
        {
            if (itemParser == null) throw new ArgumentNullException("itemParser");
            if (delimiter == null) throw new ArgumentNullException("delimiter");

            return from head in itemParser.Once()
                   from tail in
                       (from separator in delimiter
                        from item in itemParser
                        select item).XMany()
                   select head.Concat(tail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            return Repeat(parser, count, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int minimumCount, int maximumCount)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var remainder = i;
                var result = new List<T>();

                for (var n = 0; n < maximumCount; ++n)
                {
                    var r = parser(remainder);

                    if (!r.WasSuccessful && n < minimumCount)
                    {
                        var what = r.Remainder.AtEnd
                            ? "end of input"
                            : r.Remainder.Current.ToString();

                        var msg = string.Format("Unexpected '{0}'", what);
                        var exp = string.Format("'{0}' at least {1} times, but was {2}", string.Join(", ", r.Expectations), minimumCount, n);
                        return Result.Failure<IEnumerable<T>>(i, msg, new[] { exp });
                    }

                    if (remainder != r.Remainder)
                    {
                        result.Add(r.Value);
                    }

                    remainder = r.Remainder;
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Parser<T> Contained<T, U, V>(this Parser<T> parser, Parser<U> open, Parser<V> close)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (open == null) throw new ArgumentNullException("open");
            if (close == null) throw new ArgumentNullException("close");

            return from o in open
                   from item in parser
                   from c in close
                   select item;
        }
    }
}
