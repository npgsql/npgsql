using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using Mood = Npgsql.Tests.Types.EnumTests.Mood;

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
                Throws.Exception.TypeOf<NpgsqlException>()
                .With.Property("Code").EqualTo("42883"));
        }

        [Test, Description("Tests if resolving quoted functions with dots in the name works")]
        public void DeriveParametersWithDotsInFunctionName()
        {
            ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION pg_temp.""My.Dotted.Function""() RETURNS int AS
'
BEGIN
    RETURN 1;
END;
' LANGUAGE 'plpgsql';", Conn);

            var cmd = new NpgsqlCommand(@"pg_temp.""My.Dotted.Function""", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
        }

        [Test, Description("Tests if the right function is resolved according to search_path")]
        public void DeriveParametersWithCorrectSchemaResolution()
        {
            CreateSchema("schema1");
            CreateSchema("schema2");
            ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION schema1.redundantfunc() RETURNS int AS
'
BEGIN
    RETURN 1;
END;
' LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION schema2.redundantfunc(IN param1 INT, IN param2 INT) RETURNS int AS
'
BEGIN
RETURN param1 + param2;
END;
' LANGUAGE 'plpgsql';

SET search_path TO schema2;
", Conn);

            var cmd = new NpgsqlCommand(@"redundantfunc", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            cmd.Parameters[0].Value = 5;
            cmd.Parameters[1].Value = 4;
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(9));
        }

        [Test, Description("Tests if an exception is thrown if the specified function is not in the search_path")]
        public void DeriveThrowsForExistingFunctionThatIsNotInSearchPath()
        {
            CreateSchema("schema1");
            ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION schema1.schema1func() RETURNS int AS
'
BEGIN
    RETURN 1;
END;
' LANGUAGE 'plpgsql';

RESET search_path;
", Conn);

            var cmd = new NpgsqlCommand(@"schema1func", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(cmd),
                Throws.Exception.TypeOf<NpgsqlException>()
                .With.Property("Code").EqualTo("42883"));
        }

        [Test, Description("Tests if an exception is thrown if multiple functions with the specified name are in the search_path")]
        public void DeriveThrowsForMultipleFunctionNameHitsInSearchPath()
        {
            CreateSchema("schema1");
            CreateSchema("schema2");
            ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION schema1.redundantfunc() RETURNS int AS
'
BEGIN
    RETURN 1;
END;
' LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION schema2.redundantfunc(IN param1 INT, IN param2 INT) RETURNS int AS
'
BEGIN
RETURN param1 + param2;
END;
' LANGUAGE 'plpgsql';

SET search_path TO schema1, schema2;
", Conn);

            var cmd = new NpgsqlCommand(@"redundantfunc", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(cmd),
                Throws.Exception.TypeOf<NpgsqlException>()
                .With.Property("Code").EqualTo("42725"));
        }

        #region Enums

        [Test]
        public void DeriveEnum()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')");
            Conn.ReloadTypes();
            Conn.RegisterEnum<Mood>("mood");

            ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION pg_temp.make_happy(IN param1 mood, OUT param2 mood)
AS
$BODY$
BEGIN
    param2 = 'Happy'::mood;
END;
$BODY$ LANGUAGE 'plpgsql';
");

            var cmd = new NpgsqlCommand("pg_temp.make_happy", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Enum));
            Assert.That(cmd.Parameters[0].EnumType, Is.EqualTo(typeof(Mood)));
            Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Enum));
            Assert.That(cmd.Parameters[1].EnumType, Is.EqualTo(typeof(Mood)));
            cmd.Parameters[0].Value = Mood.Sad;
            cmd.ExecuteNonQuery();
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo(Mood.Sad));
            Assert.That(cmd.Parameters[1].Value, Is.EqualTo(Mood.Happy));
        }

        [Test]
        public void DeriveEnumArray()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')");
            Conn.ReloadTypes();
            Conn.RegisterEnum<Mood>("mood");

            ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION pg_temp.get_moods(IN param1 mood[], OUT param2 mood[])
AS
$BODY$
BEGIN
    param2 = enum_range('Ok'::mood, NULL);
