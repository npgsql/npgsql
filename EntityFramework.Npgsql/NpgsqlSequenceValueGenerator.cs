using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlSequenceValueGenerator<TValue> : HiLoValueGenerator<TValue>
    {
		private readonly SqlStatementExecutor _executor;
		private readonly NpgsqlEntityFrameworkConnection _connection;

		public NpgsqlSequenceValueGenerator(
			[NotNull] SqlStatementExecutor executor,
			[NotNull] NpgsqlEntityFrameworkConnection connection,
			[NotNull] string sequenceName,
			int blockSize)
            : base(blockSize)
        {
			Check.NotNull(executor, nameof(executor));
			Check.NotEmpty(sequenceName, nameof(sequenceName));

			SequenceName = sequenceName;

			_executor = executor;
			_connection = connection;
		}

		public virtual string SequenceName { get; }

		protected override long GetNewLowValue()
		{
			var commandInfo = PrepareCommand(_connection);
			var nextValue = _executor.ExecuteScalar(commandInfo.Item1, commandInfo.Item1.DbTransaction, commandInfo.Item2);

			return (long)Convert.ChangeType(nextValue, typeof(long), CultureInfo.InvariantCulture);
		}
		
		private Tuple<RelationalConnection, string> PrepareCommand(RelationalConnection connection)
		{
			// TODO: Parameterize query and/or delimit identifier without using SqlServerMigrationOperationSqlGenerator
			var sql = string.Format(
				CultureInfo.InvariantCulture,
				"SELECT NEXT VALUE FOR {0}", SequenceName);

			return Tuple.Create(connection, sql);
		}
		
		public override bool GeneratesTemporaryValues => false;
	}
}
