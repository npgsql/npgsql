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
            ExecuteNonQuery(@"CREATE FUNCTION pg_temp.func() RETURNS integer AS 'SELECT 8;' LANGUAGE 'sql'");
            using (var cmd = new NpgsqlCommand("pg_temp.func", Conn) { CommandType = CommandType.StoredProcedure })
            {
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(8));
            }
        }

        [Test, Description("Basic function call with an in parameter")]
        public void InParam()
        {
            ExecuteNonQuery(@"CREATE FUNCTION pg_temp.echo(IN param text) RETURNS text AS 'BEGIN RETURN param; END;' LANGUAGE 'plpgsql'");
            using (var cmd = new NpgsqlCommand("pg_temp.echo", Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@param", "hello");
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo("hello"));
            }
        }

        [Test, Description("Basic function call with an out parameter")]
        public void OutParam()
        {
            ExecuteNonQuery(@"CREATE FUNCTION pg_temp.echo (IN param_in text, OUT param_out text) AS 'BEGIN param_out=param_in; END;' LANGUAGE 'plpgsql'");
            using (var cmd = new NpgsqlCommand("pg_temp.echo", Conn)) {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@param", "hello");
                var outParam = new NpgsqlParameter("param_out", DbType.String) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outParam);
                cmd.ExecuteNonQuery();
                Assert.That(outParam.Value, Is.EqualTo("hello"));
            }
        }

        [Test, Description("Basic function call with an in/out parameter")]
        public void InOutParam()
        {
            ExecuteNonQuery(@"CREATE FUNCTION pg_temp.inc (INOUT param integer) AS 'BEGIN param=param+1; END;' LANGUAGE 'plpgsql'");
            using (var cmd = new NpgsqlCommand("pg_temp.inc", Conn)) {
                cmd.CommandType = CommandType.StoredProcedure;
                var outParam = new NpgsqlParameter("param", DbType.Int32) {
                    Direction = ParameterDirection.InputOutput,
                    Value = 8
                };
                cmd.Parameters.Add(outParam);
                cmd.ExecuteNonQuery();
                Assert.That(outParam.Value, Is.EqualTo(9));
            }
        }

        [Test]
        [MinPgVersion(9, 1, 0, "no binary output function available for type void before 9.1.0")]
        public void Void()
        {
            var command = new NpgsqlCommand("pg_sleep", Conn);
            command.Parameters.AddWithValue("sleep_time", 0);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        [Test]
        public void TooManyOutputParams()
        {
            var command = new NpgsqlCommand("VALUES (4,5), (6,7)", Conn);
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

        [Test]
        public void SingleRow()
        {
            ExecuteNonQuery(@"CREATE FUNCTION pg_temp.func() RETURNS TABLE (a INT, b INT) AS 'VALUES (1,2), (3,4);' LANGUAGE 'sql'");
            using (var cmd = new NpgsqlCommand("pg_temp.func", Conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader.Read(), Is.False);
                }
            }
        }

        #region Parameter Derivation

        [Test, Description("Tests function parameter derivation with IN, OUT and INOUT parameters")]
        public void DeriveParametersVarious()
        {
            // This function returns record because of the two Out (InOut & Out) parameters
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.func(IN param1 INT, OUT param2 text, INOUT param3 INT) RETURNS record AS
                              '
                              BEGIN
                                      param2 = ''sometext'';
                                      param3 = param1 + param3;
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand("pg_temp.func", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(3));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.InputOutput));
            cmd.Parameters[0].Value = 5;
            cmd.Parameters[2].Value = 4;
            cmd.ExecuteNonQuery();
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo(5));
            Assert.That(cmd.Parameters[1].Value, Is.EqualTo("sometext"));
            Assert.That(cmd.Parameters[2].Value, Is.EqualTo(9));
        }

        [Test, Description("Tests function parameter derivation with IN-only parameters")]
        public void DeriveParametersInOnly()
        {
            // This function returns record because of the two Out (InOut & Out) parameters
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.func(IN param1 INT, IN param2 INT) RETURNS int AS
                              '
                              BEGIN
                                RETURN param1 + param2;
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand("pg_temp.func", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            cmd.Parameters[0].Value = 5;
            cmd.Parameters[1].Value = 4;
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(9));
        }

        [Test, Description("Tests function parameter derivation with no parameters")]
        public void DeriveParametersNoParams()
        {
            // This function returns record because of the two Out (InOut & Out) parameters
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.func() RETURNS int AS
                              '
                              BEGIN
                                RETURN 4;
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand("pg_temp.func", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Is.Empty);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParameters()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.""FunctionCaseSensitive""(int4, text) returns int4 as
                              $BODY$
                              begin
                                return 0;
                              end
                              $BODY$
                              language 'plpgsql';");
            var command = new NpgsqlCommand("pg_temp.\"FunctionCaseSensitive\"", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        [Test]
        public void DeriveParametersWithParameterNameFromFunction()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.testoutparameter2(x int, y int, out sum int, out product int) as 'select $1 + $2, $1 * $2' language 'sql';");
            var command = new NpgsqlCommand("pg_temp.testoutparameter2", Conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(":x", command.Parameters[0].ParameterName);
            Assert.AreEqual(":y", command.Parameters[1].ParameterName);
        }

        [Test]
        public void DeriveInvalidFunction()
        {
            var invalidCommandName = new NpgsqlCommand("invalidfunctionname", Conn);
            Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(invalidCommandName),
                Throws.Exception.TypeOf<InvalidOperationException>()
                .With.Message.Contains("does not exist"));
        }

        #endregion

        public FunctionTests(string backendVersion) : base(backendVersion) { }
    }
}
