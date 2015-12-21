using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Scaffolding.Metadata;

namespace Microsoft.Data.Entity.Scaffolding.Metadata
{
    class NpgsqlColumnModel : ColumnModel
    {
        /// <summary>
        /// Identifies PostgreSQL serial columns.
        /// These columns are configured with a default value coming from a specifically-named sequence.
        /// </summary>
        /// <remarks>
        /// See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        /// </remarks>
        public bool IsSerial { get; set; }
    }
}
