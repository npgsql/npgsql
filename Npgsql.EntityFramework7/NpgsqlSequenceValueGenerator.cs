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
		private readonly string _sequenceName;
		
		public NpgsqlSequenceValueGenerator(
			[NotNull] SqlStatementExecutor executor,
			[NotNull] NpgsqlEntityFrameworkConnection connection,
			[NotNull] string sequenceName,
			[NotNull] HiLoValueGeneratorState generatorState)
            : base(generatorState)
        {
			Check.NotNull(executor, nameof(executor));
			Check.NotEmpty(sequenceName, nameof(sequenceName));

			_sequenceName = sequenceName;

			_executor = executor;
			_connection = connection;
			_sequenceName = sequenceName;
		}

		public virtual string SequenceName
		{
			get
			{
				return _sequenceName;
			}
		}

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
				"SELECT NEXT VALUE FOR {0}", _sequenceName);

			return Tuple.Create(connection, sql);
		}
		
		public override bool GeneratesTemporaryValues => false;
	}
}
