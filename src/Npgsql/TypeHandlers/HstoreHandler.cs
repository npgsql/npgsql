using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

#if !NETSTANDARD2_0 && !NETSTANDARD2_1
using System.Collections.Immutable;
#endif

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL hstore extension data type, which stores sets of key/value pairs
    /// within a single PostgreSQL value.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/hstore.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("hstore", NpgsqlDbType.Hstore, new[]
    {
        typeof(Dictionary<string, string?>),
        typeof(IDictionary<string, string?>),
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
        typeof(ImmutableDictionary<string, string?>)
#endif
    })]
    public class HstoreHandlerFactory : NpgsqlTypeHandlerFactory<Dictionary<string, string?>>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<Dictionary<string, string?>> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new HstoreHandler(postgresType, conn);
    }

    /// <summary>
    /// A type handler for the PostgreSQL hstore extension data type, which stores sets of key/value pairs within a
    /// single PostgreSQL value.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/hstore.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
#pragma warning disable CA1061 // Do not hide base class methods
    public class HstoreHandler :
        NpgsqlTypeHandler<Dictionary<string, string?>>,
        INpgsqlTypeHandler<IDictionary<string, string?>>
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
        , INpgsqlTypeHandler<ImmutableDictionary<string, string?>>
#endif
    {
        /// <summary>
        /// The text handler to which we delegate encoding/decoding of the actual strings
        /// </summary>
        readonly TextHandler _textHandler;

        internal HstoreHandler(PostgresType postgresType, NpgsqlConnection connection)
            : base(postgresType) => _textHandler = new TextHandler(postgresType, connection);

        #region Write

        /// <inheritdoc />
        public int ValidateAndGetLength(IDictionary<string, string?> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire hstore length, and go ahead an populate the individual string slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);

            var totalLen = 4;  // Number of key-value pairs
            foreach (var kv in value)
            {
                totalLen += 8;   // Key length + value length
                if (kv.Key == null)
                    throw new FormatException("HSTORE doesn't support null keys");
                totalLen += _textHandler.ValidateAndGetLength(kv.Key, ref lengthCache, null);
                if (kv.Value != null)
                    totalLen += _textHandler.ValidateAndGetLength(kv.Value!, ref lengthCache, null);
            }

            return lengthCache.Lengths[pos] = totalLen;
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength(Dictionary<string, string?> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        /// <inheritdoc />
        public async Task Write(IDictionary<string, string?> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(value.Count);
            if (value.Count == 0)
                return;

            foreach (var kv in value)
            {
                await _textHandler.WriteWithLengthInternal(kv.Key, buf, lengthCache, parameter, async, cancellationToken);
                await _textHandler.WriteWithLengthInternal(kv.Value, buf, lengthCache, parameter, async, cancellationToken);
            }
        }

        /// <inheritdoc />
        public override Task Write(Dictionary<string, string?> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write(value, buf, lengthCache, parameter, async, cancellationToken);

        #endregion

        #region Read

        async ValueTask<T> ReadInto<T>(T dictionary, int numElements, NpgsqlReadBuffer buf, bool async)
            where T : IDictionary<string, string?>
        {
            for (var i = 0; i < numElements; i++)
            {
                await buf.Ensure(4, async);
                var keyLen = buf.ReadInt32();
                Debug.Assert(keyLen != -1);
                var key = await _textHandler.Read(buf, keyLen, async);

                await buf.Ensure(4, async);
                var valueLen = buf.ReadInt32();

                dictionary[key] = valueLen == -1
                    ? null
                    : await _textHandler.Read(buf, valueLen, async);
            }
            return dictionary;
        }

        /// <inheritdoc />
        public override async ValueTask<Dictionary<string, string?>> Read(NpgsqlReadBuffer buf, int len, bool async,
            FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numElements = buf.ReadInt32();
            return await ReadInto(new Dictionary<string, string?>(numElements), numElements, buf, async);
        }

        ValueTask<IDictionary<string, string?>> INpgsqlTypeHandler<IDictionary<string, string?>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => new ValueTask<IDictionary<string, string?>>(Read(buf, len, async, fieldDescription).Result);

        #endregion

#if !NETSTANDARD2_0 && !NETSTANDARD2_1
        #region ImmutableDictionary

        /// <inheritdoc />
        public int ValidateAndGetLength(
            ImmutableDictionary<string, string?> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((IDictionary<string, string?>)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public Task Write(ImmutableDictionary<string, string?> value,
            NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((IDictionary<string, string?>)value, buf, lengthCache, parameter, async, cancellationToken);

        async ValueTask<ImmutableDictionary<string, string?>> INpgsqlTypeHandler<ImmutableDictionary<string, string?>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4, async);
            var numElements = buf.ReadInt32();
            return (await ReadInto(ImmutableDictionary<string, string?>.Empty.ToBuilder(), numElements, buf, async))
                .ToImmutable();
        }

        #endregion
#endif
    }
#pragma warning restore CA1061 // Do not hide base class methods
}
