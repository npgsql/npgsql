using Npgsql.Internal;
using NUnit.Framework;
using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class TypeMapperTests : TestBase
{
    [Test]
    public async Task ReloadTypes_across_connections_in_data_source()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        // Note that we don't actually create the type in the database at this point; we want to exercise the type being created later,
        // via the data source.

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection1 = await dataSource.OpenConnectionAsync();
        await using var connection2 = await dataSource.OpenConnectionAsync();

        await connection1.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await connection1.ReloadTypesAsync();

        // The data source type mapper has been replaced and connection1 should have the new mapper, but connection2 should retain the older
        // type mapper - where there's no mapping - as long as it's still open
        Assert.ThrowsAsync<InvalidCastException>(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
        Assert.DoesNotThrowAsync(async () => await connection1.ExecuteScalarAsync($"SELECT 'happy'::{type}"));

        // Close connection2 and reopen to make sure it picks up the new type and mapping from the data source
        var connId = connection2.ProcessID;
        await connection2.CloseAsync();
        await connection2.OpenAsync();
        Assert.That(connection2.ProcessID, Is.EqualTo(connId), "Didn't get the same connector back");

        Assert.DoesNotThrowAsync(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
    }

    [Test]
    [NonParallelizable] // Depends on citext which could be dropped concurrently
    public async Task String_to_citext()
    {
        await using var adminConnection = await OpenConnectionAsync();
        await EnsureExtensionAsync(adminConnection, "citext");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new CitextToStringTypeHandlerResolverFactory());
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand("SELECT @p = 'hello'::citext", connection);
        command.Parameters.AddWithValue("p", "HeLLo");
        Assert.That(command.ExecuteScalar(), Is.True);
    }

    [Test]
    [NonParallelizable] // Depends on citext which could be dropped concurrently
    public async Task String_to_citext_with_db_type_string()
    {
        await using var adminConnection = await OpenConnectionAsync();
        await EnsureExtensionAsync(adminConnection, "citext");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new ForceStringToCitextResolverFactory());
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand("SELECT @p = 'hello'::citext", connection);
        var parameter = new NpgsqlParameter("p", DbType.String)
        {
            Value = "HeLLo"
        };
        command.Parameters.Add(parameter);

        Assert.That(command.ExecuteScalar(), Is.True);
        Assert.That(parameter.DbType, Is.EqualTo(DbType.String));
        Assert.That(parameter.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Citext));
        Assert.That(parameter.DataTypeName, Is.EqualTo("citext"));
    }

    [Test]
    public async Task Guid_to_custom_type()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new GuidTextConverterFactory(type));
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await connection.ExecuteNonQueryAsync($"CREATE TYPE {type}");
        await connection.ExecuteNonQueryAsync($"""
            -- Input: cstring -> Custom type
            CREATE FUNCTION {type}_in(cstring)
            RETURNS {type}
            AS 'textin'
            LANGUAGE internal IMMUTABLE STRICT;

            -- Output: Custom type -> cstring
            CREATE FUNCTION {type}_out({type})
            RETURNS cstring
            AS 'textout'
            LANGUAGE internal IMMUTABLE STRICT;

            -- 3️⃣ Create wrappers for binary I/O
            CREATE FUNCTION {type}_recv(internal)
            RETURNS {type}
            AS 'textrecv'
            LANGUAGE internal IMMUTABLE STRICT;

            CREATE FUNCTION {type}_send({type})
            RETURNS bytea
            AS 'textsend'
            LANGUAGE internal IMMUTABLE STRICT;
        """);

        await connection.ExecuteNonQueryAsync($"""
            CREATE TYPE {type} (
                internallength = variable,
                input = {type}_in,
                output = {type}_out,
                receive = {type}_recv,
                send = {type}_send,
                alignment = int4
            );
            CREATE CAST ({type} AS text) WITH INOUT AS IMPLICIT;
            """);
        await connection.ReloadTypesAsync();

        var guid = Guid.NewGuid();
        await using var command = new NpgsqlCommand($"SELECT @p::text = '{guid}'", connection);
        var parameter = new NpgsqlParameter("p", DbType.Guid)
        {
            Value = guid
        };
        command.Parameters.Add(parameter);

        Assert.That(command.ExecuteScalar(), Is.True);
        Assert.That(parameter.DbType, Is.EqualTo(DbType.Guid));
        Assert.That(parameter.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
        Assert.That(parameter.DataTypeName, Is.EqualTo(type));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4582")]
    [NonParallelizable] // Drops extension
    public async Task Type_in_non_default_schema()
    {
        await using var conn = await OpenConnectionAsync();

        var schemaName = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync(@$"
DROP EXTENSION IF EXISTS citext;
CREATE EXTENSION citext SCHEMA ""{schemaName}""");

        try
        {
            await conn.ReloadTypesAsync();

            var tableName = await CreateTempTable(conn, $"created_by {schemaName}.citext NOT NULL");

            const string expected = "SomeValue";
            await conn.ExecuteNonQueryAsync($"INSERT INTO \"{tableName}\" VALUES('{expected}')");

            var value = (string?)await conn.ExecuteScalarAsync($"SELECT created_by FROM \"{tableName}\" LIMIT 1");
            Assert.That(value, Is.EqualTo(expected));
        }
        finally
        {
            await conn.ExecuteNonQueryAsync(@"DROP EXTENSION citext CASCADE");
        }
    }

    #region Support

    class CitextToStringTypeHandlerResolverFactory : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new Resolver();
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        sealed class Resolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (type == typeof(string) || dataTypeName?.UnqualifiedName == "citext")
                    if (options.DatabaseInfo.TryGetPostgresTypeByName("citext", out var pgType))
                        return new(options, new StringTextConverter(options.TextEncoding), options.ToCanonicalTypeId(pgType));

                return null;
            }
        }

    }

    class ForceStringToCitextResolverFactory : CitextToStringTypeHandlerResolverFactory
    {
        public override IDbTypeResolver? CreateDbTypeResolver() => new DbTypeResolver();

        sealed class DbTypeResolver : IDbTypeResolver
        {
            public string? GetDataTypeName(DbType dbType, PgSerializerOptions options)
            {
                if (dbType == DbType.String)
                    return "citext";

                return null;
            }

            public DbType? GetDbType(DataTypeName dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName.UnqualifiedName == "citext")
                    return DbType.String;

                return null;
            }
        }
    }

    class GuidTextConverterFactory(string typeName) : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;
        public override IPgTypeInfoResolver CreateResolver() => new GuidTextTypeInfoResolver(typeName);
        public override IDbTypeResolver? CreateDbTypeResolver() => new DbTypeResolver(typeName);

        sealed class GuidTextTypeInfoResolver(string typeName) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (type == typeof(Guid) || dataTypeName?.UnqualifiedName == typeName)
                    if (options.DatabaseInfo.TryGetPostgresTypeByName(typeName, out var pgType))
                        return new(options, new GuidTextConverter(options.TextEncoding), options.ToCanonicalTypeId(pgType));

                return null;
            }
        }

        sealed class GuidTextConverter(System.Text.Encoding encoding) : StringBasedTextConverter<Guid>(encoding)
        {
            public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
            {
                bufferRequirements = BufferRequirements.None;
                return format is DataFormat.Text;
            }
            protected override Guid ConvertFrom(string value) => Guid.Parse(value);
            protected override ReadOnlyMemory<char> ConvertTo(Guid value) => value.ToString().AsMemory();
        }

        sealed class DbTypeResolver(string typeName) : IDbTypeResolver
        {
            public string? GetDataTypeName(DbType dbType, PgSerializerOptions options)
            {
                if (dbType == DbType.Guid)
                    return typeName;
                return null;
            }

            public DbType? GetDbType(DataTypeName dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName.UnqualifiedName == typeName)
                    return DbType.Guid;
                return null;
            }
        }
    }

    enum Mood { Sad, Ok, Happy }

    #endregion Support
}
