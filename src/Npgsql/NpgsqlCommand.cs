using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.TypeMapping;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql
{
    /// <summary>
    /// Represents a SQL statement or function (stored procedure) to execute
    /// against a PostgreSQL database. This class cannot be inherited.
    /// </summary>
    // ReSharper disable once RedundantNameQualifier
    [System.ComponentModel.DesignerCategory("")]
    public sealed class NpgsqlCommand : DbCommand, ICloneable
    {
        #region Fields

        NpgsqlConnection? _connection;

        /// <summary>
        /// If this command is (explicitly) prepared, references the connector on which the preparation happened.
        /// Used to detect when the connector was changed (i.e. connection open/close), meaning that the command
        /// is no longer prepared.
        /// </summary>
        NpgsqlConnector? _connectorPreparedOn;

        string? _commandText;
        int? _timeout;
        readonly NpgsqlParameterCollection _parameters;

        readonly List<NpgsqlStatement> _statements;

        /// <summary>
        /// Returns details about each statement that this command has executed.
        /// Is only populated when an Execute* method is called.
        /// </summary>
        public IReadOnlyList<NpgsqlStatement> Statements => _statements.AsReadOnly();

        UpdateRowSource _updateRowSource = UpdateRowSource.Both;

        bool IsExplicitlyPrepared => _connectorPreparedOn != null;

        static readonly List<NpgsqlParameter> EmptyParameters = new List<NpgsqlParameter>();

        static readonly SingleThreadSynchronizationContext SingleThreadSynchronizationContext = new SingleThreadSynchronizationContext("NpgsqlRemainingAsyncSendWorker");

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlCommand));

        #endregion Fields

        #region Constants

        internal const int DefaultTimeout = 30;

        #endregion

        #region Constructors

