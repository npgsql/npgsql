using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Handles "conversions" for columns sent by the database with unknown OIDs.
    /// This differs from TextHandler in that its a text-only handler (we don't want to receive binary
    /// representations of the types registered here).
    /// Note that this handler is also used in the very initial query that loads the OID mappings
    /// (chicken and egg problem).
    /// </summary>
    internal class UnknownTypeHandler : TextHandler
    {
        static readonly string[] _pgNames = { "unknown" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return false; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Unknown };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Object };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override bool SupportsBinaryWrite { get { return false; } }
    }
}
