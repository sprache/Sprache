using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache.Union
{
    public class UnionResult<T>
    {
        public List<UnionResultValue<T>> Values { get; set; }

        public string ErrorMessage { get; set; }

        public bool WasSuccessful
        {
            get
            {
                return Values.Any(v => v.WasSuccessful);
            }
        }
    }
}
