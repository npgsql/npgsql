using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using System;
using System.Data;
using System.Configuration;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;



namespace NpgsqlTests
{
    [TestFixture]
    public class BackslashTests: BaseClassTests
    {

        protected override NpgsqlConnection TheConnection
        {
            get { return _conn; }
        }
        protected override NpgsqlTransaction TheTransaction
        {
            get { return _t; }
            set { _t = value; }
        }
        protected virtual string TheConnectionString
        {
            get { return _connString; }
        }

        [Test]
        public void TestStoreUsingSQL()
        {
            CommitTransaction = true;
            const string testString = "av\\fs\\dgdg\t\n\b\f\n\t";

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = TheConnection;
            command.CommandText = "INSERT INTO tablea(\"field_text\") VALUES(@field_text)";
            command.Parameters.AddWithValue("field_text", testString);
            command.ExecuteNonQuery();

            command.CommandText = "SELECT field_text FROM tablea WHERE oid =" + command.LastInsertedOID;
            var result = command.ExecuteScalar();

            if (result != null)
                Assert.AreEqual(testString, result.ToString());
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestSingleUsingSQL()
        {
            CommitTransaction = true;
            const string testString = @"\";

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = TheConnection;
            command.CommandText = "INSERT INTO tablea(\"field_text\") VALUES(@field_text)";
            command.Parameters.AddWithValue("field_text", testString);
            command.ExecuteNonQuery();

            command.CommandText = "SELECT field_text FROM tablea WHERE oid =" + command.LastInsertedOID;
            var result = command.ExecuteScalar();

            if (result != null)
                Assert.AreEqual(testString, result.ToString());
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestStoreUsignEF()
        {
            string testString = "av\\fs\\dgdg\t\n\b\f\n\t";

            var context = new TestContext();
            var obj = new Tablea()
                {
                    Field_text = testString,
                };
            context.Tablea.Add(obj);
            context.SaveChanges();
            context.Dispose();

            context = new TestContext();
            var result = context.Tablea.FirstOrDefault(x => x.Field_serial == obj.Field_serial);
            if (result != null)
            {
                Assert.AreEqual(result.Field_text, testString);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestSingleUsignEF()
        {
            const string testString = @"\";

            var context = new TestContext();
            var obj = new Tablea()
            {
                Field_text = testString,
            };
            context.Tablea.Add(obj);
            context.SaveChanges();
            context.Dispose();

            context = new TestContext();
            var result = context.Tablea.FirstOrDefault(x => x.Field_serial == obj.Field_serial);
            if (result != null)
            {
                Assert.AreEqual(result.Field_text, testString);
            }
            else
            {
                Assert.Fail();
            }
        }

    }
}
