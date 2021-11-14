using System;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class MultiplexingTypeTestBase : MultiplexingTestBase
    {
        public async Task AssertType<T>(
            T value,
            string sqlLiteral,
            string pgTypeName,
            NpgsqlDbType npgsqlDbType,
            DbType? dbType = null,
            DbType? inferredDbType = null,
            bool isDefaultForReading = true,
            bool isDefaultForWriting = true,
            bool? isDefault = null)
        {
            if (isDefault is not null)
                isDefaultForReading = isDefaultForWriting = isDefault.Value;

            await using var connection = await OpenConnectionAsync();
            await TypeTestBase.AssertTypeRead(connection, sqlLiteral, pgTypeName, value, isDefaultForReading);
            await TypeTestBase.AssertTypeWrite(connection, value, sqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType,
                isDefaultForWriting);
        }

        public async Task AssertTypeRead<T>(string sqlLiteral, string pgTypeName, T expected, bool isDefault = false)
        {
            await using var connection = await OpenConnectionAsync();
            await TypeTestBase.AssertTypeRead(connection, sqlLiteral, pgTypeName, expected, isDefault);
        }

        public async Task AssertTypeWrite<T>(
            T value,
            string expectedSqlLiteral,
            string pgTypeName,
            NpgsqlDbType npgsqlDbType,
            DbType? dbType = null,
            DbType? inferredDbType = null,
            bool isDefault = true)
        {
            await using var connection = await OpenConnectionAsync();
            await TypeTestBase.AssertTypeWrite(connection, value, expectedSqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType,
                isDefault);
        }

        public async Task AssertTypeUnsupported<T>(T value, string sqlLiteral, string pgTypeName)
        {
            await AssertTypeUnsupportedRead<T>(sqlLiteral, pgTypeName);
            await AssertTypeUnsupportedWrite(value, pgTypeName);
        }

        public async Task AssertTypeUnsupportedRead(string sqlLiteral, string pgTypeName)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidCastException>());
        }

        public Task<Exception> AssertTypeUnsupportedRead<T>(string sqlLiteral, string pgTypeName)
            => AssertTypeUnsupportedRead<T, InvalidCastException>(sqlLiteral, pgTypeName);

        public async Task<Exception> AssertTypeUnsupportedRead<T, TException>(string sqlLiteral, string pgTypeName)
            where TException : Exception
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            return Assert.Throws<TException>(() => reader.GetFieldValue<T>(0))!;
        }

        public Task<InvalidCastException> AssertTypeUnsupportedWrite<T>(T value, string? pgTypeName = null)
            => AssertTypeUnsupportedWrite<T, InvalidCastException>(value, pgTypeName);

        public async Task<TException> AssertTypeUnsupportedWrite<T, TException>(T value, string? pgTypeName = null)
            where TException : Exception
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1", conn)
            {
                Parameters = { new() { Value = value } }
            };

            if (pgTypeName is not null)
                cmd.Parameters[0].DataTypeName = pgTypeName;

            return Assert.ThrowsAsync<TException>(() => cmd.ExecuteReaderAsync())!;
        }

        public MultiplexingTypeTestBase(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
