namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL data type that isn't known to Npgsql and cannot be handled.
    /// </summary>
    public class UnknownBackendType : PostgresType
    {
        internal static readonly PostgresType Instance = new UnknownBackendType();

        /// <summary>
        /// Constructs a the unknown backend type.
        /// </summary>
#pragma warning disable CA2222 // Do not decrease inherited member visibility
        UnknownBackendType() : base("", "<unknown>", 0) { }
#pragma warning restore CA2222 // Do not decrease inherited member visibility
    }
}
