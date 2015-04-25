// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;
using Npgsql.EntityFramework7.Metadata;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlValueGeneratorSelector : ValueGeneratorSelector, INpgsqlValueGeneratorSelector
    {
        private readonly INpgsqlValueGeneratorCache _cache;
        private readonly INpgsqlSequenceValueGeneratorFactory _sequenceFactory;

        private readonly ValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory 
            = new ValueGeneratorFactory<SequentialGuidValueGenerator>();

        private readonly INpgsqlEFConnection _connection;

        public NpgsqlValueGeneratorSelector(
            [NotNull] INpgsqlValueGeneratorCache cache,
            [NotNull] INpgsqlSequenceValueGeneratorFactory sequenceFactory,
            [NotNull] INpgsqlEFConnection connection)
        {
            Check.NotNull(cache, nameof(cache));
            Check.NotNull(sequenceFactory, nameof(sequenceFactory));
            Check.NotNull(connection, nameof(connection));

            _cache = cache;
            _sequenceFactory = sequenceFactory;
            _connection = connection;
        }

        public override ValueGenerator Select(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var strategy = property.Npgsql().ValueGenerationStrategy;

            if (property.ClrType.IsInteger()
                && strategy == NpgsqlValueGenerationStrategy.Sequence)
            {
                return _sequenceFactory.Create(property, _cache.GetOrAddSequenceState(property), _connection);
            }

            return _cache.GetOrAdd(property, Create);
        }

        public override ValueGenerator Create(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            return property.ClrType == typeof(Guid) 
                ? _sequentialGuidFactory.Create(property) 
                : base.Create(property);
        }
    }
}
