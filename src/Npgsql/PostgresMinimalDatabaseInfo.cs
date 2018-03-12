#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.DateTimeHandlers;
using Npgsql.TypeHandlers.NumericHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    class PostgresMinimalDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        public Task<NpgsqlDatabaseInfo> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
            => Task.FromResult(
                new NpgsqlConnectionStringBuilder(conn.ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.NoTypeLoading
                    ? (NpgsqlDatabaseInfo)new PostgresMinimalDatabaseInfo()
                    : null
            );
    }

    class PostgresMinimalDatabaseInfo : PostgresDatabaseInfo
    {
        static readonly PostgresType[] Types = new[]
        {
            new PostgresBaseType("pg_catalog", "int4",        (uint)NpgsqlDbType.Integer),
            new PostgresBaseType("pg_catalog", "int2",        (uint)NpgsqlDbType.Smallint),
            new PostgresBaseType("pg_catalog", "int8",        (uint)NpgsqlDbType.Bigint),
            new PostgresBaseType("pg_catalog", "float4",      (uint)NpgsqlDbType.Real),
            new PostgresBaseType("pg_catalog", "float8",      (uint)NpgsqlDbType.Double),
            new PostgresBaseType("pg_catalog", "numeric",     (uint)NpgsqlDbType.Numeric),
            new PostgresBaseType("pg_catalog", "money",       (uint)NpgsqlDbType.Money),

            new PostgresBaseType("pg_catalog", "text",        (uint)NpgsqlDbType.Text),
            new PostgresBaseType("pg_catalog", "varchar",     (uint)NpgsqlDbType.Varchar),
            new PostgresBaseType("pg_catalog", "char",        (uint)NpgsqlDbType.Char),
            new PostgresBaseType("pg_catalog", "unknown",     (uint)NpgsqlDbType.Unknown),

            new PostgresBaseType("pg_catalog", "timestamp",   (uint)NpgsqlDbType.Timestamp),
            new PostgresBaseType("pg_catalog", "timestamptz", (uint)NpgsqlDbType.TimestampTz),
            new PostgresBaseType("pg_catalog", "date",        (uint)NpgsqlDbType.Date),
            new PostgresBaseType("pg_catalog", "time",        (uint)NpgsqlDbType.Time),
            new PostgresBaseType("pg_catalog", "timetz",      (uint)NpgsqlDbType.TimeTz),
            new PostgresBaseType("pg_catalog", "interval",    (uint)NpgsqlDbType.Interval),

            new PostgresBaseType("pg_catalog", "bool",        (uint)NpgsqlDbType.Boolean),
            new PostgresBaseType("pg_catalog", "bytea",       (uint)NpgsqlDbType.Bytea),
            new PostgresBaseType("pg_catalog", "uuid",        (uint)NpgsqlDbType.Uuid)
        };

        protected override IEnumerable<PostgresType> GetTypes() => Types;
    }
}
