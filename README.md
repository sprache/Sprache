Sprache is a simple, lightweight library for constructing parsers directly in C# code.

It doesn't compete with "industrial strength" language workbenches - it fits somewhere in between regular expressions and a full-featured toolset like [ANTLR](http://antlr.org).

Usage
-----

Unlike most parser-building frameworks, you use Sprache directly from your program code, and don't need to set up any build-time code generation tasks. Sprache itself is a single tiny assembly.

A simple parser might parse a sequence of characters:

    // Parse any number of capital 'A's in a row
    var parseA = Parse.Char('A').AtLeastOnce();

Sprache provides a number of built-in functions that can make bigger parsers from smaller ones, often callable via Linq query comprehensions:

    Parser<string> identifier =
        from leading in Parse.Whitespace.Many()
        from first in Parse.Letter.Once()
        from rest in Parse.LetterOrDigit.Many()
        from trailing in Parse.Whitespace.Many()
        select new string(first.Concat(rest).ToArray());

    var id = identifier.Parse(" abc123  ");

    Assert.AreEqual("abc123", id);
    
Background and Tutorials
------------------------

The best place to start is [this introductory article.](http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/)

Examples included with the source demonstrate:

* [Parsing XML directly to a Document object](https://github.com/sprache/Sprache/blob/master/src/XmlExample/Program.cs)
* [Parsing numeric expressions to System.Linq.Expression objects](https://github.com/sprache/Sprache/blob/master/src/LinqyCalculator/ExpressionParser.cs)
* [Parsing comma-separated (CSV) 'files' into lists of strings](https://github.com/sprache/Sprache/blob/master/src/Sprache.Tests/Scenarios/CsvTests.cs)

Parser combinators are covered extensively on the web. The original paper describing the monadic implementation by [Graham Hutton and Eric Meijer](http://www.cs.nott.ac.uk/~gmh/monparsing.pdf) is very readable. Sprache grew out of some exercises in [Hutton's Haskell book](http://www.amazon.com/Programming-Haskell-Graham-Hutton/dp/0521692695).

Sprache itself draws on some great C# tutorials:

* [Luke Hoban's Blog](http://blogs.msdn.com/b/lukeh/archive/2007/08/19/monadic-parser-combinators-using-c-3-0.aspx)
* [Brian McNamara's Blog](http://lorgonblog.wordpress.com/2007/12/02/c-3-0-lambda-and-the-first-post-of-a-series-about-monadic-parser-combinators/)
