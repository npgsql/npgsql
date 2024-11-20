using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class Int4Converter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadInt32());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteInt32(int.CreateChecked(value));
}
