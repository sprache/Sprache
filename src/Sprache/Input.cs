using System;
using System.Collections.Generic;

namespace Sprache
{
    public class Input : IEquatable<Input>
    {
        public string Source { get; set; }
        readonly string _source;
        readonly int _position;
        private readonly int _line;
        private readonly int _column;

        internal IDictionary<object, object> Memos = new Dictionary<object, object>();

        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            Source = source;

            _source = source;
            _position = position;
            this._line = line;
            this._column = column;
        }

        public Input Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new Input(_source, _position + 1, Current == '\n' ? _line + 1 : _line, Current == '\n' ? 1 : _column + 1);
        }

        public char Current { get { return _source[_position]; } }

        public bool AtEnd { get { return _position == _source.Length; } }

        public int Position { get { return _position; } }

        public int Line { get { return _line; } }

        public int Column { get { return _column; } }

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _line, _column);
        }
		public override int GetHashCode()
		{
			unchecked
			{
				return ((_source != null ? _source.GetHashCode() : 0) * 397) ^ _position;
			}
		}
		public override bool Equals(object obj)
        {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((Input) obj);
        }

	    public bool Equals(Input other)
	    {
		    if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    return string.Equals(_source, other._source) && _position == other._position;
	    }
	    public static bool operator ==(Input left, Input right)
	    {
		    return Equals(left, right);
	    }
	    public static bool operator !=(Input left, Input right)
	    {
		    return !Equals(left, right);
	    }
    }
}
