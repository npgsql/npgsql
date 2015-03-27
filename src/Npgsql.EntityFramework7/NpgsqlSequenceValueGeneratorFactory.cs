// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlSequenceValueGeneratorFactory
    {
        private readonly SqlStatementExecutor _executor;

        public NpgsqlSequenceValueGeneratorFactory([NotNull] SqlStatementExecutor executor)
        {
            Check.NotNull(executor, nameof(executor));

            _executor = executor;
        }

        public virtual ValueGenerator Create(
            [NotNull] IProperty property, 
            [NotNull] NpgsqlSequenceValueGeneratorState generatorState, 
            [NotNull] INpgsqlConnection connection)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(generatorState, nameof(generatorState));
            Check.NotNull(connection, nameof(connection));

            if (property.ClrType.UnwrapNullableType() == typeof(long))
            {
                return new NpgsqlSequenceValueGenerator<long>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(int))
            {
                return new NpgsqlSequenceValueGenerator<int>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(short))
            {
                return new NpgsqlSequenceValueGenerator<short>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(byte))
            {
                return new NpgsqlSequenceValueGenerator<byte>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(ulong))
            {
                return new NpgsqlSequenceValueGenerator<ulong>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(uint))
            {
                return new NpgsqlSequenceValueGenerator<uint>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(ushort))
            {
                return new NpgsqlSequenceValueGenerator<ushort>(_executor, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(sbyte))
            {
                return new NpgsqlSequenceValueGenerator<sbyte>(_executor, generatorState, connection);
            }

            throw new ArgumentException(Microsoft.Data.Entity.Internal.Strings.InvalidValueGeneratorFactoryProperty(
                nameof(NpgsqlSequenceValueGeneratorFactory), property.Name, property.EntityType.DisplayName()));
        }
    }
}
