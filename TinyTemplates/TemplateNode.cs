using System.Collections.Generic;
using System.IO;

namespace TinyTemplates
{
    abstract class TemplateNode
    {
        public abstract void Execute(Stack<object> model, TextWriter output);
    }
}