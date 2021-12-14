using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sprache.Tests
{
    public class CommentParserTest
    {
        [Fact]
        public void Check_single_line_comment_for_both_eol_windows_linux()
        {
            var parser = new CommentParser();
            var expected = "this is a line comment";
            var result = parser.SingleLineComment.Parse("//this is a line comment\r\nint i=5;");
            Assert.Equal(result, expected);
            result = parser.SingleLineComment.Parse("//this is a line comment\nint i=5;");
            Assert.Equal(result, expected);

        }
        [Fact]
        public void Check_any_comment_for_both_eol_windows_linux()
        {
            var parser = new CommentParser();
            var expected = "this is a line comment";
            var result = parser.AnyComment.Parse("//this is a line comment\r\nint i=5;");
            Assert.Equal(result, expected);
            result = parser.AnyComment.Parse("//this is a line comment\nint i=5;");
            Assert.Equal(result, expected);

        }
    }
}
