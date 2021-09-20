
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers.NumericHandlers
{
    partial class Int32Handler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                Int32 converted => ((INpgsqlSimpleTypeHandler<Int32>)this).ValidateAndGetLength(converted, parameter),
                
                Byte converted => ((INpgsqlSimpleTypeHandler<Byte>)this).ValidateAndGetLength(converted, parameter),
                
                Int16 converted => ((INpgsqlSimpleTypeHandler<Int16>)this).ValidateAndGetLength(converted, parameter),
                
                Int64 converted => ((INpgsqlSimpleTypeHandler<Int64>)this).ValidateAndGetLength(converted, parameter),
                
                Single converted => ((INpgsqlSimpleTypeHandler<Single>)this).ValidateAndGetLength(converted, parameter),
                
                Double converted => ((INpgsqlSimpleTypeHandler<Double>)this).ValidateAndGetLength(converted, parameter),
                
                Decimal converted => ((INpgsqlSimpleTypeHandler<Decimal>)this).ValidateAndGetLength(converted, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type Int32Handler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                Int32 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Byte converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int16 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int64 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Single converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Double converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Decimal converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type Int32Handler")
            };
    }
}
