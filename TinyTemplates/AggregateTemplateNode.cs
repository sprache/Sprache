using System.Collections.Generic;
using System.IO;

namespace TinyTemplates
{
    class AggregateTemplateNode : TemplateNode
    {
        readonly IEnumerable<TemplateNode> _content;

        public AggregateTemplateNode(IEnumerable<TemplateNode> content)
        {
            _content = content;
        }

        public override void Execute(Stack<object> model, TextWriter output)
        {
            foreach (var element in _content)
                element.Execute(model, output);
        }
    }
}