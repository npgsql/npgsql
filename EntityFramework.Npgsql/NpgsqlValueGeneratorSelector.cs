using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly NpgsqlSequenceValueGeneratorFactory _sequenceFactory;
        private readonly SimpleValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory;

        public NpgsqlValueGeneratorSelector(
            [NotNull] SimpleValueGeneratorFactory<GuidValueGenerator> guidFactory,
            [NotNull] SimpleValueGeneratorFactory<TemporaryIntegerValueGenerator> integerFactory,
            [NotNull] SimpleValueGeneratorFactory<TemporaryStringValueGenerator> stringFactory,
            [NotNull] SimpleValueGeneratorFactory<TemporaryBinaryValueGenerator> binaryFactory,
            [NotNull] NpgsqlSequenceValueGeneratorFactory sequenceFactory,
            [NotNull] SimpleValueGeneratorFactory<SequentialGuidValueGenerator> sequentialGuidFactory
            )
            : base(guidFactory, integerFactory, stringFactory, binaryFactory)
        {
            Check.NotNull(sequenceFactory, "sequenceFactory");
            Check.NotNull(sequentialGuidFactory, "sequentialGuidFactory");

            _sequenceFactory = sequenceFactory;
            _sequentialGuidFactory = sequentialGuidFactory;
        }

        public override ValueGenerator Select(IProperty property)
        {
            Check.NotNull(property, "property");

            var strategy = property.Npgsql().ValueGenerationStrategy
                           ?? property.EntityType.Model.Npgsql().ValueGenerationStrategy;

            if (property.PropertyType.IsInteger()
                && strategy == NpgsqlValueGenerationStrategy.Sequence)
            {
                return _sequenceFactory;
            }

            if (property.PropertyType == typeof(Guid))
            {
                return _sequentialGuidFactory;
            }

            return base.Select(property);
        }
    }
}
