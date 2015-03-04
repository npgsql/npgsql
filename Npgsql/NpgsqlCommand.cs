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
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Diagnostics.Contracts;
using Npgsql.FrontendMessages;

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

        List<QueryDetails> _queries;

        int _queryIndex;

        // locals about function support so we don`t need to check it everytime a function is called.
        bool _functionChecksDone;
        bool _functionNeedsColumnListDefinition; // Functions don't return record by default.

        UpdateRowSource _updateRowSource = UpdateRowSource.Both;

        static readonly Array ParamNameCharTable;
        internal Type[] ExpectedTypes { get; set; }

        FormatCode[] _resultFormatCodes;

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

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal NpgsqlConnector.NotificationBlock _notificationBlock;

        #endregion Fields

        #region Constants

        internal const int DefaultTimeout = 20;

        /// <summary>
        /// Specifies the maximum number of queries we allow in a multiquery, separated by semicolons.
        /// We limit this because of deadlocks: as we send Parse and Bind messages to the backend, the backend
        /// replies with ParseComplete and BindComplete messages which we do not read until we finished sending
        /// all messages. Once our buffer gets full the backend will get stuck writing, and then so will we.
        /// </summary>
        const int MaxQueriesInMultiquery = 5000;

        #endregion

        #region Constructors

        // Constructors
        static NpgsqlCommand()
        {
            ParamNameCharTable = BuildParameterNameCharacterTable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class.
        /// </summary>
        public NpgsqlCommand() : this(String.Empty, null, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        public NpgsqlCommand(String cmdText) : this(cmdText, null, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query and a <see cref="NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        public NpgsqlCommand(String cmdText, NpgsqlConnection connection) : this(cmdText, connection, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommand">NpgsqlCommand</see> class with the text of the query, a <see cref="NpgsqlConnection">NpgsqlConnection</see>, and the <see cref="NpgsqlTransaction">NpgsqlTransaction</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        /// <param name="transaction">The <see cref="NpgsqlTransaction">NpgsqlTransaction</see> in which the <see cref="NpgsqlCommand">NpgsqlCommand</see> executes.</param>
        public NpgsqlCommand(string cmdText, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            Init(cmdText);
            Connection = connection;
            Transaction = transaction;
        }

        /// <summary>
        /// Used to execute internal commands.
        /// </summary>
        internal NpgsqlCommand(string cmdText, NpgsqlConnector connector, int commandTimeout = 20)
        {
            Init(cmdText);
            _connector = connector;
            CommandTimeout = commandTimeout;

            // Removed this setting. It was causing too much problem.
            // Do internal commands really need different timeout setting?
            // Internal commands aren't affected by command timeout value provided by user.
            // timeout = 20;
        }

        void Init(string cmdText)
        {
            _commandText = cmdText;
            CommandType = CommandType.Text;
            _queries = new List<QueryDetails>();
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
                DeallocatePreparedStatements();
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

                IsPrepared = false;
                _connection = value;
                Transaction = null;
                _connector = _connection == null ? null : _connection.Connector;
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
            Prechecks();
            _log.Debug("Prepare command");

            using (_connector.BlockNotifications())
            {
                DeallocatePreparedStatements();
                ProcessRawQuery();

                for (var i = 0; i < _queries.Count; i++)
                {
                    var query = _queries[i];
                    ParseMessage parseMessage;
                    DescribeMessage describeMessage;
                    if (i == 0)
                    {
                        parseMessage = _connector.ParseMessage;
                        describeMessage = _connector.DescribeMessage;
                    }
                    else
                    {
                        parseMessage = new ParseMessage();
                        describeMessage = new DescribeMessage();
                    }

                    query.PreparedStatementName = _connector.NextPreparedStatementName();
                    _connector.AddMessage(parseMessage.Populate(query, _connector.TypeHandlerRegistry));
                    _connector.AddMessage(describeMessage.Populate(StatementOrPortal.Statement, query.PreparedStatementName));
                }

                _connector.AddMessage(SyncMessage.Instance);
                _connector.SendAllMessages();

                _queryIndex = 0;

                _connector.ReadPrependedMessageResponses();

                while (true)
                {
                    var msg = _connector.ReadSingleMessage();
                    switch (msg.Code)
                    {
                        case BackendMessageCode.CompletedResponse:  // prepended messages, e.g. begin transaction
                        case BackendMessageCode.ParseComplete:
                        case BackendMessageCode.ParameterDescription:
                            continue;
                        case BackendMessageCode.RowDescription:
                            var description = (RowDescriptionMessage) msg;
                            FixupRowDescription(description, _queryIndex == 0);
                            _queries[_queryIndex++].Description = description;
                            continue;
                        case BackendMessageCode.NoData:
                            _queries[_queryIndex++].Description = null;
                            continue;
                        case BackendMessageCode.ReadyForQuery:
                            Contract.Assume(_queryIndex == _queries.Count);
                            IsPrepared = true;
                            return;
                        default:
                            throw new ArgumentOutOfRangeException("Unexpected message of type " + msg.Code);
                    }            
                }
            }
        }

        void DeallocatePreparedStatements()
        {
            if (!IsPrepared) { return; }

            // TODO: Reimplement this with protocol Close commands rather than DEALLOCATE SQL
            var sb = new StringBuilder();
            foreach (var query in _queries)
            {
                sb.Append("DEALLOCATE ");
                sb.Append(query.PreparedStatementName);
                sb.Append(';');
            }
            _connector.ExecuteBlind(sb.ToString());
            IsPrepared = false;
        }

        #endregion Prepare

        #region Query analysis

        void ProcessRawQuery()
        {
            string query;
            switch (CommandType) {
            case CommandType.Text:
                query = CommandText;
                break;
            case CommandType.TableDirect:
                query = "SELECT * FROM " + CommandText;
                break;
            case CommandType.StoredProcedure:
                var numInput = _parameters.Count(p => p.IsInputDirection);
                var sb = new StringBuilder();
                sb.Append("SELECT * FROM ");
                sb.Append(CommandText);
                sb.Append('(');
                for (var i = 1; i <= numInput; i++) {
                    sb.Append('$');
                    sb.Append(i);
                    if (i < numInput) {
                        sb.Append(',');
                    }
                }
                sb.Append(')');
                query = sb.ToString();
                break;
            default:
                throw PGUtil.ThrowIfReached();
            }

            ParseRawQuery(query);
            if (_queries.Count > 1 && _parameters.Any(p => p.IsOutputDirection)) {
                throw new NotSupportedException("Commands with multiple queries cannot have out parameters");
            }
        }

        /// <summary>
        /// Receives a raw SQL query as passed in by the user, and performs some processing necessary
        /// before sending to the backend.
        /// This includes doing parameter placebolder processing (@p => $1), and splitting the query
        /// up by semicolons if needed (SELECT 1; SELECT 2)
        /// </summary>
        /// <param name="src">Raw user-provided query</param>
        /// <returns>The queries contained in the raw text</returns>
        void ParseRawQuery(string src)
        {
            var standardConformantStrings = _connection == null || _connection.UseConformantStrings;

            var currCharOfs = 0;
            var end = src.Length;
            var ch = '\0';
            var lastChar = '\0';
            var dollarTagStart = 0;
            var dollarTagEnd = 0;
            var currTokenBeg = 0;
            var blockCommentLevel = 0;

            _queries.Clear();
            // TODO: Recycle
            var paramIndexMap = new Dictionary<string, int>();
            var currentSql = new StringWriter();
            var currentParameters = new List<NpgsqlParameter>();

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
                        currentSql.Write(src.Substring(currTokenBeg, currCharOfs - 1 - currTokenBeg));
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

                    int index;
                    if (!paramIndexMap.TryGetValue(paramName, out index))
                    {
                        // Parameter hasn't been seen before in this query
                        NpgsqlParameter parameter;
                        if (!_parameters.TryGetValue(paramName, out parameter)) {
                            throw new Exception(String.Format("Parameter '{0}' referenced in SQL but not found in parameter list", paramName));
                        }

                        if (!parameter.IsInputDirection) {
                            throw new Exception(String.Format("Parameter '{0}' referenced in SQL but is an out-only parameter", paramName));
                        }

                        currentParameters.Add(parameter);
                        index = paramIndexMap[paramName] = currentParameters.Count;
                    }
                    currentSql.Write('$');
                    currentSql.Write(index);
                    currTokenBeg = currCharOfs;

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
            currentSql.Write(src.Substring(currTokenBeg, currCharOfs - currTokenBeg - 1));
            _queries.Add(new QueryDetails(currentSql.ToString(), currentParameters));
            while (currCharOfs < end)
            {
                ch = src[currCharOfs];
                if (Char.IsWhiteSpace(ch))
                {
                    currCharOfs++;
                    continue;
                }
                // TODO: Handle end of line comment? Although psql doesn't seem to handle them...

                currTokenBeg = currCharOfs;
                paramIndexMap.Clear();
                if (_queries.Count > MaxQueriesInMultiquery) {
                    throw new NotSupportedException(String.Format("A single command cannot contain more than {0} queries", MaxQueriesInMultiquery));
                }
                currentSql = new StringWriter();
                currentParameters = new List<NpgsqlParameter>();
                goto NoneContinue;
            }
            return;

        Finish:
            currentSql.Write(src.Substring(currTokenBeg, end - currTokenBeg));
            _queries.Add(new QueryDetails(currentSql.ToString(), currentParameters));
        }

        static bool IsParamNameChar(char ch)
        {
            if (ch < '.' || ch > 'z') {
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

        #endregion

        #region Frontend message creation

        void CreateMessages(CommandBehavior behavior)
        {
            if (IsPrepared)
            {
                if ((behavior & CommandBehavior.SchemaOnly) != 0)
                {
                    // For prepared SchemaOnly queries, we already have the RowDescriptions from the Prepare phase.
                    // No need to send anything
                    return;
                }
                CreateMessagesPrepared(behavior);
            }
            else
            {
                if ((behavior & CommandBehavior.SchemaOnly) == 0) {
                    CreateMessagesNonPrepared(behavior);
                } else {
                    CreateMessagesSchemaOnly(behavior);
                }
            }
        }

        void CreateMessagesNonPrepared(CommandBehavior behavior)
        {
            Contract.Requires((behavior & CommandBehavior.SchemaOnly) == 0);

            ProcessRawQuery();

            var portalNames = _queries.Count > 1
                ? Enumerable.Range(0, _queries.Count).Select(i => "MQ" + i).ToArray()
                : null;

            for (var i = 0; i < _queries.Count; i++)
            {
                var query = _queries[i];

                ParseMessage parseMessage;
                DescribeMessage describeMessage;
                BindMessage bindMessage;
                if (i == 0)
                {
                    parseMessage = _connector.ParseMessage;
                    describeMessage = _connector.DescribeMessage;
                    bindMessage = _connector.BindMessage;
                }
                else
                {
                    parseMessage = new ParseMessage();
                    describeMessage = new DescribeMessage();
                    bindMessage = new BindMessage();
                }

                _connector.AddMessage(parseMessage.Populate(query, _connector.TypeHandlerRegistry));
                _connector.AddMessage(describeMessage.Populate(StatementOrPortal.Statement));

                bindMessage.Populate(
                    _connector.TypeHandlerRegistry,
                    query.InputParameters,
                    _queries.Count == 1 ? "" : portalNames[i]
                );
                if (AllResultTypesAreUnknown) {
                    bindMessage.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                } else if (i == 0 && UnknownResultTypeList != null) {
                    bindMessage.UnknownResultTypeList = UnknownResultTypeList;
                }
                bindMessage.Prepare();
                _connector.AddMessage(bindMessage);
            }

            if (_queries.Count == 1) {
                _connector.AddMessage(_connector.ExecuteMessage.Populate("", (behavior & CommandBehavior.SingleRow) != 0 ? 1 : 0));
            } else
                for (var i = 0; i < _queries.Count; i++) {
                    // TODO: Verify SingleRow behavior for multiqueries
                    _connector.AddMessage(new ExecuteMessage(portalNames[i], (behavior & CommandBehavior.SingleRow) != 0 ? 1 : 0));
                    _connector.AddMessage(new CloseMessage(StatementOrPortal.Portal, portalNames[i]));
                }
            _connector.AddMessage(SyncMessage.Instance);
        }

        void CreateMessagesPrepared(CommandBehavior behavior)
        {
            for (var i = 0; i < _queries.Count; i++)
            {
                BindMessage bindMessage;
                ExecuteMessage executeMessage;
                if (i == 0)
                {
                    bindMessage = _connector.BindMessage;
                    executeMessage = _connector.ExecuteMessage;
                }
                else
                {
                    bindMessage = new BindMessage();
                    executeMessage = new ExecuteMessage();
                }

                var query = _queries[i];
                bindMessage.Populate(_connector.TypeHandlerRegistry, query.InputParameters, "", query.PreparedStatementName);
                if (AllResultTypesAreUnknown) {
                    bindMessage.AllResultTypesAreUnknown = AllResultTypesAreUnknown;
                } else if (i == 0 && UnknownResultTypeList != null) {
                    bindMessage.UnknownResultTypeList = UnknownResultTypeList;
                }
                bindMessage.Prepare();
                _connector.AddMessage(bindMessage);
                _connector.AddMessage(executeMessage.Populate("", (behavior & CommandBehavior.SingleRow) != 0 ? 1 : 0));
            }
            _connector.AddMessage(SyncMessage.Instance);
        }

        void CreateMessagesSchemaOnly(CommandBehavior behavior)
        {
            Contract.Requires((behavior & CommandBehavior.SchemaOnly) != 0);

            ProcessRawQuery();

            for (var i = 0; i < _queries.Count; i++)
            {
                ParseMessage parseMessage;
                DescribeMessage describeMessage;
                if (i == 0) {
                    parseMessage = _connector.ParseMessage;
                    describeMessage = _connector.DescribeMessage;
                } else {
                    parseMessage = new ParseMessage();
                    describeMessage = new DescribeMessage();
                }

                _connector.AddMessage(parseMessage.Populate(_queries[i], _connector.TypeHandlerRegistry));
                _connector.AddMessage(describeMessage.Populate(StatementOrPortal.Statement));
            }

            _connector.AddMessage(SyncMessage.Instance);
        }

        #endregion

        #region Execute

        [GenerateAsync]
        internal NpgsqlDataReader Execute(CommandBehavior behavior = CommandBehavior.Default)
        {
            Prechecks();

            foreach (NpgsqlParameter p in Parameters.Where(p => p.IsInputDirection)) {
                p.Bind(_connector.TypeHandlerRegistry);
                p.ClearLengthCache();
                p.ValidateAndGetLength();
            }

            CreateMessages(behavior);

            NpgsqlDataReader reader = null;

            _queryIndex = 0;

            // Block the notification thread before writing anything to the wire.
            _notificationBlock = _connector.BlockNotifications();
            //using (_connector.BlockNotificationThread())
            try {
                State = CommandState.InProgress;

                if (!(IsPrepared && (behavior & CommandBehavior.SchemaOnly) != 0)) {
                    //_connector.SetBackendCommandTimeout(CommandTimeout);
                    _connector.SendAllMessages();

                    _connector.ReadPrependedMessageResponses();
                }

                if (!IsPrepared) {
                    BackendMessage msg;
                    do {
                        msg = _connector.ReadSingleMessage();
                    } while (!ProcessMessageForUnprepared(msg, behavior));
                }

                reader = new NpgsqlDataReader(this, behavior, _queries);

                return reader;
            } catch (NpgsqlException) {
                // TODO: Should probably happen inside ReadSingleMessage()
                _connector.State = ConnectorState.Ready;
                throw;
            } finally {
                if (reader == null && _notificationBlock != null) {
                    _notificationBlock.Dispose();
                }
            }
        }

        bool ProcessMessageForUnprepared(BackendMessage msg, CommandBehavior behavior)
        {
            Contract.Requires(!IsPrepared);

            switch (msg.Code) {
            case BackendMessageCode.CompletedResponse:  // e.g. begin transaction
            case BackendMessageCode.ParseComplete:
            case BackendMessageCode.ParameterDescription:
                return false;
            case BackendMessageCode.RowDescription:
                Contract.Assert(_queryIndex < _queries.Count);
                var description = (RowDescriptionMessage)msg;
                FixupRowDescription(description, _queryIndex == 0);
                _queries[_queryIndex].Description = description;
                if ((behavior & CommandBehavior.SchemaOnly) != 0) {
                    _queryIndex++;
                }
                return false;
            case BackendMessageCode.NoData:
                Contract.Assert(_queryIndex < _queries.Count);
                _queries[_queryIndex].Description = null;
                return false;
            case BackendMessageCode.BindComplete:
                Contract.Assume((behavior & CommandBehavior.SchemaOnly) == 0);
                return ++_queryIndex == _queries.Count;
            case BackendMessageCode.ReadyForQuery:
                Contract.Assume((behavior & CommandBehavior.SchemaOnly) != 0);
                return true;  // End of a SchemaOnly command
            default:
                throw new ArgumentOutOfRangeException("Unexpected message of type " + msg.Code);
            }
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
            using (reader = Execute()) {
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
            using (var reader = Execute(CommandBehavior.SequentialAccess | CommandBehavior.SingleRow))
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
                return Execute(behavior);
            }
            catch (Exception e)
            {
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

                // TODO: Total duplication with DeallocatePreparedStatements
                if (IsPrepared)
                {
                    var sb = new StringBuilder();
                    foreach (var query in _queries) {
                        sb.Append("DEALLOCATE ");
                        sb.Append(query.PreparedStatementName);
                        sb.Append(';');
                    }
                    _connector.ExecuteOrDefer(sb.ToString());
                }
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
        /// Since we send the Describe command right after the Parse and before the Bind, the resulting RowDescription
        /// will have text format on all result columns. Fix that up.
        /// </summary>
        /// <remarks>
        /// Note that UnknownResultTypeList only applies to the first query, while AllResultTypesAreUnknown applies
        /// to all of them.
        /// </remarks>
        void FixupRowDescription(RowDescriptionMessage rowDescription, bool isFirst)
        {
            for (var i = 0; i < rowDescription.NumFields; i++) {
                rowDescription[i].FormatCode =
                    (UnknownResultTypeList == null || !isFirst ? AllResultTypesAreUnknown : UnknownResultTypeList[i])
                    ? FormatCode.Text
                    : FormatCode.Binary;
            }
        }

        void Prechecks()
        {
            if (State == CommandState.Disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (Connector == null)
                throw new InvalidOperationException("Connection property has not been initialized.");

            Connector.CheckReadyState();

            Contract.Assume(_connector.Buffer.ReadBytesLeft == 0, "The read buffer should be read completely before sending Parse message");
            Contract.Assume(_connector.Buffer.WritePosition == 0, "WritePosition should be 0");
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

        #endregion Misc

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

    class QueryDetails
    {
        public QueryDetails(string sql, List<NpgsqlParameter> inputParameters, string preparedStatementName = null)
        {
            Sql = sql;
            InputParameters = inputParameters;
            PreparedStatementName = preparedStatementName;
        }

        internal readonly string Sql;
        internal readonly List<NpgsqlParameter> InputParameters;

        /// <summary>
        /// The RowDescription message for this query. If null, the query does not return rows (e.g. INSERT)
        /// </summary>
        internal RowDescriptionMessage Description;

        /// <summary>
        /// For prepared statements, holds the server-side prepared statement name.
        /// </summary>
        internal string PreparedStatementName;

        public override string ToString() { return Sql; }
    }
}
