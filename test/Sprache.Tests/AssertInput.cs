using System;

namespace Sprache.Tests
{
    static class AssertInput
    {
        public static IInput AdvanceMany(this IInput input, int count)
        {
            for (int i = 0; i < count; i++)
            {
                input = input.Advance();
            }

            return input;
        }

        public static IInput AdvanceAssert(this IInput input, Action<IInput, IInput> assertion)
        {
            var result = input.Advance();
            assertion(input, result);
            return result;
        }

        public static IInput AdvanceManyAssert(this Input input, int count, Action<IInput, IInput> assertion)
        {
            var result = input.AdvanceMany(count);
            assertion(input, result);
            return result;
        }
    }
}
