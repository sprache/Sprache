using System;
using System.Collections.Generic;

namespace Sprache
{
    public class Input : IEquatable<Input>
    {
        private readonly string _source;
        private readonly Position _position;

        internal IDictionary<object, object> Memos = new Dictionary<object, object>();

        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            _source = source;
            _position = new Position(position, line, column);
        }

        public Input Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new Input(
                _source,
                _position.Pos + 1,
                Current == '\n' ? _position.Line + 1 : _position.Line,
                Current == '\n' ? 1 : _position.Column + 1);
        }

        /// <summary>
        /// Gets the whole source.
        /// </summary>
        public string Source { get { return _source; } }

        /// <summary>
        /// Gets the current <see cref="System.Char" />.
        /// </summary>
        public char Current { get { return _source[_position.Pos]; } }

        /// <summary>
        /// Gets a value indicating whether the end of the source is reached.
        /// </summary>
        public bool AtEnd { get { return _position.Pos == _source.Length; } }

        /// <summary>
        /// Gets the current positon.
        /// </summary>
        public int Position { get { return _position.Pos; } }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int Line { get { return _position.Line; } }

        /// <summary>
        /// Gets the current column.
        /// </summary>
        public int Column { get { return _position.Column; } }

        internal Position Pos { get { return _position; } }

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _position.Line, _position.Column);
        }
		
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Input" />.
        /// </returns>
        public override int GetHashCode()
		{
			unchecked
			{
				return ((_source != null ? _source.GetHashCode() : 0) * 397) ^ _position.Pos;
			}
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
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((Input) obj);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Input" /> is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
	    public bool Equals(Input other)
	    {
		    if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    return string.Equals(_source, other._source) && _position == other._position;
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
