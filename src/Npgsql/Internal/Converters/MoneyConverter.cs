using System;
using System.Numerics;

namespace Npgsql.Internal.Converters;

sealed class MoneyConverter<T> : PgBufferedConverter<T> where T : INumberBase<T>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override T ReadCore(PgReader reader) => ConvertTo(new PgMoney(reader.ReadInt64()));
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteInt64(ConvertFrom(value).GetValue());

    static PgMoney ConvertFrom(T value) => new(decimal.CreateChecked(value));
    static T ConvertTo(PgMoney money) => T.CreateChecked(money.ToDecimal());
}
