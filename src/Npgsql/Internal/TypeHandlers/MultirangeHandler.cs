using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers
{
    public partial class MultirangeHandler<TElement> : NpgsqlTypeHandler<NpgsqlRange<TElement>[]>,
        INpgsqlTypeHandler<List<NpgsqlRange<TElement>>>
    {
        /// <summary>
        /// The type handler for the range that this multirange type holds
        /// </summary>
        protected  RangeHandler<TElement> RangeHandler { get; }

        /// <inheritdoc />
        public MultirangeHandler(PostgresMultirangeType pgMultirangeType, RangeHandler<TElement> rangeHandler)
            : base(pgMultirangeType)
            => RangeHandler = rangeHandler;

        public override async ValueTask<NpgsqlRange<TElement>[]> Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new NpgsqlRange<TElement>[numRanges];

            for (var i = 0; i < numRanges; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange[i] = await RangeHandler.Read(buf, rangeLen, async, fieldDescription);
            }

            return multirange;
        }

        async ValueTask<List<NpgsqlRange<TElement>>> INpgsqlTypeHandler<List<NpgsqlRange<TElement>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new List<NpgsqlRange<TElement>>(numRanges);

            for (var i = 0; i < numRanges; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange.Add(await RangeHandler.Read(buf, rangeLen, async, fieldDescription));
            }

            return multirange;
        }

        public override int ValidateAndGetLength(NpgsqlRange<TElement>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value, ref lengthCache);

        public int ValidateAndGetLength(List<NpgsqlRange<TElement>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value, ref lengthCache);

        int ValidateAndGetLengthCore(IList<NpgsqlRange<TElement>> value, ref NpgsqlLengthCache? lengthCache)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var sum = 4 + 4 * value.Count;
            for (var i = 0; i < value.Count; i++)
                sum += RangeHandler.ValidateAndGetLength(value[i], ref lengthCache, parameter: null);

            return lengthCache.Set(sum);
        }

        public override async Task Write(
            NpgsqlRange<TElement>[] value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            buf.WriteInt32(value.Length);

            for (var i = 0; i < value.Length; i++)
                await RangeHandler.WriteWithLength(value[i], buf, lengthCache, parameter: null, async, cancellationToken);
        }

        public async Task Write(
            List<NpgsqlRange<TElement>> value,
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
}
