using Npgsql.PostgresTypes;

namespace Npgsql.CrateDbTypes
{
    /// <summary>
    /// Represents a base type supported by CrateDB.
    /// </summary>
    public class CrateDbBaseType : PostgresBaseType
    {
        /// <summary>
        /// Creates an instance of the CrateDbBaseType class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oid"></param>
        public CrateDbBaseType(string name, uint oid)
            : base("pg_catalog", name, oid)
        {
        }
    }
}
