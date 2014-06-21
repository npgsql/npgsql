// NotifyTest

// Authors:
// Udo Liess <udo.liess@gmx.net>

// Send multiple NOTIFY events and wait for all of them.
// See bug [#1011374] http://pgfoundry.org/tracker/index.php?func=detail&aid=1011374&group_id=1000140&atid=590

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

// 2013-11-26 initial version
// 2013-12-05 add constructor with parameter backendVersion

using Npgsql;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace NpgsqlTests
{
    [TestFixture]
    class NotifyTest : TestBase
    {
        public NotifyTest(string backendVersion) : base(backendVersion) { }

        readonly TimeSpan timeout = TimeSpan.FromSeconds(5); // timeout in case of failure
        const int rounds = 6; // number of rounds to test

        [Test]
        public void SendMultipleNotifiesTest()
        {
            var sync = new object(); // synchronization object
            var lastTx = 0; // last sent payload
            var lastRx = 0; // last received payload
            var misorder = false; // flag for payload misorder

            using (var wakeup = new ManualResetEvent(false)) // event to wake waiting thread
            {
                // open a listen connection
                var csbListen = new NpgsqlConnectionStringBuilder(ConnectionString);
                csbListen.SyncNotification = true;
                using (var listen = new NpgsqlConnection(csbListen.ConnectionString))
                {
                    listen.Notification += delegate(object sender, NpgsqlNotificationEventArgs e)
                    {
                        var n = int.Parse(e.AdditionalInformation, CultureInfo.InvariantCulture);
                        lock (sync) // misorder an lastRx are accessed from different threads --> synchronisation needed
                        {
                            if (n != lastRx + 1) // check the payload order
                                misorder = true; // we can not throw assert here because we are in different thread. so remember the failure state.
                            lastRx = n; // remember last received payload
                        }
                        wakeup.Set(); // wake waiting thread
                    };
                    listen.Open();
                    ExecuteNonQuery("LISTEN a", listen); // ...listerner is up and running

                    // open another notifying connection
                    var csbNotify = new NpgsqlConnectionStringBuilder(ConnectionString);
                    csbNotify.SyncNotification = false;
                    using (var notify = new NpgsqlConnection(csbNotify.ConnectionString))
                    {
                        notify.Open();
                        // create and prepare notify command
                        using (var cmd = notify.CreateCommand())
                        {
                            cmd.CommandText = "SELECT pg_notify('a', CAST(@0 AS text));";
                            cmd.Parameters.Add("0", NpgsqlTypes.NpgsqlDbType.Integer);
                            cmd.Prepare();
                            // run the test some rounds
                            for (var round = 0; round < rounds; round++)
                            {
                                // send all notifies in one transaction
                                using (var ta = notify.BeginTransaction())
                                {
                                    cmd.Transaction = ta;
                                    // in each round send an increasing number of notifies
                                    var notifies = 1 << round; // 1, 2, 4, 8, 16, ...
                                    for (var i = 0; i < notifies; i++)
                                    {
                                        cmd.Parameters[0].Value = ++lastTx;
                                        cmd.ExecuteNonQuery();
                                    }
                                    ta.Commit();
                                }
                                // after sending wait for all of them to be received
                                var start = Stopwatch.StartNew(); // start time measurement for timeout
                                for (; ; )
                                {
                                    TimeSpan remain;
                                    lock (sync)
                                    {
                                        Assert.IsFalse(misorder, "misorder");
                                        if (lastRx == lastTx) // if we received all notifies this round is done
                                            break;
                                        remain = timeout - start.Elapsed; // remaining time until timeout failure
                                        if (remain <= TimeSpan.Zero)
                                        {
                                            Assert.Fail(string.Format(CultureInfo.InvariantCulture, "timeout. round: {0}, last tx: {1}, last rx: {2}", round, lastTx, lastRx));
                                            break;
                                        }
                                    }
                                    WaitHandle.WaitAny(new[] { wakeup }, remain); // wait for wakeup event but only for limited time
                                    wakeup.Reset(); // reset event so it can be reused
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
