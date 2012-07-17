using System.Collections.Generic;
using System.IO;

namespace TinyTemplates
{
    class MemberAccessTemplateNode : TemplateNode
    {
        readonly TemplateMemberAccessor _member;

        public MemberAccessTemplateNode(TemplateMemberAccessor member)
        {
            _member = member;
        }

        public override void Execute(Stack<object> model, TextWriter output)
        {
            output.Write(_member.GetMember(model));
        }
    }
}