using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests
{
    /// <summary>
    /// A fixture for tests which interact with functions. All functions are dropped from the
    /// database before each test starts.
    /// </summary>
    public class FunctionTests : TestBase
    {
        public FunctionTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public void FunctionInOutParameters()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ""SomeFunction""(OUT param1 int, INOUT param2 int) RETURNS record AS 
                              '
                              BEGIN
                                      param1 = 1;
                                      param2 = param2 + 1; 
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand(@"""SomeFunction""", Conn);
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

        #region Parameter Derivation

        [Test, Description("Tests function parameter derivation with IN, OUT and INOUT parameters")]
        public void DeriveParametersVarious()
        {
            // This function returns record because of the two Out (InOut & Out) parameters
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ""func""(IN param1 INT, OUT param2 text, INOUT param3 INT) RETURNS record AS 
                              '
                              BEGIN
                                      param2 = ''sometext'';
                                      param3 = param1 + param3;
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand("func", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ""func""(IN param1 INT, IN param2 INT) RETURNS int AS 
                              '
                              BEGIN
                                RETURN param1 + param2;
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand("func", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ""func""() RETURNS int AS 
                              '
                              BEGIN
                                RETURN 4;
                              END;
                              ' LANGUAGE 'plpgsql';");

            var cmd = new NpgsqlCommand("func", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(cmd);
            Assert.That(cmd.Parameters, Is.Empty);
        }

        #endregion

        [Test]
        public void TestOutParameter2()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testoutparameter2(x int, y int, out sum int, out product int) as 'select $1 + $2, $1 * $2' language 'sql';");
            var command = new NpgsqlCommand("testoutparameter2", Conn);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("@x", NpgsqlDbType.Integer)).Value = 1;
            command.Parameters.Add(new NpgsqlParameter("@y", NpgsqlDbType.Integer)).Value = 2;
            command.Parameters.Add(new NpgsqlParameter("@sum", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("@product", NpgsqlDbType.Refcursor));

            command.Parameters["@sum"].Direction = ParameterDirection.Output;
            command.Parameters["@product"].Direction = ParameterDirection.Output;

            using (var dr = command.ExecuteReader())
            {
                dr.Read();

                Assert.AreEqual(3, command.Parameters["@sum"].Value);
                Assert.AreEqual(2, command.Parameters["@product"].Value);
            }
        }

        [Test]
        public void FunctionCallReturnSingleValue()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallReturnSingleValueWithPrepare()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValue()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 4;
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValueNpgsqlDbType()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            command.Parameters[0].Value = 4;
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        private void FunctionCallWithParametersPrepareReturnSingleValueInternal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            Assert.AreEqual(1, command.Parameters.Count);

            command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValue()
        {
            FunctionCallWithParametersPrepareReturnSingleValueInternal();
        }

        private void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbTypeInternal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");

            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            Assert.AreEqual(1, command.Parameters.Count);

            command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType()
        {
            FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbTypeInternal();
        }

        private void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2Internal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");

            var command = new NpgsqlCommand("funcC(@a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            Assert.AreEqual(1, command.Parameters.Count);
            //command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2()
        {
            FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2Internal();
        }

        [Test]
        public void FunctionCallReturnResultSet()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() returns setof data as 'select * from data;' language 'sql';");
            var command = new NpgsqlCommand("funcB()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        public void FunctionCallWithImplicitParameters()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("@a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            command.Parameters.Add(p);
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void PreparedFunctionCallWithImplicitParameters()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            command.Parameters.Add(p);
            command.Prepare();
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithImplicitParametersWithNoParameters()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallOutputParameter()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(1, command.Parameters["a"].Value);
        }

        [Test]
        public void FunctionCallOutputParameter2()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("@a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(1, command.Parameters["@a"].Value);
        }

        [Test]
        public void OutputParameterWithoutName()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = command.CreateParameter();
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(1, command.Parameters[0].Value);
        }

        [Test]
        public void FunctionReturnVoid()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnvoid() returns void as
                              '
                              begin
                                      insert into data(field_text) values (''testvoid'');
                                      return;
                              end;
                              ' language 'plpgsql';");
            var command = new NpgsqlCommand("testreturnvoid()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        [Test]
        public void StatementOutputParameters()
        {
            var command = new NpgsqlCommand("values (4,5), (6,7)", Conn);
            var p = new NpgsqlParameter("a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);

            p = new NpgsqlParameter("b", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);

            p = new NpgsqlParameter("c", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);

            command.ExecuteNonQuery();

            // Should bear the values of the first tuple.
            Assert.AreEqual(4, command.Parameters["a"].Value);
            Assert.AreEqual(5, command.Parameters["b"].Value);
            Assert.AreEqual(-1, command.Parameters["c"].Value);
        }

        [Test]
        public void FunctionCallStringEscape()
        {
            int warnings = 0;
            NoticeEventHandler countWarn = delegate(Object c, NpgsqlNoticeEventArgs e) { warnings += 1; };
            Conn.Notice += countWarn;

            var testStrPar = "This string has a 'literal' backslash \\";
            var command = new NpgsqlCommand("trim", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter());
            command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            command.Parameters[0].Value = testStrPar;

            try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set escape_string_warning=on", Conn).ExecuteNonQuery();
                new NpgsqlCommand("set standard_conforming_strings=off", Conn).ExecuteNonQuery();
            } catch {
            }
            using (IDataReader rdr = command.ExecuteReader()) {
                rdr.Read();
                Assert.AreEqual(testStrPar, rdr.GetString(0));
            }

            try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=on", Conn).ExecuteNonQuery();
            } catch {
            }
            using (IDataReader rdr = command.ExecuteReader()) {
                rdr.Read();
                Assert.AreEqual(testStrPar, rdr.GetString(0));
            }

            Conn.Notice -= countWarn;
            Assert.AreEqual(0, warnings);
        }

        [Test]
        public void FunctionCallInputOutputParameter()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            command.Parameters.Add(p);
            var result = (Int64)command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void AmbiguousFunctionParameterType()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ambiguousParameterType(int2, int4, int8, text, varchar(10), char(5)) returns int4 as '
                                select 4 as result;
                              ' language 'sql'");
            //NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlParameter p = new NpgsqlParameter("a", DbType.Int16);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("b", DbType.Int32);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("c", DbType.Int64);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("d", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("e", NpgsqlDbType.Char);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("f", NpgsqlDbType.Varchar);
            p.Value = "a";
            command.Parameters.Add(p);

            command.ExecuteScalar();
        }

        [Test]
        public void AmbiguousFunctionParameterTypePrepared()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ambiguousParameterType(int2, int4, int8, text, varchar(10), char(5)) returns int4 as '
                                select 4 as result;
                              ' language 'sql'");
            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlParameter p = new NpgsqlParameter("a", DbType.Int16);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("b", DbType.Int32);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("c", DbType.Int64);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("d", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("e", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("f", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);

            command.Prepare();
            command.ExecuteScalar();
        }

        [Test]
        public void ProcedureNameWithSchemaSupport()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnrecord() returns record as 'select 4 ,5' language 'sql'");
            var command = new NpgsqlCommand("public.testreturnrecord", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();

            Assert.AreEqual(4, command.Parameters[0].Value);
            Assert.AreEqual(5, command.Parameters[1].Value);
        }

        [Test]
        public void ReturnRecordSupport()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnrecord() returns record as 'select 4 ,5' language 'sql'");
            var command = new NpgsqlCommand("testreturnrecord", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();

            Assert.AreEqual(4, command.Parameters[0].Value);
            Assert.AreEqual(5, command.Parameters[1].Value);
        }

        [Test]
        public void ReturnSetofRecord()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnsetofrecord() returns setof record as 'values (8,9), (6,7)' language 'sql'");
            var command = new NpgsqlCommand("testreturnsetofrecord", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();

            Assert.AreEqual(8, command.Parameters[0].Value);
            Assert.AreEqual(9, command.Parameters[1].Value);
        }

        [Test]
        public void ReturnRecordSupportWithResultset()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnrecordresultset(x int4, y int4) returns table (a int4, b int4) as
                              $BODY$
                              begin
                              return query
                              select 1, 2;
                              end;
                              $BODY$
                              language 'plpgsql'");
            var command = new NpgsqlCommand("testreturnrecordresultset", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Value = 1;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Value = 4;

            NpgsqlDataReader dr = null;

            try {
                dr = command.ExecuteReader();
            } finally {
                if (dr != null)
                    dr.Close();
            }
        }

        [Test]
        public void FunctionCaseSensitiveName()
        {
            CreateCaseSensitiveFunction();
            var command = new NpgsqlCommand("\"FunctionCaseSensitive\"", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            var result = command.ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FunctionCaseSensitiveNameWithSchema()
        {
            CreateCaseSensitiveFunction();
            var command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            var result = command.ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParameters()
        {
            var command = new NpgsqlCommand("\"FunctionCaseSensitive\"", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParametersWithSchema()
        {
            CreateCaseSensitiveFunction();
            var command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        void CreateCaseSensitiveFunction()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ""FunctionCaseSensitive""(int4, text) returns int4 as
                              $BODY$
                              begin
                                return 0;
                              end
                              $BODY$
                              language 'plpgsql';");
        }

        // TODO: Fix according to #438
        [Test]
        public void FunctionTestTimestamptzParameterSupport()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            using (Conn.BeginTransaction()) {
                ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testtimestamptzparameter(timestamptz) returns refcursor as
                                  $BODY$
                                  declare ref refcursor;
                                  begin
                                          open ref for select * from data;
                                          return ref;
                                  end
                                  $BODY$
                                  language 'plpgsql' volatile called on null input security invoker;");
                var cmd = new NpgsqlCommand("testtimestamptzparameter", Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimestampTZ));
                var dr = cmd.ExecuteReader();
                Assert.That(dr.Read(), Is.True);
                Assert.That(dr.Read(), Is.True);
                Assert.That(dr.Read(), Is.False);
                cmd.Dispose();
            }
        }

        [Test]
        public void DeriveParametersWithParameterNameFromFunction()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testoutparameter2(x int, y int, out sum int, out product int) as 'select $1 + $2, $1 * $2' language 'sql';");
            var command = new NpgsqlCommand("testoutparameter2", Conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(":x", command.Parameters[0].ParameterName);
            Assert.AreEqual(":y", command.Parameters[1].ParameterName);
        }

        [Test]
        public void VerifyFunctionWithNoParametersWithDeriveParameters()
        {
            var command = new NpgsqlCommand("funcb", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
        }

        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncall()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() returns setof data as '
                              select * from data;
                              ' language 'sql';");
            var command = new NpgsqlCommand("funcb", Conn);
            command.CommandType = CommandType.StoredProcedure;

            using (var dr = command.ExecuteReader(CommandBehavior.SingleRow)) {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(1, i);
            }
        }

        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncallPrepare()
        {
            //FIXME: Find a way of supporting single row with prepare.
            // Problem is that prepare plan must already have the limit 1 single row support.
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() returns setof data as '
                              select * from data;
                              ' language 'sql';");

            var command = new NpgsqlCommand("funcb()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Prepare();

            using (var dr = command.ExecuteReader(CommandBehavior.SingleRow)) {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(1, i);
            }
        }

        #region Setup / Teardown

        [SetUp]
        public void Setup()
        {
            base.SetUp();

            // Drop all functions in the public schema
            const string query =
               @"SELECT proname, oidvectortypes(proargtypes)
                 FROM pg_proc INNER JOIN pg_namespace ns ON pg_proc.pronamespace = ns.oid
                 WHERE ns.nspname = 'public'";

            var funcs = new List<Tuple<string, string>>();
            using (var cmd = new NpgsqlCommand(query, Conn))
            using (var rdr = cmd.ExecuteReader())
                while (rdr.Read())
                    funcs.Add(new Tuple<string, string>(rdr.GetString(0), rdr.GetString(1)));
            foreach (var func in funcs)
                ExecuteNonQuery(String.Format(@"DROP FUNCTION ""{0}"" ({1})", func.Item1, func.Item2));
        }
        #endregion Setup / Teardown
    }
}
