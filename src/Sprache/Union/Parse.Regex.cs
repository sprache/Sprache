using System;
using System.Text.RegularExpressions;

namespace Sprache.Union
{
    partial class Parse
    {
        /// <summary>
        /// Construct a parser from the given regular expression.
        /// </summary>
        /// <param name="pattern">The regex expression.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>a parse of string</returns>
        public static UnionParser<string> Regex(string pattern, string description = null)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return Regex(new Regex(pattern), description);
        }

        /// <summary>
        /// Construct a parser from the given regular expression.
        /// </summary>
        /// <param name="regex">The regex expression.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>a parse of string</returns>
        public static UnionParser<string> Regex(Regex regex, string description = null)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            return RegexMatch(regex, description).Then(match => Return(match.Value));
        }

        /// <summary>
        /// Construct a parser from the given regular expression, returning a parser of
        /// type <see cref="Match"/>.
        /// </summary>
        /// <param name="pattern">The regex expression.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser of regex match objects.</returns>
        public static UnionParser<Match> RegexMatch(string pattern, string description = null)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return RegexMatch(new Regex(pattern), description);
        }

        /// <summary>
        /// Construct a parser from the given regular expression, returning a parser of
        /// type <see cref="Match"/>.
        /// </summary>
        /// <param name="regex">The regex expression.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser of regex match objects.</returns>
        public static UnionParser<Match> RegexMatch(Regex regex, string description = null)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));

            regex = OptimizeRegex(regex);

            var parser = new UnionParser<Match>();
            parser.Name = description ?? "RegexMatch";
            parser.Parse = (i, state) => 
            {
                if (!i.AtEnd)
                {
                    var remainder = i;
                    var input = i.Source.Substring(i.Position);
                    var match = regex.Match(input);

                    if (match.Success)
                    {
                        for (int j = 0; j < match.Length; j++)
                            remainder = remainder.Advance();

                        return UnionResult.Success(match, remainder, parser.GetFullName(state), i.Position, i.Position + match.Length);
                    }

                    return UnionResult.Failure<Match>();
                }

                return UnionResult.Failure<Match>("Unexpected end of input");
            };

            return parser;
        }

        /// <summary>
        /// Optimize the regex by only matching successfully at the start of the input.
        /// Do this by wrapping the whole regex in non-capturing parentheses preceded by
        ///  a `^'.
        /// </summary>
        /// <remarks>
        /// This method is invoked via reflection in unit tests. If renamed, the tests
        /// will need to be modified or they will fail.
        /// </remarks>
        private static Regex OptimizeRegex(Regex regex)
        {
            return new Regex(string.Format("^(?:{0})", regex), regex.Options);
        }
    }
}
