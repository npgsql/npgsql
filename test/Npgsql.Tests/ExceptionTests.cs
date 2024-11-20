using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class ExceptionTests : TestBase
{
    [Test, Description("Generates a basic server-side exception, checks that it's properly raised and populated")]
    public void Basic()
    {
        // Make sure messages are in English
        using var dataSource = CreateDataSource(csb => csb.Options = "-c lc_messages=en_US.UTF-8");
        using var conn = dataSource.OpenConnection();
        conn.ExecuteNonQuery(
"""
CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
   'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345'', DETAIL = ''testdetail''; END;'
LANGUAGE 'plpgsql';
""");

        PostgresException ex = null!;
        try
        {
            conn.ExecuteNonQuery("SELECT pg_temp.emit_exception()");
            Assert.Fail("No exception was thrown");
        }
        catch (PostgresException e)
        {
            ex = e;
        }

        Assert.That(ex.MessageText, Is.EqualTo("testexception"));
        Assert.That(ex.Severity, Is.EqualTo("ERROR"));
        Assert.That(ex.InvariantSeverity, Is.EqualTo("ERROR"));
        Assert.That(ex.SqlState, Is.EqualTo("12345"));
        Assert.That(ex.Position, Is.EqualTo(0));
        Assert.That(ex.Message, Does.StartWith("12345: testexception"));

        var data = ex.Data;
        Assert.That(data[nameof(PostgresException.Severity)], Is.EqualTo("ERROR"));
        Assert.That(data[nameof(PostgresException.SqlState)], Is.EqualTo("12345"));
        Assert.That(data.Contains(nameof(PostgresException.Position)), Is.False);

        var exString = ex.ToString();
        Assert.That(exString, Does.StartWith("Npgsql.PostgresException (0x80004005): 12345: testexception"));
        Assert.That(exString, Contains.Substring(nameof(PostgresException.Severity) + ": ERROR"));
        Assert.That(exString, Contains.Substring(nameof(PostgresException.SqlState) + ": 12345"));

        Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1), "Connection in bad state after an exception");
    }

    [Test, Description("Ensures Detail is redacted by default in PostgresException and PostgresNotice")]
    public async Task Error_details_are_redacted()
    {
        await using var conn = await OpenConnectionAsync();
        var raiseExceptionFunc = await GetTempFunctionName(conn);
        var raiseNoticeFunc = await GetTempFunctionName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE OR REPLACE FUNCTION {raiseExceptionFunc}() RETURNS VOID AS $$
BEGIN
    RAISE EXCEPTION 'testexception' USING DETAIL = 'secret';
END;
$$ LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION {raiseNoticeFunc}() RETURNS VOID AS $$
BEGIN
    RAISE NOTICE 'testexception' USING DETAIL = 'secret';
END;
$$ LANGUAGE 'plpgsql';");

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync($"SELECT * FROM {raiseExceptionFunc}()"))!;
        Assert.That(ex.Detail, Does.Not.Contain("secret"));
        Assert.That(ex.Message, Does.Not.Contain("secret"));
        Assert.That(ex.Data[nameof(PostgresException.Detail)], Does.Not.Contain("secret"));
        Assert.That(ex.ToString(), Does.Not.Contain("secret"));

        PostgresNotice? notice = null;
        conn.Notice += (___, a) => notice = a.Notice;
        await conn.ExecuteNonQueryAsync($"SELECT * FROM {raiseNoticeFunc}()");
        Assert.That(notice!.Detail, Does.Not.Contain("secret"));
    }

    [Test]
    public async Task IncludeErrorDetail()
    {
        await using var dataSource = CreateDataSource(csb => csb.IncludeErrorDetail = true);
        await using var conn = await dataSource.OpenConnectionAsync();
        var raiseExceptionFunc = await GetTempFunctionName(conn);
        var raiseNoticeFunc = await GetTempFunctionName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE OR REPLACE FUNCTION {raiseExceptionFunc}() RETURNS VOID AS $$
BEGIN
    RAISE EXCEPTION 'testexception' USING DETAIL = 'secret';
END;
$$ LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION {raiseNoticeFunc}() RETURNS VOID AS $$
BEGIN
    RAISE NOTICE 'testexception' USING DETAIL = 'secret';
END;
$$ LANGUAGE 'plpgsql';");

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync($"SELECT * FROM {raiseExceptionFunc}()"))!;
        Assert.That(ex.Detail, Does.Contain("secret"));
        Assert.That(ex.Message, Does.Contain("secret"));
        Assert.That(ex.Data[nameof(PostgresException.Detail)], Does.Contain("secret"));
        Assert.That(ex.ToString(), Does.Contain("secret"));

        PostgresNotice? notice = null;
        conn.Notice += (____, a) => notice = a.Notice;
        await conn.ExecuteNonQueryAsync($"SELECT * FROM {raiseNoticeFunc}()");
        Assert.That(notice!.Detail, Does.Contain("secret"));
    }

    [Test]
    public async Task Error_position()
    {
        await using var conn = await OpenConnectionAsync();

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync("SELECT 1; SELECT * FROM \"NonExistingTable\""))!;
        Assert.That(ex.Message, Does.Contain("POSITION: 15"));
    }

    [Test]
    public void Exception_fields_are_populated()
    {
        using var conn = OpenConnection();
        TestUtil.MinimumPgVersion(conn, "9.3.0", "5 error fields haven't been added yet");
        conn.ExecuteNonQuery("CREATE TEMP TABLE uniqueviolation (id INT NOT NULL, CONSTRAINT uniqueviolation_pkey PRIMARY KEY (id))");
        conn.ExecuteNonQuery("INSERT INTO uniqueviolation (id) VALUES(1)");
        try
        {
            conn.ExecuteNonQuery("INSERT INTO uniqueviolation (id) VALUES(1)");
        }
        catch (PostgresException ex)
        {
            Assert.That(ex.ColumnName, Is.Null, "ColumnName should not be populated for unique violations");
            Assert.That(ex.TableName, Is.EqualTo("uniqueviolation"));
            Assert.That(ex.SchemaName, Does.StartWith("pg_temp"));
            Assert.That(ex.ConstraintName, Is.EqualTo("uniqueviolation_pkey"));
            Assert.That(ex.DataTypeName, Is.Null, "DataTypeName should not be populated for unique violations");
        }
    }

    [Test]
    public void Column_name_exception_field_is_populated()
    {
        using var conn = OpenConnection();
        TestUtil.MinimumPgVersion(conn, "9.3.0", "5 error fields haven't been added yet");
        conn.ExecuteNonQuery("CREATE TEMP TABLE notnullviolation (id INT NOT NULL)");
        try
        {
            conn.ExecuteNonQuery("INSERT INTO notnullviolation (id) VALUES(NULL)");
        }
        catch (PostgresException ex)
        {
            Assert.That(ex.SchemaName, Does.StartWith("pg_temp"));
            Assert.That(ex.TableName, Is.EqualTo("notnullviolation"));
            Assert.That(ex.ColumnName, Is.EqualTo("id"));
        }
    }

    [Test]
    public async Task DataTypeName_is_populated()
    {
        // On reading the source code for PostgreSQL9.3beta1, the only time that the
        // datatypename field is populated is when using domain types. So here we'll
        // create a domain that simply does not allow NULLs then try and cast NULL
        // to it.
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.3.0", "5 error fields haven't been added yet");

        var domainName = await GetTempTypeName(conn);

        await conn.ExecuteNonQueryAsync($"CREATE DOMAIN {domainName} AS INT NOT NULL");
        var pgEx = Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteNonQueryAsync($"SELECT CAST(NULL AS {domainName})"))!;

        Assert.That(pgEx.SqlState, Is.EqualTo(PostgresErrorCodes.NotNullViolation));
        Assert.That(pgEx.SchemaName, Is.EqualTo("public"));
        Assert.That(pgEx.DataTypeName, Is.EqualTo(domainName));
    }

    [Test]
    public void NpgsqlException_with_async()
    {
        using var conn = OpenConnection();
        Assert.That(async () => await conn.ExecuteNonQueryAsync("MALFORMED"),
            Throws.Exception.TypeOf<PostgresException>());
        // Just in case, anything but a PostgresException would trigger the connection breaking, check that
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
    }

    [Test]
    public void NpgsqlException_IsTransient()
    {
        Assert.True(new NpgsqlException("", new IOException()).IsTransient);
        Assert.True(new NpgsqlException("", new SocketException()).IsTransient);
        Assert.True(new NpgsqlException("", new TimeoutException()).IsTransient);
        Assert.False(new NpgsqlException().IsTransient);
        Assert.False(new NpgsqlException("", new Exception("Inner Exception")).IsTransient);
    }

