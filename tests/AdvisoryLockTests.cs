using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using Npgsql.Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class AdvisoryLockTests : TestBase
    {
        public AdvisoryLockTests(string backendVersion) : base(backendVersion) {}

        [Test, Description("Basic scenario for an exclusive advisory lock")]
        public void Exclusive()
        {
            long count;
            var lock1 = new NpgsqlExclusiveAdvisoryLock(Conn, 8);
            using (lock1.Acquire())
            {
                count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND objid=8 AND mode='ExclusiveLock'");
                Assert.That(count, Is.EqualTo(1));

                using (var conn2 = new NpgsqlConnection(Conn.ConnectionString))
                {
                    conn2.Open();
                    var lock2 = new NpgsqlExclusiveAdvisoryLock(conn2, 8);
                    IDisposable handle;
                    var result = lock2.TryAcquire(out handle);
                    Assert.That(result, Is.False);
                    Assert.That(handle, Is.Null);
                }
            }
            count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND classid IS NULL AND objid=8 AND mode='ExclusiveLock'");
            Assert.That(count, Is.EqualTo(0));
        }

        [Test, Description("Basic scenario for an exclusive advisory lock, using two int keys instead of one long")]
        public void ExclusiveWithIntKeys()
        {
            long count;
            var lock1 = new NpgsqlExclusiveAdvisoryLock(Conn, 10, 11);
            using (lock1.Acquire())
            {
                count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND classid=10 AND objid=11 AND mode='ExclusiveLock'");
                Assert.That(count, Is.EqualTo(1));
            }
            count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND classid=10 AND objid=11 AND mode='ExclusiveLock'");
            Assert.That(count, Is.EqualTo(0));
        }

        [Test, Description("Basic scenario for a shared advisory lock")]
        public void Shared()
        {
            long count;
            var lock1 = new NpgsqlSharedAdvisoryLock(Conn, 8);
            using (lock1.Acquire())
            {
                count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND objid=8 AND mode='ShareLock'");
                Assert.That(count, Is.EqualTo(1));

                using (var conn2 = new NpgsqlConnection(Conn.ConnectionString))
                {
                    conn2.Open();
                    var lock2 = new NpgsqlSharedAdvisoryLock(conn2, 8);
                    IDisposable handle;
                    var result = lock2.TryAcquire(out handle);
                    Assert.That(result, Is.True);
                    Assert.That(handle, Is.Not.Null);
                    handle.Dispose();
                }
            }
            count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND classid IS NULL AND objid=8 AND mode='ShareLock'");
            Assert.That(count, Is.EqualTo(0));
        }

        [Test, Description("Basic scenario for an exclusive transaction advisory lock")]
        public void ExclusiveTransaction()
        {
            if (Conn.PostgreSqlVersion < new Version(9, 1, 0))
                Assert.Ignore("Transaction advisory locks aren't supported prior to Postgresql 9.1");

            using (var conn2 = new NpgsqlConnection(Conn.ConnectionString))
            {
                long count;
                var tx = Conn.BeginTransaction();

                var lock1 = new NpgsqlExclusiveTransactionAdvisoryLock(Conn, 8);
                lock1.Acquire();
                count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND objid=8 AND mode='ExclusiveLock'");
                Assert.That(count, Is.EqualTo(1));

                conn2.Open();
                var lock2 = new NpgsqlExclusiveAdvisoryLock(conn2, 8);
                IDisposable handle;
                var result = lock2.TryAcquire(out handle);
                Assert.That(result, Is.False);
                Assert.That(handle, Is.Null);

                tx.Commit();

                count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND objid=8 AND mode='ExclusiveLock'");
                Assert.That(count, Is.EqualTo(0));

                result = lock2.TryAcquire(out handle);
                Assert.That(result, Is.True);
                Assert.That(handle, Is.Not.Null);
                handle.Dispose();
            }
        }

        [Test, Description("Tests the connection's ReleaseAllAdvisoryLocks functionality")]
        public void ReleaseAll()
        {
            long count;
            var lock1 = new NpgsqlExclusiveAdvisoryLock(Conn, 8);
            using (lock1.Acquire())
            {
                Conn.ReleaseAllAdvisoryLocks();
                count = (long)ExecuteScalar(@"SELECT COUNT(*) FROM pg_locks WHERE locktype='advisory' AND classid=10 AND objid=11 AND mode='ExclusiveLock'");
                Assert.That(count, Is.EqualTo(0));
            }
        }

        [TearDown]
        public void TearDown()
        {
            ExecuteNonQuery("SELECT pg_advisory_unlock_all()");
        }
    }
}
