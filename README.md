# Sprache [![Sprache tag on Stack Overflow](https://img.shields.io/badge/stackoverflow-sprache-orange.svg)](http://stackoverflow.com/questions/tagged/sprache) [![Join the chat at https://gitter.im/sprache/Sprache](https://badges.gitter.im/sprache/Sprache.svg)](https://gitter.im/sprache/Sprache?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet](https://img.shields.io/nuget/v/Sprache.svg)](https://nuget.org/packages/Sprache) [![Build status](https://ci.appveyor.com/api/projects/status/xrn2d7b9crqj8l4a?svg=true)](https://ci.appveyor.com/project/Sprache/sprache)

Sprache is a simple, lightweight library for constructing parsers directly in C# code.

It doesn't compete with "industrial strength" language workbenches - it fits somewhere in between regular expressions and a full-featured toolset like [ANTLR](http://antlr.org).

![Sprache](https://avatars1.githubusercontent.com/u/1999078?v=3&s=200)

### Usage

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
    from first in Parse.Letter.Once().Text()
    from rest in Parse.LetterOrDigit.Many().Text()
    from trailing in Parse.WhiteSpace.Many()
    select first + rest;

var id = identifier.Parse(" abc123  ");

Assert.AreEqual("abc123", id);
```

### Examples and Tutorials

The best place to start is [this introductory article](http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/).

**Examples** included with the source demonstrate:

* [Parsing XML directly to a Document object](https://github.com/sprache/Sprache/blob/master/samples/XmlExample/Program.cs)
* [Parsing numeric expressions to `System.Linq.Expression` objects](https://github.com/sprache/Sprache/blob/master/samples/LinqyCalculator/ExpressionParser.cs)
* [Parsing comma-separated values (CSV) into lists of strings](https://github.com/sprache/Sprache/blob/master/test/Sprache.Tests/Scenarios/CsvTests.cs)

**Tutorials** explaining Sprache:
 * Overview of Sprache methods, [long guide by Justing Pealing](https://justinpealing.me.uk/post/2020-03-11-sprache1-chars/)
 * A great [CodeProject article by Alexey Yakovlev ](http://www.codeproject.com/Articles/795056/Sprache-Calc-building-yet-another-expression-evalu) (and [in Russian](http://habrahabr.ru/post/228037/))
 * Mike Hadlow's example of [parsing connection strings](http://mikehadlow.blogspot.com.au/2012/09/parsing-connection-string-with-sprache.html)
 * Alexey Golub's article on [monadic parser combinators](https://tyrrrz.me/blog/monadic-parser-combinators) that shows how to build a JSON parser using Sprache

**Real-world** parsers built with Sprache:

 * The [template parser](https://github.com/OctopusDeploy/Octostache/blob/master/source/Octostache/Templates/TemplateParser.cs) in [Octostache](https://github.com/OctopusDeploy/Octostache), the variable substitution language of [Octopus Deploy](https://octopus.com)
 * The [XAML binding expression parser](https://github.com/OmniGUI/OmniXAML/blob/master/OmniXaml/InlineParsers/Extensions/MarkupExtensionParser.cs) in [OmniXaml](https://github.com/OmniGUI/OmniXAML), the cross-platform XAML implementation
 * Parts of the filter expression parser in [Seq](https://datalust.co/seq), a structured log server for .NET
 * The [connection string parser](https://github.com/EasyNetQ/EasyNetQ/blob/master/Source/EasyNetQ/ConnectionString/ConnectionStringGrammar.cs) in [EasyNetQ](http://easynetq.com/), a .NET API for RabbitMQ
 * [ApexSharp parser](https://github.com/apexsharp/apexparser/blob/master/ApexSharp.ApexParser/Parser/ApexGrammar.cs), a two-way [Apex to C# transpiler](https://github.com/apexsharp/apexparser) (Salesforce programming language)
 * Sprache appears in the [credits for JetBrains ReSharper](https://confluence.jetbrains.com/display/ReSharper/Third-Party+Software+Shipped+With+ReSharper#Third-PartySoftwareShippedWithReSharper-Sprache)

### Background

Parser combinators are covered extensively on the web. The original paper describing the monadic implementation by [Graham Hutton and Eric Meijer](http://www.cs.nott.ac.uk/~gmh/monparsing.pdf) is very readable. Sprache was originally written by [Nicholas Blumhardt](http://nblumhardt.com) and grew out of some exercises in [Hutton's Haskell book](http://www.amazon.com/Programming-Haskell-Graham-Hutton/dp/0521692695).

The implementation of Sprache draws on ideas from:

* [Luke Hoban's Blog](http://blogs.msdn.com/b/lukeh/archive/2007/08/19/monadic-parser-combinators-using-c-3-0.aspx)
* [Brian McNamara's Blog](http://lorgonblog.wordpress.com/2007/12/02/c-3-0-lambda-and-the-first-post-of-a-series-about-monadic-parser-combinators/)
