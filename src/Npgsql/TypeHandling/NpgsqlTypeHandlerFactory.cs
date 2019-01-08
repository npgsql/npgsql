using System;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Base class for all type handler factories, which construct type handlers that know how
    /// to read and write CLR types from/to PostgreSQL types.
    /// Do not inherit from this class, inherit from <see cref="NpgsqlTypeHandlerFactory{T}"/> instead.
    /// </summary>
    public abstract class NpgsqlTypeHandlerFactory
    {
        internal abstract NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn);

        /// <summary>
        /// The default CLR type that handlers produced by this factory will read and write.
        /// </summary>
        internal abstract Type DefaultValueType { get; }
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
    public abstract class NpgsqlTypeHandlerFactory<TDefault> : NpgsqlTypeHandlerFactory
    {
        internal override NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn)
        {
            var handler = Create(conn);
            handler.PostgresType = pgType;
            return handler;
        }

        /// <summary>
        /// Creates a type handler. The provided connection can be examined to modify type handler
        /// behavior based on server settings, etc.
        /// </summary>
        protected abstract NpgsqlTypeHandler<TDefault> Create(NpgsqlConnection conn);

        /// <summary>
        /// The default CLR type that handlers produced by this factory will read and write.
        /// </summary>
        internal override Type DefaultValueType => typeof(TDefault);
    }
}
