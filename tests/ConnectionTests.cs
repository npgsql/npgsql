// project created on 30/11/2002 at 22:00
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
using Npgsql;
using System.Data;
using System.Resources;
using NUnit.Framework;
using System.Collections.Generic;
using NpgsqlTypes;

namespace NpgsqlTests
{
    [TestFixture]
    public class ConnectionTests : TestBase
    {
        public ConnectionTests(string backendVersion) : base(backendVersion) { }

        [Test]
        public void ChangeDatabase()
        {
            Conn.ChangeDatabase("template1");
            var command = new NpgsqlCommand("select current_database()", Conn);
            var result = (String)command.ExecuteScalar();
            Assert.AreEqual("template1", result);
        }

        [Test]
        public void ChangeDatabaseTestConnectionCache()
        {
            using (var conn1 = new NpgsqlConnection(ConnectionString))
            using (var conn2 = new NpgsqlConnection(ConnectionString))
            {
                //    connection 1 change database
                conn1.Open();
                conn1.ChangeDatabase("template1");
                var command = new NpgsqlCommand("select current_database()", conn1);
                var db1 = (String)command.ExecuteScalar();
                Assert.AreEqual("template1", db1);

                //    connection 2 's database should not changed, so should different from conn1
                conn2.Open();
                command = new NpgsqlCommand("select current_database()", conn2);
                var db2 = (String)command.ExecuteScalar();
                Assert.AreNotEqual(db1, db2);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NestedTransaction()
        {
            using (Conn.BeginTransaction())
            {
                using (Conn.BeginTransaction())
                {
                }
            }
        }

        [Test]
        public void SequencialTransaction()
        {
            Conn.BeginTransaction().Rollback();
            Conn.BeginTransaction();
        }

        [Test]
        public void ConnectionRefused()
        {
            try
            {
                var conn = new NpgsqlConnection("Server=127.0.0.1;Port=44444;User Id=npgsql_tets;Password=j");
                conn.Open();
            }
            catch (NpgsqlException e)
            {
                var type_NpgsqlState = typeof(NpgsqlConnection).Assembly.GetType("Npgsql.NpgsqlState");
                var resman = new ResourceManager(type_NpgsqlState);
                var expected = string.Format(resman.GetString("Exception_FailedConnection"), "127.0.0.1");
                Assert.AreEqual(expected, e.Message);
            }
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void ConnectionStringWithEqualSignValue()
        {
            var conn = new NpgsqlConnection("Server=127.0.0.1;Port=44444;User Id=npgsql_tets;Password=j==");
            conn.Open();
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void ConnectionStringWithSemicolonSignValue()
        {
            var conn = new NpgsqlConnection("Server=127.0.0.1;Port=44444;User Id=npgsql_tets;Password='j;'");
            conn.Open();
        }

        [Test]
        public void ConnectionMinPoolSize()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=30;MaxPoolSize=30");
            conn.Open();
            conn.Close();

            conn = new NpgsqlConnection(ConnectionString + ";MaxPoolSize=30;MinPoolSize=30");
            conn.Open();
            conn.Close();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConnectionMinPoolSizeLargeThanMaxPoolSize()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=2;MaxPoolSize=1");
            conn.Open();
            conn.Close();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConnectionMinPoolSizeLargeThanPoolSizeLimit()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=1025;");
            conn.Open();
            conn.Close();
        }

        [Test]
        public void SearchPathSupport()
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";searchpath=public"))
            {
                conn.Open();
                var c = new NpgsqlCommand("show search_path", conn);
                var searchpath = (String)c.ExecuteScalar();
                //Note, public is no longer implicitly added to paths, so this is no longer "public, public".
                Assert.AreEqual("public", searchpath);
            }
        }

