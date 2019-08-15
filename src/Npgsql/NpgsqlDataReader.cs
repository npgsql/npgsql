using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandling;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

#pragma warning disable CA2222 // Do not decrease inherited member visibility
namespace Npgsql
{
    /// <summary>
    /// Reads a forward-only stream of rows from a data source.
    /// </summary>
#pragma warning disable CA1010
    public sealed class NpgsqlDataReader : DbDataReader
#pragma warning restore CA1010
#if !NET461
        , IDbColumnSchemaGenerator
#endif
    {
        internal NpgsqlCommand Command { get; private set; } = default!;
        internal NpgsqlConnector Connector { get; }
        NpgsqlConnection _connection = default!;

        /// <summary>
        /// The behavior of the command with which this reader was executed.
        /// </summary>
        CommandBehavior _behavior;

        Task _sendTask = default!;

        internal ReaderState State;

        internal NpgsqlReadBuffer Buffer = default!;

        /// <summary>
        /// Holds the list of statements being executed by this reader.
        /// </summary>
        List<NpgsqlStatement> _statements = default!;

        /// <summary>
        /// The index of the current query resultset we're processing (within a multiquery)
        /// </summary>
        internal int StatementIndex { get; private set; }

        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        int _numColumns;

        /// <summary>
        /// Records, for each column, its starting offset and length in the current row.
        /// Used only in non-sequential mode.
        /// </summary>
        readonly List<(int Offset, int Length)> _columns = new List<(int Offset, int Length)>();

        /// <summary>
        /// The index of the column that we're on, i.e. that has already been parsed, is
        /// is memory and can be retrieved. Initialized to -1, which means we're on the column
        /// count (which comes before the first column).
        /// </summary>
        int _column;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the byte length of the column.
        /// Does not include the length prefix.
        /// </summary>
        internal int ColumnLen;

        internal int PosInColumn;

        /// <summary>
        /// The position in the buffer at which the current data row message ends.
        /// Used only in non-sequential mode.
        /// </summary>
        int _dataMsgEnd;

        int _charPos;

        /// <summary>
        /// The RowDescription message for the current resultset being processed
        /// </summary>
        internal RowDescriptionMessage? RowDescription;

        ulong? _recordsAffected;

        /// <summary>
        /// Whether the current result set has rows
        /// </summary>
        bool _hasRows;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler? ReaderClosed;

        bool _isSchemaOnly;
        bool _isSequential;

        /// <summary>
        /// A stream that has been opened on a column.
        /// </summary>
        NpgsqlReadBuffer.ColumnStream? ColumnStream;

        /// <summary>
        /// Used for internal temporary purposes
        /// </summary>
        char[]? _tempCharBuf;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlDataReader));

        internal NpgsqlDataReader(NpgsqlConnector connector)
        {
            Connector = connector;
        }

        internal void Init(NpgsqlCommand command, CommandBehavior behavior, List<NpgsqlStatement> statements, Task sendTask)
        {
            Command = command;
            Debug.Assert(command.Connection == Connector.Connection);
            _connection = command.Connection;
            _behavior = behavior;
            _isSchemaOnly = _behavior.HasFlag(CommandBehavior.SchemaOnly);
            _isSequential = _behavior.HasFlag(CommandBehavior.SequentialAccess);
            _statements = statements;
            StatementIndex = -1;
            _sendTask = sendTask;
            State = ReaderState.BetweenResults;
            _recordsAffected = null;
        }

        #region Read

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns><b>true</b> if there are more rows; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// The default position of a data reader is before the first record. Therefore, you must call Read to begin accessing data.
        /// </remarks>
        public override bool Read()
        {
            CheckClosed();
            var fastRead = TryFastRead();
            return fastRead.HasValue
                ? fastRead.Value
                : Read(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// This is the asynchronous version of <see cref="Read()"/> The cancellation token is currently ignored.
        /// </summary>
        /// <param name="cancellationToken">Ignored for now.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            CheckClosed();

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);

            var fastRead = TryFastRead();
            if (fastRead.HasValue)
                return fastRead.Value ? PGUtil.TrueTask : PGUtil.FalseTask;

            using (NoSynchronizationContextScope.Enter())
                return Read(true);
        }

        bool? TryFastRead()
        {
            // This is an optimized execution path that avoids calling any async methods for the (usual)
            // case where the next row (or CommandComplete) is already in memory.

            if (_behavior.HasFlag(CommandBehavior.SingleRow))
                return null;

            switch (State)
            {
            case ReaderState.BeforeResult:
                // First Read() after NextResult. Data row has already been processed.
                State = ReaderState.InResult;
                return true;
            case ReaderState.InResult:
                if (_isSequential)
                    return null;
                ConsumeRowNonSequential();
                break;
            case ReaderState.BetweenResults:
            case ReaderState.Consumed:
            case ReaderState.Closed:
                return false;
            }

            var readBuf = Connector.ReadBuffer;
            if (readBuf.ReadBytesLeft < 5)
                return null;
            var messageCode = (BackendMessageCode)readBuf.ReadByte();
            var len = readBuf.ReadInt32() - 4;  // Transmitted length includes itself
            if (messageCode != BackendMessageCode.DataRow || readBuf.ReadBytesLeft < len)
            {
                readBuf.ReadPosition -= 5;
                return null;
            }

            var msg = Connector.ParseServerMessage(readBuf, messageCode, len, false)!;
            Debug.Assert(msg.Code == BackendMessageCode.DataRow);
            ProcessMessage(msg);
            return true;
        }

        async Task<bool> Read(bool async)
        {
            switch (State)
            {
            case ReaderState.BeforeResult:
                // First Read() after NextResult. Data row has already been processed.
                State = ReaderState.InResult;
                return true;

            case ReaderState.InResult:
                await ConsumeRow(async);
                if (_behavior.HasFlag(CommandBehavior.SingleRow))
                {
                    // TODO: See optimization proposal in #410
                    await Consume(async);
                    return false;
                }
                break;

            case ReaderState.BetweenResults:
            case ReaderState.Consumed:
            case ReaderState.Closed:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                var msg2 = await ReadMessage(async);
                ProcessMessage(msg2);
                return msg2.Code == BackendMessageCode.DataRow;
            }
            catch (PostgresException)
            {
                State = ReaderState.Consumed;
                throw;
            }
        }

        ValueTask<IBackendMessage> ReadMessage(bool async)
        {
            return _isSequential
                ? ReadMessageSequential(async)
                : Connector.ReadMessage(async, DataRowLoadingMode.NonSequential);

            async ValueTask<IBackendMessage> ReadMessageSequential(bool async2)
            {
                var msg = await Connector.ReadMessage(async2, DataRowLoadingMode.Sequential);
                if (msg.Code == BackendMessageCode.DataRow)
                {
                    // Make sure that the datarow's column count is already buffered
                    await Connector.ReadBuffer.Ensure(2, async);
                    return msg;
                }
                return msg;
            }
        }

        #endregion

        #region NextResult

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of statements.
        /// </summary>
        /// <returns></returns>
        public sealed override bool NextResult()
        {
            try
            {
                return (_isSchemaOnly ? NextResultSchemaOnly(false) : NextResult(false))
                    .GetAwaiter().GetResult();
            }
            catch (PostgresException e)
            {
                State = ReaderState.Consumed;
                if (StatementIndex >= 0 && StatementIndex < _statements.Count)
                    e.Statement = _statements[StatementIndex];
                throw;
            }
        }

        /// <summary>
        /// This is the asynchronous version of NextResult.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);
            try
            {
                using (NoSynchronizationContextScope.Enter())
                    return _isSchemaOnly ? NextResultSchemaOnly(true) : NextResult(true);
            }
            catch (PostgresException e)
            {
                State = ReaderState.Consumed;
                if (StatementIndex >= 0 && StatementIndex < _statements.Count)
                    e.Statement = _statements[StatementIndex];
                throw;
            }
        }

        /// <summary>
        /// Internal implementation of NextResult
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task<bool> NextResult(bool async, bool isConsuming=false)
        {
            CheckClosed();

            IBackendMessage msg;
            Debug.Assert(!_isSchemaOnly);

            // If we're in the middle of a resultset, consume it
            switch (State)
            {
            case ReaderState.BeforeResult:
            case ReaderState.InResult:
                await ConsumeRow(async);
                while (true)
                {
                    var completedMsg = await Connector.ReadMessage(async, DataRowLoadingMode.Skip);
                    switch (completedMsg.Code)
                    {
                    case BackendMessageCode.CompletedResponse:
                    case BackendMessageCode.EmptyQueryResponse:
                        ProcessMessage(completedMsg);
                        break;
                    default:
                        continue;
                    }

                    break;
                }

                break;

            case ReaderState.BetweenResults:
                break;

            case ReaderState.Consumed:
            case ReaderState.Closed:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
            }

            Debug.Assert(State == ReaderState.BetweenResults);
            _hasRows = false;

            if (_behavior.HasFlag(CommandBehavior.SingleResult) && StatementIndex == 0 && !isConsuming)
            {
                await Consume(async);
                return false;
            }

            // We are now at the end of the previous result set. Read up to the next result set, if any.
            // Non-prepared statements receive ParseComplete, BindComplete, DescriptionRow/NoData,
            // prepared statements receive only BindComplete
            for (StatementIndex++; StatementIndex < _statements.Count; StatementIndex++)
            {
                var statement = _statements[StatementIndex];
                if (statement.IsPrepared)
                {
                    Expect<BindCompleteMessage>(await Connector.ReadMessage(async), Connector);
                    RowDescription = statement.Description;
                }
                else  // Non-prepared flow
                {
                    var pStatement = statement.PreparedStatement;
                    if (pStatement != null)
                    {
                        Debug.Assert(!pStatement.IsPrepared);
                        Debug.Assert(pStatement.Description == null);
                        if (pStatement.StatementBeingReplaced != null)
                        {
                            Expect<CloseCompletedMessage>(await Connector.ReadMessage(async), Connector);
                            pStatement.StatementBeingReplaced.CompleteUnprepare();
                            pStatement.StatementBeingReplaced = null;
                        }
                    }

                    try
                    {
                        Expect<ParseCompleteMessage>(await Connector.ReadMessage(async), Connector);
                    }
                    catch
                    {
                        // An exception occurred. Check if any statements we being prepared and revert our bookkeeping.
                        pStatement?.CompleteUnprepare();
                        throw;
                    }

                    pStatement?.CompletePrepare();

                    Expect<BindCompleteMessage>(await Connector.ReadMessage(async), Connector);
                    msg = await Connector.ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.NoData:
                        RowDescription = statement.Description = null;
                        break;
                    case BackendMessageCode.RowDescription:
                        // We have a resultset
                        RowDescription = statement.Description = (RowDescriptionMessage)msg;
                        break;
                    default:
                        throw Connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                if (RowDescription == null)
                {
                    // Statement did not generate a resultset (e.g. INSERT)
                    // Read and process its completion message and move on to the next statement

                    msg = await ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.CompletedResponse:
                    case BackendMessageCode.EmptyQueryResponse:
                        break;
                    default:
                        throw Connector.UnexpectedMessageReceived(msg.Code);
                    }

                    ProcessMessage(msg);
                    continue;
                }

                if (StatementIndex == 0 && Command.Parameters.HasOutputParameters)
                {
                    // If output parameters are present and this is the first row of the first resultset,
                    // we must always read it in non-sequential mode because it will be traversed twice (once
                    // here for the parameters, then as a regular row).
                    msg = await Connector.ReadMessage(async, DataRowLoadingMode.NonSequential);
                    ProcessMessage(msg);
                    if (msg.Code == BackendMessageCode.DataRow)
                        PopulateOutputParameters();
                }
                else
                {
                    msg = await ReadMessage(async);
                    ProcessMessage(msg);
                }

                switch (msg.Code)
                {
                case BackendMessageCode.DataRow:
                case BackendMessageCode.CompletedResponse:
                    break;
                default:
                    throw Connector.UnexpectedMessageReceived(msg.Code);
                }

                return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            ProcessMessage(Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector));
            RowDescription = null;
            return false;
        }

        void PopulateOutputParameters()
        {
            // The first row in a stored procedure command that has output parameters needs to be traversed twice -
            // once for populating the output parameters and once for the actual result set traversal. So in this
            // case we can't be sequential.
            Debug.Assert(Command.Parameters.Any(p => p.IsOutputDirection));
            Debug.Assert(StatementIndex == 0);
            Debug.Assert(RowDescription != null);
            Debug.Assert(State == ReaderState.BeforeResult);

            // Temporarily set our state to InResult to allow us to read the values
            State = ReaderState.InResult;

            var pending = new Queue<object>();
            var taken = new List<NpgsqlParameter>();
            for (var i = 0; i < FieldCount; i++)
            {
                if (Command.Parameters.TryGetValue(GetName(i), out var p) && p.IsOutputDirection)
                {
                    p.Value = GetValue(i);
                    taken.Add(p);
                }
                else
                    pending.Enqueue(GetValue(i));
            }

            // Not sure where this odd behavior comes from: all output parameters which did not get matched by
            // name now get populated with column values which weren't matched. Keeping this for backwards compat,
            // opened #2252 for investigation.
            foreach (var p in Command.Parameters.Where(p => p.IsOutputDirection && !taken.Contains(p)))
            {
                if (pending.Count == 0)
                    break;
                p.Value = pending.Dequeue();
            }

            State = ReaderState.BeforeResult;  // Set the state back
        }

        /// <summary>
        /// Note that in SchemaOnly mode there are no resultsets, and we read nothing from the backend (all
        /// RowDescriptions have already been processed and are available)
        /// </summary>
        async Task<bool> NextResultSchemaOnly(bool async)
        {
            Debug.Assert(_isSchemaOnly);

            for (StatementIndex++; StatementIndex < _statements.Count; StatementIndex++)
            {
                var statement = _statements[StatementIndex];
                if (statement.IsPrepared)
                {
                    // Row descriptions have already been populated in the statement objects at the
                    // Prepare phase
                    RowDescription = _statements[StatementIndex].Description;
                }
                else
                {
                    Expect<ParseCompleteMessage>(await Connector.ReadMessage(async), Connector);
                    Expect<ParameterDescriptionMessage>(await Connector.ReadMessage(async), Connector);
                    var msg = await Connector.ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.NoData:
                        RowDescription = _statements[StatementIndex].Description = null;
                        break;
                    case BackendMessageCode.RowDescription:
                        // We have a resultset
                        RowDescription = _statements[StatementIndex].Description = (RowDescriptionMessage)msg;
                        Command.FixupRowDescription(RowDescription, StatementIndex == 0);
                        break;
                    default:
                        throw Connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                // Found a resultset
                if (RowDescription != null)
                    return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            if (!_statements.All(s => s.IsPrepared))
            {
                ProcessMessage(Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector));
                RowDescription = null;
            }
            return false;
        }

        #endregion

        #region ProcessMessage

        internal void ProcessMessage(IBackendMessage msg)
        {
            switch (msg.Code)
            {
            case BackendMessageCode.DataRow:
                ProcessDataRowMessage((DataRowMessage)msg);
                return;

            case BackendMessageCode.CompletedResponse:
                var completed = (CommandCompleteMessage)msg;
                switch (completed.StatementType)
                {
                case StatementType.Update:
                case StatementType.Insert:
                case StatementType.Delete:
                case StatementType.Copy:
                case StatementType.Move:
                    if (!_recordsAffected.HasValue)
                        _recordsAffected = 0;
                    _recordsAffected += completed.Rows;
                    break;
                }

                _statements[StatementIndex].ApplyCommandComplete(completed);
                goto case BackendMessageCode.EmptyQueryResponse;

            case BackendMessageCode.EmptyQueryResponse:
                State = ReaderState.BetweenResults;
                return;

            case BackendMessageCode.ReadyForQuery:
                State = ReaderState.Consumed;
                return;

            default:
                throw new Exception("Received unexpected backend message of type " + msg.Code);
            }
        }

        void ProcessDataRowMessage(DataRowMessage msg)
        {
            Connector.State = ConnectorState.Fetching;

            // The connector's buffer can actually change between DataRows:
            // If a large DataRow exceeding the connector's current read buffer arrives, and we're
            // reading in non-sequential mode, a new oversize buffer is allocated. We thus have to
            // recapture the connector's buffer on each new DataRow.
            // Note that this can happen even in sequential mode, if the row description message is big
            // (see #2003)
            Buffer = Connector.ReadBuffer;

            _hasRows = true;
            _column = -1;
            ColumnLen = -1;
            PosInColumn = 0;

            // We assume that the row's number of columns is identical to the description's
            _numColumns = Buffer.ReadInt16();
            Debug.Assert(_numColumns == RowDescription!.NumFields,
                $"Row's number of columns ({_numColumns}) differs from the row description's ({RowDescription.NumFields})");

            if (!_isSequential)
            {
                _dataMsgEnd = Buffer.ReadPosition + msg.Length - 2;

                // Initialize our columns array with the offset and length of the first column
                _columns.Clear();
                var len = Buffer.ReadInt32();
                _columns.Add((Buffer.ReadPosition, len));
            }

            switch (State)
            {
            case ReaderState.BetweenResults:
                State = ReaderState.BeforeResult;
                break;
            case ReaderState.BeforeResult:
                State = ReaderState.InResult;
                break;
            case ReaderState.InResult:
                break;
            default:
                throw Connector.UnexpectedMessageReceived(BackendMessageCode.DataRow);
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.  Always returns zero.
        /// </summary>
        public override int Depth => 0;

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed => State == ReaderState.Closed;

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        public override int RecordsAffected => _recordsAffected.HasValue ? (int)_recordsAffected.Value : -1;

        /// <summary>
        /// Returns details about each statement that this reader will or has executed.
        /// </summary>
        /// <remarks>
        /// Note that some fields (i.e. rows and oid) are only populated as the reader
        /// traverses the result.
        ///
        /// For commands with multiple queries, this exposes the number of rows affected on
        /// a statement-by-statement basis, unlike <see cref="NpgsqlDataReader.RecordsAffected"/>
        /// which exposes an aggregation across all statements.
        /// </remarks>
        public IReadOnlyList<NpgsqlStatement> Statements => _statements.AsReadOnly();

        /// <summary>
        /// Gets a value that indicates whether this DbDataReader contains one or more rows.
        /// </summary>
        public override bool HasRows => State == ReaderState.Closed
            ? throw new InvalidOperationException("Invalid attempt to call HasRows when reader is closed.")
            : _hasRows;

        /// <summary>
        /// Indicates whether the reader is currently positioned on a row, i.e. whether reading a
        /// column is possible.
        /// This property is different from <see cref="HasRows"/> in that <see cref="HasRows"/> will
        /// return true even if attempting to read a column will fail, e.g. before <see cref="Read()"/>
        /// has been called
        /// </summary>
        [PublicAPI]
        public bool IsOnRow => State == ReaderState.InResult;

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal) => CheckRowDescriptionAndGetField(ordinal).Name;

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override int FieldCount
        {
            get
            {
                CheckClosed();
                return RowDescription?.NumFields ?? 0;
            }
        }

        #region Cleanup / Dispose

        /// <summary>
        /// Consumes all result sets for this reader, leaving the connector ready for sending and processing further
        /// queries
        /// </summary>
        async Task Consume(bool async)
        {
            // Skip over the other result sets. Note that this does tally records affected
            // from CommandComplete messages, and properly sets state for auto-prepared statements
            if (_isSchemaOnly)
                while (await NextResultSchemaOnly(async)) {}
            else
                while (await NextResult(async, true)) {}
        }

        /// <summary>
        /// Releases the resources used by the <see cref="NpgsqlDataReader">NpgsqlDataReader</see>.
        /// </summary>
        protected override void Dispose(bool disposing) => Close();

        /// <summary>
        /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
        /// </summary>
        public override void Close() => Close(false, false).GetAwaiter().GetResult();

        /// <summary>
        /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
        /// </summary>
#if !NET461 && !NETSTANDARD2_0
        public override Task CloseAsync()
#else
        public Task CloseAsync()
#endif
            => Close(false, true);

        internal async Task Close(bool connectionClosing, bool async)
        {
            if (State == ReaderState.Closed)
                return;

            switch (Connector.State)
            {
            case ConnectorState.Broken:
            case ConnectorState.Closed:
                // This may have happen because an I/O error while reading a value, or some non-safe
                // exception thrown from a type handler. Or if the connection was closed while the reader
                // was still open
                State = ReaderState.Closed;
                Command.State = CommandState.Idle;
                ReaderClosed?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (State != ReaderState.Consumed)
                await Consume(async);

            await Cleanup(async, connectionClosing);
        }

        internal async Task Cleanup(bool async, bool connectionClosing=false)
        {
            Log.Trace("Cleaning up reader", Connector.Id);

            // Make sure the send task for this command, which may have executed asynchronously and in
            // parallel with the reading, has completed, throwing any exceptions it generated.
            if (async)
                await _sendTask;
            else
                _sendTask.GetAwaiter().GetResult();

            State = ReaderState.Closed;
            Command.State = CommandState.Idle;
            Connector.CurrentReader = null;
            Connector.EndUserAction();
            NpgsqlEventSource.Log.CommandStop();

            // If the reader is being closed as part of the connection closing, we don't apply
            // the reader's CommandBehavior.CloseConnection
            if (_behavior.HasFlag(CommandBehavior.CloseConnection) && !connectionClosing)
                _connection.Close();

            if (ReaderClosed != null)
            {
                ReaderClosed(this, EventArgs.Empty);
                ReaderClosed = null;
            }
        }

        #endregion

        #region Simple value getters

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override bool GetBoolean(int ordinal) => GetFieldValue<bool>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal) => GetFieldValue<byte>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a single character.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal) => GetFieldValue<char>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal) => GetFieldValue<short>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal) => GetFieldValue<int>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal) => GetFieldValue<long>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override DateTime GetDateTime(int ordinal) => GetFieldValue<DateTime>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="string"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal) => GetFieldValue<string>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a <see cref="decimal"/> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal) => GetFieldValue<decimal>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a double-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal) => GetFieldValue<double>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a single-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal) => GetFieldValue<float>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a globally-unique identifier (GUID).
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal) => GetFieldValue<Guid>(ordinal);

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
        public override int GetValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckResultSet();

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
                values[i] = GetValue(i);
            return count;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[int ordinal] => GetValue(ordinal);

        #endregion

        #region Provider-specific simple type getters

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlDate"/>,
        /// Npgsql's provider-specific type for dates.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's date type represents dates from 4713 BC to 5874897 AD, while .NET's DateTime
        /// only supports years from 1 to 1999. If you require years outside this range use this accessor.
        /// The standard <see cref="DbDataReader.GetProviderSpecificValue"/> method will also return this type, but has
        /// the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlDate GetDate(int ordinal) => GetFieldValue<NpgsqlDate>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a TimeSpan,
        /// </summary>
        /// <remarks>
        /// PostgreSQL's interval type has has a resolution of 1 microsecond and ranges from
        /// -178000000 to 178000000 years, while .NET's TimeSpan has a resolution of 100 nanoseconds
        /// and ranges from roughly -29247 to 29247 years.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public TimeSpan GetTimeSpan(int ordinal) => GetFieldValue<TimeSpan>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlTimeSpan"/>,
        /// Npgsql's provider-specific type for time spans.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's interval type has has a resolution of 1 microsecond and ranges from
        /// -178000000 to 178000000 years, while .NET's TimeSpan has a resolution of 100 nanoseconds
        /// and ranges from roughly -29247 to 29247 years. If you require values from outside TimeSpan's
        /// range use this accessor.
        /// The standard ADO.NET <see cref="DbDataReader.GetProviderSpecificValue"/> method will also return this
        /// type, but has the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlTimeSpan GetInterval(int ordinal) => GetFieldValue<NpgsqlTimeSpan>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlDateTime"/>,
        /// Npgsql's provider-specific type for date/time timestamps. Note that this type covers
        /// both PostgreSQL's "timestamp with time zone" and "timestamp without time zone" types,
        /// which differ only in how they are converted upon input/output.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's timestamp type represents dates from 4713 BC to 5874897 AD, while .NET's DateTime
        /// only supports years from 1 to 1999. If you require years outside this range use this accessor.
        /// The standard <see cref="DbDataReader.GetProviderSpecificValue"/> method will also return this type, but has
        /// the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlDateTime GetTimeStamp(int ordinal) => GetFieldValue<NpgsqlDateTime>(ordinal);

        #endregion

        #region Special binary getters

        /// <summary>
        /// Reads a stream of bytes from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");

            var fieldDescription = CheckRowAndGetField(ordinal);
            var handler = fieldDescription.Handler;
            if (!(handler is ByteaHandler))
                throw new InvalidCastException("GetBytes() not supported for type " + fieldDescription.Name);

            SeekToColumn(ordinal, false).GetAwaiter().GetResult();
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");
            if (buffer == null)
                return ColumnLen;

            var dataOffset2 = (int)dataOffset;
            SeekInColumn(dataOffset2, false).GetAwaiter().GetResult();

            // Attempt to read beyond the end of the column
            if (dataOffset2 + length > ColumnLen)
                length = Math.Max(ColumnLen - dataOffset2, 0);

            var left = length;
            while (left > 0)
            {
                var read = Buffer.ReadBytes(buffer, bufferOffset, left, false).GetAwaiter().GetResult();
                bufferOffset += read;
                left -= read;
            }

            PosInColumn += length;

            return length;
        }

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override Stream GetStream(int ordinal) => GetStream(ordinal, false).Result;

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public Task<Stream> GetStreamAsync(int ordinal)
        {
            using (NoSynchronizationContextScope.Enter())
                return GetStream(ordinal, true).AsTask();
        }

        ValueTask<Stream> GetStream(int ordinal, bool async)
        {
            var fieldDescription = CheckRowAndGetField(ordinal);
            if (!(fieldDescription.Handler is ByteaHandler))
                throw new InvalidCastException($"GetStream() not supported for type {fieldDescription.Handler.PgDisplayName}");

            return GetStreamInternal(ordinal, async);
        }

        ValueTask<Stream> GetStreamInternal(int ordinal, bool async)
        {
            if (ColumnStream != null && !ColumnStream.IsDisposed)
                throw new InvalidOperationException("A stream is already open for this reader");

            var t = SeekToColumn(ordinal, async);
            if (!t.IsCompleted)
                return new ValueTask<Stream>(GetStreamLong(t));

            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");
            PosInColumn += ColumnLen;
            return new ValueTask<Stream>(ColumnStream = (NpgsqlReadBuffer.ColumnStream)Buffer.GetStream(ColumnLen, !_isSequential));

            async Task<Stream> GetStreamLong(Task seekTask)
            {
                await seekTask;
                if (ColumnLen == -1)
                    throw new InvalidCastException("Column is null");
                PosInColumn += ColumnLen;
                return ColumnStream = (NpgsqlReadBuffer.ColumnStream)Buffer.GetStream(ColumnLen, !_isSequential);
            }
        }

        #endregion

        #region Special text getters

        /// <summary>
        /// Reads a stream of characters from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");

            var fieldDescription = CheckRowAndGetField(ordinal);
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null)
                throw new InvalidCastException("GetChars() not supported for type " + fieldDescription.Name);

            SeekToColumn(ordinal, false).GetAwaiter().GetResult();
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");
            if (PosInColumn == 0)
                _charPos = 0;

            var decoder = Buffer.TextEncoding.GetDecoder();

            if (buffer == null)
            {
                // Note: Getting the length of a text column means decoding the entire field,
                // very inefficient and also consumes the column in sequential mode. But this seems to
                // be SqlClient's behavior as well.
                var (bytesSkipped, charsSkipped) = SkipChars(decoder, int.MaxValue, ColumnLen - PosInColumn);
                Debug.Assert(bytesSkipped == ColumnLen - PosInColumn);
                PosInColumn += bytesSkipped;
                _charPos += charsSkipped;
                return _charPos;
            }

            if (PosInColumn == ColumnLen || dataOffset < _charPos)
            {
                // Either the column has already been read (e.g. GetString()) or a previous GetChars()
                // has positioned us in the column *after* the requested read start offset. Seek back
                // (this will throw for sequential)
                SeekInColumn(0, false).GetAwaiter().GetResult();
                _charPos = 0;
            }

            if (dataOffset > _charPos)
            {
                var charsToSkip = (int)dataOffset - _charPos;
                var (bytesSkipped, charsSkipped) = SkipChars(decoder, charsToSkip, ColumnLen - PosInColumn);
                decoder.Reset();
                PosInColumn += bytesSkipped;
                _charPos += charsSkipped;
                if (charsSkipped < charsToSkip) // data offset is beyond the column's end
                    return 0;
            }

            // We're now positioned at the start of the segment of characters we need to read.
            if (length == 0)
                return 0;

            var (bytesRead, charsRead) = DecodeChars(decoder, buffer, bufferOffset, length, ColumnLen - PosInColumn);

            PosInColumn += bytesRead;
            _charPos += charsRead;
            return charsRead;
        }

        (int BytesRead, int CharsRead) DecodeChars(Decoder decoder, char[] output, int outputOffset, int charCount, int byteCount)
        {
            var (bytesRead, charsRead) = (0, 0);

            while (true)
            {
                Buffer.Ensure(1); // Make sure we have at least some data

                var maxBytes = Math.Min(byteCount - bytesRead, Buffer.ReadBytesLeft);
                decoder.Convert(Buffer.Buffer, Buffer.ReadPosition, maxBytes, output, outputOffset, charCount - charsRead, false,
                    out var bytesUsed, out var charsUsed, out var completed);
                Buffer.ReadPosition += bytesUsed;
                bytesRead += bytesUsed;
                charsRead += charsUsed;
                if (charsRead == charCount || bytesRead == byteCount)
                    break;
                outputOffset += charsUsed;
                Buffer.Clear();
            }

            return (bytesRead, charsRead);
        }

        internal (int BytesSkipped, int CharsSkipped) SkipChars(Decoder decoder, int charCount, int byteCount)
        {
            // TODO: Allocate on the stack with Span
            if (_tempCharBuf == null)
                _tempCharBuf = new char[1024];
            var (charsSkipped, bytesSkipped) = (0, 0);
            while (charsSkipped < charCount && bytesSkipped < byteCount)
            {
                var (bytesRead, charsRead) = DecodeChars(decoder, _tempCharBuf, 0, Math.Min(charCount, _tempCharBuf.Length), byteCount);
                bytesSkipped += bytesRead;
                charsSkipped += charsRead;
            }
            return (bytesSkipped, charsSkipped);
        }

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override TextReader GetTextReader(int ordinal)
            => GetTextReader(ordinal, false).Result;

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public Task<TextReader> GetTextReaderAsync(int ordinal)
        {
            using (NoSynchronizationContextScope.Enter())
                return GetTextReader(ordinal, true).AsTask();
        }

        async ValueTask<TextReader> GetTextReader(int ordinal, bool async)
        {
            var fieldDescription = CheckRowAndGetField(ordinal);
            if (!(fieldDescription.Handler is ITextReaderHandler handler))
                throw new InvalidCastException($"GetTextReader() not supported for type {fieldDescription.Handler.PgDisplayName}");

            var stream = async
                ? await GetStreamInternal(ordinal, async)
                : GetStreamInternal(ordinal, async).Result;

            return handler.GetTextReader(stream);
        }

        #endregion

        #region GetFieldValue

        /// <summary>
        /// Asynchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="ordinal">The type of the value to be returned.</param>
        /// <param name="cancellationToken">
        /// The cancellation instruction, which propagates a notification that operations should be canceled.
        /// This does not guarantee the cancellation.
        /// </param>
        /// <returns></returns>
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // In non-sequential, we know that the column is already buffered - no I/O will take place
            if (!_isSequential)
                return Task.FromResult(GetFieldValue<T>(ordinal));

            using (NoSynchronizationContextScope.Enter())
                return GetFieldValueSequential<T>(ordinal, true).AsTask();
        }

        /// <summary>
        /// Synchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <returns>The column to be retrieved.</returns>
        public override T GetFieldValue<T>(int ordinal)
        {
            if (_isSequential)
                return GetFieldValueSequential<T>(ordinal, false).GetAwaiter().GetResult();

            // In non-sequential, we know that the column is already buffered - no I/O will take place

            var fieldDescription = CheckRowAndGetField(ordinal);
            SeekToColumnNonSequential(ordinal);

            if (ColumnLen == -1)
            {
#pragma warning disable CS8653 // A default expression introduces a null value when 'T' is a non-nullable reference type.
                // When T is a Nullable<T>, we support returning null
                if (NullableHandler<T>.Exists)
                    return default;
#pragma warning restore CS8653
                if (typeof(T) == typeof(object))
                    return (T)(object)DBNull.Value;
                throw new InvalidCastException("Column is null");
            }

            try
            {
                return NullableHandler<T>.Exists
                    ? NullableHandler<T>.Read(fieldDescription.Handler, Buffer, ColumnLen, fieldDescription)
                    : typeof(T) == typeof(object)
                        ? (T)fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, fieldDescription)
                        : fieldDescription.Handler.Read<T>(Buffer, ColumnLen, fieldDescription);
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.OriginalException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }
        }

        async ValueTask<T> GetFieldValueSequential<T>(int column, bool async)
        {
            var fieldDescription = CheckRowAndGetField(column);
            await SeekToColumnSequential(column, async);
            CheckColumnStart();

            if (ColumnLen == -1)
            {
#pragma warning disable CS8653 // A default expression introduces a null value when 'T' is a non-nullable reference type.
                // When T is a Nullable<T>, we support returning null
                if (NullableHandler<T>.Exists)
                    return default;
#pragma warning restore CS8653
                if (typeof(T) == typeof(object))
                    return (T)(object)DBNull.Value;
                throw new InvalidCastException("Column is null");
            }

            try
            {
                return NullableHandler<T>.Exists
                    ? ColumnLen <= Buffer.ReadBytesLeft
                        ? NullableHandler<T>.Read(fieldDescription.Handler, Buffer, ColumnLen, fieldDescription)
                        : await NullableHandler<T>.ReadAsync(fieldDescription.Handler, Buffer, ColumnLen, async, fieldDescription)
                    : typeof(T) == typeof(object)
                        ? ColumnLen <= Buffer.ReadBytesLeft
                            ? (T)fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, fieldDescription)
                            : (T)await fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, async, fieldDescription)
                        : ColumnLen <= Buffer.ReadBytesLeft
                            ? fieldDescription.Handler.Read<T>(Buffer, ColumnLen, fieldDescription)
                            : await fieldDescription.Handler.Read<T>(Buffer, ColumnLen, async, fieldDescription);
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.OriginalException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }
        }

        #endregion

        #region GetValue

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal)
        {
            var fieldDescription = CheckRowAndGetField(ordinal);

            if (_isSequential) {
                SeekToColumnSequential(ordinal, false).GetAwaiter().GetResult();
                CheckColumnStart();
            } else
                SeekToColumnNonSequential(ordinal);

            if (ColumnLen == -1)
                return DBNull.Value;

            object result;
            try
            {
                result = _isSequential
                    ? fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, false, fieldDescription).GetAwaiter().GetResult()
                    : fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, fieldDescription);
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.OriginalException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }

            // Used for Entity Framework <= 6 compability
            var objectResultType = Command.ObjectResultTypes?[ordinal];
            if (objectResultType != null)
            {
                result = objectResultType == typeof(DateTimeOffset)
                    ? new DateTimeOffset((DateTime)result)
                    : Convert.ChangeType(result, objectResultType)!;
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetProviderSpecificValue(int ordinal)
        {
            var fieldDescription = CheckRowAndGetField(ordinal);

            if (_isSequential)
            {
                SeekToColumnSequential(ordinal, false).GetAwaiter().GetResult();
                CheckColumnStart();
            }
            else
                SeekToColumnNonSequential(ordinal);

            if (ColumnLen == -1)
                return DBNull.Value;

            try
            {
                return _isSequential
                    ? fieldDescription.Handler.ReadPsvAsObject(Buffer, ColumnLen, false, fieldDescription).GetAwaiter().GetResult()
                    : fieldDescription.Handler.ReadPsvAsObject(Buffer, ColumnLen, fieldDescription);
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.OriginalException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[string name] => GetValue(GetOrdinal(name));

        #endregion

        #region IsDBNull

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public override bool IsDBNull(int ordinal)
        {
            CheckRowAndGetField(ordinal);

            if (_isSequential)
                SeekToColumnSequential(ordinal, false).GetAwaiter().GetResult();
            else
                SeekToColumnNonSequential(ordinal);

            return ColumnLen == -1;
        }

        /// <summary>
        /// An asynchronous version of <see cref="IsDBNull(int)"/>, which gets a value that indicates whether the column contains non-existent or missing values.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns><b>true</b> if the specified column value is equivalent to <see cref="DBNull"/> otherwise <b>false</b>.</returns>
        public override async Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            if (!_isSequential)
                return IsDBNull(ordinal);

            CheckRowAndGetField(ordinal);

            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
            {
                await SeekToColumn(ordinal, true);
                return ColumnLen == -1;
            }
        }

        #endregion

        #region Other public accessors

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty", nameof(name));
            if (State == ReaderState.Closed)
                throw new InvalidOperationException("The reader is closed");
            if (RowDescription is null)
                throw new InvalidOperationException("No resultset is currently being traversed");
            return RowDescription.GetFieldIndex(name);
        }

        /// <summary>
        /// Gets a representation of the PostgreSQL data type for the specified field.
        /// The returned representation can be used to access various information about the field.
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        [PublicAPI]
        public PostgresType GetPostgresType(int ordinal) => CheckRowDescriptionAndGetField(ordinal).PostgresType;

        /// <summary>
        /// Gets the data type information for the specified field.
        /// This will be the PostgreSQL type name (e.g. double precision), not the .NET type
        /// (see <see cref="GetFieldType"/> for that).
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        public override string GetDataTypeName(int ordinal) => CheckRowDescriptionAndGetField(ordinal).TypeDisplayName;

        /// <summary>
        /// Gets the OID for the PostgreSQL type for the specified field, as it appears in the pg_type table.
        /// </summary>
        /// <remarks>
        /// This is a PostgreSQL-internal value that should not be relied upon and should only be used for
        /// debugging purposes.
        /// </remarks>
        /// <param name="ordinal">The zero-based column index.</param>
        public uint GetDataTypeOID(int ordinal) => CheckRowDescriptionAndGetField(ordinal).TypeOID;

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        public override Type GetFieldType(int ordinal)
            => Command.ObjectResultTypes?[ordinal]
               ?? CheckRowDescriptionAndGetField(ordinal).FieldType;

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The Type object that describes the data type of the specified column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            var fieldDescription = CheckRowDescriptionAndGetField(ordinal);
            return fieldDescription.Handler.GetProviderSpecificFieldType(fieldDescription);
        }

        /// <summary>
        /// Gets all provider-specific attribute columns in the collection for the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
        public override int GetProviderSpecificValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (State != ReaderState.InResult)
                throw new InvalidOperationException("No row is available");

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
                values[i] = GetProviderSpecificValue(i);
            return count;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.</returns>
        public override IEnumerator GetEnumerator()
            => new DbEnumerator(this);

        /// <summary>
        /// Returns schema information for the columns in the current resultset.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema()
            => RowDescription == null || RowDescription.Fields.Count == 0
                ? new List<NpgsqlDbColumn>().AsReadOnly()
                : new DbColumnSchemaGenerator(_connection, RowDescription, _behavior.HasFlag(CommandBehavior.KeyInfo))
                    .GetColumnSchema();

