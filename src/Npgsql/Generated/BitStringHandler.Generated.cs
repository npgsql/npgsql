
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Collections;

using System.Collections.Specialized;

using System.Diagnostics;

using System.Linq;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers
{
    partial class BitStringHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                BitArray converted => ((INpgsqlTypeHandler<BitArray>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                BitVector32 converted => ((INpgsqlTypeHandler<BitVector32>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Boolean converted => ((INpgsqlTypeHandler<Boolean>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                String converted => ((INpgsqlTypeHandler<String>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type BitStringHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                BitArray converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                BitVector32 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Boolean converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                String converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type BitStringHandler")
            };
    }
}
