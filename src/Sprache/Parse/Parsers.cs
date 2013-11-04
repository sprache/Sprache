using System.Globalization;
using System.Linq;

namespace Sprache
{
    partial class Parse
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
        /// Parse a digit.
        /// </summary>
        public static readonly Parser<char> Digit = Parse.Char(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static readonly Parser<char> Letter = Parse.Char(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static readonly Parser<char> LetterOrDigit = Parse.Char(char.IsLetterOrDigit, "letter or digit");

        /// <summary>
        /// Parse an identifier as a letter followed by letters, numbers, and apostrophes.
        /// </summary>
        public static readonly Parser<string> Identifier =
            from first in Parse.Letter.Once()
            from rest in Parse.Letter.Or(Parse.Digit.Or(Parse.Char('\''))).Many()
            select new string(first.Concat(rest).ToArray());

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static readonly Parser<char> Lower = Parse.Char(char.IsLower, "lowercase letter");

        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static readonly Parser<char> Upper = Parse.Char(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static readonly Parser<char> Numeric = Parse.Char(char.IsNumber, "numeric character");

        /// <summary>
        /// Parse a number.
        /// </summary>
        public static readonly Parser<string> Number = Numeric.AtLeastOnce().Text();

        static readonly Parser<string> DecimalWithoutLeadingDigits =
            from nothing in Parse.Return("") // dummy so that CultureInfo.CurrentCulture is evaluated later
            from dot in Parse.String(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Text()
            from fraction in Number
            select dot + fraction;

        static readonly Parser<string> DecimalWithLeadingDigits =
            Number.Then(n => DecimalWithoutLeadingDigits.XOr(Parse.Return("")).Select(f => n + f));

        /// <summary>
        /// Parse a decimal number.
        /// </summary>
        public static readonly Parser<string> Decimal = DecimalWithLeadingDigits.XOr(DecimalWithoutLeadingDigits);
    }
}