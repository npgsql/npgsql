using System;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DoubleConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double));
        return format is DataFormat.Binary;
    }

    public override T Read(PgReader reader) => T.CreateChecked(reader.ReadDouble());
    public override void Write(PgWriter writer, T value) => writer.WriteDouble(double.CreateChecked(value));
}
