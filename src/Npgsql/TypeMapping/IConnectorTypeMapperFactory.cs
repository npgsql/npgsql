using JetBrains.Annotations;
using System.Threading.Tasks;

namespace Npgsql.TypeMapping
{
    /// <summary>
    /// A factory which get generate instances of <see cref="ConnectorTypeMapper"/>, which describe the type database
    /// mapping for a connection. When first connecting to a database, Npgsql will attempt to load the correct
    /// mappings.
    /// </summary>
    internal interface IConnectorTypeMapperFactory
    {
        /// <summary>
        /// Given a connection, loads all necessary type mapping information about the connected database, e.g. its types.
        /// </summary>
        /// <returns>
        /// An object describing the mappings of the database to which <paramref name="conn"/> is connected, or null if the
        /// database isn't of the correct type and isn't handled by this factory.
        /// </returns>
        [ItemCanBeNull]
        Task<ConnectorTypeMapper> Load(NpgsqlConnection conn, NpgsqlConnector connector);
    }
}
