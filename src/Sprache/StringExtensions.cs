using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    internal static class StringExtensions
    {
        public static IEnumerable<char> ToEnumerable(this string @this)
        {
#if !NETSTANDARD1_0
            return @this;
#else
            if (@this == null) throw new ArgumentNullException(nameof(@this));

            for (var i = 0; i < @this.Length; ++i)
            {
                yield return @this[i];
            }
#endif
        }

        public static string Join<T>(string separator, IEnumerable<T> values)
        {
#if !NET35
            return string.Join(separator, values);
#else
            return string.Join(separator, values.Select(v => v.ToString()).ToArray());
#endif
        }
    }
}
