﻿
using System;

using System.Collections.Generic;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using NodaTime;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandlers;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;

using NpgsqlTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.NodaTime.Internal
{
    partial class DateMultirangeHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                DateInterval[] converted => ((INpgsqlTypeHandler<DateInterval[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),

                List<DateInterval> converted => ((INpgsqlTypeHandler<List<DateInterval>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),

                NpgsqlRange<LocalDate>[] converted => ((INpgsqlTypeHandler<NpgsqlRange<LocalDate>[]>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),

                List<NpgsqlRange<LocalDate>> converted => ((INpgsqlTypeHandler<List<NpgsqlRange<LocalDate>>>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),


                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type DateMultirangeHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                DateInterval[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),

                List<DateInterval> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),

                NpgsqlRange<LocalDate>[] converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                List<NpgsqlRange<LocalDate>> converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),


                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type DateMultirangeHandler")
            };
    }
}
