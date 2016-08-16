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
    /// Extensions for <see cref="IOption&lt;T&gt;"/>.
    /// </summary>
    public static class OptionExtensions
    {
        /// <summary>
        /// Gets the value or else returns a default value.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="option"></param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetOrElse<T>(this IOption<T> option, T defaultValue)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            return option.IsEmpty ? defaultValue : option.Get();
        }
    }

    internal abstract class AbstractOption<T> : IOption<T>
    {
        public abstract bool IsEmpty { get; }

        public bool IsDefined
        {
            get { return !IsEmpty; }
        }

        public T GetOrDefault()
        {
            return IsEmpty ? default(T) : Get();
        }

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
