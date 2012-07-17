using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Sprache.Tests
{
    static class AssertInput
    {
        public static Input AdvanceMany(this Input input, int count)
        {
            for (int i = 0; i < count; i++)
            {
                input = input.Advance();
            }

            return input;
        }

        public static Input AdvanceAssert(this Input input, Action<Input, Input> assertion)
        {
            var result = input.Advance();
            assertion(input, result);
            return result;
        }

        public static Input AdvanceManyAssert(this Input input, int count, Action<Input, Input> assertion)
        {
            var result = input.AdvanceMany(count);
            assertion(input, result);
            return result;
        }
    }
}
