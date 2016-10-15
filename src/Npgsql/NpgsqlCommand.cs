#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using AsyncRewriter;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    /// Represents a SQL statement or function (stored procedure) to execute
    /// against a PostgreSQL database. This class cannot be inherited.
    /// </summary>
#if WITHDESIGN
    [System.Drawing.ToolboxBitmapAttribute(typeof(NpgsqlCommand)), ToolboxItem(true)]
#endif
#if NETSTANDARD1_3
    public sealed partial class NpgsqlCommand : DbCommand
#else
    // ReSharper disable once RedundantNameQualifier
    [System.ComponentModel.DesignerCategory("")]
    public sealed partial class NpgsqlCommand : DbCommand, ICloneable
#endif
    {
        #region Fields

        NpgsqlConnection _connection;
        /// <summary>
        /// Cached version of _connection.Connector, for performance
        /// </summary>
        NpgsqlConnector _connector;
        NpgsqlTransaction _transaction;
        readonly SqlQueryParser _sqlParser = new SqlQueryParser();
        string _commandText;
        int? _timeout;
        readonly NpgsqlParameterCollection _parameters = new NpgsqlParameterCollection();

        List<NpgsqlStatement> _statements;

        /// <summary>
        /// Returns details about each statement that this command has executed.
        /// Is only populated when an Execute* method is called.
        /// </summary>
        public IReadOnlyList<NpgsqlStatement> Statements => _statements.AsReadOnly();

        [CanBeNull]
        internal Task CurrentSendTask;

        UpdateRowSource _updateRowSource = UpdateRowSource.Both;

        /// <summary>
        /// Indicates whether this command has been prepared.
        /// Never access this field directly, use <see cref="IsPrepared"/> instead.
        /// </summary>
        bool _isPrepared;

        /// <summary>
        /// For prepared commands, captures the connection's <see cref="NpgsqlConnection.OpenCounter"/>
        /// at the time the command was prepared. This allows us to know whether the connection was
        /// closed since the command was prepared.
        /// </summary>
        int _prepareConnectionOpenId;

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
            Init(cmdText);
            Connection = connection;
            Transaction = transaction;
        }

        void Init(string cmdText)
        {
            _commandText = cmdText;
            CommandType = CommandType.Text;
            _statements = new List<NpgsqlStatement>();
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
            get { return _commandText; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _commandText = value;
                Unprepare();
            }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt  to execute a command and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for the command to execute. The default value is 30 seconds.</value>
        [DefaultValue(DefaultTimeout)]
        public override int CommandTimeout
        {
            get
            {
                return _timeout ?? (_connection?.CommandTimeout ?? DefaultTimeout);
            }
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
            get { return Connection; }
            set { Connection = (NpgsqlConnection)value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlConnection">NpgsqlConnection</see>
        /// used by this instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <value>The connection to a data source. The default value is a null reference.</value>
        [DefaultValue(null)]
        [Category("Behavior")]
        public new NpgsqlConnection Connection
        {
            get { return _connection; }
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
                {
                    throw new InvalidOperationException("The Connection property can't be changed with an uncommited transaction.");
                }

                IsPrepared = false;
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
        public bool IsPrepared
        {
            get
            {
                if (_isPrepared)
                {
                    Debug.Assert(Connection != null);
                    if (Connection.State != ConnectionState.Open || _prepareConnectionOpenId != Connection.OpenCounter) {
                        _isPrepared = false;
                    }
                }
                return _isPrepared;
            }

            private set
            {
                Debug.Assert(!value || Connection != null);
                _isPrepared = value;
                if (value) {
                    _prepareConnectionOpenId = Connection.OpenCounter;
                }
            }
        }

        /// <summary>
        /// Returns whether this is a prepared statement that will persist when its connection is returned to the pool.
        /// </summary>
        public bool IsPersistent { get; private set; }

        #endregion Public properties

        #region Known/unknown Result Types Management

        /// <summary>
        /// Marks all of the query's result columns as either known or unknown.
        /// Unknown results column are requested them from PostgreSQL in text format, and Npgsql makes no
        /// attempt to parse them. They will be accessible as strings only.
        /// </summary>
        public bool AllResultTypesAreUnknown
        {
            get { return _allResultTypesAreUnknown; }
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
            get { return _unknownResultTypeList; }
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
#if WITHDESIGN
        [Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif

        public new NpgsqlParameterCollection Parameters => _parameters;

        #endregion

        #region Prepare

        /// <summary>
        /// Creates a prepared version of the command on a PostgreSQL server.
        /// </summary>
        public override void Prepare()
        {
            _connector = CheckReadyAndGetConnector();
            Prepare(_connector.Connection.Settings.PersistPrepared);
        }

        /// <summary>
        /// Creates a prepared version of the command on a PostgreSQL server.
        /// </summary>
        /// <param name="persist">
        /// If set to true, prepared statements are persisted when a pooled connection is closed for later use.
        /// </param>
        public void Prepare(bool persist)
        {
            _connector = CheckReadyAndGetConnector();
            if (Parameters.Any(p => !p.IsTypeExplicitlySet))
                throw new InvalidOperationException("The Prepare method requires all parameters to have an explicitly set type.");

            Unprepare();
            ProcessRawQuery();

            if (Log.IsEnabled(NpgsqlLogLevel.Debug))
            {
                var sb = new StringBuilder("Preparing");
                if (persist)
                    sb.Append(" (persistent)");
                sb.Append(": ").Append(CommandText);
                Log.Debug(sb.ToString(), _connector.Id);
            }

            using (_connector.StartUserAction())
            {
                Debug.Assert(Statements.All(s => !s.IsPrepared));

                var needToPrepare = false;
                if (persist)
                {
                    foreach (var statement in Statements)
                    {
                        PreparedStatementInfo statementInfo;
                        if (_connector.PersistentPreparedStatements.TryGetValue(statement.SQL, out statementInfo))
                        {
                            // Statement has already been prepared, no need to do anything
                            statement.IsPrepared = true;
                            statement.PreparedStatementName = statementInfo.Name;
                            statement.Description = statementInfo.Description;
                        }
                        else
                        {
                            // Statement hasn't been prepared yet
                            statement.PreparedStatementName = _connector.NextPersistentPreparedStatementName();
                            needToPrepare = true;
                        }
                    }
                }
                else  // Non-persisting prepare
                {
                    foreach (var statement in Statements)
                        statement.PreparedStatementName = _connector.NextPreparedStatementName();
                    needToPrepare = true;
                }

                if (needToPrepare)
                {
                    SendParseDescribe(false, CancellationToken.None).Wait();

                    // Loop over statements, skipping those that are already prepared (because they were persisted)
                    var isFirst = true;
                    foreach (var statement in Statements.Where(s => !s.IsPrepared))
                    {
                        _connector.ReadExpecting<ParseCompleteMessage>();
                        _connector.ReadExpecting<ParameterDescriptionMessage>();
                        var msg = _connector.ReadMessage(DataRowLoadingMode.NonSequential);
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
                            throw _connector.UnexpectedMessageReceived(msg.Code);
                        }
                        statement.IsPrepared = true;
                        if (persist)
                        {
                            _connector.PersistentPreparedStatements.Add(statement.SQL, new PreparedStatementInfo
                            {
                                Name = statement.PreparedStatementName,
                                Description = statement.Description
                            });
                        }
                        else
                            _connector.PreparedStatements.Add(statement.PreparedStatementName);
                        isFirst = false;
                    }

                    _connector.ReadExpecting<ReadyForQueryMessage>();
                }

                CompleteRemainingSend();

                IsPrepared = true;
                IsPersistent = persist;

                Debug.Assert(Statements.All(s => s.IsPrepared));
                Debug.Assert(Statements.All(s => s.PreparedStatementName != null));
            }
        }

        void Unprepare()
        {
            if (!IsPrepared)
                return;

            // For persistent prepared commands, we don't actually Close the server-side statements,
            // we just detach our statements from the server-side ones.
            if (IsPersistent)
            {
                foreach (var statement in Statements)
                    statement.Unprepare();
                IsPrepared = false;
                IsPersistent = false;
            }
            else
            {
                foreach (var statement in Statements)
                    _connector.PreparedStatements.Remove(statement.PreparedStatementName);
                ClosePreparedStatements();
            }
        }

        /// <summary>
        /// Unpersists a persistent prepared command.
        /// </summary>
        public void Unpersist()
        {
            // We want to allow users to unpersist without having to persist first
            if (!IsPersistent)
                Prepare(true);

            _connector = CheckReadyAndGetConnector();
            foreach (var statement in Statements)
                _connector.PersistentPreparedStatements.Remove(statement.SQL);
            ClosePreparedStatements();
        }

        void ClosePreparedStatements()
        {
            Debug.Assert(IsPrepared);
            Debug.Assert(Statements.All(s => s.IsPrepared));

            _connector = CheckReadyAndGetConnector();
            using (_connector.StartUserAction())
            {
                SendClose(false, CancellationToken.None).Wait();
                foreach (var statement in Statements)
                {
                    _connector.ReadExpecting<CloseCompletedMessage>();
                    statement.Unprepare();
                }
                _connector.ReadExpecting<ReadyForQueryMessage>();

                CompleteRemainingSend();

                IsPrepared = false;
                IsPersistent = false;
            }
        }

        #endregion Prepare

        #region Query analysis

        void ProcessRawQuery()
        {
            NpgsqlStatement statement;
            switch (CommandType) {
            case CommandType.Text:
                _sqlParser.ParseRawQuery(CommandText, _connection == null || _connection.UseConformantStrings, _parameters, _statements);
                if (_statements.Count > 1 && _parameters.Any(p => p.IsOutputDirection))
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
                bool hasWrittenFirst = false;
                for (var i = 1; i <= numInput; i++) {
                    var param = inputList[i - 1];
                    if (param.AutoAssignedName || param.CleanName == "")
                    {
                        if (hasWrittenFirst)
                        {
                            sb.Append(',');
                        }
                        sb.Append('$');
                        sb.Append(i);
                        hasWrittenFirst = true;
                    }
                }
                for (var i = 1; i <= numInput; i++)
                {
                    var param = inputList[i - 1];
                    if (!param.AutoAssignedName && param.CleanName != "")
                    {
                        if (hasWrittenFirst)
                        {
                            sb.Append(',');
                        }
                        sb.Append('"');
                        sb.Append(param.CleanName.Replace("\"", "\"\""));
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
        }

        #endregion

        #region Execute

        void ValidateParameters()
        {
            foreach (NpgsqlParameter p in Parameters.Where(p => p.IsInputDirection))
            {
                p.Bind(Connection.Connector.TypeHandlerRegistry);
                p.LengthCache?.Clear();
                p.ValidateAndGetLength();
            }
        }

        async Task<NpgsqlDataReader> Execute(CommandBehavior behavior, bool async, CancellationToken cancellationToken)
        {
            ValidateParameters();
            if (!IsPrepared)
                ProcessRawQuery();
            if (Statements.Any(s => s.InputParameters.Count > 65535))
                throw new Exception("A statement cannot have more than 65535 parameters");
            LogCommand();

            State = CommandState.InProgress;
            try
            {
                _connector = Connection.Connector;

                // If a cancellation is in progress, wait for it to "complete" before proceeding (#615)
                lock (_connector.CancelLock) { }

                // Send protocol messages for the command
                // Unless this is a prepared SchemaOnly command, in which case we already have the RowDescriptions
                // from the Prepare phase (no need to send anything).
                if (!IsPrepared || (behavior & CommandBehavior.SchemaOnly) == 0)
                {
                    _connector.UserTimeout = CommandTimeout * 1000;

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

                    if (IsPrepared)
                        CurrentSendTask = SendWrapper(SendExecutePrepared, async, cancellationToken);
                    else if ((behavior & CommandBehavior.SchemaOnly) == 0)
                        CurrentSendTask = SendWrapper(SendExecuteNonPrepared, async, cancellationToken);
                    else
                        CurrentSendTask = SendWrapper(SendParseDescribe, async, cancellationToken);

                    // The following is a hack. It raises an exception if one was thrown in the first phases
                    // of the send (i.e. in parts of the send that executed synchronously). Exceptions may
                    // still happen later and aren't properly handled. See #1323.
                    if (CurrentSendTask.IsFaulted)
                        CurrentSendTask.GetAwaiter().GetResult();
                }

                var reader = new NpgsqlDataReader(this, behavior, _statements);
                if (async)
                    await reader.NextResultAsync(cancellationToken);
                else
                    reader.NextResult();
                _connector.CurrentReader = reader;
                return reader;
            }
            catch
            {
                State = CommandState.Idle;
                throw;
            }
        }

        #endregion

        #region Send

        /// <summary>
        /// Waits until any background async send task completes.
        /// </summary>
        internal void CompleteRemainingSend()
        {
            if (CurrentSendTask != null)
            {
                CurrentSendTask.Wait();
                CurrentSendTask = null;
            }
        }

        #endregion

        #region Message Creation / Population

        internal bool FlushOccurred { get; set; }

        async Task SendWrapper(Func<bool, CancellationToken, Task> sender, bool async, CancellationToken cancellationToken)
        {
            _connector.WriteBuffer.CurrentCommand = this;
            FlushOccurred = false;
            await sender(async, cancellationToken);
            _connector.WriteBuffer.CurrentCommand = null;
            SynchronizationContext.SetSynchronizationContext(null);

            /*
            return sender(async, cancellationToken).ContinueWith((t,o) =>
            {
                if (!t.IsFaulted)
                {
                    ((WriteBuffer)o).CurrentCommand = null;
                    SynchronizationContext.SetSynchronizationContext(null);
                }
            }, _connector.WriteBuffer, cancellationToken);
            */
        }

        /// <summary>
        /// Populates the send buffer with protocol messages for the execution of non-prepared statement(s).
        /// </summary>
        /// <returns>
        /// true whether all messages could be populated in the buffer, false otherwise (method needs to be
        /// called again)
        /// </returns>
        async Task SendExecuteNonPrepared(bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(_connector != null);

            var buf = _connector.WriteBuffer;
            for (var i = 0; i < _statements.Count; i++)
            {
                if (FlushOccurred)
                {
                    async = true;
                    SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
                }

                var statement = _statements[i];

                await _connector.ParseMessage
                    .Populate(statement, _connector.TypeHandlerRegistry)
                    .Write(buf, async, cancellationToken);

                var bind = _connector.BindMessage;
                bind.Populate(statement.InputParameters);
                if (AllResultTypesAreUnknown)
                    bind.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                else if (i == 0 && UnknownResultTypeList != null)
                    bind.UnknownResultTypeList = UnknownResultTypeList;
                await _connector.BindMessage.Write(buf, async, cancellationToken);

                await _connector.DescribeMessage
                    .Populate(StatementOrPortal.Portal)
                    .Write(buf, async, cancellationToken);

                await _connector.ExecuteMessage
                    .Populate()
                    .Write(buf, async, cancellationToken);
            }
            await SyncMessage.Instance.Write(buf, async, cancellationToken);
            await buf.Flush(async, cancellationToken);
        }

        /// <summary>
        /// Populates the send buffer with protocol messages for the execution of prepared statement(s).
        /// </summary>
        /// <returns>
        /// true whether all messages could be populated in the buffer, false otherwise (method needs to be
        /// called again)
        /// </returns>
        async Task SendExecutePrepared(bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(_connector != null);

            var buf = _connector.WriteBuffer;
            for (var i = 0; i < _statements.Count; i++)
            {
                if (FlushOccurred)
                {
                    async = true;
                    SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
                }

                var statement = _statements[i];

                var bind = _connector.BindMessage;
                bind.Populate(statement.InputParameters, "", statement.PreparedStatementName);
                if (AllResultTypesAreUnknown)
                    bind.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                else if (i == 0 && UnknownResultTypeList != null)
                    bind.UnknownResultTypeList = UnknownResultTypeList;
                await _connector.BindMessage.Write(buf, async, cancellationToken);

                await _connector.ExecuteMessage
                    .Populate()
                    .Write(buf, async, cancellationToken);
            }
            await SyncMessage.Instance.Write(buf, async, cancellationToken);
            await buf.Flush(async, cancellationToken);
        }

        async Task SendParseDescribe(bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(_connector != null);

            var buf = _connector.WriteBuffer;
            foreach (var statement in _statements)
            {
                // Statement doesn't need preparation (persistent)
                if (statement.IsPrepared)
                    continue;
                if (FlushOccurred)
                {
                    async = true;
                    SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
                }

                await _connector.ParseMessage
                    .Populate(statement, _connector.TypeHandlerRegistry)
                    .Write(buf, async, cancellationToken);

                await _connector.DescribeMessage
                    .Populate(StatementOrPortal.Statement, statement.PreparedStatementName)
                    .Write(buf, async, cancellationToken);
            }
            await SyncMessage.Instance.Write(buf, async, cancellationToken);
            await buf.Flush(async, cancellationToken);
        }

        async Task SendClose(bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(_connector != null);

            var buf = _connector.WriteBuffer;
            foreach (var statement in _statements)
            {
                if (FlushOccurred)
                {
                    async = true;
                    SynchronizationContext.SetSynchronizationContext(SingleThreadSynchronizationContext);
                }

                await _connector.CloseMessage
                    .Populate(StatementOrPortal.Statement, statement.PreparedStatementName)
                    .Write(buf, async, cancellationToken);
            }
            await SyncMessage.Instance.Write(buf, async, cancellationToken);
            await buf.Flush(async, cancellationToken);
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
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
            using (cancellationToken.Register(Cancel))
                return await ExecuteNonQuery(true, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task<int> ExecuteNonQuery(bool async, CancellationToken cancellationToken)
        {
            var connector = CheckReadyAndGetConnector();
            using (connector.StartUserAction(this))
            {
                Log.Trace("ExecuteNonQuery", connector.Id);

                // Optimization: unprepared unparameterized non-queries can go through the PostgreSQL
                // simple protocol which is more efficient
                if (!IsPrepared && Parameters.Count == 0)
                    return await ExecuteSimple(async, cancellationToken); // TODO

                using (var reader = await Execute(CommandBehavior.Default, async, cancellationToken))
                {
                    while (async ? await reader.NextResultAsync(cancellationToken) : reader.NextResult()) {}
                    reader.Close();
                    return reader.RecordsAffected;
                }
            }
        }

        async Task<int> ExecuteSimple(bool async, CancellationToken cancellationToken)
        {
            ProcessRawQuery();
            LogCommand();

            var connector = Connection.Connector;

            // If a cancellation is in progress, wait for it to "complete" before proceeding (#615)
            lock (connector.CancelLock) { }

            connector.UserTimeout = CommandTimeout * 1000;
            State = CommandState.InProgress;

            try
            {
                await connector.QueryMessage
                    .Populate(CommandText)
                    .Write(connector.WriteBuffer, async, cancellationToken);
                await connector.WriteBuffer.Flush(async, cancellationToken);

                var statementIndex = 0;
                var recordsAffected = 0;
                while (true)
                {
                    var msg = connector.ReadMessage(DataRowLoadingMode.Skip);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.CompletedResponse:
                        var completedMsg = (CommandCompleteMessage)msg;
                        Statements[statementIndex++].ApplyCommandComplete(completedMsg);
                        recordsAffected += (int)completedMsg.Rows;
                        continue;
                    case BackendMessageCode.EmptyQueryResponse:
                        statementIndex++;
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        return recordsAffected;
                    default:
                        continue;
                    }
                }
            }
            finally
            {
                State = CommandState.Idle;
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
        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
            using (cancellationToken.Register(Cancel))
                return await ExecuteScalar(true, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ItemCanBeNull]
        async Task<object> ExecuteScalar(bool async, CancellationToken cancellationToken)
        {
            var connector = CheckReadyAndGetConnector();
            using (connector.StartUserAction(this))
            {
                Log.Trace("ExecuteNonScalar", connector.Id);
                using (var reader = await Execute(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow, async, cancellationToken))
                    return reader.Read() && reader.FieldCount != 0 ? reader.GetValue(0) : null;
            }
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
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
            using (cancellationToken.Register(Cancel))
                return await ExecuteDbDataReader(behavior, true, cancellationToken);
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        [NotNull]
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => ExecuteDbDataReader(behavior, false, CancellationToken.None).GetAwaiter().GetResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task<NpgsqlDataReader> ExecuteDbDataReader(CommandBehavior behavior, bool async, CancellationToken cancellationToken)
        {
            var connector = CheckReadyAndGetConnector();
            connector.StartUserAction(this);
            try
            {
                Log.Trace("ExecuteReader", connector.Id);
                return await Execute(behavior, async, cancellationToken);
            }
            catch
            {
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
            get { return Transaction; }
            set { Transaction = (NpgsqlTransaction) value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="NpgsqlTransaction">NpgsqlTransaction</see>
        /// within which the <see cref="NpgsqlCommand">NpgsqlCommand</see> executes.
        /// </summary>
        /// <value>The <see cref="NpgsqlTransaction">NpgsqlTransaction</see>.
        /// The default value is a null reference.</value>
#if WITHDESIGN
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
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
            set { _transaction = value; }
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
            {
                Log.Debug($"Skipping cancel because command is in state {State}", connector.Id);
                return;
            }

            Log.Debug("Cancelling command", connector.Id);
            try
            {
                connector.CancelRequest();
            }
            catch (Exception e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null || socketException.SocketErrorCode != SocketError.ConnectionReset)
                    Log.Debug("Exception caught while attempting to cancel command", e, connector.Id);
            }
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

            if (disposing)
            {
                // Note: we only actually perform cleanup here if called from Dispose() (disposing=true), and not
                // if called from a finalizer (disposing=false). This is because we cannot perform any SQL
                // operations from the finalizer (connection may be in use by someone else). Prepared statements
                // which aren't explicitly disposed are leaked until the connection is closed.
                if (IsPrepared)
                    Unprepare();
            }
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
            if (!Log.IsEnabled(NpgsqlLogLevel.Debug))
                return;

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

#if NET45 || NET451
        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

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
