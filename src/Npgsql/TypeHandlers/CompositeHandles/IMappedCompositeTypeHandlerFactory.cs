namespace Npgsql.TypeHandlers.CompositeHandlers
{

    /// <summary>
    /// Interface implemented by all mapped composite handler factories.
    /// Used to expose the name translator for those reflecting enum mappings (e.g. EF Core).
    /// </summary>
    public interface IMappedCompositeTypeHandlerFactory
    {
        /// <summary>
        /// The name translator used for this enum.
        /// </summary>
        INpgsqlNameTranslator NameTranslator { get; }
    }
}