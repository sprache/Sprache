using System;
using System.Collections.Generic;

namespace Sprache
{
    internal static class StringExtensions
    {
        // String does not implement IEnumerable<char> in portable subset (WinRT specifically)
        public static IEnumerable<char> ToEnumerable(this string @this)
        {
            if (@this == null) throw new ArgumentNullException("@this");

            for (var i = 0; i < @this.Length; ++i)
            {
                yield return @this[i];
            }
        }
    }
}
