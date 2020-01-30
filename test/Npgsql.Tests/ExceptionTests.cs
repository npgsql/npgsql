using System;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class ExceptionTests : TestBase
    {
        [Test, Description("Generates a basic server-side exception, checks that it's properly raised and populated")]
        public void Basic()
        {
            using (var conn = OpenConnection())
            {
                // Make sure messages are in English
                conn.ExecuteNonQuery(@"SET lc_messages='en_US.UTF-8'");
                conn.ExecuteNonQuery(@"
                     CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
                        'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                     LANGUAGE 'plpgsql';
                ");

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

                var data = ex.Data;
                Assert.That(data[nameof(PostgresException.Severity)], Is.EqualTo("ERROR"));
                Assert.That(data[nameof(PostgresException.SqlState)], Is.EqualTo("12345"));
                Assert.That(data.Contains(nameof(PostgresException.Position)), Is.False);

                var exString = ex.ToString();
                Assert.That(exString, Contains.Substring(nameof(PostgresException.Severity) + ": ERROR"));
                Assert.That(exString, Contains.Substring(nameof(PostgresException.SqlState) + ": 12345"));

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1), "Connection in bad state after an exception");
            }
        }

        [Test]
        public void ExceptionFieldsArePopulated()
        {
            using (var conn = OpenConnection())
            {
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
        }

        [Test]
        public void ColumnNameExceptionFieldIsPopulated()
        {
            using (var conn = OpenConnection())
            {
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
        }

        [Test]
        [NonParallelizable]
        public void DataTypeNameExceptionFieldIsPopulated()
        {
            // On reading the source code for PostgreSQL9.3beta1, the only time that the
            // datatypename field is populated is when using domain types. So here we'll
            // create a domain that simply does not allow NULLs then try and cast NULL
            // to it.
            const string dropDomain = @"DROP DOMAIN IF EXISTS public.intnotnull";
            const string createDomain = @"CREATE DOMAIN public.intnotnull AS INT NOT NULL";
            const string castStatement = @"SELECT CAST(NULL AS public.intnotnull)";

            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.3.0", "5 error fields haven't been added yet");
                try
                {
                    var command = new NpgsqlCommand(dropDomain, conn);
                    command.ExecuteNonQuery();

                    command = new NpgsqlCommand(createDomain, conn);
                    command.ExecuteNonQuery();

                    command = new NpgsqlCommand(castStatement, conn);
                    //Cause the NOT NULL violation
                    command.ExecuteNonQuery();

                }
                catch (PostgresException ex)
                {
                    Assert.AreEqual("public", ex.SchemaName);
                    Assert.AreEqual("intnotnull", ex.DataTypeName);
                }
            }
        }

        [Test]
        public void NpgsqlExceptionInAsync()
        {
            using (var conn = OpenConnection())
            {
                Assert.That(async () => await conn.ExecuteNonQueryAsync("MALFORMED"),
                    Throws.Exception.TypeOf<PostgresException>());
                // Just in case, anything but a PostgresException would trigger the connection breaking, check that
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
            }
        }

        [Test]
        public void NpgsqlExceptionTransience()
        {
            Assert.True(new NpgsqlException("", new IOException()).IsTransient);
            Assert.True(new NpgsqlException("", new SocketException()).IsTransient);
            Assert.True(new NpgsqlException("", new TimeoutException()).IsTransient);
            Assert.False(new NpgsqlException().IsTransient);
            Assert.False(new NpgsqlException("", new Exception("Inner Exception")).IsTransient);
        }

        [Test]
        public void PostgresExceptionTransience()
        {
            Assert.True(CreateWithSqlState("53300").IsTransient);
            Assert.False(CreateWithSqlState("0").IsTransient);

            PostgresException CreateWithSqlState(string sqlState)
            {
                var info = CreateSerializationInfo();

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

        [Test]
        public void Serialization()
        {
            var info = CreateSerializationInfo();

            info.AddValue(nameof(PostgresException.Severity), "high");
            info.AddValue(nameof(PostgresException.InvariantSeverity), "high2");
            info.AddValue(nameof(PostgresException.SqlState), "53300");
            info.AddValue(nameof(PostgresException.MessageText), "message");
            info.AddValue(nameof(PostgresException.Detail), "detail");
            info.AddValue(nameof(PostgresException.Hint), "hint");
            info.AddValue(nameof(PostgresException.Position), 18);
            info.AddValue(nameof(PostgresException.InternalPosition), 42);
            info.AddValue(nameof(PostgresException.InternalQuery), "query");
            info.AddValue(nameof(PostgresException.Where), "where");
            info.AddValue(nameof(PostgresException.SchemaName), "schema");
            info.AddValue(nameof(PostgresException.TableName), "table");
            info.AddValue(nameof(PostgresException.ColumnName), "column");
            info.AddValue(nameof(PostgresException.DataTypeName), "type");
            info.AddValue(nameof(PostgresException.ConstraintName), "constraint");
            info.AddValue(nameof(PostgresException.File), "file");
            info.AddValue(nameof(PostgresException.Line), "line");
            info.AddValue(nameof(PostgresException.Routine), "routine");

            var actual = new PostgresException(info, default);
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

        SerializationInfo CreateSerializationInfo()
        {
            var info = new SerializationInfo(typeof(PostgresException), new FormatterConverter());
            new Exception().GetObjectData(info, default);

            return info;
        }
    }
}
