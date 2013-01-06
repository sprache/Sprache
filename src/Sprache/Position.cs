using System;

namespace Sprache
{
    public class Position : IEquatable<Position>
    {
        public Position(int pos, int line, int column)
        {
            this.Pos = pos;
            this.Line = line;
            this.Column = column;
        }

        public int Pos
        {
            get;
            private set;
        }

        public int Line
        {
            get;
            private set;
        }

        public int Column
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Pos == other.Pos
                && Line == other.Line
                && Column == other.Column;
        }

        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            var h = 31;
            h = h * 13 + Pos;
            h = h * 13 + Line;
            h = h * 13 + Column;
            return h;
        }
    }
}
