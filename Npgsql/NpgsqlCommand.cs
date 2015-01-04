// created on 21/5/2002 at 20:03

// Npgsql.NpgsqlCommand.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Npgsql.Localization;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Diagnostics.Contracts;

namespace Npgsql
{
    /// <summary>
    /// Represents a SQL statement or function (stored procedure) to execute
    /// against a PostgreSQL database. This class cannot be inherited.
    /// </summary>
#if WITHDESIGN
    [System.Drawing.ToolboxBitmapAttribute(typeof(NpgsqlCommand)), ToolboxItem(true)]
#endif
    [System.ComponentModel.DesignerCategory("")]    
    public sealed partial class NpgsqlCommand : DbCommand, ICloneable
    {
        #region Fields

        NpgsqlConnection _connection;
        NpgsqlConnector _connector;
        NpgsqlTransaction _transaction;
        String _commandText;
        int? _timeout;
        readonly NpgsqlParameterCollection _parameters = new NpgsqlParameterCollection();
        String _planName;

        PrepareStatus _prepared = PrepareStatus.NotPrepared;
        byte[] _preparedCommandText;
        RowDescriptionMessage _rowDescription;
        SchemaOnlyResult _schemaOnlyResult;

        // locals about function support so we don`t need to check it everytime a function is called.
        bool _functionChecksDone;
        bool _functionNeedsColumnListDefinition; // Functions don't return record by default.

        UpdateRowSource _updateRowSource = UpdateRowSource.Both;

        static readonly Array ParamNameCharTable;
        internal Type[] ExpectedTypes { get; set; }


        FormatCode[] _resultFormatCodes;

        internal const int DefaultTimeout = 20;
        const string AnonymousPortal = "";
        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal NpgsqlConnector.NotificationBlock _notificationBlock;

        #endregion Fields

        #region Constructors

