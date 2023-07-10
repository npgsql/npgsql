using System;
using System.Numerics;
using Npgsql.Internal.Converters.Types;

namespace Npgsql.Internal.Converters;

sealed class MoneyConverter<T> : PgBufferedConverter<T>
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }
    protected override T ReadCore(PgReader reader) => ConvertTo(new PgMoney(reader.ReadInt64()));
    protected override void WriteCore(PgWriter writer, T value) => writer.WriteInt64(ConvertFrom(value).GetValue());

    static PgMoney ConvertFrom(T value)
    {
#if !NET7_0_OR_GREATER
        if (typeof(short) == typeof(T))
            return new PgMoney((decimal)(short)(object)value!);
        if (typeof(int) == typeof(T))
            return new PgMoney((decimal)(int)(object)value!);
        if (typeof(long) == typeof(T))
            return new PgMoney((decimal)(long)(object)value!);

        if (typeof(byte) == typeof(T))
            return new PgMoney((decimal)(byte)(object)value!);
        if (typeof(sbyte) == typeof(T))
            return new PgMoney((decimal)(sbyte)(object)value!);

        if (typeof(float) == typeof(T))
            return new PgMoney((decimal)(float)(object)value!);
        if (typeof(double) == typeof(T))
            return new PgMoney((decimal)(double)(object)value!);
        if (typeof(decimal) == typeof(T))
            return new PgMoney((decimal)(object)value!);

        throw new NotSupportedException();
#else
        return new PgMoney(decimal.CreateChecked(value));
#endif
    }

    static T ConvertTo(PgMoney money)
    {
#if !NET7_0_OR_GREATER
        if (typeof(short) == typeof(T))
            return (T)(object)(short)money.ToDecimal();
        if (typeof(int) == typeof(T))
            return (T)(object)(int)money.ToDecimal();
        if (typeof(long) == typeof(T))
            return (T)(object)(long)money.ToDecimal();

        if (typeof(byte) == typeof(T))
            return (T)(object)(byte)money.ToDecimal();
        if (typeof(sbyte) == typeof(T))
            return (T)(object)(sbyte)money.ToDecimal();

        if (typeof(float) == typeof(T))
            return (T)(object)(float)money.ToDecimal();
        if (typeof(double) == typeof(T))
            return (T)(object)(double)money.ToDecimal();
        if (typeof(decimal) == typeof(T))
            return (T)(object)money.ToDecimal();

        throw new NotSupportedException();
#else
        return T.CreateChecked(money.ToDecimal());
#endif
    }
}
