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
            Return("").End()
                .Or(LineEnd.End())
                .Or(LineEnd)
                .Named("LineTerminator");

        /// <summary>
        /// Parser for single line comment. Doesn't contain tail line ending
        /// </summary>
        /// <param name="commentStart">Symbols to start comment. I.e. "//" for C#, "#" for perl, ";" for assembler</param>
        /// <returns></returns>
        public static Parser<string> EndOfLineComment(string commentStart)
        {
            return
                from start in String(commentStart)
                from comment in CharExcept("\r\n").Many().Text()
                select comment;
        }

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