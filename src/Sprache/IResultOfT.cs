using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache
{
    public interface IResult<out T>
    {
        T Value { get; }
        bool WasSuccessful { get; }

        string Message { get; }
        IEnumerable<string> Expectations { get; }

        Input Remainder { get; }
    }
}
