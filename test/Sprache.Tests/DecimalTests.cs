using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Sprache.Tests
{
    public class DecimalTests : IDisposable
    {
        private static readonly Parser<string> DecimalParser = Parse.Decimal.End();
        private static readonly Parser<string> DecimalInvariantParser = Parse.DecimalInvariant.End();

        private CultureInfo _previousCulture;

        public DecimalTests()
        {
            _previousCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _previousCulture;
        }

        [Fact]
        public void LeadingDigits()
        {
            Assert.Equal("12.23", DecimalParser.Parse("12.23"));
        }

        [Fact]
        public void NoLeadingDigits()
        {
            Assert.Equal(".23", DecimalParser.Parse(".23"));
        }

        [Fact]
        public void TwoPeriods()
        {
            Assert.Throws<ParseException>(() => DecimalParser.Parse("1.2.23"));
        }

        [Fact]
        public void Letters()
        {
            Assert.Throws<ParseException>(() => DecimalParser.Parse("1A.5"));
        }

        [Fact]
        public void LeadingDigitsInvariant()
        {
            Assert.Equal("12.23", DecimalInvariantParser.Parse("12.23"));
        }

    }
}