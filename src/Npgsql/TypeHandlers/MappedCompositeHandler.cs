﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers
{
    interface IMappedCompositeHandler
    {
        /// <summary>
        /// The CLR type mapped to the PostgreSQL composite type.
        /// </summary>
        Type CompositeType { get; }
    }

    class MappedCompositeHandler<T> : NpgsqlTypeHandler<T>, IMappedCompositeHandler where T : new()
    {
        readonly INpgsqlNameTranslator _nameTranslator;
        readonly NpgsqlConnection _conn;
        [CanBeNull]
        UnmappedCompositeHandler _wrappedHandler;

        public Type CompositeType => typeof(T);

        internal MappedCompositeHandler(INpgsqlNameTranslator nameTranslator, NpgsqlConnection conn)
        {
            _nameTranslator = nameTranslator;
            _conn = conn;
        }

        void WrapHandler()
        {
            _wrappedHandler = (UnmappedCompositeHandler)new UnmappedCompositeTypeHandlerFactory(_nameTranslator).Create(PostgresType, _conn);
        }

        public override ValueTask<T> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            if (_wrappedHandler == null)
                WrapHandler();
            Debug.Assert(_wrappedHandler != null);
            return _wrappedHandler.Read<T>(buf, len, async, fieldDescription);
        }

        public override int ValidateAndGetLength(T value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (_wrappedHandler == null)
                WrapHandler();
            Debug.Assert(_wrappedHandler != null);
            return _wrappedHandler.ValidateAndGetLength(value, ref lengthCache, parameter);
        }

        public override Task Write(T value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
           => _wrappedHandler.Write(value, buf, lengthCache, parameter, async);
    }

    class MappedCompositeTypeHandlerFactory<T> : NpgsqlTypeHandlerFactory<T>
        where T : new()
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        internal MappedCompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            _nameTranslator = nameTranslator;
        }

        protected override NpgsqlTypeHandler<T> Create(NpgsqlConnection conn)
            => new MappedCompositeHandler<T>(_nameTranslator, conn);
    }
}
