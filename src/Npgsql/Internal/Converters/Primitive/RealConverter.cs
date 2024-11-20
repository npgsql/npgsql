using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class RealConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(float));
        return format is DataFormat.Binary;
    }

    protected override T ReadCore(PgReader reader) => T.CreateChecked(reader.ReadFloat());
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteFloat(float.CreateChecked(value));
}
