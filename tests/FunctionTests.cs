using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    /// <summary>
    /// A fixture for tests which interact with functions. All functions are dropped from the
    /// database before each test starts.
    /// </summary>
    public class FunctionTests : TestBase
    {
        [Test]
        public void FunctionInOutParameters()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.func(OUT param1 int, INOUT param2 int) RETURNS record AS 
                              '
                              BEGIN
                                      param1 = 1;
                                      param2 = param2 + 1; 
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand(@"pg_temp.func", Conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new NpgsqlParameter("param1", DbType.Int32) {
              Direction = ParameterDirection.Output
            });
            cmd.Parameters.Add(new NpgsqlParameter("param2", DbType.Int32) {
                Direction = ParameterDirection.InputOutput,
                Value = 5
            });

            using (var rdr = cmd.ExecuteReader())
            {
                Assert.That(rdr.Read(), Is.True);
                Assert.That(rdr.GetInt32(0), Is.EqualTo(1));
                Assert.That(rdr.GetInt32(1), Is.EqualTo(6));
                Assert.That(rdr.Read(), Is.False);
            }
        }

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

        public FunctionTests(string backendVersion) : base(backendVersion) { }
    }
}
