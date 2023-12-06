using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal;


/// <summary>
/// Hacky temporary measure used by EFCore.PG to extract user-configured enum mappings. Accessed via reflection only.
/// </summary>
public sealed class HackyEnumTypeMapping(Type enumClrType, string pgTypeName, INpgsqlNameTranslator nameTranslator)
{
    public string PgTypeName { get; } = pgTypeName;
    public Type EnumClrType { get; } = enumClrType;
    public INpgsqlNameTranslator NameTranslator { get; } = nameTranslator;
}
