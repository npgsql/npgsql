using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.DbTypeResolverExperimental)]
public abstract class DbTypeResolverFactory
{
    public abstract IDbTypeResolver CreateDbTypeResolver(NpgsqlDatabaseInfo databaseInfo);
}
