using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers
{
    public partial class MultirangeHandler<TSubtype> : NpgsqlTypeHandler<NpgsqlRange<TSubtype>[]>,
        INpgsqlTypeHandler<List<NpgsqlRange<TSubtype>>>
    {
        /// <summary>
        /// The type handler for the range that this multirange type holds
        /// </summary>
        protected RangeHandler<TSubtype> RangeHandler { get; }

        /// <inheritdoc />
        public MultirangeHandler(PostgresMultirangeType pgMultirangeType, RangeHandler<TSubtype> rangeHandler)
            : base(pgMultirangeType)
            => RangeHandler = rangeHandler;

        public override ValueTask<NpgsqlRange<TSubtype>[]> Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => ReadMultirangeArray<TSubtype>(buf, len, async, fieldDescription);

        protected async ValueTask<NpgsqlRange<TAnySubtype>[]> ReadMultirangeArray<TAnySubtype>(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new NpgsqlRange<TAnySubtype>[numRanges];

            for (var i = 0; i < numRanges; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange[i] = await RangeHandler.ReadRange<TAnySubtype>(buf, rangeLen, async, fieldDescription);
            }

            return multirange;
        }

        ValueTask<List<NpgsqlRange<TSubtype>>> INpgsqlTypeHandler<List<NpgsqlRange<TSubtype>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeList<TSubtype>(buf, len, async, fieldDescription);

        protected async ValueTask<List<NpgsqlRange<TAnySubtype>>> ReadMultirangeList<TAnySubtype>(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new List<NpgsqlRange<TAnySubtype>>(numRanges);

            for (var i = 0; i < numRanges; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange.Add(await RangeHandler.ReadRange<TAnySubtype>(buf, rangeLen, async, fieldDescription));
            }

            return multirange;
        }

        public override int ValidateAndGetLength(NpgsqlRange<TSubtype>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(List<NpgsqlRange<TSubtype>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        protected int ValidateAndGetLengthMultirange<TAnySubtype>(
            IList<NpgsqlRange<TAnySubtype>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire array length, and go ahead an populate the element slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);

            var sum = 4 + 4 * value.Count;
            for (var i = 0; i < value.Count; i++)
                sum += RangeHandler.ValidateAndGetLength(value[i], ref lengthCache, parameter);

            lengthCache.Lengths[pos] = sum;
            return sum;
        }

        public override Task Write(
            NpgsqlRange<TSubtype>[] value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(
            List<NpgsqlRange<TSubtype>> value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public async Task WriteMultirange<TAnySubtype>(
            IList<NpgsqlRange<TAnySubtype>> value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            buf.WriteInt32(value.Count);

            for (var i = 0; i < value.Count; i++)
                await RangeHandler.WriteWithLength(value[i], buf, lengthCache, parameter: null, async, cancellationToken);
        }
    }

    public class MultirangeHandler<TSubtype1, TSubtype2> : MultirangeHandler<TSubtype1>,
        INpgsqlTypeHandler<NpgsqlRange<TSubtype2>[]>, INpgsqlTypeHandler<List<NpgsqlRange<TSubtype2>>>
    {
        /// <inheritdoc />
        public MultirangeHandler(PostgresMultirangeType pgMultirangeType, RangeHandler<TSubtype1, TSubtype2> rangeHandler)
            : base(pgMultirangeType, rangeHandler) {}

        ValueTask<NpgsqlRange<TSubtype2>[]> INpgsqlTypeHandler<NpgsqlRange<TSubtype2>[]>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeArray<TSubtype2>(buf, len, async, fieldDescription);

        ValueTask<List<NpgsqlRange<TSubtype2>>> INpgsqlTypeHandler<List<NpgsqlRange<TSubtype2>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeList<TSubtype2>(buf, len, async, fieldDescription);

        public int ValidateAndGetLength(List<NpgsqlRange<TSubtype2>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<TSubtype2>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public Task Write(
            List<NpgsqlRange<TSubtype2>> value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(
            NpgsqlRange<TSubtype2>[] value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public override int ValidateObjectAndGetLength(object? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                NpgsqlRange<TSubtype1>[] converted => ((INpgsqlTypeHandler<NpgsqlRange<TSubtype1>[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                NpgsqlRange<TSubtype2>[] converted => ((INpgsqlTypeHandler<NpgsqlRange<TSubtype2>[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                List<NpgsqlRange<TSubtype1>> converted => ((INpgsqlTypeHandler<List<NpgsqlRange<TSubtype1>>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                List<NpgsqlRange<TSubtype2>> converted => ((INpgsqlTypeHandler<List<NpgsqlRange<TSubtype2>>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),

                DBNull => 0,
                null => 0,
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type RangeHandler<TElement>")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                NpgsqlRange<TSubtype1>[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                NpgsqlRange<TSubtype2>[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                List<NpgsqlRange<TSubtype1>> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                List<NpgsqlRange<TSubtype2>> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type RangeHandler<TElement>")
            };
    }
}
