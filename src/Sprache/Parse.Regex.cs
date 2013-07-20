using System;
using System.Text.RegularExpressions;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Construct a parser from the given regular expression.
        /// </summary>
        /// <param name="pattern">The regex expression.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>a parse of string</returns>
        public static Parser<string> Regex(string pattern, string description = null)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            return Regex(new Regex(pattern, RegexOptions.Compiled), description);
        }

        /// <summary>
        /// Construct a parser from the given regular expression.
        /// </summary>
        /// <param name="regex">The regex expression.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>a parse of string</returns>
        public static Parser<string> Regex(Regex regex, string description = null)
        {
            if (regex == null) throw new ArgumentNullException("regex");

            var expectations = description == null
                ? new string[0]
                : new[] { description };

            return i =>
            {
                if (!i.AtEnd)
                {
                    var remainder = i;
                    var input = i.Source.Substring(i.Position);
                    var match = regex.Match(input);

                    if (match.Success && match.Index == 0)
                    {
                        for (int j = 0; j < match.Length; j++)
                            remainder = remainder.Advance();

                        return Result.Success(match.Value, remainder);
                    }

                    var found = match.Index == input.Length
                                    ? "end of source"
                                    : string.Format("`{0}'", input[match.Index]);
                    return Result.Failure<string>(
                        remainder,
                        Observe.Error("string matching regex `" + regex.ToString() + "' expected but " + found + " found", expectations));
                }

                return Result.Failure<string>(i,
                    Observe.Error("Unexpected end of input", expectations));
            };
        }
    }
}
