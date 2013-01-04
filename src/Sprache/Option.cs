using System;

namespace Sprache
{
    // This interface can't be made covariance. See GetOrElse().
    public interface Option<T>
    {
        bool IsEmpty { get; }
        bool IsDefined { get; }

        // To support covariance, the signature needs to be
        // U GetOrElse<U>(U defaultValue) where T : U
        // And C# doesn't support the super type constraint.
        T GetOrElse(T defaultValue);

        T Get();
    }

    public abstract class AbstractOption<T> : Option<T>
    {
        public abstract bool IsEmpty { get; }

        public bool IsDefined
        {
            get { return !IsEmpty; }
        }

        public T GetOrElse(T defaultValue)
        {
            if (IsEmpty) return defaultValue;
            else return Get();
        }

        public abstract T Get();
    }

    public sealed class Some<T> : AbstractOption<T>
    {
        private T value;

        public Some(T value)
        {
            this.value = value;
        }

        public override bool IsEmpty
        {
            get { return false; }
        }

        public override T Get()
        {
            return value;
        }
    }

    public sealed class None<T> : AbstractOption<T>
    {
        public None() { }

        public override bool IsEmpty
        {
            get { return true; }
        }

        public override T Get()
        {
            throw new GettingValueFromNoneException();
        }
    }

    public class GettingValueFromNoneException : Exception { }
}