        [Test]
        public void ConnectorNotInitializedException1000581()
        {
            var command = new NpgsqlCommand();
            command.CommandText = @"SELECT 123";

            for (var i = 0; i < 2; i++)
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.Transaction = connection.BeginTransaction();
                    command.ExecuteScalar();
                    command.Transaction.Commit();
                }
            }
        }

        [Test]
        public void UseAllConnectionsInPool()
        {
            // As this method uses a lot of connections, clear all connections from all pools before starting.
            // This is needed in order to not reach the max connections allowed and start to raise errors.

            NpgsqlConnection.ClearAllPools();
            try
            {
                var openedConnections = new List<NpgsqlConnection>();
                // repeat test to exersize pool
                for (var i = 0; i < 10; ++i)
                {
                    try
                    {
                        // 18 since base class opens two and the default pool size is 20
                        for (var j = 0; j < 18; ++j)
                        {
                            var connection = new NpgsqlConnection(ConnectionString);
                            connection.Open();
                            openedConnections.Add(connection);
                        }
                    }
                    finally
                    {
                        openedConnections.ForEach(delegate(NpgsqlConnection con) { con.Dispose(); });
                        openedConnections.Clear();
                    }
                }
            }
            finally
            {
                NpgsqlConnection.ClearAllPools();
            }
        }

        [Test]
        [ExpectedException]
        public void ExceedConnectionsInPool()
        {
            var openedConnections = new List<NpgsqlConnection>();
            try
            {
                // exceed default pool size of 20
                for (var i = 0; i < 21; ++i)
                {
                    var connection = new NpgsqlConnection(ConnectionString + ";Timeout=1");
                    connection.Open();
                    openedConnections.Add(connection);
                }
            }
            finally
            {
                openedConnections.ForEach(delegate(NpgsqlConnection con) { con.Dispose(); });
                NpgsqlConnection.ClearAllPools();
            }
        }

        [Test]
        [Ignore]
        public void NpgsqlErrorRepro1()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var largeObjectMgr = new LargeObjectManager(connection);
                    try
                    {
                        var largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                        transaction.Commit();
                    }
                    catch
                    {
                        // ignore the LO failure
                    }
                } // *1* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // *2* this fails if the initial connection is used
                    }
                }
            } // *3* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"
        }

        [Test]
        public void Bug1011001()
        {
            //[#1011001] Bug in NpgsqlConnectionStringBuilder affects on cache and connection pool

            var csb1 = new NpgsqlConnectionStringBuilder(@"Server=server;Port=5432;User Id=user;Password=passwor;Database=database;");
            var cs1 = csb1.ToString();
            var csb2 = new NpgsqlConnectionStringBuilder(cs1);
            var cs2 = csb2.ToString();
            Assert.IsTrue(cs1 == cs2);
        }

        [Test]
        public void Bug1011241_DiscardAll()
        {

            var connection = new NpgsqlConnection(ConnectionString + ";SearchPath=public");
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SHOW SEARCH_PATH";
                Assert.AreEqual("public", command.ExecuteScalar());

                command.CommandText = "SET SEARCH_PATH = \"$user\"";
                command.ExecuteNonQuery();
                command.CommandText = "SHOW SEARCH_PATH";
                Assert.AreEqual("\"$user\"", command.ExecuteScalar());
            }
            connection.Close();

            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SHOW SEARCH_PATH";
                Assert.AreEqual("public", command.ExecuteScalar());
            }
            connection.Close();

        }

        [Test]
        public void NpgsqlErrorRepro2()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            var largeObjectMgr = new LargeObjectManager(connection);
            try
            {
                var largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                transaction.Commit();
            }
            catch
            {
                // ignore the LO failure
                try
                {
                    transaction.Dispose();
                }
                catch
                {
                    // ignore dispose failure
                }
                try
                {
                    connection.Dispose();
                }
                catch
                {
                    // ignore dispose failure
                }
            }

            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        // *1* this fails if the connection for the pool happens to be the bad one from above
                        Assert.IsTrue(!String.IsNullOrEmpty((string)reader["datname"]));
                    }
                }
            }
        }

        [Test]
        public void GetSchemaForeignKeys()
        {
            var dt = Conn.GetSchema("ForeignKeys");
            Assert.IsNotNull(dt);
        }

        [Test]
        public void ChangeState()
        {
            using (var c = new NpgsqlConnection(ConnectionString))
            {
                var stateChangeCalledForOpen = false;
                var stateChangeCalledForClose = false;

                c.StateChange += new StateChangeEventHandler(delegate(object sender, StateChangeEventArgs e)
                {
                    if (e.OriginalState == ConnectionState.Closed && e.CurrentState == ConnectionState.Open)
                        stateChangeCalledForOpen = true;

                    if (e.OriginalState == ConnectionState.Open && e.CurrentState == ConnectionState.Closed)
                        stateChangeCalledForClose = true;
                });

                c.Open();
                c.Close();

                Assert.IsTrue(stateChangeCalledForOpen);
                Assert.IsTrue(stateChangeCalledForClose);
            }
        }

        [Test]
        public void GetSchemaParameterMarkerFormats()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            var dt = Conn.GetSchema("DataSourceInformation");
            var parameterMarkerFormat = (string)dt.Rows[0]["ParameterMarkerFormat"];

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    const String parameterName = "@p_field_int4";
                    command.CommandText = "SELECT * FROM data WHERE field_int4=" + String.Format(parameterMarkerFormat, parameterName);
                    command.Parameters.Add(new NpgsqlParameter(parameterName, 4));
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        // This is OK, when no exceptions are occurred.
                    }
                }
            }
        }

        [Test]
        public void CheckExtraFloatingDigitsHigherThanTwo()
        {

            using (NpgsqlCommand c = new NpgsqlCommand("show extra_float_digits", Conn))
            {
                string extraDigits = (string)c.ExecuteScalar();
                if (Conn.PostgreSqlVersion >= new Version(9, 0, 0))
                {
                    Assert.AreEqual(extraDigits, "3");
                }
                else
                {
                    Assert.AreEqual(extraDigits, "2");
                }
            }
        }


        [Test]
        public void GetConnectionState()
        {
            // Test created to PR #164

            NpgsqlConnection c = new NpgsqlConnection();
            c.Dispose();

            Assert.AreEqual(ConnectionState.Closed, c.State);



        }

        [Test]
        public void ChangeApplicationNameWithConnectionStringBuilder()
        {
            // Test for issue #165 on github.
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.ApplicationName = "test";
        }


        [Test]
        public void GetSchema()
        {
            using (NpgsqlConnection c = new NpgsqlConnection())
            {
                DataTable metaDataCollections = c.GetSchema();
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more metadatacollections returned. No connectionstring is required.");
            }
        }

        [Test]
        public void GetSchemaWithDbMetaDataCollectionNames()
        {
            DataTable metaDataCollections = Conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);
            Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more metadatacollections returned.");
            foreach (DataRow row in metaDataCollections.Rows)
            {
                var collectionName = (string)row["CollectionName"];
                //checking this collection
                if (collectionName != System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections)
                {
                    var collection = Conn.GetSchema(collectionName);
                    Assert.IsNotNull(collection, "Each of the advertised metadata collections should work");
                }
            }
        }

        [Test]
        public void GetSchemaWithRestrictions()
        {
            DataTable metaDataCollections = Conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
            Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more Restrictions returned.");
        }

        [Test]
        public void GetSchemaWithReservedWords()
        {
            DataTable metaDataCollections = Conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
            Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more ReservedWords returned.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InternalCommandTimeoutIsNotIntegerException()
        {
            // Test for issue #1366 on github.
            using (var connection = new NpgsqlConnection(ConnectionString + ";InternalCommandTimeout=adc"))
            {
                connection.Open();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InternalCommandTimeoutIsNegativeException()
        {
            // Test for issue #1366 on github.
            using (var connection = new NpgsqlConnection(ConnectionString + ";InternalCommandTimeout=-3"))
            {
                connection.Open();
            }
        }

        private void InternalCommandTimeoutTestHelper(NpgsqlConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = new NpgsqlCommand(
@"DROP SCHEMA IF EXISTS issue_1366 CASCADE;
CREATE SCHEMA issue_1366;
CREATE TABLE issue_1366.section
(
  id bigint NOT NULL,
  section_name character varying NOT NULL,
  CONSTRAINT section_pkey PRIMARY KEY (id)
);
CREATE TABLE issue_1366.section_items
(
  id bigint NOT NULL,
  section_id bigint NOT NULL,
  item_name character varying NOT NULL,
  previous_item bigint,
  CONSTRAINT section_items_pkey PRIMARY KEY (id),
  CONSTRAINT section_items_previous_item_fkey FOREIGN KEY (previous_item)
	  REFERENCES issue_1366.section_items (id) MATCH SIMPLE
	  ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED,
  CONSTRAINT section_items_section_id_fkey FOREIGN KEY (section_id)
	  REFERENCES issue_1366.section (id) MATCH SIMPLE
	  ON UPDATE RESTRICT ON DELETE CASCADE,
  CONSTRAINT section_items_section_id_excl EXCLUDE 
  USING btree (section_id WITH =) WHERE (previous_item IS NULL)
  DEFERRABLE INITIALLY DEFERRED,
  CONSTRAINT section_items_section_id_previous_item_key UNIQUE (section_id, previous_item),
  CONSTRAINT section_items_check CHECK (id <> previous_item)
);"
                    , connection))
                {
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            using (var transaction = connection.BeginTransaction())
            {
                using (var command = new NpgsqlCommand(
@"-- Delete all data
TRUNCATE TABLE issue_1366.section CASCADE;

-- We need just one section
INSERT INTO issue_1366.section(id, section_name)
VALUES(1, 'Section1');

-- Let's add the list of items
INSERT INTO issue_1366.section_items(id, section_id, item_name, previous_item)
SELECT id, 1, 'Item' || id, CASE WHEN id <= 1 THEN NULL ELSE id - 1 END FROM generate_series(1, 50000) AS s(id);"
                    , connection))
                {
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            using (var transaction = connection.BeginTransaction())
            {
                using (var command = new NpgsqlCommand(
@"-- Let's remove every second item...
DELETE FROM issue_1366.section_items WHERE id % 2 = 0;

-- ...and update positions of items in the list
UPDATE issue_1366.section_items SET previous_item = previous_item - 1 WHERE previous_item IS NOT NULL;"
                    , connection))
                {
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        [Test]
        public void SetInternalCommandTimeoutToInfinity()
        {
            // Test for issue #1366 on github.
            using (var connection = new NpgsqlConnection(ConnectionString + ";CommandTimeout=3;InternalCommandTimeout=0"))
            {
                connection.Open();

                InternalCommandTimeoutTestHelper(connection);
            }
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void SetInternalCommandTimeoutToSmallValueException()
        {
            // Test for issue #1366 on github.
            using (var connection = new NpgsqlConnection(ConnectionString + ";CommandTimeout=3;InternalCommandTimeout=3"))
            {
                connection.Open();

                InternalCommandTimeoutTestHelper(connection);
            }
        }
    }
}
