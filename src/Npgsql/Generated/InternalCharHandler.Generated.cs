
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

namespace Npgsql.Internal.TypeHandlers.InternalTypeHandlers
{
    partial class InternalCharHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                Char converted => ((INpgsqlSimpleTypeHandler<Char>)this).ValidateAndGetLength(converted, parameter),
                
                Byte converted => ((INpgsqlSimpleTypeHandler<Byte>)this).ValidateAndGetLength(converted, parameter),
                
                Int16 converted => ((INpgsqlSimpleTypeHandler<Int16>)this).ValidateAndGetLength(converted, parameter),
                
                Int32 converted => ((INpgsqlSimpleTypeHandler<Int32>)this).ValidateAndGetLength(converted, parameter),
                
                Int64 converted => ((INpgsqlSimpleTypeHandler<Int64>)this).ValidateAndGetLength(converted, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type InternalCharHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                Char converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Byte converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int16 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int32 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int64 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type InternalCharHandler")
            };
    }
}
