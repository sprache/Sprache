using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache.Tests
{
    class PosAwareStr : IPositionAware<PosAwareStr>
    {
        public PosAwareStr SetPos(Position startPos, int length)
        {
            Pos = startPos;
            Length = length;
            return this;
        }

        public Position Pos
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }
}
