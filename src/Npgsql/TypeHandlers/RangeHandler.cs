﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using NpgsqlTypes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.TypeHandlers
{
    public abstract class RangeHandler : NpgsqlTypeHandler
    {
        protected RangeHandler(PostgresType rangePostgresType) : base(rangePostgresType) {}

        public override RangeHandler CreateRangeHandler(PostgresRangeType rangeBackendType)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Type handler for PostgreSQL range types
    /// </summary>
    /// <remarks>
    /// Introduced in PostgreSQL 9.2.
    /// http://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    /// <typeparam name="TElement">the range subtype</typeparam>
    public class RangeHandler<TElement> : RangeHandler, INpgsqlTypeHandler<NpgsqlRange<TElement>>
    {
        /// <summary>
        /// The type handler for the element that this range type holds
        /// </summary>
        readonly NpgsqlTypeHandler _elementHandler;

        public RangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler elementHandler)
            : base(rangePostgresType) => _elementHandler = elementHandler;

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType)
            => new ArrayHandler<NpgsqlRange<TElement>>(arrayBackendType, this);

        internal override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(NpgsqlRange<TElement>);
        internal override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(NpgsqlRange<TElement>);

        #region Read

        internal override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read<TAny>(buf, len, false, fieldDescription).Result;

        protected internal override ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            if (this is INpgsqlTypeHandler<TAny> typedHandler)
                return typedHandler.Read(buf, len, async, fieldDescription);

            buf.Skip(len); // Perform this in sync for performance
            throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                ? $"Can't cast database type to {typeof(TAny).Name}"
                : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}"
            ));
        }

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => await Read(buf, len, async, fieldDescription);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => Read(buf, len, false, fieldDescription).Result;

#pragma warning disable CS8653
        public async ValueTask<NpgsqlRange<TElement>> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(1, async);

            var flags = (RangeFlags)buf.ReadByte();
            if ((flags & RangeFlags.Empty) != 0)
                return NpgsqlRange<TElement>.Empty;

            var lowerBound = default(TElement);
            var upperBound = default(TElement);

            if ((flags & RangeFlags.LowerBoundInfinite) == 0)
                lowerBound = await _elementHandler.ReadWithLength<TElement>(buf, async);

            if ((flags & RangeFlags.UpperBoundInfinite) == 0)
                upperBound = await _elementHandler.ReadWithLength<TElement>(buf, async);

            return new NpgsqlRange<TElement>(lowerBound, upperBound, flags);
        }
#pragma warning restore CS8653

        #endregion

        #region Write

        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => this is INpgsqlTypeHandler<TAny> typedHandler
                ? typedHandler.ValidateAndGetLength(value, ref lengthCache, parameter)
                : throw new InvalidCastException($"Can't write CLR type {typeof(TAny)} to database type {PgDisplayName}");

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlRange<TElement>)value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<TElement> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            var totalLen = 1;
            var lengthCachePos = lengthCache?.Position ?? 0;
            if (!value.IsEmpty)
            {
                if (!value.LowerBoundInfinite)
                    totalLen += 4 + _elementHandler.ValidateAndGetLength(value.LowerBound, ref lengthCache, null);

                if (!value.UpperBoundInfinite)
                    totalLen += 4 + _elementHandler.ValidateAndGetLength(value.UpperBound, ref lengthCache, null);
            }

            // If we're traversing an already-populated length cache, rewind to first element slot so that
            // the elements' handlers can access their length cache values
            if (lengthCache != null && lengthCache.IsPopulated)
                lengthCache.Position = lengthCachePos;

            return totalLen;
        }

        internal override Task WriteWithLengthInternal<TAny>([AllowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
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
                    await buf.Flush(async);

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
                    return typedHandler.Write(value, buf, lengthCache, parameter, async);
                }
                else
                    throw new InvalidCastException($"Can't write CLR type {typeof(TAny)} to database type {PgDisplayName}");
            }
        }

        // The default WriteObjectWithLength casts the type handler to INpgsqlTypeHandler<T>, but that's not sufficient for
        // us (need to handle many types of T, e.g. int[], int[,]...)
        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => value is DBNull
                ? WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async)
                : WriteWithLengthInternal((NpgsqlRange<TElement>)value, buf, lengthCache, parameter, async);

        public async Task Write(NpgsqlRange<TElement> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);

            buf.WriteByte((byte)value.Flags);

            if (value.IsEmpty)
                return;

            if (!value.LowerBoundInfinite)
                await _elementHandler.WriteWithLengthInternal(value.LowerBound, buf, lengthCache, null, async);

            if (!value.UpperBoundInfinite)
                await _elementHandler.WriteWithLengthInternal(value.UpperBound, buf, lengthCache, null, async);
        }

        #endregion
    }
}
