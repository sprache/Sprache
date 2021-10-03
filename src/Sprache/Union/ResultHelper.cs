using System;

namespace Sprache.Union
{
    internal static class ResultHelper
    {
        public static UnionResult<U> IfSuccess<T, U>(this UnionResult<T> result, Func<UnionResult<T>, UnionResult<U>> next)
        {
            if(result == null) throw new ArgumentNullException(nameof(result));

            if (result.WasSuccessful)
                return next(result);

            return UnionResult.Failure<U>();
        }

        public static UnionResult<T> IfFailure<T>(this UnionResult<T> result, Func<UnionResult<T>, UnionResult<T>> next)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return result.WasSuccessful 
                ? result 
                : next(result);
        }
    }
}