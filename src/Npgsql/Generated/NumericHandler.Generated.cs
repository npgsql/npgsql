
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Globalization;

using System.Numerics;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers.NumericHandlers
{
    partial class NumericHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                Decimal converted => ((INpgsqlTypeHandler<Decimal>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Byte converted => ((INpgsqlTypeHandler<Byte>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Int16 converted => ((INpgsqlTypeHandler<Int16>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Int32 converted => ((INpgsqlTypeHandler<Int32>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Int64 converted => ((INpgsqlTypeHandler<Int64>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Single converted => ((INpgsqlTypeHandler<Single>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Double converted => ((INpgsqlTypeHandler<Double>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                BigInteger converted => ((INpgsqlTypeHandler<BigInteger>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type NumericHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                Decimal converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Byte converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int16 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int32 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Int64 converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Single converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Double converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                BigInteger converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type NumericHandler")
            };
    }
}
