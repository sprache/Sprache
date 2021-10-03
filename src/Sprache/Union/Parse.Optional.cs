using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache.Union
{
    partial class Parse
    {
        /// <summary>
        /// Construct a parser that indicates that the given parser
        /// is optional. The returned parser will succeed on
        /// any input no matter whether the given parser
        /// succeeds or not.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>An optional version of the given parser.</returns>
        public static UnionParser<IOption<T>> Optional<T>(this UnionParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            var resultParser = new UnionParser<IOption<T>>();
            resultParser.Name = parser.Name + "_Optional";
            resultParser.Parse = (i, state) =>
            {
                var pr = parser.Parse(i, state);

                if (pr.WasSuccessful)
                {
                    return UnionResult.Success<IOption<T>>(new List<UnionResultValue<IOption<T>>>(pr.Values.Select(
                        v => new UnionResultValue<IOption<T>>() { Value = new Some<T>(v.Value), EndIndex = v.EndIndex, StartIndex = v.StartIndex, ParserName = v.ParserName, Reminder = v.Reminder, WasSuccessful = v.WasSuccessful })));
                }

                return UnionResult.Success((IOption<T>)new None<T>(), i, resultParser.Name, i.Position, i.Position);
            };

            return resultParser;
        }
    }
}
