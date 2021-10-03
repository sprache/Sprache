using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache.Union
{
    public static class ParserExtensions
    {
        public static State LastState { get; private set; }

        /// <summary>
        /// Tries to parse the input without throwing an exception.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser</returns>
        public static UnionResult<T> DoTryParse<T>(this UnionParser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == null) throw new ArgumentNullException(nameof(input));

            var state = new State();
            var result = parser.Parse(new Input(input), state);

            LastState = state;

            return result;
        }

        /// <summary>
        /// Parses the specified input string.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser.</returns>
        /// <exception cref="Sprache.ParseException">It contains the details of the parsing error.</exception>
        public static List<T> DoParse<T>(this UnionParser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == null) throw new ArgumentNullException(nameof(input));

            var result = parser.DoTryParse(input);

            if (result.WasSuccessful)
                return result.Values.Select(v => v.Value).ToList();

            throw new ParseException(result.ToString());
        }
    }
}
