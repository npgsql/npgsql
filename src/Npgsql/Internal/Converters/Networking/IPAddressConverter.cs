using System.Net;
using System.Net.Sockets;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class IPAddressConverter : PgBufferedConverter<IPAddress>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => NpgsqlInetConverter.CanConvertImpl(format, out bufferRequirements);

    protected override Size BindValue(in BindContext context, IPAddress value, ref object? writeState)
        => NpgsqlInetConverter.BindValueImpl(context, value, ref writeState);

    public override IPAddress Read(PgReader reader)
        => NpgsqlInetConverter.ReadImpl(reader, shouldBeCidr: false).Address;

    public override void Write(PgWriter writer, IPAddress value)
        => NpgsqlInetConverter.WriteImpl(
            writer,
            (value, (byte)(value.AddressFamily == AddressFamily.InterNetwork ? 32 : 128)),
            isCidr: false);
}
