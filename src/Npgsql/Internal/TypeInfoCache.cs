using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

sealed class TypeInfoCache<TPgTypeId> where TPgTypeId : struct
{
    readonly PgSerializerOptions _options;

    // 8ns
    readonly ConcurrentDictionary<Type, PgTypeInfo> _cacheByClrType = new(); // most used for parameter writing
    // 8ns, about 10ns total to scan an array with 6, 7 different clr types under one pg type
    readonly ConcurrentDictionary<TPgTypeId, PgTypeInfo[]> _cacheByPgTypeId = new(); // Used for reading, occasionally for parameter writing where a db type was given.

    public TypeInfoCache(PgSerializerOptions options)
    {
        _options = options;

        if (typeof(TPgTypeId) != typeof(Oid) && typeof(TPgTypeId) != typeof(DataTypeName))
            throw new InvalidOperationException("Cannot use this type argument.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pgTypeId"></param>
    /// <param name="defaultTypeFallback">When this flag is true, and both type and pgTypeId are non null, a default info for the pgTypeId can be returned if an exact match can't be found.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PgTypeInfo? GetOrAddInfo(Type? type, TPgTypeId? pgTypeId, bool defaultTypeFallback = false)
    {
        if (pgTypeId is null && type is not null)
        {
            // No GetOrAdd as we don't want to cache potential nulls.
            return _cacheByClrType.TryGetValue(type, out var info) ? info : AddByType(type);
        }

        if (pgTypeId is not { } id)
            return null;

        if (_cacheByPgTypeId.TryGetValue(id, out var infos))
        {
            PgTypeInfo? defaultInfo = null;
            foreach (var cachedInfo in infos)
            {
                if (cachedInfo.Type == type)
                    return cachedInfo;

                if (cachedInfo.IsDefault)
                {
                    if (type is null)
                        return cachedInfo;
                    defaultInfo = cachedInfo;
                }
            }
            return defaultTypeFallback && defaultInfo is not null ? defaultInfo : AddEntryById(id, infos, defaultTypeFallback);
        }

        return AddEntryById(id, infos, defaultTypeFallback);

        PgTypeInfo? AddByType(Type type)
        {
            // We don't pass PgTypeId as we're interested in default converters here.
            var info = CreateInfo(type, null, _options, defaultTypeFallback: false);
            if (info is null)
                return null;

            // We never remove entries so either of these branches will always succeed.
            return _cacheByClrType.TryAdd(type, info) ? info : _cacheByClrType[type];
        }

        PgTypeInfo? AddEntryById(TPgTypeId pgTypeId, PgTypeInfo[]? infos, bool defaultTypeFallback)
        {
            var info = CreateInfo(type, pgTypeId, _options, defaultTypeFallback);
            if (info is null)
                return null;

            if (infos is null && _cacheByPgTypeId.TryAdd(pgTypeId, new[] { info }))
                return info;

            infos ??= _cacheByPgTypeId[pgTypeId];
            while (true)
            {
                foreach (var cachedInfo in infos)
                    if (type is null && cachedInfo.IsDefault || cachedInfo.Type == type)
                        return cachedInfo;

                var oldLength = infos.Length;
                var oldInfos = infos;
                Array.Resize(ref infos, infos.Length + 1);
                infos[oldLength] = info;
                if (!_cacheByPgTypeId.TryUpdate(pgTypeId, infos, oldInfos))
                    infos = _cacheByPgTypeId[pgTypeId];
                else
                    return info;
            }
        }

        static PgTypeInfo? CreateInfo(Type? type, TPgTypeId? pgtypeid, PgSerializerOptions options, bool defaultTypeFallback)
        {
            var typeId = AsPgTypeId(pgtypeid);
            var dataTypeName = typeId is { } id ? (DataTypeName?)options.TypeCatalog.GetDataTypeName(id, validate: true) : null;
            var info = options.TypeInfoResolver.GetTypeInfo(type, dataTypeName, options);
            if (info is null && defaultTypeFallback)
            {
                type = null;
                info = options.TypeInfoResolver.GetTypeInfo(type, dataTypeName, options);
            }

            if (info is null)
                return null;

            if (typeId is not null)
            {
                if (info.PgTypeId != typeId)
                    throw new InvalidOperationException("A Postgres type was passed but the resolved PgTypeInfo does not have an equal PgTypeId.");

                if (type is null && !info.IsDefault)
                    throw new InvalidOperationException("No CLR type was passed but the resolved PgTypeInfo does not have IsDefault set to true.");
            }

            if (type is not null)
            {
                if (info.Type != type)
                    throw new InvalidOperationException("A CLR type was passed but the resolved PgTypeInfo does not have an equal Type.");

                if (typeId is null && !info.IsDefault)
                    throw new InvalidOperationException("No Postgres type was passed but the resolved PgTypeInfo does not have IsDefault set to true.");
            }

            return info;
        }

        static PgTypeId? AsPgTypeId(TPgTypeId? pgTypeId)
        {
            if (pgTypeId is { } id)
                return (typeof(TPgTypeId) == typeof(DataTypeName)) switch
                {
                    true => new PgTypeId(Unsafe.As<TPgTypeId, DataTypeName>(ref id)),
                    false => new PgTypeId(Unsafe.As<TPgTypeId, Oid>(ref id))
                };

            return null;
        }
    }
}
