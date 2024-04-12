using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal;

/// <summary>
/// Hacky temporary measure used by EFCore.PG to extract user-configured enum mappings. Accessed via reflection only.
/// </summary>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public sealed class HackyEnumTypeMapping
{
    public HackyEnumTypeMapping(Type enumClrType, string pgTypeName, INpgsqlNameTranslator nameTranslator)
    {
        EnumClrType = enumClrType;
        PgTypeName = pgTypeName;
        NameTranslator = nameTranslator;
    }

    public string PgTypeName { get; }
    public Type EnumClrType { get; }
    public INpgsqlNameTranslator NameTranslator { get; }
}
