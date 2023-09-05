using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class MacaddrConverter : PgBufferedConverter<PhysicalAddress>
{
    readonly bool _macaddr8;

    public MacaddrConverter(bool macaddr8) => _macaddr8 = macaddr8;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = _macaddr8 ? BufferRequirements.Create(Size.CreateUpperBound(8)) : BufferRequirements.CreateFixedSize(6);
        return format is DataFormat.Binary;
    }

    public override Size GetSize(SizeContext context, PhysicalAddress value, ref object? writeState)
        => value.GetAddressBytes().Length;

    protected override PhysicalAddress ReadCore(PgReader reader)
    {
        var len = reader.CurrentRemaining;
        Debug.Assert(len is 6 or 8);

        var bytes = new byte[len];
        reader.Read(bytes);
        return new PhysicalAddress(bytes);
    }

    protected override void WriteCore(PgWriter writer, PhysicalAddress value)
    {
        var bytes = value.GetAddressBytes();
        if (!_macaddr8 && bytes.Length is not 6)
            throw new ArgumentException("A macaddr value must be 6 bytes long.");
        writer.WriteBytes(bytes);
    }
}
