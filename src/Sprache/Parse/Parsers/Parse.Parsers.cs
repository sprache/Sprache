using System.Globalization;
using System.Linq;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Parsers.
        /// </summary>
        public partial class Parsers
        {
            /// <summary>
            /// Parse an identifier as a letter followed by letters, numbers, and apostrophes.
            /// </summary>
            public static readonly Parser<string> Identifier =
                from first in Parse.Characters.Letter.Once()
                from rest in Parse.Characters.Letter.Or(Parse.Numerics.Digit.Or(Parse.Char('\''))).Many()
                select new string(first.Concat(rest).ToArray());
        }
    }
}