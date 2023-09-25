using System;

namespace Npgsql.Replication;

static class PgDateTime
{
    const long PostgresTimestampOffsetTicks = 630822816000000000L;

    public static DateTime DecodeTimestamp(long value, DateTimeKind kind)
        => new(value * 10 + PostgresTimestampOffsetTicks, kind);

    public static long EncodeTimestamp(DateTime value)
        // Rounding here would cause problems because we would round up DateTime.MaxValue
        // which would make it impossible to retrieve it back from the database, so we just drop the additional precision
        => (value.Ticks - PostgresTimestampOffsetTicks) / 10;
}
