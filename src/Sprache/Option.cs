using System;

namespace Sprache
{
    /// <summary>
    /// Represents an optional result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public interface IOption<out T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is defined.
        /// </summary>
        bool IsDefined { get; }

        /// <summary>
        /// Gets the matched result or a default value.
        /// </summary>
        /// <returns></returns>
        T GetOrDefault();

        /// <summary>
        /// Gets the matched result.
        /// </summary>
        T Get();
    }

    /// <summary>
    /// Represents an optional result.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public abstract class AbstractOption<T> : IOption<T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is defined.
        /// </summary>
        public bool IsDefined
        {
            get { return !IsEmpty; }
        }

        /// <summary>
        /// Gets the matched result or a default value.
        /// </summary>
        /// <returns></returns>
        public T GetOrDefault()
        {
            return GetOrElse(default(T));
        }

        /// <summary>
        /// Gets the or else.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T GetOrElse(T defaultValue)
        {
            if (IsEmpty)
                return defaultValue;
            return Get();
        }

        /// <summary>
        /// Gets the matched result.
        /// </summary>
        public abstract T Get();
    }

    internal sealed class Some<T> : AbstractOption<T>
    {
        private readonly T _value;

        public Some(T value)
        {
            _value = value;
        }

        public override bool IsEmpty
        {
            get { return false; }
        }

        public override T Get()
        {
            return _value;
        }
    }

    internal sealed class None<T> : AbstractOption<T>
    {
        public override bool IsEmpty
        {
            get { return true; }
        }

        public override T Get()
        {
            throw new InvalidOperationException("Cannot get value from None.");
        }
    }
}
