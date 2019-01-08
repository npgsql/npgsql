using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Indicates that this property or field correspond to a PostgreSQL field with the specified name
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PgNameAttribute : Attribute
    {
        /// <summary>
        /// The name of PostgreSQL field that corresponds to this CLR property or field
        /// </summary>
        public string PgName { get; private set; }

        /// <summary>
        /// Indicates that this property or field correspond to a PostgreSQL field with the specified name
        /// </summary>
        /// <param name="pgName">The name of PostgreSQL field that corresponds to this CLR property or field</param>
        public PgNameAttribute(string pgName)
        {
            PgName = pgName;
        }
    }
}
