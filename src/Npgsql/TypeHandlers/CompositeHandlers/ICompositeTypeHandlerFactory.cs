namespace Npgsql.TypeHandlers.CompositeHandlers
{
    /// <summary>
    /// Interface implemented by all mapped composite handler factories.
    /// Used to expose the name translator for those reflecting composite mappings (e.g. EF Core).
    /// </summary>
    public interface ICompositeTypeHandlerFactory
    {
        /// <summary>
        /// The name translator used for this composite.
        /// </summary>
        INpgsqlNameTranslator NameTranslator { get; }
    }
}
