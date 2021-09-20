using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sprache.Tests
{
    public class ParseRefTests
    {
        private static Parser<string> ParserHash = Parse.String("#").Text().Named("something");

        private static Parser<string> ParserIdentifier = (from id in Parse.String("id").Text() select id).Named("identifier");

        private static Parser<string> Parser1UnderTest =
            (from _0 in Parse.Ref(() => ParserIdentifier)
             from _1 in Parse.Ref(() => ParserHash)
             from _2 in Parse.String("_")
             select "alternative_1")
            .Or(from _0 in Parse.Ref(() => ParserIdentifier)
                from _1 in Parse.Ref(() => ParserHash)
                select "alternative_2")
            .Or(from _0 in ParserIdentifier select _0);

        private static Parser<string> Parser2UnderTest = 
            (from _0 in Parse.String("a").Text()
             from _1 in Parse.Ref(() => Parser2UnderTest)
             select _0 + _1)
            .Or(from _0 in Parse.String("b").Text()
                from _1 in Parse.Ref(() => Parser2UnderTest)
                select _0 + _1)
            .Or(from _0 in Parse.String("0").Text() select _0);

        private static Parser<string> Parser3UnderTest =
            (from _0 in Parse.Ref(() => Parser3UnderTest)
             from _1 in Parse.String("a").Text()
             select _0 + _1)
            .Or(from _0 in Parse.String("b").Text()
                from _1 in Parse.Ref(() => Parser3UnderTest)
                select _0 + _1)
            .Or(from _0 in Parse.String("0").Text() select _0);

        private static Parser<string> Parser4UnderTest =
            from _0 in Parse.Ref(() => Parser4UnderTest)
            select "simplest left recursion";

        private static Parser<string> Parser5UnderTest =
          (from _0 in Parse.String("_").Text()
           from _1 in Parse.Ref(() => Parser5UnderTest)
           select _0 + _1)
          .Or(from _0 in Parse.String("+").Text()
              from _1 in Parse.Ref(() => Parser5UnderTest)
              select _0 + _1)
          .Or(Parse.Return(""));

        [Fact]
        public void MultipleRefs() => AssertParser.SucceedsWith(Parser1UnderTest, "id=1", o => Assert.Equal("id", o));

        [Fact]
        public void RecursiveParserWithoutLeftRecursion() => AssertParser.SucceedsWith(Parser2UnderTest, "ababba0", o => Assert.Equal("ababba0", o));

        [Fact]
        public void RecursiveParserWithLeftRecursion() => Assert.Throws<ParseException>(() => Parser3UnderTest.TryParse("b0"));

        [Fact]
        public void SimplestLeftRecursion() => Assert.Throws<ParseException>(() => Parser4UnderTest.TryParse("test"));

        [Fact]
        public void EmptyAlternative1() => AssertParser.SucceedsWith(Parser5UnderTest, "_+_+a", o => Assert.Equal("_+_+", o));

        [Fact]
        public void Issue166()
        {
            var letterA = Parse.Char('a');
            var letterReferenced = Parse.Ref(() => letterA);
            var someAlternative = letterReferenced.Or(letterReferenced);

            Assert.False(someAlternative.TryParse("b").WasSuccessful);
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
    }
}
