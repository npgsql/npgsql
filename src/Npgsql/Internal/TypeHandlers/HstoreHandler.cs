using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

#if !NETSTANDARD2_0 && !NETSTANDARD2_1
using System.Collections.Immutable;
#endif

namespace Npgsql.Internal.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL hstore extension data type, which stores sets of key/value pairs within a
    /// single PostgreSQL value.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/hstore.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
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

        internal HstoreHandler(PostgresType postgresType, TextHandler textHandler)
            : base(postgresType)
            => _textHandler = textHandler;

        #region Write

        /// <inheritdoc />
        public int ValidateAndGetLength(IDictionary<string, string?> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
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
        public override int ValidateObjectAndGetLength(object? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
                ImmutableDictionary<string, string?> converted => ((INpgsqlTypeHandler<ImmutableDictionary<string, string?>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
#endif
                Dictionary<string, string?> converted => ((INpgsqlTypeHandler<Dictionary<string, string?>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                IDictionary<string, string?> converted => ((INpgsqlTypeHandler<IDictionary<string, string?>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),

                DBNull => 0,
                null => 0,
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type HstoreHandler")
            };

        /// <inheritdoc />
        public override Task WriteObjectWithLength(
            object? value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
            => value switch
            {
#if !NETSTANDARD2_0 && !NETSTANDARD2_1
                ImmutableDictionary<string, string?> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
#endif
                Dictionary<string, string?> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                IDictionary<string, string?> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type BoolHandler")
            };

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
                await _textHandler.WriteWithLength(kv.Key, buf, lengthCache, parameter, async, cancellationToken);
                await _textHandler.WriteWithLength(kv.Value, buf, lengthCache, parameter, async, cancellationToken);
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
            => new(Read(buf, len, async, fieldDescription).Result);

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
