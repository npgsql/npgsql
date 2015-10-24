#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.NumericHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Reads a forward-only stream of rows from a data source.
    /// </summary>
    public partial class NpgsqlDataReader : DbDataReader
    {
        internal NpgsqlCommand Command { get; }
        readonly NpgsqlConnector _connector;
        readonly NpgsqlConnection _connection;
        readonly CommandBehavior _behavior;

        ReaderState State { get; set; }

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
        IBackendMessage _pendingMessage;

#if NET45 || NET452 || DNX452
        /// <summary>
        /// If <see cref="GetSchemaTable"/> has been called, its results are cached here.
        /// </summary>
        DataTable _cachedSchemaTable;
#endif

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        /// <summary>
        /// In non-sequential mode, contains the cached values already read from the current row
        /// </summary>
        readonly RowCache _rowCache;

        // static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        bool IsSequential => (_behavior & CommandBehavior.SequentialAccess) != 0;
        bool IsCaching => !IsSequential;
        bool IsSchemaOnly => (_behavior & CommandBehavior.SchemaOnly) != 0;

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior, List<NpgsqlStatement> statements)
        {
            Command = command;
            _connection = command.Connection;
            _connector = _connection.Connector;
            _behavior = behavior;

            State = IsSchemaOnly ? ReaderState.BetweenResults : ReaderState.InResult;

            if (IsCaching) {
                _rowCache = new RowCache();
            }
            _statements = statements;
        }

        [RewriteAsync]
        internal void Init()
        {
            _rowDescription = _statements[0].Description;

            if (_rowDescription == null)
            {
                // This happens if the first (or only) statement does not return a resultset (e.g. INSERT).
                // At the end of Init the reader should be positioned at the beginning of the first resultset,
                // so we seek it here.
                if (!NextResult())
                {
                    // No resultsets at all
                    return;
                }
            }

            // If we have any output parameters, we need to read the first data row (in non-sequential mode)
            // and extract them
            if (Command.Parameters.Any(p => p.IsOutputDirection))
            {
                PopulateOutputParameters();
            }
            else
            {
                // If the query generated an error, we want an exception thrown here (i.e. from ExecuteQuery).
                // So read the first message
                // Note this isn't necessary if we're in SchemaOnly mode (we don't execute the query so no error
                // is possible). Also, if we have output parameters as we already read the first message above.
                if (!IsSchemaOnly)
                {
                    _pendingMessage = ReadMessage();
                }
            }
        }

        /// <summary>
        /// The first row in a stored procedure command that has output parameters needs to be traversed twice -
        /// once for populating the output parameters and once for the actual result set traversal. So in this
        /// case we can't be sequential.
        /// </summary>
        void PopulateOutputParameters()
        {
            Contract.Requires(_rowDescription != null);
            Contract.Requires(Command.Parameters.Any(p => p.IsOutputDirection));

            while (_row == null)
            {
                var msg = _connector.ReadSingleMessage(DataRowLoadingMode.NonSequential);
                switch (msg.Code)
                {
                case BackendMessageCode.DataRow:
                    _pendingMessage = msg;
                    _row = (DataRowNonSequentialMessage)msg;
                    break;
                case BackendMessageCode.CompletedResponse:
                case BackendMessageCode.EmptyQueryResponse:
                    _pendingMessage = msg;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("Unexpected message type while populating output parameter: " + msg.Code);
                }
            }

            Contract.Assume(_rowDescription.NumFields == _row.NumColumns);
            if (IsCaching) { _rowCache.Clear(); }

            var pending = new Queue<NpgsqlParameter>();
            var taken = new List<int>();
            foreach (var p in Command.Parameters.Where(p => p.IsOutputDirection))
            {
                int idx;
                if (_rowDescription.TryGetFieldIndex(p.CleanName, out idx))
                {
                    // TODO: Provider-specific check?
                    p.Value = GetValue(idx);
                    taken.Add(idx);
                }
                else
                {
                    pending.Enqueue(p);
                }
            }
            for (var i = 0; pending.Count != 0 && i != _row.NumColumns; ++i)
            {
                if (!taken.Contains(i))
                {
                    // TODO: Need to get the provider-specific value based on the out param's type
                    pending.Dequeue().Value = GetValue(i);
                }
            }
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
            return ReadInternal();
        }

        /// <summary>
        /// This is the asynchronous version of <see cref="Read"/> The cancellation token is currently ignored.
        /// </summary>
        /// <param name="cancellationToken">Ignored for now.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return await ReadInternalAsync(cancellationToken).ConfigureAwait(false);
        }

        [RewriteAsync]
        bool ReadInternal()
        {
            if (_row != null) {
                _row.Consume();
                _row = null;
            }

            switch (State)
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
                    Consume();
                    return false;
                }

                while (true)
                {
                    var msg = ReadMessage();
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
            catch (NpgsqlException)
            {
                State = ReaderState.Consumed;
                throw;
            }
        }

        ReadResult ProcessMessage(IBackendMessage msg)
        {
            Contract.Requires(msg != null);

            switch (msg.Code)
            {
                case BackendMessageCode.DataRow:
                    Contract.Assert(_rowDescription != null);
                    _connector.State = ConnectorState.Fetching;
                    _row = (DataRowMessage)msg;
                    Contract.Assume(_rowDescription.NumFields == _row.NumColumns);
                    if (IsCaching) { _rowCache.Clear(); }
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
                            if (!_recordsAffected.HasValue) {
                                _recordsAffected = 0;
                            }
                            _recordsAffected += completed.Rows;
                            break;
                    }

                    _statements[_statementIndex].StatementType = completed.StatementType;
                    _statements[_statementIndex].Rows = completed.Rows;
                    _statements[_statementIndex].OID = completed.OID;

                    goto case BackendMessageCode.EmptyQueryResponse;

                case BackendMessageCode.EmptyQueryResponse:
                    State = ReaderState.BetweenResults;
                    return ReadResult.RowNotRead;

                case BackendMessageCode.ReadyForQuery:
                    State = ReaderState.Consumed;
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
        public override sealed bool NextResult()
        {
            return IsSchemaOnly ? NextResultSchemaOnly() : NextResultInternal();
        }

        /// <summary>
        /// This is the asynchronous version of NextResult.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async sealed Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return IsSchemaOnly ? NextResultSchemaOnly() : await NextResultInternalAsync(cancellationToken).ConfigureAwait(false);
        }

        [RewriteAsync]
        bool NextResultInternal()
        {
            Contract.Requires(!IsSchemaOnly);
            // Contract.Ensures(Command.CommandType != CommandType.StoredProcedure || Contract.Result<bool>() == false);

            try
            {
                // If we're in the middle of a resultset, consume it
                switch (State)
                {
                    case ReaderState.InResult:
                        if (_row != null) {
                            _row.Consume();
                            _row = null;
                        }

                        // TODO: Duplication with SingleResult handling above
                        var completedMsg = SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.EmptyQueryResponse);
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

                Contract.Assert(State == ReaderState.BetweenResults);
                _hasRows = null;
#if NET45 || NET452 || DNX452
                _cachedSchemaTable = null;
#endif

                if ((_behavior & CommandBehavior.SingleResult) != 0)
                {
                    if (State == ReaderState.BetweenResults) {
                        Consume();
                    }
                    return false;
                }

                // We are now at the end of the previous result set. Read up to the next result set, if any.
                for (_statementIndex++; _statementIndex < _statements.Count; _statementIndex++)
                {
                    _rowDescription = _statements[_statementIndex].Description;
                    if (_rowDescription != null)
                    {
                        State = ReaderState.InResult;
                        // Found a resultset
                        return true;
                    }

                    // Next query has no resultset, read and process its completion message and move on to the next
                    var completedMsg = SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.EmptyQueryResponse);
                    ProcessMessage(completedMsg);
                }

                // There are no more queries, we're done. Read to the RFQ.
                ProcessMessage(SkipUntil(BackendMessageCode.ReadyForQuery));
                _rowDescription = null;
                return false;
            }
            catch (NpgsqlException)
            {
                State = ReaderState.Consumed;
                throw;
            }
        }

        /// <summary>
        /// Note that in SchemaOnly mode there are no resultsets, and we read nothing from the backend (all
        /// RowDescriptions have already been processed and are available)
        /// </summary>
        bool NextResultSchemaOnly()
        {
            Contract.Requires(IsSchemaOnly);

            for (_statementIndex++; _statementIndex < _statements.Count; _statementIndex++)
            {
                _rowDescription = _statements[_statementIndex].Description;
                if (_rowDescription != null)
                {
                    // Found a resultset
                    return true;
                }
            }

            return false;
        }

        #endregion

        [RewriteAsync]
        IBackendMessage ReadMessage()
        {
            if (_pendingMessage != null) {
                var msg = _pendingMessage;
                _pendingMessage = null;
                return msg;
            }
            return _connector.ReadSingleMessage(IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential);
        }

        [RewriteAsync]
        IBackendMessage SkipUntil(BackendMessageCode stopAt)
        {
            if (_pendingMessage != null)
            {
                if (_pendingMessage.Code == stopAt)
                {
                    var msg = _pendingMessage;
                    _pendingMessage = null;
                    return msg;
                }
                _pendingMessage = null;
            }
            return _connector.SkipUntil(stopAt);
        }

        [RewriteAsync]
        IBackendMessage SkipUntil(BackendMessageCode stopAt1, BackendMessageCode stopAt2)
        {
            if (_pendingMessage != null) {
                if (_pendingMessage.Code == stopAt1 || _pendingMessage.Code == stopAt2) {
                    var msg = _pendingMessage;
                    _pendingMessage = null;
                    return msg;
                }
                _pendingMessage = null;
            }
            return _connector.SkipUntil(stopAt1, stopAt2);
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.  Always returns zero.
        /// </summary>
        public override Int32 Depth => 0;

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
        public override bool HasRows
        {
            get
            {
                if (_hasRows.HasValue) {
                    return _hasRows.Value;
                }
                if (_statementIndex >= _statements.Count) {
                    return false;
                }
                while (true)
                {
                    var msg = ReadMessage();
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
                            _pendingMessage = msg;
                            _hasRows = false;
                            return false;
                        case BackendMessageCode.CloseComplete:
                            _hasRows = false;
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException("Got unexpected message type: " + msg.Code);
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the reader is currently positioned on a row, i.e. whether reading a
        /// column is possible.
        /// This property is different from <see cref="HasRows"/> in that <see cref="HasRows"/> will
        /// return true even if attempting to read a column will fail, e.g. before <see cref="Read"/>
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
            Contract.EndContractBlock();

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
        [RewriteAsync]
        void Consume()
        {
            if (IsSchemaOnly)
            {
                State = ReaderState.Consumed;
                return;
            }

            if (_row != null)
            {
                _row.Consume();
                _row = null;
            }

            // Skip over the other result sets, processing only CommandCompleted for RecordsAffected
            while (true)
            {
                var msg = SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.ReadyForQuery);
                switch (msg.Code)
                {
                    case BackendMessageCode.CompletedResponse:
                        ProcessMessage(msg);
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        ProcessMessage(msg);
                        return;
                    default:
                        throw new Exception("Unexpected message of type " + msg.Code);
                }
            }
        }

        /// <summary>
        /// Releases the resources used by the <see cref="NpgsqlDataReader">NpgsqlDataReader</see>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Close();
        }

        /// <summary>
        /// Closes the <see cref="NpgsqlDataReader"/> object.
        /// </summary>
#if NET45 || NET452 || DNX452
        public override void Close()
#else
        public void Close()
#endif
        {
            if (State == ReaderState.Closed) { return; }

            switch (_connector.State)
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

            if (State != ReaderState.Consumed) {
                Consume();
            }

            Cleanup();
        }

        internal void Cleanup()
        {
            State = ReaderState.Closed;
            Command.State = CommandState.Idle;
            _connector.CurrentReader = null;
            _connector.EndUserAction();

            if ((_behavior & CommandBehavior.CloseConnection) != 0)
            {
                _connection.Close();
            }

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
        private DataRowMessage Row
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
        public override bool GetBoolean(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumnWithoutCache<bool>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumnWithoutCache<byte>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a single character.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumnWithoutCache<char>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<short>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<int>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<long>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override DateTime GetDateTime(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<DateTime>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="string"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<string>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a <see cref="decimal"/> object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<decimal>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a double-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<double>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a single-precision floating point number.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<float>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a globally-unique identifier (GUID).
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<Guid>(ordinal);
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
        public override int GetValues(object[] values)
        {
#region Contracts
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckRow();
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= values.Length);
#endregion

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++) {
                values[i] = GetValue(i);
            }
            return count;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[int ordinal]
        {
            get
            {
                CheckRowAndOrdinal(ordinal);
                Contract.EndContractBlock();

                return GetValue(ordinal);
            }
        }

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
        public NpgsqlDate GetDate(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<NpgsqlDate>(ordinal);
        }

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
        public TimeSpan GetTimeSpan(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<TimeSpan>(ordinal);
        }

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
        public NpgsqlTimeSpan GetInterval(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<NpgsqlTimeSpan>(ordinal);
        }

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
        public NpgsqlDateTime GetTimeStamp(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            return ReadColumn<NpgsqlDateTime>(ordinal);
        }

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
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
#region Contracts
            CheckRowAndOrdinal(ordinal);
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset,
                    $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length - 1)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");
            Contract.Ensures(Contract.Result<long>() >= 0);
#endregion

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null) {
                throw new InvalidCastException("GetBytes() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.SeekToColumn(ordinal);
            row.CheckNotNull();
            return handler.GetBytes(row, (int)dataOffset, buffer, bufferOffset, length, fieldDescription);
        }

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
#if NET40
        public Stream GetStream(int ordinal)
#else
        public override Stream GetStream(int ordinal)
#endif
        {
            CheckRowAndOrdinal(ordinal);
            Contract.Ensures(Contract.Result<Stream>() != null);

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null) {
                throw new InvalidCastException("GetStream() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.SeekToColumnStart(ordinal);
            row.CheckNotNull();
            return row.GetStream();
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
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
#region Contracts
            CheckRowAndOrdinal(ordinal);
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset,
                    $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length - 1)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");
            Contract.Ensures(Contract.Result<long>() >= 0);
#endregion

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null) {
                throw new InvalidCastException("GetChars() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.SeekToColumn(ordinal);
            row.CheckNotNull();
            return handler.GetChars(row, (int)dataOffset, buffer, bufferOffset, length, fieldDescription);
        }

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
#if NET40
        public TextReader GetTextReader(int ordinal)
#else
        public override TextReader GetTextReader(int ordinal)
#endif
        {
            CheckRowAndOrdinal(ordinal);
            Contract.Ensures(Contract.Result<TextReader>() != null);

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null)
            {
                throw new InvalidCastException("GetTextReader() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.SeekToColumnStart(ordinal);
            row.CheckNotNull();

            return new StreamReader(row.GetStream());
        }

#endregion

#region IsDBNull

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public override bool IsDBNull(int ordinal)
        {
            return IsDBNullInternal(ordinal);
        }

        /// <summary>
        /// An asynchronous version of <see cref="IsDBNull"/>, which gets a value that indicates whether the column contains non-existent or missing values.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns><b>true</b> if the specified column value is equivalent to <see cref="DBNull"/> otherwise <b>false</b>.</returns>
        public override async Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            return await IsDBNullInternalAsync(cancellationToken, ordinal).ConfigureAwait(false);
        }

        [RewriteAsync]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        bool IsDBNullInternal(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            Row.SeekToColumn(ordinal);
            return _row.IsColumnNull;
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
#region Contracts
            CheckResultSet();
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty", nameof(name));
            Contract.EndContractBlock();
#endregion

            return _rowDescription.GetFieldIndex(name);
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// This will be the PostgreSQL type name (e.g. int4) as in the pg_type table,
        /// not the .NET type (see <see cref="GetFieldType"/> for that).
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        /// <returns></returns>
        public override string GetDataTypeName(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);
            Contract.EndContractBlock();

            // In AllResultTypesAreUnknown mode, the handler is UnrecognizedTypeHandler but we can still get
            // the data type name from the handler that would have been used in binary for the type OID
            var field = _rowDescription[ordinal];
            TypeHandler binaryHandler;
            return
                field.Handler is UnrecognizedTypeHandler &&
                field.OID != 0 &&
                _connector.TypeHandlerRegistry.OIDIndex.TryGetValue(field.OID, out binaryHandler)
                ? binaryHandler.PgName
                : field.Handler.PgName;
        }

        /// <summary>
        /// Gets the OID for the PostgreSQL type for the specified field, as it appears in the pg_type table.
        /// </summary>
        /// <remarks>
        /// This is a PostgreSQL-internal value that should not be relied upon and should only be used for
        /// debugging purposes.
        /// </remarks>
        /// <param name="ordinal">The zero-based column index.</param>
        /// <returns></returns>
        public uint GetDataTypeOID(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);
            Contract.EndContractBlock();

            return _rowDescription[ordinal].OID;
        }

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        public override Type GetFieldType(int ordinal)
        {
            CheckResultSet();
            CheckOrdinal(ordinal);
            Contract.EndContractBlock();

            var type = Command.ObjectResultTypes?[ordinal];
            if (type != null)
            {
                return type;
            }

            var field = _rowDescription[ordinal];
            return field.Handler.GetFieldType(field);
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
            Contract.EndContractBlock();

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

            CachedValue<object> cache = null;
            if (IsCaching)
            {
                cache = _rowCache.Get<object>(ordinal);
                if (cache.IsSet && !cache.IsProviderSpecificValue) {
                    return cache.Value;
                }
            }

            // TODO: Code duplication with ReadColumn<T>
            _row.SeekToColumnStart(ordinal);
            if (_row.IsColumnNull) {
                return DBNull.Value;
            }
            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            object result;
            try {
                result = handler.ReadValueAsObjectFully(_row, fieldDescription);
            } catch (SafeReadException e) {
                throw e.InnerException;
            } catch {
                _connector.Break();
                throw;
            }

            // Used for Entity Framework <= 6 compability
            if (Command.ObjectResultTypes?[ordinal] != null && result != null)
            {
                var type = Command.ObjectResultTypes[ordinal];
                if (type == typeof(DateTimeOffset))
                {
                    result = new DateTimeOffset((DateTime)result);
                }
                else
                {
                    result = Convert.ChangeType(result, type);
                }
            }

            if (IsCaching)
            {
                Contract.Assert(cache != null);
                cache.Value = result;
                cache.IsProviderSpecificValue = false;
            }
            return result;
        }

        /// <summary>
        /// Synchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <returns>The column to be retrieved.</returns>
        public override T GetFieldValue<T>(int ordinal)
        {
            return GetFieldValueInternal<T>(ordinal);
        }

        /// <summary>
        /// Asynchronously gets the value of the specified column as a type.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            return await GetFieldValueInternalAsync<T>(cancellationToken, ordinal).ConfigureAwait(false);
        }

        [RewriteAsync]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T GetFieldValueInternal<T>(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.EndContractBlock();

            var t = typeof(T);
            if (!t.IsArray) {
                if (t == typeof(object)) {
                    return (T)GetValue(ordinal);
                }
                return ReadColumn<T>(ordinal);
            }

            // Getting an array

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            // If the type handler can simply return the requested array, call it as usual. This is the case
            // of reading a string as char[], a bytea as a byte[]...
            var tHandler = handler as ITypeHandler<T>;
            if (tHandler != null) {
                return ReadColumn<T>(ordinal);
            }

            // We need to treat this as an actual array type, these need special treatment because of
            // typing/generics reasons
            var elementType = t.GetElementType();
            var arrayHandler = handler as IArrayHandler;
            if (arrayHandler == null) {
                throw new InvalidCastException(
                    $"Can't cast database type {fieldDescription.Handler.PgName} to {typeof (T).Name}");
            }

            if (arrayHandler.GetElementFieldType(fieldDescription) == elementType)
            {
                return (T)GetValue(ordinal);
            }
            if (arrayHandler.GetElementPsvType(fieldDescription) == elementType)
            {
                return (T)GetProviderSpecificValue(ordinal);
            }
            throw new InvalidCastException($"Can't cast database type {handler.PgName} to {typeof (T).Name}");
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetProviderSpecificValue(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);
            Contract.Ensures(Contract.Result<object>() == DBNull.Value || GetProviderSpecificFieldType(ordinal).IsInstanceOfType(Contract.Result<object>()));

            CachedValue<object> cache = null;
            if (IsCaching)
            {
                cache = _rowCache.Get<object>(ordinal);
                if (cache.IsSet && cache.IsProviderSpecificValue) {
                    return cache.Value;
                }
            }

            // TODO: Code duplication with ReadColumn<T>
            _row.SeekToColumnStart(ordinal);
            if (_row.IsColumnNull) {
                return DBNull.Value;
            }
            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            object result;
            try {
                result = handler.ReadPsvAsObjectFully(_row, fieldDescription);
            } catch (SafeReadException e) {
                throw e.InnerException;
            } catch {
                _connector.Break();
                throw;
            }

            if (IsCaching)
            {
                Contract.Assert(cache != null);
                cache.Value = result;
                cache.IsProviderSpecificValue = true;
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
#region Contracts
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckRow();
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= values.Length);
#endregion

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
            {
                values[i] = GetProviderSpecificValue(i);
            }
            return count;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.</returns>
        public override IEnumerator GetEnumerator()
        {
#if NET45 || NET452 || DNX452
            return new DbEnumerator(this);
#else
            throw new NotSupportedException("GetEnumerator not yet supported in .NET Core");
#endif
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [RewriteAsync]
        T ReadColumnWithoutCache<T>(int ordinal)
        {
            _row.SeekToColumnStart(ordinal);
            Row.CheckNotNull();
            var fieldDescription = _rowDescription[ordinal];
            try
            {
                return fieldDescription.Handler.ReadFully<T>(_row, Row.ColumnLen, fieldDescription);
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

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [RewriteAsync]
        T ReadColumn<T>(int ordinal)
        {
            CachedValue<T> cache = null;
            if (IsCaching)
            {
                cache = _rowCache.Get<T>(ordinal);
                if (cache.IsSet) {
                    return cache.Value;
                }
            }
            var result = ReadColumnWithoutCache<T>(ordinal);
            if (IsCaching)
            {
                Contract.Assert(cache != null);
                cache.Value = result;
            }
            return result;
        }

        #region Schema metadata table
#if NET45 || NET452 || DNX452

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the DataReader.
        /// </summary>
        public override DataTable GetSchemaTable()
        {
            CheckResultSet();
            Contract.Ensures(Contract.Result<DataTable>() != null);

            if (_cachedSchemaTable != null) {
                return _cachedSchemaTable;
            }

            var result = new DataTable("SchemaTable");

            result.Columns.Add("AllowDBNull", typeof(bool));
            result.Columns.Add("BaseCatalogName", typeof(string));
            result.Columns.Add("BaseColumnName", typeof(string));
            result.Columns.Add("BaseSchemaName", typeof(string));
            result.Columns.Add("BaseTableName", typeof(string));
            result.Columns.Add("ColumnName", typeof(string));
            result.Columns.Add("ColumnOrdinal", typeof(int));
            result.Columns.Add("ColumnSize", typeof(int));
            result.Columns.Add("DataType", typeof(Type));
            result.Columns.Add("IsUnique", typeof(bool));
            result.Columns.Add("IsKey", typeof(bool));
            result.Columns.Add("IsAliased", typeof(bool));
            result.Columns.Add("IsExpression", typeof(bool));
            result.Columns.Add("IsIdentity", typeof(bool));
            result.Columns.Add("IsAutoIncrement", typeof(bool));
            result.Columns.Add("IsRowVersion", typeof(bool));
            result.Columns.Add("IsHidden", typeof(bool));
            result.Columns.Add("IsLong", typeof(bool));
            result.Columns.Add("IsReadOnly", typeof(bool));
            result.Columns.Add("NumericPrecision", typeof(int));
            result.Columns.Add("NumericScale", typeof(int));
            result.Columns.Add("ProviderSpecificDataType", typeof(Type));
            result.Columns.Add("ProviderType", typeof(Type));

            FillSchemaTable(result);

            return result;
        }

        private void FillSchemaTable(DataTable schema)
        {
            var oidTableLookup = new Dictionary<uint, Table>();
            var keyLookup = new KeyLookup();
            // needs to be null because there is a difference
            // between an empty dictionary and not setting it
            // the default values will be different
            Dictionary<string, Column> columnLookup = null;

            // TODO: This is probably not what KeyInfo is supposed to do...
            if ((_behavior & CommandBehavior.KeyInfo) == CommandBehavior.KeyInfo)
            {
                var tableOids = new List<uint>();
                for (var i = 0; i != _rowDescription.NumFields; ++i)
                {
                    if (_rowDescription[i].TableOID != 0 && !tableOids.Contains(_rowDescription[i].TableOID))
                    {
                        tableOids.Add(_rowDescription[i].TableOID);
                    }
                }
                oidTableLookup = GetTablesFromOids(tableOids);

                if (oidTableLookup.Count == 1)
                {
                    // only 1, but we can't index into the Dictionary
                    foreach (var key in oidTableLookup.Keys)
                    {
                        keyLookup = GetKeys((int)key);
                    }
                }

                columnLookup = GetColumns();
            }

            for (var i = 0; i < _rowDescription.NumFields; i++)
            {
                var field = _rowDescription[i];
                var row = schema.NewRow();

                var baseColumnName = GetBaseColumnName(columnLookup, i);

                row["AllowDBNull"] = IsNullable(columnLookup, i);
                row["BaseColumnName"] = baseColumnName;
                if (field.TableOID != 0 && oidTableLookup.ContainsKey(field.TableOID))
                {
                    row["BaseCatalogName"] = oidTableLookup[_rowDescription[i].TableOID].Catalog;
                    row["BaseSchemaName"] = oidTableLookup[_rowDescription[i].TableOID].Schema;
                    row["BaseTableName"] = oidTableLookup[_rowDescription[i].TableOID].Name;
                }
                else
                {
                    row["BaseCatalogName"] = row["BaseSchemaName"] = row["BaseTableName"] = "";
                }
                row["ColumnName"] = GetName(i);
                row["ColumnOrdinal"] = i + 1;

                if (field.TypeModifier != -1 && field.Handler is TextHandler)
                {
                    row["ColumnSize"] = field.TypeModifier - 4;
                }
                else if (field.TypeModifier != -1 && field.Handler is BitStringHandler)
                {
                    row["ColumnSize"] = field.TypeModifier;
                }
                else
                {
                    row["ColumnSize"] = (int)field.TypeSize;
                }
                row["DataType"] = GetFieldType(i); // non-standard
                row["IsUnique"] = IsUnique(keyLookup, baseColumnName);
                row["IsKey"] = IsKey(keyLookup, baseColumnName);
                row["IsAliased"] = string.CompareOrdinal((string)row["ColumnName"], baseColumnName) != 0;
                row["IsExpression"] = false;
                row["IsAutoIncrement"] = IsAutoIncrement(columnLookup, i);
                row["IsIdentity"] = false; // TODO - PostgreSQL doesn't define an identity type.  The following could be used to set this to act like SQL Server: (((bool)row["IsAutoIncrement"]) && (field.Handler.NpgsqlDbType == NpgsqlDbType.Integer || field.Handler.NpgsqlDbType == NpgsqlDbType.Bigint || field.Handler.NpgsqlDbType == NpgsqlDbType.Smallint));
                row["IsRowVersion"] = false;
                row["IsHidden"] = false;
                row["IsLong"] = false;   // TODO - Large object aren't stored as columns so this should always be false.
                row["IsReadOnly"] = IsReadOnly(columnLookup, i);
                if (field.TypeModifier != -1 && field.Handler is NumericHandler)
                {
                    row["NumericPrecision"] = ((field.TypeModifier - 4) >> 16) & ushort.MaxValue;
                    row["NumericScale"] = (field.TypeModifier - 4) & ushort.MaxValue;
                }
                else
                {
                    row["NumericPrecision"] = 0;
                    row["NumericScale"] = 0;
                }
                row["ProviderType"] = GetFieldType(i);
                if (_rowDescription[i].Handler is ITypeHandlerWithPsv) {
                    row["ProviderSpecificDataType"] = GetProviderSpecificFieldType(i);
                }

                schema.Rows.Add(row);
            }
        }

        private static bool IsKey(KeyLookup keyLookup, string fieldName)
        {
            return keyLookup.PrimaryKey.Contains(fieldName);
        }

        private static bool IsUnique(KeyLookup keyLookup, string fieldName)
        {
            return keyLookup.UniqueColumns.Contains(fieldName);
        }

        private bool IsNullable(Dictionary<string, Column> columnLookup, int fieldIndex)
        {
            if (columnLookup == null || _rowDescription[fieldIndex].TableOID == 0)
            {
                return true;
            }

            var lookupKey =
                $"{_rowDescription[fieldIndex].TableOID},{_rowDescription[fieldIndex].ColumnAttributeNumber}";
            Column col;
            return !columnLookup.TryGetValue(lookupKey, out col) || !col.NotNull;
        }

        private bool IsAutoIncrement(Dictionary<string, Column> columnLookup, int fieldIndex)
        {
            if (columnLookup == null || _rowDescription[fieldIndex].TableOID == 0)
            {
                return false;
            }

            var lookupKey =
                $"{_rowDescription[fieldIndex].TableOID},{_rowDescription[fieldIndex].ColumnAttributeNumber}";
            Column col;
            return
                !columnLookup.TryGetValue(lookupKey, out col) || col.ColumnDefault is string && col.ColumnDefault.ToString().StartsWith("nextval(");
        }

        private bool IsReadOnly(Dictionary<string, Column> columnLookup, int fieldIndex)
        {
            if (columnLookup == null || _rowDescription[fieldIndex].TableOID == 0)
            {
                return false;
            }

            var lookupKey =
                $"{_rowDescription[fieldIndex].TableOID},{_rowDescription[fieldIndex].ColumnAttributeNumber}";
            Column col;
            return
                columnLookup.TryGetValue(lookupKey, out col)
                    ? !col.IsUpdateable
                    : false;
        }

        string GetBaseColumnName(Dictionary<string, Column> columnLookup, int fieldIndex)
        {
            if (columnLookup == null || _rowDescription[fieldIndex].TableOID == 0)
            {
                return GetName(fieldIndex);
            }

            var lookupKey =
                $"{_rowDescription[fieldIndex].TableOID},{_rowDescription[fieldIndex].ColumnAttributeNumber}";
            Column col;
            return columnLookup.TryGetValue(lookupKey, out col) ? col.Name : GetName(fieldIndex);
        }

        KeyLookup GetKeys(Int32 tableOid)
        {
            const string getKeys = "select a.attname, ci.relname, i.indisprimary from pg_catalog.pg_class ct, pg_catalog.pg_class ci, pg_catalog.pg_attribute a, pg_catalog.pg_index i WHERE ct.oid=i.indrelid AND ci.oid=i.indexrelid AND a.attrelid=ci.oid AND i.indisunique AND ct.oid = :tableOid order by ci.relname";

            var lookup = new KeyLookup();

            using (var metadataConn = new NpgsqlConnection(_connection.ConnectionString))
            {
                metadataConn.Open();

                using (var c = new NpgsqlCommand(getKeys, metadataConn))
                {
                    c.Parameters.Add(new NpgsqlParameter("tableOid", NpgsqlDbType.Integer)).Value = tableOid;

                    using (var dr = c.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
                    {
                        string previousKeyName = null;
                        string possiblyUniqueColumn = null;
                        // loop through adding any column that is primary to the primary key list
                        // add any column that is the only column for that key to the unique list
                        // unique here doesn't mean general unique constraint (with possibly multiple columns)
                        // it means all values in this single column must be unique
                        while (dr.Read())
                        {
                            var columnName = dr.GetString(0);
                            var currentKeyName = dr.GetString(1);
                            // if i.indisprimary
                            if (dr.GetBoolean(2))
                            {
                                // add column name as part of the primary key
                                lookup.PrimaryKey.Add(columnName);
                            }
                            if (currentKeyName != previousKeyName)
                            {
                                if (possiblyUniqueColumn != null)
                                {
                                    lookup.UniqueColumns.Add(possiblyUniqueColumn);
                                }
                                possiblyUniqueColumn = columnName;
                            }
                            else
                            {
                                possiblyUniqueColumn = null;
                            }
                            previousKeyName = currentKeyName;
                        }
                        // if finished reading and have a possiblyUniqueColumn name that is
                        // not null, then it is the name of a unique column
                        if (possiblyUniqueColumn != null)
                        {
                            lookup.UniqueColumns.Add(possiblyUniqueColumn);
                        }
                        return lookup;
                    }
                }
            }
        }

        class KeyLookup
        {
            /// <summary>
            /// Contains the column names as the keys
            /// </summary>
            public readonly List<string> PrimaryKey = new List<string>();

            /// <summary>
            /// Contains all unique columns
            /// </summary>
            public readonly List<string> UniqueColumns = new List<string>();
        }

        struct Table
        {
            public readonly string Catalog;
            public readonly string Schema;
            public readonly string Name;
            public readonly uint Id;

            public Table(NpgsqlDataReader rdr)
            {
                Catalog = rdr.GetString(0);
                Schema = rdr.GetString(1);
                Name = rdr.GetString(2);
                Id = rdr.GetFieldValue<uint>(3);
            }
        }

        Dictionary<uint, Table> GetTablesFromOids(List<uint> oids)
        {
            if (oids.Count == 0) {
                return new Dictionary<uint, Table>(); //Empty collection is simpler than requiring tests for null;
            }

            // the column index is used to find data.
            // any changes to the order of the columns needs to be reflected in struct Tables
            string commandText = string.Concat("SELECT current_database(), nc.nspname, c.relname, c.oid FROM pg_namespace nc, pg_class c WHERE c.relnamespace = nc.oid AND (c.relkind = 'r' OR c.relkind = 'v') AND c.oid IN (",
                string.Join(",", oids), ")");

            using (var connection = (NpgsqlConnection)((ICloneable)_connection).Clone())
            {
                connection.Open();

                using (var command = new NpgsqlCommand(commandText, connection))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
                    {
                        var oidLookup = new Dictionary<uint, Table>(oids.Count);
                        while (reader.Read())
                        {
                            var t = new Table(reader);
                            oidLookup.Add(t.Id, t);
                        }
                        return oidLookup;
                    }
                }
            }
        }

        class Column
        {
            internal readonly string Name;
            internal readonly bool NotNull;
            readonly uint TableId;
            readonly short ColumnNum;
            internal readonly object ColumnDefault;
            internal readonly bool IsUpdateable;

            internal string Key => $"{TableId},{ColumnNum}";

            internal Column(NpgsqlDataReader rdr)
            {
                Name = rdr.GetString(0);
                NotNull = rdr.GetBoolean(1);
                TableId = rdr.GetFieldValue<uint>(2);
                ColumnNum = rdr.GetInt16(3);
                ColumnDefault = rdr.GetValue(4);
                IsUpdateable = rdr.GetBoolean(5);
            }
        }

        [CanBeNull]
        Dictionary<string, Column> GetColumns()
        {
            var columnsFilter = _rowDescription.Fields
                .Where(f => f.TableOID != 0)
                .Select(f => $"(a.attrelid={f.TableOID} AND a.attnum={f.ColumnAttributeNumber})")
                .Join(" OR ");

            if (columnsFilter == "") {
                return null;  // No (real) columns
            }

            // the column index is used to find data.
            // any changes to the order of the columns needs to be reflected in struct Columns
            var query =
                $@"SELECT a.attname AS column_name, a.attnotnull AS column_notnull, a.attrelid AS table_id, a.attnum AS column_num, ad.adsrc as column_default
, CAST(CASE WHEN i.is_updatable = 'YES'
       THEN 1 ELSE 0 END AS bit) AS is_updatable
FROM (pg_attribute a LEFT JOIN pg_attrdef ad ON attrelid = adrelid AND attnum = adnum)
JOIN (pg_class c JOIN pg_namespace nc ON (c.relnamespace = nc.oid)) ON a.attrelid = c.oid
JOIN information_schema.columns i ON i.table_schema = nc.nspname
    AND i.table_name = c.relname AND i.column_name = a.attname
WHERE a.attnum > 0
    AND NOT a.attisdropped
    AND c.relkind in ('r', 'v', 'f')
    AND (pg_has_role(c.relowner, 'USAGE')
        OR has_column_privilege(c.oid, a.attnum, 'SELECT, INSERT, UPDATE, REFERENCES'))
    AND ({
                    columnsFilter})";

            using (var connection = new NpgsqlConnection(_connection.ConnectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
                    {
                        var columnLookup = new Dictionary<string, Column>();
                        while (reader.Read())
                        {
                            var column = new Column(reader);
                            columnLookup.Add(column.Key, column);
                        }
                        return columnLookup;
                    }
                }
            }
        }

#endif
        #endregion Schema metadata table

        #region Checks

        [ContractArgumentValidator]
        void CheckRowAndOrdinal(int ordinal)
        {
            CheckRow();
            CheckOrdinal(ordinal);
            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        void CheckRow()
        {
            if (!IsOnRow)
                throw new InvalidOperationException("No row is available");
            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        // ReSharper disable once UnusedParameter.Local
        void CheckOrdinal(int ordinal)
        {
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException($"Column must be between {0} and {(FieldCount - 1)}");
            Contract.EndContractBlock();
        }

        [ContractAbbreviator]
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
