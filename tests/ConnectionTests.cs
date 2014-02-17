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

            if (connection.PostgreSqlVersion < new Version(8, 3, 0)
                || new NpgsqlConnectionStringBuilder(ConnectionString).Protocol == ProtocolVersion.Version2)
            {
                connection.Close();
                return;
            }

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
                    const String parameterName = "p_field_int4";
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
                string extraDigits = (string) c.ExecuteScalar();
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
        public void ChangeApplicationNameWithConnectionStringBuilder()
        {
            // Test for issue #165 on github.
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.ApplicationName = "test";
        }


    }
}
