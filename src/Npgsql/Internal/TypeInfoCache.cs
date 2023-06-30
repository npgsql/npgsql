using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

sealed class TypeInfoCache<TPgTypeId> where TPgTypeId : struct
{
    readonly PgSerializerOptions _options;

    // Mostly used for parameter writing, 8ns
    readonly ConcurrentDictionary<Type, PgTypeInfo> _cacheByClrType = new();

    // Used for reading, occasionally for parameter writing where a db type was given.
    // 8ns, about 10ns total to scan an array with 6, 7 different clr types under one pg type
    readonly ConcurrentDictionary<TPgTypeId, (Type? Type, PgTypeInfo Info)[]> _cacheByPgTypeId = new();

    static TypeInfoCache()
    {
        if (typeof(TPgTypeId) != typeof(Oid) && typeof(TPgTypeId) != typeof(DataTypeName))
            throw new InvalidOperationException("Cannot use this type argument.");
    }

    public TypeInfoCache(PgSerializerOptions options)
        => _options = options;

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pgTypeId"></param>
    /// <param name="defaultTypeFallback">
    /// When this flag is true, and both type and pgTypeId are non null, a default info for the pgTypeId can be returned if an exact match
    /// can't be found.
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PgTypeInfo? GetOrAddInfo(Type? type, TPgTypeId? pgTypeId, bool defaultTypeFallback = false)
    {
        if (pgTypeId is { } id)
        {
            if (_cacheByPgTypeId.TryGetValue(id, out var infos))
                if (FindMatch(type, infos, defaultTypeFallback) is { } info)
                    return info;

            return AddEntryById(id, infos, defaultTypeFallback);
        }

        if (type is not null)
            // No GetOrAdd as we don't want to cache potential nulls.
            return _cacheByClrType.TryGetValue(type, out var info) ? info : AddByType(type);

        return null;

        PgTypeInfo? FindMatch(Type? type, (Type? Type, PgTypeInfo Info)[] infos, bool defaultTypeFallback)
        {
            PgTypeInfo? defaultInfo = null;
            for (var i = 0; i < infos.Length; i++)
            {
                ref var item = ref infos[i];
                if (item.Type == type)
                    return item.Info;

                if (defaultTypeFallback && item.Type is null)
                    defaultInfo = item.Info;
            }

            return defaultInfo;
        }

        PgTypeInfo? AddByType(Type type)
        {
            // We don't pass PgTypeId as we're interested in default converters here.
            var info = CreateInfo(type, null, _options, defaultTypeFallback: false);

            return info is null
                ? null
                : _cacheByClrType.TryAdd(type, info) // We never remove entries so either of these branches will always succeed.
                    ? info
                    : _cacheByClrType[type];
        }

        PgTypeInfo? AddEntryById(TPgTypeId pgTypeId, (Type? Type, PgTypeInfo Info)[]? infos, bool defaultTypeFallback)
        {
            var info = CreateInfo(type, pgTypeId, _options, defaultTypeFallback);
            if (info is null)
                return null;

            if (infos is null && _cacheByPgTypeId.TryAdd(pgTypeId, new[] { (type, info) }))
                return info;

            infos ??= _cacheByPgTypeId[pgTypeId];
            while (true)
            {
                if (FindMatch(type, infos, defaultTypeFallback) is { } newInfo)
                    return newInfo;

                var oldLength = infos.Length;
                var oldInfos = infos;
                if (type is null)
                {
                    // Also add it by its info type to save a future resolver lookup + resize.
                    Array.Resize(ref infos, infos.Length + 2);
                    infos[oldLength] = (type, info);
                    infos[oldLength + 1] = (info.Type, info);
                }
                else
                {
                    Array.Resize(ref infos, infos.Length + 1);
                    infos[oldLength] = (type, info);
                }

                if (!_cacheByPgTypeId.TryUpdate(pgTypeId, infos, oldInfos))
                    infos = _cacheByPgTypeId[pgTypeId];
                else
                    return info;
            }
        }

        static PgTypeInfo? CreateInfo(Type? type, TPgTypeId? typeId, PgSerializerOptions options, bool defaultTypeFallback)
        {
            var pgTypeId = AsPgTypeId(typeId);
            // Validate that we only pass data types that are supported by the backend.
            var dataTypeName = pgTypeId is { } id ? (DataTypeName?)options.TypeCatalog.GetDataTypeName(id, validate: true) : null;
            var info = options.TypeInfoResolver.GetTypeInfo(type, dataTypeName, options);
            if (info is null && defaultTypeFallback)
            {
                type = null;
                info = options.TypeInfoResolver.GetTypeInfo(type, dataTypeName, options);
            }

            if (info is null)
                return null;

            if (pgTypeId is not null && info.PgTypeId != pgTypeId)
                throw new InvalidOperationException("A Postgres type was passed but the resolved PgTypeInfo does not have an equal PgTypeId.");

            if (type is not null && !info.IsBoxing && info.Type != type)
                throw new InvalidOperationException("A CLR type was passed but the resolved PgTypeInfo does not have an equal Type.");

            return info;
        }

        static PgTypeId? AsPgTypeId(TPgTypeId? pgTypeId)
            => pgTypeId switch
            {
                { } id when typeof(TPgTypeId) == typeof(DataTypeName) => new PgTypeId(Unsafe.As<TPgTypeId, DataTypeName>(ref id)),
                { } id => new PgTypeId(Unsafe.As<TPgTypeId, Oid>(ref id)),
                null => null
            };
    }
}
