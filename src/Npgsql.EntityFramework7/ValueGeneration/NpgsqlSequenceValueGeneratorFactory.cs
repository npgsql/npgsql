// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace Npgsql.EntityFramework7.ValueGeneration
{
    public class NpgsqlSequenceValueGeneratorFactory : INpgsqlSequenceValueGeneratorFactory
    {
        private readonly ISqlStatementExecutor _executor;
        private readonly INpgsqlSqlGenerator _sqlGenerator;

        public NpgsqlSequenceValueGeneratorFactory(
            [NotNull] ISqlStatementExecutor executor,
            [NotNull] INpgsqlSqlGenerator sqlGenerator)
        {
            Check.NotNull(executor, nameof(executor));
            Check.NotNull(sqlGenerator, nameof(sqlGenerator));

            _executor = executor;
            _sqlGenerator = sqlGenerator;
        }

        public virtual ValueGenerator Create(IProperty property, NpgsqlSequenceValueGeneratorState generatorState, INpgsqlEFConnection connection)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(generatorState, nameof(generatorState));
            Check.NotNull(connection, nameof(connection));

            if (property.ClrType.UnwrapNullableType() == typeof(long))
            {
                return new NpgsqlSequenceValueGenerator<long>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(int))
            {
                return new NpgsqlSequenceValueGenerator<int>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(short))
            {
                return new NpgsqlSequenceValueGenerator<short>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(byte))
            {
                return new NpgsqlSequenceValueGenerator<byte>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(ulong))
            {
                return new NpgsqlSequenceValueGenerator<ulong>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(uint))
            {
                return new NpgsqlSequenceValueGenerator<uint>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(ushort))
            {
                return new NpgsqlSequenceValueGenerator<ushort>(_executor, _sqlGenerator, generatorState, connection);
            }

            if (property.ClrType.UnwrapNullableType() == typeof(sbyte))
            {
                return new NpgsqlSequenceValueGenerator<sbyte>(_executor, _sqlGenerator, generatorState, connection);
            }

            throw new ArgumentException(Microsoft.Data.Entity.Internal.Strings.InvalidValueGeneratorFactoryProperty(
                nameof(NpgsqlSequenceValueGeneratorFactory), property.Name, property.EntityType.DisplayName()));
        }
    }
}
