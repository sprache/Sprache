using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sprache.Tests.Scenarios
{
    public class CsvTests
    {
        static readonly Parser<char> CellSeparator = Parse.Char(',');

        static readonly Parser<char> QuotedCellDelimiter = Parse.Char('"');

        static readonly Parser<char> QuoteEscape = Parse.Char('"');

        static Parser<T> Escaped<T>(Parser<T> following)
        {
            return from escape in QuoteEscape
                   from f in following
                   select f;
        }

        static readonly Parser<char> QuotedCellContent =
            Parse.AnyChar.Except(QuotedCellDelimiter).Or(Escaped(QuotedCellDelimiter));

        static readonly Parser<char> LiteralCellContent =
            Parse.AnyChar.Except(CellSeparator).Except(Parse.String(Environment.NewLine));

        static readonly Parser<string> QuotedCell =
            from open in QuotedCellDelimiter
            from content in QuotedCellContent.Many().Text()
            from end in QuotedCellDelimiter
            select content;

        static readonly Parser<string> NewLine =
            Parse.String(Environment.NewLine).Text();

        static readonly Parser<string> RecordTerminator =
            Parse.Return("").End().XOr(
            NewLine.End()).Or(
            NewLine);

        static readonly Parser<string> Cell =
            QuotedCell.XOr(
            LiteralCellContent.XMany().Text());

        static readonly Parser<IEnumerable<string>> Record =
            from leading in Cell
            from rest in CellSeparator.Then(_ => Cell).Many()
            from terminator in RecordTerminator
            select Cons(leading, rest);

        static readonly Parser<IEnumerable<IEnumerable<string>>> Csv =
            Record.XMany().End();

        static IEnumerable<T> Cons<T>(T head, IEnumerable<T> rest)
        {
            yield return head;
            foreach (var item in rest)
                yield return item;
        }

        [Fact]
        public void ParsesSimpleList()
        {
            var input = "a,b";
            var r = Csv.Parse(input);
            Assert.Single(r);

            var l1 = r.First().ToArray();
            Assert.Equal(2, l1.Length);
            Assert.Equal("a", l1[0]);
            Assert.Equal("b", l1[1]);
        }

        [Fact]
        public void ParsesListWithEmptyEnding()
        {
            var input = "a,b,";
            var r = Csv.Parse(input);
            Assert.Single(r);

            var l1 = r.First().ToArray();
            Assert.Equal(3, l1.Length);
            Assert.Equal("a", l1[0]);
            Assert.Equal("b", l1[1]);
            Assert.Equal("", l1[2]);
        }

        [Fact]
        public void ParsesListWithNewlineEnding()
        {
            var input = "a,b," + Environment.NewLine;
            var r = Csv.Parse(input);
            Assert.Single(r);

            var l1 = r.First().ToArray();
            Assert.Equal(3, l1.Length);
            Assert.Equal("a", l1[0]);
            Assert.Equal("b", l1[1]);
            Assert.Equal("", l1[2]);
        }

        [Fact]
        public void ParsesLines()
        {
            var input = "a,b,c" + Environment.NewLine + "d,e,f";
            var r = Csv.Parse(input);
            Assert.Equal(2, r.Count());

            var l1 = r.First().ToArray();
            Assert.Equal(3, l1.Length);
            Assert.Equal("a", l1[0]);
            Assert.Equal("b", l1[1]);
            Assert.Equal("c", l1[2]);

            var l2 = r.Skip(1).First().ToArray();
            Assert.Equal(3, l2.Length);
            Assert.Equal("d", l2[0]);
            Assert.Equal("e", l2[1]);
            Assert.Equal("f", l2[2]);
        }

        [Fact]
        public void IgnoresTrailingNewline()
        {
            var input = "a,b,c" + Environment.NewLine + "d,e,f" + Environment.NewLine;
            var r = Csv.Parse(input);
            Assert.Equal(2, r.Count());
        }

        [Fact]
        public void IgnoresCommasInQuotedCells()
        {
            var input = "a,\"b,c\"";
            var r = Csv.Parse(input);
            Assert.Equal(2, r.First().Count());
        }

        [Fact]
        public void RecognisesDoubledQuotesAsSingleLiteral()
        {
            var input = "a,\"b\"\"c\"";
            var r = Csv.Parse(input);
            Assert.Equal("b\"c", r.First().ToArray()[1]);
        }

        [Fact]
        public void AllowsNewLinesInQuotedCells()
        {
            var input = "a,b,\"c" + Environment.NewLine + "d\"";
            var r = Csv.Parse(input);
            Assert.Single(r);
        }

        [Fact]
        public void IgnoresEmbeddedQuotesWhenNotFirstCharacter()
        {
            var input = "a\"b";
            var r = Csv.Parse(input);
            Assert.Equal("a\"b", r.First().First());
        }
    }
}
