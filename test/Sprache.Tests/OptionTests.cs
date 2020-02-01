using Sprache;
using Xunit;

namespace Sprache.Tests
{
    public class OptionTests
    {
        private Parser<IOption<char>> ParserOptionalSelect = Parse.Char('a').Optional().Select(o => o.Select(c => char.ToUpperInvariant(c)));

        private Parser<IOption<string>> ParserOptionalSelectMany =
                from o1 in Parse.Char('a').Optional()
                from o2 in Parse.Char('b').Optional()
                select o1.SelectMany(c1 => o2.Select(c2 => $"{c2}{c1}"));

        private Parser<IOption<string>> ParserOptionalLinq =
                from o1 in Parse.Char('a').Optional()
                from o2 in Parse.Char('b').Optional()
                select (from c1 in o1 from c2 in o2 select $"{c2}{c1}");

        private void AssertSome<T>(IOption<T> option, T expected) => Assert.True(option.IsDefined && option.Get().Equals(expected));

        [Fact]
        public void TestSelect() => AssertParser.SucceedsWith(ParserOptionalSelect, "a", o => AssertSome(o, 'A'));

        [Fact]
        public void TestSelectManySome() => AssertParser.SucceedsWith(ParserOptionalSelectMany, "ab", o => AssertSome(o, "ba"));

        [Fact]
        public void TestSelectManyNone() => AssertParser.SucceedsWith(ParserOptionalSelectMany, "b", o => Assert.True(o.IsEmpty));

        [Fact]
        public void TestSelectManyLinq() => AssertParser.SucceedsWith(ParserOptionalLinq, "ab", o => AssertSome(o, "ba"));
    }
}