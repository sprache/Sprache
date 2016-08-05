using System;
using System.Collections.Generic;
using System.IO;

namespace TinyTemplates
{
    /// <summary>
    /// Text templating engine in a file.
    /// </summary>
    /// <remarks>
    /// #field -> value of model property 'field'
    /// #{field} -> disambiguate, works with any expression
    /// #field.field -> traverse property of property
    /// ## -> #
    /// #|field (content)  #. -> make items of enumerable
    ///                          property field into model wrt content
    /// #^field -> disambiguate when child and parent fields have same
    ///            name (use parent)
    /// #$ -> the model itself
    /// </remarks>
    public class Template
    {
        readonly TemplateNode _root;

        public Template(string templateText)
        {
            if (templateText == null) throw new ArgumentNullException(nameof(templateText));
            _root = TemplateParser.ParseTemplate(templateText);
        }

        public void Execute(object model, TextWriter output)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (output == null) throw new ArgumentNullException(nameof(output));
            var modelStack = new Stack<object>();
            modelStack.Push(model);
            _root.Execute(modelStack, output);
        }

        public string Execute(object model)
        {
            var sw = new StringWriter();
            Execute(model, sw);
            return sw.ToString();
        }
    }
}
