using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Properties;

namespace Npgsql;

/// <summary>
/// Provides a simple API for configuring and creating an <see cref="NpgsqlDataSource" />, from which database connections can be obtained.
/// </summary>
public class NpgsqlDataSourceBuilder
{
    ILoggerFactory? _loggerFactory;
    bool _sensitiveDataLoggingEnabled;

    Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? _periodicPasswordProvider;
    TimeSpan _periodicPasswordSuccessRefreshInterval, _periodicPasswordFailureRefreshInterval;

    /// <summary>
    /// A connection string builder that can be used to configured the connection string on the builder.
    /// </summary>
    public NpgsqlConnectionStringBuilder ConnectionStringBuilder { get; }

    /// <summary>
    /// Returns the connection string, as currently configured on the builder.
    /// </summary>
    public string ConnectionString => ConnectionStringBuilder.ToString();

    /// <summary>
    /// Constructs a new <see cref="NpgsqlDataSourceBuilder" />, optionally starting out from the given <paramref name="connectionString"/>.
    /// </summary>
    public NpgsqlDataSourceBuilder(string? connectionString = null)
        => ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

    /// <summary>
    /// Sets the <see cref="ILoggerFactory" /> that will be used for logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to be used.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder UseLoggerFactory(ILoggerFactory? loggerFactory)
    {
        _loggerFactory = loggerFactory;
        return this;
    }

    /// <summary>
    /// Enables parameters to be included in logging. This includes potentially sensitive information from data sent to PostgreSQL.
    /// You should only enable this flag in development, or if you have the appropriate security measures in place based on the
    /// sensitivity of this data.
    /// </summary>
    /// <param name="parameterLoggingEnabled">If <see langword="true" />, then sensitive data is logged.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public NpgsqlDataSourceBuilder EnableParameterLogging(bool parameterLoggingEnabled = true)
    {
        _sensitiveDataLoggingEnabled = parameterLoggingEnabled;
        return this;
    }

    /// <summary>
    /// Configures a periodic password provider, which is automatically called by the data source at some regular interval. This is the
    /// recommended way to fetch a rotating access token.
    /// </summary>
    /// <param name="passwordProvider">A callback which returns the password to be sent to PostgreSQL.</param>
    /// <param name="successRefreshInterval">How long to cache the password before re-invoking the callback.</param>
    /// <param name="failureRefreshInterval">
    /// If a password refresh attempt fails, it will be re-attempted with this interval.
    /// This should typically be much lower than <paramref name="successRefreshInterval" />.
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    /// <remarks>
    /// <para>
    /// The provided callback is invoked in a timer, and not when opening connections. It therefore doesn't affect opening time.
    /// </para>
    /// <para>
    /// The provided cancellation token is only triggered when the entire data source is disposed. If you'd like to apply a timeout to the
    /// token fetching, do so within the provided callback.
    /// </para>
    /// </remarks>
    public NpgsqlDataSourceBuilder UsePeriodicPasswordProvider(
        Func<NpgsqlConnectionStringBuilder, CancellationToken, ValueTask<string>>? passwordProvider,
        TimeSpan successRefreshInterval,
        TimeSpan failureRefreshInterval)
    {
        if (successRefreshInterval < TimeSpan.Zero)
            throw new ArgumentException(
                string.Format(NpgsqlStrings.ArgumentMustBePositive, nameof(successRefreshInterval)), nameof(successRefreshInterval));
        if (failureRefreshInterval < TimeSpan.Zero)
            throw new ArgumentException(
                string.Format(NpgsqlStrings.ArgumentMustBePositive, nameof(failureRefreshInterval)), nameof(failureRefreshInterval));

        _periodicPasswordProvider = passwordProvider;
        _periodicPasswordSuccessRefreshInterval = successRefreshInterval;
        _periodicPasswordFailureRefreshInterval = failureRefreshInterval;

        return this;
    }

    /// <summary>
    /// Builds and returns an <see cref="NpgsqlDataSource" /> which is ready for use.
    /// </summary>
    public NpgsqlDataSource Build()
    {
        ConnectionStringBuilder.PostProcessAndValidate();

        if (_periodicPasswordProvider is not null &&
            (ConnectionStringBuilder.Password is not null || ConnectionStringBuilder.Passfile is not null))
        {
            throw new NotSupportedException(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword);
        }

        var loggingConfiguration = _loggerFactory is null
            ? NpgsqlLoggingConfiguration.NullConfiguration
            : new NpgsqlLoggingConfiguration(_loggerFactory, _sensitiveDataLoggingEnabled);

        var config = new NpgsqlDataSourceConfiguration(
            loggingConfiguration,
            _periodicPasswordProvider,
            _periodicPasswordSuccessRefreshInterval,
            _periodicPasswordFailureRefreshInterval);

        if (ConnectionStringBuilder.Host!.Contains(","))
        {
            if (ConnectionStringBuilder.TargetSessionAttributes is not null)
                throw new InvalidOperationException(NpgsqlStrings.CannotSpecifyTargetSessionAttributes);
            if (ConnectionStringBuilder.Multiplexing)
                throw new NotSupportedException("Multiplexing is not supported with multiple hosts");
            if (ConnectionStringBuilder.ReplicationMode != ReplicationMode.Off)
                throw new NotSupportedException("Replication is not supported with multiple hosts");

            return new NpgsqlMultiHostDataSource(ConnectionStringBuilder, config);
        }

        return ConnectionStringBuilder.Multiplexing
            ? new MultiplexingDataSource(ConnectionStringBuilder, config)
            : ConnectionStringBuilder.Pooling
                ? new PoolingDataSource(ConnectionStringBuilder, config)
                : new UnpooledDataSource(ConnectionStringBuilder, config);
    }

#pragma warning disable RS0016
    /// <summary>
    /// Builds and returns a <see cref="NpgsqlMultiHostDataSource" /> which is ready for use for load-balancing and failover scenarios.
    /// </summary>
    public NpgsqlMultiHostDataSource BuildMultiHost()
        => Build() as NpgsqlMultiHostDataSource ?? throw new InvalidOperationException(NpgsqlStrings.MultipleHostsMustBeSpecified);
#pragma warning restore RS0016
}
