using System;
using System.Data.Common;
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
    /// <param name="connectionString">An Npgsql connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddNpgsqlDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null,
            connectionLifetime, dataSourceLifetime, state: null);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddNpgsqlDataSourceCore(serviceCollection, serviceKey, connectionString,
            static (_, builder, state) => ((Action<NpgsqlDataSourceBuilder>)state!)(builder)
            , connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<IServiceProvider, NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddNpgsqlDataSourceCore(serviceCollection, serviceKey, connectionString,
            static (sp, builder, state) => ((Action<IServiceProvider, NpgsqlDataSourceBuilder>)state!)(sp, builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null,
            connectionLifetime, dataSourceLifetime, state: null);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddNpgsqlSlimDataSourceCore(serviceCollection, serviceKey, connectionString,
            static (_, builder, state) => ((Action<NpgsqlSlimDataSourceBuilder>)state!)(builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<IServiceProvider, NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddNpgsqlSlimDataSourceCore(serviceCollection, serviceKey, connectionString,
            static (sp, builder, state) => ((Action<IServiceProvider, NpgsqlSlimDataSourceBuilder>)state!)(sp, builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null,
            connectionLifetime, dataSourceLifetime, state: null);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, serviceKey, connectionString,
            static (_, builder, state) => ((Action<NpgsqlDataSourceBuilder>)state!)(builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<IServiceProvider, NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, serviceKey, connectionString,
            static (sp, builder, state) => ((Action<IServiceProvider, NpgsqlDataSourceBuilder>)state!)(sp, builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null,
            connectionLifetime, dataSourceLifetime, state: null);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString,
            static (_, builder, state) => ((Action<NpgsqlSlimDataSourceBuilder>)state!)(builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
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
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<IServiceProvider, NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString,
            static (sp, builder, state) => ((Action<IServiceProvider, NpgsqlSlimDataSourceBuilder>)state!)(sp, builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);

    static IServiceCollection AddNpgsqlDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<IServiceProvider, NpgsqlDataSourceBuilder, object?>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime,
        object? state)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                serviceKey,
                (sp, key) =>
                {
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(sp, dataSourceBuilder, state);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddNpgsqlSlimDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<IServiceProvider, NpgsqlSlimDataSourceBuilder, object?>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime,
        object? state)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                serviceKey,
                (sp, key) =>
                {
                    var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(sp, dataSourceBuilder, state);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddMultiHostNpgsqlDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<IServiceProvider, NpgsqlDataSourceBuilder, object?>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime,
        object? state)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlMultiHostDataSource),
                serviceKey,
                (sp, key) =>
                {
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(sp, dataSourceBuilder, state);
                    return dataSourceBuilder.BuildMultiHost();
                },
                dataSourceLifetime));

        if (serviceKey is not null)
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(NpgsqlDataSource),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<NpgsqlMultiHostDataSource>(key),
                    dataSourceLifetime));
        }
        else
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(NpgsqlDataSource),
                    sp => sp.GetRequiredService<NpgsqlMultiHostDataSource>(),
                    dataSourceLifetime));

        }

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddMultiHostNpgsqlSlimDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<IServiceProvider, NpgsqlSlimDataSourceBuilder, object?>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime,
        object? state)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlMultiHostDataSource),
                serviceKey,
                (sp, _) =>
                {
                    var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(sp, dataSourceBuilder, state);
                    return dataSourceBuilder.BuildMultiHost();
                },
                dataSourceLifetime));

        if (serviceKey is not null)
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(NpgsqlDataSource),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<NpgsqlMultiHostDataSource>(key),
                    dataSourceLifetime));
        }
        else
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(NpgsqlDataSource),
                    sp => sp.GetRequiredService<NpgsqlMultiHostDataSource>(),
                    dataSourceLifetime));

        }

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static void AddCommonServices(
        IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        // We don't try to invoke KeyedService methods if there is no service key.
        // This allows user code that use non-standard containers without support for IKeyedServiceProvider to keep on working.
        if (serviceKey is not null)
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(NpgsqlConnection),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<NpgsqlDataSource>(key).CreateConnection(),
                    connectionLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbDataSource),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<NpgsqlDataSource>(key),
                    dataSourceLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbConnection),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<NpgsqlConnection>(key),
                    connectionLifetime));
        }
        else
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(NpgsqlConnection),
                    sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection(),
                    connectionLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbDataSource),
                    sp => sp.GetRequiredService<NpgsqlDataSource>(),
                    dataSourceLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbConnection),
                    sp => sp.GetRequiredService<NpgsqlConnection>(),
                    connectionLifetime));
        }
    }
}
