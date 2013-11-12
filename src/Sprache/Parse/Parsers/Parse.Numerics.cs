using System.Globalization;
using System.Linq;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Number parsers.
        /// </summary>
        public partial class Numerics
        {
            /// <summary>
            /// Parse a digit.
            /// </summary>
            public static readonly Parser<char> Digit = Parse.Char(char.IsDigit, "digit");

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
}