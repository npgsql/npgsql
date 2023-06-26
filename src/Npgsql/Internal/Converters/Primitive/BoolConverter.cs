using System.Runtime.CompilerServices;

namespace Npgsql.Internal.Converters;

sealed class BoolConverter : PgBufferedConverter<bool>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, bool value, ref object? writeState) => sizeof(byte);

    protected override bool ReadCore(PgReader reader) => reader.ReadByte() != 0;
    protected override void WriteCore(PgWriter writer, bool value) => writer.WriteByte(Unsafe.As<bool, byte>(ref value));
}
