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
    // At least most connection pool tests should actually be parallelizable, since they operate on their own specific pool.
    // Do this properly as part of the pool redo.
    [Parallelizable(ParallelScope.None)]
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
        public void MinPoolSizeLargeThanMaxPoolSize()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";MinPoolSize=2;MaxPoolSize=1");
            Assert.That(() => conn.Open(), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void MinPoolSizeLargeThanPoolSizeLimit()
        {
            Assert.That(() => new NpgsqlConnection(ConnectionString + ";MinPoolSize=1025;"), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test, Description("Makes sure that when a pooled connection is closed it's properly reset, and that parameter settings aren't leaked")]
        public void ResetOnClose()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";SearchPath=public");
            conn.Open();
            conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS foo CASCADE");
            conn.ExecuteNonQuery("CREATE SCHEMA foo");
            try
            {
                conn.ExecuteNonQuery("SET search_path=foo");
                conn.Close();
                conn.Open();
                Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.EqualTo("public"));
                conn.Close();
            }
            finally
            {
                using (conn = OpenConnection())
                    conn.ExecuteNonQuery("DROP SCHEMA foo");
            }
        }

        [Test]
        [Ignore("Very slow test, pool about to be rewritten anyway")]
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
        [Ignore("Very slow test, pool about to be rewritten anyway")]
        public void ExceedConnectionsInPool()
        {
            var openedConnections = new List<NpgsqlConnection>();
            try {
                // exceed default pool size of 20
                Assert.That(() => {
                    for (var i = 0; i < 21; ++i) {
                        var connection = new NpgsqlConnection(ConnectionString + ";Timeout=1");
                        connection.Open();
                        openedConnections.Add(connection);
                    }
                }, Throws.Exception);
            } finally {
                openedConnections.ForEach(delegate(NpgsqlConnection con) { con.Dispose(); });
                NpgsqlConnection.ClearAllPools();
            }
        }

        public ConnectionPoolTests(string backendVersion) : base(backendVersion) {}
    }
}
