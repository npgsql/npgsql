
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Collections.Generic;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;

using NpgsqlTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers
{
    partial class MultirangeHandler<TElement>
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                NpgsqlTypes.NpgsqlRange<TElement>[] converted => ((INpgsqlTypeHandler<NpgsqlTypes.NpgsqlRange<TElement>[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                List<NpgsqlRange<TElement>> converted => ((INpgsqlTypeHandler<List<NpgsqlRange<TElement>>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type MultirangeHandler<TElement>")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                NpgsqlTypes.NpgsqlRange<TElement>[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                List<NpgsqlRange<TElement>> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type MultirangeHandler<TElement>")
            };
    }
}
