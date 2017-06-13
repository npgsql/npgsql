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
        [Test]
        public void Basic()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    AssertNumPreparedStatements(conn, 0);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                    Assert.That(cmd.IsPrepared, Is.False);

                    cmd.Prepare();
                    AssertNumPreparedStatements(conn, 1);
                    Assert.That(cmd.IsPrepared, Is.True);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                AssertNumPreparedStatements(conn, 1);
                conn.UnprepareAll();
            }
        }

        [Test]
        public void Unprepare()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                AssertNumPreparedStatements(conn, 0);
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    AssertNumPreparedStatements(conn, 1);
                    cmd.Unprepare();
                    AssertNumPreparedStatements(conn, 0);
                    Assert.That(cmd.IsPrepared, Is.False);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
            }
        }

        [Test]
        public void Parameters()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var command = new NpgsqlCommand("SELECT @a, @b", conn))
            {
                command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
                command.Parameters.Add(new NpgsqlParameter("b", DbType.Int64));
                command.Prepare();
                command.Parameters[0].Value = 3;
                command.Parameters[1].Value = 5;
                using (var reader = command.ExecuteReader())
                {
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader.GetInt32(0), Is.EqualTo(3));
                    Assert.That(reader.GetInt64(1), Is.EqualTo(5));
                }
                command.Unprepare();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1207")]
        public void DoublePrepareSameSql()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare();
                cmd.Prepare();
                AssertNumPreparedStatements(conn, 1);
                cmd.Unprepare();
                AssertNumPreparedStatements(conn, 0);
            }
        }

        [Test]
        public void DoublePrepareDifferentSql()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = "SELECT 1";
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT 2";
                cmd.Prepare();
                AssertNumPreparedStatements(conn, 2);
                cmd.ExecuteNonQuery();

                conn.UnprepareAll();
            }
        }

        [Test, Description("Checks that prepares requires all params to have explicitly set types (NpgsqlDbType or DbType)")]
        public void RequiresParamTypesSet()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var p = new NpgsqlParameter("p", 8);
                cmd.Parameters.Add(p);
                Assert.That(() => cmd.Prepare(), Throws.InvalidOperationException);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void AcrossCloseOpenSameConnector()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PrepareTests) + '.' + nameof(AcrossCloseOpenSameConnector)
            };
            using (var conn = OpenConnectionAndUnprepare(csb))
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare();
                Assert.That(cmd.IsPrepared, Is.True);
                var processId = conn.ProcessID;
                conn.Close();
                conn.Open();
                Assert.That(processId, Is.EqualTo(conn.ProcessID));
                Assert.That(cmd.IsPrepared, Is.True);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                cmd.Prepare();
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test]
        public void AcrossCloseOpenDifferentConnector()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PrepareTests) + '.' + nameof(AcrossCloseOpenDifferentConnector)
            }.ToString();
            using (var conn1 = new NpgsqlConnection(connString))
            using (var conn2 = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
            {
                conn1.Open();
                cmd.Prepare();
                Assert.That(cmd.IsPrepared, Is.True);
                var processId = conn1.ProcessID;
                conn1.Close();
                conn2.Open();
                conn1.Open();
                Assert.That(conn1.ProcessID, Is.Not.EqualTo(processId));
                Assert.That(cmd.IsPrepared, Is.False);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));  // Execute unprepared
                cmd.Prepare();
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                NpgsqlConnection.ClearPool(conn1);
            }
        }

        [Test]
        public void Multistatement()
        {
            using (var conn = OpenConnectionAndUnprepare())
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

                AssertNumPreparedStatements(conn, 2);

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

                AssertNumPreparedStatements(conn, 2);
                conn.UnprepareAll();
            }
        }

        [Test]
        public void OneCommandSameSqlTwice()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 1", conn))
            {
                cmd.Prepare();
                AssertNumPreparedStatements(conn, 1);
                cmd.ExecuteNonQuery();
                cmd.Unprepare();
            }
        }

        [Test]
        public void OneCommandSameSqlAutoPrepare()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 5,
                AutoPrepareMinUsages = 2
            };
            using (var conn = OpenConnectionAndUnprepare(csb))
            {
                var sql = new StringBuilder();
                for (var i = 0; i < 2 + 1; i++)
                    sql.Append("SELECT 1;");
                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                    cmd.ExecuteNonQuery();
                AssertNumPreparedStatements(conn, 1);
            }
        }

        [Test]
        public void UnprepareViaDifferentCommand()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd1 = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
            using (var cmd2 = new NpgsqlCommand("SELECT 2; SELECT 3", conn))
            {
                cmd1.Prepare();
                cmd2.Prepare();
                // Both commands reference the same prepared statement
                AssertNumPreparedStatements(conn, 3);
                cmd2.Unprepare();
                AssertNumPreparedStatements(conn, 1);
                Assert.That(cmd1.IsPrepared, Is.False);  // Only partially prepared, so no
                cmd1.ExecuteNonQuery();
                cmd1.Unprepare();
                AssertNumPreparedStatements(conn, 0);
                Assert.That(cmd1.IsPrepared, Is.False);
                cmd1.ExecuteNonQuery();

                conn.UnprepareAll();
            }
        }

        [Test, Description("Prepares the same SQL with different parameters (overloading)")]
        public void OverloadedSql()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add("p", NpgsqlDbType.Integer);
                    cmd.Prepare();
                    Assert.That(cmd.IsPrepared, Is.True);
                }
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, "foo");
                    cmd.Prepare();
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo("foo"));
                    Assert.That(cmd.IsPrepared, Is.False);
                }

                // SQL overloading is a pretty rare/exotic scenario. Handling it properly would involve keying
                // prepared statements not just by SQL but also by the parameter types, which would pointlessly
                // increase allocations. Instead, the second execution simply reuns unprepared
                AssertNumPreparedStatements(conn, 1);
                conn.UnprepareAll();
            }
        }

        [Test]
        public void ManyStatementsOnUnprepare()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                var sb = new StringBuilder();
                for (var i = 0; i < conn.Settings.WriteBufferSize; i++)
                    sb.Append("SELECT 1;");
                cmd.CommandText = sb.ToString();
                cmd.Prepare();
                cmd.Unprepare();
            }
        }

        [Test]
        public void IsPreparedIsFalseAfterChangingCommandText()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare();
                AssertNumPreparedStatements(conn, 1);
                cmd.CommandText = "SELECT 2";
                Assert.That(cmd.IsPrepared, Is.False);
                cmd.ExecuteNonQuery();
                Assert.That(cmd.IsPrepared, Is.False);
                AssertNumPreparedStatements(conn, 1);
                cmd.Unprepare();
            }
        }

        [Test, Description("Basic persistent prepared system scenario. Checks that statement is not deallocated in the backend after command dispose.")]
        public void PersistentAcrossCommands()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                AssertNumPreparedStatements(conn, 0);

                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    AssertNumPreparedStatements(conn, 1);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                AssertNumPreparedStatements(conn, 1);

                var stmtName = GetPreparedStatements(conn).Single();

                // Rerun the test using the persistent prepared statement
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    AssertNumPreparedStatements(conn, 1);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                AssertNumPreparedStatements(conn, 1);
                Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName));
                conn.UnprepareAll();
            }
        }

        [Test, Description("Basic persistent prepared system scenario. Checks that statement is not deallocated in the backend after connection close.")]
        public void PersistentAcrossConnections()
        {
            var connSettings = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PersistentAcrossConnections)
            };

            using (var conn = OpenConnectionAndUnprepare(connSettings))
            {
                var processId = conn.ProcessID;

                AssertNumPreparedStatements(conn, 0);
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare();

                var stmtName = GetPreparedStatements(conn).Single();
                conn.Close();

                conn.Open();
                Assert.That(conn.ProcessID, Is.EqualTo(processId), "Unexpected connection received from the pool");

                AssertNumPreparedStatements(conn, 1, "Prepared statement deallocated");
                Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName), "Prepared statement name changed unexpectedly");

                // Rerun the test using the persistent prepared statement
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
                AssertNumPreparedStatements(conn, 1, "Prepared statement deallocated");
                Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName), "Prepared statement name changed unexpectedly");

                NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test, Description("Makes sure that calling Prepare() twice on a command does not deallocate or make a new one after the first prepared statement when command does not change")]
        public void PersistentDoublePrepareCommandUnchanged()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    var stmtName = GetPreparedStatements(conn).Single();
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    AssertNumPreparedStatements(conn, 1, "Unexpected count of prepared statements");
                    Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName), "Persistent prepared statement name changed unexpectedly");
                }
                AssertNumPreparedStatements(conn, 1, "Persistent prepared statement deallocated");
                conn.UnprepareAll();
            }
        }

        [Test]
        public void PersistentDoublePrepareCommandChanged()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT 2";
                    AssertNumPreparedStatements(conn, 1);
                    cmd.Prepare();
                    AssertNumPreparedStatements(conn, 2);
                    cmd.ExecuteNonQuery();
                }
                AssertNumPreparedStatements(conn, 2);
                conn.UnprepareAll();
            }
        }

        /*
        [Test]
        public void Unpersist()
        {
            using (var conn = OpenConnectionAndUnprepare())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);

                // Unpersist via a different command
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    cmd.Unpersist();
                    AssertNumPreparedStatements(conn, 0);
                }

                // Repersist
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(true);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                    cmd.Unpersist();
                    AssertNumPreparedStatements(conn, 0);
                }

                // Unpersist via an unprepared command
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Unpersist();
                AssertNumPreparedStatements(conn, 0);

                // Unpersist via a prepared but unpersisted command
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.Prepare(true);
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.Prepare(false);
                    cmd.Unpersist();
                }
                AssertNumPreparedStatements(conn, 0);
            }
        }

        [Test]
        public void SameSqlDifferentParams()
        {
            using (var conn = OpenConnectionAndUnprepare())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                throw new NotImplementedException("Problem: currentl setting NpgsqlParameter.Value clears/invalidates...");
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Integer));
                cmd.Prepare(true);

                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Text;
                Assert.That(cmd.IsPrepared, Is.False);
                cmd.Prepare(true);
                using (var crapCmd = new NpgsqlCommand("SELECT name,statement,parameter_types::TEXT[] FROM pg_prepared_statements", conn))
                using (var reader = crapCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Statement: {reader.GetString(0)}, {reader.GetString(1)}");
                        foreach (var p in reader.GetFieldValue<string[]>(2))
                        {
                            Console.WriteLine("  Param: " + p);
                        }
                    }
                }
                //AssertNumPreparedStatements(conn, 2);
                cmd.Parameters[0].Value = "hello";
                Console.WriteLine(cmd.ExecuteScalar());
            }
        }
        */

        NpgsqlConnection OpenConnectionAndUnprepare(string connectionString = null)
        {
            var conn = OpenConnection(connectionString);
            conn.UnprepareAll();
            return conn;
        }

        NpgsqlConnection OpenConnectionAndUnprepare(NpgsqlConnectionStringBuilder csb)
            => OpenConnectionAndUnprepare(csb.ToString());

        void AssertNumPreparedStatements(NpgsqlConnection conn, int expected)
            => Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements WHERE statement NOT LIKE '%FROM pg_prepared_statements%'"), Is.EqualTo(expected));

        void AssertNumPreparedStatements(NpgsqlConnection conn, int expected, string message)
            => Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements WHERE statement NOT LIKE '%FROM pg_prepared_statements%'"), Is.EqualTo(expected), message);

        List<string> GetPreparedStatements(NpgsqlConnection conn)
        {
            var statements = new List<string>();
            using (var cmd = new NpgsqlCommand("SELECT name FROM pg_prepared_statements WHERE statement NOT LIKE '%FROM pg_prepared_statement%'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    statements.Add(reader.GetString(0));
            }
            return statements;
        }
    }
}
