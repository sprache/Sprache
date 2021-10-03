using System;

namespace Sprache.Union
{
    public interface IUnionParser<T>
    {
        string Name { get; set; }

        Func<IInput, IState, UnionResult<T>> Parse { get; set; }
    }
}
