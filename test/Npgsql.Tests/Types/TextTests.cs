#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

using System;
using System.Data;
using System.Text;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL text
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-character.html
    /// </remarks>
    public class TextTests : TestBase
    {
        [Test, Description("Roundtrips a string")]
        public void Roundtrip()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6, @p7", conn))
            {
                const string expected = "Something";
                var expectedBytes = Encoding.UTF8.GetBytes(expected);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Text);
                var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Varchar);
                var p3 = new NpgsqlParameter("p3", DbType.String);
                var p4 = new NpgsqlParameter { ParameterName = "p4", Value = expected };
                var p5 = new NpgsqlParameter("p5", NpgsqlDbType.Text);
                var p6 = new NpgsqlParameter("p6", NpgsqlDbType.Text);
                var p7 = new NpgsqlParameter("p7", NpgsqlDbType.Text);
                Assert.That(p2.DbType, Is.EqualTo(DbType.String));
                Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text));
                Assert.That(p3.DbType, Is.EqualTo(DbType.String));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                cmd.Parameters.Add(p5);
                cmd.Parameters.Add(p6);
                cmd.Parameters.Add(p7);
                p1.Value = p2.Value = p3.Value = expected;
                p5.Value = expected.ToCharArray();
                p6.Value = new ArraySegment<char>(("X" + expected).ToCharArray(), 1, expected.Length);
                p7.Value = expectedBytes;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(string)));
                        Assert.That(reader.GetString(i), Is.EqualTo(expected));
                        Assert.That(reader.GetFieldValue<string>(i), Is.EqualTo(expected));
                        Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                        Assert.That(reader.GetFieldValue<char[]>(i), Is.EqualTo(expected.ToCharArray()));
                        Assert.That(reader.GetFieldValue<byte[]>(i), Is.EqualTo(expectedBytes));
                    }
                }
            }
        }

        [Test]
        public void Long([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            using (var conn = OpenConnection())
            {
                var builder = new StringBuilder("ABCDEééé", conn.Settings.WriteBufferSize);
                builder.Append('X', conn.Settings.WriteBufferSize);
                var expected = builder.ToString();
                using (var cmd = new NpgsqlCommand(@"INSERT INTO data (name) VALUES (@p)", conn))
                {
                    conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                    cmd.Parameters.Add(new NpgsqlParameter("p", expected));
                    cmd.ExecuteNonQuery();
                }

                const string queryText = @"SELECT name, 'foo', name, name, name, name FROM data";
                using (var cmd = new NpgsqlCommand(queryText, conn))
                {
                    var reader = cmd.ExecuteReader(behavior);
                    reader.Read();

                    var actual = reader[0];
                    Assert.That(actual, Is.EqualTo(expected));

                    if (behavior.IsSequential())
                        Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                        Assert.That(reader[0], Is.EqualTo(expected));

                    Assert.That(reader.GetString(1), Is.EqualTo("foo"));
                    Assert.That(reader.GetFieldValue<string>(2), Is.EqualTo(expected));
                    Assert.That(reader.GetValue(3), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<string>(4), Is.EqualTo(expected));
                    //Assert.That(reader.GetFieldValue<char[]>(5), Is.EqualTo(expected.ToCharArray()));
                }
            }
        }

        [Test, Description("Tests that strings are truncated when the NpgsqlParameter's Size is set")]
        public void Truncate()
        {
            const string data = "SomeText";
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::TEXT", conn))
            {
                var p = new NpgsqlParameter("p", data) { Size = 4 };
                cmd.Parameters.Add(p);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data.Substring(0, 4)));

                // NpgsqlParameter.Size needs to persist when value is changed
                const string data2 = "AnotherValue";
                p.Value = data2;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2.Substring(0, 4)));

                // NpgsqlParameter.Size larger than the value size should mean the value size, as well as 0 and -1
                p.Size = data2.Length + 10;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));
                p.Size = 0;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));
                p.Size = -1;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));

                Assert.That(() => p.Size = -2, Throws.Exception.TypeOf<ArgumentException>());
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/488")]
        public void NullCharacter()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", "string with \0\0\0 null \0bytes"));
                Assert.That(() => cmd.ExecuteReader(),
                    Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("22021")
                    );
            }
        }

        [Test, Description("Tests some types which are aliased to strings")]
        [TestCase("Varchar")]
        [TestCase("Name")]
        public void AliasedPgTypes(string typename)
        {
            const string expected = "some_text";
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($"SELECT '{expected}'::{typename}", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetString(0), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<char[]>(0), Is.EqualTo(expected.ToCharArray()));
            }
        }


        [Test]
        [TestCase(DbType.AnsiString)]
        [TestCase(DbType.AnsiStringFixedLength)]
        public void AliasedDbTypes(DbType dbType)
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT @p", conn))
            {
                command.Parameters.Add(new NpgsqlParameter("p", dbType) { Value = "SomeString" });
                Assert.That(command.ExecuteScalar(), Is.EqualTo("SomeString"));
            }
        }

        [Test, Description("Tests the PostgreSQL internal \"char\" type")]
        public void InternalChar([Values(true, false)] bool prepareCommand)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                var testArr = new byte[] { prepareCommand ? (byte)200 : (byte)'}', prepareCommand ? (byte)0 : (byte)'"', 3 };
                var testArr2 = new char[] { prepareCommand ? (char)200 : '}', prepareCommand ? (char)0 : '"', (char)3 };

                cmd.CommandText = "Select 'a'::\"char\", (-3)::\"char\", :p1, :p2, :p3, :p4, :p5";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.InternalChar) { Value = 'b' });
                cmd.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.InternalChar) { Value = (byte)66 });
                cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.InternalChar) { Value = (byte)230 });
                cmd.Parameters.Add(new NpgsqlParameter("p4", NpgsqlDbType.InternalChar | NpgsqlDbType.Array) { Value = testArr });
                cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlDbType.InternalChar | NpgsqlDbType.Array) { Value = testArr2 });
                if (prepareCommand)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var expected = new char[] { 'a', (char)(256 - 3), 'b', (char)66, (char)230 };
                    for (var i = 0; i < expected.Length; i++)
                    {
                        Assert.AreEqual(expected[i], reader.GetChar(i));
                    }
                    var arr = (char[])reader.GetValue(5);
                    var arr2 = (char[])reader.GetValue(6);
                    Assert.AreEqual(testArr.Length, arr.Length);
                    for (var i = 0; i < arr.Length; i++)
                    {
                        Assert.AreEqual(testArr[i], arr[i]);
                        Assert.AreEqual(testArr2[i], arr2[i]);
                    }
                }
            }
        }

        [Test]
        public void Char()
        {
            var expected = 'f';
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetChar(0), Is.EqualTo(expected));
                    Assert.That(reader.GetString(0), Is.EqualTo(expected.ToString()));
                }
            }
        }

        [Test, Description("Checks support for the citext contrib type")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/695")]
        public void Citext()
        {
            using (var conn = OpenConnection())
            {
                if (conn.PostgreSqlVersion >= new Version(9, 1, 0))
                {
                    conn.ExecuteNonQuery("CREATE EXTENSION IF NOT EXISTS citext");
                    conn.ReloadTypes();
                }

                var value = "Foo";
                using (var cmd = new NpgsqlCommand("SELECT @p::CITEXT", conn))
                {
                    cmd.Parameters.AddWithValue("p", value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetString(0), Is.EqualTo(value));
                    }
                }
                using (var cmd = new NpgsqlCommand("SELECT @p1::CITEXT = @p2::CITEXT", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Citext, "abc");
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Citext, "ABC");
                    Assert.That(cmd.ExecuteScalar(), Is.True);
                }
            }
        }
    }
}
