
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Diagnostics;

using NodaTime;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.NodaTime.Internal
{
    partial class LegacyTimestampHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                Instant converted => ((INpgsqlSimpleTypeHandler<Instant>)this).ValidateAndGetLength(converted, parameter),
                
                LocalDateTime converted => ((INpgsqlSimpleTypeHandler<LocalDateTime>)this).ValidateAndGetLength(converted, parameter),
                
                DateTime converted => ((INpgsqlSimpleTypeHandler<DateTime>)this).ValidateAndGetLength(converted, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type LegacyTimestampHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                Instant converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                LocalDateTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                DateTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type LegacyTimestampHandler")
            };
    }
}
