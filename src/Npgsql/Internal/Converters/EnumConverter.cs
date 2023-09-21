using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Npgsql.Internal.Converters;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
sealed class EnumConverter<TEnum> : PgBufferedConverter<TEnum> where TEnum : struct, Enum
{
    readonly Dictionary<TEnum, string> _enumToLabel;
    readonly Dictionary<string, TEnum> _labelToEnum;
    readonly Encoding _encoding;

    // Unmapped enums
    public EnumConverter(Dictionary<Enum, string> enumToLabel, Dictionary<string, Enum> labelToEnum, Encoding encoding)
    {
        _enumToLabel = new(enumToLabel.Count);
        foreach (var kv in enumToLabel)
            _enumToLabel.Add((TEnum)kv.Key, kv.Value);

        _labelToEnum = new(labelToEnum.Count);
        foreach (var kv in labelToEnum)
            _labelToEnum.Add(kv.Key, (TEnum)kv.Value);

        _encoding = encoding;
    }

    public EnumConverter(Dictionary<TEnum, string> enumToLabel, Dictionary<string, TEnum> labelToEnum, Encoding encoding)
    {
        _enumToLabel = enumToLabel;
        _labelToEnum = labelToEnum;
        _encoding = encoding;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Value;
        return format is DataFormat.Binary or DataFormat.Text;
    }

    public override Size GetSize(SizeContext context, TEnum value, ref object? writeState)
    {
        if (!_enumToLabel.TryGetValue(value, out var str))
            throw new InvalidCastException($"Can't write value {value} as enum {typeof(TEnum)}");

        return _encoding.GetByteCount(str);
    }

    protected override TEnum ReadCore(PgReader reader)
    {
        var str = _encoding.GetString(reader.ReadBytes(reader.CurrentRemaining));
        var success = _labelToEnum.TryGetValue(str, out var value);

        if (!success)
            throw new InvalidCastException($"Received enum value '{str}' from database which wasn't found on enum {typeof(TEnum)}");

        return value;
    }

    protected override void WriteCore(PgWriter writer, TEnum value)
    {
        if (!_enumToLabel.TryGetValue(value, out var str))
            throw new InvalidCastException($"Can't write value {value} as enum {typeof(TEnum)}");

        writer.WriteBytes(new ReadOnlySpan<byte>(_encoding.GetBytes(str)));
    }
}
