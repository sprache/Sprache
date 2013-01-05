using System;
using System.Collections.Generic;

namespace Sprache
{
    public class Input : IEquatable<Input>
    {
        public string Source { get; set; }
        readonly string _source;
        readonly Position _position;

        internal IDictionary<object, object> Memos = new Dictionary<object, object>();

        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            Source = source;

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

        public char Current { get { return _source[_position.Pos]; } }

        public bool AtEnd { get { return _position.Pos == _source.Length; } }

        public int Position { get { return _position.Pos; } }

        public int Line { get { return _position.Line; } }

        public int Column { get { return _position.Column; } }

        internal Position Pos { get { return _position; } }

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _position.Line, _position.Column);
        }
		
        public override int GetHashCode()
		{
			unchecked
			{
				return ((_source != null ? _source.GetHashCode() : 0) * 397) ^ _position.Pos;
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