END;
$BODY$ LANGUAGE 'plpgsql';
");

            var cmd = new NpgsqlCommand("pg_temp.get_moods", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Enum | NpgsqlDbType.Array));
            Assert.That(cmd.Parameters[0].EnumType, Is.EqualTo(typeof(Mood)));
            Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Enum | NpgsqlDbType.Array));
            Assert.That(cmd.Parameters[1].EnumType, Is.EqualTo(typeof(Mood)));
            Mood[] input = new Mood[] { Mood.Sad, Mood.Sad };
            Mood[] output = new Mood[] { Mood.Ok, Mood.Happy };
            cmd.Parameters[0].Value = input;
            cmd.ExecuteNonQuery();
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo(input));
            Assert.That(cmd.Parameters[1].Value, Is.EqualTo(output));
        }

        [Test]
        public void DeriveUnmappedEnumAsString()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')", conn);
                ExecuteNonQuery(@"
CREATE OR REPLACE FUNCTION pg_temp.make_happy(IN param1 mood, OUT param2 mood)
AS
$BODY$
BEGIN
    param2 = 'Happy'::mood;
END;
$BODY$ LANGUAGE 'plpgsql';
", conn);

                var cmd = new NpgsqlCommand("pg_temp.make_happy", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
                Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
                Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
                Assert.That(cmd.Parameters[0].EnumType, Is.Null);
                Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
                Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
                Assert.That(cmd.Parameters[1].EnumType, Is.Null);
                cmd.Parameters[0].Value = Mood.Sad;
                cmd.ExecuteNonQuery();
                Assert.That(cmd.Parameters[0].Value, Is.EqualTo(Mood.Sad));
                Assert.That(cmd.Parameters[1].Value, Is.EqualTo("Happy"));
            }
        }

        #endregion

        #region Set returning functions

        // As of Npgsql 3.0.3 DeriveParameters ignores the fact that a function with OUT parameters returns a set
        // and derives IN and OUT parameters without moaning.
        // On the other hand it denies to derive parameters for functions that return TABLE and derives only IN
        // parameters for functions returning SETOF sometype.
        // I'd argue that set returning functions should be executed using NpgsqlCommand.ExecuteReader() where
        // output parameters are useless but changing this will probably break things out there.
        // In any case we should at least be consistent about whether we allow to derive named return
        // parameters (out or table) from set returning functions.
        // As it would be pretty hard to implement this kind of consistency for functions returning SETOF
        // sometype I chose to always discard OUT parameters for set returning functions.

        [Test, Description("Tests parameter derivation for a function that returns SETOF sometype")]
        public void DeriveParametersFunctionReturningSetofType()
        {
            ExecuteNonQuery(@"
DROP FUNCTION IF EXISTS getfoo(int);
DROP TABLE IF EXISTS foo CASCADE;
", Conn);
            ExecuteNonQuery(@"
CREATE TABLE foo (fooid int, foosubid int, fooname text);
", Conn);
            ExecuteNonQuery(@"
INSERT INTO foo VALUES (1, 1, 'Joe');
INSERT INTO foo VALUES (1, 2, 'Ed');
INSERT INTO foo VALUES (2, 1, 'Mary');
", Conn);
            ExecuteNonQuery(@"
CREATE FUNCTION getfoo(int) RETURNS SETOF foo AS $$
    SELECT * FROM foo WHERE foo.fooid = $1 ORDER BY foo.foosubid;
$$ LANGUAGE SQL;
", Conn);

            var cmd = new NpgsqlCommand("getfoo", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            cmd.Parameters[0].Value = 1;
            cmd.ExecuteNonQuery();
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
        }

        [Test, Description("Tests parameter derivation for a function that returns TABLE")]
        public void DeriveParametersFunctionReturningTable()
        {
            ExecuteNonQuery(@"
DROP FUNCTION IF EXISTS getfoo(int);
DROP TABLE IF EXISTS foo CASCADE;
", Conn);
            ExecuteNonQuery(@"
CREATE TABLE foo (fooid int, foosubid int, fooname text);
", Conn);
            ExecuteNonQuery(@"
INSERT INTO foo VALUES (1, 1, 'Joe');
INSERT INTO foo VALUES (1, 2, 'Ed');
INSERT INTO foo VALUES (2, 1, 'Mary');
", Conn);
            ExecuteNonQuery(@"
CREATE FUNCTION getfoo(int) RETURNS TABLE(fooid int, foosubid int, fooname text) AS $$
    SELECT * FROM foo WHERE foo.fooid = $1 ORDER BY foo.foosubid;
$$ LANGUAGE SQL;
", Conn);

            var cmd = new NpgsqlCommand("getfoo", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            cmd.Parameters[0].Value = 1;
            cmd.ExecuteNonQuery();
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
        }

        [Test, Description("Tests parameter derivation for a function that returns TABLE")]
        public void DeriveParametersFunctionReturningSetofRecord()
        {
            ExecuteNonQuery(@"
DROP FUNCTION IF EXISTS getfoo(int);
DROP TABLE IF EXISTS foo CASCADE;
", Conn);
            ExecuteNonQuery(@"
CREATE TABLE foo (fooid int, foosubid int, fooname text);
", Conn);
            ExecuteNonQuery(@"
INSERT INTO foo VALUES (1, 1, 'Joe');
INSERT INTO foo VALUES (1, 2, 'Ed');
INSERT INTO foo VALUES (2, 1, 'Mary');
", Conn);
            ExecuteNonQuery(@"
CREATE FUNCTION getfoo(int, OUT fooid int, OUT foosubid int, OUT fooname text) RETURNS SETOF record AS $$
    SELECT * FROM foo WHERE foo.fooid = $1 ORDER BY foo.foosubid;
$$ LANGUAGE SQL;
", Conn);

            var cmd = new NpgsqlCommand("getfoo", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
            Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
            cmd.Parameters[0].Value = 1;
            cmd.ExecuteNonQuery();
            Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
        }

        #endregion

        #endregion

        public FunctionTests(string backendVersion) : base(backendVersion) { }
    }
}
