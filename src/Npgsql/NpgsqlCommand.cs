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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
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
        String _commandText;
        int? _timeout;
        readonly NpgsqlParameterCollection _parameters = new NpgsqlParameterCollection();

        List<NpgsqlStatement> _statements;

        /// <summary>
        /// Returns details about each statement that this command has executed.
        /// Is only populated when an Execute* method is called.
        /// </summary>
        public IReadOnlyList<NpgsqlStatement> Statements => _statements.AsReadOnly();

        int _readStatementIndex;
        int _writeStatementIndex;

        /// <summary>
        /// If part of the send happens asynchronously (see <see cref="SendRemaining"/>,
        /// the Task for that remaining send is stored here.
        /// </summary>
        internal Task RemainingSendTask;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Constants

        internal const int DefaultTimeout = 30;

        /// <summary>
        /// Specifies the maximum number of statements we allow in a multiquery, separated by semicolons.
        /// We limit this because of deadlocks: as we send Parse and Bind messages to the backend, the backend
        /// replies with ParseComplete and BindComplete messages which we do not read until we finished sending
        /// all messages. Once our buffer gets full the backend will get stuck writing, and then so will we.
        /// </summary>
        public const int MaxStatements = 5000;

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
                Contract.EndContractBlock();

                _commandText = value;
                DeallocatePrepared();
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
        /// Gets or sets how command results are applied to the <see cref="System.Data.DataRow">DataRow</see>
        /// when used by the <see cref="System.Data.Common.DbDataAdapter.Update(DataSet)">Update</see>
        /// method of the <see cref="System.Data.Common.DbDataAdapter">DbDataAdapter</see>.
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
                    Contract.Assert(Connection != null);
                    if (Connection.State != ConnectionState.Open || _prepareConnectionOpenId != Connection.OpenCounter) {
                        _isPrepared = false;
                    }
                }
                return _isPrepared;
            }

            private set
            {
                Contract.Requires(!value || Connection != null);
                _isPrepared = value;
                if (value) {
                    _prepareConnectionOpenId = Connection.OpenCounter;
                }
            }
        }

        #endregion Public properties

        #region Known/unknown Result Types Management

        /// <summary>
        /// Marks all of the query's result columns as either known or unknown.
        /// Unknown results column are requested them from PostgreSQL in text format, and Npgsql makes no
        /// attempt to parse them. They will be accessible as strings only.
        /// </summary>
        public bool AllResultTypesAreUnknown
        {
            private get { return _allResultTypesAreUnknown; }
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
            private get { return _unknownResultTypeList; }
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

        SendState _sendState;

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
            Prechecks();
            if (Parameters.Any(p => !p.IsTypeExplicitlySet))
                throw new InvalidOperationException("The Prepare method requires all parameters to have an explicitly set type.");

            _connector = Connection.Connector;
            Log.Debug("Preparing: " + CommandText, _connector.Id);

            using (_connector.StartUserAction())
            {
                DeallocatePrepared();
                ProcessRawQuery();

                _sendState = SendState.Start;
                _writeStatementIndex = 0;
                Send(PopulatePrepare);

                _readStatementIndex = 0;

                while (true)
                {
                    var msg = _connector.ReadSingleMessage(DataRowLoadingMode.NonSequential);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.CompletedResponse: // prepended messages, e.g. begin transaction
                    case BackendMessageCode.ParseComplete:
                    case BackendMessageCode.ParameterDescription:
                        continue;
                    case BackendMessageCode.RowDescription:
                        var description = (RowDescriptionMessage) msg;
                        FixupRowDescription(description, _readStatementIndex == 0);
                        _statements[_readStatementIndex++].Description = description;
                        continue;
                    case BackendMessageCode.NoData:
                        _statements[_readStatementIndex++].Description = null;
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        Contract.Assume(_readStatementIndex == _statements.Count);
                        IsPrepared = true;
                        return;
                    default:
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    }
                }
            }
        }

        void DeallocatePrepared()
        {
            if (!IsPrepared) { return; }
            using (_connector.StartUserAction())
            {
                _writeStatementIndex = 0;
                Send(PopulateDeallocate);
                for (var i = 0; i < _statements.Count; i++)
                    _connector.ReadExpecting<CloseCompletedMessage>();
                _connector.ReadExpecting<ReadyForQueryMessage>();
                IsPrepared = false;
            }
        }

        #endregion Prepare

        #region Query analysis

        void ProcessRawQuery()
        {
            _statements.Clear();
            switch (CommandType) {
            case CommandType.Text:
                SqlQueryParser.ParseRawQuery(CommandText, _connection == null || _connection.UseConformantStrings, _parameters, _statements);
                if (_statements.Count > 1 && _parameters.Any(p => p.IsOutputDirection)) {
                    throw new NotSupportedException("Commands with multiple queries cannot have out parameters");
                }
                break;
            case CommandType.TableDirect:
                _statements.Add(new NpgsqlStatement("SELECT * FROM " + CommandText, new List<NpgsqlParameter>()));
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
                _statements.Add(new NpgsqlStatement(sb.ToString(), inputList));
                break;
            default:
                throw PGUtil.ThrowIfReached();
            }
        }

        #endregion

        #region Execute

        void Validate()
        {
            if (Parameters.Count > 65535)
                throw new Exception("A command cannot have more than 65535 parameters");
            foreach (NpgsqlParameter p in Parameters.Where(p => p.IsInputDirection))
            {
                p.Bind(Connection.Connector.TypeHandlerRegistry);
                p.LengthCache?.Clear();
                p.ValidateAndGetLength();
            }
        }

        [RewriteAsync]
        NpgsqlDataReader Execute(CommandBehavior behavior = CommandBehavior.Default)
        {
            Validate();
            if (!IsPrepared)
                ProcessRawQuery();
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

                    _sendState = SendState.Start;
                    _writeStatementIndex = 0;

                    if (IsPrepared)
                        Send(PopulateExecutePrepared);
                    else if ((behavior & CommandBehavior.SchemaOnly) == 0)
                        Send(PopulateExecuteNonPrepared);
                    else
                        Send(PopulateExecuteSchemaOnly);
                }

                var reader = new NpgsqlDataReader(this, behavior, _statements);
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

        delegate bool PopulateMethod(ref DirectBuffer directBuf);

        [RewriteAsync]
        void Send(PopulateMethod populateMethod)
        {
            while (true)
            {
                var directBuf = new DirectBuffer();
                var completed = populateMethod(ref directBuf);
                _connector.SendBuffer();
                if (completed)
                    break;  // Sent all messages

                // The following is an optimization hack for writing large byte arrays without passing
                // through our buffer
                if (directBuf.Buffer != null)
                {
                    _connector.Stream.Write(directBuf.Buffer, directBuf.Offset, directBuf.Size == 0 ? directBuf.Buffer.Length : directBuf.Size);
                    directBuf.Buffer = null;
                    directBuf.Size = 0;
                }

                if (_writeStatementIndex > 0)
                {
                    // We've send all the messages for the first statement in a multistatement command.
                    // If we continue blocking writes for the rest of the messages, we risk a deadlock where
                    // PostgreSQL sends large results for the first statement, while we're sending large
                    // parameter data for the second. To avoid this, switch to async sends.
                    // See #641
                    RemainingSendTask = SendRemaining(populateMethod, CancellationToken.None);
                    return;
                }
            }
        }

        /// <summary>
        /// This method is used to asynchronously sends all remaining protocol messages for statements
        /// beyond the first one, and *without* waiting for the send to complete. This technique is
        /// used to avoid the deadlock described in #641 by allowing the user to read query results
        /// while at the same time sending messages for later statements.
        /// </summary>
        async Task SendRemaining(PopulateMethod populateMethod, CancellationToken cancellationToken)
        {
            Contract.Requires(_writeStatementIndex > 0);
            try
            {
                while (true)
                {
                    var directBuf = new DirectBuffer();
                    var completed = populateMethod(ref directBuf);
                    await _connector.SendBufferAsync(cancellationToken);
                    if (completed)
                        return; // Sent all messages

                    // The following is an optimization hack for writing large byte arrays without passing
                    // through our buffer
                    if (directBuf.Buffer != null)
                    {
                        await _connector.Stream.WriteAsync(directBuf.Buffer, directBuf.Offset,
                                directBuf.Size == 0 ? directBuf.Buffer.Length : directBuf.Size, cancellationToken);
                        directBuf.Buffer = null;
                        directBuf.Size = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception while asynchronously sending remaining messages", e, _connector.Id);
            }
        }

        #endregion

        #region Message Creation / Population

        /// <summary>
        /// Populates the send buffer with protocol messages for the execution of non-prepared statement(s).
        /// </summary>
        /// <returns>
        /// true whether all messages could be populated in the buffer, false otherwise (method needs to be
        /// called again)
        /// </returns>
        bool PopulateExecuteNonPrepared(ref DirectBuffer directBuf)
        {
            Contract.Requires(_connector != null);

            var buf = _connector.WriteBuffer;
            for (; _writeStatementIndex < _statements.Count; _writeStatementIndex++)
            {
                var statement = _statements[_writeStatementIndex];
                switch (_sendState)
                {
                case SendState.Start:
                    _connector.ParseMessage.Populate(statement, _connector.TypeHandlerRegistry);
                    _sendState = SendState.Parse;
                    goto case SendState.Parse;

                case SendState.Parse:
                    if (!_connector.ParseMessage.Write(buf))
                        return false;
                    var bind = _connector.BindMessage;
                    bind.Populate(statement.InputParameters);
                    if (AllResultTypesAreUnknown)
                        bind.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                    else if (_writeStatementIndex == 0 && UnknownResultTypeList != null)
                        bind.UnknownResultTypeList = UnknownResultTypeList;
                    _sendState = SendState.Bind;
                    goto case SendState.Bind;

                case SendState.Bind:
                    if (!_connector.BindMessage.Write(buf, ref directBuf))
                        return false;
                    var describe = _connector.DescribeMessage;
                    describe.Populate(StatementOrPortal.Portal);
                    _sendState = SendState.Describe;
                    goto case SendState.Describe;

                case SendState.Describe:
                    describe = _connector.DescribeMessage;
                    if (describe.Length > buf.WriteSpaceLeft)
                        return false;
                    describe.WriteFully(buf);
                    var execute = _connector.ExecuteMessage;
                    execute.Populate();
                    _sendState = SendState.Execute;
                    goto case SendState.Execute;

                case SendState.Execute:
                    execute = _connector.ExecuteMessage;
                    if (execute.Length > buf.WriteSpaceLeft)
                        return false;
                    execute.WriteFully(buf);
                    _sendState = SendState.Start;
                    continue;

                default:
                    throw new ArgumentOutOfRangeException($"Invalid state {_sendState} in {nameof(PopulateExecuteNonPrepared)}");
                }
            }
            if (SyncMessage.Instance.Length > buf.WriteSpaceLeft)
                return false;
            SyncMessage.Instance.WriteFully(buf);
            return true;
        }

        /// <summary>
        /// Populates the send buffer with protocol messages for the execution of prepared statement(s).
        /// </summary>
        /// <returns>
        /// true whether all messages could be populated in the buffer, false otherwise (method needs to be
        /// called again)
        /// </returns>
        bool PopulateExecutePrepared(ref DirectBuffer directBuf)
        {
            Contract.Requires(_connector != null);

            var buf = _connector.WriteBuffer;
            for (; _writeStatementIndex < _statements.Count; _writeStatementIndex++)
            {
                var statement = _statements[_writeStatementIndex];
                switch (_sendState)
                {
                case SendState.Start:
                    var bind = _connector.BindMessage;
                    bind.Populate(statement.InputParameters, "", statement.PreparedStatementName);
                    if (AllResultTypesAreUnknown)
                        bind.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                    else if (_writeStatementIndex == 0 && UnknownResultTypeList != null)
                        bind.UnknownResultTypeList = UnknownResultTypeList;
                    _sendState = SendState.Bind;
                    goto case SendState.Bind;

                case SendState.Bind:
                    if (!_connector.BindMessage.Write(buf, ref directBuf))
                        return false;
                    var execute = _connector.ExecuteMessage;
                    execute.Populate();
                    _sendState = SendState.Execute;
                    goto case SendState.Execute;

                case SendState.Execute:
                    execute = _connector.ExecuteMessage;
                    if (execute.Length > buf.WriteSpaceLeft)
                        return false;
                    execute.WriteFully(buf);
                    _sendState = SendState.Start;
                    continue;

                default:
                    throw new ArgumentOutOfRangeException($"Invalid state {_sendState} in {nameof(PopulateExecutePrepared)}");
                }
            }
            if (SyncMessage.Instance.Length > buf.WriteSpaceLeft)
                return false;
            SyncMessage.Instance.WriteFully(buf);
            return true;
        }

        /// <summary>
        /// Populates the send buffer with Parse/Describe protocol messages, used for preparing commands
        /// and for execution in SchemaOnly mode.
        /// </summary>
        /// <returns>
        /// true whether all messages could be populated in the buffer, false otherwise (method needs to be
        /// called again)
        /// </returns>
        bool PopulatePrepare(ref DirectBuffer directBuf) => PopulateParseDescribe(true);

        /// <summary>
        /// Populates the send buffer with Parse/Describe protocol messages, used for preparing commands
        /// and for execution in SchemaOnly mode.
        /// </summary>
        /// <returns>
        /// true whether all messages could be populated in the buffer, false otherwise (method needs to be
        /// called again)
        /// </returns>
        bool PopulateExecuteSchemaOnly(ref DirectBuffer directBuf) => PopulateParseDescribe(false);

        bool PopulateParseDescribe(bool isPreparing)
        {
            Contract.Requires(_connector != null);

            var buf = _connector.WriteBuffer;
            for (; _writeStatementIndex < _statements.Count; _writeStatementIndex++)
            {
                var statement = _statements[_writeStatementIndex];
                switch (_sendState)
                {
                case SendState.Start:
                    if (isPreparing)
                        statement.PreparedStatementName = _connector.NextPreparedStatementName();
                    _connector.ParseMessage.Populate(statement, _connector.TypeHandlerRegistry);
                    _sendState = SendState.Parse;
                    goto case SendState.Parse;

                case SendState.Parse:
                    if (!_connector.ParseMessage.Write(buf))
                        return false;
                    var describe = _connector.DescribeMessage;
                    describe.Populate(StatementOrPortal.Statement, statement.PreparedStatementName);
                    _sendState = SendState.Describe;
                    goto case SendState.Describe;

                case SendState.Describe:
                    describe = _connector.DescribeMessage;
                    if (describe.Length > buf.WriteSpaceLeft)
                        return false;
                    describe.WriteFully(buf);
                    _sendState = SendState.Start;
                    continue;

                default:
                    throw new ArgumentOutOfRangeException($"Invalid state {_sendState} in {nameof(PopulateParseDescribe)}");
                }
            }
            if (SyncMessage.Instance.Length > buf.WriteSpaceLeft)
                return false;
            SyncMessage.Instance.WriteFully(buf);
            return true;
        }

        bool PopulateDeallocate(ref DirectBuffer directBuf)
        {
            Contract.Requires(_connector != null);

            var buf = _connector.WriteBuffer;
            for (; _writeStatementIndex < _statements.Count; _writeStatementIndex++)
            {
                var statement = _statements[_writeStatementIndex];
                var closeMsg = new CloseMessage(StatementOrPortal.Statement, statement.PreparedStatementName);
                if (closeMsg.Length > buf.WriteSpaceLeft)
                    return false;
                closeMsg.WriteFully(buf);
            }
            if (SyncMessage.Instance.Length > buf.WriteSpaceLeft)
                return false;
            SyncMessage.Instance.WriteFully(buf);
            return true;
        }

        #endregion

        #region Execute Non Query

        /// <summary>
        /// Executes a SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected if known; -1 otherwise.</returns>
        public override int ExecuteNonQuery()
        {
            return ExecuteNonQueryInternal();
        }

        /// <summary>
        /// Asynchronous version of <see cref="ExecuteNonQuery"/>
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the number of rows affected if known; -1 otherwise.</returns>
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (cancellationToken.Register(Cancel))
            {
                try
                {
                    return await ExecuteNonQueryInternalAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (NpgsqlException e)
                {
                    if (e.Code == "57014")
                        throw new TaskCanceledException(e.Message);
                    throw;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [RewriteAsync]
        int ExecuteNonQueryInternal()
        {
            Prechecks();
            Log.Trace("ExecuteNonQuery", Connection.Connector.Id);
            using (Connection.Connector.StartUserAction())
            {
                NpgsqlDataReader reader;
                using (reader = Execute())
                {
                    while (reader.NextResult())
                    {
                    }
                }
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
        public override object ExecuteScalar()
        {
            return ExecuteScalarInternal();
        }

        /// <summary>
        /// Asynchronous version of <see cref="ExecuteScalar"/>
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the first column of the
        /// first row in the result set, or a null reference if the result set is empty.</returns>
        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (cancellationToken.Register(Cancel))
            {
                try
                {
                    return await ExecuteScalarInternalAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (NpgsqlException e)
                {
                    if (e.Code == "57014")
                        throw new TaskCanceledException(e.Message);
                    throw;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [RewriteAsync]
        [CanBeNull]
        object ExecuteScalarInternal()
        {
            Prechecks();
            Log.Trace("ExecuteNonScalar", Connection.Connector.Id);
            using (Connection.Connector.StartUserAction())
            {
                var behavior = CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;
                using (var reader = Execute(behavior))
                {
                    return reader.Read() && reader.FieldCount != 0 ? reader.GetValue(0) : null;
                }
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
        public new NpgsqlDataReader ExecuteReader()
        {
            return (NpgsqlDataReader) base.ExecuteReader();
        }

        /// <summary>
        /// Executes the CommandText against the Connection, and returns an DbDataReader using one
        /// of the CommandBehavior values.
        /// </summary>
        /// <remarks>
        /// Unlike the ADO.NET method which it replaces, this method returns a Npgsql-specific
        /// DataReader.
        /// </remarks>
        /// <returns>A DbDataReader object.</returns>
        public new NpgsqlDataReader ExecuteReader(CommandBehavior behavior)
        {
            return (NpgsqlDataReader) base.ExecuteReader(behavior);
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">An instance of <see cref="CommandBehavior"/>.</param>
        /// <param name="cancellationToken">A task representing the operation.</param>
        /// <returns></returns>
        protected async override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (cancellationToken.Register(Cancel))
            {
                try
                {
                    return await ExecuteDbDataReaderInternalAsync(behavior, cancellationToken).ConfigureAwait(false);
                }
                catch (NpgsqlException e)
                {
                    if (e.Code == "57014")
                        throw new TaskCanceledException(e.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return ExecuteDbDataReaderInternal(behavior);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [RewriteAsync]
        NpgsqlDataReader ExecuteDbDataReaderInternal(CommandBehavior behavior)
        {
            Prechecks();

            Log.Trace("ExecuteReader", Connection.Connector.Id);

            Connection.Connector.StartUserAction();
            try
            {
                return Execute(behavior);
            }
            catch
            {
                Connection.Connector?.EndUserAction();

                // Close connection if requested even when there is an error.
                if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                {
                    _connection.Close();
                }

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
                    DeallocatePrepared();
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
        void FixupRowDescription(RowDescriptionMessage rowDescription, bool isFirst)
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

#if NET45 || NET451 || DNX451
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

        void Prechecks()
        {
            if (State == CommandState.Disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (Connection == null)
                throw new InvalidOperationException("Connection property has not been initialized.");
            Connection.CheckReady();
        }

        enum SendState
        {
            Start,
            Parse,
            Bind,
            Describe,
            Execute
        }

        #endregion

        #region Invariants

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(!(AllResultTypesAreUnknown && UnknownResultTypeList != null));
            Contract.Invariant(Connection != null || !IsPrepared);
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
