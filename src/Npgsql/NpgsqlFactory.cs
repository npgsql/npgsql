using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql;

/// <summary>
/// A factory to create instances of various Npgsql objects.
/// </summary>
[Serializable]
[RequiresUnreferencedCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums etc. Use NpgsqlSlimDataSourceBuilder to start with a reduced - reflection free - set and opt into what your app specifically requires.")]
[RequiresDynamicCode("NpgsqlDataSource uses reflection to handle various PostgreSQL types like records, unmapped enums. This can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
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

#if !NETSTANDARD2_0
    /// <summary>
    /// Specifies whether the specific <see cref="DbProviderFactory"/> supports the <see cref="DbDataAdapter"/> class.
    /// </summary>
    public override bool CanCreateDataAdapter => true;

    /// <summary>
    /// Specifies whether the specific <see cref="DbProviderFactory"/> supports the <see cref="DbCommandBuilder"/> class.
    /// </summary>
    public override bool CanCreateCommandBuilder => true;
#endif

#if NET6_0_OR_GREATER
    /// <inheritdoc/>
    public override bool CanCreateBatch => true;

    /// <inheritdoc/>
    public override DbBatch CreateBatch() => new NpgsqlBatch();

    /// <inheritdoc/>
    public override DbBatchCommand CreateBatchCommand() => new NpgsqlBatchCommand();
#endif

#if NET7_0_OR_GREATER
    /// <inheritdoc/>
    public override DbDataSource CreateDataSource(string connectionString)
        => NpgsqlDataSource.Create(connectionString);
#endif

    #region IServiceProvider Members

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>A service object of type serviceType, or null if there is no service object of type serviceType.</returns>
    public object? GetService(Type serviceType) => null;

    #endregion
}
