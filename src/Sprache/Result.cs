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
        private readonly IInput _remainder;
        private readonly bool _wasSuccessful;
        private readonly string _message;
        private readonly IEnumerable<string> _expectations;

        public Result(T value, IInput remainder)
        {
            _value = value;
            _remainder = remainder;
            _wasSuccessful = true;
            _message = null;
            _expectations = Enumerable.Empty<string>();
        }

        public Result(IInput remainder, string message, IEnumerable<string> expectations)
        {
            _value = default(T);
            _remainder = remainder;
            _wasSuccessful = false;
            _message = message;
            _expectations = expectations;
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

        public bool WasSuccessful { get { return _wasSuccessful; } }

        public string Message { get { return _message; } }

        public IEnumerable<string> Expectations { get { return _expectations; } }

        public IInput Remainder { get { return _remainder; } }

        public override string ToString()
        {
            if (WasSuccessful)
                return string.Format("Successful parsing of {0}.", Value);

            var expMsg = "";

            if (Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);

            var recentlyConsumed = CalculateRecentlyConsumed();

            return string.Format("Parsing failure: {0};{1} ({2}); recently consumed: {3}", Message, expMsg, Remainder, recentlyConsumed);
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
