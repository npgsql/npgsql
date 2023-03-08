using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

sealed class UnsupportedHandler : NpgsqlTypeHandler
{
    readonly string _exceptionMessage;

    public UnsupportedHandler(PostgresType postgresType, string exceptionMessage) : base(postgresType)
        => _exceptionMessage = exceptionMessage;

    public override ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        => throw new NotSupportedException(_exceptionMessage);

    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => throw new NotSupportedException(_exceptionMessage);

    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException(_exceptionMessage);

    protected internal override ValueTask<TAny> ReadCustom<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => throw new NotSupportedException(_exceptionMessage);

    protected override Task WriteWithLengthCustom<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken)
        => throw new NotSupportedException(_exceptionMessage);

    protected internal override int ValidateAndGetLengthCustom<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => throw new NotSupportedException(_exceptionMessage);

    public override Type GetFieldType(FieldDescription? fieldDescription = null)
        => throw new NotSupportedException(_exceptionMessage);

    public override NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode)
        => throw new NotSupportedException(_exceptionMessage);

    public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
        => throw new NotSupportedException(_exceptionMessage);

    public override NpgsqlTypeHandler CreateMultirangeHandler(PostgresMultirangeType pgMultirangeType)
        => throw new NotSupportedException(_exceptionMessage);
}
