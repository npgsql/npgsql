using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class InternalCharConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(byte));
        return format is DataFormat.Binary;
    }

    public override T Read(PgReader reader) => T.CreateChecked(reader.ReadByte());
    public override void Write(PgWriter writer, T value) => writer.WriteByte(byte.CreateChecked(value));
}
