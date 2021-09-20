
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

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
    partial class TimeTzHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                OffsetTime converted => ((INpgsqlSimpleTypeHandler<OffsetTime>)this).ValidateAndGetLength(converted, parameter),
                
                DateTimeOffset converted => ((INpgsqlSimpleTypeHandler<DateTimeOffset>)this).ValidateAndGetLength(converted, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type TimeTzHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                OffsetTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                DateTimeOffset converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type TimeTzHandler")
            };
    }
}
