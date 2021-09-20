
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Collections.Generic;

using System.Text;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;

using NpgsqlTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers.FullTextSearchHandlers
{
    partial class TsQueryHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                NpgsqlTsQueryEmpty converted => ((INpgsqlTypeHandler<NpgsqlTsQueryEmpty>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                NpgsqlTsQueryLexeme converted => ((INpgsqlTypeHandler<NpgsqlTsQueryLexeme>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                NpgsqlTsQueryNot converted => ((INpgsqlTypeHandler<NpgsqlTsQueryNot>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                NpgsqlTsQueryAnd converted => ((INpgsqlTypeHandler<NpgsqlTsQueryAnd>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                NpgsqlTsQueryOr converted => ((INpgsqlTypeHandler<NpgsqlTsQueryOr>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                NpgsqlTsQueryFollowedBy converted => ((INpgsqlTypeHandler<NpgsqlTsQueryFollowedBy>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type TsQueryHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                NpgsqlTsQueryEmpty converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlTsQueryLexeme converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlTsQueryNot converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlTsQueryAnd converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlTsQueryOr converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                NpgsqlTsQueryFollowedBy converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type TsQueryHandler")
            };
    }
}
