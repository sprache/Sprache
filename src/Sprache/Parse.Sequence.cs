namespace Sprache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Parse
    {
        public static Parser<IEnumerable<T>> DelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (delimiter == null) throw new ArgumentNullException("delimiter");

            return from head in parser.Once()
                   from tail in
                       (from separator in delimiter
                        from item in parser
                        select item).Many()
                   select head.Concat(tail);
        }

        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var remainder = i;
                var result = new List<T>();

                for (var n = 0; n < count; ++n)
                {
                    var r = parser(remainder);
                    if (!r.WasSuccessful)
                    {
                        var what = r.Remainder.AtEnd
                            ? "end of input"
                            : r.Remainder.Current.ToString();

                        var msg = string.Format("Unexpected '{0}'", what);
                        var exp = string.Format("'{0}' {1} times, but was {2}", string.Join(", ", r.Observations.SelectMany(x => x.Expectations)), count, n);
                        return Result.Failure<IEnumerable<T>>(i, 
                            Observe.Error(msg, exp));
                    }

                    if (remainder != r.Remainder)
                    {
                        result.Add(r.Value);
                    }

                    remainder = r.Remainder;
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }


        public static Parser<T> Contained<T, U, V>(this Parser<T> parser, Parser<U> open, Parser<V> close)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (open == null) throw new ArgumentNullException("open");
            if (close == null) throw new ArgumentNullException("close");

            return from o in open
                   from item in parser
                   from c in close
                   select item;
        }
    }
}
