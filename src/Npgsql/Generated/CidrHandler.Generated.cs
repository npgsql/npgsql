
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Net;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;

using NpgsqlTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers.NetworkHandlers
{
    partial class CidrHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                ValueTuple<IPAddress,Int32> converted => ((INpgsqlSimpleTypeHandler<ValueTuple<IPAddress,Int32>>)this).ValidateAndGetLength(converted, parameter),
                
                NpgsqlInet converted => ((INpgsqlSimpleTypeHandler<NpgsqlInet>)this).ValidateAndGetLength(converted, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type CidrHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                ValueTuple<IPAddress,Int32> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlInet converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type CidrHandler")
            };
    }
}
