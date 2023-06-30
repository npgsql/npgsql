using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

/// <inheritdoc />
public sealed class GlobalTypeMapper : INpgsqlTypeMapper
{
    PgSerializerOptions MappingSerializerOptions { get; }

    GlobalTypeMapper()
    {
        var typeCatalog = new PostgresMinimalDatabaseInfo();
        typeCatalog.ProcessTypes();
        MappingSerializerOptions = new(typeCatalog)
        {
            // This means we don't ever have a missing oid for a datatypename as our canonical format is datatypenames.
            PortableTypeIds = true,
            // Irrelevant but required.
            TextEncoding = PGUtil.UTF8Encoding,
            TypeInfoResolver = AdoTypeInfoResolver.Instance
        };
        Reset();
    }

    // We only load the base types and we do some static pattern matching to figure out arrays and well known ranges.
    internal DataTypeName? TryGetDataTypeName(Type type, object value)
    {
        var isArray = false;
        if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
        {
            // Special case char[] to skip decomposition, instead map it to text.
            if (type.GetElementType() != typeof(char))
            {
                isArray = true;
                type = type.GetElementType() ?? type.GetGenericArguments()[0];
            }
        }

        var isRange = false;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
        {
            isRange = true;
            type = type.GetGenericArguments()[0];
        }

        var typeInfo = MappingSerializerOptions.GetTypeInfo(type);
        DataTypeName? dataTypeName;
        if (typeInfo is PgResolverTypeInfo info)
            dataTypeName = info.GetResolutionAsObject(value, null).PgTypeId.DataTypeName;
        else
            dataTypeName = typeInfo?.GetConcreteResolution().PgTypeId.DataTypeName;

        if (dataTypeName is { } name)
        {
            // If we're both range and array we're actually a multirange.
            if (isRange)
            {
                dataTypeName = DataTypeNames.TryGetRangeName(name);
                if (isArray)
                    dataTypeName = dataTypeName?.ToDefaultMultirangeName();
            }
            else if (isArray)
                dataTypeName = name.ToArrayName();
        }

        return dataTypeName;
    }

    internal static GlobalTypeMapper Instance { get; }
    internal UserTypeMapper UserTypeMapper { get; } = new();
    internal List<IPgTypeInfoResolver> TypeInfoResolverChain { get; } = new();

    internal ReaderWriterLockSlim Lock { get; }
        = new(LockRecursionPolicy.SupportsRecursion);

    static GlobalTypeMapper()
        => Instance = new GlobalTypeMapper();

    /// <summary>
    /// Adds a type info resolver which can add or modify support for PostgreSQL types.
    /// Typically used by plugins.
    /// </summary>
    /// <param name="resolver">The type resolver to be added.</param>
    public void AddTypeInfoResolver(IPgTypeInfoResolver resolver)
    {
        Lock.EnterWriteLock();
        try
        {
            var type = resolver.GetType();

            // Since EFCore.PG plugins (and possibly other users) repeatedly call NpgsqlConnection.GlobalTypeMapper.UseNodaTime,
            // we replace an existing resolver of the same CLR type.
            if (TypeInfoResolverChain[0].GetType() == type)
                TypeInfoResolverChain[0] = resolver;
            for (var i = 0; i < TypeInfoResolverChain.Count; i++)
            {
                if (TypeInfoResolverChain[i].GetType() == type)
                {
                    TypeInfoResolverChain.RemoveAt(i);
                    break;
                }
            }

            TypeInfoResolverChain.Insert(0, resolver);
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        Lock.EnterWriteLock();
        try
        {
            TypeInfoResolverChain.Clear();
            TypeInfoResolverChain.Add(new AdoWithArrayTypeInfoResolver());

            UserTypeMapper.Items.Clear();
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public INpgsqlNameTranslator DefaultNameTranslator
    {
        get => UserTypeMapper.DefaultNameTranslator;
        set => UserTypeMapper.DefaultNameTranslator = value;
    }

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
    {
        Lock.EnterWriteLock();
        try
        {
            UserTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);
            return this;
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
    {
        Lock.EnterWriteLock();
        try
        {
            return UserTypeMapper.UnmapEnum<TEnum>(pgName, nameTranslator);
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        Lock.EnterWriteLock();
        try
        {
            UserTypeMapper.MapComposite<T>(pgName, nameTranslator);
            return this;
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        Lock.EnterWriteLock();
        try
        {
            return UserTypeMapper.UnmapComposite<T>(pgName, nameTranslator);
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        Lock.EnterWriteLock();
        try
        {
            UserTypeMapper.MapComposite(clrType, pgName, nameTranslator);
            return this;
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        Lock.EnterWriteLock();
        try
        {
            return UserTypeMapper.UnmapComposite(clrType, pgName, nameTranslator);
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }
}
