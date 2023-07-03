using System;

namespace Npgsql.Internal.Converters.Internal;

// Void is not a value so we read it as a null reference, not a DBNull.
sealed class VoidConverter : PgBufferedConverter<object?>
{
    public override Size GetSize(SizeContext context, object value, ref object? writeState) => throw new NotImplementedException();
    protected override void WriteCore(PgWriter writer, object? value) => throw new NotImplementedException();

    protected override object? ReadCore(PgReader reader) => null;
}
