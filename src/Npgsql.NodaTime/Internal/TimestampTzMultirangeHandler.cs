using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.NodaTime.Internal
{
    public partial class TimestampTzMultirangeHandler : MultirangeHandler<Instant>,
        INpgsqlTypeHandler<Interval[]>, INpgsqlTypeHandler<List<Interval>>,
        INpgsqlTypeHandler<NpgsqlRange<ZonedDateTime>[]>, INpgsqlTypeHandler<List<NpgsqlRange<ZonedDateTime>>>,
        INpgsqlTypeHandler<NpgsqlRange<OffsetDateTime>[]>, INpgsqlTypeHandler<List<NpgsqlRange<OffsetDateTime>>>,
        INpgsqlTypeHandler<NpgsqlRange<DateTime>[]>, INpgsqlTypeHandler<List<NpgsqlRange<DateTime>>>,
        INpgsqlTypeHandler<NpgsqlRange<DateTimeOffset>[]>, INpgsqlTypeHandler<List<NpgsqlRange<DateTimeOffset>>>
    {
        readonly INpgsqlTypeHandler<Interval> _intervalHandler;

        public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(Interval[]);
        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(Interval[]);

        public TimestampTzMultirangeHandler(PostgresMultirangeType pgMultirangeType, TimestampTzRangeHandler rangeHandler)
            : base(pgMultirangeType, rangeHandler)
            => _intervalHandler = rangeHandler;

        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async,
            FieldDescription? fieldDescription = null)
            => (await Read<Interval[]>(buf, len, async, fieldDescription))!;

        async ValueTask<Interval[]> INpgsqlTypeHandler<Interval[]>.Read(NpgsqlReadBuffer buf, int len, bool async,
            FieldDescription? fieldDescription)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new Interval[numRanges];

            for (var i = 0; i < multirange.Length; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange[i] = await _intervalHandler.Read(buf, rangeLen, async, fieldDescription);
            }

            return multirange;
        }

        async ValueTask<List<Interval>> INpgsqlTypeHandler<List<Interval>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new List<Interval>(numRanges);

            for (var i = 0; i < numRanges; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange.Add(await _intervalHandler.Read(buf, rangeLen, async, fieldDescription));
            }

            return multirange;
        }

        public int ValidateAndGetLength(List<Interval> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value, ref lengthCache);

        public int ValidateAndGetLength(Interval[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value, ref lengthCache);

        int ValidateAndGetLengthCore(IList<Interval> value, ref NpgsqlLengthCache? lengthCache)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var sum = 4 + 4 * value.Count;
            for (var i = 0; i < value.Count; i++)
                sum += _intervalHandler.ValidateAndGetLength(value[i], ref lengthCache, parameter: null);

            return lengthCache!.Set(sum);
        }

        public async Task Write(Interval[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            buf.WriteInt32(value.Length);

            for (var i = 0; i < value.Length; i++)
                await RangeHandler.WriteWithLength(value[i], buf, lengthCache, parameter: null, async, cancellationToken);
        }

        public async Task Write(List<Interval> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);

            buf.WriteInt32(value.Count);

            for (var i = 0; i < value.Count; i++)
                await RangeHandler.WriteWithLength(value[i], buf, lengthCache, parameter: null, async, cancellationToken);
        }

        #region Boilerplate

        ValueTask<NpgsqlRange<ZonedDateTime>[]> INpgsqlTypeHandler<NpgsqlRange<ZonedDateTime>[]>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeArray<ZonedDateTime>(buf, len, async, fieldDescription);

        ValueTask<List<NpgsqlRange<ZonedDateTime>>> INpgsqlTypeHandler<List<NpgsqlRange<ZonedDateTime>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeList<ZonedDateTime>(buf, len, async, fieldDescription);

        ValueTask<NpgsqlRange<OffsetDateTime>[]> INpgsqlTypeHandler<NpgsqlRange<OffsetDateTime>[]>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeArray<OffsetDateTime>(buf, len, async, fieldDescription);

        ValueTask<List<NpgsqlRange<OffsetDateTime>>> INpgsqlTypeHandler<List<NpgsqlRange<OffsetDateTime>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeList<OffsetDateTime>(buf, len, async, fieldDescription);

        ValueTask<NpgsqlRange<DateTime>[]> INpgsqlTypeHandler<NpgsqlRange<DateTime>[]>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeArray<DateTime>(buf, len, async, fieldDescription);

        ValueTask<List<NpgsqlRange<DateTime>>> INpgsqlTypeHandler<List<NpgsqlRange<DateTime>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeList<DateTime>(buf, len, async, fieldDescription);

        ValueTask<NpgsqlRange<DateTimeOffset>[]> INpgsqlTypeHandler<NpgsqlRange<DateTimeOffset>[]>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeArray<DateTimeOffset>(buf, len, async, fieldDescription);

        ValueTask<List<NpgsqlRange<DateTimeOffset>>> INpgsqlTypeHandler<List<NpgsqlRange<DateTimeOffset>>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadMultirangeList<DateTimeOffset>(buf, len, async, fieldDescription);

        public int ValidateAndGetLength(NpgsqlRange<ZonedDateTime>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(List<NpgsqlRange<ZonedDateTime>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<OffsetDateTime>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(List<NpgsqlRange<OffsetDateTime>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<DateTime>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(List<NpgsqlRange<DateTime>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<DateTimeOffset>[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(List<NpgsqlRange<DateTimeOffset>> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthMultirange(value, ref lengthCache, parameter);

        public Task Write(NpgsqlRange<ZonedDateTime>[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
                NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(List<NpgsqlRange<ZonedDateTime>> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(NpgsqlRange<OffsetDateTime>[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(List<NpgsqlRange<OffsetDateTime>> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(NpgsqlRange<DateTime>[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(List<NpgsqlRange<DateTime>> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(NpgsqlRange<DateTimeOffset>[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(List<NpgsqlRange<DateTimeOffset>> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteMultirange(value, buf, lengthCache, parameter, async, cancellationToken);

        #endregion Boilerplate
    }
}
