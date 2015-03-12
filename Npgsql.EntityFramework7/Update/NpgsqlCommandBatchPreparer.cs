using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlCommandBatchPreparer : CommandBatchPreparer
    {
        public NpgsqlCommandBatchPreparer(
            [NotNull] NpgsqlModificationCommandBatchFactory modificationCommandBatchFactory,
            [NotNull] ParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] ModificationCommandComparer modificationCommandComparer)
            : base(modificationCommandBatchFactory, parameterNameGeneratorFactory, modificationCommandComparer)
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