using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;
using NpgsqlTypes;

namespace Npgsql.NodaTime.Internal;

public class DateIntervalConverter(
    PgConverter<NpgsqlRange<MaybeInfiniteLocalDate>> readRangeConverter,
    PgConverter<NpgsqlRange<LocalDate>> writeRangeConverter,
    bool dateTimeInfinityConversions)
    : PgStreamingConverter<DateInterval>
{
    public override DateInterval Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<DateInterval> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<DateInterval> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        var range = async
            ? await readRangeConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            : readRangeConverter.Read(reader);

        if (range.IsEmpty)
            throw new InvalidCastException("Cannot read an empty range as a NodaTime DateInterval.");

        var lowerBound = range.LowerBound;
        var lowerBoundValue = lowerBound.Value;
        var upperBound = range.UpperBound;
        var upperBoundValue = upperBound.Value;

        if (range.LowerBoundInfinite || lowerBound.IsInfinite)
        {
            lowerBoundValue = dateTimeInfinityConversions
                ? LocalDate.MinIsoValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue);
        }

        if (range.UpperBoundInfinite || upperBound.IsInfinite)
        {
            upperBoundValue = dateTimeInfinityConversions
                ? LocalDate.MaxIsoValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue);
        }
        else
        {
            // PostgreSQL normalizes ranges to [,) format
            // Since DateInterval implies [,], subtract a single day from the upper bound
            upperBoundValue -= Period.FromDays(1);
        }

        return new(lowerBoundValue, upperBoundValue);
    }

    protected override Size BindValue(in BindContext context, DateInterval value, ref object? writeState)
        => writeRangeConverter.Bind(context, new NpgsqlRange<LocalDate>(value.Start, value.End), ref writeState);

    public override void Write(PgWriter writer, DateInterval value)
        => writeRangeConverter.Write(writer, new NpgsqlRange<LocalDate>(value.Start, value.End));

    public override ValueTask WriteAsync(PgWriter writer, DateInterval value, CancellationToken cancellationToken = default)
        => writeRangeConverter.WriteAsync(writer, new NpgsqlRange<LocalDate>(value.Start, value.End), cancellationToken);
}
