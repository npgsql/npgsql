using System.Diagnostics;
using System.Net.NetworkInformation;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class MacaddrConverter : PgBufferedConverter<PhysicalAddress>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => CanConvertBufferedDefault(format, out bufferRequirements);

    public override Size GetSize(SizeContext context, PhysicalAddress value, ref object? writeState)
        => value.GetAddressBytes().Length;

    protected override PhysicalAddress ReadCore(PgReader reader)
    {
        var len = reader.CurrentSize;
        Debug.Assert(len is 6 or 8);

        var bytes = new byte[len];
        reader.CopyTo(bytes);
        return new PhysicalAddress(bytes);
    }

    protected override void WriteCore(PgWriter writer, PhysicalAddress value)
    {
        var bytes = value.GetAddressBytes();
        writer.WriteRaw(bytes);
    }
}