#if !NET9_0_OR_GREATER
#pragma warning disable SYSLIB0051
#pragma warning disable 618
    [Test]
    public void PostgresException_IsTransient()
    {
        Assert.True(CreateWithSqlState("53300").IsTransient);
        Assert.False(CreateWithSqlState("0").IsTransient);

        PostgresException CreateWithSqlState(string sqlState)
        {
            var info = CreateSerializationInfo();
            new Exception().GetObjectData(info, default);

            info.AddValue(nameof(PostgresException.Severity), null);
            info.AddValue(nameof(PostgresException.InvariantSeverity), null);
            info.AddValue(nameof(PostgresException.SqlState), sqlState);
            info.AddValue(nameof(PostgresException.MessageText), null);
            info.AddValue(nameof(PostgresException.Detail), null);
            info.AddValue(nameof(PostgresException.Hint), null);
            info.AddValue(nameof(PostgresException.Position), 0);
            info.AddValue(nameof(PostgresException.InternalPosition), 0);
            info.AddValue(nameof(PostgresException.InternalQuery), null);
            info.AddValue(nameof(PostgresException.Where), null);
            info.AddValue(nameof(PostgresException.SchemaName), null);
            info.AddValue(nameof(PostgresException.TableName), null);
            info.AddValue(nameof(PostgresException.ColumnName), null);
            info.AddValue(nameof(PostgresException.DataTypeName), null);
            info.AddValue(nameof(PostgresException.ConstraintName), null);
            info.AddValue(nameof(PostgresException.File), null);
            info.AddValue(nameof(PostgresException.Line), null);
            info.AddValue(nameof(PostgresException.Routine), null);

            return new PostgresException(info, default);
        }
    }
