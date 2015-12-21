using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Migrations.Internal
{
    public class NpgsqlMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            if (property.ValueGenerated == ValueGenerated.OnAdd &&
                property.ClrType.IsIntegerForSerial()) {
                yield return new Annotation(NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Serial, true);
            }

            // TODO: Named sequences

            // TODO: We don't support ValueGenerated.OnAddOrUpdate, so should we throw an exception?
            // Other providers don't seem to...
        }

        public override IEnumerable<IAnnotation> For(IIndex index)
        {
            if (index.Npgsql().Method != null)
            {
                yield return new Annotation(
                     NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.IndexMethod,
                     index.Npgsql().Method);
            }
        }
    }
}
