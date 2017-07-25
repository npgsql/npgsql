#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Reads a forward-only stream of rows from a data source.
    /// </summary>
#pragma warning disable CA1010
    public sealed class NpgsqlDataReader : DbDataReader
#pragma warning restore CA1010
#if NETSTANDARD1_3
        , IDbColumnSchemaGenerator
#endif
    {
        internal NpgsqlCommand Command { get; }
        readonly NpgsqlConnector _connector;
        readonly NpgsqlConnection _connection;
        readonly CommandBehavior _behavior;
        readonly Task _sendTask;

        ReaderState _state;

        /// <summary>
        /// Holds the list of statements being executed by this reader.
        /// </summary>
        readonly List<NpgsqlStatement> _statements;

        /// <summary>
        /// The index of the current query resultset we're processing (within a multiquery)
        /// </summary>
        int _statementIndex;

        /// <summary>
        /// The RowDescription message for the current resultset being processed
        /// </summary>
        RowDescriptionMessage _rowDescription;

        [CanBeNull]
        DataRowMessage _row;
        uint? _recordsAffected;

        /// <summary>
        /// Indicates that at least one row has been read across all result sets
        /// </summary>
        bool _readOneRow;

        /// <summary>
        /// Whether the current result set has rows
        /// </summary>
        bool? _hasRows;

        /// <summary>
        /// If HasRows was called before any rows were read, it was forced to read messages. A pending
        /// message may be stored here for processing in the next Read() or NextResult().
        /// </summary>
        [CanBeNull]
        IBackendMessage _pendingMessage;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        // static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        bool IsSequential => (_behavior & CommandBehavior.SequentialAccess) != 0;
        bool IsSchemaOnly => (_behavior & CommandBehavior.SchemaOnly) != 0;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior, List<NpgsqlStatement> statements, Task sendTask)
        {
            Command = command;
            _connection = command.Connection;
            _connector = _connection.Connector;
            _behavior = behavior;
            _statements = statements;
            _statementIndex = -1;
            _sendTask = sendTask;
            _state = ReaderState.BetweenResults;
        }

        /// <summary>
        /// The first row in a stored procedure command that has output parameters needs to be traversed twice -
        /// once for populating the output parameters and once for the actual result set traversal. So in this
        /// case we can't be sequential.
        /// </summary>
        void PopulateOutputParameters()
        {
            Debug.Assert(Command.Parameters.Any(p => p.IsOutputDirection));
            Debug.Assert(_statementIndex == 0);
            Debug.Assert(_pendingMessage != null);
            Debug.Assert(_rowDescription != null);

            var asDataRow = _pendingMessage as DataRowMessage;
            if (asDataRow == null) // The first resultset was empty
                return;
            Debug.Assert(asDataRow is DataRowNonSequentialMessage);
            Debug.Assert(asDataRow.NumColumns == _rowDescription.NumFields);

            // Temporarily set _row to the pending data row in order to retrieve the values
            _row = asDataRow;

            var pending = new Queue<NpgsqlParameter>();
            var taken = new List<int>();
            foreach (var p in Command.Parameters.Where(p => p.IsOutputDirection))
            {
                if (_rowDescription.TryGetFieldIndex(p.CleanName, out var idx))
                {
                    // TODO: Provider-specific check?
                    p.Value = GetValue(idx);
                    taken.Add(idx);
                }
                else
                    pending.Enqueue(p);
            }
            for (var i = 0; pending.Count != 0 && i != _row.NumColumns; ++i)
            {
                // TODO: Need to get the provider-specific value based on the out param's type
                if (!taken.Contains(i))
                    pending.Dequeue().Value = GetValue(i);
            }

            _row = null;
        }

        #region Read

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns><b>true</b> if there are more rows; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// The default position of a data reader is before the first record. Therefore, you must call Read to begin accessing data.
        /// </remarks>
        public override bool Read() => Read(false).GetAwaiter().GetResult();

        /// <summary>
        /// This is the asynchronous version of <see cref="Read()"/> The cancellation token is currently ignored.
        /// </summary>
        /// <param name="cancellationToken">Ignored for now.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
            => SynchronizationContextSwitcher.NoContext(async () => await Read(true));

        async Task<bool> Read(bool async)
        {
            if (_row != null) {
                await _row.Consume(async );
                _row = null;
            }

            switch (_state)
            {
            case ReaderState.InResult:
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
                if ((_behavior & CommandBehavior.SingleRow) != 0 && _readOneRow)
                {
                    // TODO: See optimization proposal in #410
                    await Consume(async);
                    return false;
                }

                while (true)
                {
                    var msg = await ReadMessage(async);
                    switch (ProcessMessage(msg))
                    {
                    case ReadResult.RowRead:
                        return true;
                    case ReadResult.RowNotRead:
                        return false;
                    case ReadResult.ReadAgain:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (PostgresException)
            {
                _state = ReaderState.Consumed;
                throw;
            }
        }

        ReadResult ProcessMessage(IBackendMessage msg)
        {
            Debug.Assert(msg != null);

            switch (msg.Code)
            {
            case BackendMessageCode.DataRow:
                Debug.Assert(_rowDescription != null);
                _connector.State = ConnectorState.Fetching;
                _row = (DataRowMessage)msg;
                Debug.Assert(_rowDescription.NumFields == _row.NumColumns);
                _readOneRow = true;
                _hasRows = true;
                return ReadResult.RowRead;

            case BackendMessageCode.CompletedResponse:
                var completed = (CommandCompleteMessage) msg;
                switch (completed.StatementType)
                {
                case StatementType.Update:
                case StatementType.Insert:
                case StatementType.Delete:
                case StatementType.Copy:
                case StatementType.Move:
                    if (!_recordsAffected.HasValue) {
                        _recordsAffected = 0;
                    }
                    _recordsAffected += completed.Rows;
                    break;
                }

                _statements[_statementIndex].ApplyCommandComplete(completed);

                goto case BackendMessageCode.EmptyQueryResponse;

            case BackendMessageCode.EmptyQueryResponse:
                _state = ReaderState.BetweenResults;
                return ReadResult.RowNotRead;

            case BackendMessageCode.ReadyForQuery:
                _state = ReaderState.Consumed;
                return ReadResult.RowNotRead;

            case BackendMessageCode.BindComplete:
            case BackendMessageCode.CloseComplete:
                return ReadResult.ReadAgain;

            default:
                throw new Exception("Received unexpected backend message of type " + msg.Code);
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
                return (IsSchemaOnly ? NextResultSchemaOnly(false) : NextResult(false))
                    .GetAwaiter().GetResult();
            }
            catch (PostgresException e)
            {
                _state = ReaderState.Consumed;
                if ((_statementIndex >= 0) && (_statementIndex < _statements.Count))
                    e.Statement = _statements[_statementIndex];
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
            => SynchronizationContextSwitcher.NoContext(async () =>
            {
                try
                {
                    return IsSchemaOnly ? await NextResultSchemaOnly(true) : await NextResult(true);
                }
                catch (PostgresException e)
                {
                    _state = ReaderState.Consumed;
                    if (_statementIndex >= 0 && _statementIndex < _statements.Count)
                        e.Statement = _statements[_statementIndex];
                    throw;
                }
            });

        async Task<bool> NextResult(bool async)
        {
            Debug.Assert(!IsSchemaOnly);

            // If we're in the middle of a resultset, consume it
            switch (_state)
            {
            case ReaderState.InResult:
                if (_row != null) {
                    await _row.Consume(async);
                    _row = null;
                }

                // TODO: Duplication with SingleResult handling above
                var completedMsg = await SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.EmptyQueryResponse, async);
                ProcessMessage(completedMsg);
                break;

            case ReaderState.BetweenResults:
                break;

            case ReaderState.Consumed:
            case ReaderState.Closed:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
            }

            Debug.Assert(_state == ReaderState.BetweenResults);
            _hasRows = null;

            if ((_behavior & CommandBehavior.SingleResult) != 0 && _statementIndex == 0)
            {
                if (_state == ReaderState.BetweenResults)
                    await Consume(async);
                return false;
            }

            // We are now at the end of the previous result set. Read up to the next result set, if any.
            // Non-prepared statements receive ParseComplete, BindComplete, DescriptionRow/NoData,
            // prepared statements receive only BindComplete
            for (_statementIndex++; _statementIndex < _statements.Count; _statementIndex++)
            {
                var statement = _statements[_statementIndex];
                if (statement.IsPrepared)
                {
                    await _connector.ReadExpecting<BindCompleteMessage>(async);
                    _rowDescription = statement.Description;
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
                            await _connector.ReadExpecting<CloseCompletedMessage>(async);
                            pStatement.StatementBeingReplaced.CompleteUnprepare();
                            pStatement.StatementBeingReplaced = null;
                        }
                    }

                    await _connector.ReadExpecting<ParseCompleteMessage>(async);
                    await _connector.ReadExpecting<BindCompleteMessage>(async);
                    var msg = await _connector.ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.NoData:
                        _rowDescription = statement.Description = null;
                        break;
                    case BackendMessageCode.RowDescription:
                        // We have a resultset
                        _rowDescription = statement.Description = (RowDescriptionMessage)msg;
                        break;
                    default:
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    }

                    if (pStatement != null)
                    {
                        Debug.Assert(!pStatement.IsPrepared);
                        pStatement.CompletePrepare();
                    }
                }

                if (_rowDescription == null)
                {
                    // Statement did not generate a resultset (e.g. INSERT)
                    // Read and process its completion message and move on to the next
                    var msg = await _connector.ReadMessage(async);
                    if (msg.Code != BackendMessageCode.CompletedResponse && msg.Code != BackendMessageCode.EmptyQueryResponse)
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    ProcessMessage(msg);
                    continue;
                }

                // We got a new resultset.

                // Read the next message and store it in _pendingRow, this is to make sure that if the
                // statement generated an error, it gets thrown here and not on the first call to Read().

                if (_statementIndex == 0 && Command.Parameters.HasOutputParameters)
                {
                    // If output parameters are present and this is the first row of the first resultset,
                    // we must read it in non-sequential mode because it will be traversed twice (once
                    // here for the parameters, then as a regular row).
                    _pendingMessage = await _connector.ReadMessage(async);
                    PopulateOutputParameters();
                }
                else
                    _pendingMessage = await _connector.ReadMessage(async, IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential);

                _state = ReaderState.InResult;
                return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            ProcessMessage(_connector.ReadExpecting<ReadyForQueryMessage>());
            _rowDescription = null;
            return false;
        }

        /// <summary>
        /// Note that in SchemaOnly mode there are no resultsets, and we read nothing from the backend (all
        /// RowDescriptions have already been processed and are available)
        /// </summary>
        async Task<bool> NextResultSchemaOnly(bool async)
        {
            Debug.Assert(IsSchemaOnly);

            for (_statementIndex++; _statementIndex < _statements.Count; _statementIndex++)
            {
                var statement = _statements[_statementIndex];
                if (statement.IsPrepared)
                {
                    // Row descriptions have already been populated in the statement objects at the
                    // Prepare phase
                    _rowDescription = _statements[_statementIndex].Description;
                }
                else
                {
                    await _connector.ReadExpecting<ParseCompleteMessage>(async);
                    await _connector.ReadExpecting<ParameterDescriptionMessage>(async);
                    var msg = await _connector.ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.NoData:
                        _rowDescription = _statements[_statementIndex].Description = null;
                        break;
                    case BackendMessageCode.RowDescription:
                        // We have a resultset
                        _rowDescription = _statements[_statementIndex].Description = (RowDescriptionMessage)msg;
                        Command.FixupRowDescription(_rowDescription, _statementIndex == 0);
                        break;
                    default:
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                // Found a resultset
                if (_rowDescription != null)
                    return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            if (!_statements.All(s => s.IsPrepared))
            {
                ProcessMessage(await _connector.ReadExpecting<ReadyForQueryMessage>(async));
                _rowDescription = null;
            }
            return false;
        }

        #endregion

        ValueTask<IBackendMessage> ReadMessage(bool async)
        {
            if (_pendingMessage != null) {
                var msg = _pendingMessage;
                _pendingMessage = null;
                return new ValueTask<IBackendMessage>(msg);
            }
            return _connector.ReadMessage(async, IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential);
        }

        async ValueTask<IBackendMessage> SkipUntil(BackendMessageCode stopAt1, BackendMessageCode stopAt2, bool async)
        {
            if (_pendingMessage != null) {
                if (_pendingMessage.Code == stopAt1 || _pendingMessage.Code == stopAt2) {
                    var msg = _pendingMessage;
                    _pendingMessage = null;
                    return msg;
                }
                var asDataRow = _pendingMessage as DataRowMessage;
                // ReSharper disable once UseNullPropagation
                if (asDataRow != null)
                    await asDataRow.Consume(async);
                _pendingMessage = null;
            }
            return await _connector.SkipUntil(stopAt1, stopAt2, async);
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.  Always returns zero.
        /// </summary>
        public override int Depth => 0;

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed => _state == ReaderState.Closed;

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
        public override bool HasRows
        {
            get
            {
                if (_hasRows.HasValue)
                    return _hasRows.Value;
                if (_statementIndex >= _statements.Count)
                    return false;
                while (true)
                {
                    var msg = ReadMessage(false).Result;
                    switch (msg.Code)
                    {
                    case BackendMessageCode.BindComplete:
                    case BackendMessageCode.RowDescription:
                        ProcessMessage(msg);
                        continue;
                    case BackendMessageCode.DataRow:
                        _pendingMessage = msg;
                        _hasRows = true;
                        return true;
                    case BackendMessageCode.CompletedResponse:
                    case BackendMessageCode.EmptyQueryResponse:
                    case BackendMessageCode.ReadyForQuery:
                        _pendingMessage = msg;
                        _hasRows = false;
                        return false;
                    case BackendMessageCode.CloseComplete:
                        _hasRows = false;
                        return false;
                    default:
                        throw new InvalidOperationException("Got unexpected message type: " + msg.Code);
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the reader is currently positioned on a row, i.e. whether reading a
        /// column is possible.
        /// This property is different from <see cref="HasRows"/> in that <see cref="HasRows"/> will
        /// return true even if attempting to read a column will fail, e.g. before <see cref="Read()"/>
        /// has been called
        /// </summary>
        [PublicAPI]
        public bool IsOnRow => _row != null;

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);

            return _rowDescription[ordinal].Name;
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override int FieldCount => _rowDescription?.NumFields ?? 0;

        #region Cleanup / Dispose

        /// <summary>
        /// Consumes all result sets for this reader, leaving the connector ready for sending and processing further
        /// queries
        /// </summary>
        async Task Consume(bool async)
        {
            if (IsSchemaOnly && _statements.All(s => s.IsPrepared))
            {
                _state = ReaderState.Consumed;
                return;
            }

            if (_row != null)
            {
                await _row.Consume(async);
                _row = null;
            }

            // Skip over the other result sets, processing only CommandCompleted for RecordsAffected
            while (true)
            {
                var msg = await SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.ReadyForQuery, async);
                switch (msg.Code)
                {
                case BackendMessageCode.CompletedResponse:
                    ProcessMessage(msg);
                    continue;
                case BackendMessageCode.ReadyForQuery:
                    ProcessMessage(msg);
                    return;
                default:
                    throw new NpgsqlException("Unexpected message of type " + msg.Code);
                }
            }
        }

        /// <summary>
        /// Releases the resources used by the <see cref="NpgsqlDataReader">NpgsqlDataReader</see>.
        /// </summary>
        protected override void Dispose(bool disposing) => Close();

        /// <summary>
        /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
        /// </summary>
#if NETSTANDARD1_3
        public void Close()
#else
        public override void Close()
#endif
            => Close(false, false).GetAwaiter().GetResult();

        /// <summary>
        /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
        /// </summary>
        public Task CloseAsync() => Close(false, true);

        internal async Task Close(bool connectionClosing, bool async)
        {
            if (_state == ReaderState.Closed)
                return;

            switch (_connector.State)
            {
            case ConnectorState.Broken:
            case ConnectorState.Closed:
                // This may have happen because an I/O error while reading a value, or some non-safe
                // exception thrown from a type handler. Or if the connection was closed while the reader
                // was still open
                _state = ReaderState.Closed;
                Command.State = CommandState.Idle;
                ReaderClosed?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_state != ReaderState.Consumed)
                await Consume(async);

            await Cleanup(async, connectionClosing);
        }

        internal async Task Cleanup(bool async, bool connectionClosing=false)
        {
            Log.Trace("Cleaning up reader", _connector.Id);

            // Make sure the send task for this command, which may have executed asynchronously and in
            // parallel with the reading, has completed, throwing any exceptions it generated.
            if (async)
                await _sendTask;
            else
                _sendTask.GetAwaiter().GetResult();

            _state = ReaderState.Closed;
            Command.State = CommandState.Idle;
            _connector.CurrentReader = null;
            _connector.EndUserAction();

            // If the reader is being closed as part of the connection closing, we don't apply
            // the reader's CommandBehavior.CloseConnection
            if ((_behavior & CommandBehavior.CloseConnection) != 0 && !connectionClosing)
                _connection.Close();

            if (ReaderClosed != null)
            {
                ReaderClosed(this, EventArgs.Empty);
                ReaderClosed = null;
            }
        }

#endregion

        /// <summary>
        /// Returns the current row, or throws an exception if a row isn't available
        /// </summary>
        DataRowMessage Row
        {
            get
            {
                if (_row == null) {
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                }
                return _row;
            }
        }

        #region Simple value getters

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override bool GetBoolean(int ordinal) => ReadColumn<bool>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal) => ReadColumn<byte>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a single character.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal) => ReadColumn<char>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal) => ReadColumn<short>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal) => ReadColumn<int>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal) => ReadColumn<long>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override DateTime GetDateTime(int ordinal) => ReadColumn<DateTime>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="string"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal) => ReadColumn<string>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a <see cref="decimal"/> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal) => ReadColumn<decimal>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a double-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal) => ReadColumn<double>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a single-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal) => ReadColumn<float>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as a globally-unique identifier (GUID).
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal) => ReadColumn<Guid>(ordinal);

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
        public override int GetValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckRow();

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

        #region Provider-specific type getters

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlDate"/>,
        /// Npgsql's provider-specific type for dates.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's date type represents dates from 4713 BC to 5874897 AD, while .NET's DateTime
        /// only supports years from 1 to 1999. If you require years outside this range use this accessor.
        /// The standard <see cref="GetProviderSpecificValue"/> method will also return this type, but has
        /// the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlDate GetDate(int ordinal) => ReadColumn<NpgsqlDate>(ordinal);

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
        public TimeSpan GetTimeSpan(int ordinal) => ReadColumn<TimeSpan>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlTimeSpan"/>,
        /// Npgsql's provider-specific type for time spans.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's interval type has has a resolution of 1 microsecond and ranges from
        /// -178000000 to 178000000 years, while .NET's TimeSpan has a resolution of 100 nanoseconds
        /// and ranges from roughly -29247 to 29247 years. If you require values from outside TimeSpan's
        /// range use this accessor.
        /// The standard ADO.NET <see cref="GetProviderSpecificValue"/> method will also return this
        /// type, but has the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlTimeSpan GetInterval(int ordinal) => ReadColumn<NpgsqlTimeSpan>(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlDateTime"/>,
        /// Npgsql's provider-specific type for date/time timestamps. Note that this type covers
        /// both PostgreSQL's "timestamp with time zone" and "timestamp without time zone" types,
        /// which differ only in how they are converted upon input/output.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's timestamp type represents dates from 4713 BC to 5874897 AD, while .NET's DateTime
        /// only supports years from 1 to 1999. If you require years outside this range use this accessor.
        /// The standard <see cref="GetProviderSpecificValue"/> method will also return this type, but has
        /// the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlDateTime GetTimeStamp(int ordinal) => ReadColumn<NpgsqlDateTime>(ordinal);

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
        public override long GetBytes(int ordinal, long dataOffset, [CanBeNull] byte[] buffer, int bufferOffset, int length)
        {
            CheckRowAndOrdinal(ordinal);
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length - 1)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler ??
                (fieldDescription.Handler as PostgisGeometryHandler)?.ByteaHandler;
            if (handler == null)
                throw new InvalidCastException("GetBytes() not supported for type " + fieldDescription.Name);

            var row = Row;
            row.SeekToColumn(ordinal, false).GetAwaiter().GetResult();
            row.CheckNotNull();
            return handler.GetBytes(row, (int)dataOffset, buffer, bufferOffset, length, fieldDescription);
        }

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override Stream GetStream(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null)
                throw new InvalidCastException("GetStream() not supported for type " + fieldDescription.Handler.PgDisplayName);

            var row = Row;
            row.SeekToColumnStart(ordinal, false).GetAwaiter().GetResult();
            row.CheckNotNull();
            return row.GetStream();
        }

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public Task<Stream> GetStreamAsync(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null)
                throw new InvalidCastException("GetStream() not supported for type " + fieldDescription.Handler.PgDisplayName);

            return SynchronizationContextSwitcher.NoContext(async () =>
            {
                var row = Row;
                await row.SeekToColumnStart(ordinal, false);
                row.CheckNotNull();
                return row.GetStream();
            });
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
        public override long GetChars(int ordinal, long dataOffset, [CanBeNull] char[] buffer, int bufferOffset, int length)
        {
            CheckRowAndOrdinal(ordinal);
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length - 1)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null)
                throw new InvalidCastException("GetChars() not supported for type " + fieldDescription.Name);

            var row = Row;
            row.SeekToColumn(ordinal, false).GetAwaiter().GetResult(); ;
            row.CheckNotNull();
            return handler.GetChars(row, (int)dataOffset, buffer, bufferOffset, length, fieldDescription);
        }

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override TextReader GetTextReader(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ITextReaderHandler;
            if (handler == null)
                throw new InvalidCastException("GetTextReader() not supported for type " + fieldDescription.Handler.PgDisplayName);

            var row = Row;
            row.SeekToColumnStart(ordinal, false).GetAwaiter().GetResult();
            row.CheckNotNull();

            return handler.GetTextReader(row.GetStream());
        }

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public Task<TextReader> GetTextReaderAsync(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ITextReaderHandler;
            if (handler == null)
                throw new InvalidCastException("GetTextReader() not supported for type " + fieldDescription.Handler.PgDisplayName);

            return SynchronizationContextSwitcher.NoContext(async () =>
            {
                var row = Row;
                await row.SeekToColumnStart(ordinal, false);
                row.CheckNotNull();
                return handler.GetTextReader(row.GetStream());
            });
        }

        #endregion

        #region IsDBNull

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public override bool IsDBNull(int ordinal) => IsDBNull(ordinal, false).GetAwaiter().GetResult();

        /// <summary>
        /// An asynchronous version of <see cref="IsDBNull(int)"/>, which gets a value that indicates whether the column contains non-existent or missing values.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns><b>true</b> if the specified column value is equivalent to <see cref="DBNull"/> otherwise <b>false</b>.</returns>
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
            => SynchronizationContextSwitcher.NoContext(async () => await IsDBNull(ordinal, true));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        async Task<bool> IsDBNull(int ordinal, bool async)
        {
            CheckRowAndOrdinal(ordinal);

            await Row.SeekToColumn(ordinal, async);
            return Row.IsColumnNull;
        }

        #endregion

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[string name] => GetValue(GetOrdinal(name));

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name)
        {
            CheckResultSet();
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty", nameof(name));

            return _rowDescription.GetFieldIndex(name);
        }

        /// <summary>
        /// Gets a representation of the PostgreSQL data type for the specified field.
        /// The returned representation can be used to access various information about the field.
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        [PublicAPI]
        public PostgresType GetPostgresType(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);

            return _rowDescription[ordinal].PostgresType;
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// This will be the PostgreSQL type name (e.g. int4) as in the pg_type table,
        /// not the .NET type (see <see cref="GetFieldType"/> for that).
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        /// <returns></returns>
        public override string GetDataTypeName(int ordinal) => GetPostgresType(ordinal).DisplayName;

        /// <summary>
        /// Gets the OID for the PostgreSQL type for the specified field, as it appears in the pg_type table.
        /// </summary>
        /// <remarks>
        /// This is a PostgreSQL-internal value that should not be relied upon and should only be used for
        /// debugging purposes.
        /// </remarks>
        /// <param name="ordinal">The zero-based column index.</param>
        public uint GetDataTypeOID(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);

            return _rowDescription[ordinal].TypeOID;
        }

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        [NotNull]
        public override Type GetFieldType(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);

            var type = Command.ObjectResultTypes?[ordinal];
            return type ?? _rowDescription[ordinal].FieldType;
        }

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The Type object that describes the data type of the specified column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);

            var fieldDescription = _rowDescription[ordinal];
            return fieldDescription.Handler.GetProviderSpecificFieldType(fieldDescription);
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            // TODO: Code duplication with ReadColumn<T>
            _row.SeekToColumnStart(ordinal, false).GetAwaiter().GetResult();
            if (_row.IsColumnNull)
                return DBNull.Value;
            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            object result;
            try {
                result = handler.ReadAsObject(_row, fieldDescription);
            } catch (SafeReadException e) {
                throw e.InnerException;
            } catch {
                _connector.Break();
                throw;
            }

            // Used for Entity Framework <= 6 compability
            if (Command.ObjectResultTypes?[ordinal] != null)
            {
                var type = Command.ObjectResultTypes[ordinal];
                result = type == typeof(DateTimeOffset)
                    ? new DateTimeOffset((DateTime)result)
                    : Convert.ChangeType(result, type);
            }

            return result;
        }

        /// <summary>
        /// Synchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <returns>The column to be retrieved.</returns>
        public override T GetFieldValue<T>(int ordinal) => GetFieldValue<T>(ordinal, false).Result;

        /// <summary>
        /// Asynchronously gets the value of the specified column as a type.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
            => SynchronizationContextSwitcher.NoContext(async () => await GetFieldValue<T>(ordinal, true));

        async ValueTask<T> GetFieldValue<T>(int ordinal, bool async)
        {
            CheckRowAndOrdinal(ordinal);

            var t = typeof(T);
            if (!t.IsArray) {
                if (t == typeof(object))
                    return (T)GetValue(ordinal);  // TODO: Sync...
                return await ReadColumn<T>(ordinal, async);
            }

            // Getting an array

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            // If the type handler can simply return the requested array, call it as usual. This is the case
            // of reading a string as char[], a bytea as a byte[]...
            var tHandler = handler as ITypeHandler<T>;
            if (tHandler != null)
                return await ReadColumn<T>(ordinal, async);

            // We need to treat this as an actual array type, these need special treatment because of
            // typing/generics reasons
            var elementType = t.GetElementType();
            var arrayHandler = handler as ArrayHandler;
            if (arrayHandler == null)
                throw new InvalidCastException($"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(T).Name}");

            if (arrayHandler.GetElementFieldType(fieldDescription) == elementType)
                return (T)GetValue(ordinal);
            if (arrayHandler.GetElementPsvType(fieldDescription) == elementType)
                return (T)GetProviderSpecificValue(ordinal);
            throw new InvalidCastException($"Can't cast database type {handler.PgDisplayName} to {typeof(T).Name}");
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetProviderSpecificValue(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            // TODO: Code duplication with ReadColumn<T>
            _row.SeekToColumnStart(ordinal, false).GetAwaiter().GetResult();
            if (_row.IsColumnNull)
                return DBNull.Value;
            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            object result;
            try {
                result = handler.ReadPsvAsObject(_row, fieldDescription);
            } catch (SafeReadException e) {
                throw e.InnerException;
            } catch {
                _connector.Break();
                throw;
            }

            return result;
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
            CheckRow();

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
        {
#if NETSTANDARD1_3
            throw new NotSupportedException("GetEnumerator not yet supported in .NET Core");
#else
            return new DbEnumerator(this);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async ValueTask<T> ReadColumn<T>(int ordinal, bool async)
        {
            CheckRowAndOrdinal(ordinal);

            await _row.SeekToColumnStart(ordinal, false);
            Row.CheckNotNull();
            var fieldDescription = _rowDescription[ordinal];
            try
            {
                return await fieldDescription.Handler.Read<T>(_row, Row.ColumnLen, async, fieldDescription);
            }
            catch (SafeReadException e)
            {
                throw e.InnerException;
            }
            catch
            {
                _connector.Break();
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T ReadColumn<T>(int ordinal) => ReadColumn<T>(ordinal, false).Result;

        #region New (CoreCLR) schema API

        /// <summary>
        /// Returns schema information for the columns in the current resultset.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema()
            => new DbColumnSchemaGenerator(_connection, _rowDescription, (_behavior & CommandBehavior.KeyInfo) != 0)
                .GetColumnSchema();

#if NETSTANDARD1_3
        ReadOnlyCollection<DbColumn> IDbColumnSchemaGenerator.GetColumnSchema()
            => new ReadOnlyCollection<DbColumn>(GetColumnSchema().Cast<DbColumn>().ToList());
#endif

        #endregion

        #region Schema metadata table
#if !NETSTANDARD1_3

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the DataReader.
        /// </summary>
        [CanBeNull]
        public override DataTable GetSchemaTable()
        {
            if (FieldCount == 0) // No resultset
                return null;

            var table = new DataTable("SchemaTable");

            table.Columns.Add("AllowDBNull", typeof(bool));
            table.Columns.Add("BaseCatalogName", typeof(string));
            table.Columns.Add("BaseColumnName", typeof(string));
            table.Columns.Add("BaseSchemaName", typeof(string));
            table.Columns.Add("BaseTableName", typeof(string));
            table.Columns.Add("ColumnName", typeof(string));
            table.Columns.Add("ColumnOrdinal", typeof(int));
            table.Columns.Add("ColumnSize", typeof(int));
            table.Columns.Add("DataType", typeof(Type));
            table.Columns.Add("IsUnique", typeof(bool));
            table.Columns.Add("IsKey", typeof(bool));
            table.Columns.Add("IsAliased", typeof(bool));
            table.Columns.Add("IsExpression", typeof(bool));
            table.Columns.Add("IsIdentity", typeof(bool));
            table.Columns.Add("IsAutoIncrement", typeof(bool));
            table.Columns.Add("IsRowVersion", typeof(bool));
            table.Columns.Add("IsHidden", typeof(bool));
            table.Columns.Add("IsLong", typeof(bool));
            table.Columns.Add("IsReadOnly", typeof(bool));
            table.Columns.Add("NumericPrecision", typeof(int));
            table.Columns.Add("NumericScale", typeof(int));
            table.Columns.Add("ProviderSpecificDataType", typeof(Type));
            table.Columns.Add("ProviderType", typeof(Type));

            foreach (var column in GetColumnSchema())
            {
                var row = table.NewRow();

                row["AllowDBNull"] = (object)column.AllowDBNull ?? DBNull.Value;
                row["BaseColumnName"] = column.BaseColumnName;
                row["BaseCatalogName"] = column.BaseCatalogName;
                row["BaseSchemaName"] = column.BaseSchemaName;
                row["BaseTableName"] = column.BaseTableName;
                row["ColumnName"] = column.ColumnName;
                row["ColumnOrdinal"] = column.ColumnOrdinal ?? -1;
                row["ColumnSize"] = column.ColumnSize ?? -1;
                row["DataType"] = row["ProviderType"] = column.DataType; // Non-standard
                row["IsUnique"] = column.IsUnique == true;
                row["IsKey"] = column.IsKey == true;
                row["IsAliased"] = column.IsAliased == true;
                row["IsExpression"] = column.IsExpression == true;
                row["IsAutoIncrement"] = column.IsAutoIncrement == true;
                row["IsIdentity"] = column.IsIdentity == true;
                row["IsRowVersion"] = false;
                row["IsHidden"] = column.IsHidden == true;
                row["IsLong"] = column.IsLong == true;
                row["NumericPrecision"] = column.NumericPrecision ?? 0;
                row["NumericScale"] = column.NumericScale ?? 0;

                table.Rows.Add(row);
            }

            return table;
        }

#endif
        #endregion Schema metadata table

        #region Checks

        void CheckRowAndOrdinal(int ordinal)
        {
            CheckRow();
            CheckOrdinal(ordinal);
        }

        void CheckRow()
        {
            if (!IsOnRow)
                throw new InvalidOperationException("No row is available");
        }

        // ReSharper disable once UnusedParameter.Local
        void CheckOrdinal(int ordinal)
        {
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException($"Column must be between {0} and {(FieldCount - 1)}");
        }

        void CheckResultSet()
        {
            if (FieldCount == 0)
                throw new InvalidOperationException("No resultset is currently being traversed");
        }

        #endregion

        #region Enums

        enum ReaderState
        {
            InResult,
            BetweenResults,
            Consumed,
            Closed,
        }

        enum ReadResult
        {
            RowRead,
            RowNotRead,
            ReadAgain,
        }

        #endregion
    }
}
