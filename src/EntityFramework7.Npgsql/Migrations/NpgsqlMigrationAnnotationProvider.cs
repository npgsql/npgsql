using System;
using System.Collections.Generic;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using EntityFramework7.Npgsql.Metadata;

namespace EntityFramework7.Npgsql.Migrations
{
    public class NpgsqlMigrationAnnotationProvider : MigrationAnnotationProvider
    {
        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            if (property.StoreGeneratedPattern != StoreGeneratedPattern.None)
            {
                // TODO: The following is a workaround for https://github.com/aspnet/EntityFramework/issues/2539
                if (property.Npgsql().DefaultValueSql    == null &&
                    property.Npgsql().DefaultValue       == null &&
                    property.Npgsql().ComputedExpression == null)
                {
                    yield return new Annotation(NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration, property.StoreGeneratedPattern.ToString());
                }
            }
        }
    }
}
