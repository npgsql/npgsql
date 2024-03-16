using System;
using System.ComponentModel;
using Npgsql;

namespace Microsoft.Extensions.DependencyInjection;

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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddNpgsqlDataSourceCore(
            serviceCollection, serviceKey: null, connectionString, dataSourceBuilderAction: null,
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddNpgsqlDataSourceCore(serviceCollection, serviceKey: null, connectionString,
            static (_, builder, state) => ((Action<NpgsqlDataSourceBuilder>)state!)(builder),
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey: null, connectionString, dataSourceBuilderAction: null,
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddNpgsqlSlimDataSourceCore(serviceCollection, serviceKey: null, connectionString,
            static (_, builder, state) => ((Action<NpgsqlSlimDataSourceBuilder>)state!)(builder),
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, serviceKey: null, connectionString, dataSourceBuilderAction: null,
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddMultiHostNpgsqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddMultiHostNpgsqlDataSourceCore(
            serviceCollection, serviceKey: null, connectionString,
            static (_, builder, state) => ((Action<NpgsqlDataSourceBuilder>)state!)(builder),
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey: null, connectionString, dataSourceBuilderAction: null,
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
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Defined for binary compatibility with 7.0")]
    public static IServiceCollection AddMultiHostNpgsqlSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<NpgsqlSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
        => AddMultiHostNpgsqlSlimDataSourceCore(
            serviceCollection, serviceKey: null, connectionString,
            static (_, builder, state) => ((Action<NpgsqlSlimDataSourceBuilder>)state!)(builder),
            connectionLifetime, dataSourceLifetime, state: dataSourceBuilderAction);
}
