using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Sprache.Tests
{
    [TestFixture]
    public class ParseTests
    {
        [Test]
        public void Parser_OfChar_AcceptsThatChar()
        {
            AssertParser.SucceedsWithOne(Parse.Char('a').Once(), "a", 'a');
        }

        [Test]
        public void Parser_OfChar_AcceptsOnlyOneChar()
        {
            AssertParser.SucceedsWithOne(Parse.Char('a').Once(), "aaa", 'a');
        }

        [Test]
        public void Parser_OfChar_DoesNotAcceptNonMatchingChar()
        {
            AssertParser.FailsAt(Parse.Char('a').Once(), "b", 0);
        }

        [Test]
        public void Parser_OfChar_DoesNotAcceptEmptyInput()
        {
            AssertParser.Fails(Parse.Char('a').Once(), "");
        }

        [Test]
        public void Parser_OfManyChars_AcceptsEmptyInput()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "");
        }

        [Test]
        public void Parser_OfManyChars_AcceptsManyChars()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "aaa");
        }

        [Test]
        public void Parser_OfAtLeastOneChar_DoesNotAcceptEmptyInput()
        {
            AssertParser.Fails(Parse.Char('a').AtLeastOnce(), "");
        }

        [Test]
        public void Parser_OfAtLeastOneChar_AcceptsOneChar()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').AtLeastOnce(), "a");
        }

        [Test]
        public void Parser_OfAtLeastOneChar_AcceptsManyChars()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').AtLeastOnce(), "aaa");
        }

        [Test]
        public void ConcatenatingParsers_ConcatenatesResults()
        {
            var p = Parse.Char('a').Once().Then(a =>
                Parse.Char('b').Once().Select(b => a.Concat(b)));
            AssertParser.SucceedsWithAll(p, "ab"); 
        }

        [Test]
        public void ReturningValue_DoesNotAdvanceInput()
        {
            var p = Parse.Return(1);
            AssertParser.SucceedsWith(p, "abc", n => Assert.AreEqual(1, n));
        }

        [Test]
        public void ReturningValue_ReturnsValueAsResult()
        {
            var p = Parse.Return(1);
            var r = (Result<int>)p.TryParse("abc");
            Assert.AreEqual(0, r.Remainder.Position);
        }

        [Test]
        public void CanSpecifyParsersUsingQueryComprehensions()
        {
            var p = from a in Parse.Char('a').Once()
                    from bs in Parse.Char('b').Many()
                    from cs in Parse.Char('c').AtLeastOnce()
                    select a.Concat(bs).Concat(cs);

            AssertParser.SucceedsWithAll(p, "abbbc");
        }

        [Test]
        public void WhenFirstOptionSucceedsButConsumesNothing_SecondOptionTried()
        {
            var p = Parse.Char('a').Many().XOr(Parse.Char('b').Many());
            AssertParser.SucceedsWithAll(p, "bbb");
        }

        [Test]
        public void WithXOr_WhenFirstOptionFailsAndConsumesInput_SecondOptionNotTried()
        {
            var first = Parse.Char('a').Once().Concat(Parse.Char('b').Once());
            var second = Parse.Char('a').Once();
            var p = first.XOr(second);
            AssertParser.FailsAt(p, "a", 1);
        }

        [Test]
        public void WithOr_WhenFirstOptionFailsAndConsumesInput_SecondOptionTried()
        {
            var first = Parse.Char('a').Once().Concat(Parse.Char('b').Once());
            var second = Parse.Char('a').Once();
            var p = first.Or(second);
            AssertParser.SucceedsWithAll(p, "a");
        }

        [Test]
        public void ParsesString_AsSequenceOfChars()
        {
            var p = Parse.String("abc");
            AssertParser.SucceedsWithAll(p, "abc");
        }

        static readonly Parser<IEnumerable<char>> ASeq =
            (from first in Parse.Ref(() => ASeq)
             from comma in Parse.Char(',')
             from rest in Parse.Char('a').Once()
             select first.Concat(rest))
            .Or(Parse.Char('a').Once());

        [Test, Ignore("Not Implemented")]
        public void CanParseLeftRecursiveGrammar()
        {
            AssertParser.SucceedsWith(ASeq.End(), "a,a,a", r => Assert.AreEqual(new string(r.ToArray()), "aaa"));
        }

        [Test]
        public void DetectsLeftRecursion()
        {
            Assert.Throws<ParseException>(() => ASeq.TryParse("a,a,a"));
        }

        static readonly Parser<IEnumerable<char>> ABSeq =
            (from first in Parse.Ref(() => BASeq)
             from rest in Parse.Char('a').Once()
             select first.Concat(rest))
            .Or(Parse.Char('a').Once());

        static readonly Parser<IEnumerable<char>> BASeq =
            (from first in Parse.Ref(() => ABSeq)
             from rest in Parse.Char('b').Once()
             select first.Concat(rest))
            .Or(Parse.Char('b').Once());

        [Test, Ignore("Not Implemented")]
        public void CanParseMutuallyLeftRecursiveGrammar()
        {
            AssertParser.SucceedsWithAll(ABSeq.End(), "baba");
        }

        [Test]
        public void DetectsMutualLeftRecursion()
        {
            Assert.Throws<ParseException>(() => ABSeq.End().TryParse("baba"));
        }

        [Test]
        public void WithMany_WhenLastElementFails_FailureReportedAtLastElement()
        {
            var ab = from a in Parse.Char('a')
                     from b in Parse.Char('b')
                     select "ab";

            var p = ab.Many().End();

            AssertParser.FailsAt(p, "ababaf", 4);
        }

        [Test]
        public void WithXMany_WhenLastElementFails_FailureReportedAtLastElement()
        {
            var ab = from a in Parse.Char('a')
                     from b in Parse.Char('b')
                     select "ab";

            var p = ab.XMany().End();

            AssertParser.FailsAt(p, "ababaf", 5);
        }

        [Test]
        public void ExceptStopsConsumingInputWhenExclusionParsed()
        {
            var exceptAa = Parse.AnyChar.Except(Parse.String("aa")).Many().Text();
            AssertParser.SucceedsWith(exceptAa, "abcaab", r => Assert.AreEqual("abc", r));
        }

        [Test]
        public void UntilProceedsUntilTheStopConditionIsMetAndReturnsAllButEnd()
        {
            var untilAa = Parse.AnyChar.Until(Parse.String("aa")).Text();
            var r = untilAa.TryParse("abcaab");
            Assert.IsInstanceOf<Result<string>>(r);
            var s = (Result<string>)r;
            Assert.AreEqual("abc", s.Value);
            Assert.AreEqual(5, s.Remainder.Position);
        }

        [Test]
        public void OptionalParserConsumesInputOnSuccessfulMatch()
        {
            var optAbc = Parse.String("abc").Text().Optional();
            var r = optAbc.TryParse("abcd");
            Assert.IsTrue(r.WasSuccessful);
            Assert.AreEqual(3, r.Remainder.Position);
            Assert.IsTrue(r.Value.IsDefined);
            Assert.AreEqual("abc", r.Value.Get());
        }

        [Test]
        public void OptionalParserDoesNotConsumeInputOnFailedMatch()
        {
            var optAbc = Parse.String("abc").Text().Optional();
            var r = optAbc.TryParse("d");
            Assert.IsTrue(r.WasSuccessful);
            Assert.AreEqual(0, r.Remainder.Position);
            Assert.IsTrue(r.Value.IsEmpty);
        }

        [Test]
        public void RegexParserConsumesInputOnSuccessfulMatch()
        {
            var digits = Parse.Regex(@"\d+");
            var r = digits.TryParse("123d");
            Assert.IsTrue(r.WasSuccessful);
            Assert.AreEqual("123", r.Value);
            Assert.AreEqual(3, r.Remainder.Position);
        }

        [Test]
        public void RegexParserDoesNotConsumeInputOnFailedMatch()
        {
            var digits = Parse.Regex(@"\d+");
            var r = digits.TryParse("d123");
            Assert.IsFalse(r.WasSuccessful);
            Assert.AreEqual(0, r.Remainder.Position);
        }

        [Test]
        public void PositionedParser()
        {
            var pos = (from s in Parse.String("winter").Text()
                       select new PosAwareStr { Value = s })
                       .Positioned();
            var r = pos.TryParse("winter");
            Assert.IsTrue(r.WasSuccessful);
            Assert.AreEqual(0, r.Value.Pos.Pos);
            Assert.AreEqual(6, r.Value.Length);
        }

        [Test]
        public void XAtLeastOnceParser_WhenLastElementFails_FailureReportedAtLastElement()
        {
            var ab = Parse.String("ab").Text();
            var p = ab.XAtLeastOnce().End();
            AssertParser.FailsAt(p, "ababaf", 5);
        }

        [Test]
        public void XAtLeastOnceParser_WhenFirstElementFails_FailureReportedAtFirstElement()
        {
            var ab = Parse.String("ab").Text();
            var p = ab.XAtLeastOnce().End();
            AssertParser.FailsAt(p, "d", 0);
        }

        [Test]
        public void NotParserConsumesNoInputOnFailure()
        {
            var notAb = Parse.String("ab").Text().Not();
            AssertParser.FailsAt(notAb, "abc", 0);
        }

        [Test]
        public void NotParserConsumesNoInputOnSuccess()
        {
            var notAb = Parse.String("ab").Text().Not();
            var r = notAb.TryParse("d");
            Assert.IsTrue(r.WasSuccessful);
            Assert.AreEqual(0, r.Remainder.Position);
        }

        [Test]
        public void IgnoreCaseParser()
        {
            var ab = Parse.IgnoreCase("ab").Text();
            AssertParser.SucceedsWith(ab, "Ab", m => Assert.AreEqual("Ab", m));
        }
    }
}
