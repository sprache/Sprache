using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache
{
    /// <summary>
    /// Represents a customizable comment parser.
    /// </summary>
    public interface IComment
    {
        ///<summary>
        /// Single-line comment header.
        ///</summary>
        string Single { get; set; }

        ///<summary>
        /// Newline character preference.
        ///</summary>
        string NewLine { get; set; }

        ///<summary>
        /// Multi-line comment opener.
        ///</summary>
        string MultiOpen { get; set; }

        ///<summary>
        /// Multi-line comment closer.
        ///</summary>
        string MultiClose { get; set; }

        ///<summary>
        /// Parse a single-line comment.
        ///</summary>
        Parser<string> SingleLineComment { get; }

        ///<summary>
        /// Parse a multi-line comment.
        ///</summary>
        Parser<string> MultiLineComment { get; }

        ///<summary>
        /// Parse a comment.
        ///</summary>
        Parser<string> AnyComment { get; }
    }
}
