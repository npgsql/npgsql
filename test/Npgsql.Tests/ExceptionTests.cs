#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Npgsql;
using NpgsqlTypes;


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
                conn.ExecuteNonQuery(@"SET lc_messages='en_US.UTF8'");
                conn.ExecuteNonQuery(@"
                     CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
                        'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                     LANGUAGE 'plpgsql';
                ");

                PostgresException ex = null;
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
                Assert.That(ex.SqlState, Is.EqualTo("12345"));
                Assert.That(ex.Position, Is.EqualTo(0));

                var data = ex.Data;
                Assert.That(data[nameof(PostgresException.Severity)], Is.EqualTo("ERROR"));
                Assert.That(data[nameof(PostgresException.SqlState)], Is.EqualTo("12345"));
                Assert.That(data.Contains(nameof(PostgresException.Position)), Is.False);

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
        [Parallelizable(ParallelScope.None)]
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
    }
}
