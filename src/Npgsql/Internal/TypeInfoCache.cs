using System;
using System.Collections.Concurrent;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class TypeInfoCache<TPgTypeId>(PgSerializerOptions options, bool validatePgTypeIds = true)
    where TPgTypeId : struct
{
    // Mostly used for parameter writing, 8ns
    readonly ConcurrentDictionary<Type, PgTypeInfo?> _cacheByClrType = new();

    // Used for reading, occasionally for parameter writing where a db type was given.
    // 8ns, about 10ns total to scan an array with 6, 7 different clr types under one pg type
    readonly ConcurrentDictionary<TPgTypeId, (Type? Type, PgTypeInfo Info)[]> _cacheByPgTypeId = new();

    static TypeInfoCache()
    {
        if (typeof(TPgTypeId) != typeof(Oid) && typeof(TPgTypeId) != typeof(DataTypeName))
            throw new InvalidOperationException("Cannot use this type argument.");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pgTypeId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PgTypeInfo? GetOrAddInfo(Type? type, TPgTypeId? pgTypeId)
    {
        if (pgTypeId is { } id)
        {
            if (_cacheByPgTypeId.TryGetValue(id, out var infos))
                if (FindMatch(type, infos) is { } info)
                    return info;

            return AddEntryById(type, id, infos);
        }

        if (type is not null)
            return _cacheByClrType.TryGetValue(type, out var info) ? info : AddByType(type);

        return null;

        PgTypeInfo? FindMatch(Type? type, (Type? Type, PgTypeInfo Info)[] infos)
        {
            for (var i = 0; i < infos.Length; i++)
            {
                ref var item = ref infos[i];
                if (item.Type == type)
                    return item.Info;
            }

            return null;
        }

        PgTypeInfo? AddByType(Type type)
        {
            // We don't pass PgTypeId as we're interested in default converters here.
            var info = CreateInfo(type, null, options, validatePgTypeIds);

            return info is null
                ? null
                : _cacheByClrType.TryAdd(type, info) // We never remove entries so either of these branches will always succeed.
                    ? info
                    : _cacheByClrType[type];
        }

        PgTypeInfo? AddEntryById(Type? type, TPgTypeId pgTypeId, (Type? Type, PgTypeInfo Info)[]? infos)
        {
            if (CreateInfo(type, pgTypeId, options, validatePgTypeIds) is not { } info)
                return null;

            var isDefaultInfo = type is null;
            if (infos is null)
            {
                // Also add defaults by their info type to save a future resolver lookup + resize.
                infos = isDefaultInfo
                    ? new [] { (type, info), (info.Type, info) }
                    : new [] { (type, info) };

                if (_cacheByPgTypeId.TryAdd(pgTypeId, infos))
                    return info;
            }

            // We have to update it instead.
            while (true)
            {
                infos = _cacheByPgTypeId[pgTypeId];
                if (FindMatch(type, infos) is { } racedInfo)
                    return racedInfo;

                // Also add defaults by their info type to save a future resolver lookup + resize.
                var oldInfos = infos;
                var hasExactType = false;
                if (isDefaultInfo)
                {
                    foreach (var oldInfo in oldInfos)
                        if (oldInfo.Type == info.Type)
                            hasExactType = true;
                }
                Array.Resize(ref infos, oldInfos.Length + (isDefaultInfo && !hasExactType ? 2 : 1));
                infos[oldInfos.Length] = (type, info);
                if (isDefaultInfo && !hasExactType)
                    infos[oldInfos.Length + 1] = (info.Type, info);

                if (_cacheByPgTypeId.TryUpdate(pgTypeId, infos, oldInfos))
                    return info;
            }
        }

        static PgTypeInfo? CreateInfo(Type? type, TPgTypeId? typeId, PgSerializerOptions options, bool validatePgTypeIds)
        {
            var pgTypeId = AsPgTypeId(typeId);
            // Validate that we only pass data types that are supported by the backend.
            var dataTypeName = pgTypeId is { } id ? (DataTypeName?)options.DatabaseInfo.GetDataTypeName(id, validate: validatePgTypeIds) : null;
            var info = options.TypeInfoResolver.GetTypeInfo(type, dataTypeName, options);
            if (info is null)
                return null;

            if (pgTypeId is not null && info.PgTypeId != pgTypeId)
                throw new InvalidOperationException("A Postgres type was passed but the resolved PgTypeInfo does not have an equal PgTypeId.");

            if (type is not null && info.Type != type)
            {
                // Types were not equal, throw for IsBoxing = false, otherwise we throw when the returned type isn't assignable to the requested type (after unboxing).
                if (!info.IsBoxing || !info.Type.IsAssignableTo(type))
                    throw new InvalidOperationException($"A CLR type '{type}' was passed but the resolved PgTypeInfo does not have an equal Type: {info.Type}.");
            }

            return info;
        }

        static PgTypeId? AsPgTypeId(TPgTypeId? pgTypeId)
            => pgTypeId switch
            {
                { } id when typeof(TPgTypeId) == typeof(DataTypeName) => new((DataTypeName)(object)id),
                { } id => new((Oid)(object)id),
                null => null
            };
    }
}
