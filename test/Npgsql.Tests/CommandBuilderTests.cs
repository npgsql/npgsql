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

#if NET451

using System;
using System.Data;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class CommandBuilderTests : TestBase
    {
        [Test, Description("Tests function parameter derivation with IN, OUT and INOUT parameters")]
        public void DeriveParametersVarious()
        {
            using (var conn = OpenConnection())
            {
                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
                    CREATE OR REPLACE FUNCTION pg_temp.func(IN param1 INT, OUT param2 text, INOUT param3 INT) RETURNS record AS
                    '
                    BEGIN
                            param2 = ''sometext'';
                            param3 = param1 + param3;
                    END;
                    ' LANGUAGE 'plpgsql';
                ");

                var cmd = new NpgsqlCommand("pg_temp.func", conn) { CommandType = CommandType.StoredProcedure };
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
        }

        [Test, Description("Tests function parameter derivation with IN-only parameters")]
        public void DeriveParametersInOnly()
        {
            using (var conn = OpenConnection())
            {
                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
                    CREATE OR REPLACE FUNCTION pg_temp.func(IN param1 INT, IN param2 INT) RETURNS int AS
                    '
                    BEGIN
                    RETURN param1 + param2;
                    END;
                    ' LANGUAGE 'plpgsql';
                ");

                var cmd = new NpgsqlCommand("pg_temp.func", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
                Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
                cmd.Parameters[0].Value = 5;
                cmd.Parameters[1].Value = 4;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(9));
            }
        }

        [Test, Description("Tests function parameter derivation with no parameters")]
        public void DeriveParametersNoParams()
        {
            using (var conn = OpenConnection())
            {
                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
                    CREATE OR REPLACE FUNCTION pg_temp.func() RETURNS int AS
                    '
                    BEGIN
                    RETURN 4;
                    END;
                    ' LANGUAGE 'plpgsql';
                ");

                var cmd = new NpgsqlCommand("pg_temp.func", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Is.Empty);
            }
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParameters()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(
                    @"CREATE OR REPLACE FUNCTION pg_temp.""FunctionCaseSensitive""(int4, text) returns int4 as
                              $BODY$
                              begin
                                return 0;
                              end
                              $BODY$
                              language 'plpgsql';");
                var command = new NpgsqlCommand("pg_temp.\"FunctionCaseSensitive\"", conn);
                NpgsqlCommandBuilder.DeriveParameters(command);
                Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
                Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
            }
        }

        [Test]
        public void DeriveParametersWithParameterNameFromFunction()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.testoutparameter2(x int, y int, out sum int, out product int) as 'select $1 + $2, $1 * $2' language 'sql';");
                var command = new NpgsqlCommand("pg_temp.testoutparameter2", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(command);
                Assert.AreEqual(":x", command.Parameters[0].ParameterName);
                Assert.AreEqual(":y", command.Parameters[1].ParameterName);
            }
        }

        [Test]
        public void DeriveInvalidFunction()
        {
            using (var conn = OpenConnection())
            {
                var invalidCommandName = new NpgsqlCommand("invalidfunctionname", conn);
                Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(invalidCommandName),
                    Throws.Exception.TypeOf<InvalidOperationException>()
                                    .With.Message.Contains("does not exist"));
            }
        }
    }
}

#endif

