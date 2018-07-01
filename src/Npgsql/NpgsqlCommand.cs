#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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
using System.Net.Sockets;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using NpgsqlTypes;
using static Npgsql.Statics;

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

        [CanBeNull]
        NpgsqlConnection _connection;

        /// <summary>
        /// If this command is (explicitly) prepared, references the connector on which the preparation happened.
        /// Used to detect when the connector was changed (i.e. connection open/close), meaning that the command
        /// is no longer prepared.
        /// </summary>
        [CanBeNull]
        NpgsqlConnector _connectorPreparedOn;

        NpgsqlTransaction _transaction;
        string _commandText;
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

        static readonly SingleThreadSynchronizationContext SingleThreadSynchronizationContext = new SingleThreadSynchronizationContext("NpgsqlRemainingAsyncSendWorker");

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Constants

        internal const int DefaultTimeout = 30;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class.
        /// </summary>
        public NpgsqlCommand() : this(string.Empty, null, null) {}

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
        public NpgsqlCommand(string cmdText, [CanBeNull] NpgsqlConnection connection, [CanBeNull] NpgsqlTransaction transaction)
        {
            GC.SuppressFinalize(this);
            _statements = new List<NpgsqlStatement>(1);
            _parameters = new NpgsqlParameterCollection();
            _commandText = cmdText;
            Connection = connection;
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
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _commandText = value;
                ResetExplicitPreparation();
                // TODO: Technically should do this also if the parameter list (or type) changes
            }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt  to execute a command and generating an error.
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
        protected override DbConnection DbConnection
        {
            get => Connection;
            set => Connection = (NpgsqlConnection)value;
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// used by this instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <value>The connection to a data source. The default value is a null reference.</value>
        [DefaultValue(null)]
        [Category("Behavior")]
        [CanBeNull]
        public new NpgsqlConnection Connection
        {
            get => _connection;
            set
            {
                if (_connection == value)
                {
                    return;
                }

                //if (this._transaction != null && this._transaction.Connection == null)
                //  this._transaction = null;

                // All this checking needs revising. It should be simpler.
                // This this.Connector != null check was added to remove the nullreferenceexception in case
                // of the previous connection has been closed which makes Connector null and so the last check would fail.
                // See bug 1000581 for more details.
                if (_transaction != null && _connection != null && _connection.Connector != null && _connection.Connector.InTransaction)
                    throw new InvalidOperationException("The Connection property can't be changed with an uncommited transaction.");

                _connection = value;
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
            get { return _updateRowSource; }
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
            _connectorPreparedOn == Connection?.Connector &&
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
        public bool[] UnknownResultTypeList
        {
            get => _unknownResultTypeList;
            set
            {
                // TODO: Check that this isn't modified after calling prepare
                _allResultTypesAreUnknown = false;
                _unknownResultTypeList = value;
            }
        }

        bool[] _unknownResultTypeList;

        #endregion

        #region Result Types Management

        /// <summary>
        /// Marks result types to be used when using GetValue on a data reader, on a column-by-column basis.
        /// Used for Entity Framework 5-6 compability.
        /// Only primitive numerical types and DateTimeOffset are supported.
        /// Set the whole array or just a value to null to use default type.
        /// </summary>
        internal Type[] ObjectResultTypes { get; set; }

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
            if (Statements.Where(s => s?.PreparedStatement.IsExplicit == true).Any())
                throw new NpgsqlException("Deriving parameters isn't supported for commands that are already prepared.");

            // Here we unprepare statements that possibly are autoprepared
            Unprepare();

            Parameters.Clear();

            if (CommandType == CommandType.StoredProcedure)
                DeriveParametersForFunction();
            else if (CommandType == CommandType.Text)
                DeriveParametersForQuery();
        }

        void DeriveParametersForFunction()
        {
            using (var c = new NpgsqlCommand(DeriveParametersForFunctionQuery, Connection))
            {
                c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Text));
                c.Parameters[0].Value = CommandText;

                string[] names = null;
                uint[] types = null;
                char[] modes = null;

                using (var rdr = c.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            names = rdr.GetValue(0) as string[];
                        if (!rdr.IsDBNull(2))
                            types = rdr.GetValue(2) as uint[];
                        if (!rdr.IsDBNull(3))
                            modes = rdr.GetValue(3) as char[];
                        if (types == null)
                        {
                            if (rdr.IsDBNull(1) || rdr.GetFieldValue<uint[]>(1).Length == 0)
                                return;  // Parameterless function
                            types = rdr.GetFieldValue<uint[]>(1);
                        }
                    }
                    else
                        throw new InvalidOperationException($"{CommandText} does not exist in pg_proc");
                }

                for (var i = 0; i < types.Length; i++)
                {
                    var param = new NpgsqlParameter();

                    (var npgsqlDbType, var postgresType) =
                        c.Connection.Connector.TypeMapper.GetTypeInfoByOid(types[i]);

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
                        switch (modes[i])
                        {
                            case 'i':
                                param.Direction = ParameterDirection.Input;
                                break;
                            case 'o':
                            case 't':
                                param.Direction = ParameterDirection.Output;
                                break;
                            case 'b':
                                param.Direction = ParameterDirection.InputOutput;
                                break;
                            case 'v':
                                throw new NotImplementedException("Cannot derive function parameter of type VARIADIC");
                            default:
                                throw new ArgumentOutOfRangeException("proargmode", modes[i],
                                    "Unknown code in proargmodes while deriving: " + modes[i]);
                        }
                    }

                    Parameters.Add(param);
                }
            }
        }

        void DeriveParametersForQuery()
        {
            var connector = CheckReadyAndGetConnector();
            using (connector.StartUserAction())
            {
                Log.Debug($"Deriving Parameters for query: {CommandText}", connector.Id);
                ProcessRawQuery(true);

                var sendTask = SendDeriveParameters(false);

                foreach (var statement in _statements)
                {
                    Expect<ParseCompleteMessage>(connector.ReadMessage());
                    var paramTypeOIDs = Expect<ParameterDescriptionMessage>(connector.ReadMessage()).TypeOIDs;

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

                            (var npgsqlDbType, var postgresType) = connector.TypeMapper.GetTypeInfoByOid(paramOid);

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

                Expect<ReadyForQueryMessage>(connector.ReadMessage());
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
        public Task PrepareAsync() => PrepareAsync(CancellationToken.None);

        /// <summary>
        /// Creates a server-side prepared statement on the PostgreSQL server.
        /// This will make repeated future executions of this command much faster.
        /// </summary>
#pragma warning disable CA1801 // Review unused parameters
        public Task PrepareAsync(CancellationToken cancellationToken)
#pragma warning restore CA1801 // Review unused parameters
        {
            cancellationToken.ThrowIfCancellationRequested();
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

            // It's possible the command was already prepared, or that presistent prepared statements were found for
            // all statements. Nothing to do here, move along.
            return needToPrepare
                ? PrepareLong()
                : PGUtil.CompletedTask;

            async Task PrepareLong()
            {
                using (connector.StartUserAction())
                {
                    var sendTask = SendPrepare(async);

                    // Loop over statements, skipping those that are already prepared (because they were persisted)
                    var isFirst = true;
                    foreach (var statement in _statements.Where(s =>
                        s.PreparedStatement?.State == PreparedState.BeingPrepared))
                    {
                        var pStatement = statement.PreparedStatement;
                        Debug.Assert(pStatement != null);
                        Debug.Assert(pStatement.Description == null);
                        if (pStatement.StatementBeingReplaced != null)
                        {
                            Expect<CloseCompletedMessage>(await connector.ReadMessage(async));
                            pStatement.StatementBeingReplaced.CompleteUnprepare();
                            pStatement.StatementBeingReplaced = null;
                        }

                        Expect<ParseCompleteMessage>(await connector.ReadMessage(async));
                        Expect<ParameterDescriptionMessage>(await connector.ReadMessage(async));
                        var msg = await connector.ReadMessage(async);
                        switch (msg.Code)
                        {
                        case BackendMessageCode.RowDescription:
                            var description = (RowDescriptionMessage)msg;
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

                    Expect<ReadyForQueryMessage>(await connector.ReadMessage(async));

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
                var sendTask = SendClose(false);
                foreach (var statement in _statements.Where(s => s.PreparedStatement?.State == PreparedState.BeingUnprepared))
                {
                    Expect<CloseCompletedMessage>(connector.ReadMessage());
                    Debug.Assert(statement.PreparedStatement != null);
                    statement.PreparedStatement.CompleteUnprepare();
                    statement.PreparedStatement = null;
                }
                Expect<ReadyForQueryMessage>(connector.ReadMessage());
                sendTask.GetAwaiter().GetResult();
            }
        }

        #endregion Prepare

        #region Query analysis

        void ProcessRawQuery(bool deriveParameters = false)
        {
            NpgsqlStatement statement;
            switch (CommandType) {
            case CommandType.Text:
                Debug.Assert(_connection?.Connector != null);
                var connector = _connection.Connector;
                connector.SqlParser.ParseRawQuery(CommandText, connector.UseConformantStrings, _parameters, _statements, deriveParameters);
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
                if (s.InputParameters.Count > 65535)
                    throw new Exception("A statement cannot have more than 65535 parameters");
        }

        #endregion

        #region Execute

        void ValidateParameters()
        {
            for (var i = 0; i < Parameters.Count; i++)
            {
                var p = Parameters[i];
                if (!p.IsInputDirection)
                    continue;
                p.Bind(Connection.Connector.TypeMapper);
                p.LengthCache?.Clear();
                p.ValidateAndGetLength();
            }
        }

        #endregion

        #region Message Creation / Population

        internal bool FlushOccurred { get; set; }

        void BeginSend()
        {
            Debug.Assert(Connection?.Connector != null);
            Connection.Connector.WriteBuffer.CurrentCommand = this;
            FlushOccurred = false;
        }

        void CleanupSend()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (SynchronizationContext.Current != null)  // Check first because SetSynchronizationContext allocates
                SynchronizationContext.SetSynchronizationContext(null);
        }

        async Task SendExecute(bool async)
        {
            BeginSend();
            var connector = Connection.Connector;
            Debug.Assert(connector != null);

            var buf = connector.WriteBuffer;
            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];
                var pStatement = statement.PreparedStatement;

                if (pStatement == null || pStatement.State == PreparedState.ToBePrepared)
                {
                    if (pStatement?.StatementBeingReplaced != null)
                    {
                        // We have a prepared statement that replaces an existing statement - close the latter first.
                        await connector.CloseMessage
                            .Populate(StatementOrPortal.Statement, pStatement.StatementBeingReplaced.Name)
                            .Write(buf, async);
                    }

                    await connector.ParseMessage
                        .Populate(statement.SQL, statement.StatementName, statement.InputParameters, connector.TypeMapper)
                        .Write(buf, async);
                }

                var bind = connector.BindMessage;
                bind.Populate(statement.InputParameters, "", statement.StatementName);
                if (AllResultTypesAreUnknown)
                    bind.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                else if (i == 0 && UnknownResultTypeList != null)
                    bind.UnknownResultTypeList = UnknownResultTypeList;
                await connector.BindMessage.Write(buf, async);

                if (pStatement == null || pStatement.State == PreparedState.ToBePrepared)
                {
                    await connector.DescribeMessage
                        .Populate(StatementOrPortal.Portal)
                        .Write(buf, async);
                    if (statement.PreparedStatement != null)
                        statement.PreparedStatement.State = PreparedState.BeingPrepared;
                }

                await ExecuteMessage.DefaultExecute.Write(buf, async);
            }
            await SyncMessage.Instance.Write(buf, async);
            await buf.Flush(async);
            CleanupSend();
        }

        async Task SendExecuteSchemaOnly(bool async)
        {
            BeginSend();
            var connector = Connection.Connector;
            Debug.Assert(connector != null);

            var wroteSomething = false;

            var buf = connector.WriteBuffer;
            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];

                if (statement.PreparedStatement?.State == PreparedState.Prepared)
                    continue;   // Prepared, we already have the RowDescription
                Debug.Assert(statement.PreparedStatement == null);

                await connector.ParseMessage
                    .Populate(statement.SQL, "", statement.InputParameters, connector.TypeMapper)
                    .Write(buf, async);

                await connector.DescribeMessage
                    .Populate(StatementOrPortal.Statement, statement.StatementName)
                    .Write(buf, async);
                wroteSomething = true;
            }

            if (wroteSomething)
            {
                await SyncMessage.Instance.Write(buf, async);
                await buf.Flush(async);
            }
            CleanupSend();
        }

        async Task SendDeriveParameters(bool async)
        {
            BeginSend();
            var connector = Connection.Connector;
            Debug.Assert(connector != null);
            var buf = connector.WriteBuffer;
            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                await connector.ParseMessage
                    .Populate(_statements[i].SQL, string.Empty)
                    .Write(buf, async);

                await connector.DescribeMessage
                    .Populate(StatementOrPortal.Statement, string.Empty)
                    .Write(buf, async);
            }
            await SyncMessage.Instance.Write(buf, async);
            await buf.Flush(async);
            CleanupSend();
        }

        async Task SendPrepare(bool async)
        {
            BeginSend();
            var connector = Connection.Connector;
            Debug.Assert(connector != null);
            var buf = connector.WriteBuffer;
            for (var i = 0; i < _statements.Count; i++)
            {
                async = ForceAsyncIfNecessary(async, i);

                var statement = _statements[i];
                var pStatement = statement.PreparedStatement;

                // A statement may be already prepared, already in preparation (i.e. same statement twice
                // in the same command), or we can't prepare (overloaded SQL)
                if (pStatement?.State != PreparedState.ToBePrepared)
                    continue;

                var statementToClose = pStatement?.StatementBeingReplaced;
                if (statementToClose != null)
                {
                    // We have a prepared statement that replaces an existing statement - close the latter first.
                    await connector.CloseMessage
                        .Populate(StatementOrPortal.Statement, statementToClose.Name)
                        .Write(buf, async);
                }

                await connector.ParseMessage
                    .Populate(statement.SQL, pStatement.Name, statement.InputParameters, connector.TypeMapper)
                    .Write(buf, async);

                await connector.DescribeMessage
                    .Populate(StatementOrPortal.Statement, pStatement.Name)
                    .Write(buf, async);

                pStatement.State = PreparedState.BeingPrepared;
            }
            await SyncMessage.Instance.Write(buf, async);
            await buf.Flush(async);
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

        async Task SendClose(bool async)
        {
            BeginSend();
            var connector = Connection.Connector;
            Debug.Assert(connector != null);

            var buf = connector.WriteBuffer;
            foreach (var statement in _statements.Where(s => s.IsPrepared))
            {
                if (FlushOccurred)
                {
                    async = true;
                    SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
                }

                await connector.CloseMessage
                    .Populate(StatementOrPortal.Statement, statement.StatementName)
                    .Write(buf, async);
                Debug.Assert(statement.PreparedStatement != null);
                statement.PreparedStatement.State = PreparedState.BeingUnprepared;
            }
            await SyncMessage.Instance.Write(buf, async);
            await buf.Flush(async);
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
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return ExecuteNonQuery(true, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task<int> ExecuteNonQuery(bool async, CancellationToken cancellationToken)
        {
            using (var reader = await ExecuteDbDataReader(CommandBehavior.Default, async, cancellationToken))
            {
                while (async ? await reader.NextResultAsync(cancellationToken) : reader.NextResult()) {}
                reader.Close();
                return reader.RecordsAffected;
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
        [CanBeNull]
        public override object ExecuteScalar() => ExecuteScalar(false, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronous version of <see cref="ExecuteScalar()"/>
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the first column of the
        /// first row in the result set, or a null reference if the result set is empty.</returns>
        [ItemCanBeNull]
        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return ExecuteScalar(true, cancellationToken).AsTask();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ItemCanBeNull]
        async ValueTask<object> ExecuteScalar(bool async, CancellationToken cancellationToken)
        {
            var behavior = CommandBehavior.SingleRow;
            if (!Parameters.HasOutputParameters)
                behavior |= CommandBehavior.SequentialAccess;
            using (var reader = await ExecuteDbDataReader(behavior, async, cancellationToken))
                return reader.Read() && reader.FieldCount != 0 ? reader.GetValue(0) : null;
        }

        #endregion Execute Scalar

        #region Execute Reader

        /// <summary>
        /// Executes the CommandText against the Connection, and returns an DbDataReader.
        /// </summary>
        /// <remarks>
        /// Unlike the ADO.NET method which it replaces, this method returns a Npgsql-specific
        /// DataReader.
        /// </remarks>
        /// <returns>A DbDataReader object.</returns>
        public new NpgsqlDataReader ExecuteReader() => (NpgsqlDataReader) base.ExecuteReader();

        /// <summary>
        /// Executes the CommandText against the Connection, and returns an DbDataReader using one
        /// of the CommandBehavior values.
        /// </summary>
        /// <remarks>
        /// Unlike the ADO.NET method which it replaces, this method returns a Npgsql-specific
        /// DataReader.
        /// </remarks>
        /// <returns>A DbDataReader object.</returns>
        public new NpgsqlDataReader ExecuteReader(CommandBehavior behavior) => (NpgsqlDataReader) base.ExecuteReader(behavior);

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">An instance of <see cref="CommandBehavior"/>.</param>
        /// <param name="cancellationToken">A task representing the operation.</param>
        /// <returns></returns>
        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return ExecuteDbDataReader(behavior, true, cancellationToken).AsTask();
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        [NotNull]
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => ExecuteDbDataReader(behavior, false, CancellationToken.None).GetAwaiter().GetResult();

        async ValueTask<DbDataReader> ExecuteDbDataReader(CommandBehavior behavior, bool async, CancellationToken cancellationToken)
        {
            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(this);
            try
            {
                using (cancellationToken.Register(cmd => ((NpgsqlCommand)cmd).Cancel(), this))
                {
                    ValidateParameters();
                    if ((behavior & CommandBehavior.SequentialAccess) != 0 && Parameters.HasOutputParameters)
                        throw new NotSupportedException("Output parameters aren't supported with SequentialAccess");

                    if (IsExplicitlyPrepared)
                    {
                        Debug.Assert(_connectorPreparedOn != null);
                        if (_connectorPreparedOn != Connection.Connector)
                        {
                            // The command was prepared, but since then the connector has changed. Detach all prepared statements.
                            foreach (var s in _statements)
                                s.PreparedStatement = null;
                            ResetExplicitPreparation();
                            ProcessRawQuery();
                        }
                    }
                    else
                        ProcessRawQuery();

                    State = CommandState.InProgress;

                    if (Log.IsEnabled(NpgsqlLogLevel.Debug))
                        LogCommand();
                    Task sendTask;

                    // If a cancellation is in progress, wait for it to "complete" before proceeding (#615)
                    lock (connector.CancelLock) { }

                    connector.UserTimeout = CommandTimeout * 1000;

                    if ((behavior & CommandBehavior.SchemaOnly) == 0)
                    {
                        if (connector.Settings.MaxAutoPrepare > 0)
                        {
                            foreach (var statement in _statements)
                            {
                                // If this statement isn't prepared, see if it gets implicitly prepared.
                                // Note that this may return null (not enough usages for automatic preparation).
                                if (!statement.IsPrepared)
                                    statement.PreparedStatement =
                                        connector.PreparedStatementManager.TryGetAutoPrepared(statement);
                                if (statement.PreparedStatement != null)
                                    statement.PreparedStatement.LastUsed = DateTime.UtcNow;
                            }
                            _connectorPreparedOn = connector;
                        }

                        // We do not wait for the entire send to complete before proceeding to reading -
                        // the sending continues in parallel with the user's reading. Waiting for the
                        // entire send to complete would trigger a deadlock for multistatement commands,
                        // where PostgreSQL sends large results for the first statement, while we're sending large
                        // parameter data for the second. See #641.
                        // Instead, all sends for non-first statements and for non-first buffers are performed
                        // asynchronously (even if the user requested sync), in a special synchronization context
                        // to prevents a dependency on the thread pool (which would also trigger deadlocks).
                        // The WriteBuffer notifies this command when the first buffer flush occurs, so that the
                        // send functions can switch to the special async mode when needed.
                        sendTask = SendExecute(async);
                    }
                    else
                    {
                        sendTask = SendExecuteSchemaOnly(async);
                    }

                    // The following is a hack. It raises an exception if one was thrown in the first phases
                    // of the send (i.e. in parts of the send that executed synchronously). Exceptions may
                    // still happen later and aren't properly handled. See #1323.
                    if (sendTask.IsFaulted)
                        sendTask.GetAwaiter().GetResult();

                    //var reader = new NpgsqlDataReader(this, behavior, _statements, sendTask);
                    var reader = (behavior & CommandBehavior.SequentialAccess) == 0
                        ? (NpgsqlDataReader)connector.DefaultDataReader
                        : connector.SequentialDataReader;
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
                Connection.Connector?.EndUserAction();

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
        protected override DbTransaction DbTransaction
        {
            get => Transaction;
            set => Transaction = (NpgsqlTransaction) value;
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlTransaction">NpgsqlTransaction</see>
        /// within which the <see cref="NpgsqlCommand">NpgsqlCommand</see> executes.
        /// </summary>
        /// <value>The <see cref="NpgsqlTransaction">NpgsqlTransaction</see>.
        /// The default value is a null reference.</value>
        public new NpgsqlTransaction Transaction
        {
            get
            {
                if (_transaction != null && _transaction.Connection == null)
                {
                    _transaction = null;
                }
                return _transaction;
            }
            set => _transaction = value;
        }

        #endregion Transactions

        #region Cancel

        /// <summary>
        /// Attempts to cancel the execution of a <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <remarks>As per the specs, no exception will be thrown by this method in case of failure</remarks>
        public override void Cancel()
        {
            var connector = Connection?.Connector;
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
            Connection = null;
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

        void LogCommand()
        {
            var sb = new StringBuilder();
            sb.Append("Executing statement(s):");
            foreach (var s in _statements)
                sb.AppendLine().Append("\t").Append(s.SQL);

            if (NpgsqlLogManager.IsParameterLoggingEnabled && Parameters.Any())
            {
                sb.AppendLine().AppendLine("Parameters:");
                for (var i = 0; i < Parameters.Count; i++)
                    sb.Append("\t$").Append(i + 1).Append(": ").Append(Convert.ToString(Parameters[i].Value, CultureInfo.InvariantCulture));
            }

            Log.Debug(sb.ToString(), Connection.Connector.Id);
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
        [PublicAPI]
        public NpgsqlCommand Clone()
        {
            var clone = new NpgsqlCommand(CommandText, Connection, Transaction)
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
            if (Connection == null)
                throw new InvalidOperationException("Connection property has not been initialized.");
            return Connection.CheckReadyAndGetConnector();
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
