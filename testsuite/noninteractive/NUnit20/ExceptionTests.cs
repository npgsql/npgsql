// project created on 30/11/2002 at 22:00
//
// Author:
//  David Rowley <dgrowley@gmail.com>
// 	Francisco Figueiredo Jr. <fxjrlists@yahoo.com>
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
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
                Assert.AreEqual("", ex.ColumnName); // Should not be populated for unique violations.
                Assert.AreEqual("uniqueviolation", ex.TableName);
                Assert.AreEqual("public", ex.SchemaName);
                Assert.AreEqual("uniqueviolation_pkey", ex.ConstraintName);
                Assert.AreEqual("", ex.DataTypeName); // Should not be populated for unique violations.
            }
        }

        [Test]
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

        [Test]
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
