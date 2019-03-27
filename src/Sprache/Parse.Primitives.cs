namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// \n or \r\n
        /// </summary>
        public static Parser<string> LineEnd =
            (from r in Char('\r').Optional()
            from n in Char('\n')
            select r.IsDefined ? r.Get().ToString() + n : n.ToString())
            .Named("LineEnd");

        /// <summary>
        /// line ending or end of input
        /// </summary>
        public static Parser<string> LineTerminator =
            Return("").End() | LineEnd.End() | LineEnd.Named("LineTerminator");

        /// <summary>
        /// Parser for identifier starting with <paramref name="firstLetterParser"/> and continuing with <paramref name="tailLetterParser"/>
        /// </summary>
        public static Parser<string> Identifier(Parser<char> firstLetterParser, Parser<char> tailLetterParser)
        {
            return
                from firstLetter in firstLetterParser
                from tail in tailLetterParser.Many().Text()
                select firstLetter + tail;
        }
    }
}