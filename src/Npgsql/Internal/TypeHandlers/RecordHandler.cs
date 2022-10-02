using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.Internal.TypeHandlers;

/// <summary>
/// Type handler for PostgreSQL record types. Defaults to returning object[], but can also return <see cref="ValueTuple" /> or <see cref="Tuple"/>.
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-pseudo.html
///
/// Encoding (identical to composite):
/// A 32-bit integer with the number of columns, then for each column:
/// * An OID indicating the type of the column
/// * The length of the column(32-bit integer), or -1 if null
/// * The column data encoded as binary
/// </remarks>
sealed partial class RecordHandler : NpgsqlTypeHandler<object[]>
{
    readonly TypeMapper _typeMapper;

    public RecordHandler(PostgresType postgresType, TypeMapper typeMapper)
        : base(postgresType)
        => _typeMapper = typeMapper;

    #region Read

    protected internal override async ValueTask<T> ReadCustom<T>(
        NpgsqlReadBuffer buf,
        int len,
        bool async,
        FieldDescription? fieldDescription)
    {
        if (typeof(T) == typeof(object[]))
            return (T)(object)await Read(buf, len, async, fieldDescription);

        if (typeof(T).FullName?.StartsWith("System.ValueTuple`", StringComparison.Ordinal) == true ||
            typeof(T).FullName?.StartsWith("System.Tuple`", StringComparison.Ordinal) == true)
        {
            var asArray = await Read(buf, len, async, fieldDescription);
            if (typeof(T).GenericTypeArguments.Length != asArray.Length)
                throw new InvalidCastException($"Cannot read record type with {asArray.Length} fields as {typeof(T)}");

            var constructor = typeof(T).GetConstructors().Single(c => c.GetParameters().Length == asArray.Length);
            return (T)constructor.Invoke(asArray);
        }

        return await base.ReadCustom<T>(buf, len, async, fieldDescription);
    }

    public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        => await Read(buf, len, async, fieldDescription);

    public override async ValueTask<object[]> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
    {
        await buf.Ensure(4, async);
        var fieldCount = buf.ReadInt32();
        var result = new object[fieldCount];

        for (var i = 0; i < fieldCount; i++)
        {
            await buf.Ensure(8, async);
            var typeOID = buf.ReadUInt32();
            var fieldLen = buf.ReadInt32();
            if (fieldLen == -1)  // Null field, simply skip it and leave at default
                continue;
            result[i] = await _typeMapper.ResolveByOID(typeOID).ReadAsObject(buf, fieldLen, async);
        }

        return result;
    }

    /// <inheritdoc />
    public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override NpgsqlTypeHandler CreateMultirangeHandler(PostgresMultirangeType pgMultirangeType)
        => throw new NotSupportedException();

    #endregion

    #region Write (unsupported)

    public override int ValidateAndGetLength(object[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => throw new NotSupportedException("Can't write record types");

    public override Task Write(
        object[] value,
        NpgsqlWriteBuffer buf,
        NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter,
        bool async,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Can't write record types");

    #endregion
}