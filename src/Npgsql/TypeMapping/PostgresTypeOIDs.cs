#pragma warning disable RS0016
#pragma warning disable 1591

namespace Npgsql.TypeMapping
{
    /// <summary>
    /// Holds well-known, built-in PostgreSQL type OIDs.
    /// </summary>
    /// <remarks>
    /// Source: <see href="https://github.com/postgres/postgres/blob/master/src/include/catalog/pg_type.dat" />
    /// </remarks>
    public static class PostgresTypeOIDs
    {
        // Numeric
        public const uint Int8         = 20;
        public const uint Float8       = 701;
        public const uint Int4         = 23;
        public const uint Numeric      = 1700;
        public const uint Float4       = 700;
        public const uint Int2         = 21;
        public const uint Money        = 790;

        // Boolean
        public const uint Bool        = 16;

        // Geometric
        public const uint Box         = 603;
        public const uint Circle      = 718;
        public const uint Line        = 628;
        public const uint LSeg        = 601;
        public const uint Path        = 602;
        public const uint Point       = 600;
        public const uint Polygon     = 604;

        // Character
        public const uint BPChar      = 1042;
        public const uint Text        = 25;
        public const uint Varchar     = 1043;
        public const uint Name        = 19;
        public const uint Char        = 18;

        // Binary data
        public const uint Bytea       = 17;

        // Date/Time
        public const uint Date        = 1082;
        public const uint Time        = 1083;
        public const uint Timestamp   = 1114;
        public const uint TimestampTz = 1184;
        public const uint Interval    = 1186;
        public const uint TimeTz      = 1266;
        public const uint Abstime     = 702;

        // Network address
        public const uint Inet        = 869;
        public const uint Cidr        = 650;
        public const uint Macaddr     = 829;
        public const uint Macaddr8    = 774;

        // Bit string
        public const uint Bit         = 1560;
        public const uint Varbit      = 1562;

        // Text search
        public const uint TsVector    = 3614;
        public const uint TsQuery     = 3615;
        public const uint Regconfig   = 3734;

        // UUID
        public const uint Uuid        = 2950;

        // XML
        public const uint Xml         = 142;

        // JSON
        public const uint Json        = 114;
        public const uint Jsonb       = 3802;
        public const uint JsonPath    = 4072;

        // public
        public const uint Refcursor   = 1790;
        public const uint Oidvector   = 30;
        public const uint Int2vector  = 22;
        public const uint Oid         = 26;
        public const uint Xid         = 28;
        public const uint Xid8        = 5069;
        public const uint Cid         = 29;
        public const uint Regtype     = 2206;
        public const uint Tid         = 27;
        public const uint PgLsn       = 3220;

        // Special
        public const uint Record      = 2249;
        public const uint Void        = 2278;
        public const uint Unknown     = 705;

        // Range types
        public const uint Int4Range   = 3904;
        public const uint Int8Range   = 3926;
        public const uint NumRange    = 3906;
        public const uint TsRange     = 3908;
        public const uint TsTzRange   = 3910;
        public const uint DateRange   = 3912;

        // Multirange types
        public const uint Int4Multirange   = 4451;
        public const uint Int8Multirange   = 4536;
        public const uint NumMultirange    = 4532;
        public const uint TsMultirange     = 4533;
        public const uint TsTzMultirange   = 4534;
        public const uint DateMultirange   = 4535;
    }
}
