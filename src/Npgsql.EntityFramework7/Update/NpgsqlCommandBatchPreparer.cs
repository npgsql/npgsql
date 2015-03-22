using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Update
{
    public class NpgsqlCommandBatchPreparer : CommandBatchPreparer
    {
        public NpgsqlCommandBatchPreparer(
            [NotNull] NpgsqlModificationCommandBatchFactory modificationCommandBatchFactory,
            [NotNull] ParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] ModificationCommandComparer modificationCommandComparer,
            [NotNull] IBoxedValueReaderSource boxedValueReaderSource)
            : base(modificationCommandBatchFactory, parameterNameGeneratorFactory, modificationCommandComparer, boxedValueReaderSource)
        {
        }

        public override IRelationalPropertyExtensions GetPropertyExtensions(IProperty property)
        {
            Check.NotNull(property, "property");

            return property.Npgsql();
        }

        public override IRelationalEntityTypeExtensions GetEntityTypeExtensions(IEntityType entityType)
        {
            Check.NotNull(entityType, "entityType");

            return entityType.Npgsql();
        }
    }
}