        // Constructors
        static NpgsqlCommand()
        {
            ParamNameCharTable = BuildParameterNameCharacterTable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class.
        /// </summary>
        public NpgsqlCommand()
            : this(String.Empty, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        public NpgsqlCommand(String cmdText)
            : this(cmdText, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query and a <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        public NpgsqlCommand(String cmdText, NpgsqlConnection connection)
            : this(cmdText, connection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query, a <see cref="NpgsqlConnection">NpgsqlConnection</see>, and the <see cref="NpgsqlTransaction">NpgsqlTransaction</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction">NpgsqlTransaction</see> in which the <see cref="NpgsqlCommand">NpgsqlCommand</see> executes.</param>
        public NpgsqlCommand(String cmdText, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _planName = String.Empty;
            _commandText = cmdText;
            Connection = connection;
            CommandType = CommandType.Text;
            Transaction = transaction;
        }

        /// <summary>
        /// Used to execute internal commands.
        /// </summary>
        internal NpgsqlCommand(String cmdText, NpgsqlConnector connector, int commandTimeout = 20)
        {
            _planName = String.Empty;
            _commandText = cmdText;
            _connector = connector;
            CommandTimeout = commandTimeout;
            CommandType = CommandType.Text;

            // Removed this setting. It was causing too much problem.
            // Do internal commands really need different timeout setting?
            // Internal commands aren't affected by command timeout value provided by user.
            // timeout = 20;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the SQL statement or function (stored procedure) to execute at the data source.
        /// </summary>
        /// <value>The Transact-SQL statement or stored procedure to execute. The default is an empty string.</value>
        [Category("Data"), DefaultValue("")]
        public override String CommandText
        {
            get { return _commandText; }
            set
            {
                // [TODO] Validate commandtext.
                _commandText = value;
                UnPrepare();
                _functionChecksDone = false;
            }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt
        /// to execute a command and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for the command to execute.
        /// The default is 20 seconds.</value>
        [DefaultValue(DefaultTimeout)]
        public override int CommandTimeout
        {
            get
            {
                return _timeout ?? (
                    _connection != null
                      ? _connection.CommandTimeout
                      : (int)NpgsqlConnectionStringBuilder.GetDefaultValue(Keywords.CommandTimeout)
                );
            }
            set
            {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("value", L10N.CommandTimeoutLessZero);
                }

                _timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the
        /// <see cref="NpgsqlCommand.CommandText">CommandText</see> property is to be interpreted.
        /// </summary>
        /// <value>One of the <see cref="System.Data.CommandType">CommandType</see> values. The default is <see cref="System.Data.CommandType">CommandType.Text</see>.</value>
        [Category("Data"), DefaultValue(CommandType.Text)]
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
        [Category("Behavior"), DefaultValue(null)]
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
                if (_transaction != null && _connection != null && Connector != null && Connector.Transaction != null)
                {
                    throw new InvalidOperationException(L10N.SetConnectionInTransaction);
                }

                if (_connection != null) {
                    _connection.StateChange -= OnConnectionStateChange;
                }
                _connection = value;
                if (_connection != null) {
                    _connection.StateChange += OnConnectionStateChange;
                }
                Transaction = null;
                if (_connection != null)
                {
                    _connector = _connection.Connector;
                    _prepared = _connector != null && _connector.AlwaysPrepare ? PrepareStatus.NeedsPrepare : PrepareStatus.NotPrepared;
                }
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
#if WITHDESIGN
        [Category("Behavior"), DefaultValue(UpdateRowSource.Both)]
#endif
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
                switch (_prepared)
                {
                    case PrepareStatus.NotPrepared:
                        return false;
                    case PrepareStatus.NeedsPrepare:
                    case PrepareStatus.Prepared:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion Public properties

        #region State management

        volatile int _state;

        /// <summary>
        /// Gets the current state of the connector
        /// </summary>
        internal CommandState State
        {
            get { return (CommandState)_state; }
            set
            {
                var newState = (int)value;
                if (newState == _state)
                    return;
                Interlocked.Exchange(ref _state, newState);
            }
        }

        void OnConnectionStateChange(object sender, StateChangeEventArgs stateChangeEventArgs)
        {
            switch (stateChangeEventArgs.CurrentState)
            {
                case ConnectionState.Broken:
                case ConnectionState.Closed:
                    _prepared = PrepareStatus.NotPrepared;
                    break;
                case ConnectionState.Open:
                    switch (stateChangeEventArgs.OriginalState)
                    {
                        case ConnectionState.Closed:
                        case ConnectionState.Broken:
                            _prepared = _connector != null && _connector.AlwaysPrepare ? PrepareStatus.NeedsPrepare : PrepareStatus.NotPrepared;
                            break;
                    }
                    break;
                case ConnectionState.Connecting:
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
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
        protected override DbParameterCollection DbParameterCollection
        {
            get { return Parameters; }
        }

        /// <summary>
        /// Gets the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see>.
        /// </summary>
        /// <value>The parameters of the SQL statement or function (stored procedure). The default is an empty collection.</value>
#if WITHDESIGN
        [Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif

        public new NpgsqlParameterCollection Parameters { get { return _parameters; } }

        static Array BuildParameterNameCharacterTable()
        {
            // Table has lower bound of (int)'.';
            var paramNameCharTable = Array.CreateInstance(typeof(byte), new[] { 'z' - '.' + 1 }, new int[] { '.' });

            paramNameCharTable.SetValue((byte)'.', (int)'.');

            for (int i = '0'; i <= '9'; i++)
            {
                paramNameCharTable.SetValue((byte)i, i);
            }

            for (int i = 'A'; i <= 'Z'; i++)
            {
                paramNameCharTable.SetValue((byte)i, i);
            }

            paramNameCharTable.SetValue((byte)'_', (int)'_');

            for (int i = 'a'; i <= 'z'; i++)
            {
                paramNameCharTable.SetValue((byte)i, i);
            }

            return paramNameCharTable;
        }

        #endregion

        #region Prepare

        /// <summary>
        /// Creates a prepared version of the command on a PostgreSQL server.
        /// </summary>
        public override void Prepare()
        {
            _log.Debug("Prepare command");
            CheckConnectionState();
            UnPrepare();
            using (_connector.BlockNotifications())
            {
                WriteQuery(true, false);
            }
        }

        string RewriteSqlForCommandType()
        {
            if (CommandType == CommandType.Text)
                return CommandText;
            if (CommandType == CommandType.TableDirect)
                return "SELECT * FROM " + CommandText;

            Contract.Assert(CommandType == CommandType.StoredProcedure, "Invalid CommandType");

            var numInput = _parameters.Count(p => p.IsInputDirection);

            return "SELECT * FROM " + CommandText + "(" + string.Join(",", Enumerable.Range(1, numInput).Select(i => "$" + i.ToString())) + ")";
        }

        void SetUpBind(uint[] parameterTypeOIDs, NpgsqlTextWriter[] parameterTexts, int[] parameterSizes, FormatCode[] formatCodes, List<int> sizeArr)
        {
            // Traverse parameters
            for (int i = 0, j = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].IsInputDirection)
                {
                    // Figure out whether text or binary should be used.
                    // If binary, only get the sizes here.
                    // If text, write everything to buffers (NpgsqlTextWriters)

                    var handler = _connector.TypeHandlerRegistry[parameterTypeOIDs[j]];

                    if (!handler.SupportsBinaryWrite || handler.PreferTextWrite)
                    {
                        formatCodes[j] = FormatCode.Text;
                        if (!_parameters[i].IsNull)
                        {
                            parameterTexts[j] = new NpgsqlTextWriter();
                            handler.WriteText(_parameters[i].Value, parameterTexts[j]);
                            parameterSizes[j] = parameterTexts[j].GetTotalByteLength();
                        }
                    }
                    else
                    {
                        formatCodes[j] = FormatCode.Binary;
                        if (!_parameters[i].IsNull)
                        {
                            parameterSizes[j] = handler.BinarySize(_connector.TypeHandlerRegistry, parameterTypeOIDs[j], _parameters[i].Value, sizeArr) - 4;
                        }
                    }

                    j++;
                }
            }
        }

        void SendBind(NpgsqlBuffer buf, int numInputParameters, uint[] parameterTypeOIDs, NpgsqlTextWriter[] parameterTexts, int[] parameterSizes, FormatCode[] formatCodes, List<int> sizeArr, FormatCode[] resultFormatCodes = null)
        {
            int formatCodesSum = formatCodes.Sum(c => (int)c);
            int formatCodeListLength = formatCodesSum == 0 ? 0 : formatCodesSum == formatCodes.Length ? 1 : formatCodes.Length;

            int totalSize =
                4 + // Message length
                1 + // Unnamed portal name
                _planName.Length + 1 + // Statement name
                2 + // Number of parameter format codes that follow (0 means all text, 1 means that the following should be applied to all parameters)
                2 * formatCodeListLength + // List of format codes
                2 + // Number of parameters
                4 * numInputParameters + parameterSizes.Sum() +
                2 + // Number of result format codes
                2 * (resultFormatCodes == null ? 1 : resultFormatCodes.Length); // Use binary for everything that is received if unknown

            buf.EnsureWrite(5 + 1 + _planName.Length + 1 + 2 + 2);
            buf.WriteByte((byte)FrontendMessageCode.Bind);
            buf.WriteInt32(totalSize);
            buf.WriteByte(0); // Empty portal name
            buf.WriteString(_planName, _planName.Length);
            buf.WriteByte(0);
            buf.WriteInt16(formatCodeListLength);
            if (formatCodeListLength == 1)
            {
                buf.WriteInt16((short)FormatCode.Binary);
            }
            else if (formatCodeListLength > 1)
            {
                if (2 * formatCodeListLength <= buf.Size)
                {
                    buf.EnsureWrite(2 * formatCodeListLength);
                    foreach (var code in formatCodes)
                        buf.WriteInt16((short)code);
                }
                else
                {
                    // Special case
                    foreach (var code in formatCodes)
                        buf.EnsuredWriteInt16((short)code);
                }
            }

            buf.EnsuredWriteInt16(numInputParameters);

            // Traverse parameters
            int sizeForWriteBinary = 0;
            for (int i = 0, j = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].IsInputDirection)
                {
                    // Here we actually write the values since we now know all sizes
                    // Text: use stored text
                    // Binary: convert on the fly. If exception occurs (format exceptions etc.),
                    // send zeros for the rest and set the last format code to something invalid, which will definitely make the backend
                    // throw an exception which we can recover from instead of leaving the connector in an undefined state

                    if (_parameters[i].IsNull)
                    {
                        buf.EnsuredWriteInt32(-1);
                    }
                    else
                    {
                        if (formatCodes[j] == FormatCode.Text)
                        {
                            buf.EnsuredWriteInt32(parameterSizes[j]);
                            parameterTexts[j].WriteToOutputBuffer(buf);
                        }
                        else
                        {
                            var handler = _connector.TypeHandlerRegistry[parameterTypeOIDs[j]];
                            var flushedBytesBeforeThisValue = buf.TotalBytesFlushed;
                            try
                            {
                                handler.WriteBinary(_connector.TypeHandlerRegistry, parameterTypeOIDs[j], _parameters[i].Value, buf, sizeArr, ref sizeForWriteBinary);
                            }
                            catch (Exception e)
                            {
                                if (e is System.Net.Sockets.SocketException || e is EndOfStreamException || e is ThreadAbortException)
                                    throw;

                                if (buf.TotalBytesFlushed == 0)
                                {
                                    // Yay, we have not written anything yet to the network stream so we can just drop what was in the buffer
                                    buf.WritePosition = 0;
                                    throw;
                                }
                                else
                                {
                                    // Here comes a tricky part, we must send a complete Bind message, but an incorrect one so that the backend will generate an error
                                    throw new NotImplementedException("Not implemented recovery");
                                }
                            }
                        }
                    }
                    j++;
                }
            }
            Contract.Assert(sizeForWriteBinary == sizeArr.Count, "Mismatched size arr counter");

            // Send result format codes
            buf.EnsureWrite(4);
            buf.WriteInt16(resultFormatCodes == null ? 1 : resultFormatCodes.Length);
            if (resultFormatCodes != null)
            {
                if (buf.WriteSpaceLeft >= 2 * resultFormatCodes.Length)
                    foreach (var code in resultFormatCodes)
                        buf.WriteInt16((short)code);
                else
                {
                    foreach (var code in resultFormatCodes)
                        buf.EnsuredWriteInt16((short)code);
                }
            }
            else
            {
                buf.WriteInt16((short)FormatCode.Text);
            }

            // Bind has now been sent!
        }

        byte[] WriteQuery(bool prepare, bool onlySchema)
        {
            Contract.Requires(_connector != null && _connector.IsConnected, "Should have an open connection");

            var buf = _connector.Buffer;
            Contract.Assert(buf.ReadBytesLeft == 0, "The read buffer should be read completely before sending Parse message");
            Contract.Assert(buf.WritePosition == 0, "WritePosition should be 0");

            string query = RewriteSqlForCommandType();

            buf.WriteByte((byte)FrontendMessageCode.Parse); // Might be updated
            buf.WriteInt32(0); // Will be updated
            if (prepare)
            {
                _planName = _connector.NextPlanName();
                buf.WriteString(_planName, _planName.Length);
                buf.WriteByte(0);
            } else {
                _planName = "";
                buf.WriteByte(0); // Empty statement name, might be overwritten by a space character if we must use simple protocol
            }

            byte[] savedQueryCopy = null;
            uint[] parameterTypeOIDs;

            var writer = new NpgsqlTextWriter(buf);
            buf.WritePosition = 0; // The writer takes over the write position. We put 0 here to handle if an exception is later thrown.

            var queries = ProcessRawQueryText(query);
            Contract.Assert(queries.Count == 1 || _parameters.All(p => !p.IsInputDirection), "Multiquery with parameters");
            if (queries.Count > 1)
            {
                throw new NotImplementedException("Multiqueries");
            }
            writer.WriteString(queries[0]);

            var numInputParameters = _parameters.Count(p => p.IsInputDirection);
            parameterTypeOIDs = new uint[numInputParameters];
            for (int i = 0, j = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].IsInputDirection)
                {
                    parameterTypeOIDs[j++] = _parameters[i].GetTypeOid(_connector.TypeHandlerRegistry);
                }
            }

            NpgsqlTextWriter[] parameterTexts = null;
            int[] parameterSizes = null;
            FormatCode[] formatCodes = null;
            List<int> sizeArr = null;

            if (!prepare && !onlySchema)
            {
                // We do this before we write the query to the socket so we can
                // recover easily if SetUpBind throws an exception (due to invalid parameters)

                parameterTexts = new NpgsqlTextWriter[numInputParameters];
                parameterSizes = new int[numInputParameters];
                formatCodes = new FormatCode[numInputParameters];
                sizeArr = new List<int>();

                SetUpBind(parameterTypeOIDs, parameterTexts, parameterSizes, formatCodes, sizeArr);
            }

            savedQueryCopy = writer.GetTruncatedCopy();
            writer.WriteSingleChar('\0');
            buf.WritePosition = 1;
            buf.WriteInt32(4 + _planName.Length + 1 + writer.GetTotalByteLength() + 2 + 4 * numInputParameters);
            buf.ResetTotalBytesFlushed();
            writer.WriteToOutputBuffer();
                
            // Tell to mediator what command is being sent.
            _connector.Mediator.SetSqlSent(savedQueryCopy, NpgsqlMediator.SQLSentType.Simple);

            if (2 + 4 * numInputParameters <= buf.Size)
            {
                buf.EnsureWrite(2 + 4 * numInputParameters);
                buf.WriteInt16(numInputParameters);
                foreach (var oid in parameterTypeOIDs)
                    buf.WriteInt32((int)oid);
            }
            else
            {
                // Special case, very many parameters, not common
                buf.EnsuredWriteInt16(numInputParameters);
                foreach (var oid in parameterTypeOIDs)
                    buf.EnsuredWriteInt32((int)oid);
            }

            // Parse has now been sent!

            if (prepare || onlySchema)
            {
                // Only send Describe statement + Sync

                buf.EnsureWrite(1 + 4 + 1 + _planName.Length + 1 + 1 + 4);
                buf.WriteByte((byte)FrontendMessageCode.Describe);
                buf.WriteInt32(4 + 1 + _planName.Length + 1);
                buf.WriteByte((byte)FrontendMessageCode.DescribeStatement);
                if (_planName != "")
                    buf.WriteString(_planName, _planName.Length);
                buf.WriteByte(0);

                buf.WriteByte((byte)FrontendMessageCode.Sync);
                buf.WriteInt32(4);

                buf.Flush();

                _preparedCommandText = savedQueryCopy;
            }
            else
            {
                SendBind(buf, numInputParameters, parameterTypeOIDs, parameterTexts, parameterSizes, formatCodes, sizeArr);

                buf.EnsureWrite(
                    1 + 4 + 1 + 1 +
                    1 + 4 + 1 + 4 +
                    1 + 4);

                // Describe
                buf.WriteByte((byte)FrontendMessageCode.Describe);
                buf.WriteInt32(4 + 1 + 1);
                buf.WriteByte((byte)FrontendMessageCode.DescribePortal);
                buf.WriteByte(0); // Empty portal name

                // Execute
                buf.WriteByte((byte)FrontendMessageCode.Execute);
                buf.WriteInt32(4 + 1 + 4);
                buf.WriteByte(0); // Empty portal name
                buf.WriteInt32(0); // Maximum number of rows, 0 = no limit

                // Sync
                buf.WriteByte((byte)FrontendMessageCode.Sync);
                buf.WriteInt32(4);

                buf.Flush();
            }

            // Everything has now been sent and flushed
            // Now read everything until RowDescription/NoData
            ParameterDescriptionMessage parameterDescription = null;
            RowDescriptionMessage rowDescriptionMessage = null;
            bool noData = false;
            for (; ; )
            {
                var msg = _connector.ReadSingleMessage();
                switch (msg.Code)
                {
                    case BackendMessageCode.ParseComplete:
                        continue;
                    case BackendMessageCode.BindComplete:
                        Contract.Assume(!prepare && !onlySchema);
                        goto AfterLoop;
                    case BackendMessageCode.ParameterDescription:
                        parameterDescription = (ParameterDescriptionMessage)msg;
                        continue;
                    case BackendMessageCode.RowDescription:
                        rowDescriptionMessage = (RowDescriptionMessage)msg;
                        continue;
                    case BackendMessageCode.NoData:
                        noData = true;
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        Contract.Assume(prepare || onlySchema);
                        goto AfterLoop;
                    default:
                        throw new ArgumentOutOfRangeException("Unexpected message of type " + msg.Code);
                }
            }
        AfterLoop:

            if (parameterDescription != null)
            {
                Contract.Assume(prepare || onlySchema);
                if (parameterDescription.TypeOIDs.Count != parameterTypeOIDs.Length)
                {
                    throw new InvalidOperationException("Wrong number of parameters. Got " + parameterTypeOIDs.Length + " but expected " + parameterDescription.TypeOIDs.Count + ".");
                }

                for (int i = 0, j = 0; j < parameterTypeOIDs.Length; i++)
                {
                    if (_parameters[i].IsInputDirection)
                    {
                        uint oidFromBackend = parameterDescription.TypeOIDs[j];
                        if (oidFromBackend != parameterTypeOIDs[j])
                        {
                            Contract.Assert(parameterTypeOIDs[j] == 0 || _connector.TypeHandlerRegistry[parameterTypeOIDs[j]].PgName == "unknown", "Parameter type shouldn't be changed on non-unknown inputs");
                            _parameters[i].SetTypeOID(oidFromBackend, _connector.NativeToBackendTypeConverterOptions);
                        }

                        j++;
                    }
                }
            }

            if (rowDescriptionMessage != null)
            {
                if (prepare)
                    _resultFormatCodes = new FormatCode[rowDescriptionMessage.NumFields];

                for (var i = 0; i < rowDescriptionMessage.NumFields; i++)
                {
                    var returnRowDescData = rowDescriptionMessage[i];

                    if (returnRowDescData.Handler != null)
                    {
                        // Binary format?
                        // PG always defaults to text encoding.  We can fix up the row description
                        // here based on support for binary encoding.  Once this is done,
                        // there is no need to request another row description after Bind.
                        // Row Description messages from Describe Statement will always have FormatCode.Text since what we specify is not known yet
                        returnRowDescData.FormatCode = returnRowDescData.Handler.SupportsBinaryRead ? FormatCode.Binary : FormatCode.Text;
                        if (prepare)
                            _resultFormatCodes[i] = returnRowDescData.FormatCode;
                    }
                }
                _rowDescription = rowDescriptionMessage;
            }
            else
            {
                if (prepare)
                    _resultFormatCodes = new FormatCode[] { }; // All text
                _rowDescription = null;
            }

            if (prepare)
                _prepared = PrepareStatus.Prepared;

            if (onlySchema)
            {
                Contract.Assert(noData || _rowDescription != null);
                _schemaOnlyResult = noData ? SchemaOnlyResult.HasNoData : SchemaOnlyResult.HasRowDescription;
            }

            return savedQueryCopy;
        }

        enum SchemaOnlyResult
        {
            UsedSimpleQuery,
            HasRowDescription,
            HasNoData
        }

        void UnPrepare()
        {
            if (_prepared == PrepareStatus.Prepared)
            {
                _connector.ExecuteBlind("DEALLOCATE " + _planName);
                _rowDescription = null;
                _prepared = PrepareStatus.NeedsPrepare;
            }

            _preparedCommandText = null;
        }

        #endregion Prepare

        #region Query preparation

#if false
        bool CheckFunctionNeedsColumnDefinitionList()
        {
            // If and only if a function returns "record" and has no OUT ("o" in proargmodes), INOUT ("b"), or TABLE
            // ("t") return arguments to characterize the result columns, we must provide a column definition list.
            // See http://pgfoundry.org/forum/forum.php?thread_id=1075&forum_id=519
            // We would use our Output and InputOutput parameters to construct that column definition list.  If we have
            // no such parameters, skip the check: we could only construct "AS ()", which yields a syntax error.

            // Updated after 0.99.3 to support the optional existence of a name qualifying schema and allow for case insensitivity
            // when the schema or procedure name do not contain a quote.
            // The hard-coded schema name 'public' was replaced with code that uses schema as a qualifier, only if it is provided.

            String returnRecordQuery;

            var parameterTypes = new StringBuilder("");

            // Process parameters

            var seenDef = false;
            foreach (NpgsqlParameter p in Parameters)
            {
                if (p.IsInputDirection)
                {
                    parameterTypes.Append(Connection.Connector.OidToNameMapping[p.TypeInfo.Name].OID.ToString() + " ");
                }

                if ((p.Direction == ParameterDirection.Output) || (p.Direction == ParameterDirection.InputOutput))
                {
                    seenDef = true;
                }
            }

            if (!seenDef)
            {
                return false;
            }

            // Process schema name.

            var schemaName = String.Empty;
            var procedureName = String.Empty;

            var fullName = CommandText.Split('.');

            const string predicate = "prorettype = ( select oid from pg_type where typname = 'record' ) "
                + "and proargtypes=:proargtypes and proname=:proname "
                // proargmodes && array['o','b','t']::"char"[] performs just as well, but it requires PostgreSQL 8.2.
                + "and ('o' = any (proargmodes) OR 'b' = any (proargmodes) OR 't' = any (proargmodes)) is not true";
            if (fullName.Length == 2)
            {
                returnRecordQuery =
                "select count(*) > 0 from pg_proc p left join pg_namespace n on p.pronamespace = n.oid where " + predicate + " and n.nspname=:nspname";

                schemaName = (fullName[0].IndexOf("\"") != -1) ? fullName[0] : fullName[0].ToLower();
                procedureName = (fullName[1].IndexOf("\"") != -1) ? fullName[1] : fullName[1].ToLower();
            }
            else
            {
                // Instead of defaulting don't use the nspname, as an alternative, query pg_proc and pg_namespace to try and determine the nspname.
                // schemaName = "public"; // This was removed after build 0.99.3 because the assumption that a function is in public is often incorrect.
                returnRecordQuery = "select count(*) > 0 from pg_proc p where " + predicate;

                procedureName = (CommandText.IndexOf("\"") != -1) ? CommandText : CommandText.ToLower();
            }

            bool ret;

            using (var c = new NpgsqlCommand(returnRecordQuery, Connection))
            {
                c.Parameters.Add(new NpgsqlParameter("proargtypes", NpgsqlDbType.Oidvector));
                c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Name));

                c.Parameters[0].Value = parameterTypes.ToString();
                c.Parameters[1].Value = procedureName;

                if (!string.IsNullOrEmpty(schemaName))
                {
                    c.Parameters.Add(new NpgsqlParameter("nspname", NpgsqlDbType.Name));
                    c.Parameters[2].Value = schemaName;
                }

                ret = (bool)c.ExecuteScalar();
            }

            return ret;
        }

        void AddFunctionColumnListSupport(Stream st)
        {
            var isFirstOutputOrInputOutput = true;

            st.WriteString(" AS (");

            for (var i = 0; i < Parameters.Count; i++)
            {
                var p = Parameters[i];

                switch (p.Direction)
                {
                    case ParameterDirection.Output:
                    case ParameterDirection.InputOutput:
                        if (isFirstOutputOrInputOutput)
                        {
                            isFirstOutputOrInputOutput = false;
                        }
                        else
                        {
                            st.WriteString(", ");
                        }

                        st.WriteString(p.CleanName);
                        st.WriteByte((byte) ASCIIBytes.Space);
                        st.WriteString(p.TypeInfo.Name);

                        break;
                }
            }

            st.WriteByte((byte)ASCIIBytes.ParenRight);
        }

        /// <summary>
        /// Process this._commandText, trimming each distinct command and substituting paramater
        /// tokens.
        /// </summary>
        /// <param name="prepare"></param>
        /// <returns>UTF8 encoded command ready to be sent to the backend.</returns>
        byte[] GetCommandText(bool prepare)
        {
            var commandBuilder = new MemoryStream();

            if (CommandType == CommandType.TableDirect)
            {
                foreach (var table in _commandText.Split(';'))
                {
                    if (table.Trim().Length == 0)
                    {
                        continue;
                    }

                    commandBuilder
                        .WriteString("SELECT * FROM ")
                        .WriteString(table)
                        .WriteString(";");
                }
            }
            else if (CommandType == CommandType.StoredProcedure)
            {
                if (!prepare && !_functionChecksDone)
                {
                    _functionNeedsColumnListDefinition = Parameters.Count != 0 && CheckFunctionNeedsColumnDefinitionList();

                    _functionChecksDone = true;
                }

                commandBuilder.WriteString("SELECT * FROM ");

                if (_commandText.TrimEnd().EndsWith(")"))
                {
                    if (!AppendCommandReplacingParameterValues(commandBuilder, _commandText, prepare, false))
                    {
                        throw new NotSupportedException("Multiple queries not supported for stored procedures");
                    }
                }
                else
                {
                    commandBuilder
                        .WriteString(_commandText)
                        .WriteByte((byte)ASCIIBytes.ParenLeft);

                    if (prepare)
                    {
                        AppendParameterPlaceHolders(commandBuilder);
                    }
                    else
                    {
                        AppendParameterValues(commandBuilder);
                    }

                    commandBuilder.WriteByte((byte)ASCIIBytes.ParenRight);
                }

                if (!prepare && _functionNeedsColumnListDefinition)
                {
                    AddFunctionColumnListSupport(commandBuilder);
                }
            }
            else
            {
                if (!AppendCommandReplacingParameterValues(commandBuilder, _commandText, prepare, !prepare))
                {
                    throw new NotSupportedException("Multiple queries not supported for prepared statements");
                }
            }

            commandBuilder.Write(ASCIIByteArrays.Byte_0, 0, 1);
            return commandBuilder.ToArray();
        }

        void AppendParameterPlaceHolders(Stream dest)
        {
            var first = true;

            for (var i = 0; i < _parameters.Count; i++)
            {
                var parameter = _parameters[i];

                if (parameter.IsInputDirection)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        dest.WriteString(", ");
                    }

                    AppendParameterPlaceHolder(dest, parameter, i + 1);
                }
            }
        }
#endif

        static bool IsParamNameChar(char ch)
        {
            if (ch < '.' || ch > 'z')
            {
                return false;
            }
            return ((byte)ParamNameCharTable.GetValue(ch) != 0);
        }

        static bool IsLetter(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z';
        }

        static bool IsIdentifierStart(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_' || 128 <= ch && ch <= 255;
        }

        static bool IsDollarTagIdentifier(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || 128 <= ch && ch <= 255;
        }

        static bool IsIdentifier(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || ch == '$' || 128 <= ch && ch <= 255;
        }

        /// <summary>
        /// Receives a raw SQL query as passed in by the user, and performs some processing necessary
        /// before sending to the backend.
        /// This includes doing parameter placebolder processing (@p => $1), and splitting the query
        /// up by semicolons if needed (SELECT 1; SELECT 2)
        /// </summary>
        /// <param name="src">Raw user-provided query</param>
        /// <returns>the queries contained in the raw text</returns>
        List<string> ProcessRawQueryText(string src)
        {
            var standardConformantStrings = _connection == null || _connection.UseConformantStrings;
            var typeHandlerRegistry = _connector.TypeHandlerRegistry;

            var currCharOfs = 0;
            var end = src.Length;
            var ch = '\0';
            var lastChar = '\0';
            var dollarTagStart = 0;
            var dollarTagEnd = 0;
            var currTokenBeg = 0;
            var blockCommentLevel = 0;

            var resultQueries = new List<string>();
            var currentQuery = new StringWriter();
            Dictionary<NpgsqlParameter, int> paramOrdinalMap = null;

            if (_parameters.Any(p => p.IsInputDirection))
            {
                paramOrdinalMap = new Dictionary<NpgsqlParameter, int>();

                for (int i = 0, cnt = 0; i < _parameters.Count; i++)
                {
                    if (_parameters[i].IsInputDirection)
                    {
                        paramOrdinalMap[_parameters[i]] = ++cnt;
                    }
                }
            }

            /*if (allowMultipleStatements && _parameters.Count == 0 && !checkForMultipleStatements)
            {
                dest.WriteString(src);
                return true;
            }*/

        None:
            if (currCharOfs >= end)
            {
                goto Finish;
            }
            lastChar = ch;
            ch = src[currCharOfs++];
        NoneContinue:
            for (; ; lastChar = ch, ch = src[currCharOfs++])
            {
                switch (ch)
                {
                    case '/': goto BlockCommentBegin;
                    case '-': goto LineCommentBegin;
                    case '\'': if (standardConformantStrings) goto Quoted; else goto Escaped;
                    case '$': if (!IsIdentifier(lastChar)) goto DollarQuotedStart; else break;
                    case '"': goto DoubleQuoted;
                    case ':': if (lastChar != ':') goto ParamStart; else break;
                    case '@': if (lastChar != '@') goto ParamStart; else break;
                    case ';': goto SemiColon;

                    case 'e':
                    case 'E': if (!IsLetter(lastChar)) goto EscapedStart; else break;
                }

                if (currCharOfs >= end)
                {
                    goto Finish;
                }
            }

        ParamStart:
            if (currCharOfs < end)
            {
                lastChar = ch;
                ch = src[currCharOfs];
                if (IsParamNameChar(ch))
                {
                    if (currCharOfs - 1 > currTokenBeg)
                    {
                        currentQuery.Write(src.Substring(currTokenBeg, currCharOfs - 1 - currTokenBeg));
                    }
                    currTokenBeg = currCharOfs++ - 1;
                    goto Param;
                }
                else
                {
                    currCharOfs++;
                    goto NoneContinue;
                }
            }
            goto Finish;

        Param:
            // We have already at least one character of the param name
            for (; ; )
            {
                lastChar = ch;
                if (currCharOfs >= end || !IsParamNameChar(ch = src[currCharOfs]))
                {
                    var paramName = src.Substring(currTokenBeg, currCharOfs - currTokenBeg);
                    NpgsqlParameter parameter;

                    if (_parameters.TryGetValue(paramName, out parameter) && parameter.IsInputDirection)
                    {
                        Contract.Assert(paramOrdinalMap != null);
                        currentQuery.Write('$');
                        currentQuery.Write(paramOrdinalMap[parameter]);
                        currTokenBeg = currCharOfs;
                    }

                    if (currCharOfs >= end)
                    {
                        goto Finish;
                    }

                    currCharOfs++;
                    goto NoneContinue;
                }
                else
                {
                    currCharOfs++;
                }
            }

        Quoted:
            while (currCharOfs < end)
            {
                if (src[currCharOfs++] == '\'')
                {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        DoubleQuoted:
            while (currCharOfs < end)
            {
                if (src[currCharOfs++] == '"')
                {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        EscapedStart:
            if (currCharOfs < end)
            {
                lastChar = ch;
                ch = src[currCharOfs++];
                if (ch == '\'')
                {
                    goto Escaped;
                }
                goto NoneContinue;
            }
            goto Finish;

        Escaped:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\'')
                {
                    goto MaybeConcatenatedEscaped;
                }
                if (ch == '\\')
                {
                    if (currCharOfs >= end)
                    {
                        goto Finish;
                    }
                    currCharOfs++;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                {
                    goto MaybeConcatenatedEscaped2;
                }
                if (ch != ' ' && ch != '\t' && ch != '\f')
                {
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped2:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\'')
                {
                    goto Escaped;
                }
                if (ch == '-')
                {
                    if (currCharOfs >= end)
                    {
                        goto Finish;
                    }
                    ch = src[currCharOfs++];
                    if (ch == '-')
                    {
                        goto MaybeConcatenatedEscapeAfterComment;
                    }
                    lastChar = '\0';
                    goto NoneContinue;

                }
                if (ch != ' ' && ch != '\t' && ch != '\n' & ch != '\r' && ch != '\f')
                {
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscapeAfterComment:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                {
                    goto MaybeConcatenatedEscaped2;
                }
            }
            goto Finish;

        DollarQuotedStart:
            if (currCharOfs < end)
            {
                ch = src[currCharOfs];
                if (ch == '$')
                {
                    // Empty tag
                    dollarTagStart = dollarTagEnd = currCharOfs;
                    currCharOfs++;
                    goto DollarQuoted;
                }
                if (IsIdentifierStart(ch))
                {
                    dollarTagStart = currCharOfs;
                    currCharOfs++;
                    goto DollarQuotedInFirstDelim;
                }
                lastChar = '$';
                currCharOfs++;
                goto NoneContinue;
            }
            goto Finish;

        DollarQuotedInFirstDelim:
            while (currCharOfs < end)
            {
                lastChar = ch;
                ch = src[currCharOfs++];
                if (ch == '$')
                {
                    dollarTagEnd = currCharOfs - 1;
                    goto DollarQuoted;
                }
                if (!IsDollarTagIdentifier(ch))
                {
                    goto NoneContinue;
                }
            }
            goto Finish;

        DollarQuoted:
            {
                var tag = src.Substring(dollarTagStart - 1, dollarTagEnd - dollarTagStart + 2);
                var pos = src.IndexOf(tag, dollarTagEnd + 1); // Not linear time complexity, but that's probably not a problem, since PostgreSQL backend's isn't either
                if (pos == -1)
                {
                    currCharOfs = end;
                    goto Finish;
                }
                currCharOfs = pos + dollarTagEnd - dollarTagStart + 2;
                ch = '\0';
                goto None;
            }

        LineCommentBegin:
            if (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '-')
                {
                    goto LineComment;
                }
                lastChar = '\0';
                goto NoneContinue;
            }
            goto Finish;

        LineComment:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                {
                    goto None;
                }
            }
            goto Finish;

        BlockCommentBegin:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '*')
                {
                    blockCommentLevel++;
                    goto BlockComment;
                }
                if (ch != '/')
                {
                    if (blockCommentLevel > 0)
                    {
                        goto BlockComment;
                    }
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        BlockComment:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '*')
                {
                    goto BlockCommentEnd;
                }
                if (ch == '/')
                {
                    goto BlockCommentBegin;
                }
            }
            goto Finish;

        BlockCommentEnd:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '/')
                {
                    if (--blockCommentLevel > 0)
                    {
                        goto BlockComment;
                    }
                    goto None;
                }
                if (ch != '*')
                {
                    goto BlockComment;
                }
            }
            goto Finish;

