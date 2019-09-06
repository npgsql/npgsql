using System;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Interface for all type handler factories, which construct type handlers that know how
    /// to read and write CLR types from/to PostgreSQL types.
    /// Do not inherit from this class, inherit from <see cref="NpgsqlTypeHandlerFactory{T}"/> instead.
    /// </summary>
    public interface INpgsqlTypeHandlerFactory
    {
        /// <summary>
        /// Creates a type handler.
        /// </summary>
        NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn);

        /// <summary>
        /// The default CLR type that handlers produced by this factory will read and write.
        /// </summary>
        Type DefaultValueType { get; }
    }

    /// <summary>
    /// Base class for all type handler factories, which construct type handlers that know how
    /// to read and write CLR types from/to PostgreSQL types. Type handler factories are set up
    /// via <see cref="NpgsqlTypeMapping"/> in either the global or connection-specific type mapper.
    /// </summary>
    /// <seealso cref="NpgsqlTypeMapping"/>
    /// <seealso cref="NpgsqlConnection.GlobalTypeMapper"/>
    /// <seealso cref="NpgsqlConnection.TypeMapper"/>
    /// <typeparam name="TDefault">The default CLR type that handlers produced by this factory will read and write.</typeparam>
    public abstract class NpgsqlTypeHandlerFactory<TDefault> : INpgsqlTypeHandlerFactory
    {
        /// <summary>
        /// Creates a type handler
        /// </summary>
        public abstract NpgsqlTypeHandler<TDefault> Create(PostgresType pgType, NpgsqlConnection conn);

        NpgsqlTypeHandler INpgsqlTypeHandlerFactory.Create(PostgresType pgType, NpgsqlConnection conn)
            => Create(pgType, conn);

        /// <inheritdoc />
        public Type DefaultValueType => typeof(TDefault);
    }
}
