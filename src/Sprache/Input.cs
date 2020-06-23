using System;
using System.Collections.Generic;

namespace Sprache
{
    /// <summary>
    /// Represents an input for parsing.
    /// </summary>
    public class Input : IInput
    {
        /// <summary>
        /// Gets the list of memos assigned to the <see cref="Input" /> instance.
        /// </summary>
        public IDictionary<object, object> Memos { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Input" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            Source = source;
            Position = position;
            Line = line;
            Column = column;

            Memos = new Dictionary<object, object>();
        }

        /// <summary>
        /// Advances the input.
        /// </summary>
        /// <returns>A new <see cref="IInput" /> that is advanced.</returns>
        /// <exception cref="System.InvalidOperationException">The input is already at the end of the source.</exception>
        public IInput Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new Input(Source, Position + 1, Current == '\n' ? Line + 1 : Line, Current == '\n' ? 1 : Column + 1);
        }

        /// <summary>
        /// Gets the whole source.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the current <see cref="System.Char" />.
        /// </summary>
        public char Current => Source[Position];

        /// <summary>
        /// Gets a value indicating whether the end of the source is reached.
        /// </summary>
        public bool AtEnd => Position == Source.Length;

        /// <summary>
        /// Gets the current positon.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the current column.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", Line, Column);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Input" />.
        /// </returns>
        public override int GetHashCode()
        {
#if NETSTANDARD2_1
            return HashCode.Combine(Source, Position);
#else
            unchecked
            {
                return ((Source != null ? Source.GetHashCode() : 0) * 397) ^ Position;
            }
#endif
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="Input" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="Input" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            return Equals(obj as IInput);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Input" /> is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IInput other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Source, other.Source) && Position == other.Position;
        }

        /// <summary>
        /// Indicates whether the left <see cref="Input" /> is equal to the right <see cref="Input" />.
        /// </summary>
        /// <param name="left">The left <see cref="Input" />.</param>
        /// <param name="right">The right <see cref="Input" />.</param>
        /// <returns>true if both objects are equal.</returns>
        public static bool operator ==(Input left, Input right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the left <see cref="Input" /> is not equal to the right <see cref="Input" />.
        /// </summary>
        /// <param name="left">The left <see cref="Input" />.</param>
        /// <param name="right">The right <see cref="Input" />.</param>
        /// <returns>true if the objects are not equal.</returns>
        public static bool operator !=(Input left, Input right)
        {
            return !Equals(left, right);
        }
    }
}
