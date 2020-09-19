﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers
{
    interface IMappedEnumHandler
    {
        /// <summary>
        /// The CLR type mapped to the PostgreSQL composite type.
        /// </summary>
        Type EnumType { get; }
    }

    class MappedEnumHandler<T> : NpgsqlTypeHandler<T>, IMappedEnumHandler where T : new()
    {
        readonly INpgsqlNameTranslator _nameTranslator;
        readonly NpgsqlConnection _conn;
        readonly UnmappedEnumHandler _wrappedHandler;

        public Type EnumType => typeof(T);

        internal MappedEnumHandler(PostgresType pgType, INpgsqlNameTranslator nameTranslator, NpgsqlConnection conn)
            : base(pgType)
        {
            _nameTranslator = nameTranslator;
            _conn = conn;
            _wrappedHandler = (UnmappedEnumHandler)new UnmappedEnumTypeHandlerFactory(_nameTranslator).Create(PostgresType, _conn);
        }

        public override ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null, CancellationToken cancellationToken = default)
            => _wrappedHandler.Read<T>(buf, len, async, fieldDescription, cancellationToken);

        public override int ValidateAndGetLength(T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => _wrappedHandler.ValidateAndGetLength(value, ref lengthCache, parameter);

        public override Task Write(T value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
           => _wrappedHandler.Write(value!, buf, lengthCache, parameter, async);
    }

    class MappedEnumTypeHandlerFactory<T> : NpgsqlTypeHandlerFactory<T>
        where T : new()
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        internal MappedEnumTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            _nameTranslator = nameTranslator;
        }

        public override NpgsqlTypeHandler<T> Create(PostgresType pgType, NpgsqlConnection conn)
            => new MappedEnumHandler<T>(pgType, _nameTranslator, conn);
    }
}
