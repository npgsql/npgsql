using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Internal;
using NpgsqlTypes;

namespace Npgsql.NodaTime.Internal;

public class IntervalConverter : PgStreamingConverter<Interval>
{
    readonly PgConverter<NpgsqlRange<Instant>> _rangeConverter;

    public IntervalConverter(PgConverter<NpgsqlRange<Instant>> rangeConverter)
        => _rangeConverter = rangeConverter;

    public override Interval Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<Interval> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<Interval> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        var range = async
            ? await _rangeConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            : _rangeConverter.Read(reader);

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

    public override Size GetSize(SizeContext context, Interval value, ref object? writeState)
        => _rangeConverter.GetSize(context, IntervalToNpgsqlRange(value), ref writeState);

    public override void Write(PgWriter writer, Interval value)
        => _rangeConverter.Write(writer, IntervalToNpgsqlRange(value));

    public override ValueTask WriteAsync(PgWriter writer, Interval value, CancellationToken cancellationToken = default)
        => _rangeConverter.WriteAsync(writer, IntervalToNpgsqlRange(value), cancellationToken);

    static NpgsqlRange<Instant> IntervalToNpgsqlRange(Interval interval)
        => new(
            interval.HasStart ? interval.Start : default, true, !interval.HasStart,
            interval.HasEnd ? interval.End : default, false, !interval.HasEnd);
}
