using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache.Tests.Scenarios
{
    [TestFixture]
    public class StarDateTest
    {
        static readonly Parser<DateTime> StarTrek2009_StarDate =
            from year in Parse.Digit.Many().Text()
            from delimiter in Parse.Char('.')
            from dayOfYear in Parse.Digit.Repeat(1,3).Text().End()
            select new DateTime(int.Parse(year), 1, 1).AddDays(int.Parse(dayOfYear) - 1);

        [Test]
        public void ItIsPossibleToParseAStarDate()
        {
            Assert.That(StarTrek2009_StarDate.Parse("2259.55"), Is.EqualTo(new DateTime(2259, 2, 24)));
        }

        [Test, ExpectedException(typeof(ParseException))]
        public void InvalidStarDatesAreNotParsed()
        {
            var date = StarTrek2009_StarDate.Parse("2259.4000");
        }
    }
}
