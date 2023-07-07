using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using NpgsqlTypes;

namespace Npgsql.NodaTime.Internal;

public class DateIntervalConverter : PgStreamingConverter<DateInterval>
{
    readonly bool _dateTimeInfinityConversions;
    readonly RangeConverter<LocalDate> _rangeConverter;

    public DateIntervalConverter(bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _rangeConverter = new(new LocalDateConverter(dateTimeInfinityConversions));
    }

    public override DateInterval Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<DateInterval> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<DateInterval> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        var range = async
            ? await _rangeConverter.ReadAsync(reader, cancellationToken).ConfigureAwait(false)
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            : _rangeConverter.Read(reader);

        var upperBound = range.UpperBound;

        if (upperBound != LocalDate.MaxIsoValue || !_dateTimeInfinityConversions)
            upperBound -= Period.FromDays(1);

        return new(range.LowerBound, upperBound);
    }

    public override Size GetSize(SizeContext context, DateInterval value, ref object? writeState)
        => _rangeConverter.GetSize(context, new NpgsqlRange<LocalDate>(value.Start, value.End), ref writeState);

    public override void Write(PgWriter writer, DateInterval value)
        => _rangeConverter.Write(writer, new NpgsqlRange<LocalDate>(value.Start, value.End));

    public override ValueTask WriteAsync(PgWriter writer, DateInterval value, CancellationToken cancellationToken = default)
        => _rangeConverter.WriteAsync(writer, new NpgsqlRange<LocalDate>(value.Start, value.End), cancellationToken);
}
