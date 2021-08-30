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
    public partial class MultirangeHandler<TElement> : NpgsqlTypeHandler<NpgsqlRange<TElement>[]>,
        INpgsqlTypeHandler<List<NpgsqlRange<TElement>>>
    {
        /// <summary>
        /// The type handler for the range that this multirange type holds
        /// </summary>
        readonly RangeHandler<TElement> _rangeHandler;

        /// <inheritdoc />
        public Type[] SupportedMultirangeClrTypes { get; }

        /// <inheritdoc />
        public MultirangeHandler(PostgresMultirangeType pgMultirangeType, RangeHandler<TElement> rangeHandler)
            : this(pgMultirangeType, rangeHandler, new[] { typeof(NpgsqlRange<TElement>[]), typeof(List<NpgsqlRange<TElement>>) }) {}

        /// <inheritdoc />
        protected MultirangeHandler(
            PostgresMultirangeType pgMultirangeType, RangeHandler<TElement> rangeHandler, Type[] supportedSubtypeClrTypes)
            : base(pgMultirangeType)
        {
            _rangeHandler = rangeHandler;
            SupportedMultirangeClrTypes = supportedSubtypeClrTypes;
        }

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
                multirange[i] = await _rangeHandler.Read(buf, rangeLen, async, fieldDescription);
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
                multirange.Add(await _rangeHandler.Read(buf, rangeLen, async, fieldDescription));
            }

            return multirange;
        }

        public override int ValidateAndGetLength(NpgsqlRange<TElement>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var sum = 4 + 4 * value.Length;
            for (var i = 0; i < value.Length; i++)
                sum += _rangeHandler.ValidateAndGetLength(value[i], ref lengthCache, parameter: null);

            return lengthCache.Set(sum);
        }

        public int ValidateAndGetLength(List<NpgsqlRange<TElement>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var sum = 4 + 4 * value.Count;
            for (var i = 0; i < value.Count; i++)
                sum += _rangeHandler.ValidateAndGetLength(value[i], ref lengthCache, parameter: null);

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
                await _rangeHandler.WriteWithLength(value[i], buf, lengthCache, parameter: null, async, cancellationToken);
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
                await _rangeHandler.WriteWithLength(value[i], buf, lengthCache, parameter: null, async, cancellationToken);
        }
    }
}
