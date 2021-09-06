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
    public partial class DateRangeHandler : RangeHandler<LocalDate>, INpgsqlTypeHandler<DateInterval>
    {
        public DateRangeHandler(PostgresType rangePostgresType, NpgsqlTypeHandler subtypeHandler)
            : base(rangePostgresType, subtypeHandler)
        {
        }

        async ValueTask<DateInterval> INpgsqlTypeHandler<DateInterval>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        {
            var range = await Read(buf, len, async, fieldDescription);
            return new(range.LowerBound, range.UpperBound - Period.FromDays(1));
        }

        public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(DateInterval);
        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(DateInterval);

        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async,
            FieldDescription? fieldDescription = null)
            => (await Read<DateInterval>(buf, len, async, fieldDescription))!;

        public int ValidateAndGetLength(DateInterval value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(new NpgsqlRange<LocalDate>(value.Start, value.End), ref lengthCache, parameter);

        public Task Write(
            DateInterval value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
            CancellationToken cancellationToken = default)
            => Write(new NpgsqlRange<LocalDate>(value.Start, value.End), buf, lengthCache, parameter, async, cancellationToken);
    }
}
