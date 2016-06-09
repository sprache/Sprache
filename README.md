Sprache is a simple, lightweight library for constructing parsers directly in C# code.

It doesn't compete with "industrial strength" language workbenches - it fits somewhere in between regular expressions and a full-featured toolset like [ANTLR](http://antlr.org).

Usage
-----

[![Join the chat at https://gitter.im/sprache/Sprache](https://badges.gitter.im/sprache/Sprache.svg)](https://gitter.im/sprache/Sprache?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Unlike most parser-building frameworks, you use Sprache directly from your program code, and don't need to set up any build-time code generation tasks. Sprache itself is a single tiny assembly.

A simple parser might parse a sequence of characters:

```csharp
// Parse any number of capital 'A's in a row
var parseA = Parse.Char('A').AtLeastOnce();
```

Sprache provides a number of built-in functions that can make bigger parsers from smaller ones, often callable via Linq query comprehensions:

```csharp
Parser<string> identifier =
    from leading in Parse.WhiteSpace.Many()
    from first in Parse.Letter.Once()
    from rest in Parse.LetterOrDigit.Many()
    from trailing in Parse.WhiteSpace.Many()
    select new string(first.Concat(rest).ToArray());

var id = identifier.Parse(" abc123  ");

Assert.AreEqual("abc123", id);
```

Examples and Tutorials
----------------------

The best place to start is [this introductory article](http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/).

**Examples** included with the source demonstrate:

* [Parsing XML directly to a Document object](https://github.com/sprache/Sprache/blob/master/src/XmlExample/Program.cs)
* [Parsing numeric expressions to `System.Linq.Expression` objects](https://github.com/sprache/Sprache/blob/master/src/LinqyCalculator/ExpressionParser.cs)
* [Parsing comma-separated values (CSV) into lists of strings](https://github.com/sprache/Sprache/blob/master/src/Sprache.Tests/Scenarios/CsvTests.cs)

**Tutorials** explaining Sprache:

 * A great [CodeProject article by Alexey Yakovlev ](http://www.codeproject.com/Articles/795056/Sprache-Calc-building-yet-another-expression-evalu) (and [in Russian](http://habrahabr.ru/post/228037/))
 * Mike Hadlow's example of [parsing connection strings](http://mikehadlow.blogspot.com.au/2012/09/parsing-connection-string-with-sprache.html)

**Real-world** parsers built with Sprache:

 * The [template parser](https://github.com/OctopusDeploy/Octostache/blob/master/source/Octostache/Templates/TemplateParser.cs) in [Octostache](https://github.com/OctopusDeploy/Octostache), the variable substitution language of [Octopus Deploy](https://octopus.com)
 * The [XAML binding expression parser](https://github.com/SuperJMN/OmniXAML/blob/master/Source/OmniXaml/Parsers/MarkupExtensions/MarkupExtensionParser.cs) in [OmniXaml](https://github.com/SuperJMN/OmniXAML), the cross-platform XAML implementation
 * The query parser in [Seq](https://getseq.net), a structured log server for .NET
 * The [connection string parser](https://github.com/EasyNetQ/EasyNetQ/blob/master/Source/EasyNetQ/ConnectionString/ConnectionStringGrammar.cs) in [EasyNetQ](http://easynetq.com/), a .NET API for RabbitMQ
 * Sprache appears in the [credits for JetBrains ReSharper](https://confluence.jetbrains.com/display/ReSharper/Third-Party+Software+Shipped+With+ReSharper#Third-PartySoftwareShippedWithReSharper-Sprache)

Background
----------

Parser combinators are covered extensively on the web. The original paper describing the monadic implementation by [Graham Hutton and Eric Meijer](http://www.cs.nott.ac.uk/~gmh/monparsing.pdf) is very readable. Sprache was originally written by [Nicholas Blumhardt](http://nblumhardt.com) and grew out of some exercises in [Hutton's Haskell book](http://www.amazon.com/Programming-Haskell-Graham-Hutton/dp/0521692695).

The implementation of Sprache draws on ideas from:

* [Luke Hoban's Blog](http://blogs.msdn.com/b/lukeh/archive/2007/08/19/monadic-parser-combinators-using-c-3-0.aspx)
* [Brian McNamara's Blog](http://lorgonblog.wordpress.com/2007/12/02/c-3-0-lambda-and-the-first-post-of-a-series-about-monadic-parser-combinators/)
