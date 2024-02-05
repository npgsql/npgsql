using System;
using System.Collections;
using System.Collections.Generic;

namespace Npgsql.Internal;

struct PgTypeInfoResolverChainBuilder
{
    readonly List<PgTypeInfoResolverFactory> _factories = new();
    Action<PgTypeInfoResolverChainBuilder, List<IPgTypeInfoResolver>>? _addRangeResolvers;
    Action<PgTypeInfoResolverChainBuilder, List<IPgTypeInfoResolver>>? _addMultirangeResolvers;
    RangeArrayHandler _rangeArrayHandler = RangeArrayHandler.Instance;
    MultirangeArrayHandler _multirangeArrayHandler = MultirangeArrayHandler.Instance;
    Action<PgTypeInfoResolverChainBuilder, List<IPgTypeInfoResolver>>? _addArrayResolvers;

    public PgTypeInfoResolverChainBuilder()
    {
    }

    public void Clear() => _factories.Clear();

    public void AppendResolverFactory(PgTypeInfoResolverFactory factory) => AddResolverFactory(factory);
    public void PrependResolverFactory(PgTypeInfoResolverFactory factory) => AddResolverFactory(factory, prepend: true);

    void AddResolverFactory(PgTypeInfoResolverFactory factory, bool prepend = false)
    {
        var type = factory.GetType();

        for (var i = 0; i < _factories.Count; i++)
            if (_factories[i].GetType() == type)
            {
                _factories.RemoveAt(i);
                break;
            }

        if (prepend)
            _factories.Insert(0, factory);
        else
            _factories.Add(factory);
    }

    public void EnableRanges()
    {
        _addRangeResolvers ??= AddResolvers;
        _rangeArrayHandler = RangeArrayHandlerImpl.Instance;

        static void AddResolvers(PgTypeInfoResolverChainBuilder instance, List<IPgTypeInfoResolver> resolvers)
        {
            foreach (var factory in instance._factories)
                if (factory.CreateRangeResolver() is { } resolver)
                    resolvers.Add(resolver);
        }
    }

    public void EnableMultiranges()
    {
        _addMultirangeResolvers ??= AddResolvers;
        _multirangeArrayHandler = MultirangeArrayHandlerImpl.Instance;

        static void AddResolvers(PgTypeInfoResolverChainBuilder instance, List<IPgTypeInfoResolver> resolvers)
        {
            foreach (var factory in instance._factories)
                if (factory.CreateMultirangeResolver() is { } resolver)
                    resolvers.Add(resolver);
        }
    }

    public void EnableArrays()
    {
        _addArrayResolvers ??= AddResolvers;

        static void AddResolvers(PgTypeInfoResolverChainBuilder instance, List<IPgTypeInfoResolver> resolvers)
        {
            foreach (var factory in instance._factories)
                if (factory.CreateArrayResolver() is { } resolver)
                    resolvers.Add(resolver);

            if (instance._addRangeResolvers is not null)
                foreach (var factory in instance._factories)
                    if (instance._rangeArrayHandler.CreateRangeArrayResolver(factory) is { } resolver)
                        resolvers.Add(resolver);

            if (instance._addMultirangeResolvers is not null)
                foreach (var factory in instance._factories)
                    if (instance._multirangeArrayHandler.CreateMultirangeArrayResolver(factory) is { } resolver)
                        resolvers.Add(resolver);
        }
    }

    public PgTypeInfoResolverChain Build(Action<List<IPgTypeInfoResolver>>? configure = null)
    {
        var resolvers = new List<IPgTypeInfoResolver>();
        foreach (var factory in _factories)
            resolvers.Add(factory.CreateResolver());
        var instance = this;
        _addRangeResolvers?.Invoke(instance, resolvers);
        _addMultirangeResolvers?.Invoke(instance, resolvers);
        _addArrayResolvers?.Invoke(instance, resolvers);
        configure?.Invoke(resolvers);
        return new(
            resolvers,
            rangesEnabled: _addRangeResolvers is not null,
            multirangesEnabled: _addMultirangeResolvers is not null,
            arraysEnabled: _addArrayResolvers is not null
        );
    }

    class RangeArrayHandler
    {
        public static RangeArrayHandler Instance { get; } = new();

        public virtual IPgTypeInfoResolver? CreateRangeArrayResolver(PgTypeInfoResolverFactory factory) => null;
    }

    sealed class RangeArrayHandlerImpl : RangeArrayHandler
    {
        public new static RangeArrayHandlerImpl Instance { get; } = new();

        public override IPgTypeInfoResolver? CreateRangeArrayResolver(PgTypeInfoResolverFactory factory) => factory.CreateRangeArrayResolver();
    }

    class MultirangeArrayHandler
    {
        public static MultirangeArrayHandler Instance { get; } = new();

        public virtual IPgTypeInfoResolver? CreateMultirangeArrayResolver(PgTypeInfoResolverFactory factory) => null;
    }

    sealed class MultirangeArrayHandlerImpl : MultirangeArrayHandler
    {
        public new static MultirangeArrayHandlerImpl Instance { get; } = new();

        public override IPgTypeInfoResolver? CreateMultirangeArrayResolver(PgTypeInfoResolverFactory factory) => factory.CreateMultirangeArrayResolver();
    }
}

readonly struct PgTypeInfoResolverChain : IEnumerable<IPgTypeInfoResolver>
{
    [Flags]
    enum EnabledFlags
    {
        None = 0,
        Ranges = 1,
        Multiranges = 2,
        Arrays = 4
    }

    readonly EnabledFlags _enabled;
    readonly List<IPgTypeInfoResolver> _resolvers;

    public PgTypeInfoResolverChain(List<IPgTypeInfoResolver> resolvers, bool rangesEnabled, bool multirangesEnabled, bool arraysEnabled)
    {
        _enabled = rangesEnabled ? EnabledFlags.Ranges | _enabled : _enabled;
        _enabled = multirangesEnabled ? EnabledFlags.Multiranges | _enabled : _enabled;
        _enabled = arraysEnabled ? EnabledFlags.Arrays | _enabled : _enabled;
        _resolvers = resolvers;
    }

    public bool RangesEnabled => _enabled.HasFlag(EnabledFlags.Ranges);
    public bool MultirangesEnabled => _enabled.HasFlag(EnabledFlags.Multiranges);
    public bool ArraysEnabled => _enabled.HasFlag(EnabledFlags.Arrays);

    public IEnumerator<IPgTypeInfoResolver> GetEnumerator()
        => _resolvers?.GetEnumerator() ?? (IEnumerator<IPgTypeInfoResolver>)Array.Empty<IPgTypeInfoResolver>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
        => _resolvers?.GetEnumerator() ?? Array.Empty<IPgTypeInfoResolver>().GetEnumerator();
}
