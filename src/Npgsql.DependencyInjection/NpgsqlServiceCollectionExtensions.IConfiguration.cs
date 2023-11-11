using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension method for setting up Npgsql services in an <see cref="IServiceCollection" />.
/// </summary>
public static partial class NpgsqlServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="NpgsqlDataSourceBuilder" /> for further customizations of the <see cref="NpgsqlDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlDataSourceCore(serviceCollection, configuration, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlDataSourceCore(
            serviceCollection, configuration, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="NpgsqlSlimDataSourceBuilder" /> for further customizations of the <see cref="NpgsqlDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlSlimDataSourceCore(serviceCollection, configuration, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlSlimDataSourceCore(
            serviceCollection, configuration, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="NpgsqlDataSourceBuilder" /> for further customizations of the <see cref="NpgsqlDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, configuration, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, configuration, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="NpgsqlDataSourceBuilder" /> for further customizations of the <see cref="NpgsqlDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, configuration, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, configuration, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    static IServiceCollection AddNpgsqlDataSourceCore(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                sp =>
                {
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder();
                    configuration?.Bind(dataSourceBuilder.ConnectionStringBuilder);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    static IServiceCollection AddNpgsqlSlimDataSourceCore(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlSlimDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                sp =>
                {
                    var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder();
                    configuration?.Bind(dataSourceBuilder.ConnectionStringBuilder);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    static IServiceCollection AddMultiHostNpgsqlDataSourceCore(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlMultiHostDataSource),
                sp =>
                {
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder();
                    configuration?.Bind(dataSourceBuilder.ConnectionStringBuilder);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.BuildMultiHost();
                },
                dataSourceLifetime));

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                sp => sp.GetRequiredService<NpgsqlMultiHostDataSource>(),
                dataSourceLifetime));

        AddCommonServices(serviceCollection, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    [RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
    [RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    static IServiceCollection AddMultiHostNpgsqlSlimDataSourceCore(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<NpgsqlSlimDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlMultiHostDataSource),
                sp =>
                {
                    var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder();
                    configuration?.Bind(dataSourceBuilder.ConnectionStringBuilder);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.BuildMultiHost();
                },
                dataSourceLifetime));

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                sp => sp.GetRequiredService<NpgsqlMultiHostDataSource>(),
                dataSourceLifetime));

        AddCommonServices(serviceCollection, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }
}