        SemiColon:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (Char.IsWhiteSpace(ch)) {
                    continue;
                }
                // TODO: Handle end of line comment? Although psql doesn't seem to handle them...
                if (_parameters.Any(p => p.IsInputDirection))
                {
                    throw new NotSupportedException("Commands with multiple queries and parameters aren't supported");
                }
                resultQueries.Add(currentQuery.ToString());
                currentQuery = new StringWriter();
                goto NoneContinue;
            }
        // implicit goto Finish

        Finish:
            currentQuery.Write(src.Substring(currTokenBeg, end - currTokenBeg));
            resultQueries.Add(currentQuery.ToString());
            return resultQueries;
        }

        #endregion Query preparation

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
#if NET45
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
#else
        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
#endif
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            try
            {
                return await ExecuteNonQueryInternalAsync();
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "57014")
                    throw new TaskCanceledException(e.Message);
                throw;
            }
        }

#if !NET45
        public Task<int> ExecuteNonQueryAsync()
        {
            return ExecuteNonQueryAsync(CancellationToken.None);
        }
#endif

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [GenerateAsync]
        int ExecuteNonQueryInternal()
        {
            _log.Debug("ExecuteNonQuery");
            NpgsqlDataReader reader;
            using (reader = GetReader()) {
                while (reader.NextResult()) ;
            }
            return reader.RecordsAffected;
        }

        #endregion Execute Non Query

        #region Execute Scalar

        /// <summary>
        /// Executes the query, and returns the first column of the first row
        /// in the result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <returns>The first column of the first row in the result set,
        /// or a null reference if the result set is empty.</returns>
        public override Object ExecuteScalar()
        {
            return ExecuteScalarInternal();
        }

        /// <summary>
        /// Asynchronous version of <see cref="ExecuteScalar"/>
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the first column of the
        /// first row in the result set, or a null reference if the result set is empty.</returns>
