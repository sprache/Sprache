using Xunit;


namespace Sprache.Tests
{
    public class RefTests
    {
        private static Parser<string> ParserSomething = Parse.String("#").Text().Named("something");

        private static Parser<string> ParserIdentifier = (from a in Parse.String("a") select "a").Named("identifier");

        private static Parser<string> ParserUnderTest =
         (from _0 in Parse.Ref(() => ParserIdentifier)
          from _1 in Parse.Ref(() => ParserSomething)
          from _2 in Parse.String("_")
          select "assignment_1")
         .Or(from _0 in Parse.Ref(() => ParserIdentifier)
             from _1 in Parse.Ref(() => ParserSomething)
             select "assignment_2")
        .Or(from _0 in ParserIdentifier select _0);

        [Fact]
        public void TestOr() => AssertParser.SucceedsWith(ParserUnderTest, "a=1", o => Assert.Equal("a", o));
    }
}
