// created on 27/12/2002 at 17:05
//
// Author:
//     Francisco Figueiredo Jr. <fxjrlists@yahoo.com>
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
    public class DataReaderTests : TestBase
    {
        public DataReaderTests(string backendVersion) : base(backendVersion) { }

/*        [Test]
        public void TestNew()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from data where field_serial = 4;", Conn);

            command.Prepare();

            NpgsqlDataReaderNew dr = command.ExecuteReaderNew(CommandBehavior.Default);

            while(dr.Read());
        }*/

        [Test]
        public void RecordsAffected()
        {
            var command = new NpgsqlCommand("insert into data (field_int4) values (7); insert into data (field_int4) values (8)", Conn);
            using (var dr = command.ExecuteReader()) {
                Assert.AreEqual(2, dr.RecordsAffected);
            }
        }

        [Test]
        public void RecordsAffectedSelect()
        {
            var command = new NpgsqlCommand("SELECT * FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Close();
                Assert.That(dr.RecordsAffected, Is.EqualTo(-1));
            }
        }

        [Test]
        public void RecordsAffectedUpdateZero()
        {
            var command = new NpgsqlCommand("UPDATE data SET field_int4=8", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Close();
                Assert.That(dr.RecordsAffected, Is.EqualTo(0));
            }
        }

        [Test]
        public void GetBoolean()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_bool) VALUES (true)");
            var command = new NpgsqlCommand(@"SELECT field_bool FROM DATA", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetBoolean(0);
                Assert.AreEqual(true, result);
            }
        }

        [Test]
        public void GetChars()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Random text')");
            var command = new NpgsqlCommand("SELECT field_text FROM DATA", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = new Char[6];
                dr.GetChars(0, 0, result, 0, 6);
                Assert.AreEqual("Random", new String(result));
            }
        }

        [Test]
        public void GetCharsSequential()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Random text')");
            var command = new NpgsqlCommand("SELECT field_text FROM data;", Conn);
            using (var dr = command.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                dr.Read();
                var result = new Char[6];
                dr.GetChars(0, 0, result, 0, 6);
                Assert.AreEqual("Random", new String(result));
            }
        }

        [Test]
        public void GetBytes1()
        {
            ExecuteNonQuery(string.Format(@"INSERT INTO data (field_bytea) VALUES ({0}'\{1}123\{1}056')", ! Conn.UseConformantStrings && Conn.Supports_E_StringPrefix ? "E" : "", Conn.UseConformantStrings ? "" : @"\"));
            var command = new NpgsqlCommand("SELECT field_bytea FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = new Byte[2];

                var a = dr.GetBytes(0, 0, result, 0, 2);
                var b = dr.GetBytes(0, result.Length, result, 0, 2);

                Assert.AreEqual('S', (Char) result[0]);
                Assert.AreEqual('.', (Char) result[1]);
                Assert.AreEqual(2, a);
                Assert.AreEqual(0, b);
            }
        }

        [Test]
        public void GetBytes2()
        {
            var command = new NpgsqlCommand(string.Format(@"select {0}'\{1}001\{1}002\{1}003'::bytea;", ! Conn.UseConformantStrings && Conn.Supports_E_StringPrefix ? "E" : "", Conn.UseConformantStrings ? "" : @"\"), Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = new Byte[3];

                var a = dr.GetBytes(0, 0, result, 0, 0);
                var b = dr.GetBytes(0, 0, result, 0, 1);
                var c = dr.GetBytes(0, 0, result, 0, 2);
                var d = dr.GetBytes(0, 0, result, 0, 3);

                Assert.AreEqual(1, result[0]);
                Assert.AreEqual(2, result[1]);
                Assert.AreEqual(3, result[2]);
                Assert.AreEqual(0, a);
                Assert.AreEqual(1, b);
                Assert.AreEqual(2, c);
                Assert.AreEqual(3, d);
            }
        }

        [Test]
        [Ignore]
        public void GetBytesSequential()
        {
            var command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                dr.Read();
                var result = new Byte[2];

                var a = dr.GetBytes(0, 0, result, 0, 2);
                var b = dr.GetBytes(0, result.Length, result, 0, 2);

                Assert.AreEqual('S', (Char)result[0]);
                Assert.AreEqual('.', (Char)result[1]);
                Assert.AreEqual(2, a);
                Assert.AreEqual(0, b);
            }
        }

        [Test]
        public void GetInt32()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            var command = new NpgsqlCommand("SELECT field_int4 FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetInt32(0);
                //ConsoleWriter cw = new ConsoleWriter(Console.Out);
                //cw.WriteLine(result.GetType().Name);
                Assert.AreEqual(4, result);
            }
        }

        [Test]
        public void GetInt16()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int2) VALUES (2)");
            var command = new NpgsqlCommand("SELECT field_int2 FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetInt16(0);
                Assert.AreEqual(2, result);
            }
        }

        [Test]
        public void GetDecimal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_numeric) VALUES (4.23)");
            var command = new NpgsqlCommand(@"SELECT field_numeric FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(0);
                Assert.AreEqual(4.2300000M, result);
            }
        }

        [Test]
        public void GetDouble()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_float8) VALUES (.123456789012345)");
            var command = new NpgsqlCommand("SELECT field_float8 FROM data;", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                //Double result = Double.Parse(dr.GetInt32(2).ToString());
                var result = dr.GetDouble(0);
                Assert.AreEqual(.123456789012345D, result);
            }
        }

        [Test]
        public void GetFloat()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_float4) VALUES (.123456)");
            var command = new NpgsqlCommand("SELECT field_float4 FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                //Single result = Single.Parse(dr.GetInt32(2).ToString());
                var result = dr.GetFloat(0);
                Assert.AreEqual(.123456F, result);
            }
        }

        [Test]
        public void GetMoney()
        {
            var cmd = new NpgsqlCommand("select :param", Conn);
            const decimal monAmount = 1234.5m;
            cmd.Parameters.Add("param", NpgsqlDbType.Money).Value = monAmount;
            using (var rdr = cmd.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(monAmount, rdr[0]);
            }
        }

        [Test]
        public void GetString()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Random text')");
            var command = new NpgsqlCommand("SELECT field_text FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetString(0);
                Assert.AreEqual("Random text", result);
            }
        }

        [Test]
        public void GetStringWithParameter()
        {
            const string text = "Random text";
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", text));

            var command = new NpgsqlCommand("SELECT field_text FROM data WHERE field_text = :value;", Conn);
            var param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = text.Length;
            param.Value = text;
            command.Parameters.Add(param);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetString(0);
                Assert.AreEqual(text, result);
            }
        }

        [Test]
        public void GetStringWithQuoteWithParameter()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Text with '' single quote')");

            const string test = "Text with ' single quote";
            var command = new NpgsqlCommand("SELECT field_text FROM data WHERE field_text = :value;", Conn);

            var param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetString(0);
                Assert.AreEqual(test, result);
            }
        }

        [Test]
        public void GetValueByName()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Random text')");
            var command = new NpgsqlCommand(@"SELECT field_text FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = (String) dr["field_text"];
                Assert.AreEqual("Random text", result);
            }
        }

        [Test]
        public void ExcuteReaderGettingEmptyResultSetWithOutputParameter()
        {
            var command = new NpgsqlCommand("select * from data where field_text = NULL;", Conn);
            var param = new NpgsqlParameter("some_param", NpgsqlDbType.Varchar);
            param.Direction = ParameterDirection.Output;
            command.Parameters.Add(param);
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                Assert.IsFalse(dr.NextResult());
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValueFromEmptyResultset()
        {
            var command = new NpgsqlCommand("select * from data where field_text = :value;", Conn);

            const string test = "Text single quote";
            var param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                // This line should throw the invalid operation exception as the datareader will
                // have an empty resultset.
                Console.WriteLine(dr.IsDBNull(1));
            }
        }

        [Test]
        public void GetInt32ArrayFieldType()
        {
            var command = new NpgsqlCommand("select cast(null as integer[])", Conn);
            using (var dr = command.ExecuteReader())
            {
                Assert.AreEqual(typeof(int[]), dr.GetFieldType(0));
            }
        }

        [Test]
        public void TestMultiDimensionalArray()
        {
            var command = new NpgsqlCommand("select :i", Conn);
            command.Parameters.AddWithValue(":i", (new decimal[,]{{0,1,2},{3,4,5}}));
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(2, (dr[0] as Array).Rank);
                var da = (decimal[,])dr[0];
                Assert.AreEqual(da.GetUpperBound(0), 1);
                Assert.AreEqual(da.GetUpperBound(1), 2);
                decimal cmp = 0m;
                foreach(decimal el in da)
                    Assert.AreEqual(el, cmp++);
            }
        }

        [Test]
        public void TestArrayOfBytea1()
        {
            var command = new NpgsqlCommand("select get_byte(:i[1], 2)", Conn);
            command.Parameters.AddWithValue(":i", new byte[][]{new byte[]{0,1,2}, new byte[]{3,4,5}});
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr[0], 2);
            }
        }

        [Test]
        public void TestArrayOfBytea2()
        {
            var command = new NpgsqlCommand("select get_byte(:i[1], 2)", Conn);
            command.Parameters.AddWithValue(":i", new byte[][]{new byte[]{1,2,3}, new byte[]{4,5,6}});
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr[0], 3);
            }
        }

        [Test]
        public void TestOverlappedParameterNames()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :test_name or field_serial = :test_name_long", Conn);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "a"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            using (var dr = command.ExecuteReader())
            {
                Assert.IsNotNull(dr);
            }
        }

        [Test]
        public void TestOverlappedParameterNamesWithPrepare()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :test_name or field_serial = :test_name_long", Conn);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "abc_de"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "abc_defg"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            command.Prepare();

            using (var dr = command.ExecuteReader())
            {
                Assert.IsNotNull(dr);
            }
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void TestNonExistentParameterName()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :a or field_serial = :aa", Conn);
            command.Parameters.Add(new NpgsqlParameter(":b", DbType.Int32, 4, "b"));
            command.Parameters.Add(new NpgsqlParameter(":aa", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            using (var dr = command.ExecuteReader())
            {
                Assert.IsNotNull(dr);
            }
        }

        [Test]
        public void UseDataAdapter()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            var da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapter.xml");
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            command.Connection = Conn;
            var da = new NpgsqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            var da = new NpgsqlDataAdapter("select * from data", Conn);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            var da = new NpgsqlDataAdapter("select * from data", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            var da = new NpgsqlDataAdapter("select * from data", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        public void DataGridWebControlSupport()
        {
            var command = new NpgsqlCommand("select * from data;", Conn);
            var dr = command.ExecuteReader();
            //Console.WriteLine(dr.FieldCount);
            var dg = new DataGrid();
            dg.DataSource = dr;
            dg.DataBind();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadPastDataReaderEnd()
        {
            var command = new NpgsqlCommand("select * from data;", Conn);
            var dr = command.ExecuteReader();
            while (dr.Read()) {}
            Object o = dr[0];
            Assert.IsNotNull(o);
        }

        [Test]
        public void IsDBNull()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text, field_int4) VALUES ('X', 1)");
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (2)");
            var command = new NpgsqlCommand("SELECT field_text FROM data ORDER BY field_int4", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.IsFalse(dr.IsDBNull(0));
                dr.Read();
                Assert.IsTrue(dr.IsDBNull(0));
            }
        }

        [Test]
        public void TypesNames()
        {
            var command = new NpgsqlCommand("SELECT field_int2, field_int4, field_int8, field_numeric, field_text, field_bool, field_timestamp FROM data WHERE 1 = 2;", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual("int2", dr.GetDataTypeName(0));
                Assert.AreEqual("int4", dr.GetDataTypeName(1));
                Assert.AreEqual("int8", dr.GetDataTypeName(2));
                Assert.AreEqual("numeric", dr.GetDataTypeName(3));
                Assert.AreEqual("text", dr.GetDataTypeName(4));
                Assert.AreEqual("bool", dr.GetDataTypeName(5));
                Assert.AreEqual("timestamp", dr.GetDataTypeName(6));
            }
        }

        [Test]
        public void SingleRowCommandBehaviorSupport()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            var command = new NpgsqlCommand(@"SELECT * FROM data", Conn);
            var dr = command.ExecuteReader(CommandBehavior.SingleRow);
            var i = 0;
            while (dr.Read())
                i++;
            Assert.AreEqual(1, i);
        }

        [Test]
        public void SingleRowSequentialCommandBehaviorSupport()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text, field_bool) VALUES ('X', true)");
            ExecuteNonQuery(@"INSERT INTO data (field_text, field_bool) VALUES ('Y', false)");
            var command = new NpgsqlCommand("SELECT field_int4, field_text, field_char5, field_float4, field_date, field_bool FROM data ORDER BY field_text", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess))
            {
                Assert.That(dr.Read(), Is.True);
                Assert.IsTrue(dr.IsDBNull(0));  // field_int4
                Assert.IsFalse(dr.IsDBNull(1)); // field_text
                Assert.That(dr.GetString(1), Is.EqualTo("X"));
                Assert.IsTrue(dr.IsDBNull(2));  // field_char5
                // field_float4 - do not read this field to test skip
                Assert.IsTrue(dr.IsDBNull(4));  // field_date
                Assert.IsFalse(dr.IsDBNull(5)); // field_bool
                Assert.That(dr.GetValue(5), Is.True);

                Assert.That(dr.Read(), Is.False);
            }
        }

        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncall()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            ExecuteNonQuery(@"DROP FUNCTION IF EXISTS funcB()");
            ExecuteNonQuery(@"CREATE FUNCTION funcB() returns setof data as '
                              select * from data;
                              ' language 'sql';");
            var command = new NpgsqlCommand("funcb", Conn);
            command.CommandType = CommandType.StoredProcedure;

            using (var dr = command.ExecuteReader(CommandBehavior.SingleRow))
            {
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
            ExecuteNonQuery(@"DROP FUNCTION IF EXISTS funcB()");
            ExecuteNonQuery(@"CREATE FUNCTION funcB() returns setof data as '
                              select * from data;
                              ' language 'sql';");

            var command = new NpgsqlCommand("funcb()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Prepare();

            using (var dr = command.ExecuteReader(CommandBehavior.SingleRow))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(1, i);
            }
        }

        [Test, Description("In sequential access, performing a null check on a non-first field would check the first field")]
        public void SequentialNullCheckOnNonFirstField()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            using (var cmd = new NpgsqlCommand("SELECT field_text, field_int4 FROM data", Conn))
            using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                dr.Read();
                Assert.That(dr.IsDBNull(1), Is.True);
            }
        }

        #region GetSchemaTable

        [Test]
        public void PrimaryKeyFieldsMetadataSupport()
        {
            ExecuteNonQuery("DROP TABLE IF EXISTS DATA2 CASCADE");
            ExecuteNonQuery(@"CREATE TABLE DATA2 (
                                field_pk1                      INT2 NOT NULL,
                                field_pk2                      INT2 NOT NULL,
                                field_serial                   SERIAL,
                                CONSTRAINT data2_pkey PRIMARY KEY (field_pk1, field_pk2)
                                ) WITH OIDS");

            using (var command = new NpgsqlCommand("SELECT * FROM DATA2", Conn))
            {
                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    dr.Read();
                    var metadata = dr.GetSchemaTable();
                    int keyCount = 0;

                    foreach (DataRow r in metadata.Rows)
                    {
                        if ((Boolean)r["IsKey"])
                        {
                            switch (keyCount)
                            {
                                case 0:
                                    Assert.AreEqual("field_pk1", r["ColumnName"]);
                                    break;
                                case 1:
                                    Assert.AreEqual("field_pk2", r["ColumnName"]);
                                    break;
                                default:
                                    break;
                            }

                            keyCount++;
                        }

                    }
                    if (keyCount == 0)
                        Assert.Fail("No primary key found!");
                    else if (keyCount != 2)
                        Assert.Fail(string.Format("Expected 2 primary keys but {0} were found.", keyCount));
                }
            }

            ExecuteNonQuery("DROP TABLE IF EXISTS DATA2 CASCADE");
        }

        [Test]
        public void PrimaryKeyFieldMetadataSupport()
        {
            using (var command = new NpgsqlCommand("SELECT * FROM data", Conn))
            {
                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    dr.Read();
                    var metadata = dr.GetSchemaTable();
                    var keyfound = false;

                    foreach (DataRow r in metadata.Rows)
                    {
                        if ((Boolean)r["IsKey"])
                        {
                            Assert.AreEqual("field_pk", r["ColumnName"]);
                            keyfound = true;
                        }

                    }
                    if (!keyfound)
                        Assert.Fail("No primary key found!");
                }
            }
        }

        [Test]
        public void IsAutoIncrementMetadataSupport()
        {
            var command = new NpgsqlCommand("SELECT * FROM data", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
            {
                var metadata = dr.GetSchemaTable();

                foreach (DataRow r in metadata.Rows)
                {
                    switch ((string)r["ColumnName"])
                    {
                        case "field_serial":
                        case "field_bigserial":
                        case "field_smallserial": // 9.2 and later
                            Assert.IsTrue((bool)r["IsAutoIncrement"]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [Test]
        public void IsReadOnlyMetadataSupport()
        {
            ExecuteNonQuery("CREATE OR REPLACE VIEW DataView (field_pk, field_int2) AS select field_pk, field_int2 + field_int2 as field_int2 from data");

            var command = new NpgsqlCommand("SELECT * FROM DataView", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
            {
                var metadata = dr.GetSchemaTable();

                foreach (DataRow r in metadata.Rows)
                {
                    switch ((string)r["ColumnName"])
                    {
                        case "field_pk":
                            if (Conn.PostgreSqlVersion < new Version("9.4"))
                            {
                                // 9.3 and earlier: IsUpdatable = False
                                Assert.IsTrue((bool)r["IsReadonly"], "field_pk");
                            }
                            else
                            {
                                // 9.4: IsUpdatable = True
                                Assert.IsFalse((bool)r["IsReadonly"], "field_pk");
                            }
                            break;
                        case "field_int2":
                            Assert.IsTrue((bool)r["IsReadonly"], "field_int2");
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        [Test]
        public void IsIdentityMetadataSupport()
        {
            DoIsIdentityMetadataSupport();
        }

        public virtual void DoIsIdentityMetadataSupport()
        {
            var command = new NpgsqlCommand("SELECT * FROM data", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
            {
                var metadata = dr.GetSchemaTable();

                foreach (DataRow r in metadata.Rows)
                {
                    if (((string)r["ColumnName"]) == "field_serial")
                    {
                        Assert.IsTrue((bool)r["IsAutoIncrement"]);
                        return;
                    }
                }
            }
        }

        [Test]
        public void HasRowsWithoutResultset()
        {
            var command = new NpgsqlCommand("delete from data where field_serial = 2000000", Conn);
            var dr = command.ExecuteReader();
            Assert.IsFalse(dr.HasRows);
        }

        [Test]
        public void ParameterAppearMoreThanOneTime()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :parameter and field_int4 = :parameter", Conn);
            command.Parameters.Add("parameter", NpgsqlDbType.Integer);
            command.Parameters["parameter"].Value = 1;
            using (var dr = command.ExecuteReader())
            {
                Assert.IsFalse(dr.HasRows);
            }
        }

        [Test]
        public void SchemaOnlySingleRowCommandBehaviorSupport()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.SingleRow))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(0, i);
            }
        }

        [Test]
        public void SchemaOnlyCommandBehaviorSupport()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(0, i);
            }
        }

        [Test]
        public void SchemaOnlyCommandBehaviorSupportFunctioncall()
        {
            ExecuteNonQuery(@"DROP FUNCTION IF EXISTS funcB()");
            ExecuteNonQuery(@"CREATE FUNCTION funcB() returns setof data as '
                              select * from data;
                              ' language 'sql';");
            var command = new NpgsqlCommand("funcb", Conn);
            command.CommandType = CommandType.StoredProcedure;
            using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(0, i);
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldNameDoesntExistOnGetOrdinal()
        {
            var command = new NpgsqlCommand("select field_serial from data", Conn);

            using (var dr = command.ExecuteReader())
            {
                int idx =  dr.GetOrdinal("field_int");
            }
        }

        [Test]
        public void FieldNameDoesntExistBackwardsCompat()
        {
            using (var backCompatCon = new NpgsqlConnection(ConnectionString + ";Compatible=2.0.2"))
                using (var command = new NpgsqlCommand("select field_serial from data", backCompatCon))
                {
                    backCompatCon.Open();
                    using (var rdr = command.ExecuteReader())
                        Assert.AreEqual(rdr.GetOrdinal("field_int"), -1);
                }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldNameDoesntExist()
        {
            var command = new NpgsqlCommand("select field_serial from data", Conn);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Object a = dr["field_int"];
                Assert.IsNotNull(a);
            }
        }

        [Test]
        public void FieldNameKanaWidthWideRequestForNarrowFieldName()
        {//Should ignore Kana width and hence find the first of these two fields
            var command = new NpgsqlCommand("select 123 as ｦｧｨｩｪｫｬ, 124 as ヲァィゥェォャ", Conn);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr["ｦｧｨｩｪｫｬ"], 123);
                Assert.AreEqual(dr["ヲァィゥェォャ"], 123);// Wide version.
            }
        }

        [Test]
        public void FieldNameKanaWidthNarrowRequestForWideFieldName()
        {//Should ignore Kana width and hence find the first of these two fields
            var command = new NpgsqlCommand("select 123 as ヲァィゥェォャ, 124 as ｦｧｨｩｪｫｬ", Conn);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr["ヲァィゥェォャ"], 123);
                Assert.AreEqual(dr["ｦｧｨｩｪｫｬ"], 123);// Narrow version.
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldIndexDoesntExist()
        {
            var command = new NpgsqlCommand("select field_serial from data", Conn);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Object a = dr[5];
                Assert.IsNotNull(a);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteReaderBeforeClosingReader()
        {
            if (Conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();

            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);
            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            using (var dr2 = cmd2.ExecuteReader())
            {
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteScalarBeforeClosingReader()
        {
            if (Conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();

            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);

            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            {
                cmd2.ExecuteScalar();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteNonQueryBeforeClosingReader()
        {
            if (Conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();

            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);
            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            {
                cmd2.ExecuteNonQuery();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrepareBeforeClosingReader()
        {
            if (Conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();

            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);
            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            {
                cmd2.Prepare();
            }
        }

        [Test]
        public void LoadDataTable()
        {
            using (var command = new NpgsqlCommand("SELECT field_char5, field_varchar5 FROM data", Conn))
            using (var dr = command.ExecuteReader())
            {
                var dt = new DataTable();
                dt.Load(dr);
                dr.Close();

                Assert.AreEqual(5, dt.Columns[0].MaxLength);
                Assert.AreEqual(5, dt.Columns[1].MaxLength);
            }
        }

        [Test]
        public void CleansupOkWithDisposeCalls()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                dr.Close();

                using (var upd = Conn.CreateCommand())
                {
                    upd.CommandText = "select * from data";
                    upd.Prepare();
                }
            }
        }

        [Test]
        public void TestOutParameter2()
        {
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
        public void GetValueWithNullFields()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Random text')");
            var command = new NpgsqlCommand(@"SELECT field_int4 FROM data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.IsDBNull(0);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void HasRowsGetValue()
        {
            var command = new NpgsqlCommand("select 1", Conn);
            using (var dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.HasRows);
                Assert.IsTrue(dr.Read());
                Assert.IsTrue(dr.HasRows);
                Assert.AreEqual(1, dr.GetValue(0));
            }
        }

        [Test]
        public void IntervalAsTimeSpan()
        {
            var command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = Conn;

            using (var dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.HasRows);
                Assert.IsTrue(dr.Read());
                Assert.IsTrue(dr.HasRows);
                var ts = dr.GetTimeSpan(0);
            }
        }
    }
/*
    [TestFixture]
    public class DataReaderTestsV2 : DataReaderTests
    {
        protected override int BackendProtocolVersion { get { return 2; } }
        public override void DoIsIdentityMetadataSupport()
        {
            //Not possible with V2?
        }
    }
 */
}
