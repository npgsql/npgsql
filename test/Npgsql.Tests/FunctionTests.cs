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
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    /// <summary>
    /// A fixture for tests which interact with functions.
    /// All tests should create functions in the pg_temp schema only to ensure there's no interaction between
    /// the tests.
    /// </summary>
    public class FunctionTests : TestBase
    {
        [Test, Description("Simple function with no parameters, results accessed as a resultset")]
        public void ResultSet()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE FUNCTION pg_temp.func() RETURNS integer AS 'SELECT 8;' LANGUAGE 'sql'");
                using (var cmd = new NpgsqlCommand("pg_temp.func", conn) { CommandType = CommandType.StoredProcedure })
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(8));
            }
        }

        [Test, Description("Basic function call with an in parameter")]
        public void InParam()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE FUNCTION pg_temp.echo(IN param text) RETURNS text AS 'BEGIN RETURN param; END;' LANGUAGE 'plpgsql'");
                using (var cmd = new NpgsqlCommand("pg_temp.echo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@param", "hello");
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo("hello"));
                }
            }
        }

        [Test, Description("Basic function call with an out parameter")]
        public void OutParam()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE FUNCTION pg_temp.echo (IN param_in text, OUT param_out text) AS 'BEGIN param_out=param_in; END;' LANGUAGE 'plpgsql'");
                using (var cmd = new NpgsqlCommand("pg_temp.echo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@param_in", "hello");
                    var outParam = new NpgsqlParameter("param_out", DbType.String) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outParam);
                    cmd.ExecuteNonQuery();
                    Assert.That(outParam.Value, Is.EqualTo("hello"));
                }
            }
        }

        [Test, Description("Basic function call with an in/out parameter")]
        public void InOutParam()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE FUNCTION pg_temp.inc (INOUT param integer) AS 'BEGIN param=param+1; END;' LANGUAGE 'plpgsql'");
                using (var cmd = new NpgsqlCommand("pg_temp.inc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var outParam = new NpgsqlParameter("param", DbType.Int32)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = 8
                    };
                    cmd.Parameters.Add(outParam);
                    cmd.ExecuteNonQuery();
                    Assert.That(outParam.Value, Is.EqualTo(9));
                }
            }
        }

        [Test]
        public void Void()
        {
            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.1.0", "no binary output function available for type void before 9.1.0");
                var command = new NpgsqlCommand("pg_sleep", conn);
                command.Parameters.AddWithValue(0);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void NamedParameters()
        {
            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.4.0", "make_timestamp was introduced in 9.4");
                using (var command = new NpgsqlCommand("make_timestamp", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("year", 2015);
                    command.Parameters.AddWithValue("month", 8);
                    command.Parameters.AddWithValue("mday", 1);
                    command.Parameters.AddWithValue("hour", 2);
                    command.Parameters.AddWithValue("min", 3);
                    command.Parameters.AddWithValue("sec", 4);
                    var dt = (DateTime) command.ExecuteScalar();

                    Assert.AreEqual(new DateTime(2015, 8, 1, 2, 3, 4), dt);

                    command.Parameters[0].Value = 2014;
                    command.Parameters[0].ParameterName = ""; // 2014 will be sent as a positional parameter
                    dt = (DateTime) command.ExecuteScalar();
                    Assert.AreEqual(new DateTime(2014, 8, 1, 2, 3, 4), dt);
                }
            }
        }

        [Test]
        public void TooManyOutputParams()
        {
            using (var conn = OpenConnection())
            {
                var command = new NpgsqlCommand("VALUES (4,5), (6,7)", conn);
                command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                });
                command.Parameters.Add(new NpgsqlParameter("b", DbType.Int32)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                });
                command.Parameters.Add(new NpgsqlParameter("c", DbType.Int32)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                });

                command.ExecuteNonQuery();

                Assert.That(command.Parameters["a"].Value, Is.EqualTo(4));
                Assert.That(command.Parameters["b"].Value, Is.EqualTo(5));
                Assert.That(command.Parameters["c"].Value, Is.EqualTo(-1));
            }
        }
    }
}
