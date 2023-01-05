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
public static class NpgsqlServiceCollectionExtensions
{
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
    /// Defaults to <see cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlDataSourceCore(serviceCollection, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlDataSource" /> and an <see cref="NpgsqlConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlDataSourceCore(
            serviceCollection, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

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
    /// Defaults to <see cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlMultiHostDataSourceCore(
            serviceCollection, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="NpgsqlMultiHostDataSource" /> and an <see cref="NpgsqlConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An Npgsql connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="NpgsqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddNpgsqlMultiHostDataSourceCore(
            serviceCollection, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    static IServiceCollection AddNpgsqlDataSourceCore(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlDataSource),
                sp =>
                {
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddNpgsqlMultiHostDataSourceCore(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(NpgsqlMultiHostDataSource),
                sp =>
                {
                    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
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

    static void AddCommonServices(
        IServiceCollection serviceCollection,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
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