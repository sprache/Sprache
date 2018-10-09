namespace Sprache
{
    /// <summary>
    /// Represents a text span of the matched result.
    /// </summary>
    /// <typeparam name="T">Type of the matched result.</typeparam>
    public interface ITextSpan<T>
    {
        /// <summary>
        /// Gets the resulting value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the starting <see cref="Position"/>.
        /// </summary>
        Position Start { get; }

        /// <summary>
        /// Gets the ending <see cref="Position"/>.
        /// </summary>
        Position End { get; }

        /// <summary>
        /// Gets the length of the text span.
        /// </summary>
        int Length { get; }
    }
}