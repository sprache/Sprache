using System;
using System.Collections.Generic;
using System.Text;

namespace Sprache.Union
{
    public interface IState
    {
        Stack<string> Path { get; set; }

        bool HasValue(int startIndex, string parserName);

        T GetValue<T>(int startIndex, string parserName) where T : class;

        void Store<U>(int startIndex, string parserName, U result);
    }
}
