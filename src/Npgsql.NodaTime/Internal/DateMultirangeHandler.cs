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
    public partial class DateMultirangeHandler : MultirangeHandler<LocalDate>,
        INpgsqlTypeHandler<DateInterval[]>, INpgsqlTypeHandler<List<DateInterval>>
    {
        readonly INpgsqlTypeHandler<DateInterval> _dateIntervalHandler;

        public DateMultirangeHandler(PostgresMultirangeType multirangePostgresType, DateRangeHandler rangeHandler)
            : base(multirangePostgresType, rangeHandler)
            => _dateIntervalHandler = rangeHandler;

        public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(DateInterval[]);
        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(DateInterval[]);

        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async,
            FieldDescription? fieldDescription = null)
            => (await Read<DateInterval[]>(buf, len, async, fieldDescription))!;

        async ValueTask<DateInterval[]> INpgsqlTypeHandler<DateInterval[]>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new DateInterval[numRanges];

            for (var i = 0; i < multirange.Length; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange[i] = await _dateIntervalHandler.Read(buf, rangeLen, async, fieldDescription);
            }

            return multirange;
        }

        async ValueTask<List<DateInterval>> INpgsqlTypeHandler<List<DateInterval>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            await buf.Ensure(4, async);
            var numRanges = buf.ReadInt32();
            var multirange = new List<DateInterval>(numRanges);

            for (var i = 0; i < numRanges; i++)
            {
                await buf.Ensure(4, async);
                var rangeLen = buf.ReadInt32();
                multirange.Add(await _dateIntervalHandler.Read(buf, rangeLen, async, fieldDescription));
            }

            return multirange;
        }

        public int ValidateAndGetLength(DateInterval[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value, ref lengthCache);

        public int ValidateAndGetLength(List<DateInterval> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value, ref lengthCache);

        int ValidateAndGetLengthCore(IList<DateInterval> value, ref NpgsqlLengthCache? lengthCache)
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var sum = 4 + 4 * value.Count;
            for (var i = 0; i < value.Count; i++)
                sum += _dateIntervalHandler.ValidateAndGetLength(value[i], ref lengthCache, parameter: null);

            return lengthCache!.Set(sum);
        }

        public async Task Write(
            DateInterval[] value,
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
            List<DateInterval> value,
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
            {
                var interval = value[i];
                await RangeHandler.WriteWithLength(
                    new NpgsqlRange<LocalDate>(interval.Start, interval.End), buf, lengthCache, parameter: null, async, cancellationToken);
            }
        }
    }
}
