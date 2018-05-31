using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class AutoPrepareTests : TestBase
    {
        [Test]
        public void Basic()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            };

            using (var conn = OpenConnection(csb))
            using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
            {
                checkCmd.Prepare();

                conn.ExecuteNonQuery("SELECT 1");
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(0));

                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.ExecuteScalar();
                    Assert.That(cmd.IsPrepared, Is.True);
                    Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                    cmd.ExecuteScalar();
                    Assert.That(cmd.IsPrepared, Is.True);
                    Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                }

                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                {
                    cmd.ExecuteScalar();
                    Assert.That(cmd.IsPrepared, Is.True);
                }
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                conn.UnprepareAll();
            }
        }

        [Test, Description("Passes the maximum limit for autoprepared statements, recycling the least-recently used one")]
        public void Recycle()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                AutoPrepareMinUsages = 2,
                MaxAutoPrepare = 2
            };

            using (var conn = OpenConnection(csb))
            using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
            {
                checkCmd.Prepare();

                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(0));
                var cmd1 = new NpgsqlCommand("SELECT 1", conn);
                cmd1.ExecuteNonQuery(); cmd1.ExecuteNonQuery();
                Assert.That(cmd1.IsPrepared, Is.True);
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                Thread.Sleep(10);

                var cmd2 = new NpgsqlCommand("SELECT 2", conn);
                cmd2.ExecuteNonQuery(); cmd2.ExecuteNonQuery();
                Assert.That(cmd2.IsPrepared, Is.True);
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(2));

                // Use cmd1 to make cmd2 the lru
                Thread.Sleep(1);
                cmd1.ExecuteNonQuery();

                // Cause another statement to be autoprepared. This should eject cmd2.
                conn.ExecuteNonQuery("SELECT 3"); conn.ExecuteNonQuery("SELECT 3");
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(2));

                cmd2.ExecuteNonQuery();
                Assert.That(cmd2.IsPrepared, Is.False);
                using (var getTextCmd = new NpgsqlCommand("SELECT statement FROM pg_prepared_statements WHERE statement NOT LIKE '%COUNT%' ORDER BY statement", conn))
                using (var reader = getTextCmd.ExecuteReader())
                {
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader.GetString(0), Is.EqualTo("SELECT 1"));
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader.GetString(0), Is.EqualTo("SELECT 3"));
                }
                conn.UnprepareAll();
            }
        }

        [Test]
        public void Persist()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(Persist),
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            }.ToString();
            try
            {
                using (var conn = OpenConnection(connString))
                using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
                {
                    checkCmd.Prepare();
                    conn.ExecuteNonQuery("SELECT 1"); conn.ExecuteNonQuery("SELECT 1");
                    Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                }

                // We now have two prepared statements which should be persisted

                using (var conn = OpenConnection(connString))
                using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
                {
                    checkCmd.Prepare();
                    Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                    using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    {
                        cmd.ExecuteScalar();
                        //Assert.That(cmd.IsPrepared);
                    }
                    Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                }
            }
            finally
            {
                using (var conn = new NpgsqlConnection(connString))
                    NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test]
        public void PromoteAutoToExplicit()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            };
            using (var conn = OpenConnection(csb))
            using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
            using (var cmd1 = new NpgsqlCommand("SELECT 1", conn))
            using (var cmd2 = new NpgsqlCommand("SELECT 1", conn))
            {
                checkCmd.Prepare();

                cmd1.ExecuteNonQuery(); cmd1.ExecuteNonQuery();
                // cmd1 is now autoprepared
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                Assert.That(conn.Connector.PreparedStatementManager.NumPrepared, Is.EqualTo(2));

                // Promote (replace) the autoprepared statement with an explicit one.
                cmd2.Prepare();
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                Assert.That(conn.Connector.PreparedStatementManager.NumPrepared, Is.EqualTo(2));

                // cmd1's statement is no longer valid (has been closed), make sure it still works (will run unprepared)
                cmd2.ExecuteScalar();
                conn.UnprepareAll();
            }
        }

        [Test]
        public void CandidateEject()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 3
            };
            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;

                for (var i = 0; i < PreparedStatementManager.CandidateCount; i++)
                {
                    cmd.CommandText = $"SELECT {i}";
                    cmd.ExecuteNonQuery();
                    Thread.Sleep(1);
                }

                // The candidate list is now full with single-use statements.

                cmd.CommandText = $"SELECT 'double_use'";
                cmd.ExecuteNonQuery(); cmd.ExecuteNonQuery();
                // We now have a single statement that has been used twice.

                for (var i = PreparedStatementManager.CandidateCount; i < PreparedStatementManager.CandidateCount * 2; i++)
                {
                    cmd.CommandText = $"SELECT {i}";
                    cmd.ExecuteNonQuery();
                    Thread.Sleep(1);
                }

                // The new single-use statements should have ejected all previous single-use statements
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery(); cmd.ExecuteNonQuery();
                Assert.That(cmd.IsPrepared, Is.False);

                // But the double-use statement should still be there
                cmd.CommandText = "SELECT 'double_use'";
                cmd.ExecuteNonQuery();
                Assert.That(cmd.IsPrepared, Is.True);

                conn.UnprepareAll();
            }
        }

        [Test]
        public void OneCommandSameSqlTwice()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            };
            using (var conn = OpenConnection(csb))
            using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
            using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 1; SELECT 1; SELECT 1", conn))
            {
                cmd.Prepare();
                Assert.That(cmd.IsPrepared, Is.True);
                cmd.ExecuteNonQuery();
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                conn.UnprepareAll();
            }
        }

        [Test]
        public void AcrossCloseOpenDifferentConnector()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(AutoPrepareTests) + '.' + nameof(AcrossCloseOpenDifferentConnector),
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            }.ToString();
            using (var conn1 = new NpgsqlConnection(connString))
            using (var conn2 = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
            {
                conn1.Open();
                cmd.ExecuteNonQuery(); cmd.ExecuteNonQuery();
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
        public void UnprepareAll()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            };

            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare();  // Explicit
                conn.ExecuteNonQuery("SELECT 2"); conn.ExecuteNonQuery("SELECT 2");  // Auto
                Assert.That(conn.ExecuteScalar(CountPreparedStatements), Is.EqualTo(2));
                conn.UnprepareAll();
                Assert.That(conn.ExecuteScalar(CountPreparedStatements), Is.Zero);
            }
        }

        [Test, Description("Prepares the same SQL with different parameters (overloading)")]
        public void OverloadedSql()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            };
            using (var conn = OpenConnection(csb))
            {
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Integer, 8);
                    cmd.ExecuteNonQuery(); cmd.ExecuteNonQuery();
                    Assert.That(cmd.IsPrepared, Is.True);
                }
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, "foo");
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo("foo"));
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo("foo"));
                    Assert.That(cmd.IsPrepared, Is.False);
                }

                // SQL overloading is a pretty rare/exotic scenario. Handling it properly would involve keying
                // prepared statements not just by SQL but also by the parameter types, which would pointlessly
                // increase allocations. Instead, the second execution simply reuns unprepared
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                conn.UnprepareAll();
            }
        }

        [Test, Description("Tests parameter derivation a parameterized query (CommandType.Text) that is already auto-prepared.")]
        public void DeriveParametersForAutoPreparedStatement()
        {
            const string query = "SELECT @p::integer";
            const int answer = 42;
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2
            };
            using (var conn = OpenConnection(csb))
            using (var checkCmd = new NpgsqlCommand(CountPreparedStatements, conn))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                checkCmd.Prepare();
                cmd.Parameters.AddWithValue("@p", NpgsqlDbType.Integer, answer);
                cmd.ExecuteNonQuery(); cmd.ExecuteNonQuery(); // cmd1 is now autoprepared
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(1));
                Assert.That(conn.Connector.PreparedStatementManager.NumPrepared, Is.EqualTo(2));

                // Derive parameters for the already autoprepared statement
                NpgsqlCommandBuilder.DeriveParameters(cmd);
                Assert.That(cmd.Parameters.Count, Is.EqualTo(1));
                Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("p"));

                // DeriveParameters should have silently unprepared the autoprepared statements
                Assert.That(checkCmd.ExecuteScalar(), Is.EqualTo(0));
                Assert.That(conn.Connector.PreparedStatementManager.NumPrepared, Is.EqualTo(1));

                cmd.Parameters["@p"].Value = answer;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(answer));

                conn.UnprepareAll();
            }
        }

        // Exclude some internal Npgsql queries which include pg_type as well as the count statement itself
        const string CountPreparedStatements = @"
SELECT COUNT(*) FROM pg_prepared_statements
    WHERE statement NOT LIKE '%pg_prepared_statements%'
    AND statement NOT LIKE '%pg_type%'";

        void DumpPreparedStatements(NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand("SELECT name,statement FROM pg_prepared_statements", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    Console.WriteLine($"{reader.GetString(0)}: {reader.GetString(1)}");
            }
        }
    }
}