#nullable disable

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class.
        /// </summary>
        public NpgsqlCommand() : this(null, null, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        // ReSharper disable once IntroduceOptionalParameters.Global
        public NpgsqlCommand(string cmdText) : this(cmdText, null, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query and a <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        // ReSharper disable once IntroduceOptionalParameters.Global
        public NpgsqlCommand(string cmdText, NpgsqlConnection connection) : this(cmdText, connection, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query, a <see cref="NpgsqlConnection">NpgsqlConnection</see>, and the <see cref="NpgsqlTransaction">NpgsqlTransaction</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction">NpgsqlTransaction</see> in which the <see cref="NpgsqlCommand">NpgsqlCommand</see> executes.</param>
        public NpgsqlCommand(string cmdText, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            GC.SuppressFinalize(this);
            _statements = new List<NpgsqlStatement>(1);
            _parameters = new NpgsqlParameterCollection();
            _commandText = cmdText;
            _connection = connection;
            Transaction = transaction;
            CommandType = CommandType.Text;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the SQL statement or function (stored procedure) to execute at the data source.
        /// </summary>
        /// <value>The Transact-SQL statement or stored procedure to execute. The default is an empty string.</value>
        [DefaultValue("")]
        [Category("Data")]
        public override string CommandText
        {
            get => _commandText;
            set
            {
                _commandText = State == CommandState.Idle
                    ? value ?? string.Empty
                    : throw new InvalidOperationException("An open data reader exists for this command.");

                ResetExplicitPreparation();
                // TODO: Technically should do this also if the parameter list (or type) changes
            }
        }

#nullable restore

        /// <summary>
        /// Gets or sets the wait time (in seconds) before terminating the attempt  to execute a command and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for the command to execute. The default value is 30 seconds.</value>
        [DefaultValue(DefaultTimeout)]
        public override int CommandTimeout
        {
            get => _timeout ?? (_connection?.CommandTimeout ?? DefaultTimeout);
            set
            {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "CommandTimeout can't be less than zero.");
                }

                _timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the
        /// <see cref="NpgsqlCommand.CommandText">CommandText</see> property is to be interpreted.
        /// </summary>
        /// <value>One of the <see cref="System.Data.CommandType">CommandType</see> values. The default is <see cref="System.Data.CommandType">CommandType.Text</see>.</value>
        [DefaultValue(CommandType.Text)]
        [Category("Data")]
        public override CommandType CommandType { get; set; }

        /// <summary>
        /// DB connection.
        /// </summary>
        protected override DbConnection? DbConnection
        {
            get => _connection;
            set => _connection = (NpgsqlConnection?)value;
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// used by this instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <value>The connection to a data source. The default value is a null reference.</value>
        [DefaultValue(null)]
        [Category("Behavior")]
        public new NpgsqlConnection? Connection
        {
            get => _connection;
            set
            {
                if (_connection == value)
                    return;

                _connection = State == CommandState.Idle
                    ? value
                    : throw new InvalidOperationException("An open data reader exists for this command.");

                Transaction = null;
            }
        }

        /// <summary>
        /// Design time visible.
        /// </summary>
        public override bool DesignTimeVisible { get; set; }

        /// <summary>
        /// Gets or sets how command results are applied to the DataRow when used by the
        /// DbDataAdapter.Update(DataSet) method.
        /// </summary>
        /// <value>One of the <see cref="System.Data.UpdateRowSource">UpdateRowSource</see> values.</value>
        [Category("Behavior"), DefaultValue(UpdateRowSource.Both)]
        public override UpdateRowSource UpdatedRowSource
        {
            get => _updateRowSource;
            set
            {
                switch (value)
                {
                    // validate value (required based on base type contract)
                    case UpdateRowSource.None:
                    case UpdateRowSource.OutputParameters:
                    case UpdateRowSource.FirstReturnedRecord:
                    case UpdateRowSource.Both:
                        _updateRowSource = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Returns whether this query will execute as a prepared (compiled) query.
        /// </summary>
        public bool IsPrepared =>
            _connectorPreparedOn == _connection?.Connector &&
            _statements.Any() && _statements.All(s => s.PreparedStatement?.IsPrepared == true);

        #endregion Public properties

        #region Known/unknown Result Types Management

        /// <summary>
        /// Marks all of the query's result columns as either known or unknown.
        /// Unknown results column are requested them from PostgreSQL in text format, and Npgsql makes no
        /// attempt to parse them. They will be accessible as strings only.
        /// </summary>
        public bool AllResultTypesAreUnknown
        {
            get => _allResultTypesAreUnknown;
            set
            {
                // TODO: Check that this isn't modified after calling prepare
                _unknownResultTypeList = null;
                _allResultTypesAreUnknown = value;
            }
        }

        bool _allResultTypesAreUnknown;

        /// <summary>
        /// Marks the query's result columns as known or unknown, on a column-by-column basis.
        /// Unknown results column are requested them from PostgreSQL in text format, and Npgsql makes no
        /// attempt to parse them. They will be accessible as strings only.
        /// </summary>
        /// <remarks>
        /// If the query includes several queries (e.g. SELECT 1; SELECT 2), this will only apply to the first
        /// one. The rest of the queries will be fetched and parsed as usual.
        ///
        /// The array size must correspond exactly to the number of result columns the query returns, or an
        /// error will be raised.
        /// </remarks>
        public bool[]? UnknownResultTypeList
        {
            get => _unknownResultTypeList;
            set
            {
                // TODO: Check that this isn't modified after calling prepare
                _allResultTypesAreUnknown = false;
                _unknownResultTypeList = value;
            }
        }

        bool[]? _unknownResultTypeList;

        #endregion

        #region Result Types Management

        /// <summary>
        /// Marks result types to be used when using GetValue on a data reader, on a column-by-column basis.
        /// Used for Entity Framework 5-6 compability.
        /// Only primitive numerical types and DateTimeOffset are supported.
        /// Set the whole array or just a value to null to use default type.
        /// </summary>
        internal Type[]? ObjectResultTypes { get; set; }

        #endregion

        #region State management

        int _state;

        /// <summary>
        /// Gets the current state of the connector
        /// </summary>
        internal CommandState State
        {
            private get { return (CommandState)_state; }
            set
            {
                var newState = (int)value;
                if (newState == _state)
                    return;
                Interlocked.Exchange(ref _state, newState);
            }
        }

        void ResetExplicitPreparation() => _connectorPreparedOn = null;

        #endregion State management

        #region Parameters

        /// <summary>
        /// Creates a new instance of an <see cref="System.Data.Common.DbParameter">DbParameter</see> object.
        /// </summary>
        /// <returns>An <see cref="System.Data.Common.DbParameter">DbParameter</see> object.</returns>
        protected override DbParameter CreateDbParameter()
        {
            return CreateParameter();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="NpgsqlParameter">NpgsqlParameter</see> object.
        /// </summary>
        /// <returns>A <see cref="NpgsqlParameter">NpgsqlParameter</see> object.</returns>
        public new NpgsqlParameter CreateParameter()
        {
            return new NpgsqlParameter();
        }

        /// <summary>
        /// DB parameter collection.
        /// </summary>
        protected override DbParameterCollection DbParameterCollection => Parameters;

        /// <summary>
        /// Gets the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see>.
        /// </summary>
        /// <value>The parameters of the SQL statement or function (stored procedure). The default is an empty collection.</value>
        public new NpgsqlParameterCollection Parameters => _parameters;

        #endregion

        #region DeriveParameters

        const string DeriveParametersForFunctionQuery = @"
SELECT
CASE
	WHEN pg_proc.proargnames IS NULL THEN array_cat(array_fill(''::name,ARRAY[pg_proc.pronargs]),array_agg(pg_attribute.attname ORDER BY pg_attribute.attnum))
	ELSE pg_proc.proargnames
END AS proargnames,
pg_proc.proargtypes,
CASE
	WHEN pg_proc.proallargtypes IS NULL AND (array_agg(pg_attribute.atttypid))[1] IS NOT NULL THEN array_cat(string_to_array(pg_proc.proargtypes::text,' ')::oid[],array_agg(pg_attribute.atttypid ORDER BY pg_attribute.attnum))
	ELSE pg_proc.proallargtypes
END AS proallargtypes,
CASE
	WHEN pg_proc.proargmodes IS NULL AND (array_agg(pg_attribute.atttypid))[1] IS NOT NULL THEN array_cat(array_fill('i'::""char"",ARRAY[pg_proc.pronargs]),array_fill('o'::""char"",ARRAY[array_length(array_agg(pg_attribute.atttypid), 1)]))
    ELSE pg_proc.proargmodes
END AS proargmodes
FROM pg_proc
LEFT JOIN pg_type ON pg_proc.prorettype = pg_type.oid
LEFT JOIN pg_attribute ON pg_type.typrelid = pg_attribute.attrelid AND pg_attribute.attnum >= 1 AND NOT pg_attribute.attisdropped
WHERE pg_proc.oid = :proname::regproc
GROUP BY pg_proc.proargnames, pg_proc.proargtypes, pg_proc.proallargtypes, pg_proc.proargmodes, pg_proc.pronargs;
";

        internal void DeriveParameters()
        {
            if (Statements.Any(s => s.PreparedStatement?.IsExplicit == true))
                throw new NpgsqlException("Deriving parameters isn't supported for commands that are already prepared.");

            // Here we unprepare statements that possibly are auto-prepared
            Unprepare();

            Parameters.Clear();

            if (CommandType == CommandType.StoredProcedure)
                DeriveParametersForFunction();
            else if (CommandType == CommandType.Text)
                DeriveParametersForQuery();
        }

        void DeriveParametersForFunction()
        {
            using var c = new NpgsqlCommand(DeriveParametersForFunctionQuery, _connection);
            c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Text));
            c.Parameters[0].Value = CommandText;

            string[]? names = null;
            uint[]? types = null;
            char[]? modes = null;

            using (var rdr = c.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                        names = rdr.GetFieldValue<string[]>(0);
                    if (!rdr.IsDBNull(2))
                        types = rdr.GetFieldValue<uint[]>(2);
                    if (!rdr.IsDBNull(3))
                        modes = rdr.GetFieldValue<char[]>(3);
                    if (types == null)
                    {
                        if (rdr.IsDBNull(1) || rdr.GetFieldValue<uint[]>(1).Length == 0)
                            return;  // Parameter-less function
                        types = rdr.GetFieldValue<uint[]>(1);
                    }
                }
                else
                    throw new InvalidOperationException($"{CommandText} does not exist in pg_proc");
            }

            var typeMapper = c._connection!.Connector!.TypeMapper;

            for (var i = 0; i < types.Length; i++)
            {
                var param = new NpgsqlParameter();

                var (npgsqlDbType, postgresType) = typeMapper.GetTypeInfoByOid(types[i]);

                param.DataTypeName = postgresType.DisplayName;
                param.PostgresType = postgresType;
                if (npgsqlDbType.HasValue)
                    param.NpgsqlDbType = npgsqlDbType.Value;

                if (names != null && i < names.Length)
                    param.ParameterName = names[i];
                else
                    param.ParameterName = "parameter" + (i + 1);

                if (modes == null) // All params are IN, or server < 8.1.0 (and only IN is supported)
                    param.Direction = ParameterDirection.Input;
                else
                {
                    param.Direction = modes[i] switch
                    {
                        'i' => ParameterDirection.Input,
                        'o' => ParameterDirection.Output,
                        't' => ParameterDirection.Output,
                        'b' => ParameterDirection.InputOutput,
                        'v' => throw new NotSupportedException("Cannot derive function parameter of type VARIADIC"),
                        _ => throw new ArgumentOutOfRangeException("Unknown code in proargmodes while deriving: " + modes[i])
                    };
                }

                Parameters.Add(param);
            }
        }

        void DeriveParametersForQuery()
        {
            var connector = CheckReadyAndGetConnector();
            using (connector.StartUserAction())
            {
                Log.Debug($"Deriving Parameters for query: {CommandText}", connector.Id);
                ProcessRawQuery(true);

                var sendTask = SendDeriveParameters(connector, false);

                foreach (var statement in _statements)
                {
                    Expect<ParseCompleteMessage>(connector.ReadMessage(), connector);
                    var paramTypeOIDs = Expect<ParameterDescriptionMessage>(connector.ReadMessage(), connector).TypeOIDs;

                    if (statement.InputParameters.Count != paramTypeOIDs.Count)
                    {
                        connector.SkipUntil(BackendMessageCode.ReadyForQuery);
                        Parameters.Clear();
                        throw new NpgsqlException("There was a mismatch in the number of derived parameters between the Npgsql SQL parser and the PostgreSQL parser. Please report this as bug to the Npgsql developers (https://github.com/npgsql/npgsql/issues).");
                    }

                    for (var i = 0; i < paramTypeOIDs.Count; i++)
                    {
                        try
                        {
                            var param = statement.InputParameters[i];
                            var paramOid = paramTypeOIDs[i];

                            var (npgsqlDbType, postgresType) = connector.TypeMapper.GetTypeInfoByOid(paramOid);

                            if (param.NpgsqlDbType != NpgsqlDbType.Unknown && param.NpgsqlDbType != npgsqlDbType)
                                throw new NpgsqlException("The backend parser inferred different types for parameters with the same name. Please try explicit casting within your SQL statement or batch or use different placeholder names.");

                            param.DataTypeName = postgresType.DisplayName;
                            param.PostgresType = postgresType;
                            if (npgsqlDbType.HasValue)
                                param.NpgsqlDbType = npgsqlDbType.Value;
                        }
                        catch
                        {
                            connector.SkipUntil(BackendMessageCode.ReadyForQuery);
                            Parameters.Clear();
                            throw;
                        }
                    }

                    var msg = connector.ReadMessage();
                    switch (msg.Code)
                    {
                        case BackendMessageCode.RowDescription:
                        case BackendMessageCode.NoData:
                            break;
                        default:
                            throw connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                Expect<ReadyForQueryMessage>(connector.ReadMessage(), connector);
                sendTask.GetAwaiter().GetResult();
            }
        }

        #endregion

        #region Prepare

        /// <summary>
        /// Creates a server-side prepared statement on the PostgreSQL server.
        /// This will make repeated future executions of this command much faster.
        /// </summary>
        public override void Prepare() => Prepare(false).GetAwaiter().GetResult();

        /// <summary>
        /// Creates a server-side prepared statement on the PostgreSQL server.
        /// This will make repeated future executions of this command much faster.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
#if !NET461 && !NETSTANDARD2_0
        public override Task PrepareAsync(CancellationToken cancellationToken = default)
#else
        public Task PrepareAsync(CancellationToken cancellationToken = default)
#endif
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Prepare(true);
        }

        Task Prepare(bool async)
        {
            var connector = CheckReadyAndGetConnector();
            for (var i = 0; i < Parameters.Count; i++)
                Parameters[i].Bind(connector.TypeMapper);

            ProcessRawQuery();
            Log.Debug($"Preparing: {CommandText}", connector.Id);

            var needToPrepare = false;
            foreach (var statement in _statements)
            {
                if (statement.IsPrepared)
                    continue;
                statement.PreparedStatement = connector.PreparedStatementManager.GetOrAddExplicit(statement);
                if (statement.PreparedStatement?.State == PreparedState.NotPrepared)
                {
                    statement.PreparedStatement.State = PreparedState.ToBePrepared;
                    needToPrepare = true;
                }
            }

            _connectorPreparedOn = connector;

            // It's possible the command was already prepared, or that persistent prepared statements were found for
            // all statements. Nothing to do here, move along.
            return needToPrepare
                ? PrepareLong()
                : Task.CompletedTask;

            async Task PrepareLong()
            {
                using (connector.StartUserAction())
                {
                    var sendTask = SendPrepare(connector, async);

                    // Loop over statements, skipping those that are already prepared (because they were persisted)
                    var isFirst = true;
                    for (var i = 0; i < _statements.Count; i++)
                    {
                        var statement = _statements[i];
                        var pStatement = statement.PreparedStatement;
                        if (pStatement?.State != PreparedState.BeingPrepared)
                            continue;

                        try
                        {
                            if (pStatement.StatementBeingReplaced != null)
                            {
                                Expect<CloseCompletedMessage>(await connector.ReadMessage(async), connector);
                                pStatement.StatementBeingReplaced.CompleteUnprepare();
                                pStatement.StatementBeingReplaced = null;
                            }

                            Expect<ParseCompleteMessage>(await connector.ReadMessage(async), connector);
                            Expect<ParameterDescriptionMessage>(await connector.ReadMessage(async), connector);
                            var msg = await connector.ReadMessage(async);
                            switch (msg.Code)
                            {
                            case BackendMessageCode.RowDescription:
                                // Clone the RowDescription for use with the prepared statement (the one we have is reused
                                // by the connection)
                                var description = ((RowDescriptionMessage)msg).Clone();
                                FixupRowDescription(description, isFirst);
                                statement.Description = description;
                                break;
                            case BackendMessageCode.NoData:
                                statement.Description = null;
                                break;
                            default:
                                throw connector.UnexpectedMessageReceived(msg.Code);
                            }

                            pStatement.CompletePrepare();
                            isFirst = false;
                        }
                        catch
                        {
                            // The statement wasn't prepared successfully, update the bookkeeping for it and
                            // all following statements
                            for (; i < _statements.Count; i++)
                            {
                                pStatement = _statements[i].PreparedStatement;
                                if (pStatement?.State == PreparedState.BeingPrepared)
                                    pStatement.CompleteUnprepare();
                            }

                            throw;
                        }
                    }

                    Expect<ReadyForQueryMessage>(await connector.ReadMessage(async), connector);

                    if (async)
                        await sendTask;
                    else
                        sendTask.GetAwaiter().GetResult();
                }
            }
        }

        /// <summary>
        /// Unprepares a command, closing server-side statements associated with it.
        /// Note that this only affects commands explicitly prepared with <see cref="Prepare()"/>, not
        /// automatically prepared statements.
        /// </summary>
        public void Unprepare()
        {
            if (_statements.All(s => !s.IsPrepared))
                return;

            var connector = CheckReadyAndGetConnector();
            Log.Debug("Closing command's prepared statements", connector.Id);
            using (connector.StartUserAction())
            {
                var sendTask = SendClose(connector, false);
                foreach (var statement in _statements)
                    if (statement.PreparedStatement?.State == PreparedState.BeingUnprepared)
                    {
                        Expect<CloseCompletedMessage>(connector.ReadMessage(), connector);
                        statement.PreparedStatement.CompleteUnprepare();
                        statement.PreparedStatement = null;
                    }
                Expect<ReadyForQueryMessage>(connector.ReadMessage(), connector);
                sendTask.GetAwaiter().GetResult();
            }
        }

        #endregion Prepare

        #region Query analysis

        void ProcessRawQuery(bool deriveParameters = false)
        {
            if (string.IsNullOrEmpty(CommandText))
                throw new InvalidOperationException("CommandText property has not been initialized");

            NpgsqlStatement statement;
            switch (CommandType) {
            case CommandType.Text:
                _connection!.Connector!.SqlParser.ParseRawQuery(CommandText, _parameters, _statements, deriveParameters);
                if (_statements.Count > 1 && _parameters.HasOutputParameters)
                    throw new NotSupportedException("Commands with multiple queries cannot have out parameters");
                break;

            case CommandType.TableDirect:
                if (_statements.Count == 0)
                    statement = new NpgsqlStatement();
                else
                {
                    statement = _statements[0];
                    statement.Reset();
                    _statements.Clear();
                }
                _statements.Add(statement);
                statement.SQL = "SELECT * FROM " + CommandText;
                break;

            case CommandType.StoredProcedure:
                var inputList = _parameters.Where(p => p.IsInputDirection).ToList();
                var numInput = inputList.Count;
                var sb = new StringBuilder();
                sb.Append("SELECT * FROM ");
                sb.Append(CommandText);
                sb.Append('(');
                var hasWrittenFirst = false;
                for (var i = 1; i <= numInput; i++) {
                    var param = inputList[i - 1];
                    if (param.TrimmedName == "")
                    {
                        if (hasWrittenFirst)
                            sb.Append(',');
                        sb.Append('$');
                        sb.Append(i);
                        hasWrittenFirst = true;
                    }
                }
                for (var i = 1; i <= numInput; i++)
                {
                    var param = inputList[i - 1];
                    if (param.TrimmedName != "")
                    {
                        if (hasWrittenFirst)
                            sb.Append(',');
                        sb.Append('"');
                        sb.Append(param.TrimmedName.Replace("\"", "\"\""));
                        sb.Append("\" := ");
                        sb.Append('$');
                        sb.Append(i);
                        hasWrittenFirst = true;
                    }
                }
                sb.Append(')');

                if (_statements.Count == 0)
                    statement = new NpgsqlStatement();
                else
                {
                    statement = _statements[0];
                    statement.Reset();
                    _statements.Clear();
                }
                statement.SQL = sb.ToString();
                statement.InputParameters.AddRange(inputList);
                _statements.Add(statement);
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {CommandType} of enum {nameof(CommandType)}. Please file a bug.");
            }

            foreach (var s in _statements)
                if (s.InputParameters.Count > ushort.MaxValue)
                    throw new NpgsqlException($"A statement cannot have more than {ushort.MaxValue} parameters");
        }

        #endregion

        #region Execute

        void ValidateParameters(ConnectorTypeMapper typeMapper)
        {
            for (var i = 0; i < Parameters.Count; i++)
            {
                var p = Parameters[i];
                if (!p.IsInputDirection)
                    continue;
                p.Bind(typeMapper);
                p.LengthCache?.Clear();
                p.ValidateAndGetLength();
            }
        }

        #endregion

        #region Message Creation / Population

        internal bool FlushOccurred { get; set; }

        void BeginSend()
        {
            _connection!.Connector!.WriteBuffer.CurrentCommand = this;
            FlushOccurred = false;
        }

        void CleanupSend()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (SynchronizationContext.Current != null)  // Check first because SetSynchronizationContext allocates
                SynchronizationContext.SetSynchronizationContext(null);
        }

        async Task SendExecute(NpgsqlConnector connector, bool async)
        {
            BeginSend();

            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];
                var pStatement = statement.PreparedStatement;

                if (pStatement == null || pStatement.State == PreparedState.ToBePrepared)
                {
                    // We may have a prepared statement that replaces an existing statement - close the latter first.
                    if (pStatement?.StatementBeingReplaced != null)
                        await connector.WriteClose(StatementOrPortal.Statement, pStatement.StatementBeingReplaced.Name!, async);

                    await connector.WriteParse(statement.SQL, statement.StatementName, statement.InputParameters, async);
                }

                await connector.WriteBind(
                    statement.InputParameters, string.Empty, statement.StatementName, AllResultTypesAreUnknown,
                    i == 0 ? UnknownResultTypeList : null,
                    async);

                if (pStatement == null || pStatement.State == PreparedState.ToBePrepared)
                {
                    await connector.WriteDescribe(StatementOrPortal.Portal, string.Empty, async);
                    if (statement.PreparedStatement != null)
                        statement.PreparedStatement.State = PreparedState.BeingPrepared;
                }

                await connector.WriteExecute(0, async);

                if (pStatement != null)
                    pStatement.LastUsed = DateTime.UtcNow;
            }

            await connector.WriteSync(async);
            await connector.Flush(async);

            CleanupSend();
        }

        async Task SendExecuteSchemaOnly(NpgsqlConnector connector, bool async)
        {
            BeginSend();

            var wroteSomething = false;
            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];

                if (statement.PreparedStatement?.State == PreparedState.Prepared)
                    continue;   // Prepared, we already have the RowDescription
                Debug.Assert(statement.PreparedStatement == null);

                await connector.WriteParse(statement.SQL, string.Empty, statement.InputParameters, async);
                await connector.WriteDescribe(StatementOrPortal.Statement, statement.StatementName, async);
                wroteSomething = true;
            }

            if (wroteSomething)
            {
                await connector.WriteSync(async);
                await connector.Flush(async);
            }

            CleanupSend();
        }

        async Task SendDeriveParameters(NpgsqlConnector connector, bool async)
        {
            BeginSend();

            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];

                await connector.WriteParse(statement.SQL, string.Empty, EmptyParameters, async);
                await connector.WriteDescribe(StatementOrPortal.Statement, string.Empty, async);
            }

            await connector.WriteSync(async);
            await connector.Flush(async);

            CleanupSend();
        }

        async Task SendPrepare(NpgsqlConnector connector, bool async)
        {
            BeginSend();

            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];
                var pStatement = statement.PreparedStatement;

                // A statement may be already prepared, already in preparation (i.e. same statement twice
                // in the same command), or we can't prepare (overloaded SQL)
                if (pStatement?.State != PreparedState.ToBePrepared)
                    continue;

                // We may have a prepared statement that replaces an existing statement - close the latter first.
                var statementToClose = pStatement.StatementBeingReplaced;
                if (statementToClose != null)
                    await connector.WriteClose(StatementOrPortal.Statement, statementToClose.Name!, async);

                await connector.WriteParse(statement.SQL, pStatement.Name!, statement.InputParameters, async);
                await connector.WriteDescribe(StatementOrPortal.Statement, pStatement.Name!, async);

                pStatement.State = PreparedState.BeingPrepared;
            }

            await connector.WriteSync(async);
            await connector.Flush(async);

            CleanupSend();
        }

        bool ForceAsyncIfNecessary(bool async, int numberOfStatementInBatch)
        {
            if (!async && FlushOccurred && numberOfStatementInBatch > 0)
            {
                // We're synchronously sending the non-first statement in a batch and a flush
                // has already occured. Switch to async. See long comment in Execute() above.
                async = true;
                SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
            }

            return async;
        }

        async Task SendClose(NpgsqlConnector connector, bool async)
        {
            BeginSend();

            foreach (var statement in _statements.Where(s => s.IsPrepared))
            {
                if (FlushOccurred)
                {
                    async = true;
                    SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
                }

                await connector.WriteClose(StatementOrPortal.Statement, statement.StatementName, async);
                statement.PreparedStatement!.State = PreparedState.BeingUnprepared;
            }

            await connector.WriteSync(async);
            await connector.Flush(async);

            CleanupSend();
        }

        #endregion

        #region Execute Non Query

        /// <summary>
        /// Executes a SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected if known; -1 otherwise.</returns>
        public override int ExecuteNonQuery() => ExecuteNonQuery(false, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous version of <see cref="ExecuteNonQuery()"/>
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the number of rows affected if known; -1 otherwise.</returns>
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ExecuteNonQuery(true, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task<int> ExecuteNonQuery(bool async, CancellationToken cancellationToken)
        {
            using var reader = await ExecuteReaderAsync(CommandBehavior.Default, async, cancellationToken);
            while (async ? await reader.NextResultAsync(cancellationToken) : reader.NextResult()) ;

            reader.Close();

            return reader.RecordsAffected;
        }

        #endregion Execute Non Query

        #region Execute Scalar

#nullable disable

        /// <summary>
        /// Executes the query, and returns the first column of the first row
        /// in the result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <returns>The first column of the first row in the result set,
        /// or a null reference if the result set is empty.</returns>
        public override object ExecuteScalar() => ExecuteScalar(false, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous version of <see cref="ExecuteScalar()"/>
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the first column of the
        /// first row in the result set, or a null reference if the result set is empty.</returns>
        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<object>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ExecuteScalar(true, cancellationToken).AsTask();
        }

#nullable restore

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async ValueTask<object?> ExecuteScalar(bool async, CancellationToken cancellationToken)
        {
            var behavior = CommandBehavior.SingleRow;
            if (!Parameters.HasOutputParameters)
                behavior |= CommandBehavior.SequentialAccess;

            using var reader = await ExecuteReaderAsync(behavior, async, cancellationToken);
            return reader.Read() && reader.FieldCount != 0 ? reader.GetValue(0) : null;
        }

        #endregion Execute Scalar

        #region Execute Reader

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => ExecuteReader(behavior);

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">An instance of <see cref="CommandBehavior"/>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
            => await ExecuteReaderAsync(behavior, cancellationToken);

        /// <summary>
        /// Executes the <see cref="CommandText"/> against the <see cref="Connection"/>
        /// and returns a <see cref="NpgsqlDataReader"/>.
        /// </summary>
        /// <param name="behavior">One of the enumeration values that specified the command behavior.</param>
        /// <returns>A task representing the operation.</returns>
        public new NpgsqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
            => ExecuteReaderAsync(behavior, async: false, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// An asynchronous version of <see cref="ExecuteReader"/>, which executes
        /// the <see cref="CommandText"/> against the <see cref="Connection"/>
        /// and returns a <see cref="NpgsqlDataReader"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public new Task<NpgsqlDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
            => ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);

        /// <summary>
        /// An asynchronous version of <see cref="ExecuteReader(CommandBehavior)"/>,
        /// which executes the <see cref="CommandText"/> against the <see cref="Connection"/>
        /// and returns a <see cref="NpgsqlDataReader"/>.
        /// </summary>
        /// <param name="behavior">One of the enumeration values that specified the command behavior.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public new Task<NpgsqlDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<NpgsqlDataReader>(cancellationToken);

            using (NoSynchronizationContextScope.Enter())
                return ExecuteReaderAsync(behavior, async: true, cancellationToken).AsTask();
        }

        async ValueTask<NpgsqlDataReader> ExecuteReaderAsync(CommandBehavior behavior, bool async, CancellationToken cancellationToken)
        {
            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(this);
            try
            {
                using (cancellationToken.Register(cmd => ((NpgsqlCommand)cmd!).Cancel(), this))
                {
                    ValidateParameters(connector.TypeMapper);

                    switch (IsExplicitlyPrepared)
                    {
                    case true:
                        Debug.Assert(_connectorPreparedOn != null);
                        if (_connectorPreparedOn != connector)
                        {
                            // The command was prepared, but since then the connector has changed. Detach all prepared statements.
                            foreach (var s in _statements)
                                s.PreparedStatement = null;
                            ResetExplicitPreparation();
                            goto case false;
                        }
                        NpgsqlEventSource.Log.CommandStartPrepared();
                        break;

                    case false:
                        ProcessRawQuery();

                        if (connector.Settings.MaxAutoPrepare > 0)
                        {
                            var numPrepared = 0;
                            foreach (var statement in _statements)
                            {
                                // If this statement isn't prepared, see if it gets implicitly prepared.
                                // Note that this may return null (not enough usages for automatic preparation).
                                if (!statement.IsPrepared)
                                    statement.PreparedStatement = connector.PreparedStatementManager.TryGetAutoPrepared(statement);
                                if (statement.PreparedStatement != null)
                                    numPrepared++;
                            }

                            if (numPrepared > 0)
                            {
                                _connectorPreparedOn = connector;
                                if (numPrepared == _statements.Count)
                                    NpgsqlEventSource.Log.CommandStartPrepared();
                            }
                        }
                        break;
                    }

                    State = CommandState.InProgress;

                    if (Log.IsEnabled(NpgsqlLogLevel.Debug))
                        LogCommand(connector.Id);
                    NpgsqlEventSource.Log.CommandStart(CommandText);
                    Task sendTask;

                    // If a cancellation is in progress, wait for it to "complete" before proceeding (#615)
                    lock (connector.CancelLock) { }

                    connector.UserTimeout = CommandTimeout * 1000;

                    // We do not wait for the entire send to complete before proceeding to reading -
                    // the sending continues in parallel with the user's reading. Waiting for the
                    // entire send to complete would trigger a deadlock for multi-statement commands,
                    // where PostgreSQL sends large results for the first statement, while we're sending large
                    // parameter data for the second. See #641.
                    // Instead, all sends for non-first statements and for non-first buffers are performed
                    // asynchronously (even if the user requested sync), in a special synchronization context
                    // to prevents a dependency on the thread pool (which would also trigger deadlocks).
                    // The WriteBuffer notifies this command when the first buffer flush occurs, so that the
                    // send functions can switch to the special async mode when needed.
                    sendTask = (behavior & CommandBehavior.SchemaOnly) == 0
                        ? SendExecute(connector, async)
                        : SendExecuteSchemaOnly(connector, async);

                    // The following is a hack. It raises an exception if one was thrown in the first phases
                    // of the send (i.e. in parts of the send that executed synchronously). Exceptions may
                    // still happen later and aren't properly handled. See #1323.
                    if (sendTask.IsFaulted)
                        sendTask.GetAwaiter().GetResult();

                    var reader = connector.DataReader;
                    reader.Init(this, behavior, _statements, sendTask);
                    connector.CurrentReader = reader;
                    if (async)
                        await reader.NextResultAsync(cancellationToken);
                    else
                        reader.NextResult();
                    return reader;
                }
            }
            catch
            {
                State = CommandState.Idle;
                _connection!.Connector?.EndUserAction();

                // Close connection if requested even when there is an error.
                if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                    _connection.Close();
                throw;
            }
        }

        #endregion

        #region Transactions

        /// <summary>
        /// DB transaction.
        /// </summary>
        protected override DbTransaction? DbTransaction
        {
            get => Transaction;
            set => Transaction = (NpgsqlTransaction?)value;
        }
        /// <summary>
        /// This property is ignored by Npgsql. PostgreSQL only supports a single transaction at a given time on
        /// a given connection, and all commands implicitly run inside the current transaction started via
        /// <see cref="NpgsqlConnection.BeginTransaction()"/>
        /// </summary>
        public new NpgsqlTransaction? Transaction { get; set; }

        #endregion Transactions

        #region Cancel

        /// <summary>
        /// Attempts to cancel the execution of a <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <remarks>As per the specs, no exception will be thrown by this method in case of failure</remarks>
        public override void Cancel()
        {
            var connector = _connection?.Connector;
            if (connector == null)
                return;

            if (State != CommandState.InProgress)
                return;

            connector.CancelRequest();
        }

        #endregion Cancel

        #region Dispose

        /// <summary>
        /// Releases the resources used by the <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (State == CommandState.Disposed)
                return;
            Transaction = null;
            _connection = null;
            State = CommandState.Disposed;
            base.Dispose(disposing);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Fixes up the text/binary flag on result columns.
        /// Since Prepare() describes a statement rather than a portal, the resulting RowDescription
        /// will have text format on all result columns. Fix that up.
        /// </summary>
        /// <remarks>
        /// Note that UnknownResultTypeList only applies to the first query, while AllResultTypesAreUnknown applies
        /// to all of them.
        /// </remarks>
        internal void FixupRowDescription(RowDescriptionMessage rowDescription, bool isFirst)
        {
            for (var i = 0; i < rowDescription.NumFields; i++)
                rowDescription[i].FormatCode = (UnknownResultTypeList == null || !isFirst ? AllResultTypesAreUnknown : UnknownResultTypeList[i]) ? FormatCode.Text : FormatCode.Binary;
        }

        void LogCommand(int connectorId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Executing statement(s):");
            foreach (var s in _statements)
            {
                sb.Append("\t").AppendLine(s.SQL);
                var p = s.InputParameters;
                if (NpgsqlLogManager.IsParameterLoggingEnabled && p.Count > 0)
                {
                    sb.Append('\t').Append("Parameters:");
                    for (var i = 0; i < p.Count; i++)
                        sb.Append("\t$").Append(i + 1).Append(": ").Append(Convert.ToString(p[i].Value, CultureInfo.InvariantCulture));
                }
            }

            Log.Debug(sb.ToString(), connectorId);
        }

        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        public NpgsqlCommand Clone()
        {
            var clone = new NpgsqlCommand(CommandText, _connection, Transaction)
            {
                CommandTimeout = CommandTimeout, CommandType = CommandType, DesignTimeVisible = DesignTimeVisible, _allResultTypesAreUnknown = _allResultTypesAreUnknown, _unknownResultTypeList = _unknownResultTypeList, ObjectResultTypes = ObjectResultTypes
            };
            _parameters.CloneTo(clone._parameters);
            return clone;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        NpgsqlConnector CheckReadyAndGetConnector()
        {
            if (State == CommandState.Disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (_connection == null)
                throw new InvalidOperationException("Connection property has not been initialized.");
            return _connection.CheckReadyAndGetConnector();
        }

        #endregion
    }

    enum CommandState
    {
        Idle,
        InProgress,
        Disposed
    }
}
