using System;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.TimeZones;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.NodaTime.Tests
{
    // Since this test suite manipulates TimeZone, it is incompatible with multiplexing
    [NonParallelizable]
    public class LegacyNodaTimeTests : TestBase
    {
        [Test]
        public Task Timestamp_as_Instant()
            => AssertType(
                new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
                "1998-04-12 13:26:38.789",
                "timestamp without time zone",
                NpgsqlDbType.Timestamp,
                DbType.DateTime);

        [Test]
        public Task Timestamp_as_LocalDateTime()
            => AssertType(
                new LocalDateTime(1998, 4, 12, 13, 26, 38, 789),
                "1998-04-12 13:26:38.789",
                "timestamp without time zone",
                NpgsqlDbType.Timestamp,
                DbType.DateTime,
                isDefaultForReading: false);

        [Test]
        public Task Timestamptz_as_Instant()
            => AssertType(
                new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
                "1998-04-12 15:26:38.789+02",
                "timestamp with time zone",
                NpgsqlDbType.TimestampTz,
                DbType.DateTimeOffset,
                isDefault: false);

        #region Support

        protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
        {
            var conn = new NpgsqlConnection(connectionString ?? ConnectionString);
            await conn.OpenAsync();
            await conn.ExecuteNonQueryAsync("SET TimeZone='Europe/Berlin'");
            return conn;
        }

        protected override NpgsqlConnection OpenConnection(string? connectionString = null)
            => throw new NotSupportedException();

        [OneTimeSetUp]
        public void Setup()
        {
#if DEBUG
            Internal.NodaTimeUtils.LegacyTimestampBehavior = true;
            Util.Statics.LegacyTimestampBehavior = true;
#else
            Assert.Ignore(
                "Legacy NodaTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif

        }

        [OneTimeTearDown]
        public void Teardown()
        {
#if DEBUG
            Internal.NodaTimeUtils.LegacyTimestampBehavior = false;
            Util.Statics.LegacyTimestampBehavior = false;
#endif
        }

        #endregion Support
    }
}
