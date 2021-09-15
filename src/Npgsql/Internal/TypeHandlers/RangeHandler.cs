using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers
{
    /// <summary>
    /// A type handler for PostgreSQL range types.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/rangetypes.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    /// <typeparam name="TElement">The range subtype.</typeparam>
    public partial class RangeHandler<TElement> : NpgsqlTypeHandler<NpgsqlRange<TElement>>
    {
        /// <summary>
        /// The type handler for the subtype that this range type holds
        /// </summary>
        readonly NpgsqlTypeHandler _subtypeHandler;

        /// <inheritdoc />
        public RangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler subtypeHandler)
            : base(rangePostgresType)
            => _subtypeHandler = subtypeHandler;

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandler<NpgsqlRange<TElement>>(pgArrayType, this, arrayNullabilityMode);

        public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(NpgsqlRange<TElement>);
        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(NpgsqlRange<TElement>);

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
            => throw new NotSupportedException();

        #region Read

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
                lowerBound = await _subtypeHandler.ReadWithLength<TAny>(buf, async);

            if ((flags & RangeFlags.UpperBoundInfinite) == 0)
                upperBound = await _subtypeHandler.ReadWithLength<TAny>(buf, async);

            return new NpgsqlRange<TAny>(lowerBound, upperBound, flags);
        }

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlRange<TElement> value, [NotNullIfNotNull("lengthCache")] ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
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
                    if (value.LowerBound is not null)
                        totalLen += _subtypeHandler.ValidateAndGetLength(value.LowerBound, ref lengthCache, null);
                }

                if (!value.UpperBoundInfinite)
                {
                    totalLen += 4;
                    if (value.UpperBound is not null)
                        totalLen += _subtypeHandler.ValidateAndGetLength(value.UpperBound, ref lengthCache, null);
                }
            }

            // If we're traversing an already-populated length cache, rewind to first element slot so that
            // the elements' handlers can access their length cache values
            if (lengthCache != null && lengthCache.IsPopulated)
                lengthCache.Position = lengthCachePos;

            return totalLen;
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
                await _subtypeHandler.WriteWithLength(value.LowerBound, buf, lengthCache, null, async, cancellationToken);

            if (!value.UpperBoundInfinite)
                await _subtypeHandler.WriteWithLength(value.UpperBound, buf, lengthCache, null, async, cancellationToken);
        }

        #endregion
    }

    /// <summary>
    /// Type handler for PostgreSQL range types.
    /// </summary>
    /// <remarks>
    /// Introduced in PostgreSQL 9.2.
    /// https://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    /// <typeparam name="TElement1">The main range subtype.</typeparam>
    /// <typeparam name="TElement2">An alternative range subtype.</typeparam>
    public class RangeHandler<TElement1, TElement2> : RangeHandler<TElement1>, INpgsqlTypeHandler<NpgsqlRange<TElement2>>
    {
        /// <inheritdoc />
        public RangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler subtypeHandler)
            : base(rangePostgresType, subtypeHandler) {}

        ValueTask<NpgsqlRange<TElement2>> INpgsqlTypeHandler<NpgsqlRange<TElement2>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => DoRead<TElement2>(buf, len, async, fieldDescription);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlRange<TElement2> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength<TElement2>(value, ref lengthCache, parameter);

        /// <inheritdoc />
        public Task Write(NpgsqlRange<TElement2> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write<TElement2>(value, buf, lengthCache, parameter, async, cancellationToken);

        public override int ValidateObjectAndGetLength(object? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                NpgsqlRange<TElement1> converted => ((INpgsqlTypeHandler<NpgsqlRange<TElement1>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                NpgsqlRange<TElement2> converted => ((INpgsqlTypeHandler<NpgsqlRange<TElement2>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),

                DBNull => 0,
                null => 0,
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type RangeHandler<TElement>")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                NpgsqlRange<TElement1> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                NpgsqlRange<TElement2> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type RangeHandler<TElement>")
            };
    }
}
