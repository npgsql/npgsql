using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Util.Statics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Npgsql.Properties;
using System.Collections;

namespace Npgsql;

/// <summary>
/// Represents a SQL statement or function (stored procedure) to execute
/// against a PostgreSQL database. This class cannot be inherited.
/// </summary>
// ReSharper disable once RedundantNameQualifier
[System.ComponentModel.DesignerCategory("")]
public class NpgsqlCommand : DbCommand, ICloneable, IComponent
{
    #region Fields

    NpgsqlTransaction? _transaction;

    readonly NpgsqlConnector? _connector;

    /// <summary>
    /// If this command is (explicitly) prepared, references the connector on which the preparation happened.
    /// Used to detect when the connector was changed (i.e. connection open/close), meaning that the command
    /// is no longer prepared.
    /// </summary>
    NpgsqlConnector? _connectorPreparedOn;

    string _commandText;
    CommandBehavior _behavior;
    int? _timeout;
    internal NpgsqlParameterCollection? _parameters;

    internal NpgsqlBatch? WrappingBatch { get; }

    internal List<NpgsqlBatchCommand> InternalBatchCommands { get; }

    internal Activity? CurrentActivity { get; private set; }

    /// <summary>
    /// Returns details about each statement that this command has executed.
    /// Is only populated when an Execute* method is called.
    /// </summary>
    [Obsolete("Use the new DbBatch API")]
    public IReadOnlyList<NpgsqlBatchCommand> Statements => InternalBatchCommands.AsReadOnly();

    UpdateRowSource _updateRowSource = UpdateRowSource.Both;

    bool IsExplicitlyPrepared => _connectorPreparedOn != null;

    /// <summary>
    /// Whether this command is cached by <see cref="NpgsqlConnection" /> and returned by <see cref="NpgsqlConnection.CreateCommand" />.
    /// </summary>
    internal bool IsCacheable { get; set; }

#if DEBUG
    internal static bool EnableSqlRewriting;
    internal static bool EnableStoredProcedureCompatMode;
#else
    internal static readonly bool EnableSqlRewriting;
    internal static readonly bool EnableStoredProcedureCompatMode;
#endif

    internal bool EnableErrorBarriers { get; set; }