#pragma warning restore SYSLIB0051
#pragma warning restore 618

#pragma warning disable SYSLIB0011
#pragma warning disable SYSLIB0050

#if !NET9_0_OR_GREATER // BinaryFormatter serialization and deserialization have been removed. See https://aka.ms/binaryformatter for more information.
    [Test]
    public void Serialization()
    {
        var actual = new PostgresException("message text", "high", "high2", "53300", "detail", "hint", 18, 42, "internal query",
            "where", "schema", "table", "column", "data type", "constraint", "file", "line", "routine");

        var formatter = new BinaryFormatter();
        var stream = new MemoryStream();

        formatter.Serialize(stream, actual);
        stream.Seek(0, SeekOrigin.Begin);

        var expected = (PostgresException)formatter.Deserialize(stream);

        Assert.That(expected.Severity, Is.EqualTo(actual.Severity));
        Assert.That(expected.InvariantSeverity, Is.EqualTo(actual.InvariantSeverity));
        Assert.That(expected.SqlState, Is.EqualTo(actual.SqlState));
        Assert.That(expected.MessageText, Is.EqualTo(actual.MessageText));
        Assert.That(expected.Detail, Is.EqualTo(actual.Detail));
        Assert.That(expected.Hint, Is.EqualTo(actual.Hint));
        Assert.That(expected.Position, Is.EqualTo(actual.Position));
        Assert.That(expected.InternalPosition, Is.EqualTo(actual.InternalPosition));
        Assert.That(expected.InternalQuery, Is.EqualTo(actual.InternalQuery));
        Assert.That(expected.Where, Is.EqualTo(actual.Where));
        Assert.That(expected.SchemaName, Is.EqualTo(actual.SchemaName));
        Assert.That(expected.TableName, Is.EqualTo(actual.TableName));
        Assert.That(expected.ColumnName, Is.EqualTo(actual.ColumnName));
        Assert.That(expected.DataTypeName, Is.EqualTo(actual.DataTypeName));
        Assert.That(expected.ConstraintName, Is.EqualTo(actual.ConstraintName));
        Assert.That(expected.File, Is.EqualTo(actual.File));
        Assert.That(expected.Line, Is.EqualTo(actual.Line));
        Assert.That(expected.Routine, Is.EqualTo(actual.Routine));
    }
#endif

    SerializationInfo CreateSerializationInfo() => new(typeof(PostgresException), new FormatterConverter());
#pragma warning restore SYSLIB0011

#pragma warning disable SYSLIB0051
    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3204")]
    public void Base_exception_property_serialization()
    {
        var ex = new PostgresException("the message", "low", "low2", "XX123");

        var info = CreateSerializationInfo();
        ex.GetObjectData(info, default);

        // Check virtual base properties, which can be incorrectly deserialized if overridden, because the base
        // Exception.GetObjectData() method writes the fields, not the properties (e.g. "_message" instead of "Message").
        Assert.That(ex.Data, Is.EquivalentTo((IDictionary?)info.GetValue("Data", typeof(IDictionary))));
        Assert.That(ex.HelpLink, Is.EqualTo(info.GetValue("HelpURL", typeof(string))));
        Assert.That(ex.Message, Is.EqualTo(info.GetValue("Message", typeof(string))));
        Assert.That(ex.Source, Is.EqualTo(info.GetValue("Source", typeof(string))));
        Assert.That(ex.StackTrace, Is.EqualTo(info.GetValue("StackTraceString", typeof(string))));
    }
#pragma warning restore SYSLIB0051
#endif
}
