using Xunit;
using System;

namespace Sprache.Tests.Scenarios
{
    public class StarDateTest
    {
        static readonly Parser<DateTime> StarTrek2009_StarDate =
            from year in Parse.Digit.Many().Text()
            from delimiter in Parse.Char('.')
            from dayOfYear in Parse.Digit.Repeat(1,3).Text().End()
            select new DateTime(int.Parse(year), 1, 1).AddDays(int.Parse(dayOfYear) - 1);

        [Fact]
        public void ItIsPossibleToParseAStarDate()
        {
            Assert.Equal(new DateTime(2259, 2, 24), StarTrek2009_StarDate.Parse("2259.55"));
        }

        [Fact]
        public void InvalidStarDatesAreNotParsed()
        {
            Assert.Throws<ParseException>(() => { var date = StarTrek2009_StarDate.Parse("2259.4000"); });
        }
    }
}
