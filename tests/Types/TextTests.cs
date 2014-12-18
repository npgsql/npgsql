using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void Read(PrepareOrNot prepare, CommandBehavior behavior)
        {
            var builder = new StringBuilder("ABCDEééé", Conn.BufferSize);
            builder.Append('X', Conn.BufferSize);
            var expected = builder.ToString();
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", expected));

            const string queryText = @"SELECT field_text, 'foo', field_text, field_text, field_text, field_text FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
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
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void GetChars(PrepareOrNot prepare, CommandBehavior behavior)
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
            Assert.That(reader.GetString(3), Is.EqualTo("bar"));
            reader.GetChars(4, 0, actual, 0, 2);
            // Jump to another column from the middle of the column
            reader.GetChars(5, 0, actual, 0, 2);
            // Close in the middle of a column
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential")]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential")]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential")]
        public void GetTextReader(PrepareOrNot prepare, CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            const string str = "ABCDE";
            var expected = str.ToCharArray();
            var actual = new char[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", str));

            const string queryText = @"SELECT field_text, 'foo' FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            var textReader = reader.GetTextReader(0);
            textReader.Read(actual, 0, 2);
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            Assert.That(() => reader.GetString(1), Throws.Exception.TypeOf<InvalidOperationException>(), "Access row while streaming");
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

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential")]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential")]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential")]
        public void Null(PrepareOrNot prepare, CommandBehavior behavior)
        {
            var buf = new char[8];
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES (NULL)");
            var cmd = new NpgsqlCommand("SELECT field_text FROM data", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();
            Assert.That(reader.IsDBNull(0), Is.True);
            Assert.That(() => reader.GetChars(0, 0, buf, 0, 1), Throws.Exception, "GetChars");
            Assert.That(() => reader.GetTextReader(0), Throws.Exception, "GetTextReader");
            Assert.That(() => reader.GetChars(0, 0, null, 0, 0), Throws.Exception, "GetChars with null buffer");
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("Tests some types which are aliased to strings")]
        [TestCase("varchar")]
        [TestCase("name")]
        public void AliasedTypes(string typename)
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

        // Older tests from here

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
    }
}
