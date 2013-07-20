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
        /// Gets a value indicating whether parsing was successful.
        /// </summary>
        bool WasSuccessful { get; }

        /// <summary>
        /// Gets any observations as a result of parsing.
        /// </summary>
        IEnumerable<ResultObservation> Observations { get; }
            
        /// <summary>
        /// Gets the remainder of the input.
        /// </summary>
        Input Remainder { get; }
    }
}
