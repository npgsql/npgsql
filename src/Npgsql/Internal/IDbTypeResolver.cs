using System.Data;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

/// <summary>
/// An Npgsql resolver for DbType. Used by Npgsql map DbType and DataTypeName.
/// </summary>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public interface IDbTypeResolver
{
    string? GetDataTypeName(DbType dbType, PgSerializerOptions options);
    DbType? GetDbType(DataTypeName dataTypeName, PgSerializerOptions options);
}