#if NET45
        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
#else
        public async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
#endif
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            try
            {
                return await ExecuteScalarInternalAsync();
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "57014")
                    throw new TaskCanceledException(e.Message);
                throw;
            }
        }

#if !NET45
        public Task<object> ExecuteScalarAsync()
        {
            return ExecuteScalarAsync(CancellationToken.None);
        }
#endif

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [GenerateAsync]
        object ExecuteScalarInternal()
        {
            using (var reader = GetReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
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
        public new NpgsqlDataReader ExecuteReader()
        {
            return (NpgsqlDataReader)base.ExecuteReader();
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
            return (NpgsqlDataReader)base.ExecuteReader(behavior);
        }

#if NET45
        protected async override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            try
            {
                return await ExecuteDbDataReaderInternalAsync(behavior);
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "57014")
                    throw new TaskCanceledException(e.Message);
                throw;
            }
        }
#else
        /// <summary>
        /// Executes the CommandText against the Connection, and returns an DbDataReader using one
        /// of the CommandBehavior values.
        /// </summary>
        /// <returns>A DbDataReader object.</returns>
        public async Task<NpgsqlDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            try
            {
                return await ExecuteDbDataReaderInternalAsync(behavior);
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "57014")
                    throw new TaskCanceledException(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously executes the CommandText against the Connection, and returns an DbDataReader.
        /// </summary>
        /// <returns>A DbDataReader object.</returns>
        public Task<NpgsqlDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);
        }

        /// <summary>
        /// Executes the CommandText against the Connection, and returns an DbDataReader using one
        /// of the CommandBehavior values.
        /// </summary>
        /// <returns>A DbDataReader object.</returns>
        public Task<NpgsqlDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            return ExecuteReaderAsync(behavior, CancellationToken.None);
        }

        /// <summary>
        /// Executes the CommandText against the Connection, and returns an DbDataReader using one
        /// of the CommandBehavior values.
        /// </summary>
        /// <returns>A DbDataReader object.</returns>
        public Task<NpgsqlDataReader> ExecuteReaderAsync()
        {
            return ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);
        }
