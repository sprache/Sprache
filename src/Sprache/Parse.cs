﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sprache
{
    /// <summary>
    /// Parsers and combinators.
    /// </summary>
    public static partial class Parse
    {
        /// <summary>
        /// TryParse a single character matching 'predicate'
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Parser<char> Char(Predicate<char> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (description == null) throw new ArgumentNullException("description");

            return i =>
            {
                if (!i.AtEnd)
                {
                    if (predicate(i.Current))
                        return Result.Success(i.Current, i.Advance());

                    return Result.Failure<char>(i, 
                        Observe.Error(string.Format("unexpected '{0}'", i.Current), description));
                }

                return Result.Failure<char>(i,
                    Observe.Error("Unexpected end of input reached", description));
            };
        }

        /// <summary>
        /// Parse a single character except those matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Characters not to match.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser for characters except those matching <paramref name="predicate"/>.</returns>
        public static Parser<char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), "any character except " + description);
        }

        /// <summary>
        /// Parse a single character c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> Char(char c)
        {
            return Char(ch => c == ch, char.ToString(c));
        }


        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> Chars(params char[] c)
        {
            return Char(c.Contains, string.Join("|", c));
        }

        /// <summary>
        /// Parse a single character of any in c
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> Chars(string c)
        {
            return Char(c.Contains, string.Join("|", c.ToCharArray()));
        }


        /// <summary>
        /// Parse a single character except c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> CharExcept(char c)
        {
            return CharExcept(ch => c == ch, char.ToString(c));
        }

        /// <summary>
        /// Parses a single character except for those in the given parameters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> CharExcept(IEnumerable<char> c)
        {
            var chars = c as char[] ?? c.ToArray();
            return CharExcept(chars.Contains, string.Join("|", chars));
        }

        /// <summary>
        /// Parses a single character except for those in c
        /// </summary>  
        /// <param name="c"></param>
        /// <returns></returns> 
        public static Parser<char> CharExcept(string c)
        {
            return CharExcept(c.Contains, string.Join("|", c.ToCharArray()));
        }

        /// <summary>
        /// Parse a single character in a case-insensitive fashion.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> IgnoreCase(char c)
        {
            return Char(ch => char.ToLower(c) == char.ToLower(ch), char.ToString(c));
        }

        /// <summary>
        /// Parse a string in a case-insensitive fashion.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<char>> IgnoreCase(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            return s
                .Select(IgnoreCase)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);
        }

        /// <summary>
        /// Parse any character.
        /// </summary>
        public static readonly Parser<char> AnyChar = Char(c => true, "any character");

        /// <summary>
        /// Parse a whitespace.
        /// </summary>
        public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parse a digit.
        /// </summary>
        public static readonly Parser<char> Digit = Char(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static readonly Parser<char> Letter = Char(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static readonly Parser<char> LetterOrDigit = Char(char.IsLetterOrDigit, "letter or digit");

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static readonly Parser<char> Lower = Char(char.IsLower, "lowercase letter");
        
        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static readonly Parser<char> Upper = Char(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static readonly Parser<char> Numeric = Char(char.IsNumber, "numeric character");

        /// <summary>
        /// Parse a string of characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<char>> String(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            return s
                .Select(Char)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in regex.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the opposite of the given parser.</returns>
        public static Parser<object> Not<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var result = parser(i);

                if (result.WasSuccessful)
                {
                    var msg = string.Format("'{0}' was not expected", string.Join(", ", result.Observations.SelectMany(x => x.Expectations)));
                    return Result.Failure<object>(i, Observe.Error(msg));
                }
                return Result.Success<object>(null, i);
            };
        }
 
        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
        }

        /// <summary>
        /// Parses, but does not consume the input
        /// </summary>
        /// <param name="parser"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<T> LookAhead<T>(this Parser<T> parser)
        {
            return input =>
            {
                var results = parser(input);
                return results.WasSuccessful
                    ? Result.Success(results.Value, input, results.Observations)
                    : results;
            };
        }

        /// <summary>
        /// Parse a stream of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <remarks>Implemented imperatively to decrease stack usage.</remarks>
        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var r = parser(i);

                while (r.WasSuccessful)
                {
                    if (remainder == r.Remainder)
                        break;

                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parser(remainder);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        /// <summary>
        /// Parse a stream of elements, attemting to skip over invalid elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <remarks>Implemented imperatively to decrease stack usage.</remarks>
        public static Parser<IEnumerable<T>> ManyWithPanic<T, U>(this Parser<T> parser, Parser<U> panicUntil, string elementDescription)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (panicUntil == null) throw new ArgumentNullException("panicUntil");

            return i =>
            {
                var remainder = i;
                var result = new List<T>();
                var observations = new List<ResultObservation>();

                var r = parser(i);

                while (true)
                {
                    if (r.WasSuccessful && remainder == r.Remainder)
                        break;

                    if (!r.WasSuccessful)
                    {
                        if (remainder == r.Remainder)
                            break;

                        observations.Add(Observe.Error("Unexpected {0}.", elementDescription));

                        remainder = r.Remainder;

                        var panicResult = panicUntil(remainder);

                        while (!panicResult.WasSuccessful && !remainder.AtEnd)
                        {
                            remainder = remainder.Advance();
                            panicResult = panicUntil(remainder);
                        }

                        remainder = panicResult.Remainder;
                    }
                    else
                    {
                        result.Add(r.Value);
                        remainder = r.Remainder;    
                    }

                    r = parser(remainder);
                }

                return Result.Success<IEnumerable<T>>(result, remainder, observations);
            };
        }

        /// <summary>
        /// Parse a stream of elements. If any element is partially parsed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <remarks>Implemented imperatively to decrease stack usage.</remarks>
        public static Parser<IEnumerable<T>> XMany<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Many().Then(m => parser.Once().XOr(Return(m)));
        }

        /// <summary>
        /// TryParse a stream of elements with at least one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Once().Then(t1 => parser.Many().Select(ts => t1.Concat(ts)));
        }

        /// <summary>
        /// TryParse a stream of elements with at least one item. Except the first
        /// item, all other items will be matched with the <code>XMany</code> operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> XAtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Once().Then(t1 => parser.XMany().Select(ts => t1.Concat(ts)));
        }

        /// <summary>
        /// Parse end-of-input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<T> End<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i => parser(i).IfSuccess(s =>
                s.Remainder.AtEnd 
                    ? s
                    : Result.Failure<T>(s.Remainder,
                        Observe.Error(string.Format("unexpected '{0}'", s.Remainder.Current),
                                      "end of input")));
        }

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (convert == null) throw new ArgumentNullException("convert");

            return parser.Then(t => Return(convert(t)));
        }

        /// <summary>
        /// Parse the token, embedded in any amount of whitespace characters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<T> Token<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

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
        public static Parser<T> Ref<T>(Func<Parser<T>> reference)
        {
            if (reference == null) throw new ArgumentNullException("reference");

            Parser<T> p = null;

            return i =>
                       {
                           if (p == null)
                               p = reference();

                           if (i.Memos.ContainsKey(p))
                               throw new ParseException(i.Memos[p].ToString());

                           i.Memos[p] = Result.Failure<T>(i,
                               Observe.Error("Left recursion in the grammar."));

                           var result = p(i);
                           i.Memos[p] = result;
                           return result;
                       };
        }

        /// <summary>
        /// Convert a stream of characters to a string.
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static Parser<string> Text(this Parser<IEnumerable<char>> characters)
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
        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i =>
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    return second(i).IfFailure(sf => Result.Failure<T>(
                        fr.Remainder,
                        fr.Observations.Union(sf.Observations)));
                }
                
                if (fr.Remainder == i)
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        /// <summary>
        /// Names part of the grammar for help with error messages.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Parser<T> Named<T>(this Parser<T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (name == null) throw new ArgumentNullException("name");

            return i => parser(i).IfFailure(f => f.Remainder == i ?
                Result.Failure<T>(f.Remainder, f.Observations.Concat(new[]{Observe.Error(string.Format("Rule named '{0}' failed.", name))})) :
                f);
        }

        /// <summary>
        /// Parse first, if it succeeds, return first, otherwise try second.
        /// Assumes that the first parsed character will determine the parser chosen (see Try).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<T> XOr<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i => {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    if (fr.Remainder != i)
                        return fr;

                    return second(i).IfFailure(sf => Result.Failure<T>(
                        fr.Remainder,
                        fr.Observations.Concat(sf.Observations)));
                }

                if (fr.Remainder == i)
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        /// <summary>
        /// Parse a stream of elements containing only one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Select(r => (IEnumerable<T>)new[] { r });
        }

        /// <summary>
        /// Concatenate two streams of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return first.Then(f => second.Select(s => f.Concat(s)));
        }

        /// <summary>
        /// Succeed immediately and return value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }

        /// <summary>
        /// Version of Return with simpler inline syntax.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Parser<U> Return<T, U>(this Parser<T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            return parser.Select(t => value);
        }

        /// <summary>
        /// Attempt parsing only if the <paramref name="except"/> parser fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="except"></param>
        /// <returns></returns>
        public static Parser<T> Except<T, U>(this Parser<T> parser, Parser<U> except)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (except == null) throw new ArgumentNullException("except");

            // Could be more like: except.Then(s => s.Fail("..")).XOr(parser)
            return i =>
                {
                    var r = except(i);
                    if (r.WasSuccessful)
                        return Result.Failure<T>(i, 
                            Observe.Error("Excepted parser succeeded.", "other than the excepted input" ));
                    return parser(i);
                };
        }

        /// <summary>
        /// Parse a sequence of items until a terminator is reached.
        /// Returns the sequence, discarding the terminator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parser"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<T>> Until<T, U>(this Parser<T> parser, Parser<U> until)
        {
            return parser.Except(until).Many().Then(r => until.Return(r));
        }

        /// <summary>
        /// Succeed if the parsed value matches predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return i => parser(i).IfSuccess(s =>
                predicate(s.Value) ? s : Result.Failure<T>(i,
                    Observe.Error(string.Format("Unexpected {0}.", s.Value))));
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
        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (selector == null) throw new ArgumentNullException("selector");
            if (projector == null) throw new ArgumentNullException("projector");

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        /// <summary>
        /// Chain a left-associative operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOp"></typeparam>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static Parser<T> ChainOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply));
        }

        static Parser<T> ChainOperatorRest<T, TOp>(
            T firstOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return op.Then(opvalue =>
                    operand.Then(operandValue =>
                        ChainOperatorRest(apply(opvalue, firstOperand, operandValue), op, operand, apply)))
                .Or(Return(firstOperand));
        }

        /// <summary>
        /// Chain a right-associative operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOp"></typeparam>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static Parser<T> ChainRightOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply));
        }

        static Parser<T> ChainRightOperatorRest<T, TOp>(
            T lastOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return op.Then(opvalue =>
                    operand.Then(operandValue =>
                        ChainRightOperatorRest(operandValue, op, operand, apply)).Then(r =>
                            Return(apply(opvalue, lastOperand, r))))
                .Or(Return(lastOperand));
        }

        /// <summary>
        /// Parse a number.
        /// </summary>
        public static readonly Parser<string> Number = Numeric.AtLeastOnce().Text();

        static readonly Parser<string> DecimalWithoutLeadingDigits =
            from nothing in Return("") // dummy so that CultureInfo.CurrentCulture is evaluated later
            from dot in String(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Text()
            from fraction in Number
            select dot + fraction;

        static readonly Parser<string> DecimalWithLeadingDigits =
            Number.Then(n => DecimalWithoutLeadingDigits.XOr(Return("")).Select(f => n + f));

        /// <summary>
        /// Parse a decimal number.
        /// </summary>
 	    public static readonly Parser<string> Decimal = DecimalWithLeadingDigits.XOr(DecimalWithoutLeadingDigits);
    }
}
