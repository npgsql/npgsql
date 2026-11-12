using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

#pragma warning disable CS0618 // NpgsqlCidr is obsolete
sealed class NpgsqlCidrConverter : PgBufferedConverter<NpgsqlCidr>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => NpgsqlInetConverter.GetDescriptorImpl(context);

    protected override Size BindValue(in BindContext context, NpgsqlCidr value, ref object? writeState)
        => NpgsqlInetConverter.BindValueImpl(context, value.Address, ref writeState);

    public override NpgsqlCidr Read(PgReader reader)
    {
        var (ip, netmask) = NpgsqlInetConverter.ReadImpl(reader, shouldBeCidr: true);
        return new(ip, netmask);
    }

    public override void Write(PgWriter writer, NpgsqlCidr value)
        => NpgsqlInetConverter.WriteImpl(writer, (value.Address, value.Netmask), isCidr: true);
}
#pragma warning restore CS0618
