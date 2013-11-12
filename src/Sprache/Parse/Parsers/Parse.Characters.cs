using System.Globalization;
using System.Linq;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Character parsers.
        /// </summary>
        public partial class Characters
        {
            /// <summary>
            /// Parse any character.
            /// </summary>
            public static readonly Parser<char> AnyChar = Parse.Char(c => true, "any character");

            /// <summary>
            /// Parse a whitespace.
            /// </summary>
            public static readonly Parser<char> WhiteSpace = Parse.Char(char.IsWhiteSpace, "whitespace");

            /// <summary>
            /// Parse a letter.
            /// </summary>
            public static readonly Parser<char> Letter = Parse.Char(char.IsLetter, "letter");

            /// <summary>
            /// Parse a letter or digit.
            /// </summary>
            public static readonly Parser<char> LetterOrDigit = Parse.Char(char.IsLetterOrDigit, "letter or digit");

            /// <summary>
            /// Parse a lowercase letter.
            /// </summary>
            public static readonly Parser<char> Lower = Parse.Char(char.IsLower, "lowercase letter");

            /// <summary>
            /// Parse an uppercase letter.
            /// </summary>
            public static readonly Parser<char> Upper = Parse.Char(char.IsUpper, "uppercase letter");
        }
    }
}