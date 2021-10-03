using System;
using System.Collections.Generic;
using System.Text;

namespace Sprache.Union
{
    public class State : IState
    {
        private Dictionary<string, object> _cache = new Dictionary<string, object>();

        public Stack<string> Path { get; set; } = new Stack<string>();

        public T GetValue<T>(int startIndex, string parserName) where T : class
        {
            return _cache[startIndex + "_" + parserName] as T;
        }

        public bool HasValue(int startIndex, string parserName)
        {
            return _cache.ContainsKey(startIndex + "_" + parserName);
        }

        public void Store<U>(int startIndex, string parserName, U result)
        {
            _cache[startIndex + "_" + parserName] = result;
        }
    }

}
