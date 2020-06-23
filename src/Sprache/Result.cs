using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    /// <summary>
    /// Contains helper functions to create <see cref="IResult&lt;T&gt;"/> instances.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates a success result.
        /// </summary>
        /// <typeparam name="T">The type of the result (value).</typeparam>
        /// <param name="value">The sucessfully parsed value.</param>
        /// <param name="remainder">The remainder of the input.</param>
        /// <returns>The new <see cref="IResult&lt;T&gt;"/>.</returns>
        public static IResult<T> Success<T>(T value, IInput remainder)
        {
            return new Result<T>(value, remainder);
        }

        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="remainder">The remainder of the input.</param>
        /// <param name="message">The error message.</param>
        /// <param name="expectations">The parser expectations.</param>
        /// <returns>The new <see cref="IResult&lt;T&gt;"/>.</returns>
        public static IResult<T> Failure<T>(IInput remainder, string message, IEnumerable<string> expectations)
        {
            return new Result<T>(remainder, message, expectations);
        }
    }

    internal class Result<T> : IResult<T>
    {
        private readonly T _value;

        public Result(T value, IInput remainder)
        {
            _value = value;
            Remainder = remainder;
            WasSuccessful = true;
            Message = null;
            Expectations = Enumerable.Empty<string>();
        }

        public Result(IInput remainder, string message, IEnumerable<string> expectations)
        {
            _value = default;
            Remainder = remainder;
            WasSuccessful = false;
            Message = message;
            Expectations = expectations;
        }

        public T Value
        {
            get
            {
                if (!WasSuccessful)
                    throw new InvalidOperationException("No value can be computed.");

                return _value;
            }
        }

        public bool WasSuccessful { get; }

        public string Message { get; }

        public IEnumerable<string> Expectations { get; }

        public IInput Remainder { get; }

        public override string ToString()
        {
            if (WasSuccessful)
                return $"Successful parsing of {Value}.";

            var expMsg = "";

            if (Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);

            var recentlyConsumed = CalculateRecentlyConsumed();

            return $"Parsing failure: {Message};{expMsg} ({Remainder}); recently consumed: {recentlyConsumed}";
        }

        private string CalculateRecentlyConsumed()
        {
            const int windowSize = 10;

            var totalConsumedChars = Remainder.Position;
            var windowStart = totalConsumedChars - windowSize;
            windowStart = windowStart < 0 ? 0 : windowStart;

            var numberOfRecentlyConsumedChars = totalConsumedChars - windowStart;

            return Remainder.Source.Substring(windowStart, numberOfRecentlyConsumedChars);
        }
    }
}
