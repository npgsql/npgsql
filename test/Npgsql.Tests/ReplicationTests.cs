using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Replication;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class ReplicationTests : TestBase
    {
        [Test, NonParallelizable]
        public async Task EndToEnd()
        {
            using var conn = OpenConnection();
            using var replConn = new NpgsqlLogicalReplicationConnection(ConnectionString);
            var slotName = nameof(EndToEnd);

            conn.ExecuteNonQuery("DROP TABLE IF EXISTS end_to_end_replication");
            conn.ExecuteNonQuery("CREATE TABLE end_to_end_replication (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT)");

            await replConn.OpenAsync();
            await replConn.CreateReplicationSlot(slotName, "test_decoding");

            var confirmedFlushLsn = conn.ExecuteScalar($"SELECT confirmed_flush_lsn FROM pg_replication_slots WHERE slot_name = '{slotName}'");
            Assert.That(confirmedFlushLsn, Is.Null);
            //Assert.That((await replConn.IdentifySystem()).XLogPos, Is.EqualTo(confirmedFlushLsn));

            // Make some changes
            conn.ExecuteNonQuery("INSERT INTO end_to_end_replication (name) VALUES ('val1')");
            conn.ExecuteNonQuery("UPDATE end_to_end_replication SET name='val2' WHERE name='val1'");

            await replConn.StartReplication(slotName, "0/0");

            Assert.That(UTF8Encoding.UTF8.GetString((await replConn.GetNextMessage())!.Value.Data.ToArray()),
                Does.StartWith("BEGIN "));
            Assert.That(UTF8Encoding.UTF8.GetString((await replConn.GetNextMessage())!.Value.Data.ToArray()),
                Is.EqualTo("table public.end_to_end_replication: INSERT: id[integer]:1 name[text]:'val1'"));
            var msg = (await replConn.GetNextMessage())!.Value;
            Assert.That(UTF8Encoding.UTF8.GetString(msg.Data.ToArray()), Does.StartWith("COMMIT "));

            // Pretend we've completely processed this transaction, inform the server manually
            // (in real life we can wait until the automatic periodic update does this)
            //await replConn.SendStatusUpdate(msg.WalEnd, msg.WalEnd, msg.WalEnd);
            //confirmedFlushLsn = conn.ExecuteScalar($"SELECT confirmed_flush_lsn FROM pg_replication_slots WHERE slot_name = '{slotName}'");
            //Assert.That(confirmedFlushLsn, Is.Not.Null);  // There's obviously a misunderstanding here

            Assert.That(UTF8Encoding.UTF8.GetString((await replConn.GetNextMessage())!.Value.Data.ToArray()),
                Does.StartWith("BEGIN "));
            Assert.That(UTF8Encoding.UTF8.GetString((await replConn.GetNextMessage())!.Value.Data.ToArray()),
                Is.EqualTo("table public.end_to_end_replication: UPDATE: id[integer]:1 name[text]:'val2'"));
            Assert.That(UTF8Encoding.UTF8.GetString((await replConn.GetNextMessage())!.Value.Data.ToArray()),
                Does.StartWith("COMMIT "));

            replConn.Cancel();

            // TODO: Bad example: pretend we don't know what's coming
            // Drain any messages
            while (await replConn.GetNextMessage() != null) ;

            // Make sure the connection is back to idle state
            Assert.That(await replConn.Show("integer_datetimes"), Is.EqualTo("on"));

            await replConn.DropReplicationSlot(slotName);
        }

        [Test]
        public async Task IdentifySystem()
        {
            using var conn = new NpgsqlLogicalReplicationConnection(ConnectionString);
            await conn.OpenAsync();
            var identificationInfo = await conn.IdentifySystem();
            Assert.That(identificationInfo.DbName, Is.EqualTo(new NpgsqlConnectionStringBuilder(ConnectionString).Database));
        }

        [Test]
        public async Task Show()
        {
            using var conn = new NpgsqlLogicalReplicationConnection(ConnectionString);
            await conn.OpenAsync();
            Assert.That(await conn.Show("integer_datetimes"), Is.EqualTo("on"));
        }

        [Test]
        public async Task CreateDropLogicalSlot()
        {
            using var conn = new NpgsqlLogicalReplicationConnection(ConnectionString);
            await conn.OpenAsync();
            await conn.CreateReplicationSlot(nameof(CreateDropLogicalSlot), "test_decoding");
            await conn.DropReplicationSlot(nameof(CreateDropLogicalSlot));
        }

        #region Support

        [SetUp]
        public void Setup()
        {
            using var conn = OpenConnection();
            var walLevel = (string)conn.ExecuteScalar("SHOW wal_level");
            if (walLevel != "logical")
                TestUtil.IgnoreExceptOnBuildServer("wal_level needs to be set to 'logical' in the PostgreSQL conf");
            DropAllReplicationSlots();
        }

        void DropAllReplicationSlots()
        {
            using var conn = OpenConnection();

            var slots = new List<string>();
            using (var cmd = new NpgsqlCommand("SELECT slot_name FROM pg_replication_slots", conn))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    slots.Add(reader.GetString(0));

            foreach (var slot in slots)
                conn.ExecuteNonQuery($"SELECT pg_drop_replication_slot('{slot}')");

        }

        #endregion Support
    }
}
