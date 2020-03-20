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

        /// <summary>
        /// Maps a function over the value or else returns an empty option.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <typeparam name="U">The output type.</typeparam>
        /// <param name="option">The option containing the value to apply <paramref name="map" /> to.</param>
        /// <param name="map">The function to apply to the value of <paramref name="option" />.</param>
        /// <returns>An options result containing the result if there was an input value.</returns>
        public static IOption<U> Select<T, U>(this IOption<T> option, Func<T,U> map)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            return option.IsDefined ? (IOption<U>) new Some<U>(map(option.Get())) : new None<U>();
        }

        /// <summary>
        /// Binds the value to a function with optional result and flattens the result to a single optional.
        /// A result projection is applied aftherwards.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <typeparam name="U">The output type of <paramref name="bind" />.</typeparam>
        /// <typeparam name="V">The final output type.</typeparam>
        /// <param name="option">The option containing the value to bind to.</param>
        /// <param name="bind">The function that receives the input values and returns an optional value.</param>
        /// <param name="project">The function that is projects the result of <paramref name="bind" />.</param>
        /// <returns>An option result containing the result if there were was an input value and bind result.</returns>
        public static IOption<V> SelectMany<T,U,V>(this IOption<T> option, Func<T,IOption<U>> bind, Func<T,U,V> project)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            if (option.IsEmpty) return new None<V>();

            var t = option.Get();
            return bind(t).Select(u => project(t,u));
        }

        /// <summary>
        /// Binds the value to a function with optional result and flattens the result to a single optional.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <typeparam name="U">The output type.</typeparam>
        /// <param name="option">The option containing the value to bind to.</param>
        /// <param name="bind">The function that receives the input values and returns an optional value.</param>
        /// <returns>An option result containing the result if there were was an input value and bind result.</returns>
        public static IOption<U> SelectMany<T,U>(this IOption<T> option, Func<T,IOption<U>> bind) => option.SelectMany(bind, (_,x) => x);
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
