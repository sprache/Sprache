using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using Xunit;

namespace Sprache.Tests
{
    static class AssertParser
    {
        public static void SucceedsWithOne<T>(Parser<IEnumerable<T>> parser, string input, T expectedResult)
        {
            SucceedsWith(parser, input, t =>
            {
                Assert.Single(t);
                Assert.Equal(expectedResult, t.Single());
            });
        }

        public static void SucceedsWithMany<T>(Parser<IEnumerable<T>> parser, string input, IEnumerable<T> expectedResult)
        {
            SucceedsWith(parser, input, t => Assert.True(t.SequenceEqual(expectedResult)));
        }

        public static void SucceedsWithAll(Parser<IEnumerable<char>> parser, string input)
        {
            SucceedsWithMany(parser, input, input.ToCharArray());
        }

        public static void SucceedsWith<T>(Parser<T> parser, string input, Action<T> resultAssertion)
        {
            parser.TryParse(input)
                .IfFailure(f =>
                {
                    Assert.True(false, $"Parsing of \"input\" failed unexpectedly. f");
                    return f;
                })
                .IfSuccess(s =>
                {
                    resultAssertion(s.Value);
                    return s;
                });
        }

        public static void Fails<T>(Parser<T> parser, string input)
        {
            FailsWith(parser, input, f => { });
        }

        public static void FailsAt<T>(Parser<T> parser, string input, int position)
        {
            FailsWith(parser, input, f => Assert.Equal(position, f.Remainder.Position));
        }

        public static void FailsWith<T>(Parser<T> parser, string input, Action<IResult<T>> resultAssertion)
        {
            parser.TryParse(input)
                .IfSuccess(s =>
                {
                    Assert.True(false, $"Expected failure but succeeded with {s.Value}.");
                    return s;
                })
                .IfFailure(f =>
                {
                    resultAssertion(f);
                    return f;
                });
        }
    }
}
