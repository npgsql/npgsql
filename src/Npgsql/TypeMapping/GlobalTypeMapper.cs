using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Util;

namespace Npgsql.TypeMapping;

/// <inheritdoc />
public sealed class GlobalTypeMapper : INpgsqlTypeMapper
{
    PgSerializerOptions MappingSerializerOptions { get; } = new(new PostgresMinimalDatabaseInfo())
    {
        // This means we don't ever have a missing oid for a datatypename as our canonical format is datatypenames.
        PortableTypeIds = true,
        // Irrelevant but required.
        TextEncoding = PGUtil.UTF8Encoding,
        TypeInfoResolver = AdoTypeInfoResolver.Instance
    };

    // TODO how to deal with NpgsqlRange and Array/List? pattern matching the type?
    // Otherwise we need to load more types/infos by default again...
    internal DataTypeName? TryGetDataTypeName(Type type)
    {
        if (MappingSerializerOptions.GetTypeInfo(type) is { } info)
            return info.GetResolutionOrThrow().PgTypeId.DataTypeName;

        return null;
    }

    internal static GlobalTypeMapper Instance { get; }
    internal UserTypeMapper UserTypeMapper { get; } = new();
    internal List<IPgTypeInfoResolver> TypeInfoResolverChain { get; } = new();

    internal ReaderWriterLockSlim Lock { get; }
        = new(LockRecursionPolicy.SupportsRecursion);

    static GlobalTypeMapper()
        => Instance = new GlobalTypeMapper();

    GlobalTypeMapper()
        => Reset();

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
