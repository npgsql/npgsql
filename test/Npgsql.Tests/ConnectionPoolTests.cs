#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class ConnectionPoolTests : TestBase
    {
        [Test]
        public void MinPoolSize()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=30;MaxPoolSize=30");
            conn.Open();
            conn.Close();

            conn = new NpgsqlConnection(ConnectionString + ";MaxPoolSize=30;MinPoolSize=30");
            conn.Open();
            conn.Close();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MinPoolSizeLargeThanMaxPoolSize()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=2;MaxPoolSize=1");
            conn.Open();
            conn.Close();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MinPoolSizeLargeThanPoolSizeLimit()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=1025;");
            conn.Open();
            conn.Close();
        }

        [Test, Description("Makes sure that when a pooled connection is closed it's properly reset, and that parameter settings aren't leaked")]
        public void ResetOnClose()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";SearchPath=public");
            conn.Open();
            ExecuteNonQuery("DROP SCHEMA IF EXISTS foo CASCADE");
            ExecuteNonQuery("CREATE SCHEMA foo");
            try
            {
                ExecuteNonQuery("SET search_path=foo", conn);
                conn.Close();
                conn.Open();
                Assert.That(ExecuteScalar("SHOW search_path", conn), Is.EqualTo("public"));
                conn.Close();
            }
            finally
            {
                ExecuteNonQuery("DROP SCHEMA foo");
            }
        }

        [Test]
        public void UseAllConnectionsInPool()
        {
            // As this method uses a lot of connections, clear all connections from all pools before starting.
            // This is needed in order to not reach the max connections allowed and start to raise errors.

            NpgsqlConnection.ClearAllPools();
            try {
                var openedConnections = new List<NpgsqlConnection>();
                // repeat test to exersize pool
                for (var i = 0; i < 10; ++i) {
                    try {
                        // 18 since base class opens two and the default pool size is 20
                        for (var j = 0; j < 18; ++j) {
                            var connection = new NpgsqlConnection(ConnectionString);
                            connection.Open();
                            openedConnections.Add(connection);
                        }
                    } finally {
                        openedConnections.ForEach(delegate(NpgsqlConnection con) { con.Dispose(); });
                        openedConnections.Clear();
                    }
                }
            } finally {
                NpgsqlConnection.ClearAllPools();
            }
        }

        [Test]
        [ExpectedException]
        public void ExceedConnectionsInPool()
        {
            var openedConnections = new List<NpgsqlConnection>();
            try {
                // exceed default pool size of 20
                for (var i = 0; i < 21; ++i) {
                    var connection = new NpgsqlConnection(ConnectionString + ";Timeout=1");
                    connection.Open();
                    openedConnections.Add(connection);
                }
            } finally {
                openedConnections.ForEach(delegate(NpgsqlConnection con) { con.Dispose(); });
                NpgsqlConnection.ClearAllPools();
            }
        }

        public ConnectionPoolTests(string backendVersion) : base(backendVersion) {}
    }
}
