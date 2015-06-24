// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.ValueGeneration;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;
using Npgsql.EntityFramework7.Metadata;

namespace Npgsql.EntityFramework7.ValueGeneration
{
    public class NpgsqlValueGeneratorSelector : RelationalValueGeneratorSelector
    {
        private readonly INpgsqlSequenceValueGeneratorFactory _sequenceFactory;

        private readonly ValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory
            = new ValueGeneratorFactory<SequentialGuidValueGenerator>();

        private readonly NpgsqlDatabaseConnection  _connection;

        public NpgsqlValueGeneratorSelector(
            [NotNull] INpgsqlValueGeneratorCache cache,
            [NotNull] INpgsqlSequenceValueGeneratorFactory sequenceFactory,
            [NotNull] NpgsqlDatabaseConnection connection)
            : base(cache)
        {
            Check.NotNull(sequenceFactory, nameof(sequenceFactory));
            Check.NotNull(connection, nameof(connection));

            _sequenceFactory = sequenceFactory;
            _connection = connection;
        }

        public new virtual INpgsqlValueGeneratorCache Cache => (INpgsqlValueGeneratorCache)base.Cache;

        public override ValueGenerator Select(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            var strategy = property.Npgsql().ValueGenerationStrategy;

            return property.ClrType.IsInteger()
                   && strategy == NpgsqlValueGenerationStrategy.Sequence
                ? _sequenceFactory.Create(property, Cache.GetOrAddSequenceState(property), _connection)
                : Cache.GetOrAdd(property, entityType, Create);
        }

        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            return property.ClrType == typeof(Guid)
                ? _sequentialGuidFactory.Create(property)
                : base.Create(property, entityType);
        }
    }
}
