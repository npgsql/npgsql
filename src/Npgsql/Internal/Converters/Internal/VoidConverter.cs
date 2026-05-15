using System;

namespace Npgsql.Internal.Converters.Internal;

// Void is not a value so we read it as a null reference, not a DBNull.
sealed class VoidConverter : PgBufferedConverter<object?>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(0);
        return true;
    }

    public override object? Read(PgReader reader) => null;
    public override void Write(PgWriter writer, object? value) => throw new NotSupportedException();
}
