using System;

namespace Sprache
{
    public class Parser<T>
    {
        public Func<IInput, IResult<T>> Func { get; }

        public Parser(Func<IInput, IResult<T>> func)
        {
            Func = func;
        }

        public IResult<T> TryParse(IInput input) => Func(input);

        public IResult<T> TryParse(string input) => TryParse(new Input(input));

        public T Parse(string input)
        {
            var result = TryParse(input);

            if (result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString(), Position.FromInput(result.Remainder));
        }

        public static implicit operator Func<IInput, IResult<T>>(Parser<T> parser) => parser.Func;

        public static implicit operator Parser<T>(Func<IInput, IResult<T>> func) => new Parser<T>(func);

        public static Parser<T> operator |(Parser<T> a, Parser<T> b) => a.Or(b);
    }
}
