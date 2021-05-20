using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    class TypeInfo
    {
        /// <summary>
        /// ID of the data type.
        /// </summary>
        public uint TypeId { get; private set; }

        /// <summary>
        /// Namespace (empty string for pg_catalog).
        /// </summary>
        public string Namespace { get; private set; } = null!;

        /// <summary>
        /// Name of the data type.
        /// </summary>
        public string Name { get; private set; } = null!;

        /// <summary>
        /// Name of the data type.
        /// </summary>
        public string FullName { get; private set; } = null!;

        public TypeInfo Populate(uint typeId, string ns, string name)
        {
            TypeId = typeId;
            Namespace = ns;
            // Hack: This is an unsafe way to detect array types
            // - What if the actual type starts with an underscore?
            // - What do we see here if the type name needs quotes?
            Name = name.StartsWith("_", StringComparison.Ordinal) ? $"{name.Substring(1)}[]" : name;
            FullName = Namespace.Length == 0 ? Name : $"{Namespace}.{Name}";
            return this;
        }
    }
}
