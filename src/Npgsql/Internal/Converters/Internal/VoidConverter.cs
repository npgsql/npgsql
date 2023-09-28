using System;

namespace Npgsql.Internal.Converters.Internal;

// Void is not a value so we read it as a null reference, not a DBNull.
sealed class VoidConverter : PgBufferedConverter<object?>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => CanConvertBufferedDefault(DataFormat.Binary, out bufferRequirements); // Text is identical

    protected override object? ReadCore(PgReader reader) => null;
    protected override void WriteCore(PgWriter writer, object? value) => throw new NotSupportedException();
}
