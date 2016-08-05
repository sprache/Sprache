using System.Linq;
using Sprache;

namespace TinyTemplates
{
    static class TemplateParser
    {
        static readonly Parser<char> Hash = Parse.Char('#');

        static readonly Parser<TemplateNode> EscapedHash =
            Hash.Select(h => new LiteralTemplateNode(h.ToString()))
            .Named("escaped '#' character");

        static readonly Parser<string> Identifier =
            from first in Parse.Letter.Once().Text()
            from rest in Parse.LetterOrDigit.Many().Text()
            select first + rest;

        static readonly Parser<TemplateMemberAccessor> Member =
            from first in Identifier.Once()
            from subs in Parse.Char('.').Then(_ => Identifier).Many()
            select new TemplateMemberAccessor(first.Concat(subs));

        static readonly Parser<TemplateNode> FreeSymbol =
            Member.Select(m => new MemberAccessTemplateNode(m));

        static Parser<T> OptionallyDelimited<T>(Parser<T> p)
        {
            return (from open in Parse.Char('{')
                    from inner in p
                    from close in Parse.Char('}').Named("closing brace")
                    select inner).XOr(p);
        }

        static readonly Parser<TemplateNode> Symbol =
            OptionallyDelimited(FreeSymbol)
            .Named("replacement directive");

        static readonly Parser<TemplateNode> Literal =
            Parse.AnyChar.Except(Hash).AtLeastOnce().Text()
            .Select(t => new LiteralTemplateNode(t));

        static readonly Parser<TemplateNode> Iteration =
            (from i in OptionallyDelimited(Parse.Char('|').Then(_ => Member))
// ReSharper disable StaticFieldInitializersReferesToFieldBelow
             from content in Parse.Ref(() => Aggregate)
// ReSharper restore StaticFieldInitializersReferesToFieldBelow
             from end in Parse.String("#.")
             select new IterationTemplateNode(i, content)).Named("iteration directive");

        static readonly Parser<TemplateNode> Directive = Hash
            .Then(_ => EscapedHash.Or(Iteration).Or(Symbol));

        static readonly Parser<TemplateNode> Element = Literal.XOr(Directive);

        static readonly Parser<TemplateNode> Aggregate =
            Element.Many().Select(ee => new AggregateTemplateNode(ee));

        public static TemplateNode ParseTemplate(string templateText)
        {
            return Aggregate.End().Parse(templateText);
        }
    }
}
