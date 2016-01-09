using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Scaffolding.Metadata;

namespace Microsoft.Data.Entity.Scaffolding.Metadata
{
    public class NpgsqlIndexModel : IndexModel
    {
        /// <summary>
        /// If the index contains an expression (rather than simple column references), the expression is contained here.
        /// This is currently unsupported and will be ignored.
        /// </summary>
        public string Expression { get; set; }
    }
}
