
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

namespace Npgsql.Internal.TypeHandlers
{
    partial class ByteaHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                byte[] converted => ((INpgsqlTypeHandler<byte[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                ArraySegment<Byte> converted => ((INpgsqlTypeHandler<ArraySegment<Byte>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                ReadOnlyMemory<Byte> converted => ((INpgsqlTypeHandler<ReadOnlyMemory<Byte>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Memory<Byte> converted => ((INpgsqlTypeHandler<Memory<Byte>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type ByteaHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                byte[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                ArraySegment<Byte> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                ReadOnlyMemory<Byte> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Memory<Byte> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type ByteaHandler")
            };
    }
}
