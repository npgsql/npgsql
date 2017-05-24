#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

#if !NETCOREAPP1_1

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
                    Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("42883"));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1212")]
        public void TableParameters()
        {
            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.2.0");

                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
                    CREATE FUNCTION pg_temp.func(IN in1 INT) RETURNS TABLE(t1 INT, t2 INT) AS
                      'SELECT in1,in1+1' LANGUAGE 'sql';
                ");

                var cmd = new NpgsqlCommand("pg_temp.func", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Has.Count.EqualTo(3));
                Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
                cmd.Parameters[0].Value = 5;
                cmd.ExecuteNonQuery();
                Assert.That(cmd.Parameters[1].Value, Is.EqualTo(5));
                Assert.That(cmd.Parameters[2].Value, Is.EqualTo(6));
            }
        }

        [Test, Description("Tests function parameter derivation for quoted functions with double quotes in the name works")]
        public void FunctionQuoteCharactersInNameDeriveParameters()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(
                    @"CREATE OR REPLACE FUNCTION pg_temp.""""""FunctionQuote""""CharactersInName""""""(int4, text) returns int4 as
                              $BODY$
                              begin
                                return 0;
                              end
                              $BODY$
                              language 'plpgsql';");
                var command = new NpgsqlCommand("pg_temp.\"\"\"FunctionQuote\"\"CharactersInName\"\"\"", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(command);
                Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
                Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
            }
        }

        [Test, Description("Tests function parameter derivation for quoted functions with dots in the name works")]
        public void FunctionDotCharacterInNameDeriveParameters()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(
                    @"CREATE OR REPLACE FUNCTION pg_temp.""My.Dotted.Function""(int4, text) returns int4 as
                              $BODY$
                              begin
                                return 0;
                              end
                              $BODY$
                              language 'plpgsql';");
                var command = new NpgsqlCommand("pg_temp.\"My.Dotted.Function\"", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(command);
                Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
                Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
            }
        }

        [Test, Description("Tests if the right function according to search_path is used in function parameter derivation")]
        public void DeriveParametersWithCorrectSchemaResolution()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS schema1, schema2 CASCADE; CREATE SCHEMA schema1; CREATE SCHEMA schema2;");
                try
                {
                    conn.ExecuteNonQuery(
                        @"
CREATE OR REPLACE FUNCTION schema1.redundantfunc() RETURNS int AS
$BODY$
BEGIN
    RETURN 1;
END;
$BODY$
LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION schema2.redundantfunc(IN param1 INT, IN param2 INT) RETURNS int AS
$BODY$
BEGIN
RETURN param1 + param2;
END;
$BODY$
LANGUAGE 'plpgsql';

SET search_path TO schema2;
");
                    var command = new NpgsqlCommand("redundantfunc", conn) { CommandType = CommandType.StoredProcedure };
                    NpgsqlCommandBuilder.DeriveParameters(command);
                    Assert.That(command.Parameters, Has.Count.EqualTo(2));
                    Assert.That(command.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                    Assert.That(command.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
                    command.Parameters[0].Value = 5;
                    command.Parameters[1].Value = 4;
                    Assert.That(command.ExecuteScalar(), Is.EqualTo(9));
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS schema1, schema2 CASCADE");
                }
            }
        }

        [Test, Description("Tests if function parameter derivation throws an exception if the specified function is not in the search_path")]
        public void DeriveThrowsForExistingFunctionThatIsNotInSearchPath()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS schema1 CASCADE; CREATE SCHEMA schema1;");
                try
                {
                    conn.ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION schema1.schema1func() RETURNS int AS
$BODY$
BEGIN
    RETURN 1;
END;
$BODY$
LANGUAGE 'plpgsql';

RESET search_path;
");
                    var command = new NpgsqlCommand("schema1func", conn) { CommandType = CommandType.StoredProcedure };
                    Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(command),
                        Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("42883"));
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS schema1 CASCADE");
                }
            }
        }

        [Test, Description("Tests if an exception is thrown if multiple functions with the specified name are in the search_path")]
        public void DeriveThrowsForMultipleFunctionNameHitsInSearchPath()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS schema1, schema2 CASCADE; CREATE SCHEMA schema1; CREATE SCHEMA schema2;");
                try
                {
                    conn.ExecuteNonQuery(
                        @"
CREATE OR REPLACE FUNCTION schema1.redundantfunc() RETURNS int AS
$BODY$
BEGIN
    RETURN 1;
END;
$BODY$
LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION schema2.redundantfunc(IN param1 INT, IN param2 INT) RETURNS int AS
$BODY$
BEGIN
RETURN param1 + param2;
END;
$BODY$
LANGUAGE 'plpgsql';

SET search_path TO schema1, schema2;
");
                    var command = new NpgsqlCommand("redundantfunc", conn) { CommandType = CommandType.StoredProcedure };
                    Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(command),
                        Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("42725"));
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS schema1, schema2 CASCADE");
                }
            }
        }

        #region Set returning functions

        [Test, Description("Tests parameter derivation for a function that returns SETOF sometype")]
        public void DeriveParametersFunctionReturningSetofType()
        {
            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.2.0");

                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
CREATE TABLE pg_temp.foo (fooid int, foosubid int, fooname text);

INSERT INTO pg_temp.foo VALUES
(1, 1, 'Joe'),
(1, 2, 'Ed'),
(2, 1, 'Mary');

CREATE FUNCTION pg_temp.getfoo(int) RETURNS SETOF foo AS $$
    SELECT * FROM pg_temp.foo WHERE pg_temp.foo.fooid = $1 ORDER BY pg_temp.foo.foosubid;
$$ LANGUAGE SQL;
                ");

                var cmd = new NpgsqlCommand("pg_temp.getfoo", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Has.Count.EqualTo(4));
                Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
                cmd.Parameters[0].Value = 1;
                cmd.ExecuteNonQuery();
                Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
            }
        }

        [Test, Description("Tests parameter derivation for a function that returns TABLE")]
        public void DeriveParametersFunctionReturningTable()
        {
            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.2.0");

                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
CREATE TABLE pg_temp.foo (fooid int, foosubid int, fooname text);

INSERT INTO pg_temp.foo VALUES
(1, 1, 'Joe'),
(1, 2, 'Ed'),
(2, 1, 'Mary');

CREATE FUNCTION pg_temp.getfoo(int) RETURNS TABLE(fooid int, foosubid int, fooname text) AS $$
    SELECT * FROM pg_temp.foo WHERE pg_temp.foo.fooid = $1 ORDER BY pg_temp.foo.foosubid;
$$ LANGUAGE SQL;
                ");

                var cmd = new NpgsqlCommand("pg_temp.getfoo", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Has.Count.EqualTo(4));
                Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
                cmd.Parameters[0].Value = 1;
                cmd.ExecuteNonQuery();
                Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
            }
        }

        [Test, Description("Tests parameter derivation for a function that returns SETOF record")]
        public void DeriveParametersFunctionReturningSetofRecord()
        {
            using (var conn = OpenConnection())
            {
                TestUtil.MinimumPgVersion(conn, "9.2.0");

                // This function returns record because of the two Out (InOut & Out) parameters
                conn.ExecuteNonQuery(@"
CREATE TABLE pg_temp.foo (fooid int, foosubid int, fooname text);

INSERT INTO pg_temp.foo VALUES
(1, 1, 'Joe'),
(1, 2, 'Ed'),
(2, 1, 'Mary');

CREATE FUNCTION pg_temp.getfoo(int, OUT fooid int, OUT foosubid int, OUT fooname text) RETURNS SETOF record AS $$
    SELECT * FROM pg_temp.foo WHERE pg_temp.foo.fooid = $1 ORDER BY pg_temp.foo.foosubid;
$$ LANGUAGE SQL;
                ");

                var cmd = new NpgsqlCommand("pg_temp.getfoo", conn) { CommandType = CommandType.StoredProcedure };
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Has.Count.EqualTo(4));
                Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
                cmd.Parameters[0].Value = 1;
                cmd.ExecuteNonQuery();
                Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
            }
        }

        #endregion
    }
}

#endif

