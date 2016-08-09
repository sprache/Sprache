using System;
using Xunit;

namespace TinyTemplates.Tests
{
    public class TemplateTests
    {
        static readonly DateTime SaturdayInSeptember = new DateTime(2010, 9, 4);

        [Fact]
        public void AnEmptyTemplateProducesNoOutput()
        {
            var tt = new Template("");
            var o = tt.Execute(new object());
            Assert.Equal("", o);
        }

        [Fact]
        public void LiteralTextIsOutputLiterally()
        {
            const string txt = "abc";
            var tt = new Template(txt);
            var o = tt.Execute(new object());
            Assert.Equal(txt, o);
        }

        [Fact]
        public void ADoubleHashIsEscaped()
        {
            var tt = new Template("##");
            var o = tt.Execute(new object());
            Assert.Equal("#", o);
        }

        [Fact]
        public void AHashBeforeAnIdentifierSubstitutesAModelProperty()
        {
            var tt = new Template("#DayOfWeek");
            var o = tt.Execute(SaturdayInSeptember);
            Assert.Equal("Saturday", o);
        }

        [Fact]
        public void BracesOptionallyDelimitDirectives()
        {
            var tt = new Template("#{DayOfWeek}");
            var o = tt.Execute(SaturdayInSeptember);
            Assert.Equal("Saturday", o);
        }

        [Fact]
        public void IdentifiersAreNotCaseSensitive()
        {
            var tt = new Template("#DAYOFWEEK");
            var o = tt.Execute(SaturdayInSeptember);
            Assert.Equal("Saturday", o);
        }

        [Fact]
        public void IterationCollectsElements()
        {
            var tt = new Template("#|Days#DayOfWeek#.");
            var m = new Model1 {Days = new[] {SaturdayInSeptember, SaturdayInSeptember.AddDays(1)}};
            var o = tt.Execute(m);
            Assert.Equal("SaturdaySunday", o);
        }

        [Fact]
        public void ModelPropertiesAreTraversed()
        {
            var tt = new Template("#DateTime1.DayOfWeek");
            var m = new Model1 { DateTime1 = SaturdayInSeptember };
            var o = tt.Execute(m);
            Assert.Equal("Saturday", o);
        }

        [Fact(Skip = "Ignore")]
        public void WithinIterationParentModelPropertiesAreAccessible()
        {
            var tt = new Template("#|Days#DateTime1.DayOfWeek#.");
            var m = new Model1
                        {
                            DateTime1 = SaturdayInSeptember,
                            Days = new[] { SaturdayInSeptember, SaturdayInSeptember.AddDays(1) }
                        };
            var o = tt.Execute(m);
            Assert.Equal("SaturdaySaturday", o);
        }
    }
}
