
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using NodaTime;

using NodaTime.TimeZones;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.NodaTime.Internal
{
    partial class LegacyTimestampTzHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                Instant converted => ((INpgsqlSimpleTypeHandler<Instant>)this).ValidateAndGetLength(converted, parameter),
                
                ZonedDateTime converted => ((INpgsqlSimpleTypeHandler<ZonedDateTime>)this).ValidateAndGetLength(converted, parameter),
                
                OffsetDateTime converted => ((INpgsqlSimpleTypeHandler<OffsetDateTime>)this).ValidateAndGetLength(converted, parameter),
                
                DateTimeOffset converted => ((INpgsqlSimpleTypeHandler<DateTimeOffset>)this).ValidateAndGetLength(converted, parameter),
                
                DateTime converted => ((INpgsqlSimpleTypeHandler<DateTime>)this).ValidateAndGetLength(converted, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type LegacyTimestampTzHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                Instant converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                ZonedDateTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                OffsetDateTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                DateTimeOffset converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                DateTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type LegacyTimestampTzHandler")
            };
    }
}
