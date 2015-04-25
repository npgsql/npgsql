// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlSequenceValueGenerator<TValue> : HiLoValueGenerator<TValue>
    {
        private readonly ISqlStatementExecutor _executor;
        private readonly INpgsqlSqlGenerator _sqlGenerator;
        private readonly INpgsqlEFConnection _connection;
        private readonly string _sequenceName;

        public NpgsqlSequenceValueGenerator(
            [NotNull] ISqlStatementExecutor executor,
            [NotNull] INpgsqlSqlGenerator sqlGenerator,
            [NotNull] NpgsqlSequenceValueGeneratorState generatorState,
            [NotNull] INpgsqlEFConnection connection)
            : base(Check.NotNull(generatorState, nameof(generatorState)))
        {
            Check.NotNull(executor, nameof(executor));
            Check.NotNull(sqlGenerator, nameof(sqlGenerator));
            Check.NotNull(connection, nameof(connection));

            _sequenceName = generatorState.SequenceName;
            _executor = executor;
            _sqlGenerator = sqlGenerator;
            _connection = connection;
        }

        protected override long GetNewLowValue()
        {
            var nextValue = _executor.ExecuteScalar(
                _connection,
                _connection.DbTransaction,
                _sqlGenerator.GenerateNextSequenceValueOperation(_sequenceName));

            return (long)Convert.ChangeType(nextValue, typeof(long), CultureInfo.InvariantCulture);
        }

        public override bool GeneratesTemporaryValues => false;
    }
}
