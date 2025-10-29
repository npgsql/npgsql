using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Internal;
using NpgsqlTypes;

namespace Npgsql.NodaTime.Internal;

sealed class IntervalConverter(PgConverter<NpgsqlRange<Instant>> rangeConverter, bool dateTimeInfinityConversions) : PgStreamingConverter<Interval>
{
    public override Interval Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<Interval> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<Interval> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        var range = async
            ? await rangeConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            : rangeConverter.Read(reader);

        // NodaTime Interval includes the start instant and excludes the end instant.
        Instant? start = range.LowerBoundInfinite
            ? null
            : range.LowerBoundIsInclusive
                ? range.LowerBound
                : range.LowerBound + Duration.Epsilon;
        // For ranges with element types with infinity values (datetime, date etc.) an
        // inclusive lower/upper bound causes their -/+ infinity (respectively) to fall within the range.
        // If those values are returned for such a range postgres will not mark the affected bound as infinite accordingly.
        // This is documented in https://www.postgresql.org/docs/current/rangetypes.html#RANGETYPES-INFINITE
        // As NodaTime uses an exclusive upper bound we must consider this case as being another form of infinity (null).
        Instant? end = range.UpperBoundInfinite || (dateTimeInfinityConversions && range.UpperBoundIsInclusive && range.UpperBound == Instant.MaxValue)
            ? null
            : range.UpperBoundIsInclusive
                ? range.UpperBound + Duration.Epsilon
                : range.UpperBound;

        return new(start, end);
    }

    public override Size GetSize(SizeContext context, Interval value, ref object? writeState)
        => rangeConverter.GetSize(context, IntervalToNpgsqlRange(value), ref writeState);

    public override void Write(PgWriter writer, Interval value)
        => rangeConverter.Write(writer, IntervalToNpgsqlRange(value));

    public override ValueTask WriteAsync(PgWriter writer, Interval value, CancellationToken cancellationToken = default)
        => rangeConverter.WriteAsync(writer, IntervalToNpgsqlRange(value), cancellationToken);

    static NpgsqlRange<Instant> IntervalToNpgsqlRange(Interval interval)
        => new(
            interval.HasStart ? interval.Start : default, true, !interval.HasStart,
            interval.HasEnd ? interval.End : default, false, !interval.HasEnd);
}
