using System;
using EntityFramework.Npgsql.Metadata;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql
{
	public class NpgsqlValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly INpgsqlValueGeneratorCache _cache;
        private readonly NpgsqlSequenceValueGeneratorFactory _sequenceFactory;
        private readonly ValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory;
        private readonly NpgsqlEntityFrameworkConnection _connection;

        public NpgsqlValueGeneratorSelector(
            [NotNull] INpgsqlValueGeneratorCache cache,
            [NotNull] NpgsqlSequenceValueGeneratorFactory sequenceFactory,
            [NotNull] ValueGeneratorFactory<SequentialGuidValueGenerator> sequentialGuidFactory,
            [NotNull] NpgsqlEntityFrameworkConnection connection
            )
        {
            Check.NotNull(cache, nameof(cache));
            Check.NotNull(sequenceFactory, "sequenceFactory");
            Check.NotNull(sequentialGuidFactory, "sequentialGuidFactory");
            Check.NotNull(connection, "connection");

            _cache = cache;
            _sequenceFactory = sequenceFactory;
            _sequentialGuidFactory = sequentialGuidFactory;
            _connection = connection;
        }

        public override ValueGenerator Select(IProperty property)
        {
            Check.NotNull(property, "property");

            var strategy = property.Npgsql().ValueGenerationStrategy
                           ?? property.EntityType.Model.Npgsql().ValueGenerationStrategy;

            if (property.ClrType.IsInteger()
                && strategy == NpgsqlValueGenerationStrategy.Sequence)
            {
                return _sequenceFactory.Create(property, _cache.GetOrAddSequenceState(property), _connection);
            }

            if (property.ClrType == typeof(Guid))
            {
                return _sequentialGuidFactory.Create(property);
            }

            return _cache.GetOrAdd(property, Create);
        }
    }
}
