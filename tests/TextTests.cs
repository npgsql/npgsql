using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class TextTests : TestBase
    {
        public TextTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public void GetStringSequential([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";BufferSize=16"))
            {
                conn.Open();
                const string expected = "This string is longer than the connection buffer";
                using (var cmd = new NpgsqlCommand(String.Format("SELECT '{0}'", expected), conn))
                using (var rdr = cmd.ExecuteReader(behavior))
                {
                    rdr.Read();
                    Assert.That(rdr.GetString(0), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void Read(PrepareOrNot prepare, CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            const string str = "ABCDE";
            var expected = str.ToCharArray();
            var actual = new char[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", str));

            const string queryText = @"SELECT field_text, 'foo', field_text, 'bar', field_text, field_text FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            Assert.That(reader.GetChars(0, 0, actual, 0, 2), Is.EqualTo(2));
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            //Assert.That(reader.GetChars(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");
            Assert.That(() => reader.GetChars(0, expected.Length + 1, actual, 0, 1), Throws.Exception, "GetChars from after column ends");
            // Note that the column was consumed in the previous line
            Assert.That(reader.GetChars(2, 0, actual, 0, 2), Is.EqualTo(2));
            if (IsSequential(behavior))
                Assert.That(() => reader.GetChars(2, 0, actual, 4, 1), Throws.Exception, "Seek back sequential");
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
            Assert.That(reader.GetString(3), Is.EqualTo("bar"));
            reader.GetChars(4, 0, actual, 0, 2);
            // Jump to another column from the middle of the column
            reader.GetChars(5, 0, actual, 0, 2);
            // Close in the middle of a column
            reader.Close();
            cmd.Dispose();

            // Redo the query to test GetTextReader()
            cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            reader = cmd.ExecuteReader(behavior);
            reader.Read();
            var textReader = reader.GetTextReader(2);
            textReader.Read(actual, 0, 2);
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            textReader.Close();
            if (IsSequential(behavior))
                Assert.That(() => reader.GetChars(0, 0, actual, 4, 1), Throws.Exception, "Seek back sequential");
            else
            {
                Assert.That(reader.GetChars(0, 0, actual, 4, 1), Is.EqualTo(1));
                Assert.That(actual[4], Is.EqualTo(expected[0]));
            }
            Assert.That(reader.GetString(3), Is.EqualTo("bar"));
            reader.Close();
            cmd.Dispose();

            // Test null column
            ExecuteNonQuery(@"TRUNCATE data");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES (NULL)");
            cmd = new NpgsqlCommand("SELECT field_text FROM data", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            reader = cmd.ExecuteReader(behavior);
            reader.Read();
            Assert.That(() => reader.GetChars(0, 0, null, 0, 0), Throws.Exception, "GetBytes with empty buffer on null");
            Assert.That(() => reader.GetChars(0, 0, actual, 0, 1), Throws.Exception, "GetBytes on null");
            Assert.That(reader.IsDBNull(0), Is.True);
            reader.Close();
            cmd.Dispose();

            //var result = (byte[]) cmd.ExecuteScalar();
            //Assert.AreEqual(2, result.Length);
        }
    }
}
