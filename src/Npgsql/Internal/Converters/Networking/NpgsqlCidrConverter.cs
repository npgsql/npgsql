using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class NpgsqlCidrConverter : PgBufferedConverter<NpgsqlCidr>
{
    public override Size GetSize(SizeContext context, NpgsqlCidr value, ref object? writeState)
        => NpgsqlInetConverter.DoGetSize(context, value.Address, ref writeState);

    protected override NpgsqlCidr ReadCore(PgReader reader)
    {
        var (ip, netmask) = NpgsqlInetConverter.DoReadCore(reader, shouldBeCidr: true);
        return new(ip, netmask);
    }

    protected override void WriteCore(PgWriter writer, NpgsqlCidr value)
        => NpgsqlInetConverter.DoWriteCore(writer, (value.Address, value.Netmask), isCidr: false);
}
