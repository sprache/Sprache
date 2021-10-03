using System;
using Xunit;
using Sprache.Union;

namespace SpracheTests.Union
{
    public class BasicTests
    {
        [Fact]
        public void FirstTest()
        {
            var parser =
                (from x in Parse.String("a").Text()
                 select x)
                .Or(
                    from x in Parse.String("ab").Text()
                    select x
                ).Many();

            var parser2 = from x in parser
                          from y in Parse.String("x")
                          select "test";

            var result = parser.DoParse("abab");
            Assert.Equal(3, result.Count);

            var result2 = parser2.DoParse("ababx");
            Assert.Single(result2);
        }
    }
}
