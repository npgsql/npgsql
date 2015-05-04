using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This file was injected into your project by the AsyncRewriter package

namespace AsyncRewriter
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class RewriteAsyncAttribute : Attribute
    {
        public RewriteAsyncAttribute(bool withOverride = false) { }
    }
}
