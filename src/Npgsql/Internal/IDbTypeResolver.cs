using System.Data;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

/// <summary>
/// An Npgsql resolver for DbType. Used by Npgsql to map DbType to DataTypeName and back.
/// </summary>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public interface IDbTypeResolver
{
    string? GetDataTypeName(DbType dbType, PgSerializerOptions options);
    DbType? GetDbType(DataTypeName dataTypeName, PgSerializerOptions options);
}
