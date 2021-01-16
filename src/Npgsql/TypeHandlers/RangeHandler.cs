using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// An interface implementing by <see cref="RangeHandler{TElement}"/>, exposing the handler's supported range
    /// CLR types.
    /// </summary>
    public interface IRangeHandler
    {
        /// <summary>
        /// Exposes the range CLR types supported by this handler.
        /// </summary>
        Type[] SupportedRangeClrTypes { get; }
    }

    /// <summary>
    /// A type handler for PostgreSQL range types.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/rangetypes.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    /// <typeparam name="TElement">the range subtype</typeparam>
    public class RangeHandler<TElement> : NpgsqlTypeHandler<NpgsqlRange<TElement>>, IRangeHandler
    {
        /// <summary>
        /// The type handler for the element that this range type holds
        /// </summary>
        readonly NpgsqlTypeHandler _elementHandler;

        /// <inheritdoc />
        public Type[] SupportedRangeClrTypes { get; }

        /// <inheritdoc />
        public RangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler elementHandler)
            : this(rangePostgresType, elementHandler, new[] { typeof(NpgsqlRange<TElement>)}) {}

        /// <inheritdoc />
        protected RangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler elementHandler, Type[] supportedElementClrTypes)
            : base(rangePostgresType)
        {
            _elementHandler = elementHandler;
            SupportedRangeClrTypes = supportedElementClrTypes;
        }

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandler<NpgsqlRange<TElement>>(arrayBackendType, this, arrayNullabilityMode);

        internal override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(NpgsqlRange<TElement>);
        internal override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(NpgsqlRange<TElement>);

        /// <inheritdoc />
        public override IRangeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => throw new NotSupportedException();

        #region Read

        /// <inheritdoc />
        public override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read<TAny>(buf, len, false, fieldDescription).Result;

        /// <inheritdoc />
        public override ValueTask<NpgsqlRange<TElement>> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => DoRead<TElement>(buf, len, async, fieldDescription);

        private protected async ValueTask<NpgsqlRange<TAny>> DoRead<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(1, async);

            var flags = (RangeFlags)buf.ReadByte();
            if ((flags & RangeFlags.Empty) != 0)
                return NpgsqlRange<TAny>.Empty;

            var lowerBound = default(TAny);
            var upperBound = default(TAny);

            if ((flags & RangeFlags.LowerBoundInfinite) == 0)
                lowerBound = await _elementHandler.ReadWithLength<TAny>(buf, async);

            if ((flags & RangeFlags.UpperBoundInfinite) == 0)
                upperBound = await _elementHandler.ReadWithLength<TAny>(buf, async);

            return new NpgsqlRange<TAny>(lowerBound, upperBound, flags);
        }

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlRange<TElement> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        private protected int ValidateAndGetLength<TAny>(NpgsqlRange<TAny> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            var totalLen = 1;
            var lengthCachePos = lengthCache?.Position ?? 0;
            if (!value.IsEmpty)
            {
                if (!value.LowerBoundInfinite)
                {
                    totalLen += 4;
                    if (!(value.LowerBound is null) && typeof(TElement) != typeof(DBNull))
                        totalLen += _elementHandler.ValidateAndGetLength(value.LowerBound, ref lengthCache, null);
                }

                if (!value.UpperBoundInfinite)
                {
                    totalLen += 4;
                    if (!(value.UpperBound is null) && typeof(TElement) != typeof(DBNull))
                        totalLen += _elementHandler.ValidateAndGetLength(value.UpperBound, ref lengthCache, null);
                }
            }

            // If we're traversing an already-populated length cache, rewind to first element slot so that
            // the elements' handlers can access their length cache values
            if (lengthCache != null && lengthCache.IsPopulated)
                lengthCache.Position = lengthCachePos;

            return totalLen;
        }

        internal override Task WriteWithLengthInternal<TAny>([AllowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            if (value == null || typeof(TAny) == typeof(DBNull))
            {
                buf.WriteInt32(-1);
                return Task.CompletedTask;
            }

            return WriteWithLengthCore();

            async Task WriteWithLengthLong()
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);

                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    buf.WriteInt32(-1);
                    return;
                }

                await WriteWithLengthCore();
            }

            Task WriteWithLengthCore()
            {
                if (this is INpgsqlTypeHandler<TAny> typedHandler)
                {
                    buf.WriteInt32(typedHandler.ValidateAndGetLength(value, ref lengthCache, parameter));
                    return typedHandler.Write(value, buf, lengthCache, parameter, async, cancellationToken);
                }
                else
                    throw new InvalidCastException($"Can't write CLR type {typeof(TAny)} to database type {PgDisplayName}");
            }
        }

        /// <inheritdoc />
        public override Task Write(NpgsqlRange<TElement> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write(value, buf, lengthCache, parameter, async, cancellationToken);

        private protected async Task Write<TAny>(NpgsqlRange<TAny> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async, cancellationToken);

            buf.WriteByte((byte)value.Flags);

            if (value.IsEmpty)
                return;

            if (!value.LowerBoundInfinite)
                await _elementHandler.WriteWithLengthInternal(value.LowerBound, buf, lengthCache, null, async, cancellationToken);

            if (!value.UpperBoundInfinite)
                await _elementHandler.WriteWithLengthInternal(value.UpperBound, buf, lengthCache, null, async, cancellationToken);
        }

        #endregion
    }

    /// <summary>
    /// Type handler for PostgreSQL range types
    /// </summary>
    /// <remarks>
    /// Introduced in PostgreSQL 9.2.
    /// https://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    /// <typeparam name="TElement1">the main range subtype</typeparam>
    /// <typeparam name="TElement2">an alternative range subtype</typeparam>
    public class RangeHandler<TElement1, TElement2> : RangeHandler<TElement1>, INpgsqlTypeHandler<NpgsqlRange<TElement2>>
    {
        /// <inheritdoc />
        public RangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler elementHandler)
            : base(rangePostgresType, elementHandler, new[] { typeof(NpgsqlRange<TElement1>), typeof(NpgsqlRange<TElement2>) }) {}

        ValueTask<NpgsqlRange<TElement2>> INpgsqlTypeHandler<NpgsqlRange<TElement2>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => DoRead<TElement2>(buf, len, async, fieldDescription);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlRange<TElement2> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength<TElement2>(value, ref lengthCache, parameter);

        /// <inheritdoc />
        public Task Write(NpgsqlRange<TElement2> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write<TElement2>(value, buf, lengthCache, parameter, async, cancellationToken);
    }
}
