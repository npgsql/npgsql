using System;
using System.Data.Common;

namespace Npgsql;

/// <summary>
/// A factory to create instances of various Npgsql objects.
/// </summary>
[Serializable]
public sealed class NpgsqlFactory : DbProviderFactory, IServiceProvider
{
    /// <summary>
    /// Gets an instance of the <see cref="NpgsqlFactory"/>.
    /// This can be used to retrieve strongly typed data objects.
    /// </summary>
    public static readonly NpgsqlFactory Instance = new();

    NpgsqlFactory() {}

    /// <summary>
    /// Returns a strongly typed <see cref="DbCommand"/> instance.
    /// </summary>
    public override DbCommand CreateCommand() => new NpgsqlCommand();

    /// <summary>
    /// Returns a strongly typed <see cref="DbConnection"/> instance.
    /// </summary>
    public override DbConnection CreateConnection() => new NpgsqlConnection();

    /// <summary>
    /// Returns a strongly typed <see cref="DbParameter"/> instance.
    /// </summary>
    public override DbParameter CreateParameter() => new NpgsqlParameter();

    /// <summary>
    /// Returns a strongly typed <see cref="DbConnectionStringBuilder"/> instance.
    /// </summary>
    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new NpgsqlConnectionStringBuilder();

    /// <summary>
    /// Returns a strongly typed <see cref="DbCommandBuilder"/> instance.
    /// </summary>
    public override DbCommandBuilder CreateCommandBuilder() => new NpgsqlCommandBuilder();

    /// <summary>
    /// Returns a strongly typed <see cref="DbDataAdapter"/> instance.
    /// </summary>
    public override DbDataAdapter CreateDataAdapter() => new NpgsqlDataAdapter();

    /// <summary>
    /// Specifies whether the specific <see cref="DbProviderFactory"/> supports the <see cref="DbDataAdapter"/> class.
    /// </summary>
    public override bool CanCreateDataAdapter => true;

    /// <summary>
    /// Specifies whether the specific <see cref="DbProviderFactory"/> supports the <see cref="DbCommandBuilder"/> class.
    /// </summary>
    public override bool CanCreateCommandBuilder => true;

    /// <inheritdoc/>
    public override bool CanCreateBatch => true;

    /// <inheritdoc/>
    public override DbBatch CreateBatch() => new NpgsqlBatch();

    /// <inheritdoc/>
    public override DbBatchCommand CreateBatchCommand() => new NpgsqlBatchCommand();

    /// <inheritdoc/>
    public override DbDataSource CreateDataSource(string connectionString)
        => NpgsqlDataSource.Create(connectionString);

    #region IServiceProvider Members

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>A service object of type serviceType, or null if there is no service object of type serviceType.</returns>
    public object? GetService(Type serviceType) => null;

    #endregion
}
