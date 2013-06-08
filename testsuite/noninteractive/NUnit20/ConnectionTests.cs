// project created on 30/11/2002 at 22:00
//
// Author:
// 	Francisco Figueiredo Jr. <fxjrlists@yahoo.com>
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
    public class ConnectionTests : BaseClassTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _conn; }
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _t; }
            set { _t = value; }
        }
        protected virtual string TheConnectionString {
            get { return _connString; }
        }
        [Test]
        public void ChangeDatabase()
        {
            TheConnection.ChangeDatabase("template1");

            NpgsqlCommand command = new NpgsqlCommand("select current_database()", TheConnection);

            String result = (String)command.ExecuteScalar();

            Assert.AreEqual("template1", result);
        }

		[Test]
		public void ChangeDatabaseTestConnectionCache()
		{
			NpgsqlConnection conn1 = new NpgsqlConnection(TheConnectionString);
			NpgsqlConnection conn2 = new NpgsqlConnection(TheConnectionString);

			NpgsqlCommand command;

			//	connection 1 change database
			conn1.Open();
			conn1.ChangeDatabase("template1");
			command = new NpgsqlCommand("select current_database()", conn1);
			string db1 = (String)command.ExecuteScalar();

			Assert.AreEqual("template1", db1);

			//	connection 2 's database should not changed, so should different from conn1
			conn2.Open();
			command = new NpgsqlCommand("select current_database()", conn2);
			string db2 = (String)command.ExecuteScalar();

			Assert.AreNotEqual(db1, db2);

			conn1.Close();
			conn2.Close();
		}

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NestedTransaction()
        {
            NpgsqlTransaction t;
                
            t = TheConnection.BeginTransaction();
            if(t == null)
              return;
        }

        [Test]
        public void SequencialTransaction()
        {
            TheTransaction.Rollback();

            TheTransaction = TheConnection.BeginTransaction();
        }

        [Test]
        public void ConnectionRefused()
        {
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=44444;User Id=npgsql_tets;Password=j");

                conn.Open();
            }

            catch (NpgsqlException e)
            {
				Type type_NpgsqlState = typeof(NpgsqlConnection).Assembly.GetType("Npgsql.NpgsqlState");
				ResourceManager resman = new ResourceManager(type_NpgsqlState);
				string expected = string.Format(resman.GetString("Exception_FailedConnection"), "127.0.0.1");
				Assert.AreEqual(expected, e.Message);
            }
        }
		
		[Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void ConnectionStringWithEqualSignValue()
        {
            
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=44444;User Id=npgsql_tets;Password=j==");
            
            conn.Open();
            
        }
        
        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void ConnectionStringWithSemicolonSignValue()
        {
            
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=44444;User Id=npgsql_tets;Password='j;'");
            
            conn.Open();
            
        }
        
        [Test]
        public void SearchPathSupport()
        {
            
            NpgsqlConnection conn = new NpgsqlConnection(TheConnectionString + ";searchpath=public");
            try 
            {
                conn.Open();
            
                NpgsqlCommand c = new NpgsqlCommand("show search_path", conn);
                
                String searchpath = (String) c.ExecuteScalar();
                //Note, public is no longer implicitly added to paths, so this is no longer "public, public".
                Assert.AreEqual("public", searchpath );
                
            }
            
            finally 
            {
                conn.Close();
                
            }
            
            
            
        }
        
        [Test]
        public void ConnectorNotInitializedException1000581()
        {
            
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandText = @"SELECT 123";

            for(int i = 0; i < 2; i++)
            {
                NpgsqlConnection connection = new NpgsqlConnection(TheConnectionString);
                
                try 
                {
                    connection.Open();
                    command.Connection = connection;
                    command.Transaction = connection.BeginTransaction();
                    command.ExecuteScalar();
                    command.Transaction.Commit();
                  
                    
                } 
                finally 
                {
                     connection.Close();
                    
                }
               

            }
            
            
        }

        [Test]
        public void UseAllConnectionsInPool()
        {

            // As this method uses a lot of connections, clear all connections from all pools before starting. 
            // This is needed in order to not reach the max connections allowed and start to raise errors.

            NpgsqlConnection.ClearAllPools ();

            List<NpgsqlConnection> openedConnections = new List<NpgsqlConnection>();
            // repeat test to exersize pool
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    // 18 since base class opens two and the default pool size is 20
                    for (int j = 0; j < 18; ++j)
                    {
                        NpgsqlConnection connection = new NpgsqlConnection(TheConnectionString);
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

            NpgsqlConnection.ClearAllPools ();
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void ExceedConnectionsInPool()
        {
            List<NpgsqlConnection> openedConnections = new List<NpgsqlConnection>();
            try
            {
                // exceed default pool size of 20
                for (int i = 0; i < 21; ++i)
                {
                    NpgsqlConnection connection = new NpgsqlConnection(TheConnectionString);
                    connection.Open();
                    openedConnections.Add(connection);
                }
            }
            finally
            {
                openedConnections.ForEach(delegate(NpgsqlConnection con) { con.Dispose(); });
                NpgsqlConnection.ClearAllPools ();
            }
        }

        [Test]
        [Ignore]
        public void NpgsqlErrorRepro1()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(TheConnectionString))
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    LargeObjectManager largeObjectMgr = new LargeObjectManager(connection);
                    try
                    {
                        LargeObject largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        //Console.WriteLine(ex.ToString());
                        // ignore the LO failure
                    }
                } // *1* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (NpgsqlDataReader reader = command.ExecuteReader())
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
            
            string connString = "Server=server;Port=5432;User Id=user;Password=passwor;Database=database;";
    
            NpgsqlConnectionStringBuilder csb1 = new NpgsqlConnectionStringBuilder(connString);

            string cs1 = csb1.ToString();
        
            NpgsqlConnectionStringBuilder csb2 = new NpgsqlConnectionStringBuilder(cs1);
        
            string cs2 = csb2.ToString();
            
            Assert.IsTrue(cs1 == cs2);
            
                                
      
        }
        
        [Test]
        public void NpgsqlErrorRepro2()
        {
            NpgsqlConnection connection = new NpgsqlConnection(TheConnectionString);
            connection.Open();
            NpgsqlTransaction transaction = connection.BeginTransaction();
            LargeObjectManager largeObjectMgr = new LargeObjectManager(connection);
            try
            {
                LargeObject largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
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

            using (connection = new NpgsqlConnection(TheConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (NpgsqlDataReader reader = command.ExecuteReader())
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
            
            DataTable dt = TheConnection.GetSchema("ForeignKeys");
            
            Assert.IsNotNull(dt);
            
            
        }

		[Test]
		public void GetSchemaParameterMarkerFormats()
		{
			DataTable dt = TheConnection.GetSchema("DataSourceInformation");
			String parameterMarkerFormat = dt.Rows[0]["ParameterMarkerFormat"] as string;

			using (NpgsqlConnection connection = new NpgsqlConnection(TheConnectionString))
			{
				connection.Open();
				using (NpgsqlCommand command = connection.CreateCommand())
				{
					const String parameterName = "p_field_int4";
					command.CommandText = "SELECT * FROM tablea WHERE field_int4=" + String.Format(parameterMarkerFormat, parameterName);
					command.Parameters.Add(new NpgsqlParameter(parameterName,4));
					using (NpgsqlDataReader reader = command.ExecuteReader())
					{
						Assert.IsTrue(reader.Read());
						// This is OK, when no exceptions are occurred.
					}
				}
			}
		}

    }
    [TestFixture]
    public class ConnectionTestsV2 : ConnectionTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _connV2; }
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _tV2; }
            set { _tV2 = value; }
        }
        protected override string TheConnectionString {
            get { return _connV2String; }
        }
    }
}
