using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Npgsql.Localization;
using Npgsql.BackendMessages;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.NumericHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    public class NpgsqlDataReader : DbDataReader
    {
        private ReaderState _state;
        internal NpgsqlCommand Command { get; private set; }
        readonly NpgsqlConnector _connector;
        readonly NpgsqlConnection _connection;
        readonly CommandBehavior _behavior;

        // TODO: Protect with Interlocked?
        internal ReaderState State {
            get { return _state; }
            private set { _state = value; }
        }

        /// <summary>
        /// Holds the list of RowDescription messages for each of the resultsets this reader is to process.
        /// </summary>
        List<QueryDetails> _queries;

        /// <summary>
        /// The index of the current query resultset we're processing (within a multiquery)
        /// </summary>
        int _queryIndex;

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
        BackendMessage _pendingMessage;

        /// <summary>
        /// If <see cref="GetSchemaTable"/> has been called, its results are cached here.
        /// </summary>
        DataTable _cachedSchemaTable;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        /// <summary>
        /// In non-sequential mode, contains the cached values already read from the current row
        /// </summary>
        readonly RowCache _rowCache;

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal bool IsSequential { get { return (_behavior & CommandBehavior.SequentialAccess) != 0; } }
        internal bool IsCaching { get { return !IsSequential; } }
        internal bool IsSchemaOnly { get { return (_behavior & CommandBehavior.SchemaOnly) != 0; } }

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior, List<QueryDetails> queries)
        {
            Command = command;
            _connector = command.Connector;
            _connection = command.Connection;
            _behavior = behavior;
            _recordsAffected = null;

            State = IsSchemaOnly ? ReaderState.BetweenResults : ReaderState.InResult;
            _connector.State = ConnectorState.Executing;

            if (IsCaching) {
                _rowCache = new RowCache();
            }
            _connector.CurrentReader = this;
            _queries = queries;

            _rowDescription = _queries[0].Description;
            if (_rowDescription == null)
            {
                // The first query has not result set, seek forward to the first query that does (if any)
                if (!NextResult())
                {
                    // No resultsets at all
                    return;
                }
            }

            if (Command.Parameters.Any(p => p.IsOutputDirection)) {
                PopulateOutputParameters();
            }
        }

        #region Read

        public override bool Read()
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
                CleanUpDueToException();
                throw;
            }
        }

        ReadResult ProcessMessage(BackendMessage msg)
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
                    if (completed.RowsAffected.HasValue)
                    {
                        _recordsAffected = !_recordsAffected.HasValue
                            ? completed.RowsAffected
                            : _recordsAffected.Value + completed.RowsAffected.Value;
                    }
                    if (completed.LastInsertedOID.HasValue) {
                        LastInsertedOID = completed.LastInsertedOID.Value;
                    }
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

        public override sealed bool NextResult()
        {
            return IsSchemaOnly ? NextResultSchemaOnly() : NextResultInternal();
        }

        bool NextResultInternal()
        {
            Contract.Requires(!IsSchemaOnly);
            Contract.Ensures(Command.CommandType != CommandType.StoredProcedure || Contract.Result<bool>() == false);

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
                _cachedSchemaTable = null;

                if ((_behavior & CommandBehavior.SingleResult) != 0) {
                    Consume();
                    return false;
                }

                // We are now at the end of the previous result set. Read up to the next result set, if any.
                for (_queryIndex++; _queryIndex < _queries.Count; _queryIndex++)
                {
                    _rowDescription = _queries[_queryIndex].Description;
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
                CleanUpDueToException();
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

            for (_queryIndex++; _queryIndex < _queries.Count; _queryIndex++)
            {
                _rowDescription = _queries[_queryIndex].Description;
                if (_rowDescription != null)
                {
                    // Found a resultset
                    return true;
                }
            }

            return false;
        }

        #endregion

        BackendMessage ReadMessage()
        {
            if (_pendingMessage != null) {
                var msg = _pendingMessage;
                _pendingMessage = null;
                return msg;
            }
            return _connector.ReadSingleMessage(IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential);
        }

        BackendMessage SkipUntil(params BackendMessageCode[] stopAt)
        {
            if (_pendingMessage != null)
            {
                if (stopAt.Contains(_pendingMessage.Code))
                {
                    var msg = _pendingMessage;
                    _pendingMessage = null;
                    return msg;
                }
                _pendingMessage = null;
            }
            return _connector.SkipUntil(stopAt);
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.  Always returns zero.
        /// </summary>
        public override Int32 Depth
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed
        {
            get { return State == ReaderState.Closed; }
        }

        public override int RecordsAffected
        {
            get { return _recordsAffected.HasValue ? (int)_recordsAffected.Value : -1; }
       }

        /// <summary>
        /// Returns the OID of the last inserted row.
        /// If table is created without OIDs, this will always be 0.
        /// </summary>
        public uint LastInsertedOID { get; private set; }

        public override bool HasRows
        {
            get
            {
                if (_hasRows.HasValue) {
                    return _hasRows.Value;
                }
                if (_queryIndex >= _queries.Count) {
                    return false;
                }
                while (true)
                {
                    var msg = _connector.ReadSingleMessage(IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential);
                    switch (msg.Code)
                    {
                        case BackendMessageCode.RowDescription:
                            ProcessMessage(msg);
                            continue;
                        case BackendMessageCode.DataRow:
                            _pendingMessage = msg;
                            return true;
                        case BackendMessageCode.CompletedResponse:
                        case BackendMessageCode.EmptyQueryResponse:
                            _pendingMessage = msg;
                            return false;
                        case BackendMessageCode.CloseComplete:
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
        public bool IsOnRow { get { return _row != null; } }

        public override string GetName(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException(L10N.NoResultSetAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return _rowDescription[ordinal].Name;
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override int FieldCount
        {
            get
            {
                // Note MSDN docs that seem to say we should case -1 in this case:
                // http://msdn.microsoft.com/en-us/library/system.data.idatarecord.fieldcount(v=vs.110).aspx
                // But SqlClient returns 0
                return _rowDescription == null ? 0 : _rowDescription.NumFields;
            }
        }

        #region Cleanup / Dispose

        /// <summary>
        /// Consumes all result sets for this reader, leaving the connector ready for sending and processing further
        /// queries
        /// </summary>
        private void Consume()
        {
            switch (State)
            {
                case ReaderState.Consumed:
                case ReaderState.Closed:
                    return;
            }

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

        public override void Close()
        {
            Consume();
            if (Command._notificationBlock != null) {
                Command._notificationBlock.Dispose();
                Command._notificationBlock = null;
            }
            if ((_behavior & CommandBehavior.CloseConnection) != 0) {
                _connection.Close();
            }
            State = ReaderState.Closed;
            _connector.State = ConnectorState.Ready;
            if (ReaderClosed != null) {
                ReaderClosed(this, EventArgs.Empty);
            }
        }

        private void CleanUpDueToException()
        {
            // The ReadyForQuery message that is sent after the ErrorResponse has already been consumed
            if (Command._notificationBlock != null)
            {
                Command._notificationBlock.Dispose();
                Command._notificationBlock = null;
            }
            State = ReaderState.Consumed;
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

        public override bool GetBoolean(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<bool>(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<byte>(ordinal);
        }

        public override char GetChar(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<char>(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<short>(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<int>(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion
            
            return ReadColumn<long>(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<DateTime>(ordinal);
        }

        public override string GetString(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<string>(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<decimal>(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<double>(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<float>(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<Guid>(ordinal);
        }

        public override int GetValues(object[] values)
        {
            #region Contracts
            if (values == null)
                throw new ArgumentNullException("values");
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= values.Length);
            #endregion

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++) {
                values[i] = GetValue(i);
            }
            return count;
        }

        public override object this[int ordinal]
        {
            get
            {
                #region Contracts
                if (!IsOnRow)
                    throw new InvalidOperationException(L10N.NoRowAvailable);
                if (ordinal < 0 || ordinal >= FieldCount)
                    throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
                Contract.EndContractBlock();
                #endregion

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
        /// See http://www.postgresql.org/docs/9.4/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlDate GetDate(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlDate>(ordinal);
        }

        // TODO: Probably get rid of this, we can use TimeSpan or DateTime, see https://github.com/npgsql/npgsql/issues/347
        public NpgsqlTime GetTime(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<NpgsqlTime>(ordinal);
        }

        // TODO: Replace with DateTimeOffset?
        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlTimeTZ"/>,
        /// Npgsql's provider-specific type for dates.
        /// </summary>
        /// <remarks>
        /// PostgreSQL has a "time with time zone" type, which combines a time of day with a time zone.
        /// Since .NET contains no analog type, the PostgreSQL value can be fetched as <see cref="NpgsqlTimeTZ"/>.
        /// The standard ADO.NET <see cref="GetProviderSpecificValue"/> method will also return this
        /// type, but has the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/9.4/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        // ReSharper disable once InconsistentNaming
        public NpgsqlTimeTZ GetTimeTZ(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlTimeTZ>(ordinal);
        }

        #endregion

        #region Provider-specific type getters

        /// <summary>
        /// Gets the value of the specified column as a TimeSpan,
        /// </summary>
        /// <remarks>
        /// PostgreSQL's interval type has has a resolution of 1 microsecond and ranges from
        /// -178000000 to 178000000 years, while .NET's TimeSpan has a resolution of 100 nanoseconds
        /// and ranges from roughly -29247 to 29247 years.
        /// See http://www.postgresql.org/docs/9.4/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public TimeSpan GetTimeSpan(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<TimeSpan>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlInterval"/>,
        /// Npgsql's provider-specific type for time spans.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's interval type has has a resolution of 1 microsecond and ranges from
        /// -178000000 to 178000000 years, while .NET's TimeSpan has a resolution of 100 nanoseconds
        /// and ranges from roughly -29247 to 29247 years. If you require values from outside TimeSpan's
        /// range use this accessor.
        /// The standard ADO.NET <see cref="GetProviderSpecificValue"/> method will also return this
        /// type, but has the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/9.4/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlInterval GetInterval(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlInterval>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as an <see cref="NpgsqlTimeStamp"/>,
        /// Npgsql's provider-specific type for date/time timestamps. Note that this type covers
        /// both PostgreSQL's "timestamp with time zone" and "timestamp without time zone" types,
        /// which differ only in how they are converted upon input/output.
        /// </summary>
        /// <remarks>
        /// PostgreSQL's timestamp type represents dates from 4713 BC to 5874897 AD, while .NET's DateTime
        /// only supports years from 1 to 1999. If you require years outside this range use this accessor.
        /// The standard <see cref="GetProviderSpecificValue"/> method will also return this type, but has
        /// the disadvantage of boxing the value.
        /// See http://www.postgresql.org/docs/9.4/static/datatype-datetime.html
        /// </remarks>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public NpgsqlTimeStamp GetTimeStamp(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlTimeStamp>(ordinal);
        }

        // TODO: Remove, not needed
        public NpgsqlTimeStampTZ GetTimeStampTZ(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<NpgsqlTimeStampTZ>(ordinal);
        }

        #endregion

        #region Special binary getters

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, String.Format(L10N.MustBeBetweenXAndY, "dataOffset", 0, int.MaxValue));
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "bufferOffset", 0, (buffer.Length - 1)));
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "length", 0, buffer.Length - bufferOffset));
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

#if NET45
        public override Stream GetStream(int ordinal)
#else
        public Stream GetStream(int ordinal)
#endif
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.Ensures(Contract.Result<Stream>() != null);
            #endregion

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

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, String.Format(L10N.MustBeBetweenXAndY, "dataOffset", 0, int.MaxValue));
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "bufferOffset", 0, (buffer.Length - 1)));
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "length", 0, buffer.Length - bufferOffset));
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

#if NET45
        public override TextReader GetTextReader(int ordinal)
#else
        public TextReader GetTextReader(int ordinal)
#endif
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.Ensures(Contract.Result<TextReader>() != null);
            #endregion

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

        public override bool IsDBNull(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            Row.SeekToColumn(ordinal);
            return _row.IsColumnNull;
        }

        public override object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public override int GetOrdinal(string name)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException(String.Format(L10N.CannotBeEmpty, "name"), "name");
            Contract.EndContractBlock();
            #endregion

            return _rowDescription.GetFieldIndex(name);
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// This will be the Postgresql type name (e.g. int4), not the .NET type (<see cref="GetFieldType"/>)
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetDataTypeName(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException(L10N.NoResultSetAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            return _rowDescription[ordinal].Handler.PgName;
        }

        public override Type GetFieldType(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException(L10N.NoResultSetAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            var fieldDescription = _rowDescription[ordinal];
            return fieldDescription.Handler.GetFieldType(fieldDescription);
        }

        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException(L10N.NoResultSetAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            var fieldDescription = _rowDescription[ordinal];
            return fieldDescription.Handler.GetProviderSpecificFieldType(fieldDescription);
        }

        public override object GetValue(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.Ensures(Contract.Result<object>() == DBNull.Value || GetFieldType(ordinal).IsInstanceOfType(Contract.Result<object>()));
            #endregion

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

            // The buffer might not contain the entire column in sequential mode.
            // Handlers of arbitrary-length values handle this internally, reading themselves from the buffer.
            // For simple, primitive type handlers we need to handle this here.
            var result = handler.ReadValueAsObject(_row, fieldDescription);

            if (IsCaching)
            {
                Contract.Assert(cache != null);
                cache.Value = result;
                cache.IsProviderSpecificValue = false;
            }
            return result;
        }

#if NET45
        public override T GetFieldValue<T>(int ordinal)
#else
        public T GetFieldValue<T>(int ordinal)
#endif
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.EndContractBlock();
            #endregion

            var t = typeof(T);
            if (!t.IsArray) {
                return ReadColumn<T>(ordinal);
            }

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            // If the type handler can simply return the requested array, call it as usual. This is the case
            // of reading a string as char[], a bytea as a byte[]...
            var tHandler = handler as ITypeReader<T>;
            if (tHandler != null) {
                return ReadColumn<T>(ordinal);
            }

            // We need to treat this as an actual array type, these need special treatment because of
            // typing/generics reasons
            var elementType = t.GetElementType();
            var arrayHandler = handler as ArrayHandler;
            if (arrayHandler == null) {
                throw new InvalidCastException(String.Format("Can't cast database type {0} to {1}", fieldDescription.Handler.PgName, typeof(T).Name));
            }

            if (arrayHandler.GetElementFieldType(fieldDescription) == elementType)
            {
                return (T)GetValue(ordinal);
            }
            if (arrayHandler.GetElementPsvType(fieldDescription) == elementType)
            {
                return (T)GetProviderSpecificValue(ordinal);
            }
            throw new InvalidCastException(String.Format("Can't cast database type {0} to {1}", handler.PgName, typeof(T).Name));
        }

        public override object GetProviderSpecificValue(int ordinal)
        {
            #region Contracts
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException(String.Format(L10N.MustBeBetweenXAndY, "Column", 0, (FieldCount - 1)));
            Contract.Ensures(Contract.Result<object>() == DBNull.Value || GetProviderSpecificFieldType(ordinal).IsInstanceOfType(Contract.Result<object>()));
            #endregion

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
            // The buffer might not contain the entire column in sequential mode.
            // Handlers of arbitrary-length values handle this internally, reading themselves from the buffer.
            // For simple, primitive type handlers we need to handle this here.
            var result = handler.ReadPsvAsObject(_row, fieldDescription);

            if (IsCaching)
            {
                Contract.Assert(cache != null);
                cache.Value = result;
                cache.IsProviderSpecificValue = true;
            }
            return result;
        }

        public override int GetProviderSpecificValues(object[] values)
        {
            #region Contracts
            if (values == null)
                throw new ArgumentNullException("values");
            if (!IsOnRow)
                throw new InvalidOperationException(L10N.NoRowAvailable);
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= values.Length);
            #endregion

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
            {
                values[i] = GetProviderSpecificValue(i);
            }
            return count;
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }

        /// <summary>
        /// The first row in a stored procedure command that has output parameters needs to be traversed twice -
        /// once for populating the output parameters and once for the actual result set traversal. So in this
        /// case we can't be sequential.
        /// </summary>
        void PopulateOutputParameters()
        {
            // TODO: Should we really use Contract here, instead of throwing an Exception?
            Contract.Requires(_rowDescription != null);
            Contract.Requires(Command.Parameters.Any(p => p.IsOutputDirection));

            while (_row == null)
            {
                var msg = _connector.ReadSingleMessage(DataRowLoadingMode.NonSequential);
                switch (msg.Code)
                {
                    case BackendMessageCode.DataRow:
                        _pendingMessage = msg;
                        _row = (DataRowNonSequentialMessage) msg;
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

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        T ReadColumnWithoutCache<T>(int ordinal)
        {
            _row.SeekToColumnStart(ordinal);
            Row.CheckNotNull();
            var fieldDescription = _rowDescription[ordinal];
            try
            {
                return fieldDescription.Handler.Read<T>(_row, fieldDescription, Row.ColumnLen);
            }
            catch (SafeReadException e)
            {
                throw e.InnerException;
            }
            catch
            {
                _connector.State = ConnectorState.Broken;
                throw;
            }
        }

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the DataReader.
        /// </summary>
        public override DataTable GetSchemaTable()
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            Contract.Ensures(Contract.Result<DataTable>() != null);
            #endregion

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
                row["IsIdentity"] = false;   // TODO
                row["IsAutoIncrement"] = IsAutoIncrement(columnLookup, i);
                row["IsRowVersion"] = false;
                row["IsHidden"] = false;
                row["IsLong"] = false;   // TODO
                row["IsReadOnly"] = false;   // TODO (check for views?)
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

            var lookupKey = string.Format("{0},{1}", _rowDescription[fieldIndex].TableOID, _rowDescription[fieldIndex].ColumnAttributeNumber);
            Column col;
            return !columnLookup.TryGetValue(lookupKey, out col) || !col.NotNull;
        }

        private bool IsAutoIncrement(Dictionary<string, Column> columnLookup, int fieldIndex)
        {
            if (columnLookup == null || _rowDescription[fieldIndex].TableOID == 0)
            {
                return false;
            }

            var lookupKey = string.Format("{0},{1}", _rowDescription[fieldIndex].TableOID, _rowDescription[fieldIndex].ColumnAttributeNumber);
            Column col;
            return
                columnLookup.TryGetValue(lookupKey, out col)
                    ? col.ColumnDefault is string && col.ColumnDefault.ToString().StartsWith("nextval(")
                    : true;
        }

        string GetBaseColumnName(Dictionary<string, Column> columnLookup, int fieldIndex)
        {
            if (columnLookup == null || _rowDescription[fieldIndex].TableOID == 0)
            {
                return GetName(fieldIndex);
            }

            var lookupKey = string.Format("{0},{1}", _rowDescription[fieldIndex].TableOID, _rowDescription[fieldIndex].ColumnAttributeNumber);
            Column col;
            return columnLookup.TryGetValue(lookupKey, out col) ? col.Name : GetName(fieldIndex);
        }

        KeyLookup GetKeys(Int32 tableOid)
        {
            const string getKeys = "select a.attname, ci.relname, i.indisprimary from pg_catalog.pg_class ct, pg_catalog.pg_class ci, pg_catalog.pg_attribute a, pg_catalog.pg_index i WHERE ct.oid=i.indrelid AND ci.oid=i.indexrelid AND a.attrelid=ci.oid AND i.indisunique AND ct.oid = :tableOid order by ci.relname";

            var lookup = new KeyLookup();

            using (var metadataConn = _connection.Clone())
            {
                var c = new NpgsqlCommand(getKeys, metadataConn);
                c.Parameters.Add(new NpgsqlParameter("tableOid", NpgsqlDbType.Integer)).Value = tableOid;

                using (var dr = c.Execute(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
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
            var sb = new StringBuilder("SELECT current_database(), nc.nspname, c.relname, c.oid FROM pg_namespace nc, pg_class c WHERE c.relnamespace = nc.oid AND (c.relkind = 'r' OR c.relkind = 'v') AND c.oid IN (");
            var first = true;
            foreach (var oid in oids)
            {
                if (!first)
                {
                    sb.Append(',');
                }
                sb.Append(oid);
                first = false;
            }
            sb.Append(')');

            using (var connection = _connection.Clone())
            {
                using (var command = new NpgsqlCommand(sb.ToString(), connection))
                {
                    using (var reader = command.Execute(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
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
            public readonly string Name;
            public readonly bool NotNull;
            public readonly uint TableId;
            public readonly short ColumnNum;
            public readonly object ColumnDefault;

            public string Key
            {
                get { return string.Format("{0},{1}", TableId, ColumnNum); }
            }

            public Column(NpgsqlDataReader rdr)
            {
                Name = rdr.GetString(0);
                NotNull = rdr.GetBoolean(1);
                TableId = rdr.GetFieldValue<uint>(2);
                ColumnNum = rdr.GetInt16(3);
                ColumnDefault = rdr.GetValue(4);
            }
        }

        Dictionary<string, Column> GetColumns()
        {
            var sb = new StringBuilder();

            // the column index is used to find data.
            // any changes to the order of the columns needs to be reflected in struct Columns
            sb.Append("SELECT a.attname AS column_name, a.attnotnull AS column_notnull, a.attrelid AS table_id, a.attnum AS column_num, d.adsrc as column_default");
            sb.Append(" FROM pg_attribute a LEFT OUTER JOIN pg_attrdef d ON a.attrelid = d.adrelid AND a.attnum = d.adnum WHERE a.attnum > 0 AND (");
            var first = true;
            for (var i = 0; i < _rowDescription.NumFields; ++i)
            {
                if (_rowDescription[i].TableOID == 0) { continue; }
                if (!first)
                {
                    sb.Append(" OR ");
                }
                sb.AppendFormat("(a.attrelid={0} AND a.attnum={1})", _rowDescription[i].TableOID,
                    _rowDescription[i].ColumnAttributeNumber);
                first = false;
            }
            sb.Append(')');

            // if the loop ended without setting first to false, then there will be no results from the query
            if (first)
            {
                return null;
            }

            using (var connection = _connection.Clone())
            {
                using (var command = new NpgsqlCommand(sb.ToString(), connection))
                {
                    using (var reader = command.Execute(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
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

        #endregion Schema metadata table
    }

    enum ReaderState
    {
        InResult,
        BetweenResults,
        Consumed,
        Closed
    }

    enum ReadResult
    {
        RowRead,
        RowNotRead,
        ReadAgain,
    }
}
