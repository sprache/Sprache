using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Concatenate two streams of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return first.Then(f => second.Select(s => f.Concat(s)));
        }

        /// <summary>
        /// TryParse a stream of elements with at least one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Once().Then(t1 => parser.Many().Select(ts => t1.Concat(ts)));
        }

        /// <summary>
        /// TryParse a stream of elements with at least one item. Except the first
        /// item, all other items will be matched with the <code>XMany</code> operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> XAtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Once().Then(t1 => parser.XMany().Select(ts => t1.Concat(ts)));
        }

        /// <summary>
        /// Parse a stream of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <remarks>Implemented imperatively to decrease stack usage.</remarks>
        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var r = parser(i);

                while (r.WasSuccessful)
                {
                    if (remainder == r.Remainder)
                        break;

                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parser(remainder);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        /// <summary>
        /// Parse a stream of elements. If any element is partially parsed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <remarks>Implemented imperatively to decrease stack usage.</remarks>
        public static Parser<IEnumerable<T>> XMany<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Many().Then(m => parser.Once().XOr(Return(m)));
        }

        /// <summary>
        /// Parse a stream of elements containing only one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Select(r => (IEnumerable<T>)new[] { r });
        }
    }
}