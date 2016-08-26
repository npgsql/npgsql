using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class PrepareTests: TestBase
    {
        [Test, Description("Basic prepared system scenario. Checks proper backend deallocation of the statement.")]
        public void Basic()
        {
            using (var conn = OpenConnection())
            {
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));

                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                    Assert.That(cmd.IsPrepared, Is.True);
                    Assert.That(cmd.IsPersistent, Is.False);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
            }
        }

        [Test]
        public void Parameters()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER, long BIGINT)");
                using (var command = new NpgsqlCommand("select * from data where int = :a and long = :b;", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
                    command.Parameters.Add(new NpgsqlParameter("b", DbType.Int64));
                    Assert.AreEqual(2, command.Parameters.Count);
                    Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

                    command.Prepare();
                    command.Parameters[0].Value = 3;
                    command.Parameters[1].Value = 5;
                    using (var dr = command.ExecuteReader())
                    {
                        Assert.IsNotNull(dr);
                    }
                }
            }
        }

        [Test, Description("Makes sure that calling Prepare() twice on a command deallocates the first prepared statement")]
        public void DoublePrepare()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT 1";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT 2";
                    cmd.Prepare();
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                    cmd.ExecuteNonQuery();
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1207")]
        public void DoublePrepare2()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare();
                cmd.Prepare();
            }
        }

        [Test, Description("Checks that prepares requires all params to have explicitly set types (NpgsqlDbType or DbType)")]
        public void RequiresParamTypesSet()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var p = new NpgsqlParameter("p", 8);
                cmd.Parameters.Add(p);
                Assert.That(() => cmd.Prepare(), Throws.InvalidOperationException);
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/393")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/299")]
        public void DisposeAfterCommandClose()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select 1";
                cmd.Prepare();
                conn.Close();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void AcrossCloseOpen()
        {
            using (var conn1 = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
            {
                cmd.Prepare();
                Assert.That(cmd.IsPrepared, Is.True);
                conn1.Close();
                conn1.Open();
                Assert.That(cmd.IsPrepared, Is.False);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1)); // Execute unprepared
                cmd.Prepare();
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
            }
        }

        [Test, Description("This scenario used to be supported in 3.0, but isn't supported starting 3.1")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/416")]
        public void DisposeWithOpenReader()
        {
            using (var conn = OpenConnection())
            {
                var cmd1 = new NpgsqlCommand("SELECT 1", conn);
                var cmd2 = new NpgsqlCommand("SELECT 1", conn);
                cmd1.Prepare();
                cmd2.Prepare();
                var reader = cmd2.ExecuteReader();
                reader.Read();
                Assert.That(() => cmd1.Dispose(), Throws.Exception.TypeOf<InvalidOperationException>());
                reader.Close();
                cmd1.Dispose();
                cmd2.Dispose();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
            }
        }

        [Test]
        public void Multistatement([Values(true, false)] bool persist)
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                PersistPrepared = persist
            };

            using (var conn = OpenConnection(connSettings))
            {
                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
                {
                    cmd.Prepare();
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                        reader.NextResult();
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(2));
                    }
                }

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"),
                    Is.EqualTo(persist ? 2 : 0));

                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
                {
                    cmd.Prepare();
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                        reader.NextResult();
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(2));
                    }
                }

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"),
                    Is.EqualTo(persist ? 2 : 0));
            }
        }

        [Test]
        public void ManyStatementsOnCommandClose()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                var sb = new StringBuilder();
                for (var i = 0; i < conn.BufferSize; i++)
                    sb.Append("SELECT 1;");
                cmd.CommandText = sb.ToString();
                cmd.Prepare();
            }
        }

        [Test]
        public void ManyStatementsOnConnectionClose()
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ManyStatementsOnConnectionClose)
            };

            NpgsqlConnection conn = null;
            try
            {
                conn = OpenConnection(connSettings);
                var processId = conn.ProcessID;

                var cmd = new NpgsqlCommand { Connection = conn };
                var sb = new StringBuilder();
                for (var i = 0; i < conn.BufferSize; i++)
                    sb.Append("SELECT 1;");
                cmd.CommandText = sb.ToString();
                cmd.Prepare();
                conn.Close();

                conn.Open();
                Assert.That(conn.ProcessID, Is.EqualTo(processId), "Unexpected connection received from the pool");
                Assert.That(conn.ExecuteScalar("SELECT 8"), Is.EqualTo(8));
            }
            finally
            {
                if (conn != null)
                    NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test, Description("Basic persistent prepared system scenario. Checks that statement is not deallocated in the backend after command dispose.")]
        public void PersistentTwoCommands([Values(true, false)] bool viaConnString)
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                PersistPrepared = viaConnString
            };

            using (var conn = OpenConnection(connSettings))
            {
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));

                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    if (viaConnString)
                        cmd.Prepare();
                    else
                        cmd.Prepare(true);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));

                var stmtName = (string)conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1");

                // Rerun the test using the persistent prepared statement
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    if (viaConnString)
                        cmd.Prepare();
                    else
                        cmd.Prepare(true);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                Assert.That(conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1"), Is.EqualTo(stmtName));
            }
        }

        [Test, Description("Basic persistent prepared system scenario. Checks that statement is not deallocated in the backend after connection close.")]
        public void PersistentAcrossConnections()
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PersistentAcrossConnections)
            };

            NpgsqlConnection conn = null;
            try
            {
                conn = OpenConnection(connSettings);
                var processId = conn.ProcessID;

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);

                var stmtName = (string)conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1");
                conn.Close();

                conn.Open();
                Assert.That(conn.ProcessID, Is.EqualTo(processId), "Unexpected connection received from the pool");

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1), "Persistent prepared statement deallocated");
                Assert.That(conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1"), Is.EqualTo(stmtName), "Persistent prepared statement name changed unexpectedly");

                // Rerun the test using the persistent prepared statement
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1), "Persistent prepared statement deallocated");
                Assert.That(conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1") as string, Is.EqualTo(stmtName), "Persistent prepared statement name changed unexpectedly");
                conn.Dispose();
            }
            finally
            {
                if (conn != null)
                    NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test, Description("Makes sure that calling Prepare() twice on a persistent command does not deallocate or make a new one after the first prepared statement when command does not change")]
        public void PersistentDoublePrepareCommandUnchanged()
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };

            using (var conn = OpenConnection(connSettings))
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    cmd.ExecuteNonQuery();
                    var stmtName = (string)conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1");
                    cmd.Prepare(true);
                    cmd.ExecuteNonQuery();
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1), "Unexpected count of prepared statements");
                    Assert.That(conn.ExecuteScalar("SELECT name FROM pg_prepared_statements LIMIT 1") as string, Is.EqualTo(stmtName), "Persistent prepared statement name changed unexpectedly");
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1), "Persistent prepared statement deallocated");
            }
        }

        [Test]
        public void PersistentDoublePrepareCommandChanged()
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };

            using (var conn = OpenConnection(connSettings))
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT 2";
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                    cmd.Prepare(true);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(2));
                    cmd.ExecuteNonQuery();
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(2));
            }
        }

        [Test]
        public void PersistentMixedWithNonPersistent()
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };

            using (var conn = OpenConnection(connSettings))
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));

                using (var cmd = new NpgsqlCommand("SELECT 2; SELECT 1; SELECT 3", conn))
                {
                    cmd.Prepare(true);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(2));
                        reader.NextResult();
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                        reader.NextResult();
                        reader.Read();
                        Assert.That(reader.GetInt32(0), Is.EqualTo(3));
                    }
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(3));
            }
        }

        [Test]
        public void Unpersist()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);

                // Unpersist via a different command
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    cmd.Unpersist();
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
                }

                // Repersist
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                    cmd.Unpersist();
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
                }

                // Unpersist via an unprepared command
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Unpersist();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));

                // Unpersist via a prepared but unpersisted command
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(false);
                    cmd.Unpersist();
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
            }
        }

        [Test]
        public void PrepareWithoutPersistAfterPersist()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    cmd.Prepare(false);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(2));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
            }
        }
    }
}
