using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache
{
    public delegate IResult<T> Parser<out T>(Input input);

    public static class ParserExtensions
    {
        public static IResult<T> TryParse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            return parser(new Input(input));
        }

        public static T Parse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            var result = parser.TryParse(input);
            
            if(result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString());
        }
    }
}
