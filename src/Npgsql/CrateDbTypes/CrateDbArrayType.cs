using Npgsql.PostgresTypes;

namespace Npgsql.CrateDbTypes
{
    /// <summary>
    /// Represents a array type supported by CrateDB.
    /// </summary>
    public class CrateDbArrayType : PostgresArrayType
    {
        /// <summary>
        /// Creates an instance of the CrateDbArrayType class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oid"></param>
        /// <param name="elementType"></param>
        public CrateDbArrayType(string name, uint oid, PostgresType elementType)
            : base("pg_catalog", name, oid, elementType)
        {
        }
    }
}
