﻿using System;
using System.Diagnostics;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.Internal.TypeHandling
{
    /// <summary>
    /// Base class for all type handler factories, which construct type handlers that know how
    /// to read and write CLR types from/to PostgreSQL types.
    /// </summary>
    /// <remarks>
    /// In general, do not inherit from this class, inherit from <see cref="NpgsqlTypeHandlerFactory{T}"/> instead.
    /// </remarks>
    public abstract class NpgsqlTypeHandlerFactory
    {
        /// <summary>
        /// Creates a type handler.
        /// </summary>
        public abstract NpgsqlTypeHandler CreateNonGeneric(PostgresType pgType, NpgsqlConnector conn);

        /// <summary>
        /// The default CLR type that handlers produced by this factory will read and write.
        /// </summary>
        public abstract Type DefaultValueType { get; }
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
        /// <summary>
        /// Creates a type handler.
        /// </summary>
        public abstract NpgsqlTypeHandler<TDefault> Create(PostgresType pgType, NpgsqlConnector conn);

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateNonGeneric(PostgresType pgType, NpgsqlConnector conn)
        {
            var handler = Create(pgType, conn);
            Debug.Assert(handler.PostgresType is not null);
            return handler;
        }

        /// <inheritdoc />
        public override Type DefaultValueType => typeof(TDefault);
    }

    /// <summary>
    /// A type handler factory used to instantiate Npgsql's built-in type handlers.
    /// </summary>
    class DefaultTypeHandlerFactory<THandler, TDefault> : NpgsqlTypeHandlerFactory
        where THandler : NpgsqlTypeHandler<TDefault>, new()
    {
        public override NpgsqlTypeHandler CreateNonGeneric(PostgresType pgType, NpgsqlConnector conn)
            => new THandler { PostgresType = pgType };

        public override Type DefaultValueType => typeof(TDefault);
    }
}
