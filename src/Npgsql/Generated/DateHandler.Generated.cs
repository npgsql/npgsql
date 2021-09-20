
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;

using NpgsqlTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers
{
    partial class DateHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                DateTime converted => ((INpgsqlSimpleTypeHandler<DateTime>)this).ValidateAndGetLength(converted, parameter),
                
                NpgsqlDate converted => ((INpgsqlSimpleTypeHandler<NpgsqlDate>)this).ValidateAndGetLength(converted, parameter),
#if NET6_0_OR_GREATER
                DateOnly converted => ((INpgsqlSimpleTypeHandler<DateOnly>)this).ValidateAndGetLength(converted, parameter),
#endif

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type DateHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                DateTime converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlDate converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
#if NET6_0_OR_GREATER
                DateOnly converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
#endif


                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type DateHandler")
            };
    }
}
