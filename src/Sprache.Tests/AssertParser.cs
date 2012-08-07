using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Sprache.Tests
{
    static class AssertParser
    {
        public static void SucceedsWithOne<T>(Parser<IEnumerable<T>> parser, string input, T expectedResult)
        {
            SucceedsWith(parser, input, t =>
            {
                Assert.AreEqual(1, t.Count());
                Assert.AreEqual(expectedResult, t.Single());
            });
        }

        public static void SucceedsWithMany<T>(Parser<IEnumerable<T>> parser, string input, IEnumerable<T> expectedResult)
        {
            SucceedsWith(parser, input, t => Assert.IsTrue(t.SequenceEqual(expectedResult)));
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
                    Assert.Fail("Parsing of \"{0}\" failed unexpectedly at position {1}: {2}", input, f.FailedInput.Position, f.Message);
                    return f;
                })
                .IfSuccess(s =>
                {
                    resultAssertion(s.Result);
                    return s;
                });
        }

        public static void Fails<T>(Parser<T> parser, string input)
        {
            FailsWith(parser, input, f => { });
        }

        public static void FailsAt<T>(Parser<T> parser, string input, int position)
        {
            FailsWith(parser, input, f => Assert.AreEqual(position, f.FailedInput.Position));
        }

        public static void FailsWith<T>(Parser<T> parser, string input, Action<IFailure<T>> resultAssertion)
        {
            parser.TryParse(input)
                .IfSuccess(s =>
                {
                    Assert.Fail("Expected failure but succeeded with {0}.", s.Result);
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
