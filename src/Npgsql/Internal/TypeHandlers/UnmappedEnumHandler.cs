using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers;

sealed class UnmappedEnumHandler : TextHandler
{
    readonly INpgsqlNameTranslator _nameTranslator;

    // Note that a separate instance of UnmappedEnumHandler is created for each PG enum type, so concurrency isn't "really" needed.
    // However, in theory multiple different CLR enums may be used with the same PG enum type, and even if there's only one, we only know
    // about it late (after construction), when the user actually reads/writes with one. So this handler is fully thread-safe.
    readonly ConcurrentDictionary<Type, TypeRecord> _types = new();

    internal UnmappedEnumHandler(PostgresEnumType pgType, INpgsqlNameTranslator nameTranslator, Encoding encoding)
        : base(pgType, encoding)
        => _nameTranslator = nameTranslator;

    #region Read

    protected internal override async ValueTask<TAny> ReadCustom<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
    {
        var s = await base.Read(buf, len, async, fieldDescription);
        if (typeof(TAny) == typeof(string))
            return (TAny)(object)s;

        var typeRecord = GetTypeRecord(typeof(TAny));

        if (!typeRecord.LabelToEnum.TryGetValue(s, out var value))
            throw new InvalidCastException($"Received enum value '{s}' from database which wasn't found on enum {typeof(TAny)}");

        // TODO: Avoid boxing
        return (TAny)(object)value;
    }

    public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        => base.Read(buf, len, async, fieldDescription);

    #endregion

    #region Write

    public override int ValidateObjectAndGetLength(object? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => value is null || value is DBNull
            ? 0
            : ValidateAndGetLength(value, ref lengthCache, parameter);

    protected internal override int ValidateAndGetLengthCustom<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength(value!, ref lengthCache, parameter);

    [UnconditionalSuppressMessage("Unmapped enums currently aren't trimming-safe.", "IL2072")]
    int ValidateAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var type = value.GetType();
        if (type == typeof(string))
            return base.ValidateAndGetLength((string)value, ref lengthCache, parameter);

        var typeRecord = GetTypeRecord(type);

        // TODO: Avoid boxing
        return typeRecord.EnumToLabel.TryGetValue((Enum)value, out var str)
            ? base.ValidateAndGetLength(str, ref lengthCache, parameter)
            : throw new InvalidCastException($"Can't write value {value} as enum {type}");
    }

    // TODO: This boxes the enum (again)
    protected override Task WriteWithLengthCustom<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
        => WriteObjectWithLength(value!, buf, lengthCache, parameter, async, cancellationToken);

    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        if (value is null || value is DBNull)
            return WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken);

        if (buf.WriteSpaceLeft < 4)
            return WriteWithLengthLong(value, buf, lengthCache, parameter, async, cancellationToken);

        buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
        return Write(value, buf, lengthCache, parameter, async, cancellationToken);

        async Task WriteWithLengthLong(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
        {
            await buf.Flush(async, cancellationToken);
            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            await Write(value, buf, lengthCache, parameter, async, cancellationToken);
        }
    }

    [UnconditionalSuppressMessage("Unmapped enums currently aren't trimming-safe.", "IL2072")]
    internal Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = value.GetType();
        if (type == typeof(string))
            return base.Write((string)value, buf, lengthCache, parameter, async, cancellationToken);

        var typeRecord = GetTypeRecord(type);

        // TODO: Avoid boxing
        if (!typeRecord.EnumToLabel.TryGetValue((Enum)value, out var str))
            throw new InvalidCastException($"Can't write value {value} as enum {type}");
        return base.Write(str, buf, lengthCache, parameter, async, cancellationToken);
    }

    #endregion

    #region Misc

    TypeRecord GetTypeRecord(Type type)
    {
#if NETSTANDARD2_0
        return _types.GetOrAdd(type, t => CreateTypeRecord(t, _nameTranslator));
#else
        return _types.GetOrAdd(type, static (t, translator) => CreateTypeRecord(t, translator), _nameTranslator);
#endif
    }

    static TypeRecord CreateTypeRecord(Type type, INpgsqlNameTranslator nameTranslator)
    {
        var enumToLabel = new Dictionary<Enum, string>();
        var labelToEnum = new Dictionary<string, Enum>();

        foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var attribute = (PgNameAttribute?)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
            var enumName = attribute?.PgName ?? nameTranslator.TranslateMemberName(field.Name);
            var enumValue = (Enum)field.GetValue(null)!;

            enumToLabel[enumValue] = enumName;
            labelToEnum[enumName] = enumValue;
        }

        return new(enumToLabel, labelToEnum);
    }

    #endregion

    record struct TypeRecord(Dictionary<Enum, string> EnumToLabel, Dictionary<string, Enum> LabelToEnum);
}