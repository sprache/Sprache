﻿using NUnit.Framework;
using System.Globalization;

namespace Sprache.Tests
{
    [TestFixture]
    public class DecimalTests
    {
        private static readonly Parser<string> DecimalParser = Parse.Decimal.End();
        private static readonly Parser<string> DecimalInvariantParser = Parse.DecimalInvariant.End();

        private CultureInfo _previousCulture;

        [SetUp]
        public void Init()
        {
            _previousCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TearDown]
        public void Cleanup()
        {
            CultureInfo.CurrentCulture = _previousCulture;
        }

        [Test]
        public void LeadingDigits()
        {
            Assert.AreEqual("12.23", DecimalParser.Parse("12.23"));
        }

        [Test]
        public void NoLeadingDigits()
        {
            Assert.AreEqual(".23", DecimalParser.Parse(".23"));
        }

        [Test]
        public void TwoPeriods()
        {
            Assert.Throws<ParseException>(() => DecimalParser.Parse("1.2.23"));
        }

        [Test]
        public void Letters()
        {
            Assert.Throws<ParseException>(() => DecimalParser.Parse("1A.5"));
        }

        [Test]
        public void LeadingDigitsInvariant()
        {
            Assert.AreEqual("12.23", DecimalInvariantParser.Parse("12.23"));
        }

    }
}