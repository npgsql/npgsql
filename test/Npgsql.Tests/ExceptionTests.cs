#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Web.UI.WebControls;

using Npgsql;
using NpgsqlTypes;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests
{
    [TestFixture]
    public class ExceptionTests : TestBase
    {
        public ExceptionTests(string backendVersion) : base(backendVersion) { }

        [Test, Description("Generates a basic server-side exception, checks that it's properly raised and populated")]
        public void Basic()
        {
            // Make sure messages are in English
            ExecuteNonQuery(@"SET lc_messages='English_United States.1252'");
            ExecuteNonQuery(@"
                 CREATE OR REPLACE FUNCTION emit_exception() RETURNS VOID AS
                    'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                 LANGUAGE 'plpgsql';
            ");

            NpgsqlException ex = null;
            try
            {
                ExecuteNonQuery("SELECT emit_exception()");
                Assert.Fail("No exception was thrown");
            }
            catch (NpgsqlException e)
            {
                ex = e;
            }

            Assert.That(ex.MessageText, Is.EqualTo("testexception"));
            Assert.That(ex.Severity, Is.EqualTo("ERROR"));
            Assert.That(ex.Code, Is.EqualTo("12345"));
            Assert.That(ex.Position, Is.EqualTo(0));

            var data = ex.Data;
            Assert.That(data["Severity"], Is.EqualTo("ERROR"));
            Assert.That(data["Code"], Is.EqualTo("12345"));
            Assert.That(data.Contains("Position"), Is.False);

            Assert.That(ExecuteScalar("SELECT 1"), Is.EqualTo(1), "Connection in bad state after an exception");
        }

        [Test]
        [MinPgVersion(9, 3, 0, "5 error fields haven't been added yet")]
        public void ExceptionFieldsArePopulated()
        {
            const string dropTable = @"DROP TABLE IF EXISTS public.uniqueviolation";
            const string createTable = @"CREATE TABLE public.uniqueviolation (id INT NOT NULL, CONSTRAINT uniqueviolation_pkey PRIMARY KEY (id))";
            const string insertStatement = @"INSERT INTO public.uniqueviolation (id) VALUES(1)";

            // In this case we'll test a simple unique violation, we're not too interested in testing more
            // cases than this as the same code is executed in all error situations.
            try
            {
                var command = new NpgsqlCommand(dropTable, Conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(createTable, Conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(insertStatement, Conn);
                command.ExecuteNonQuery();

                //Now cause the unique violation...
                command.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Assert.That(ex.ColumnName, Is.Null, "ColumnName should not be populated for unique violations");
                Assert.That(ex.TableName, Is.EqualTo("uniqueviolation"));
                Assert.That(ex.SchemaName, Is.EqualTo("public"));
                Assert.That(ex.ConstraintName, Is.EqualTo("uniqueviolation_pkey"));
                Assert.That(ex.DataTypeName, Is.Null, "DataTypeName should not be populated for unique violations");
            }
        }

        [Test]
        [MinPgVersion(9, 3, 0, "5 error fields haven't been added yet")]
        public void ColumnNameExceptionFieldIsPopulated()
        {
            const string dropTable = @"DROP TABLE IF EXISTS public.notnullviolation";
            const string createTable = @"CREATE TABLE public.notnullviolation (id INT NOT NULL)";
            const string insertStatement = @"INSERT INTO public.notnullviolation (id) VALUES(NULL)";

            try
            {
                var command = new NpgsqlCommand(dropTable, Conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(createTable, Conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(insertStatement, Conn);
                //Cause the NOT NULL violation
                command.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Assert.AreEqual("public", ex.SchemaName);
                Assert.AreEqual("notnullviolation", ex.TableName);
                Assert.AreEqual("id", ex.ColumnName);
            }
        }

        [Test]
        [MinPgVersion(9, 3, 0, "5 error fields haven't been added yet")]
        public void DataTypeNameExceptionFieldIsPopulated()
        {
            // On reading the source code for PostgreSQL9.3beta1, the only time that the
            // datatypename field is populated is when using domain types. So here we'll
            // create a domain that simply does not allow NULLs then try and cast NULL
            // to it.
            const string dropDomain = @"DROP DOMAIN IF EXISTS public.intnotnull";
            const string createDomain = @"CREATE DOMAIN public.intnotnull AS INT NOT NULL";
            const string castStatement = @"SELECT CAST(NULL AS public.intnotnull)";

            try
            {
                var command = new NpgsqlCommand(dropDomain, Conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(createDomain, Conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(castStatement, Conn);
                //Cause the NOT NULL violation
                command.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Assert.AreEqual("public", ex.SchemaName);
                Assert.AreEqual("intnotnull", ex.DataTypeName);
            }
        }

        [Test]
        public void NpgsqlExceptionInAsync()
        {
            Assert.That(async () => await ExecuteNonQueryAsync("MALFORMED"), Throws.Exception.TypeOf<NpgsqlException>());
            // Just in case, anything but an NpgsqlException would trigger the connection breaking, check that
            Assert.That(Conn.FullState, Is.EqualTo(ConnectionState.Open));
        }
    }
}
