using System;
using System.Data;
using System.Web.UI.WebControls;

using Npgsql;
using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{

    [TestFixture]
    public class ExceptionTests : BaseClassTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _conn;}
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _t; }
            set { _t = value; }
        }
        protected virtual string TheConnectionString {
            get { return _connString; }
        }

        [Test]
        public void ProblemSqlInsideException()
        {
            String sql = "selec 1 as test";
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);

                command.ExecuteReader();
            }
            catch (NpgsqlException ex)
            {
                Assert.AreEqual(sql, ex.ErrorSql);
            }
        }

        [Test]
        public void ExceptionFieldsArePopulated1()
        {
            String dropTable = "DROP TABLE IF EXISTS public.uniqueviolation";
            String createTable = "CREATE TABLE public.uniqueviolation (id INT NOT NULL, CONSTRAINT uniqueviolation_pkey PRIMARY KEY (id))";
            String insertStatement = "INSERT INTO public.uniqueviolation (id) VALUES(1)";

            // Since the 5 error fields were added as of PostgreSQL 9.3, we'll skip testing for versions previous to that.
            if (TheConnection.PostgreSqlVersion < new Version("9.3"))
                return;

            // In this case we'll test a simple unique violation, we're not too interested in testing more
            // cases than this as the same code is executed in all error situations.
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(dropTable, TheConnection);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(createTable, TheConnection);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(insertStatement, TheConnection);
                command.ExecuteNonQuery();

                //Now cause the unique violation...
                command.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Assert.AreEqual("id", ex.ColumnName);
                Assert.AreEqual("uniqueviolation", ex.TableName);
                Assert.AreEqual("public", ex.SchemaName);
                Assert.AreEqual("uniqueviolation_pkey", ex.ConstraintName);
                Assert.AreEqual("integer", ex.DataTypeName);
            }
        }

        public void ColumnNameExceptionFieldsIsPopulated()
        {
            String dropTable = "DROP TABLE IF EXISTS public.notnullviolation";
            String createTable = "CREATE TABLE public.notnullviolation (id INT NOT NULL)";
            String insertStatement = "INSERT INTO public.notnullviolation (id) VALUES(NULL)";

            // Since the 5 error fields were added as of PostgreSQL 9.3, we'll skip testing for versions previous to that.
            if (TheConnection.PostgreSqlVersion < new Version("9.3"))
                return;

            try
            {
                NpgsqlCommand command = new NpgsqlCommand(dropTable, TheConnection);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(createTable, TheConnection);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(insertStatement, TheConnection);
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

        public void DataTypeNameExceptionFieldsIsPopulated()
        {
            String dropDomain = "DROP DOMAIN IF EXISTS public.intnotnull";
            String createDomain = "CREATE DOMAIN public.intnotnull AS INT NOT NULL";
            String castStatement = "SELECT CAST(NULL AS public.intnotnull)";

            // Since the 5 error fields were added as of PostgreSQL 9.3, we'll skip testing for versions previous to that.
            if (TheConnection.PostgreSqlVersion < new Version("9.3"))
                return;

            try
            {
                NpgsqlCommand command = new NpgsqlCommand(dropDomain, TheConnection);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(createDomain, TheConnection);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand(castStatement, TheConnection);
                //Cause the NOT NULL violation
                command.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Assert.AreEqual("public", ex.SchemaName);
                Assert.AreEqual("intnotnull", ex.DataTypeName);
            }
        }

    }
}