#endif

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            _log.Debug("ExecuteReader with CommandBehavior=" + behavior);
            return ExecuteDbDataReaderInternal(behavior);
        }

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [GenerateAsync]
        NpgsqlDataReader ExecuteDbDataReaderInternal(CommandBehavior behavior)
        {
            // Close connection if requested even when there is an error.
            try
            {
                return GetReader(behavior);
            }
            catch (Exception)
            {
                if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                {
                    _connection.Close();
                }

                throw;
            }
        }

        [GenerateAsync]
        internal NpgsqlDataReader GetReader(CommandBehavior cb = CommandBehavior.Default)
        {
            CheckConnectionState();

            NpgsqlDataReader reader = null;

            if (_prepared == PrepareStatus.NeedsPrepare)
                Prepare();

            // Block the notification thread before writing anything to the wire.
            _notificationBlock = _connector.BlockNotifications();
            //using (_connector.BlockNotificationThread())
            try
            {
                State = CommandState.InProgress;

                _connector.SetBackendCommandTimeout(CommandTimeout);

                
                if (_prepared == PrepareStatus.NotPrepared)
                {
                    WriteQuery(false, (cb & CommandBehavior.SchemaOnly) != 0);

                    if ((cb & CommandBehavior.SchemaOnly) == 0 || _schemaOnlyResult == SchemaOnlyResult.UsedSimpleQuery)
                        return reader = new NpgsqlDataReader(this, cb);
                    else
                        return reader = new NpgsqlDataReader(this, cb, _rowDescription, true);
                }
                else
                {
                    // Bind the parameters, execute and sync

                    var numInputParameters = _parameters.Count(p => p.IsInputDirection);
                    var parameterTypeOIDs = new uint[numInputParameters];
                    for (int i = 0, j = 0; i < _parameters.Count; i++)
                    {
                        if (_parameters[i].IsInputDirection)
                        {
                            parameterTypeOIDs[j++] = _parameters[i].GetTypeOid(_connector.TypeHandlerRegistry);
                        }
                    }

                    NpgsqlTextWriter[] parameterTexts = null;
                    int[] parameterSizes = null;
                    FormatCode[] formatCodes = null;
                    List<int> sizeArr = null;

                    parameterTexts = new NpgsqlTextWriter[numInputParameters];
                    parameterSizes = new int[numInputParameters];
                    formatCodes = new FormatCode[numInputParameters];
                    sizeArr = new List<int>();

                    _connector.Buffer.ResetTotalBytesFlushed();
                    SetUpBind(parameterTypeOIDs, parameterTexts, parameterSizes, formatCodes, sizeArr);
                    SendBind(_connector.Buffer, numInputParameters, parameterTypeOIDs, parameterTexts, parameterSizes, formatCodes, sizeArr, _resultFormatCodes);

                    _connector.SendExecute();
                    _connector.SendSync();

                    // Tell to mediator what command is being sent.
                    _connector.Mediator.SetSqlSent(_preparedCommandText, NpgsqlMediator.SQLSentType.Execute);

                    // TODO: Allocation
                    return reader = new NpgsqlDataReader(this, cb, _rowDescription);

                    // Construct the return reader, possibly with a saved row description from Prepare().
                    /*
                    reader = new NpgsqlDataReader(this, cb, _connector.BlockNotificationThread(), true, _currentRowDescription);
                    if (_currentRowDescription == null) {
                        reader.NextResultInternal();
                    }
                    reader.UpdateOutputParameters();
                     */
                }

                return reader;
            }
            catch (NpgsqlException)
            {
                _connector.State = ConnectorState.Ready;
                throw;
            }
            finally
            {
                if (reader == null && _notificationBlock != null) {
                    _notificationBlock.Dispose();
                }
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
            set { Transaction = (NpgsqlTransaction)value; }
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
            set
            {
                _transaction = value;
            }
        }

        #endregion Transactions

        #region Cancel

        /// <summary>
        /// Attempts to cancel the execution of a <see cref="NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <remarks>As per the specs, no exception will be thrown by this method in case of failure</remarks>
        public override void Cancel()
        {
            if (State != CommandState.InProgress) {
                _log.DebugFormat("Skipping cancel because command is in state {0}", State);
                return;
            }

            _log.Debug("Cancelling command");
            try
            {
                // get copy for thread safety of null test
                var connector = Connector;
                if (connector != null)
                {
                    connector.CancelRequest();
                }
            }
            catch (IOException)
            {
                Connection.ClearPool();
            }
            catch (Exception e)
            {
                _log.Warn("Exception caught while attempting to cancel command", e);
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
                // operations from the finalizer (connection may be in use by someone else).
                // We can implement a queue-based solution that will perform cleanup during the next possible
                // window, but this isn't trivial (should not occur in transactions because of possible exceptions,
                // etc.).

                if (_prepared == PrepareStatus.Prepared) {
                    _connector.ExecuteOrDefer("DEALLOCATE " + _planName);
                }
            }
            Transaction = null;
            Connection = null;
            State = CommandState.Disposed;
            base.Dispose(disposing);
        }

        #endregion

        #region Misc

        ///<summary>
        /// This method checks the connection state to see if the connection
        /// is set or it is open. If one of this conditions is not met, throws
        /// an InvalidOperationException
        ///</summary>
        void CheckConnectionState()
        {
            if (Connector == null)
            {
                throw new InvalidOperationException(L10N.ConnectionNotOpen);
            }

            Connector.CheckReadyState();
        }

        internal NpgsqlConnector Connector
        {
            get
            {
                if (_connection != null)
                {
                    _connector = _connection.Connector;
                }

                return _connector;
            }
        }

        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        Object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        public NpgsqlCommand Clone()
        {
            // TODO: Add consistency checks.

            var clone = new NpgsqlCommand(CommandText, Connection, Transaction)
            {
                CommandTimeout = CommandTimeout,
                CommandType = CommandType,
                DesignTimeVisible = DesignTimeVisible
            };
            if (ExpectedTypes != null)
            {
                clone.ExpectedTypes = (Type[])ExpectedTypes.Clone();
            }
            foreach (NpgsqlParameter parameter in Parameters)
            {
                clone.Parameters.Add(parameter.Clone());
            }
            return clone;
        }

        enum PrepareStatus
        {
            NotPrepared,
            NeedsPrepare,
            Prepared
        }

        #endregion Misc
    }

    enum CommandState
    {
        Idle,
        InProgress,
        Disposed
    }
}
