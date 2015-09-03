using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using EntityFramework7.Npgsql.Metadata;
using Microsoft.Data.Entity.Migrations;

namespace EntityFramework7.Npgsql.Migrations
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
    }
}
