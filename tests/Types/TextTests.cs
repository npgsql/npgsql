using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL text
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-character.html
    /// </remarks>
    public class TextTests : TestBase
    {
        public TextTests(string backendVersion) : base(backendVersion) {}

        [Test, Description("Roundtrips a string")]
        public void Roundtrip()
        {
            const string expected = "Something";
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Text);
            var p2 = new NpgsqlParameter("p2", DbType.String);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text));
            Assert.That(p3.DbType, Is.EqualTo(DbType.String));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i),          Is.EqualTo(typeof(string)));
                Assert.That(reader.GetString(i),             Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<string>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i),              Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<char[]>(i), Is.EqualTo(expected.ToCharArray()));
            }

            reader.Close();
            cmd.Dispose();            
        }

        [Test]
        public void Long([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            var builder = new StringBuilder("ABCDEééé", Conn.BufferSize);
            builder.Append('X', Conn.BufferSize);
            var expected = builder.ToString();
            var cmd = new NpgsqlCommand(@"INSERT INTO data (field_text) VALUES (@p)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("p", expected));
            cmd.ExecuteNonQuery();

            const string queryText = @"SELECT field_text, 'foo', field_text, field_text, field_text, field_text FROM data";
            cmd = new NpgsqlCommand(queryText, Conn);
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            var actual = reader[0];
            Assert.That(actual, Is.EqualTo(expected));

            if (IsSequential(behavior))
                Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
            else
                Assert.That(reader[0], Is.EqualTo(expected));

            Assert.That(reader.GetString(1), Is.EqualTo("foo"));
            Assert.That(reader.GetFieldValue<string>(2), Is.EqualTo(expected));
            Assert.That(reader.GetValue(3), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<string>(4), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<char[]>(5), Is.EqualTo(expected.ToCharArray()));
        }

        [Test]
        public void GetChars([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            const string str = "ABCDE";
            var expected = str.ToCharArray();
            var actual = new char[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", str));

            const string queryText = @"SELECT field_text, 3, field_text, 4, field_text, field_text, field_text FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            Assert.That(reader.GetChars(0, 0, actual, 0, 2), Is.EqualTo(2));
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            Assert.That(reader.GetChars(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");
            // Note: Unlike with bytea, finding out the length of the column consumes it (variable-width
            // UTF8 encoding)
            Assert.That(reader.GetChars(2, 0, actual, 0, 2), Is.EqualTo(2));
            if (IsSequential(behavior))
                Assert.That(() => reader.GetChars(2, 0, actual, 4, 1), Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
            else
            {
                Assert.That(reader.GetChars(2, 0, actual, 4, 1), Is.EqualTo(1));
                Assert.That(actual[4], Is.EqualTo(expected[0]));
            }
            Assert.That(reader.GetChars(2, 2, actual, 2, 3), Is.EqualTo(3));
            Assert.That(actual, Is.EqualTo(expected));
            //Assert.That(reader.GetChars(2, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");

            Assert.That(() => reader.GetChars(3, 0, null, 0, 0), Throws.Exception.TypeOf<InvalidCastException>(), "GetChars on non-text");
            Assert.That(() => reader.GetChars(3, 0, actual, 0, 1), Throws.Exception.TypeOf<InvalidCastException>(), "GetChars on non-text");
            Assert.That(reader.GetInt32(3), Is.EqualTo(4));
            reader.GetChars(4, 0, actual, 0, 2);
            // Jump to another column from the middle of the column
            reader.GetChars(5, 0, actual, 0, 2);
            Assert.That(reader.GetChars(5, expected.Length - 1, actual, 0, 2), Is.EqualTo(1), "Length greater than data length");
            Assert.That(actual[0], Is.EqualTo(expected[expected.Length - 1]), "Length greater than data length");
            Assert.That(() => reader.GetChars(5, 0, actual, 0, actual.Length + 1), Throws.Exception.TypeOf<IndexOutOfRangeException>(), "Length great than output buffer length");
            // Close in the middle of a column
            reader.GetChars(6, 0, actual, 0, 2);
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void GetTextReader([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            const string str = "ABCDE";
            var expected = str.ToCharArray();
            var actual = new char[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", str));

            const string queryText = @"SELECT field_text, 'foo' FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            var textReader = reader.GetTextReader(0);
            textReader.Read(actual, 0, 2);
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            if (behavior == CommandBehavior.Default) {
                var textReader2 = reader.GetTextReader(0);
                var actual2 = new char[2];
                textReader2.Read(actual2, 0, 2);
                Assert.That(actual2[0], Is.EqualTo(expected[0]));
                Assert.That(actual2[1], Is.EqualTo(expected[1]));
            } else {
                Assert.That(() => reader.GetTextReader(0), Throws.Exception.TypeOf<InvalidOperationException>(), "Sequential text reader twice on same column");
            }
            textReader.Read(actual, 2, 1);
            Assert.That(actual[2], Is.EqualTo(expected[2]));
            textReader.Close();

            if (IsSequential(behavior))
                Assert.That(() => reader.GetChars(0, 0, actual, 4, 1), Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
            else
            {
                Assert.That(reader.GetChars(0, 0, actual, 4, 1), Is.EqualTo(1));
                Assert.That(actual[4], Is.EqualTo(expected[0]));
            }
            Assert.That(reader.GetString(1), Is.EqualTo("foo"));
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("In sequential mode, checks that moving to the next column disposes a currently open text reader")]
        public void TextReaderDisposeOnSequentialColumn()
        {
            var cmd = new NpgsqlCommand(@"SELECT 'some_text', 'some_text'", Conn);
            var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            reader.Read();
            var textReader = reader.GetTextReader(0);
            // ReSharper disable once UnusedVariable
            var v = reader.GetValue(1);
            Assert.That(() => textReader.Peek(), Throws.Exception.TypeOf<ObjectDisposedException>());
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("In non-sequential mode, checks that moving to the next row disposes all currently open text readers")]
        public void TextReaderDisposeOnNonSequentialRow()
        {
            var cmd = new NpgsqlCommand(@"SELECT 'some_text', 'some_text'", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            var tr1 = reader.GetTextReader(0);
            var tr2 = reader.GetTextReader(0);
            reader.Read();
            Assert.That(() => tr1.Peek(), Throws.Exception.TypeOf<ObjectDisposedException>());
            Assert.That(() => tr2.Peek(), Throws.Exception.TypeOf<ObjectDisposedException>());
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void Null([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            var buf = new char[8];
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES (NULL)");
            var cmd = new NpgsqlCommand("SELECT field_text FROM data", Conn);
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();
            Assert.That(reader.IsDBNull(0), Is.True);
            Assert.That(() => reader.GetChars(0, 0, buf, 0, 1), Throws.Exception, "GetChars");
            Assert.That(() => reader.GetTextReader(0), Throws.Exception, "GetTextReader");
            Assert.That(() => reader.GetChars(0, 0, null, 0, 0), Throws.Exception, "GetChars with null buffer");
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("Tests that strings are truncated when the NpgsqlParameter's Size is set")]
        public void Truncate()
        {
            const string data = "SomeText";
            var cmd = new NpgsqlCommand("SELECT @p::TEXT", Conn);
            var p = new NpgsqlParameter("p", data) { Size = 4 };
            cmd.Parameters.Add(p);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data.Substring(0, 4)));

            // NpgsqlParameter.Size needs to persist when value is changed
            const string data2 = "AnotherValue";
            p.Value = data2;
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2.Substring(0, 4)));

            // NpgsqlParameter.Size larger than the value size should mean the value size
            p.Size = data2.Length + 10;
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/488")]
        public void NullCharacter()
        {
            var cmd = new NpgsqlCommand("SELECT * FROM data WHERE field_text = :p1", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("p1", "string with \0\0\0 null \0bytes"));
            Assert.That(() => cmd.ExecuteReader(),
                Throws.Exception.TypeOf<NpgsqlException>()
                .With.Property("Code").EqualTo("22021")
            );
        }

        [Test, Description("Tests some types which are aliased to strings")]
        [TestCase("varchar")]
        [TestCase("name")]
        public void AliasedPgTypes(string typename)
        {
            const string expected = "some_text";
            var cmd = new NpgsqlCommand(String.Format("SELECT '{0}'::{1}", expected, typename), Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<char[]>(0), Is.EqualTo(expected.ToCharArray()));
            reader.Dispose();
            cmd.Dispose();
        }


        [Test]
        [TestCase(DbType.AnsiString)]
        [TestCase(DbType.AnsiStringFixedLength)]
        public void AliasedDbTypes(DbType dbType)
        {
            using (var command = new NpgsqlCommand("SELECT @p", Conn))
            {
                command.Parameters.Add(new NpgsqlParameter("p", dbType) { Value = "SomeString" });
                Assert.That(command.ExecuteScalar(), Is.EqualTo("SomeString"));
            }
        }

        // Older tests from here

        [Test]
        public void CharParameterValueSupport()
        {
            const String query = @"create temp table test ( tc char(1) );
                                   insert into test values(' ');
                                   select * from test where tc=:charparam";
            var command = new NpgsqlCommand(query, Conn);
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "charparam";

            // Exception Can't cast System.Char into any valid DbType.
            sqlParam.Value = ' ';
            command.Parameters.Add(sqlParam);
            var res = (String)command.ExecuteScalar();

            Assert.AreEqual(" ", res);
        }

        [Test]
        public void TestCharParameterLength()
        {
            const string sql = "insert into data(field_char5) values ( :a );";
            const string aValue = "atest";
            var command = new NpgsqlCommand(sql, Conn);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Char));
            command.Parameters[":a"].Value = aValue;
            command.Parameters[":a"].Size = 5;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);

            var command2 = new NpgsqlCommand("select field_char5 from data where field_serial = (select max(field_serial) from data)", Conn);
            using (var dr = command2.ExecuteReader()) {
                dr.Read();
                String a = dr.GetString(0);
                Assert.AreEqual(aValue, a);
            }
        }

        [Test]
        public void GetCharsOld()
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
        public void SingleChar([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = Conn.CreateCommand())
            {
                var testArr = new byte[] { prepareCommand ? (byte)200 : (byte)'}', prepareCommand ? (byte)0 : (byte)'"', 3 };
                var testArr2 = new char[] { prepareCommand ? (char)200 : '}', prepareCommand ? (char)0 : '"', (char)3 };

                cmd.CommandText = "Select 'a'::\"char\", (-3)::\"char\", :p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.SingleChar) { Value = 'b' });
                cmd.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.SingleChar) { Value = 66 });
                cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.SingleChar) { Value = "" });
                cmd.Parameters.Add(new NpgsqlParameter("p4", NpgsqlDbType.SingleChar) { Value = "\0" });
                cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlDbType.SingleChar) { Value = "a" });
                cmd.Parameters.Add(new NpgsqlParameter("p6", NpgsqlDbType.SingleChar) { Value = (byte)231 });
                cmd.Parameters.Add(new NpgsqlParameter("p7", NpgsqlDbType.SingleChar | NpgsqlDbType.Array) { Value = testArr });
                cmd.Parameters.Add(new NpgsqlParameter("p8", NpgsqlDbType.SingleChar | NpgsqlDbType.Array) { Value = testArr2 });
                if (prepareCommand)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var expected = new char[] { 'a', (char)(256 - 3), 'b', (char)66, '\0', '\0', 'a', (char)231 };
                    for (int i = 0; i < expected.Length; i++)
                    {
                        Assert.AreEqual(expected[i], reader.GetChar(i));
                    }
                    var arr = (char[])reader.GetValue(8);
                    var arr2 = (char[])reader.GetValue(9);
                    Assert.AreEqual(testArr.Length, arr.Length);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        Assert.AreEqual(testArr[i], arr[i]);
                        Assert.AreEqual(testArr2[i], arr2[i]);
                    }
                }
            }
        }
    }
}
