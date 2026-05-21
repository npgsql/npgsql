using System;
using System.Numerics;

namespace Npgsql.Internal.Converters;

sealed class MoneyConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    public override T Read(PgReader reader) => ConvertTo(new PgMoney(reader.ReadInt64()));
    public override void Write(PgWriter writer, T value) => writer.WriteInt64(ConvertFrom(value).GetValue());

    static PgMoney ConvertFrom(T value) => new(decimal.CreateChecked(value));
    static T ConvertTo(PgMoney money) => T.CreateChecked(money.ToDecimal());
}
