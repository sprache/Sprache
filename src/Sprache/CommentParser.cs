namespace Sprache
{
    /// <summary>
    /// Constructs customizable comment parsers.
    /// </summary>
    public class CommentParser : IComment
    {
        ///<summary>
        ///Single-line comment header.
        ///</summary>
        public string Single { get; set; }

        ///<summary>
        ///Newline character preference.
        ///</summary>
        public string NewLine { get; set; }

        ///<summary>
        ///Multi-line comment opener.
        ///</summary>
        public string MultiOpen { get; set; }

        ///<summary>
        ///Multi-line comment closer.
        ///</summary>
        public string MultiClose { get; set; }

        /// <summary>
        /// Initializes a Comment with C-style headers and Windows newlines.
        /// </summary>
        public CommentParser()
        {
            Single = "//";
            MultiOpen = "/*";
            MultiClose = "*/";
            NewLine = "\n";
        }

        /// <summary>
        /// Initializes a Comment with custom multi-line headers and newline characters.
        /// Single-line headers are made null, it is assumed they would not be used.
        /// </summary>
        /// <param name="multiOpen"></param>
        /// <param name="multiClose"></param>
        /// <param name="newLine"></param>
        public CommentParser(string multiOpen, string multiClose, string newLine)
        {
            Single = null;
            MultiOpen = multiOpen;
            MultiClose = multiClose;
            NewLine = newLine;
        }

        /// <summary>
        /// Initializes a Comment with custom headers and newline characters.
        /// </summary>
        /// <param name="single"></param>
        /// <param name="multiOpen"></param>
        /// <param name="multiClose"></param>
        /// <param name="newLine"></param>
        public CommentParser(string single, string multiOpen, string multiClose, string newLine)
        {
            Single = single;
            MultiOpen = multiOpen;
            MultiClose = multiClose;
            NewLine = newLine;
        }

        ///<summary>
        ///Parse a single-line comment.
        ///</summary>
        public Parser<string> SingleLineComment
        {
            get
            {
                if (Single == null)
                    throw new ParseException("Field 'Single' is null; single-line comments not allowed.");

                return from first in Parse.String(Single)
                       from rest in Parse.CharExcept(NewLine).Many().Text()
                       select rest;
            }
            private set { }
        }

        ///<summary>
        ///Parse a multi-line comment.
        ///</summary>
        public Parser<string> MultiLineComment
        {
            get
            {
                if (MultiOpen == null)
                    throw new ParseException("Field 'MultiOpen' is null; multi-line comments not allowed.");
                else if (MultiClose == null)
                    throw new ParseException("Field 'MultiClose' is null; multi-line comments not allowed.");

                return from first in Parse.String(MultiOpen)
                       from rest in Parse.AnyChar
                                    .Until(Parse.String(MultiClose)).Text()
                       select rest;
            }
            private set { }
        }

        ///<summary>
        ///Parse a comment.
        ///</summary>
        public Parser<string> AnyComment
        {
            get
            {
                if (Single != null && MultiOpen != null && MultiClose != null)
                    return SingleLineComment.Or(MultiLineComment);
                else if (Single != null && (MultiOpen == null || MultiClose == null))
                    return SingleLineComment;
                else if (Single == null && (MultiOpen != null && MultiClose != null))
                    return MultiLineComment;
                else throw new ParseException("Unable to parse comment; check values of fields 'MultiOpen' and 'MultiClose'.");
            }
            private set { }
        }
    }
}
