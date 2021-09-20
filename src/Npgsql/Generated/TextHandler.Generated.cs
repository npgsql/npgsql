
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Data;

using System.Diagnostics.CodeAnalysis;

using System.IO;

using System.Text;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;

using Npgsql.TypeMapping;

using NpgsqlTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.Internal.TypeHandlers
{
    partial class TextHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                String converted => ((INpgsqlTypeHandler<String>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                char[] converted => ((INpgsqlTypeHandler<char[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                ArraySegment<Char> converted => ((INpgsqlTypeHandler<ArraySegment<Char>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Char converted => ((INpgsqlTypeHandler<Char>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                byte[] converted => ((INpgsqlTypeHandler<byte[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type TextHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                String converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                char[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                ArraySegment<Char> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Char converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                byte[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type TextHandler")
            };
    }
}
