using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Npgsql.Internal.Converters;

sealed class MacaddrConverter : PgBufferedConverter<PhysicalAddress>
{
    public override Size GetSize(SizeContext context, PhysicalAddress value, ref object? writeState)
        => value.GetAddressBytes().Length;

    protected override PhysicalAddress ReadCore(PgReader reader)
    {
        var len = reader.Current.Size.Value;
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
