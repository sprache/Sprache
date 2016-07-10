using System.Collections.Generic;

namespace Sprache
{
    /// <summary>
    /// Represents a parsing result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public interface IResult<out T>
    {
        /// <summary>
        /// Gets the resulting value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets a value indicating whether wether parsing was successful.
        /// </summary>
        bool WasSuccessful { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the parser expectations in case of error.
        /// </summary>
        IEnumerable<string> Expectations { get; }

        /// <summary>
        /// Gets the remainder of the input.
        /// </summary>
        IInput Remainder { get; }
    }
}
