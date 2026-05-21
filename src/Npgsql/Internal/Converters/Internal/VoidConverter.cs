using System;

namespace Npgsql.Internal.Converters.Internal;

// Void is not a value so we read it as a null reference, not a DBNull.
sealed class VoidConverter : PgBufferedConverter<object?>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(0) };

    public override object? Read(PgReader reader) => null;
    public override void Write(PgWriter writer, object? value) => throw new NotSupportedException();
}
