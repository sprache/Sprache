using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache.Union
{
    public static partial class Parse
    {
        public static UnionParser<char> Char(Predicate<char> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (description == null) throw new ArgumentNullException(nameof(description));

            var parser = new UnionParser<char>();
            parser.Name = "Char: " + description;
            parser.Parse = (i, state) =>
            {
                var start = i.Position;
                if (!i.AtEnd)
                {
                    if (predicate(i.Current))
                        return UnionResult.Success(i.Current, i.Advance(), parser.GetFullName(state), start, start + 1);

                    return UnionResult.Failure<char>();

                }

                return UnionResult.Failure<char>();
            };

            return parser;
        }

        /// <summary>
        /// Parse a single character except those matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Characters not to match.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser for characters except those matching <paramref name="predicate"/>.</returns>
        public static UnionParser<char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), "any character except " + description);
        }

        /// <summary>
        /// Parse a single character c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> Char(char c)
        {
            return Char(ch => c == ch, char.ToString(c));
        }


        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> Chars(params char[] c)
        {
            return Char(c.Contains, StringExtensions.Join("|", c));
        }

        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> Chars(string c)
        {
            return Char(c.ToEnumerable().Contains, StringExtensions.Join("|", c.ToEnumerable()));
        }


        /// <summary>
        /// Parse a single character except c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> CharExcept(char c)
        {
            return CharExcept(ch => c == ch, char.ToString(c));
        }

        /// <summary>
        /// Parses a single character except for those in the given parameters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> CharExcept(IEnumerable<char> c)
        {
            var chars = c as char[] ?? c.ToArray();
            return CharExcept(chars.Contains, StringExtensions.Join("|", chars));
        }

        /// <summary>
        /// Parses a single character except for those in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> CharExcept(string c)
        {
            return CharExcept(c.ToEnumerable().Contains, StringExtensions.Join("|", c.ToEnumerable()));
        }

        /// <summary>
        /// Parse a single character in a case-insensitive fashion.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static UnionParser<char> IgnoreCase(char c)
        {
            return Char(ch => char.ToLower(c) == char.ToLower(ch), char.ToString(c));
        }

        /// <summary>
        /// Parse any character.
        /// </summary>
        public static readonly UnionParser<char> AnyChar = Char(c => true, "any character");

        /// <summary>
        /// Parse a whitespace.
        /// </summary>
        public static readonly UnionParser<char> WhiteSpace = Char(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parse a digit.
        /// </summary>
        public static readonly UnionParser<char> Digit = Char(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static readonly UnionParser<char> Letter = Char(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static readonly UnionParser<char> LetterOrDigit = Char(char.IsLetterOrDigit, "letter or digit");

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static readonly UnionParser<char> Lower = Char(char.IsLower, "lowercase letter");

        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static readonly UnionParser<char> Upper = Char(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static readonly UnionParser<char> Numeric = Char(char.IsNumber, "numeric character");


        public static string LeftRecursionErrorMessage = "Left recursion detected";

        /// <summary>
        /// Parse a string of characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static UnionParser<IEnumerable<char>> String(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            var parser = s
                .ToEnumerable()
                .Select(Char)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);

            parser.Name = s;

            return parser;
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in regex.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the opposite of the given parser.</returns>
        public static UnionParser<object> Not<T>(this UnionParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            var resultParser = new UnionParser<object>();
            resultParser.Name = $"{parser.Name}_Not";
            resultParser.Parse = (i, state) =>
            {
                var startIndex = i.Position;
                var result = parser.Parse(i, state);

                if (result.WasSuccessful)
                {
                    return UnionResult.Failure<object>();
                }
                return UnionResult.Success<object>(null, i, resultParser.GetFullName(state), startIndex, i.Position);
            };

            return resultParser;
        }

        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static UnionParser<U> Then<T, U>(this UnionParser<T> first, Func<T, UnionParser<U>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            var parser = new UnionParser<U>();
            parser.Name = $"{first.Name}_Then";
            parser.Parse = (i, state) =>
            {
                var firstResult = first.Parse(i, state);
                var values = new List<UnionResultValue<U>>();

                if (firstResult.WasSuccessful)
                {
                    foreach (var item in firstResult.Values)
                    {
                        var secondParser = second(item.Value);

                        state.Path.Push(first.Name);
                        foreach (var secondItem in secondParser.Parse(item.Reminder, state).Values)
                        {
                            if (secondItem.WasSuccessful)
                            {
                                secondItem.StartIndex = item.StartIndex;
                                values.Add(secondItem);
                            }
                        }
                        state.Path.Pop();
                    }
                    return UnionResult.Success(values);
                }
                else
                {
                    return UnionResult.Failure<U>();
                }
            };

            return parser;
        }

        /// <summary>
        /// Parse a stream of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static UnionParser<IEnumerable<T>> Many<T>(this UnionParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            var resultParser = new UnionParser<IEnumerable<T>>();
            resultParser.Name = $"{parser.Name}_Many";
            resultParser.Parse = (i, state) =>
            {
                var items = new List<UnionResultValue<IEnumerable<T>>>();
               
                var r = parser.Parse(i, state);

                foreach (var item in r.Values)
                {
                    if (item.WasSuccessful && item.StartIndex != item.EndIndex)
                    {
                        var tmp = resultParser.Parse(item.Reminder, state);

                        if (tmp.WasSuccessful)
                        {
                            foreach (var itemTmp in tmp.Values)
                            {
                                var list = itemTmp.Value.ToList();
                                list.Insert(0, item.Value);
                                itemTmp.Value = list;
                                items.Add(itemTmp);
                            }
                        }
                        else
                        {
                            items.Add(new UnionResultValue<IEnumerable<T>>()
                            {
                                Value = new List<T>() { item.Value },
                                WasSuccessful = true,
                                ParserName = parser.GetFullName(state),
                                StartIndex = item.StartIndex,
                                EndIndex = item.EndIndex,
                                Reminder = item.Reminder
                            });
                        }
                    }
                }
                return UnionResult.Success(items);
            };

            return resultParser;
        }

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public static UnionParser<U> Select<T, U>(this UnionParser<T> parser, Func<T, U> convert)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (convert == null) throw new ArgumentNullException(nameof(convert));

            return parser.Then(t => Return(convert(t)));
        }

        /// <summary>
        /// Parse the token, embedded in any amount of whitespace characters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static UnionParser<T> Token<T>(this UnionParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return from leading in WhiteSpace.Many()
                   from item in parser
                   from trailing in WhiteSpace.Many()
                   select item;
        }
        

        /// <summary>
        /// Refer to another parser indirectly. This allows circular compile-time dependency between parsers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static UnionParser<T> Ref<T>(Func<UnionParser<T>> reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            UnionParser<T> p = null;

            var resultParser = new UnionParser<T>();
            resultParser.Name = "Reference";

            resultParser.Parse = (input, state) =>
            {
                if (p == null)
                    p = reference();

                if (input.Memos.ContainsKey(p))
                {
                    var pResult = input.Memos[p] as UnionResult<T>;
                    if (pResult.WasSuccessful)
                        return pResult;

                    if (!pResult.WasSuccessful && pResult.ErrorMessage == LeftRecursionErrorMessage)
                        throw new ParseException(pResult.ToString());
                }

                input.Memos[p] = UnionResult.Failure<T>(LeftRecursionErrorMessage);
                var result = p.Parse(input, state);
                input.Memos[p] = result;
                return result;
            };

            return resultParser;
        }

        /// <summary>
        /// Convert a stream of characters to a string.
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static UnionParser<string> Text(this UnionParser<IEnumerable<char>> characters)
        {
            return characters.Select(chs => new string(chs.ToArray()));
        }

        /// <summary>
        /// Parse first, if it succeeds, return first, otherwise try second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static UnionParser<T> Or<T>(this UnionParser<T> first, UnionParser<T> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            
            var parser = new UnionParser<T>();
            parser.Name = $"{first.Name}_OR_{second.Name}";

            parser.Parse = (i, state) =>
            {
                var firstResult = first.Parse(i, state);
                var secondResult = second.Parse(i, state);

                if (firstResult.WasSuccessful || secondResult.WasSuccessful)
                {
                    return UnionResult.Success(secondResult.Values.Concat(firstResult.Values));
                }
                else
                {
                    return UnionResult.Failure<T>();
                }
            };

            return parser;
        }

        /// <summary>
        /// Names part of the grammar for help with error messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UnionParser<T> Named<T>(this UnionParser<T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (name == null) throw new ArgumentNullException(nameof(name));

            parser.Name += name;

            return parser;
        }

        /// <summary>
        /// Parse a stream of elements containing only one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static UnionParser<IEnumerable<T>> Once<T>(this UnionParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Select(r => (IEnumerable<T>)new[] { r });
        }

        /// <summary>
        /// Concatenate two streams of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static UnionParser<IEnumerable<T>> Concat<T>(this UnionParser<IEnumerable<T>> first, UnionParser<IEnumerable<T>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return first.Then(f => second.Select(f.Concat));
        }

        /// <summary>
        /// Succeed immediately and return value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UnionParser<T> Return<T>(T value)
        {
            var parser = new UnionParser<T>();
            parser.Name = "Return";
            parser.Parse = (input, state) => UnionResult.Success(value, input, parser.Name, input.Position, input.Position);
            return parser;
        }

        /// <summary>
        /// Version of Return with simpler inline syntax.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UnionParser<U> Return<T, U>(this UnionParser<T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            return parser.Select(t => value);
        }

        /// <summary>
        /// Succeed if the parsed value matches predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static UnionParser<T> Where<T>(this UnionParser<T> parser, Func<T, bool> predicate)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var resultParser = new UnionParser<T>();
            resultParser.Name = $"{parser.Name}_Where";
            resultParser.Parse = (input, state) =>
            {
                var result = parser.Parse(input, state).IfSuccess(s => {

                    var filteredValues = s.Values.Where(i => predicate(i.Value));

                    if (filteredValues.Any())
                    {
                        return UnionResult.Success(filteredValues);
                    }
                    return UnionResult.Failure<T>();
                });

                return result;

            };
            return resultParser;
            
        }

        /// <summary>
        /// Monadic combinator Then, adapted for Linq comprehension syntax.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser"></param>
        /// <param name="selector"></param>
        /// <param name="projector"></param>
        /// <returns></returns>
        public static UnionParser<V> SelectMany<T, U, V>(
            this UnionParser<T> parser,
            Func<T, UnionParser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }
    }
}
