using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// TryParse a single character matching 'predicate'
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Parser<char> Char(Predicate<char> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (description == null) throw new ArgumentNullException("description");

            return i =>
            {
                if (!i.AtEnd)
                {
                    if (predicate(i.Current))
                        return Result.Success(i.Current, i.Advance());

                    return Result.Failure<char>(i,
                        string.Format("unexpected '{0}'", i.Current),
                        new[] { description });
                }

                return Result.Failure<char>(i,
                    "Unexpected end of input reached",
                    new[] { description });
            };
        }

        /// <summary>
        /// Parse a single character except those matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Characters not to match.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser for characters except those matching <paramref name="predicate"/>.</returns>
        public static Parser<char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), "any character except " + description);
        }

        /// <summary>
        /// Parse a single character c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> Char(char c)
        {
            return Char(ch => c == ch, char.ToString(c));
        }


        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> Chars(params char[] c)
        {
            return Char(c.Contains, string.Join("|", c));
        }

        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> Chars(string c)
        {
            return Char(c.Contains, string.Join("|", c.ToCharArray()));
        }


        /// <summary>
        /// Parse a single character except c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> CharExcept(char c)
        {
            return CharExcept(ch => c == ch, char.ToString(c));
        }

        /// <summary>
        /// Parses a single character except for those in the given parameters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> CharExcept(IEnumerable<char> c)
        {
            var chars = c as char[] ?? c.ToArray();
            return CharExcept(chars.Contains, string.Join("|", chars));
        }

        /// <summary>
        /// Parses a single character except for those in c
        /// </summary>  
        /// <param name="c"></param>
        /// <returns></returns> 
        public static Parser<char> CharExcept(string c)
        {
            return CharExcept(c.Contains, string.Join("|", c.ToCharArray()));
        }

        /// <summary>
        /// Parse a single character in a case-insensitive fashion.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> IgnoreCase(char c)
        {
            return Char(ch => char.ToLower(c) == char.ToLower(ch), char.ToString(c));
        }

        /// <summary>
        /// Parse a string in a case-insensitive fashion.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<char>> IgnoreCase(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            return s
                .Select(IgnoreCase)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);
        }

        /// <summary>
        /// Parse a string of characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<char>> String(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            return s
                .Select(Char)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);
        }
    }
}
