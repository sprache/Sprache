using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TinyTemplates
{
    class IterationTemplateNode : TemplateNode
    {
        readonly TemplateMemberAccessor _templateMemberAccessor;
        readonly TemplateNode _content;

        public IterationTemplateNode(TemplateMemberAccessor templateMemberAccessor, TemplateNode content)
        {
            _templateMemberAccessor = templateMemberAccessor;
            _content = content;
        }

        public override void Execute(Stack<object> model, TextWriter output)
        {
            var m = _templateMemberAccessor.GetMember(model);

            foreach (var i in (IEnumerable)m)
            {
                model.Push(i);
                _content.Execute(model, output);
                model.Pop();
            }
        }
    }
}