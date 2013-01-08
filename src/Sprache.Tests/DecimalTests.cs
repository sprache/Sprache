﻿using NUnit.Framework;
using System.Globalization;
using System.Threading;

namespace Sprache.Tests
{
    [TestFixture]
    public class DecimalTests
    {
        private static readonly Parser<string> DecimalParser = Parse.Decimal.End();

        private CultureInfo _previousCulture;

        [SetUp]
        public void Init()
        {
            _previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TearDown]
        public void Cleanup()
        {
            Thread.CurrentThread.CurrentCulture = _previousCulture;
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
        [ExpectedException(typeof(ParseException))]
        public void TwoPeriods()
        {
            DecimalParser.Parse("1.2.23");
        }

        [Test]
        [ExpectedException(typeof(ParseException))]
        public void Letters()
        {
            DecimalParser.Parse("1A.5");
        }
    }
}