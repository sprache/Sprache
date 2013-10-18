using System;
using System.IO;

namespace Sprache
{
    /// <summary>
    /// Represents a parser.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="input">The input to parse.</param>
    /// <returns>The result of the parser.</returns>
    public delegate IResult<T> Parser<out T>(Input input);

    /// <summary>
    /// Contains some extension methods for <see cref="Parser&lt;T&gt;" />.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Tries to parse the input without throwing an exception.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser</returns>
        public static IResult<T> TryParse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            return parser(new Input(input));
        }

        public static IResult<T> TryParse<T>(this Parser<T> parser, TextReader _input)
        {
            string input = _input.ReadToEnd();

            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            return parser(new Input(input));
        }

        /// <summary>
        /// Parses the specified input string.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser.</returns>
        /// <exception cref="Sprache.ParseException">It contains the details of the parsing error.</exception>
        public static T Parse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            var result = parser.TryParse(input);
            
            if(result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString());
        }

        public static T Parse<T>(this Parser<T> parser, TextReader _input)
        {
            string input = _input.ReadToEnd();

            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            var result = parser.TryParse(input);

            if (result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString());
        }
    }
}
