using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgTypeInfoResolverFactory
{
    public abstract IPgTypeInfoResolver CreateResolver();
    public abstract IPgTypeInfoResolver? CreateArrayResolver();

    public virtual IPgTypeInfoResolver? CreateRangeResolver() => null;
    public virtual IPgTypeInfoResolver? CreateRangeArrayResolver() => null;

    public virtual IPgTypeInfoResolver? CreateMultirangeResolver() => null;
    public virtual IPgTypeInfoResolver? CreateMultirangeArrayResolver() => null;
}
