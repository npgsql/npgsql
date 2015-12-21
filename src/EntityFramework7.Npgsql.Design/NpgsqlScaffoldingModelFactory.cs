using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Scaffolding.Internal;
using Microsoft.Data.Entity.Scaffolding.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Extensions.Logging;

namespace Microsoft.Data.Entity.Scaffolding
{
    public class NpgsqlScaffoldingModelFactory : RelationalScaffoldingModelFactory
    {
        public NpgsqlScaffoldingModelFactory(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] IDatabaseModelFactory databaseModelFactory)
            : base(loggerFactory, typeMapper, databaseModelFactory)
        {
        }

        public override IModel Create(string connectionString, TableSelectionSet tableSelectionSet)
        {
            var model = base.Create(connectionString, tableSelectionSet);
            model.Scaffolding().UseProviderMethodName = nameof(NpgsqlDbContextOptionsExtensions.UseNpgsql);
            return model;
        }

        [CanBeNull]
        protected override KeyBuilder VisitPrimaryKey(EntityTypeBuilder builder, TableModel table)
        {
            var keyBuilder = base.VisitPrimaryKey(builder, table);

            if (keyBuilder == null)
            {
                return null;
            }

            // If this property is the single integer primary key on the EntityType then
            // KeyConvention assumes ValueGeneratedOnAdd(). If the underlying column does
            // not have Serial set then we need to set to ValueGeneratedNever() to
            // override this behavior.

            // TODO use KeyConvention directly to detect when it will be applied
            var pkColumns = table.Columns.Where(c => c.PrimaryKeyOrdinal.HasValue).Cast<NpgsqlColumnModel>().ToList();
            if (pkColumns.Count != 1 || pkColumns[0].IsSerial)
            {
                return keyBuilder;
            }

            // TODO 
            var property = builder.Metadata.FindProperty(GetPropertyName(pkColumns[0]));
            var propertyType = property?.ClrType?.UnwrapNullableType();

            if (propertyType?.IsIntegerForSerial() == true || propertyType == typeof(Guid))
            {
                property.ValueGenerated = ValueGenerated.Never;
            }

            return keyBuilder;
        }

        [CanBeNull]
        protected override IndexBuilder VisitIndex(EntityTypeBuilder builder, IndexModel index)
        {
            var npgsqlIndex = (NpgsqlIndexModel)index;
            if (npgsqlIndex.Expression != null)
            {
                Logger.LogWarning($"Ignoring unsupported index {index.Name} which contains an expression ({npgsqlIndex.Expression})");
                return null;
            }

            return base.VisitIndex(builder, index);
        }
    }
}
