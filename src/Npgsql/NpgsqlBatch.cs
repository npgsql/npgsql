using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <inheritdoc />
    public class NpgsqlBatch : DbBatch
    {
        readonly NpgsqlCommand _command;

        /// <inheritdoc />
        protected override DbBatchCommandCollection DbBatchCommands => BatchCommands;

        /// <inheritdoc cref="DbBatch.BatchCommands"/>
        public new NpgsqlBatchCommandCollection BatchCommands { get; }

        /// <inheritdoc />
        public override int Timeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        /// <inheritdoc cref="DbBatch.Connection"/>
        public new NpgsqlConnection? Connection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }

        /// <inheritdoc />
        protected override DbConnection? DbConnection
        {
            get => Connection;
            set => Connection = (NpgsqlConnection?)value;
        }

        /// <inheritdoc cref="DbBatch.Transaction"/>
        public new NpgsqlTransaction? Transaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }

        /// <inheritdoc />
        protected override DbTransaction? DbTransaction
        {
            get => Transaction;
            set => Transaction = (NpgsqlTransaction?)value;
        }

        /// <summary>
        /// Initializes a new <see cref="NpgsqlBatch"/>.
        /// </summary>
        /// <param name="connection">A <see cref="NpgsqlConnection"/> that represents the connection to a PostgreSQL server.</param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction"/> in which the <see cref="NpgsqlCommand"/> executes.</param>
        public NpgsqlBatch(NpgsqlConnection? connection = null, NpgsqlTransaction? transaction = null)
        {
            var batchCommands = new List<NpgsqlBatchCommand>(5);
            _command = new(batchCommands);
            BatchCommands = new NpgsqlBatchCommandCollection(batchCommands);

            Connection = connection;
            Transaction = transaction;
        }

        /// <inheritdoc />
        protected override DbBatchCommand CreateDbBatchCommand()
            => new NpgsqlBatchCommand();

        /// <inheritdoc />
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => ExecuteReader(behavior);

        /// <inheritdoc cref="DbBatch.ExecuteReader"/>
        public new NpgsqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
            => _command.ExecuteReader();

        /// <inheritdoc />
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken)
            => await ExecuteReaderAsync(cancellationToken);

        /// <inheritdoc cref="DbBatch.ExecuteReaderAsync(CancellationToken)"/>
        public new Task<NpgsqlDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
            => _command.ExecuteReaderAsync(cancellationToken);

        /// <inheritdoc cref="DbBatch.ExecuteReaderAsync(CommandBehavior,CancellationToken)"/>
        public new Task<NpgsqlDataReader> ExecuteReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken = default)
            => _command.ExecuteReaderAsync(behavior, cancellationToken);

        /// <inheritdoc />
        public override int ExecuteNonQuery()
            => _command.ExecuteNonQuery();

        /// <inheritdoc />
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
            => _command.ExecuteNonQueryAsync(cancellationToken);

        /// <inheritdoc />
        public override object? ExecuteScalar()
            => _command.ExecuteScalar();

        /// <inheritdoc />
        public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken = default)
            => _command.ExecuteScalarAsync(cancellationToken);

        /// <inheritdoc />
        public override void Prepare()
            => _command.Prepare();

        /// <inheritdoc />
        public override Task PrepareAsync(CancellationToken cancellationToken = default)
            => _command.PrepareAsync(cancellationToken);

        /// <inheritdoc />
        public override void Cancel() => _command.Cancel();
    }
}