    static readonly TaskScheduler ConstrainedConcurrencyScheduler =
        new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, Math.Max(1, Environment.ProcessorCount / 2)).ConcurrentScheduler;

    #endregion Fields

    #region Constants

    internal const int DefaultTimeout = 30;

    #endregion

    #region Constructors

    static NpgsqlCommand()
    {
        EnableSqlRewriting = !AppContext.TryGetSwitch("Npgsql.EnableSqlRewriting", out var enabled) || enabled;
        EnableStoredProcedureCompatMode = AppContext.TryGetSwitch("Npgsql.EnableStoredProcedureCompatMode", out enabled) && enabled;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlCommand"/> class.
    /// </summary>
    public NpgsqlCommand() : this(null, null, null) {}

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlCommand"/> class with the text of the query.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    // ReSharper disable once IntroduceOptionalParameters.Global
    public NpgsqlCommand(string? cmdText) : this(cmdText, null, null) {}

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlCommand"/> class with the text of the query and a
    /// <see cref="NpgsqlConnection"/>.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="connection">A <see cref="NpgsqlConnection"/> that represents the connection to a PostgreSQL server.</param>
    // ReSharper disable once IntroduceOptionalParameters.Global
    public NpgsqlCommand(string? cmdText, NpgsqlConnection? connection)
    {
        GC.SuppressFinalize(this);
        InternalBatchCommands = new List<NpgsqlBatchCommand>(1);
        _commandText = cmdText ?? string.Empty;
        InternalConnection = connection;
        CommandType = CommandType.Text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlCommand"/> class with the text of the query, a
    /// <see cref="NpgsqlConnection"/>, and the <see cref="NpgsqlTransaction"/>.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="connection">A <see cref="NpgsqlConnection"/> that represents the connection to a PostgreSQL server.</param>
    /// <param name="transaction">The <see cref="NpgsqlTransaction"/> in which the <see cref="NpgsqlCommand"/> executes.</param>
    public NpgsqlCommand(string? cmdText, NpgsqlConnection? connection, NpgsqlTransaction? transaction)
        : this(cmdText, connection)
        => Transaction = transaction;

    /// <summary>
    /// Used when this <see cref="NpgsqlCommand"/> instance is wrapped inside an <see cref="NpgsqlBatch"/>.
    /// </summary>
    internal NpgsqlCommand(NpgsqlBatch batch, int batchCommandCapacity, NpgsqlConnection? connection = null)
    {
        GC.SuppressFinalize(this);
        InternalBatchCommands = new List<NpgsqlBatchCommand>(batchCommandCapacity);
        InternalConnection = connection;
        CommandType = CommandType.Text;
        WrappingBatch = batch;

        // These can/should never be used in this mode
        _commandText = null!;
        _parameters = null!;
    }

    internal NpgsqlCommand(string? cmdText, NpgsqlConnector connector) : this(cmdText)
        => _connector = connector;

    /// <summary>
    /// Used when this <see cref="NpgsqlCommand"/> instance is wrapped inside an <see cref="NpgsqlBatch"/>.
    /// </summary>
    internal NpgsqlCommand(NpgsqlBatch batch, NpgsqlConnector connector, int batchCommandCapacity)
        : this(batch, batchCommandCapacity)
        => _connector = connector;

    internal static NpgsqlCommand CreateCachedCommand(NpgsqlConnection connection)
        => new(null, connection) { IsCacheable = true };

    #endregion Constructors

    #region Public properties

    /// <summary>
    /// Gets or sets the SQL statement or function (stored procedure) to execute at the data source.
    /// </summary>
    /// <value>The SQL statement or function (stored procedure) to execute. The default is an empty string.</value>
    [AllowNull, DefaultValue("")]
    [Category("Data")]
    public override string CommandText
    {
        get => _commandText;
        set
        {
            Debug.Assert(WrappingBatch is null);

            if (State != CommandState.Idle)
                ThrowHelper.ThrowInvalidOperationException("An open data reader exists for this command.");

            _commandText = value ?? string.Empty;

            ResetPreparation();
            // TODO: Technically should do this also if the parameter list (or type) changes
        }
    }

    string GetBatchFullCommandText()
    {
        Debug.Assert(WrappingBatch is not null);
        if (InternalBatchCommands.Count == 0)
            return string.Empty;
        if (InternalBatchCommands.Count == 1)
            return InternalBatchCommands[0].CommandText;
        // TODO: Potentially cache on connector/command?
        var sb = new StringBuilder();
        sb.Append(InternalBatchCommands[0].CommandText);
        for (var i = 1; i < InternalBatchCommands.Count; i++)
        {
            sb
                .Append(';')
                .AppendLine()
                .Append(InternalBatchCommands[i].CommandText);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets or sets the wait time (in seconds) before terminating the attempt  to execute a command and generating an error.
    /// </summary>
    /// <value>The time (in seconds) to wait for the command to execute. The default value is 30 seconds.</value>
    [DefaultValue(DefaultTimeout)]
    public override int CommandTimeout
    {
        get => _timeout ?? (InternalConnection?.CommandTimeout ?? DefaultTimeout);
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            _timeout = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating how the <see cref="NpgsqlCommand.CommandText"/> property is to be interpreted.
    /// </summary>
    /// <value>
    /// One of the <see cref="System.Data.CommandType"/> values. The default is <see cref="System.Data.CommandType.Text"/>.
    /// </value>
    [DefaultValue(CommandType.Text)]
    [Category("Data")]
    public override CommandType CommandType { get; set; }

    internal NpgsqlConnection? InternalConnection { get; private set; }

    /// <summary>
    /// DB connection.
    /// </summary>
    protected override DbConnection? DbConnection
    {
        get => InternalConnection;
        set
        {
            if (InternalConnection == value)
                return;

            InternalConnection = State == CommandState.Idle
                ? (NpgsqlConnection?)value
                : throw new InvalidOperationException("An open data reader exists for this command.");

            Transaction = null;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="NpgsqlConnection"/> used by this instance of the <see cref="NpgsqlCommand"/>.
    /// </summary>
    /// <value>The connection to a data source. The default value is <see langword="null"/>.</value>
    [DefaultValue(null)]
    [Category("Behavior")]
    public new NpgsqlConnection? Connection
    {
        get => (NpgsqlConnection?)DbConnection;
        set => DbConnection = value;
    }

    /// <summary>
    /// Design time visible.
    /// </summary>
    public override bool DesignTimeVisible { get; set; }

    /// <summary>
    /// Gets or sets how command results are applied to the DataRow when used by the
    /// DbDataAdapter.Update(DataSet) method.
    /// </summary>
    /// <value>One of the <see cref="System.Data.UpdateRowSource"/> values.</value>
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
    public bool IsPrepared
    {
        get
        {
            return _connectorPreparedOn == (InternalConnection?.Connector ?? _connector) && AllPrepared();

            bool AllPrepared()
            {
                if (InternalBatchCommands.Count is 0)
                    return false;

                foreach (var s in InternalBatchCommands)
                    if (s.PreparedStatement is null || !s.PreparedStatement.IsPrepared)
                        return false;
                return true;
            }
        }
    }

    #endregion Public properties

    #region Known/unknown Result Types Management

    /// <summary>
    /// Marks all of the query's result columns as either known or unknown.
    /// Unknown result columns are requested from PostgreSQL in text format, and Npgsql makes no
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
    /// Unknown result columns are requested from PostgreSQL in text format, and Npgsql makes no
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

    #region State management

    volatile int _state;

    /// <summary>
    /// The current state of the command
    /// </summary>
    internal CommandState State
    {
        get => (CommandState)_state;
        set
        {
            var newState = (int)value;
            if (newState == _state)
                return;
            _state = newState;
        }
    }

    internal void ResetPreparation() => _connectorPreparedOn = null;

    #endregion State management

    #region Parameters

    /// <summary>
    /// Creates a new instance of an <see cref="System.Data.Common.DbParameter"/> object.
    /// </summary>
    /// <returns>A <see cref="System.Data.Common.DbParameter"/> object.</returns>
    protected override DbParameter CreateDbParameter() => CreateParameter();

    /// <summary>
    /// Creates a new instance of a <see cref="NpgsqlParameter"/> object.
    /// </summary>
    /// <returns>An <see cref="NpgsqlParameter"/> object.</returns>
    public new NpgsqlParameter CreateParameter() => new();

    /// <summary>
    /// DB parameter collection.
    /// </summary>
    protected override DbParameterCollection DbParameterCollection => Parameters;

    /// <summary>
    /// Gets the <see cref="NpgsqlParameterCollection"/>.
    /// </summary>
    /// <value>The parameters of the SQL statement or function (stored procedure). The default is an empty collection.</value>
    public new NpgsqlParameterCollection Parameters => _parameters ??= [];

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
        var conn = CheckAndGetConnection();
        Debug.Assert(conn is not null);

        if (string.IsNullOrEmpty(CommandText))
            throw new InvalidOperationException("CommandText property has not been initialized");

        using var _ = conn.StartTemporaryBindingScope(out var connector);

        foreach (var s in InternalBatchCommands)
            if (s.PreparedStatement?.IsExplicit == true)
                throw new NpgsqlException("Deriving parameters isn't supported for commands that are already prepared.");

        // Here we unprepare statements that possibly are auto-prepared
        Unprepare();

        Parameters.Clear();

        switch (CommandType)
        {
        case CommandType.Text:
            DeriveParametersForQuery(connector);
            break;
        case CommandType.StoredProcedure:
            DeriveParametersForFunction();
            break;
        default:
            throw new NotSupportedException("Cannot derive parameters for CommandType " + CommandType);
        }
    }

    void DeriveParametersForFunction()
    {
        using var c = new NpgsqlCommand(DeriveParametersForFunctionQuery, InternalConnection);
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

        var serializerOptions = c.InternalConnection!.Connector!.SerializerOptions;

        for (var i = 0; i < types.Length; i++)
        {
            var param = new NpgsqlParameter();

            var postgresType = serializerOptions.DatabaseInfo.GetPostgresType(types[i]);
            var npgsqlDbType = postgresType.DataTypeName.ToNpgsqlDbType();
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

    void DeriveParametersForQuery(NpgsqlConnector connector)
    {
        using (connector.StartUserAction())
        {
            LogMessages.DerivingParameters(connector.CommandLogger, CommandText, connector.Id);

            if (WrappingBatch is not null)
                foreach (var batchCommand in InternalBatchCommands)
                    connector.SqlQueryParser.ParseRawQuery(batchCommand, connector.UseConformingStrings, deriveParameters: true);
            else
                connector.SqlQueryParser.ParseRawQuery(this, connector.UseConformingStrings, deriveParameters: true);

            var sendTask = SendDeriveParameters(connector, false);
            if (sendTask.IsFaulted)
                sendTask.GetAwaiter().GetResult();

            try
            {
                foreach (var batchCommand in InternalBatchCommands)
                {
                    Expect<ParseCompleteMessage>(
                        connector.ReadMessage(async: false).GetAwaiter().GetResult(), connector);
                    var paramTypeOIDs = Expect<ParameterDescriptionMessage>(
                        connector.ReadMessage(async: false).GetAwaiter().GetResult(), connector).TypeOIDs;

                    if (batchCommand.PositionalParameters.Count != paramTypeOIDs.Count)
                    {
                        connector.SkipUntil(BackendMessageCode.ReadyForQuery);
                        Parameters.Clear();
                        throw new NpgsqlException(
                            "There was a mismatch in the number of derived parameters between the Npgsql SQL parser and the PostgreSQL parser. Please report this as bug to the Npgsql developers (https://github.com/npgsql/npgsql/issues).");
                    }

                    for (var i = 0; i < paramTypeOIDs.Count; i++)
                    {
                        try
                        {
                            var param = batchCommand.PositionalParameters[i];
                            var paramOid = paramTypeOIDs[i];

                            var postgresType = connector.SerializerOptions.DatabaseInfo.GetPostgresType(paramOid);
                            // We want to keep any domain types visible on the parameter, it will internally do a representational lookup again if necessary.
                            var npgsqlDbType = postgresType.GetRepresentationalType().DataTypeName.ToNpgsqlDbType();
                            if (param.NpgsqlDbType != NpgsqlDbType.Unknown && param.NpgsqlDbType != npgsqlDbType)
                                throw new NpgsqlException(
                                    "The backend parser inferred different types for parameters with the same name. Please try explicit casting within your SQL statement or batch or use different placeholder names.");

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

                    var msg = connector.ReadMessage(async: false).GetAwaiter().GetResult();
                    switch (msg.Code)
                    {
                    case BackendMessageCode.RowDescription:
                    case BackendMessageCode.NoData:
                        break;
                    default:
                        throw connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                Expect<ReadyForQueryMessage>(connector.ReadMessage(async: false).GetAwaiter().GetResult(), connector);
            }
            finally
            {
                try
                {
                    // Make sure sendTask is complete so we don't race against asynchronous flush
                    sendTask.GetAwaiter().GetResult();
                }
                catch
                {
                    // ignored
                }
            }
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    public override Task PrepareAsync(CancellationToken cancellationToken = default)
        => Prepare(async: true, cancellationToken);

    Task Prepare(bool async, CancellationToken cancellationToken = default)
    {
        var connection = CheckAndGetConnection();
        Debug.Assert(connection is not null);
        if (connection.Settings.Multiplexing)
            throw new NotSupportedException("Explicit preparation not supported with multiplexing");
        var connector = connection.Connector!;
        var logger = connector.CommandLogger;

        var needToPrepare = false;

        if (WrappingBatch is not null)
        {
            foreach (var batchCommand in InternalBatchCommands)
            {
                batchCommand._parameters?.ProcessParameters(connector.SerializerOptions, validateValues: false, batchCommand.CommandType);
                ProcessRawQuery(connector.SqlQueryParser, connector.UseConformingStrings, batchCommand);

                needToPrepare = batchCommand.ExplicitPrepare(connector) || needToPrepare;
                batchCommand.ConnectorPreparedOn = connector;
            }

            if (logger.IsEnabled(LogLevel.Debug) && needToPrepare)
                LogMessages.PreparingCommandExplicitly(logger, string.Join("; ", CommandTexts()), connector.Id);

            IEnumerable<string> CommandTexts()
            {
                foreach (var c in InternalBatchCommands)
                    yield return c.CommandText;
            }
        }
        else
        {
            _parameters?.ProcessParameters(connector.SerializerOptions, validateValues: false, CommandType);
            ProcessRawQuery(connector.SqlQueryParser, connector.UseConformingStrings, batchCommand: null);

            foreach (var batchCommand in InternalBatchCommands)
                needToPrepare = batchCommand.ExplicitPrepare(connector) || needToPrepare;

            if (logger.IsEnabled(LogLevel.Debug) && needToPrepare)
                LogMessages.PreparingCommandExplicitly(logger, CommandText, connector.Id);
        }

        _connectorPreparedOn = connector;

        // It's possible the command was already prepared, or that persistent prepared statements were found for
        // all statements. Nothing to do here, move along.
        return needToPrepare
            ? PrepareLong(this, async, connector, cancellationToken)
            : Task.CompletedTask;

        static async Task PrepareLong(NpgsqlCommand command, bool async, NpgsqlConnector connector, CancellationToken cancellationToken)
        {
            try
            {
                using (connector.StartUserAction(cancellationToken))
                {
                    var sendTask = command.SendPrepare(connector, async, CancellationToken.None);
                    if (sendTask.IsFaulted)
                        sendTask.GetAwaiter().GetResult();

                    try
                    {
                        // Loop over statements, skipping those that are already prepared (because they were persisted)
                        var isFirst = true;
                        foreach (var batchCommand in command.InternalBatchCommands)
                        {
                            if (!batchCommand.IsPreparing)
                                continue;

                            var pStatement = batchCommand.PreparedStatement!;
                            var replacedStatement = pStatement.StatementBeingReplaced;

                            if (replacedStatement != null)
                            {
                                Expect<CloseCompletedMessage>(await connector.ReadMessage(async).ConfigureAwait(false), connector);
                                replacedStatement.CompleteUnprepare();

                                if (!replacedStatement.IsExplicit)
                                    connector.PreparedStatementManager.AutoPrepared[replacedStatement.AutoPreparedSlotIndex] = null;

                                pStatement.StatementBeingReplaced = null;
                            }

                            Expect<ParseCompleteMessage>(await connector.ReadMessage(async).ConfigureAwait(false), connector);
                            Expect<ParameterDescriptionMessage>(await connector.ReadMessage(async).ConfigureAwait(false), connector);
                            var msg = await connector.ReadMessage(async).ConfigureAwait(false);
                            switch (msg.Code)
                            {
                            case BackendMessageCode.RowDescription:
                                // Clone the RowDescription for use with the prepared statement (the one we have is reused
                                // by the connection)
                                var description = ((RowDescriptionMessage)msg).Clone();
                                command.FixupRowDescription(description, isFirst);
                                batchCommand.Description = description;
                                break;
                            case BackendMessageCode.NoData:
                                batchCommand.Description = null;
                                break;
                            default:
                                throw connector.UnexpectedMessageReceived(msg.Code);
                            }

                            pStatement.State = PreparedState.Prepared;
                            connector.PreparedStatementManager.NumPrepared++;
                            batchCommand.IsPreparing = false;
                            isFirst = false;
                        }

                        Expect<ReadyForQueryMessage>(await connector.ReadMessage(async).ConfigureAwait(false), connector);
                    }
                    finally
                    {
                        try
                        {
                            // Make sure sendTask is complete so we don't race against asynchronous flush
                            if (async)
                                await sendTask.ConfigureAwait(false);
                            else
                                sendTask.GetAwaiter().GetResult();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                LogMessages.CommandPreparedExplicitly(connector.CommandLogger, connector.Id);
            }
            catch
            {
                // The statements weren't prepared successfully, update the bookkeeping for them
                foreach (var batchCommand in command.InternalBatchCommands)
                {
                    if (batchCommand.IsPreparing)
                    {
                        batchCommand.IsPreparing = false;
                        batchCommand.PreparedStatement!.AbortPrepare();
                    }
                }

                throw;
            }
        }
    }

    /// <summary>
    /// Unprepares a command, closing server-side statements associated with it.
    /// Note that this only affects commands explicitly prepared with <see cref="Prepare()"/>, not
    /// automatically prepared statements.
    /// </summary>
    public void Unprepare()
        => Unprepare(false).GetAwaiter().GetResult();

    /// <summary>
    /// Unprepares a command, closing server-side statements associated with it.
    /// Note that this only affects commands explicitly prepared with <see cref="Prepare()"/>, not
    /// automatically prepared statements.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    public Task UnprepareAsync(CancellationToken cancellationToken = default)
        => Unprepare(async: true, cancellationToken);

    async Task Unprepare(bool async, CancellationToken cancellationToken = default)
    {
        var connection = CheckAndGetConnection();
        Debug.Assert(connection is not null);
        if (connection.Settings.Multiplexing)
            throw new NotSupportedException("Explicit preparation not supported with multiplexing");

        var forall = true;
        foreach (var statement in InternalBatchCommands)
            if (statement.IsPrepared)
            {
                forall = false;
                break;
            }
        if (forall)
            return;

        var connector = connection.Connector!;

        LogMessages.UnpreparingCommand(connector.CommandLogger, connector.Id);

        using (connector.StartUserAction(cancellationToken))
        {
            // Just wait for SendClose to complete since each statement takes no more than 20 bytes
            await SendClose(connector, async, cancellationToken).ConfigureAwait(false);

            foreach (var batchCommand in InternalBatchCommands)
            {
                if (batchCommand.PreparedStatement?.State == PreparedState.BeingUnprepared)
                {
                    Expect<CloseCompletedMessage>(await connector.ReadMessage(async).ConfigureAwait(false), connector);

                    var pStatement = batchCommand.PreparedStatement;
                    pStatement.CompleteUnprepare();

                    if (!pStatement.IsExplicit)
                        connector.PreparedStatementManager.AutoPrepared[pStatement.AutoPreparedSlotIndex] = null;

                    batchCommand.PreparedStatement = null;
                }
            }

            Expect<ReadyForQueryMessage>(await connector.ReadMessage(async).ConfigureAwait(false), connector);
        }
    }

    #endregion Prepare

    #region Query analysis

    internal void ProcessRawQuery(SqlQueryParser? parser, bool standardConformingStrings, NpgsqlBatchCommand? batchCommand)
    {
        var (commandText, commandType, parameters) = batchCommand is null
            ? (CommandText, CommandType, _parameters)
            : (batchCommand.CommandText, batchCommand.CommandType, batchCommand._parameters);

        if (string.IsNullOrEmpty(commandText))
            ThrowHelper.ThrowInvalidOperationException("CommandText property has not been initialized");

        switch (commandType)
        {
        case CommandType.Text:
            switch (parameters?.PlaceholderType ?? PlaceholderType.NoParameters)
            {
            case PlaceholderType.Positional:
                // In positional parameter mode, we don't need to parse/rewrite the CommandText or reorder the parameters - just use
                // them as is. If the SQL contains a semicolon (legacy batching) when positional parameters are in use, we just send
                // that and PostgreSQL will error (this behavior is by-design - use the new batching API).
                if (batchCommand is null)
                {
                    batchCommand = TruncateStatementsToOne();
                    batchCommand.FinalCommandText = CommandText;
                    if (parameters is not null)
                    {
                        batchCommand.PositionalParameters = parameters.InternalList;
                        batchCommand._parameters = parameters;
                    }
                }
                else
                {
                    batchCommand.FinalCommandText = batchCommand.CommandText;
                    if (parameters is not null)
                        batchCommand.PositionalParameters = parameters.InternalList;
                }

                ValidateParameterCount(batchCommand);
                break;

            case PlaceholderType.NoParameters:
                // Unless the EnableSqlRewriting AppContext switch is explicitly disabled, queries with no parameters are parsed just
                // like queries with named parameters, since they may contain a semicolon (legacy batching).
                if (EnableSqlRewriting)
                    goto case PlaceholderType.Named;
                goto case PlaceholderType.Positional;

            case PlaceholderType.Named:
                if (!EnableSqlRewriting)
                    ThrowHelper.ThrowNotSupportedException($"Named parameters are not supported when Npgsql.{nameof(EnableSqlRewriting)} is disabled");

                // The parser is cached on NpgsqlConnector - unless we're in multiplexing mode.
                parser ??= new SqlQueryParser();

                if (batchCommand is null)
                {
                    parser.ParseRawQuery(this, standardConformingStrings);
                    if (InternalBatchCommands.Count > 1 && _parameters?.HasOutputParameters == true)
                        ThrowHelper.ThrowNotSupportedException("Commands with multiple queries cannot have out parameters");
                    for (var i = 0; i < InternalBatchCommands.Count; i++)
                        ValidateParameterCount(InternalBatchCommands[i]);
                }
                else
                {
                    parser.ParseRawQuery(batchCommand, standardConformingStrings);
                    ValidateParameterCount(batchCommand);
                }

                break;

            case PlaceholderType.Mixed:
                ThrowHelper.ThrowNotSupportedException("Mixing named and positional parameters isn't supported");
                break;

            default:
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(PlaceholderType), $"Unknown {nameof(PlaceholderType)} value: {{0}}", _parameters?.PlaceholderType ?? PlaceholderType.NoParameters);
                break;
            }

            break;

        case CommandType.TableDirect:
            batchCommand ??= TruncateStatementsToOne();
            batchCommand.FinalCommandText = "SELECT * FROM " + CommandText;
            break;

        case CommandType.StoredProcedure:
            var sqlBuilder = new StringBuilder()
                .Append(EnableStoredProcedureCompatMode ? "SELECT * FROM " : "CALL ")
                .Append(commandText)
                .Append('(');

            var isFirstParam = true;
            var seenNamedParam = false;
            var inputParameters = NpgsqlBatchCommand.EmptyParameters;
            if (parameters is not null)
            {
                inputParameters = new List<NpgsqlParameter>(parameters.Count);
                for (var i = 0; i < parameters.Count; i++)
                {
                    var parameter = parameters[i];

                    // With functions, output parameters are never present when calling the function (they only define the schema of the
                    // returned table). With stored procedures they must be specified in the CALL argument list (see below).
                    if (EnableStoredProcedureCompatMode && parameter.Direction == ParameterDirection.Output)
                        continue;

                    if (parameter.Direction == ParameterDirection.ReturnValue)
                        continue;

                    if (isFirstParam)
                        isFirstParam = false;
                    else
                        sqlBuilder.Append(", ");

                    if (parameter.IsPositional)
                    {
                        if (seenNamedParam)
                            ThrowHelper.ThrowArgumentException(NpgsqlStrings.PositionalParameterAfterNamed);
                    }
                    else
                    {
                        seenNamedParam = true;

                        sqlBuilder
                            .Append('"')
                            .Append(parameter.TrimmedName.Replace("\"", "\"\""))
                            .Append("\" := ");
                    }

                    if (parameter.Direction == ParameterDirection.Output)
                        sqlBuilder.Append("NULL");
                    else
                    {
                        inputParameters!.Add(parameter);
                        sqlBuilder.Append('$').Append(inputParameters.Count);
                    }
                }
            }

            sqlBuilder.Append(')');

            batchCommand ??= TruncateStatementsToOne();
            batchCommand.FinalCommandText = sqlBuilder.ToString();
            batchCommand._parameters = parameters;
            batchCommand.PositionalParameters.AddRange(inputParameters);
            ValidateParameterCount(batchCommand);

            break;

        default:
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(CommandType), $"Internal Npgsql bug: unexpected value {{0}} of enum {nameof(CommandType)}. Please file a bug.", commandType);
            break;
        }

        static void ValidateParameterCount(NpgsqlBatchCommand batchCommand)
        {
            if (batchCommand is { HasParameters: true, PositionalParameters.Count: > ushort.MaxValue })
                ThrowHelper.ThrowNpgsqlException("A statement cannot have more than 65535 parameters");
        }
    }

    #endregion

    #region Message Creation / Population

    void BeginSend(NpgsqlConnector connector)
        => connector.WriteBuffer.Timeout = TimeSpan.FromSeconds(CommandTimeout);

    internal Task Write(NpgsqlConnector connector, bool async, bool flush, CancellationToken cancellationToken = default)
    {
        return (_behavior & CommandBehavior.SchemaOnly) == 0
            ? WriteExecute(connector, async, flush, cancellationToken)
            : WriteExecuteSchemaOnly(connector, async, flush, cancellationToken);

        async Task WriteExecute(NpgsqlConnector connector, bool async, bool flush, CancellationToken cancellationToken)
        {
            NpgsqlBatchCommand? batchCommand = null;

            var syncCaller = !async;
            for (var i = 0; i < InternalBatchCommands.Count; i++)
            {
                // The following is only for deadlock avoidance when doing sync I/O (so never in multiplexing)
                if (syncCaller && ShouldSchedule(ref async, i))
                    await new TaskSchedulerAwaitable(ConstrainedConcurrencyScheduler);

                batchCommand = InternalBatchCommands[i];
                var pStatement = batchCommand.PreparedStatement;

                Debug.Assert(batchCommand.FinalCommandText is not null);

                if (pStatement == null || batchCommand.IsPreparing)
                {
                    // The statement should either execute unprepared, or is being auto-prepared.
                    // Send Parse, Bind, Describe

                    // We may have a prepared statement that replaces an existing statement - close the latter first.
                    if (pStatement?.StatementBeingReplaced != null)
                        await connector.WriteClose(StatementOrPortal.Statement, pStatement.StatementBeingReplaced.Name!, async, cancellationToken).ConfigureAwait(false);

                    await connector.WriteParse(batchCommand.FinalCommandText, batchCommand.StatementName,
                        batchCommand.CurrentParametersReadOnly, async, cancellationToken).ConfigureAwait(false);

                    await connector.WriteBind(
                        batchCommand.CurrentParametersReadOnly,
                        string.Empty, batchCommand.StatementName, AllResultTypesAreUnknown,
                        i == 0 ? UnknownResultTypeList : null,
                        async, cancellationToken).ConfigureAwait(false);

                    await connector.WriteDescribe(StatementOrPortal.Portal, [], async, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    // The statement is already prepared, only a Bind is needed
                    await connector.WriteBind(
                        batchCommand.CurrentParametersReadOnly,
                        string.Empty, batchCommand.StatementName, AllResultTypesAreUnknown,
                        i == 0 ? UnknownResultTypeList : null,
                        async, cancellationToken).ConfigureAwait(false);
                }

                await connector.WriteExecute(0, async, cancellationToken).ConfigureAwait(false);

                if (batchCommand.AppendErrorBarrier ?? EnableErrorBarriers)
                    await connector.WriteSync(async, cancellationToken).ConfigureAwait(false);

                pStatement?.RefreshLastUsed();
            }

            if (batchCommand is null || !(batchCommand.AppendErrorBarrier ?? EnableErrorBarriers))
            {
                await connector.WriteSync(async, cancellationToken).ConfigureAwait(false);
            }

            if (flush)
                await connector.Flush(async, cancellationToken).ConfigureAwait(false);
        }

        async Task WriteExecuteSchemaOnly(NpgsqlConnector connector, bool async, bool flush, CancellationToken cancellationToken)
        {
            var wroteSomething = false;
            var syncCaller = !async;
            for (var i = 0; i < InternalBatchCommands.Count; i++)
            {
                if (syncCaller && ShouldSchedule(ref async, i))
                    await new TaskSchedulerAwaitable(ConstrainedConcurrencyScheduler);

                var batchCommand = InternalBatchCommands[i];

                if (batchCommand.PreparedStatement?.State == PreparedState.Prepared)
                    continue; // Prepared, we already have the RowDescription

                await connector.WriteParse(batchCommand.FinalCommandText!, batchCommand.StatementName,
                    batchCommand.CurrentParametersReadOnly,
                    async, cancellationToken).ConfigureAwait(false);
                await connector.WriteDescribe(StatementOrPortal.Statement, batchCommand.StatementName, async, cancellationToken).ConfigureAwait(false);
                wroteSomething = true;
            }

            if (wroteSomething)
            {
                await connector.WriteSync(async, cancellationToken).ConfigureAwait(false);
                if (flush)
                    await connector.Flush(async, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    async Task SendDeriveParameters(NpgsqlConnector connector, bool async, CancellationToken cancellationToken = default)
    {
        BeginSend(connector);

        var syncCaller = !async;
        for (var i = 0; i < InternalBatchCommands.Count; i++)
        {
            if (syncCaller && ShouldSchedule(ref async, i))
                await new TaskSchedulerAwaitable(ConstrainedConcurrencyScheduler);

            var batchCommand = InternalBatchCommands[i];

            await connector.WriteParse(batchCommand.FinalCommandText!, [], NpgsqlBatchCommand.EmptyParameters, async, cancellationToken).ConfigureAwait(false);
            await connector.WriteDescribe(StatementOrPortal.Statement, [], async, cancellationToken).ConfigureAwait(false);
        }

        await connector.WriteSync(async, cancellationToken).ConfigureAwait(false);
        await connector.Flush(async, cancellationToken).ConfigureAwait(false);
    }

    async Task SendPrepare(NpgsqlConnector connector, bool async, CancellationToken cancellationToken = default)
    {
        BeginSend(connector);

        var syncCaller = !async;
        for (var i = 0; i < InternalBatchCommands.Count; i++)
        {
            if (syncCaller && ShouldSchedule(ref async, i))
                await new TaskSchedulerAwaitable(ConstrainedConcurrencyScheduler);

            var batchCommand = InternalBatchCommands[i];
            var pStatement = batchCommand.PreparedStatement;

            // A statement may be already prepared, already in preparation (i.e. same statement twice
            // in the same command), or we can't prepare (overloaded SQL)
            if (!batchCommand.IsPreparing)
                continue;

            // We may have a prepared statement that replaces an existing statement - close the latter first.
            var statementToClose = pStatement!.StatementBeingReplaced;
            if (statementToClose != null)
                await connector.WriteClose(StatementOrPortal.Statement, statementToClose.Name!, async, cancellationToken).ConfigureAwait(false);

            await connector.WriteParse(batchCommand.FinalCommandText!, pStatement.Name!, batchCommand.CurrentParametersReadOnly, async,
                cancellationToken).ConfigureAwait(false);
            await connector.WriteDescribe(StatementOrPortal.Statement, pStatement.Name!, async, cancellationToken).ConfigureAwait(false);
        }

        await connector.WriteSync(async, cancellationToken).ConfigureAwait(false);
        await connector.Flush(async, cancellationToken).ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ShouldSchedule(ref bool async, int indexOfStatementInBatch)
    {
        if (indexOfStatementInBatch <= 0)
            return false;

        // We're synchronously sending the non-first statement in a batch - switch to async writing.
        // See long comment in Execute() above.

        // TODO: we can simply do all batch writing asynchronously, instead of starting with the 2nd statement.
        // For now, writing the first statement synchronously gives us a better chance of handling and bubbling up errors correctly
        // (see sendTask.IsFaulted in Execute()). Once #1323 is done, that shouldn't be needed any more and entire batches should
        // be written asynchronously.
        async = true;
        return TaskScheduler.Current != ConstrainedConcurrencyScheduler;
    }

    async Task SendClose(NpgsqlConnector connector, bool async, CancellationToken cancellationToken = default)
    {
        BeginSend(connector);

        foreach (var batchCommand in InternalBatchCommands)
        {
            if (!batchCommand.IsPrepared)
                continue;
            // No need to force async here since each statement takes no more than 20 bytes
            await connector.WriteClose(StatementOrPortal.Statement, batchCommand.StatementName, async, cancellationToken).ConfigureAwait(false);
            batchCommand.PreparedStatement!.State = PreparedState.BeingUnprepared;
        }

        await connector.WriteSync(async, cancellationToken).ConfigureAwait(false);
        await connector.Flush(async, cancellationToken).ConfigureAwait(false);
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation, with the number of rows affected if known; -1 otherwise.</returns>
    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        => ExecuteNonQuery(async: true, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    async Task<int> ExecuteNonQuery(bool async, CancellationToken cancellationToken)
    {
        var reader = await ExecuteReader(async, CommandBehavior.Default, cancellationToken).ConfigureAwait(false);
        try
        {
            while (async ? await reader.NextResultAsync(cancellationToken).ConfigureAwait(false) : reader.NextResult()) ;

            return reader.RecordsAffected;
        }
        finally
        {
            if (async)
                await reader.DisposeAsync().ConfigureAwait(false);
            else
                reader.Dispose();
        }
    }

    #endregion Execute Non Query

    #region Execute Scalar

    /// <summary>
    /// Executes the query, and returns the first column of the first row
    /// in the result set returned by the query. Extra columns or rows are ignored.
    /// </summary>
    /// <returns>The first column of the first row in the result set,
    /// or a null reference if the result set is empty.</returns>
    public override object? ExecuteScalar() => ExecuteScalar(false, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Asynchronous version of <see cref="ExecuteScalar()"/>
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation, with the first column of the
    /// first row in the result set, or a null reference if the result set is empty.</returns>
    public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        => ExecuteScalar(async: true, cancellationToken).AsTask();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    async ValueTask<object?> ExecuteScalar(bool async, CancellationToken cancellationToken)
    {
        var behavior = CommandBehavior.SingleRow;
        if (WrappingBatch is not null || _parameters?.HasOutputParameters != true)
            behavior |= CommandBehavior.SequentialAccess;

        var reader = await ExecuteReader(async, behavior, cancellationToken).ConfigureAwait(false);
        try
        {
            var read = async ? await reader.ReadAsync(cancellationToken).ConfigureAwait(false) : reader.Read();
            return read && reader.FieldCount != 0 ? reader.GetValue(0) : null;
        }
        finally
        {
            if (async)
                await reader.DisposeAsync().ConfigureAwait(false);
            else
                reader.Dispose();
        }
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
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        => await ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Executes the <see cref="CommandText"/> against the <see cref="Connection"/>
    /// and returns a <see cref="NpgsqlDataReader"/>.
    /// </summary>
    /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
    /// <returns>A task representing the operation.</returns>
    public new NpgsqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        => ExecuteReader(async: false, behavior, CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// An asynchronous version of <see cref="ExecuteReader(CommandBehavior)"/>, which executes
    /// the <see cref="CommandText"/> against the <see cref="Connection"/>
    /// and returns a <see cref="NpgsqlDataReader"/>.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public new Task<NpgsqlDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        => ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);

    /// <summary>
    /// An asynchronous version of <see cref="ExecuteReader(CommandBehavior)"/>,
    /// which executes the <see cref="CommandText"/> against the <see cref="Connection"/>
    /// and returns a <see cref="NpgsqlDataReader"/>.
    /// </summary>
    /// <param name="behavior">One of the enumeration values that specifies the command behavior.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public new Task<NpgsqlDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken = default)
        => ExecuteReader(async: true, behavior, cancellationToken).AsTask();

    // TODO: Maybe pool these?
    internal ManualResetValueTaskSource<NpgsqlConnector> ExecutionCompletion { get; }
        = new();

    internal virtual async ValueTask<NpgsqlDataReader> ExecuteReader(bool async, CommandBehavior behavior, CancellationToken cancellationToken)
    {
        var conn = CheckAndGetConnection();
        _behavior = behavior;

        NpgsqlConnector? connector;
        if (_connector is not null)
        {
            Debug.Assert(conn is null);
            if (behavior.HasFlag(CommandBehavior.CloseConnection))
                ThrowHelper.ThrowArgumentException($"{nameof(CommandBehavior.CloseConnection)} is not supported with {nameof(NpgsqlConnector)}", nameof(behavior));
            connector = _connector;
        }
        else
        {
            Debug.Assert(conn is not null);
            conn.TryGetBoundConnector(out connector);
        }

        try
        {
            if (connector is not null)
            {
                var logger = connector.CommandLogger;

                cancellationToken.ThrowIfCancellationRequested();
                // We cannot pass a token here, as we'll cancel a non-send query
                // Also, we don't pass the cancellation token to StartUserAction, since that would make it scope to the entire action (command execution)
                // whereas it should only be scoped to the Execute method.
                connector.StartUserAction(ConnectorState.Executing, this, CancellationToken.None);

                Task? sendTask;

                var validateParameterValues = !behavior.HasFlag(CommandBehavior.SchemaOnly);
                long startTimestamp;

                try
                {
                    switch (IsExplicitlyPrepared)
                    {
                    case true:
                        Debug.Assert(_connectorPreparedOn != null);
                        if (WrappingBatch is not null)
                        {
                            foreach (var batchCommand in InternalBatchCommands)
                            {
                                if (batchCommand.ConnectorPreparedOn != connector)
                                {
                                    foreach (var s in InternalBatchCommands)
                                        s.ResetPreparation();
                                    ResetPreparation();
                                    goto case false;
                                }

                                batchCommand._parameters?.ProcessParameters(connector.SerializerOptions, validateParameterValues, batchCommand.CommandType);
                            }
                        }
                        else
                        {
                            if (_connectorPreparedOn != connector)
                            {
                                // The command was prepared, but since then the connector has changed. Detach all prepared statements.
                                foreach (var s in InternalBatchCommands)
                                    s.PreparedStatement = null;
                                ResetPreparation();
                                goto case false;
                            }
                            _parameters?.ProcessParameters(connector.SerializerOptions, validateParameterValues, CommandType);
                        }

                        NpgsqlEventSource.Log.CommandStartPrepared();
                        connector.DataSource.MetricsReporter.CommandStartPrepared();
                        break;

                    case false:
                        var numPrepared = 0;

                        if (WrappingBatch is not null)
                        {
                            for (var i = 0; i < InternalBatchCommands.Count; i++)
                            {
                                var batchCommand = InternalBatchCommands[i];

                                batchCommand._parameters?.ProcessParameters(connector.SerializerOptions, validateParameterValues, batchCommand.CommandType);
                                ProcessRawQuery(connector.SqlQueryParser, connector.UseConformingStrings, batchCommand);

                                if (connector.Settings.MaxAutoPrepare > 0 && batchCommand.TryAutoPrepare(connector))
                                {
                                    batchCommand.ConnectorPreparedOn = connector;
                                    numPrepared++;
                                }
                            }
                        }
                        else
                        {
                            _parameters?.ProcessParameters(connector.SerializerOptions, validateParameterValues, CommandType);
                            ProcessRawQuery(connector.SqlQueryParser, connector.UseConformingStrings, batchCommand: null);

                            if (connector.Settings.MaxAutoPrepare > 0)
                                for (var i = 0; i < InternalBatchCommands.Count; i++)
                                    if (InternalBatchCommands[i].TryAutoPrepare(connector))
                                        numPrepared++;
                        }

                        if (numPrepared > 0)
                        {
                            _connectorPreparedOn = connector;
                            if (numPrepared == InternalBatchCommands.Count)
                            {
                                NpgsqlEventSource.Log.CommandStartPrepared();
                                connector.DataSource.MetricsReporter.CommandStartPrepared();
                            }
                        }

                        break;
                    }

                    // If a cancellation is in progress, wait for it to "complete" before proceeding (#615)
                    // We do it before changing the state because we only allow sending cancellation request if State == InProgress
                    connector.ResetCancellation();

                    State = CommandState.InProgress;

                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        connector.QueryLogStopWatch.Restart();

                        if (logger.IsEnabled(LogLevel.Debug))
                            LogExecutingCompleted(connector, executing: true);
                    }

                    NpgsqlEventSource.Log.CommandStart(CommandText);
                    startTimestamp = connector.DataSource.MetricsReporter.ReportCommandStart();
                    TraceCommandStart(connector.Settings, connector.DataSource.Configuration.TracingOptions);
                    TraceCommandEnrich(connector);

                    // We do not wait for the entire send to complete before proceeding to reading -
                    // the sending continues in parallel with the user's reading. Waiting for the
                    // entire send to complete would trigger a deadlock for multi-statement commands,
                    // where PostgreSQL sends large results for the first statement, while we're sending large
                    // parameter data for the second. See #641.
                    // Instead, all sends for non-first statements are performed asynchronously (even if the user requested sync),
                    // in a special synchronization context to prevents a dependency on the thread pool (which would also trigger
                    // deadlocks).
                    BeginSend(connector);
                    sendTask = Write(connector, async, flush: true, CancellationToken.None);

                    // The following is a hack. It raises an exception if one was thrown in the first phases
                    // of the send (i.e. in parts of the send that executed synchronously). Exceptions may
                    // still happen later and aren't properly handled. See #1323.
                    if (sendTask.IsFaulted)
                        sendTask.GetAwaiter().GetResult();
                }
                catch
                {
                    connector.EndUserAction();
                    throw;
                }

                // TODO: DRY the following with multiplexing, but be careful with the cancellation registration...
                var reader = connector.DataReader;
                reader.Init(this, behavior, InternalBatchCommands, startTimestamp, sendTask);
                connector.CurrentReader = reader;
                if (async)
                    await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                else
                    reader.NextResult();

                TraceReceivedFirstResponse(connector.DataSource.Configuration.TracingOptions);

                return reader;
            }
            else
            {
                Debug.Assert(conn is not null);
                Debug.Assert(conn.Settings.Multiplexing);

                // The connection isn't bound to a connector - it's multiplexing time.
                var dataSource = (MultiplexingDataSource)conn.NpgsqlDataSource;

                if (!async)
                {
                    // The waiting on the ExecutionCompletion ManualResetValueTaskSource is necessarily
                    // asynchronous, so allowing sync would mean sync-over-async.
                    ThrowHelper.ThrowNotSupportedException("Synchronous command execution is not supported when multiplexing is on");
                }

                if (WrappingBatch is not null)
                {
                    foreach (var batchCommand in InternalBatchCommands)
                    {
                        batchCommand._parameters?.ProcessParameters(dataSource.SerializerOptions, validateValues: true, batchCommand.CommandType);
                        ProcessRawQuery(null, standardConformingStrings: true, batchCommand);
                    }
                }
                else
                {
                    _parameters?.ProcessParameters(dataSource.SerializerOptions, validateValues: true, CommandType);
                    ProcessRawQuery(null, standardConformingStrings: true, batchCommand: null);
                }

                State = CommandState.InProgress;

                TraceCommandStart(conn.Settings, conn.NpgsqlDataSource.Configuration.TracingOptions);

                // TODO: Experiment: do we want to wait on *writing* here, or on *reading*?
                // Previous behavior was to wait on reading, which throw the exception from ExecuteReader (and not from
                // the first read). But waiting on writing would allow us to do sync writing and async reading.
                ExecutionCompletion.Reset();
                try
                {
                    await dataSource.MultiplexCommandWriter.WriteAsync(this, cancellationToken).ConfigureAwait(false);
                }
                catch (ChannelClosedException ex)
                {
                    Debug.Assert(ex.InnerException is not null);
                    throw ex.InnerException;
                }
                connector = await new ValueTask<NpgsqlConnector>(ExecutionCompletion, ExecutionCompletion.Version).ConfigureAwait(false);
                // TODO: Overload of StartBindingScope?
                conn.Connector = connector;
                connector.Connection = conn;
                conn.ConnectorBindingScope = ConnectorBindingScope.Reader;

                var reader = connector.DataReader;
                reader.Init(this, behavior, InternalBatchCommands);
                connector.CurrentReader = reader;
                await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);

                TraceReceivedFirstResponse(connector.DataSource.Configuration.TracingOptions);

                return reader;
            }
        }
        catch (Exception e)
        {
            var reader = connector?.CurrentReader;
            if (e is not NpgsqlOperationInProgressException && reader is not null)
                await reader.Cleanup(async).ConfigureAwait(false);

            TraceSetException(e);

            State = CommandState.Idle;

            // Reader disposal contains logic for closing the connection if CommandBehavior.CloseConnection is
            // specified. However, close here as well in case of an error before the reader was even instantiated
            // (e.g. write I/O error)
            if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
            {
                Debug.Assert(_connector is null && conn is not null);
                conn.Close();
            }

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
        get => _transaction;
        set => _transaction = (NpgsqlTransaction?)value;
    }

    /// <summary>
    /// This property is ignored by Npgsql. PostgreSQL only supports a single transaction at a given time on
    /// a given connection, and all commands implicitly run inside the current transaction started via
    /// <see cref="NpgsqlConnection.BeginTransaction()"/>
    /// </summary>
    public new NpgsqlTransaction? Transaction
    {
        get => (NpgsqlTransaction?)DbTransaction;
        set => DbTransaction = value;
    }

    #endregion Transactions

    #region Cancel

    /// <summary>
    /// Attempts to cancel the execution of an <see cref="NpgsqlCommand" />.
    /// </summary>
    /// <remarks>As per the specs, no exception will be thrown by this method in case of failure.</remarks>
    public override void Cancel()
    {
        if (State != CommandState.InProgress)
            return;

        var connector = Connection?.Connector ?? _connector;
        if (connector is null)
            return;

        connector.PerformImmediateUserCancellation();
    }

    #endregion Cancel

    #region Dispose

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ResetTransaction();

        State = CommandState.Disposed;

        if (IsCacheable && InternalConnection is not null && InternalConnection.CachedCommand is null)
        {
            Reset();
            InternalConnection.CachedCommand = this;
            return;
        }

        IsCacheable = false;
    }

    internal void Reset()
    {
        // TODO: Optimize NpgsqlParameterCollection to recycle NpgsqlParameter instances as well
        // TODO: Statements isn't cleared/recycled, leaving this for now, since it'll be replaced by the new batching API
        _commandText = string.Empty;
        CommandType = CommandType.Text;
        // Can be null if it's owned by batch
        _parameters?.Clear();
        _timeout = null;
        AllResultTypesAreUnknown = false;
        Debug.Assert(_unknownResultTypeList is null);
        EnableErrorBarriers = false;
    }

    internal void ResetTransaction() => _transaction = null;

    #endregion

    #region Tracing

    internal void TraceCommandStart(NpgsqlConnectionStringBuilder settings, NpgsqlTracingOptions tracingOptions)
    {
        Debug.Assert(CurrentActivity is null);

        if (NpgsqlActivitySource.IsEnabled)
        {
            var enableTracing = WrappingBatch is not null
                ? tracingOptions.BatchFilter?.Invoke(WrappingBatch) ?? true
                : tracingOptions.CommandFilter?.Invoke(this) ?? true;

            if (enableTracing)
            {
                var spanName = WrappingBatch is not null
                    ? tracingOptions.BatchSpanNameProvider?.Invoke(WrappingBatch)
                    : tracingOptions.CommandSpanNameProvider?.Invoke(this);

                CurrentActivity = NpgsqlActivitySource.CommandStart(
                    settings,
                    WrappingBatch is not null ? GetBatchFullCommandText() : CommandText,
                    CommandType,
                    spanName);
            }
        }
    }

    internal void TraceCommandEnrich(NpgsqlConnector connector)
    {
        if (CurrentActivity is not null)
        {
            NpgsqlActivitySource.Enrich(CurrentActivity, connector);
            var tracingOptions = connector.DataSource.Configuration.TracingOptions;
            if (WrappingBatch is not null)
                tracingOptions.BatchEnrichmentCallback?.Invoke(CurrentActivity, WrappingBatch);
            else
                tracingOptions.CommandEnrichmentCallback?.Invoke(CurrentActivity, this);
        }
    }

    internal void TraceReceivedFirstResponse(NpgsqlTracingOptions tracingOptions)
    {
        if (CurrentActivity is not null)
            NpgsqlActivitySource.ReceivedFirstResponse(CurrentActivity, tracingOptions);
    }

    internal void TraceCommandStop()
    {
        if (CurrentActivity is not null)
        {
            NpgsqlActivitySource.CommandStop(CurrentActivity);
            CurrentActivity = null;
        }
    }

    internal void TraceSetException(Exception e)
    {
        if (CurrentActivity is not null)
        {
            NpgsqlActivitySource.SetException(CurrentActivity, e);
            CurrentActivity = null;
        }
    }

    #endregion Tracing

    #region Misc

    NpgsqlBatchCommand TruncateStatementsToOne()
    {
        switch (InternalBatchCommands.Count)
        {
        case 0:
            var statement = new NpgsqlBatchCommand();
            InternalBatchCommands.Add(statement);
            return statement;

        case 1:
            statement = InternalBatchCommands[0];
            statement.Reset();
            return statement;

        default:
            statement = InternalBatchCommands[0];
            statement.Reset();
            InternalBatchCommands.Clear();
            InternalBatchCommands.Add(statement);
            return statement;
        }
    }

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
        for (var i = 0; i < rowDescription.Count; i++)
        {
            var field = rowDescription[i];
            field.DataFormat = (UnknownResultTypeList == null || !isFirst ? AllResultTypesAreUnknown : UnknownResultTypeList[i])
                ? DataFormat.Text
                : DataFormat.Binary;
        }
    }

    internal void LogExecutingCompleted(NpgsqlConnector connector, bool executing)
    {
        var logParameters = connector.LoggingConfiguration.IsParameterLoggingEnabled || connector.Settings.LogParameters;
        var logger = connector.LoggingConfiguration.CommandLogger;

        if (InternalBatchCommands.Count == 1)
        {
            var singleCommand = InternalBatchCommands[0];

            if (logParameters && singleCommand.HasParameters)
            {
                if (executing)
                {
                    LogMessages.ExecutingCommandWithParameters(
                        logger,
                        singleCommand.FinalCommandText!,
                        GetParametersForLogging(singleCommand),
                        connector.Id);
                }
                else
                {
                    LogMessages.CommandExecutionCompletedWithParameters(
                        logger,
                        singleCommand.FinalCommandText!,
                        GetParametersForLogging(singleCommand),
                        connector.QueryLogStopWatch.ElapsedMilliseconds,
                        connector.Id);
                }
            }
            else
            {
                if (executing)
                    LogMessages.ExecutingCommand(logger, singleCommand.FinalCommandText!, connector.Id);
                else
                    LogMessages.CommandExecutionCompleted(logger, singleCommand.FinalCommandText!, connector.QueryLogStopWatch.ElapsedMilliseconds, connector.Id);
            }
        }
        else
        {
            if (logParameters)
            {
                var commands = new (string, object[])[InternalBatchCommands.Count];
                for (var i = 0; i < InternalBatchCommands.Count; i++)
                    commands[i] = (InternalBatchCommands[i].FinalCommandText!, GetParametersForLogging(InternalBatchCommands[i]));

                if (executing)
                    LogMessages.ExecutingBatchWithParameters(logger, commands, connector.Id);
                else
                    LogMessages.BatchExecutionCompletedWithParameters(logger, commands, connector.QueryLogStopWatch.ElapsedMilliseconds, connector.Id);
            }
            else
            {
                var commands = new string[InternalBatchCommands.Count];
                for (var i = 0; i < InternalBatchCommands.Count; i++)
                    commands[i] = InternalBatchCommands[i].FinalCommandText!;
                if (executing)
                    LogMessages.ExecutingBatch(logger, commands, connector.Id);
                else
                    LogMessages.BatchExecutionCompleted(logger, commands, connector.QueryLogStopWatch.ElapsedMilliseconds, connector.Id);
            }
        }

        static object[] GetParametersForLogging(NpgsqlBatchCommand c)
        {
            var positionalParameters = c.CurrentParametersReadOnly;
            var parameters = new object[positionalParameters.Count];
            for (var i = 0; i < positionalParameters.Count; i++)
            {
                parameters[i] = GetParameterForLogging(positionalParameters[i].Value);
            }
            return parameters;

            object GetParameterForLogging(object? value)
            {
                return value switch
                {
                    DBNull or null => "NULL",
                    IEnumerable enumerable and not string => GetEnumerableForLogging(enumerable),
                    _ => value
                };

                string GetEnumerableForLogging(IEnumerable enumerable)
                {
                    var vsb = new StringBuilder(256);
                    var count = 0;
                    vsb.Append('[');
                    foreach (var e in enumerable)
                    {
                        if (count > 9)
                        {
                            vsb.Append(", ...");
                            break;
                        }

                        if (count > 0)
                        {
                            vsb.Append(", ");
                        }

                        vsb.Append(GetParameterForLogging(e));
                        count++;
                    }

                    vsb.Append(']');
                    return vsb.ToString();
                }
            }
        }
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
    public virtual NpgsqlCommand Clone()
    {
        var clone = new NpgsqlCommand(CommandText, InternalConnection, Transaction)
        {
            CommandTimeout = CommandTimeout,
            CommandType = CommandType,
            DesignTimeVisible = DesignTimeVisible,
            _allResultTypesAreUnknown = _allResultTypesAreUnknown,
            _unknownResultTypeList = _unknownResultTypeList
        };
        _parameters?.CloneTo(clone.Parameters);
        return clone;
    }

    NpgsqlConnection? CheckAndGetConnection()
    {
        ObjectDisposedException.ThrowIf(State is CommandState.Disposed, this);

        var conn = InternalConnection;
        if (conn is null)
        {
            if (_connector is null)
                ThrowHelper.ThrowInvalidOperationException("Connection property has not been initialized.");
            return null;
        }

        if (!conn.FullState.HasFlag(ConnectionState.Open))
            ThrowHelper.ThrowInvalidOperationException("Connection is not open");

        return conn;
    }

    /// <summary>
    /// This event is unsupported by Npgsql. Use <see cref="System.Data.Common.DbConnection.StateChange"/> instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Disposed
    {
        add => throw new NotSupportedException("The Disposed event isn't supported by Npgsql. Use DbConnection.StateChange instead.");
        remove => throw new NotSupportedException("The Disposed event isn't supported by Npgsql. Use DbConnection.StateChange instead.");
    }

    event EventHandler? IComponent.Disposed
    {
        add => Disposed += value;
        remove => Disposed -= value;
    }

    #endregion
}

enum CommandState
{
    Idle,
    InProgress,
    Disposed
}
