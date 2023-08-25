using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class TypeInfoCache<TPgTypeId> where TPgTypeId : struct
{
    readonly PgSerializerOptions _options;
    readonly bool _validatePgTypeIds;

    // Mostly used for parameter writing, 8ns
    readonly ConcurrentDictionary<Type, PgTypeInfo?> _cacheByClrType = new();

    // Used for reading, occasionally for parameter writing where a db type was given.
    // 8ns, about 10ns total to scan an array with 6, 7 different clr types under one pg type
    readonly ConcurrentDictionary<TPgTypeId, (Type? Type, PgTypeInfo? Info)[]> _cacheByPgTypeId = new();

    static TypeInfoCache()
    {
        if (typeof(TPgTypeId) != typeof(Oid) && typeof(TPgTypeId) != typeof(DataTypeName))
            throw new InvalidOperationException("Cannot use this type argument.");
    }

    public TypeInfoCache(PgSerializerOptions options, bool validatePgTypeIds = true)
    {
        _options = options;
        _validatePgTypeIds = validatePgTypeIds;
    }

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
            return _cacheByClrType.TryGetValue(type, out var info) ? info : AddByType(type);

        return null;

        PgTypeInfo? FindMatch(Type? type, (Type? Type, PgTypeInfo? Info)[] infos, bool defaultTypeFallback)
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
            var info = CreateInfo(type, null, _options, defaultTypeFallback: false, _validatePgTypeIds);

            return info is null
                ? null
                : _cacheByClrType.TryAdd(type, info) // We never remove entries so either of these branches will always succeed.
                    ? info
                    : _cacheByClrType[type];
        }

        PgTypeInfo? AddEntryById(TPgTypeId pgTypeId, (Type? Type, PgTypeInfo? Info)[]? infos, bool defaultTypeFallback)
        {
            // We cache negatives (null info) to allow 'object or default' checks to never hit the resolvers after the first lookup.
            var info = CreateInfo(type, pgTypeId, _options, defaultTypeFallback, _validatePgTypeIds);

            var isDefaultInfo = type is null && info is not null;
            if (infos is null)
            {
                // Also add defaults by their info type to save a future resolver lookup + resize.
                infos = isDefaultInfo
                    ? new [] { (type, info), (info!.Type, info) }
                    : new [] { (type, info) };

                if (_cacheByPgTypeId.TryAdd(pgTypeId, infos))
                    return info;
            }

            // We have to update it instead.
            while (true)
            {
                infos = _cacheByPgTypeId[pgTypeId];
                if (FindMatch(type, infos, defaultTypeFallback) is { } racedInfo)
                    return racedInfo;

                // Also add defaults by their info type to save a future resolver lookup + resize.
                var oldInfos = infos;
                Array.Resize(ref infos, oldInfos.Length + (isDefaultInfo ? 2 : 1));
                infos[oldInfos.Length] = (type, info);
                if (isDefaultInfo)
                    infos[oldInfos.Length + 1] = (info!.Type, info);

                if (_cacheByPgTypeId.TryUpdate(pgTypeId, infos, oldInfos))
                    return info;
            }
        }

        static PgTypeInfo? CreateInfo(Type? type, TPgTypeId? typeId, PgSerializerOptions options, bool defaultTypeFallback, bool validatePgTypeIds)
        {
            var pgTypeId = AsPgTypeId(typeId);
            // Validate that we only pass data types that are supported by the backend.
            var dataTypeName = pgTypeId is { } id ? (DataTypeName?)options.TypeCatalog.GetDataTypeName(id, validate: validatePgTypeIds) : null;
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
                throw new InvalidOperationException($"A CLR type '{type}' was passed but the resolved PgTypeInfo does not have an equal Type: {info.Type}.");

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
