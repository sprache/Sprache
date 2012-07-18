using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Sprache.Tests
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void FailureContainingBracketFormattedSuccessfully()
        {
            var p = Parse.String("xy").Text().XMany().End();
            var r = (Failure<IEnumerable<string>>)p.TryParse("x{");
            Assert.That(r.Message.Contains("unexpected '{'"));
        }
    }
}
