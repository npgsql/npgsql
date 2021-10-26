using System;
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
    public partial class TimestampTzRangeHandler : RangeHandler<Instant>,
        INpgsqlTypeHandler<Interval>, INpgsqlTypeHandler<NpgsqlRange<ZonedDateTime>>, INpgsqlTypeHandler<NpgsqlRange<OffsetDateTime>>,
        INpgsqlTypeHandler<NpgsqlRange<DateTime>>, INpgsqlTypeHandler<NpgsqlRange<DateTimeOffset>>
    {
        public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(Interval);
        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(Interval);

        public TimestampTzRangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler subtypeHandler)
            : base(rangePostgresType, subtypeHandler)
        {
        }

        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async,
            FieldDescription? fieldDescription = null)
            => (await Read<Interval>(buf, len, async, fieldDescription))!;

        // internal Interval ConvertRangetoInterval(NpgsqlRange<Instant> range)
        async ValueTask<Interval> INpgsqlTypeHandler<Interval>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            var range = await Read(buf, len, async, fieldDescription);

            // NodaTime Interval includes the start instant and excludes the end instant.
            Instant? start = range.LowerBoundInfinite
                ? null
                : range.LowerBoundIsInclusive
                    ? range.LowerBound
                    : range.LowerBound + Duration.Epsilon;
            Instant? end = range.UpperBoundInfinite
                ? null
                : range.UpperBoundIsInclusive
                    ? range.UpperBound + Duration.Epsilon
                    : range.UpperBound;
            return new(start, end);
        }

        public int ValidateAndGetLength(Interval value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthRange(
                new NpgsqlRange<Instant>(value.Start, true, !value.HasStart, value.End, false, !value.HasEnd), ref lengthCache, parameter);

        public Task Write(Interval value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteRange(new NpgsqlRange<Instant>(value.Start, true, !value.HasStart, value.End, false, !value.HasEnd),
                buf, lengthCache, parameter, async, cancellationToken);

        #region Boilerplate

        ValueTask<NpgsqlRange<ZonedDateTime>> INpgsqlTypeHandler<NpgsqlRange<ZonedDateTime>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadRange<ZonedDateTime>(buf, len, async, fieldDescription);

        ValueTask<NpgsqlRange<OffsetDateTime>> INpgsqlTypeHandler<NpgsqlRange<OffsetDateTime>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadRange<OffsetDateTime>(buf, len, async, fieldDescription);

        ValueTask<NpgsqlRange<DateTime>> INpgsqlTypeHandler<NpgsqlRange<DateTime>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadRange<DateTime>(buf, len, async, fieldDescription);

        ValueTask<NpgsqlRange<DateTimeOffset>> INpgsqlTypeHandler<NpgsqlRange<DateTimeOffset>>.Read(
            NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadRange<DateTimeOffset>(buf, len, async, fieldDescription);

        public int ValidateAndGetLength(NpgsqlRange<ZonedDateTime> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthRange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<OffsetDateTime> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthRange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<DateTime> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthRange(value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlRange<DateTimeOffset> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthRange(value, ref lengthCache, parameter);

        public Task Write(NpgsqlRange<ZonedDateTime> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteRange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(NpgsqlRange<OffsetDateTime> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteRange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(NpgsqlRange<DateTime> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteRange(value, buf, lengthCache, parameter, async, cancellationToken);

        public Task Write(NpgsqlRange<DateTimeOffset> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteRange(value, buf, lengthCache, parameter, async, cancellationToken);

        #endregion Boilerplate
    }
}
