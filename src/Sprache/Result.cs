using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    public static class Result
    {
        public static Result<T> Success<T>(T value, Input remainder)
        {
            return new Result<T>(value, remainder);
        }

        public static Result<T> Failure<T>(Input remainder, string message, IEnumerable<string> expectations)
        {
            return new Result<T>(remainder, message, expectations);
        }
    }

    public class Result<T> : IResult<T>
    {
        private readonly T _Value;
        private readonly Input _Remainder;
        private readonly bool _WasSuccessful;
        private readonly string _Message;
        private readonly IEnumerable<string> _Expectations;

        public Result(T value, Input remainder)
        {
            _Value = value;
            _Remainder = remainder;
            _WasSuccessful = true;
            _Message = null;
            _Expectations = Enumerable.Empty<string>();
        }

        public Result(Input remainder, string message, IEnumerable<string> expectations)
        {
            _Value = default(T);
            _Remainder = remainder;
            _WasSuccessful = false;
            _Message = message;
            _Expectations = expectations;
        }

        public T Value
        {
            get
            {
                if (!WasSuccessful)
                    throw new InvalidOperationException("No value can be computed.");

                return _Value;
            }
        }
        public bool WasSuccessful { get { return _WasSuccessful; } }

        public string Message { get { return _Message; } }
        public IEnumerable<string> Expectations { get { return _Expectations; } }

        public Input Remainder { get { return _Remainder; } }

        public override string ToString()
        {
            if(WasSuccessful)
                return string.Format("Successful parsing of {0}.", Value);

            var expMsg = "";

            if (Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);

            return string.Format("Parsing failure: {0};{1} ({2}).", Message, expMsg, Remainder);
        }

    }
}
