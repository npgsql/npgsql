using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class CopyTests : TestBase
    {
        public CopyTests(string backendVersion) : base(backendVersion) {}

        [SetUp]
        public void Bomb()
        {
            throw new NotImplementedException("Temporarily disabled for v3.0 refactor");
        }

        [Test]
        [SetCulture("nl-BE")]
        public void InvariantCultureNpgsqlCopySerializer()
        {
            // Test for https://github.com/npgsql/Npgsql/pull/92
            // SetCulture is used to set a culture where a comma is used to separate decimal values (0,5) which will cause problems if Npgsql 
            // doesn't convert correctly to use a point. (0.5)

            var cmd = new NpgsqlCommand("COPY data (field_int4, field_int8, field_float4) FROM STDIN", Conn);
            var npgsqlCopySerializer = new NpgsqlCopySerializer(Conn);
            var npgsqlCopyIn = new NpgsqlCopyIn(cmd, Conn, npgsqlCopySerializer.ToStream);

            npgsqlCopyIn.Start();
            npgsqlCopySerializer.AddInt32(300000);
            npgsqlCopySerializer.AddInt64(1000000);
            npgsqlCopySerializer.AddNumber(0.5);
            npgsqlCopySerializer.EndRow();
            npgsqlCopySerializer.Flush();
            npgsqlCopyIn.End();

            NpgsqlDataReader dr = new NpgsqlCommand("select field_int4, field_int8, field_float4 from data", Conn).ExecuteReader();
            dr.Read();

            Assert.AreEqual(300000, dr[0]);
            Assert.AreEqual(1000000, dr[1]);
            Assert.AreEqual(0.5, dr[2]);
        }

        [Test]
        public void Bug188BufferNpgsqlCopySerializer()
        {
            var cmd = new NpgsqlCommand("COPY data (field_int4, field_text) FROM STDIN", Conn);
            var npgsqlCopySerializer = new NpgsqlCopySerializer(Conn);
            var npgsqlCopyIn = new NpgsqlCopyIn(cmd, Conn, npgsqlCopySerializer.ToStream);

            string str = "Very long string".PadRight(NpgsqlCopySerializer.DEFAULT_BUFFER_SIZE, 'z');

            npgsqlCopyIn.Start();
            npgsqlCopySerializer.AddInt32(12345678);
            npgsqlCopySerializer.AddString(str);
            npgsqlCopySerializer.EndRow();
            npgsqlCopySerializer.Flush();
            npgsqlCopyIn.End();



            NpgsqlDataReader dr = new NpgsqlCommand("select field_int4, field_text from data", Conn).ExecuteReader();
            dr.Read();

            Assert.AreEqual(12345678, dr[0]);
            Assert.AreEqual(str, dr[1]);
        }

        [Test]
        public void Bug219NpgsqlCopyInConcurrentUsage()
        {

            try
            {
                // Create temporary test tables

                ExecuteNonQuery(@"CREATE TABLE Bug219_table1 (
                                            id integer,
                                            name character varying(100)
                                            )
                                            WITH (
                                            OIDS=FALSE
                                            );");

                ExecuteNonQuery(@"CREATE TABLE Bug219_table2 (
                                            id integer,
                                            null1 integer,
                                            name character varying(100),
                                            null2 integer,
                                            description character varying(1000),
                                            null3 integer
                                            )
                                            WITH (
                                            OIDS=FALSE
                                            );");



                using (var connection1 = new NpgsqlConnection(ConnectionString))
                using (var connection2 = new NpgsqlConnection(ConnectionString))
                {

                    connection1.Open();
                    connection2.Open();

                    var copy1 = new NpgsqlCopyIn("COPY Bug219_table1 FROM STDIN;", connection1);
                    var copy2 = new NpgsqlCopyIn("COPY Bug219_table2 FROM STDIN;", connection2);

                    copy1.Start();
                    copy2.Start();

                    NpgsqlCopySerializer cs1 = new NpgsqlCopySerializer(connection1);
                    //NpgsqlCopySerializer cs2 = new NpgsqlCopySerializer(connection2);

                    for (int index = 0; index < 10; index++)
                    {
                        cs1.AddInt32(index);
                        cs1.AddString(string.Format("Index {0} ", index));
                        cs1.EndRow();

                        /*cs2.AddInt32(index);
                        cs2.AddNull();
                        cs2.AddString(string.Format("Index {0} ", index));
                        cs2.AddNull();
                        cs2.AddString("jjjjj");
                        cs2.AddNull();
                        cs2.EndRow();*/

                    }
                    cs1.Close(); //Exception
                    //cs2.Close();

                    copy1.End();
                    copy2.End();

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ExecuteNonQuery(@"DROP TABLE IF EXISTS Bug219_table1");
                ExecuteNonQuery(@"DROP TABLE IF EXISTS Bug219_table2");
            }
        }

        [Test]
        public void Bug221MillisecondsFieldNotCopied()
        {
            // Test for https://github.com/npgsql/Npgsql/issues/221
            // The milliseconds field is not properly copied in NpgsqlCopySerializer.cs in method AddDateTime

            var cmd = new NpgsqlCommand("COPY data (field_timestamp) FROM STDIN", Conn);
            var npgsqlCopySerializer = new NpgsqlCopySerializer(Conn);
            var npgsqlCopyIn = new NpgsqlCopyIn(cmd, Conn, npgsqlCopySerializer.ToStream);
            var testDate = DateTime.Parse("2002-02-02 09:00:23.005");

            npgsqlCopyIn.Start();
            npgsqlCopySerializer.AddDateTime(testDate);
            npgsqlCopySerializer.EndRow();
            npgsqlCopySerializer.Flush();
            npgsqlCopyIn.End();

            NpgsqlDataReader dr = new NpgsqlCommand("select field_timestamp from data", Conn).ExecuteReader();
            dr.Read();

            Assert.AreEqual(testDate, dr[0]);
        }
    }
}
