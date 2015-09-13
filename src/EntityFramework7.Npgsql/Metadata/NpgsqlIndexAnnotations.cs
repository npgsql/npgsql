using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlIndexAnnotations : RelationalIndexAnnotations
    {
        public NpgsqlIndexAnnotations([NotNull] IIndex index)
            : base(index, NpgsqlAnnotationNames.Prefix)
        {
        }

        protected NpgsqlIndexAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations)
        {
        }

        /// <summary>
        /// The PostgreSQL index method to be used. Null selects the default (currently btree).
        /// </summary>
        /// <remarks>
        /// http://www.postgresql.org/docs/current/static/sql-createindex.html
        /// </remarks>
        public string Method
        {
            get { return (string) Annotations.GetAnnotation(NpgsqlAnnotationNames.IndexMethod); }
            set { Annotations.SetAnnotation(NpgsqlAnnotationNames.IndexMethod, value); }
        }
    }
}
