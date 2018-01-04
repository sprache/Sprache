using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sprache.Tests
{
    public class ParseTests
    {
        [Fact]
        public void Parser_OfChar_AcceptsThatChar()
        {
            AssertParser.SucceedsWithOne(Parse.Char('a').Once(), "a", 'a');
        }

        [Fact]
        public void Parser_OfChar_AcceptsOnlyOneChar()
        {
            AssertParser.SucceedsWithOne(Parse.Char('a').Once(), "aaa", 'a');
        }

        [Fact]
        public void Parser_OfChar_DoesNotAcceptNonMatchingChar()
        {
            AssertParser.FailsAt(Parse.Char('a').Once(), "b", 0);
        }

        [Fact]
        public void Parser_OfChar_DoesNotAcceptEmptyInput()
        {
            AssertParser.Fails(Parse.Char('a').Once(), "");
        }

        [Fact]
        public void Parser_OfChars_AcceptsAnyOfThoseChars()
        {
            var parser = Parse.Chars('a', 'b', 'c').Once();
            AssertParser.SucceedsWithOne(parser, "a", 'a');
            AssertParser.SucceedsWithOne(parser, "b", 'b');
            AssertParser.SucceedsWithOne(parser, "c", 'c');
        }

        [Fact]
        public void Parser_OfChars_UsingString_AcceptsAnyOfThoseChars()
        {
            var parser = Parse.Chars("abc").Once();
            AssertParser.SucceedsWithOne(parser, "a", 'a');
            AssertParser.SucceedsWithOne(parser, "b", 'b');
            AssertParser.SucceedsWithOne(parser, "c", 'c');
        }

        [Fact]
        public void Parser_OfManyChars_AcceptsEmptyInput()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "");
        }

        [Fact]
        public void Parser_OfManyChars_AcceptsManyChars()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "aaa");
        }

        [Fact]
        public void Parser_OfAtLeastOneChar_DoesNotAcceptEmptyInput()
        {
            AssertParser.Fails(Parse.Char('a').AtLeastOnce(), "");
        }

        [Fact]
        public void Parser_OfAtLeastOneChar_AcceptsOneChar()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').AtLeastOnce(), "a");
        }

        [Fact]
        public void Parser_OfAtLeastOneChar_AcceptsManyChars()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').AtLeastOnce(), "aaa");
        }

        [Fact]
        public void ConcatenatingParsers_ConcatenatesResults()
        {
            var p = Parse.Char('a').Once().Then(a =>
                Parse.Char('b').Once().Select(b => a.Concat(b)));
            AssertParser.SucceedsWithAll(p, "ab");
        }

        [Fact]
        public void ReturningValue_DoesNotAdvanceInput()
        {
            var p = Parse.Return(1);
            AssertParser.SucceedsWith(p, "abc", n => Assert.Equal(1, n));
        }

        [Fact]
        public void ReturningValue_ReturnsValueAsResult()
        {
            var p = Parse.Return(1);
            var r = (Result<int>)p.TryParse("abc");
            Assert.Equal(0, r.Remainder.Position);
        }

        [Fact]
        public void CanSpecifyParsersUsingQueryComprehensions()
        {
            var p = from a in Parse.Char('a').Once()
                    from bs in Parse.Char('b').Many()
                    from cs in Parse.Char('c').AtLeastOnce()
                    select a.Concat(bs).Concat(cs);

            AssertParser.SucceedsWithAll(p, "abbbc");
        }

        [Fact]
        public void WhenFirstOptionSucceedsButConsumesNothing_SecondOptionTried()
        {
            var p = Parse.Char('a').Many().XOr(Parse.Char('b').Many());
            AssertParser.SucceedsWithAll(p, "bbb");
        }

        [Fact]
        public void WithXOr_WhenFirstOptionFailsAndConsumesInput_SecondOptionNotTried()
        {
            var first = Parse.Char('a').Once().Concat(Parse.Char('b').Once());
            var second = Parse.Char('a').Once();
            var p = first.XOr(second);
            AssertParser.FailsAt(p, "a", 1);
        }

        [Fact]
        public void WithOr_WhenFirstOptionFailsAndConsumesInput_SecondOptionTried()
        {
            var first = Parse.Char('a').Once().Concat(Parse.Char('b').Once());
            var second = Parse.Char('a').Once();
            var p = first.Or(second);
            AssertParser.SucceedsWithAll(p, "a");
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void DetectsMutualLeftRecursion()
        {
            Assert.Throws<ParseException>(() => ABSeq.End().TryParse("baba"));
        }

        [Fact]
        public void WithMany_WhenLastElementFails_FailureReportedAtLastElement()
        {
            var ab = from a in Parse.Char('a')
                     from b in Parse.Char('b')
                     select "ab";

            var p = ab.Many().End();

            AssertParser.FailsAt(p, "ababaf", 4);
        }

        [Fact]
        public void WithXMany_WhenLastElementFails_FailureReportedAtLastElement()
        {
            var ab = from a in Parse.Char('a')
                     from b in Parse.Char('b')
                     select "ab";

            var p = ab.XMany().End();

            AssertParser.FailsAt(p, "ababaf", 5);
        }

        [Fact]
        public void ExceptStopsConsumingInputWhenExclusionParsed()
        {
            var exceptAa = Parse.AnyChar.Except(Parse.String("aa")).Many().Text();
            AssertParser.SucceedsWith(exceptAa, "abcaab", r => Assert.Equal("abc", r));
        }

        [Fact]
        public void UntilProceedsUntilTheStopConditionIsMetAndReturnsAllButEnd()
        {
            var untilAa = Parse.AnyChar.Until(Parse.String("aa")).Text();
            var r = untilAa.TryParse("abcaab");
            Assert.IsType<Result<string>>(r);
            var s = (Result<string>)r;
            Assert.Equal("abc", s.Value);
            Assert.Equal(5, s.Remainder.Position);
        }

        [Fact]
        public void OptionalParserConsumesInputOnSuccessfulMatch()
        {
            var optAbc = Parse.String("abc").Text().Optional();
            var r = optAbc.TryParse("abcd");
            Assert.True(r.WasSuccessful);
            Assert.Equal(3, r.Remainder.Position);
            Assert.True(r.Value.IsDefined);
            Assert.Equal("abc", r.Value.Get());
        }

        [Fact]
        public void OptionalParserDoesNotConsumeInputOnFailedMatch()
        {
            var optAbc = Parse.String("abc").Text().Optional();
            var r = optAbc.TryParse("d");
            Assert.True(r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);
            Assert.True(r.Value.IsEmpty);
        }

        [Fact]
        public void RegexParserConsumesInputOnSuccessfulMatch()
        {
            var digits = Parse.Regex(@"\d+");
            var r = digits.TryParse("123d");
            Assert.True(r.WasSuccessful);
            Assert.Equal("123", r.Value);
            Assert.Equal(3, r.Remainder.Position);
        }

        [Fact]
        public void RegexParserDoesNotConsumeInputOnFailedMatch()
        {
            var digits = Parse.Regex(@"\d+");
            var r = digits.TryParse("d123");
            Assert.False(r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);
        }

        [Fact]
        public void RegexMatchParserConsumesInputOnSuccessfulMatch()
        {
            var digits = Parse.RegexMatch(@"\d(\d*)");
            var r = digits.TryParse("123d");
            Assert.True(r.WasSuccessful);
            Assert.Equal("123", r.Value.Value);
            Assert.Equal("23", r.Value.Groups[1].Value);
            Assert.Equal(3, r.Remainder.Position);
        }

        [Fact]
        public void RegexMatchParserDoesNotConsumeInputOnFailedMatch()
        {
            var digits = Parse.RegexMatch(@"\d+");
            var r = digits.TryParse("d123");
            Assert.False(r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);
        }

        [Fact]
        public void PositionedParser()
        {
            var pos = (from s in Parse.String("winter").Text()
                       select new PosAwareStr { Value = s })
                       .Positioned();
            var r = pos.TryParse("winter");
            Assert.True(r.WasSuccessful);
            Assert.Equal(0, r.Value.Pos.Pos);
            Assert.Equal(6, r.Value.Length);
        }

        [Fact]
        public void XAtLeastOnceParser_WhenLastElementFails_FailureReportedAtLastElement()
        {
            var ab = Parse.String("ab").Text();
            var p = ab.XAtLeastOnce().End();
            AssertParser.FailsAt(p, "ababaf", 5);
        }

        [Fact]
        public void XAtLeastOnceParser_WhenFirstElementFails_FailureReportedAtFirstElement()
        {
            var ab = Parse.String("ab").Text();
            var p = ab.XAtLeastOnce().End();
            AssertParser.FailsAt(p, "d", 0);
        }

        [Fact]
        public void NotParserConsumesNoInputOnFailure()
        {
            var notAb = Parse.String("ab").Text().Not();
            AssertParser.FailsAt(notAb, "abc", 0);
        }

        [Fact]
        public void NotParserConsumesNoInputOnSuccess()
        {
            var notAb = Parse.String("ab").Text().Not();
            var r = notAb.TryParse("d");
            Assert.True(r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);
        }

        [Fact]
        public void IgnoreCaseParser()
        {
            var ab = Parse.IgnoreCase("ab").Text();
            AssertParser.SucceedsWith(ab, "Ab", m => Assert.Equal("Ab", m));
        }

        [Fact]
        public void RepeatParserConsumeInputOnSuccessfulMatch()
        {
            var repeated = Parse.Char('a').Repeat(3);
            var r = repeated.TryParse("aaabbb");
            Assert.True(r.WasSuccessful);
            Assert.Equal(3, r.Remainder.Position);
        }

        [Fact]
        public void RepeatParserDoesntConsumeInputOnFailedMatch()
        {
            var repeated = Parse.Char('a').Repeat(3);
            var r = repeated.TryParse("bbbaaa");
            Assert.True(!r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);
        }

        [Fact]
        public void RepeatParserCanParseWithCountOfZero()
        {
            var repeated = Parse.Char('a').Repeat(0);
            var r = repeated.TryParse("bbb");
            Assert.True(r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);
        }

        [Fact]
        public void RepeatParserCanParseAMinimumNumberOfValues()
        {
            var repeated = Parse.Char('a').Repeat(4, 5);

            // Test failure.
            var r = repeated.TryParse("aaa");
            Assert.False(r.WasSuccessful);
            Assert.Equal(0, r.Remainder.Position);

            // Test success.
            r = repeated.TryParse("aaaa");
            Assert.True(r.WasSuccessful);
            Assert.Equal(4, r.Remainder.Position);
        }

        [Fact]
        public void RepeatParserCanParseAMaximumNumberOfValues()
        {
            var repeated = Parse.Char('a').Repeat(4, 5);

            var r = repeated.TryParse("aaaa");
            Assert.True(r.WasSuccessful);
            Assert.Equal(4, r.Remainder.Position);

            r = repeated.TryParse("aaaaa");
            Assert.True(r.WasSuccessful);
            Assert.Equal(5, r.Remainder.Position);

            r = repeated.TryParse("aaaaaa");
            Assert.True(r.WasSuccessful);
            Assert.Equal(5, r.Remainder.Position);
        }

        [Fact]
        public void RepeatParserErrorMessagesAreReadable()
        {
            var repeated = Parse.Char('a').Repeat(4, 5);

            var expectedMessage = "Parsing failure: Unexpected 'end of input'; expected 'a' between 4 and 5 times, but found 3";

            try
            {
                var r = repeated.Parse("aaa");
            }
            catch(ParseException ex)
            {
                Assert.StartsWith(expectedMessage, ex.Message);
            }
        }

        [Fact]
        public void CanParseSequence()
        {
            var sequence = Parse.Char('a').DelimitedBy(Parse.Char(','));
            var r = sequence.TryParse("a,a,a");
            Assert.True(r.WasSuccessful);
            Assert.True(r.Remainder.AtEnd);
        }

        [Fact]
        public void FailGracefullyOnSequence()
        {
            var sequence = Parse.Char('a').XDelimitedBy(Parse.Char(','));
            AssertParser.FailsWith(sequence, "a,a,b", result =>
            {
                Assert.Contains("unexpected 'b'", result.Message);
                Assert.Contains("a", result.Expectations);
            });
        }

        [Fact]
        public void CanParseContained()
        {
            var parser = Parse.Char('a').Contained(Parse.Char('['), Parse.Char(']'));
            var r = parser.TryParse("[a]");
            Assert.True(r.WasSuccessful);
            Assert.True(r.Remainder.AtEnd);
        }

        [Fact]
        public void TextSpanParserReturnsTheSpanOfTheParsedValue()
        {
            var parser =
                from leading in Parse.WhiteSpace.Many()
                from span in Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Span()
                from trailing in Parse.WhiteSpace.Many()
                select span;

            var r = parser.TryParse("  Hello!");
            Assert.True(r.WasSuccessful);
            Assert.False(r.Remainder.AtEnd);

            var id = r.Value;
            Assert.Equal("Hello", id.Value);
            Assert.Equal(5, id.Length);

            Assert.Equal(2, id.Start.Pos);
            Assert.Equal(1, id.Start.Line);
            Assert.Equal(3, id.Start.Column);

            Assert.Equal(7, id.End.Pos);
            Assert.Equal(1, id.End.Line);
            Assert.Equal(8, id.End.Column);
        }
    }
}
