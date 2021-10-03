using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache.Union
{
    public static class UnionResult
    {
        public static UnionResult<T> Success<T>(T value, IInput remainder, string parserName, int startIndex, int endIndex)
        {
            return new UnionResult<T>()
            {
                Values = new List<UnionResultValue<T>> { new UnionResultValue<T>() { Value = value, Reminder = remainder, ParserName = parserName, StartIndex = startIndex, EndIndex = endIndex, WasSuccessful = true } }
            };
        }

        public static UnionResult<T> Failure<T>()
        {
            return new UnionResult<T>()
            {
                Values = new List<UnionResultValue<T>> { new UnionResultValue<T>() { WasSuccessful = false } }
            };
        }

        public static UnionResult<T> Success<T>(IEnumerable<UnionResultValue<T>> values)
        {
            return new UnionResult<T>()
            {
                Values = values.ToList()
            };
        }

        public static UnionResult<T> Failure<T>(string errorMessage)
        {
            return new UnionResult<T>()
            {
                Values = new List<UnionResultValue<T>> { new UnionResultValue<T>() { WasSuccessful = false } },
                ErrorMessage = errorMessage
            };
        }
    }
}
