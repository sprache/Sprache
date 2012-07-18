using System;
using NUnit.Framework;

namespace TinyTemplates.Tests
{
    [TestFixture]
    public class TemplateTests
    {
        static readonly DateTime SaturdayInSeptember = new DateTime(2010, 9, 4);

        [Test]
        public void AnEmptyTemplateProducesNoOutput()
        {
            var tt = new Template("");
            var o = tt.Execute(new object());
            Assert.AreEqual("", o);
        }

        [Test]
        public void LiteralTextIsOutputLiterally()
        {
            const string txt = "abc";
            var tt = new Template(txt);
            var o = tt.Execute(new object());
            Assert.AreEqual(txt, o);
        }

        [Test]
        public void ADoubleHashIsEscaped()
        {
            var tt = new Template("##");
            var o = tt.Execute(new object());
            Assert.AreEqual("#", o);
        }

        [Test]
        public void AHashBeforeAnIdentifierSubstitutesAModelProperty()
        {
            var tt = new Template("#DayOfWeek");
            var o = tt.Execute(SaturdayInSeptember);
            Assert.AreEqual("Saturday", o);
        }

        [Test]
        public void BracesOptionallyDelimitDirectives()
        {
            var tt = new Template("#{DayOfWeek}");
            var o = tt.Execute(SaturdayInSeptember);
            Assert.AreEqual("Saturday", o);
        }

        [Test]
        public void IdentifiersAreNotCaseSensitive()
        {
            var tt = new Template("#DAYOFWEEK");
            var o = tt.Execute(SaturdayInSeptember);
            Assert.AreEqual("Saturday", o);
        }

        [Test]
        public void IterationCollectsElements()
        {
            var tt = new Template("#|Days#DayOfWeek#.");
            var m = new Model1 {Days = new[] {SaturdayInSeptember, SaturdayInSeptember.AddDays(1)}};
            var o = tt.Execute(m);
            Assert.AreEqual("SaturdaySunday", o);
        }

        [Test]
        public void ModelPropertiesAreTraversed()
        {
            var tt = new Template("#DateTime1.DayOfWeek");
            var m = new Model1 { DateTime1 = SaturdayInSeptember };
            var o = tt.Execute(m);
            Assert.AreEqual("Saturday", o);
        }

        [Test, Ignore]
        public void WithinIterationParentModelPropertiesAreAccessible()
        {
            var tt = new Template("#|Days#DateTime1.DayOfWeek#.");
            var m = new Model1
                        {
                            DateTime1 = SaturdayInSeptember,
                            Days = new[] { SaturdayInSeptember, SaturdayInSeptember.AddDays(1) }
                        };
            var o = tt.Execute(m);
            Assert.AreEqual("SaturdaySaturday", o);
        }
    }
}