#if !NET461
        ReadOnlyCollection<DbColumn> IDbColumnSchemaGenerator.GetColumnSchema()
            => new ReadOnlyCollection<DbColumn>(GetColumnSchema().Cast<DbColumn>().ToList());
#endif

        #endregion

        #region Schema metadata table


        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the DataReader.
        /// </summary>
#nullable disable
        public override DataTable GetSchemaTable()
#nullable enable
        {
            if (FieldCount == 0) // No resultset
                return null;

            var table = new DataTable("SchemaTable");

            // Note: column order is important to match SqlClient's, some ADO.NET users appear
            // to assume ordering (see #1671)
            table.Columns.Add("ColumnName", typeof(string));
            table.Columns.Add("ColumnOrdinal", typeof(int));
            table.Columns.Add("ColumnSize", typeof(int));
            table.Columns.Add("NumericPrecision", typeof(int));
            table.Columns.Add("NumericScale", typeof(int));
            table.Columns.Add("IsUnique", typeof(bool));
            table.Columns.Add("IsKey", typeof(bool));
            table.Columns.Add("BaseServerName", typeof(string));
            table.Columns.Add("BaseCatalogName", typeof(string));
            table.Columns.Add("BaseColumnName", typeof(string));
            table.Columns.Add("BaseSchemaName", typeof(string));
            table.Columns.Add("BaseTableName", typeof(string));
            table.Columns.Add("DataType", typeof(Type));
            table.Columns.Add("AllowDBNull", typeof(bool));
            table.Columns.Add("ProviderType", typeof(int));
            table.Columns.Add("IsAliased", typeof(bool));
            table.Columns.Add("IsExpression", typeof(bool));
            table.Columns.Add("IsIdentity", typeof(bool));
            table.Columns.Add("IsAutoIncrement", typeof(bool));
            table.Columns.Add("IsRowVersion", typeof(bool));
            table.Columns.Add("IsHidden", typeof(bool));
            table.Columns.Add("IsLong", typeof(bool));
            table.Columns.Add("IsReadOnly", typeof(bool));
            table.Columns.Add("ProviderSpecificDataType", typeof(Type));
            table.Columns.Add("DataTypeName", typeof(string));

            foreach (var column in GetColumnSchema())
            {
                var row = table.NewRow();

                row["ColumnName"] = column.ColumnName;
                row["ColumnOrdinal"] = column.ColumnOrdinal ?? -1;
                row["ColumnSize"] = column.ColumnSize ?? -1;
                row["NumericPrecision"] = column.NumericPrecision ?? 0;
                row["NumericScale"] = column.NumericScale ?? 0;
                row["IsUnique"] = column.IsUnique == true;
                row["IsKey"] = column.IsKey == true;
                row["BaseServerName"] = "";
                row["BaseCatalogName"] = column.BaseCatalogName;
                row["BaseColumnName"] = column.BaseColumnName;
                row["BaseSchemaName"] = column.BaseSchemaName;
                row["BaseTableName"] = column.BaseTableName;
                row["DataType"] = column.DataType;
                row["AllowDBNull"] = (object?)column.AllowDBNull ?? DBNull.Value;
                row["ProviderType"] = column.NpgsqlDbType ?? NpgsqlDbType.Unknown;
                row["IsAliased"] = column.IsAliased == true;
                row["IsExpression"] = column.IsExpression == true;
                row["IsIdentity"] = column.IsIdentity == true;
                row["IsAutoIncrement"] = column.IsAutoIncrement == true;
                row["IsRowVersion"] = false;
                row["IsHidden"] = column.IsHidden == true;
                row["IsLong"] = column.IsLong == true;
                row["DataTypeName"] = column.DataTypeName;

                table.Rows.Add(row);
            }

            return table;
        }

        #endregion Schema metadata table

        #region Seeking

        Task SeekToColumn(int column, bool async)
        {
            if (_isSequential)
                return SeekToColumnSequential(column, async);
            SeekToColumnNonSequential(column);
            return Task.CompletedTask;
        }

        void SeekToColumnNonSequential(int column)
        {
            // Shut down any streaming going on on the column
            if (ColumnStream != null)
            {
                ColumnStream.Dispose();
                ColumnStream = null;
            }

            for (var lastColumnRead = _columns.Count; column >= lastColumnRead; lastColumnRead++)
            {
                int lastColumnLen;
                (Buffer.ReadPosition, lastColumnLen) = _columns[lastColumnRead-1];
                if (lastColumnLen != -1)
                    Buffer.ReadPosition += lastColumnLen;
                var len = Buffer.ReadInt32();
                _columns.Add((Buffer.ReadPosition, len));
            }

            (Buffer.ReadPosition, ColumnLen) = _columns[column];
            _column = column;
            PosInColumn = 0;
        }

        /// <summary>
        /// Seeks to the given column. The 4-byte length is read and stored in <see cref="ColumnLen"/>.
        /// </summary>
        async Task SeekToColumnSequential(int column, bool async)
        {
            if (column < 0 || column >= _numColumns)
                throw new IndexOutOfRangeException("Column index out of range");

            if (column < _column)
                throw new InvalidOperationException($"Invalid attempt to read from column ordinal '{column}'. With CommandBehavior.SequentialAccess, you may only read from column ordinal '{_column}' or greater.");

            if (column == _column)
                return;

            // Need to seek forward

            // Shut down any streaming going on on the column
            if (ColumnStream != null)
            {
                ColumnStream.Dispose();
                ColumnStream = null;
                // Disposing the stream leaves us at the end of the column
                PosInColumn = ColumnLen;
            }

            // Skip to end of column if needed
            // TODO: Simplify by better initializing _columnLen/_posInColumn
            var remainingInColumn = ColumnLen == -1 ? 0 : ColumnLen - PosInColumn;
            if (remainingInColumn > 0)
                await Buffer.Skip(remainingInColumn, async);

            // Skip over unwanted fields
            for (; _column < column - 1; _column++)
            {
                await Buffer.Ensure(4, async);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.Skip(len, async);
            }

            await Buffer.Ensure(4, async);
            ColumnLen = Buffer.ReadInt32();
            PosInColumn = 0;
            _column = column;
        }

        Task SeekInColumn(int posInColumn, bool async)
        {
            if (_isSequential)
                return SeekInColumnSequential(posInColumn, async);

            if (posInColumn > ColumnLen)
                posInColumn = ColumnLen;

            Buffer.ReadPosition = _columns[_column].Offset + posInColumn;
            PosInColumn = posInColumn;
            return Task.CompletedTask;

            async Task SeekInColumnSequential(int posInColumn2, bool async2)
            {
                Debug.Assert(_column > -1);

                if (posInColumn2 < PosInColumn)
                    throw new InvalidOperationException("Attempt to read a position in the column which has already been read");

                if (posInColumn2 > ColumnLen)
                    posInColumn2 = ColumnLen;

                if (posInColumn2 > PosInColumn)
                {
                    await Buffer.Skip(posInColumn2 - PosInColumn, async2);
                    PosInColumn = posInColumn2;
                }
            }
        }

        #endregion

        #region ConsumeRow

        Task ConsumeRow(bool async)
        {
            Debug.Assert(State == ReaderState.InResult || State == ReaderState.BeforeResult);

            if (_isSequential)
                return ConsumeRowSequential(async);
            else
            {
                ConsumeRowNonSequential();
                return Task.CompletedTask;
            }

            async Task ConsumeRowSequential(bool async2)
            {
                if (ColumnStream != null)
                {
                    ColumnStream.Dispose();
                    ColumnStream = null;
                    // Disposing the stream leaves us at the end of the column
                    PosInColumn = ColumnLen;
                }

                // TODO: Potential for code-sharing with ReadColumn above, which also skips
                // Skip to end of column if needed
                var remainingInColumn = ColumnLen == -1 ? 0 : ColumnLen - PosInColumn;
                if (remainingInColumn > 0)
                    await Buffer.Skip(remainingInColumn, async2);

                // Skip over the remaining columns in the row
                for (; _column < _numColumns - 1; _column++)
                {
                    await Buffer.Ensure(4, async2);
                    var len = Buffer.ReadInt32();
                    if (len != -1)
                        await Buffer.Skip(len, async2);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ConsumeRowNonSequential()
        {
            Debug.Assert(State == ReaderState.InResult || State == ReaderState.BeforeResult);

            if (ColumnStream != null)
            {
                ColumnStream.Dispose();
                ColumnStream = null;
                // Disposing the stream leaves us at the end of the column
                PosInColumn = ColumnLen;
            }
            Buffer.ReadPosition = _dataMsgEnd;
        }

        #endregion

        #region Checks

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        RowDescriptionMessage CheckResultSet()
        {
            switch (State)
            {
            case ReaderState.BeforeResult:
            case ReaderState.InResult:
                break;
            case ReaderState.Closed:
                throw new InvalidOperationException("The reader is closed");
            default:
                throw new InvalidOperationException("No resultset is currently being traversed");
            }

            return RowDescription!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        FieldDescription CheckResultSetAndGetField(int column)
        {
            var rowDescription = CheckResultSet();

            if (column < 0 || column >= rowDescription.NumFields)
                throw new IndexOutOfRangeException($"Column must be between {0} and {rowDescription.NumFields - 1}");

            return rowDescription[column];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        FieldDescription CheckRowAndGetField(int column)
        {
            switch (State)
            {
            case ReaderState.InResult:
                break;
            case ReaderState.Closed:
                throw new InvalidOperationException("The reader is closed");
            default:
                throw new InvalidOperationException("No row is available");
            }

            if (column < 0 || column >= RowDescription!.NumFields)
                throw new IndexOutOfRangeException($"Column must be between {0} and {RowDescription!.NumFields - 1}");

            return RowDescription[column];
        }

        /// <summary>
        /// Checks that we have a RowDescription, but not necessary an actual resultset
        /// (for operations which work in SchemaOnly mode.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        FieldDescription CheckRowDescriptionAndGetField(int column)
        {
            if (RowDescription == null)
                throw new InvalidOperationException("No resultset is currently being traversed");

            if (column < 0 || column >= RowDescription.NumFields)
                throw new IndexOutOfRangeException($"Column must be between {0} and {RowDescription.NumFields - 1}");

            return RowDescription[column];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckColumnStart()
        {
            Debug.Assert(_isSequential);
            if (PosInColumn != 0)
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckClosed()
        {
            if (State == ReaderState.Closed)
                throw new InvalidOperationException("The reader is closed");
        }

        #endregion
    }

    enum ReaderState
    {
        BeforeResult,
        InResult,
        BetweenResults,
        Consumed,
        Closed,
    }
}
