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
using Npgsql.Messages;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    public class NpgsqlDataReader : DbDataReader
    {
        internal NpgsqlCommand Command { get; private set; }
        readonly NpgsqlConnector _connector;
        readonly NpgsqlConnection _connection;
        readonly CommandBehavior _behavior;

        // TODO: Protect with Interlocked?
        internal ReaderState State { get; private set; }

        RowDescriptionMessage _rowDescription;
        DataRowMessage _row;
        int _recordsAffected;
        internal long? LastInsertedOid { get; private set; }

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
        ServerMessage _pendingMessage;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        /// <summary>
        /// In non-sequential mode, contains the cached values already read from the current row
        /// </summary>
        RowCache _rowCache;

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal bool IsSequential { get { return (_behavior & CommandBehavior.SequentialAccess) != 0; } }
        internal bool IsCaching { get { return !IsSequential; } }

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior, RowDescriptionMessage rowDescription = null)
        {
            if (command.CommandType == CommandType.StoredProcedure && (behavior & CommandBehavior.SequentialAccess) != 0) {
                throw new Exception("A StoredProcedure call can't be sequential");
            }
            Contract.EndContractBlock();

            Command = command;
            _connector = command.Connector;
            _connection = command.Connection;
            _behavior = behavior;
            _recordsAffected = -1;
            _rowDescription = rowDescription;
            if (IsCaching)
            {
                _rowCache = new RowCache();
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

        ReadResult ProcessMessage(ServerMessage msg)
        {
            Contract.Requires(msg != null);

            switch (msg.Code)
            {
                case BackEndMessageCode.RowDescription:
                    _rowDescription = (RowDescriptionMessage)msg;
                    return ReadResult.ReadAgain;

                case BackEndMessageCode.DataRow:
                    Contract.Assert(_rowDescription != null);
                    _connector.State = ConnectorState.Fetching;
                    _row = (DataRowMessage)msg;
                    Contract.Assume(_rowDescription.NumFields == _row.NumColumns);
                    if (IsCaching) {
                        _rowCache.Clear();
                    }
                    if (!_readOneRow && Command.CommandType == CommandType.StoredProcedure) {
                        PopulateOutputParameters();
                    }
                    _readOneRow = true;
                    _hasRows = true;
                    return ReadResult.RowRead;

                case BackEndMessageCode.CompletedResponse:
                    var completed = (CommandCompleteMessage) msg;
                    if (completed.RowsAffected.HasValue)
                    {
                        _recordsAffected = _recordsAffected == -1
                            ? completed.RowsAffected.Value
                            : _recordsAffected + completed.RowsAffected.Value;
                    }
                    if (completed.LastInsertedOID.HasValue) {
                        LastInsertedOid = completed.LastInsertedOID;
                    }
                    goto case BackEndMessageCode.EmptyQueryResponse;

                case BackEndMessageCode.EmptyQueryResponse:
                    _row = null;
                    if (!_hasRows.HasValue) {
                        _hasRows = false;
                    }
                    _rowDescription = null;
                    State = ReaderState.BetweenResults;
                    return ReadResult.RowNotRead;

                case BackEndMessageCode.ReadyForQuery:
                    State = ReaderState.Consumed;
                    return ReadResult.RowNotRead;

                case BackEndMessageCode.BindComplete:
                    return ReadResult.ReadAgain;

                default:
                    throw new Exception("Received unexpected backend message of type " + msg.Code);
            }
        }

        #endregion

        #region NextResult

        public override bool NextResult()
        {
            Contract.Ensures(Command.CommandType != CommandType.StoredProcedure || Contract.Result<bool>() == false);

            if (!_readOneRow && Command.CommandType == CommandType.StoredProcedure && Command.Parameters.Any(p => p.IsOutputDirection))
            {
                // We have a stored procedure with output params that haven't yet been populated, read the first data row
                Read();
            }

            switch (State)
            {
                case ReaderState.InResult:
                    if (_row != null) {
                        _row.Consume();
                        _row = null;
                    }

                    // TODO: Duplication with SingleResult handling above
                    var completedMsg = SkipUntil(BackEndMessageCode.CompletedResponse, BackEndMessageCode.EmptyQueryResponse);
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

            ServerMessage msg;
            if ((_behavior & CommandBehavior.SingleResult) != 0)
            {
                Consume();
                return false;
            }

            while (true)
            {
                msg = ReadMessage();
                switch (msg.Code)
                {
                    case BackEndMessageCode.CompletedResponse:
                        // Another completion in a multi-query, process to get affected records and read again
                        ProcessMessage(msg);
                        continue;
                    case BackEndMessageCode.ReadyForQuery:
                        State = ReaderState.Consumed;
                        return false;
                    case BackEndMessageCode.RowDescription:
                        _rowDescription = (RowDescriptionMessage)msg;
                        _hasRows = null;
                        State = ReaderState.InResult;
                        return true;
                }
            }
        }

        #endregion

        ServerMessage ReadMessage()
        {
            if (_pendingMessage != null) {
                var msg = _pendingMessage;
                _pendingMessage = null;
                return msg;
            }
            // The first row in a stored procedure command that has output parameters needs to be traversed twice -
            // once for populating the output parameters and once for the actual result set traversal. So in this
            // case we can't be sequential.
            var sequential = (_behavior & CommandBehavior.SequentialAccess) != 0 && !(
                !_readOneRow &&
                Command.CommandType == CommandType.StoredProcedure &&
                Command.Parameters.Any(p => p.IsOutputDirection)
            );
            return _connector.ReadSingleMessage(sequential);
        }

        ServerMessage SkipUntil(params BackEndMessageCode[] stopAt)
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


        public override int Depth
        {
            get { throw new NotImplementedException(); }
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
            get { return _recordsAffected; }
       }

        public override bool HasRows
        {
            get
            {
                if (_hasRows.HasValue) {
                    return _hasRows.Value;
                }
                while (true)
                {
                    var msg = _connector.ReadSingleMessage((_behavior & CommandBehavior.SequentialAccess) != 0);
                    switch (msg.Code)
                    {
                        case BackEndMessageCode.RowDescription:
                            ProcessMessage(msg);
                            continue;
                        case BackEndMessageCode.DataRow:
                            _pendingMessage = msg;
                            return true;
                        case BackEndMessageCode.CompletedResponse:
                        case BackEndMessageCode.EmptyQueryResponse:
                            _pendingMessage = msg;
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int FieldCount
        {
            get
            {
                return _row == null ? 0 : _rowDescription.NumFields;
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

            if (_row != null)
            {
                _row.Consume();
                _row = null;
            }

            // Skip over the other result sets, processing only CommandCompleted for RecordsAffected
            while (true)
            {
                var msg = SkipUntil(BackEndMessageCode.CompletedResponse, BackEndMessageCode.ReadyForQuery);
                switch (msg.Code)
                {
                    case BackEndMessageCode.CompletedResponse:
                        ProcessMessage(msg);
                        continue;
                    case BackEndMessageCode.ReadyForQuery:
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
            if ((_behavior & CommandBehavior.CloseConnection) != 0) {
                _connection.Close();
            }
            State = ReaderState.Closed;
            _connector.State = ConnectorState.Ready;
            if (ReaderClosed != null) {
                ReaderClosed(this, EventArgs.Empty);
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

        public override bool GetBoolean(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<bool>(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<byte>(ordinal);
        }

        public override char GetChar(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<char>(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<short>(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<int>(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion
            
            return ReadColumn<long>(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<DateTime>(ordinal);
        }

        public override string GetString(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<string>(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<decimal>(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<double>(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<float>(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<Guid>(ordinal);
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override object this[int ordinal]
        {
            get
            {
                #region Contracts
                if (FieldCount == 0)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                if (ordinal < 0 || ordinal >= FieldCount)
                    throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
                Contract.EndContractBlock();
                #endregion

                return GetValue(ordinal);
            }
        }

        #endregion

        #region Provider-specific type getters

        public NpgsqlDate GetDate(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlDate>(ordinal);
        }

        public NpgsqlTime GetTime(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumnWithoutCache<NpgsqlTime>(ordinal);
        }

        public NpgsqlTimeTZ GetTimeTZ(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlTimeTZ>(ordinal);
        }

        public TimeSpan GetTimeSpan(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<TimeSpan>(ordinal);
        }

        public NpgsqlInterval GetInterval(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlInterval>(ordinal);
        }

        public NpgsqlTimeStamp GetTimeStamp(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return ReadColumn<NpgsqlTimeStamp>(ordinal);
        }

        public NpgsqlTimeStampTZ GetTimeStampTZ(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
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
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "dataOffset must be between 0 and Int32.MaxValue");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException("bufferOffset must be between 0 and " + (buffer.Length - 1));
            if (buffer != null && length > buffer.Length - bufferOffset)
                throw new ArgumentException("length must not exceed ", "length");
            Contract.Ensures(Contract.Result<long>() >= 0);
            #endregion

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null) {
                throw new InvalidCastException("GetBytes() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.CheckNotStreaming();
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
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.Ensures(Contract.Result<Stream>() != null);
            #endregion

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null) {
                throw new InvalidCastException("GetStream() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.CheckNotStreaming();
            row.CheckNotNull();
            row.SeekToColumnStart(ordinal);

            row.IsStreaming = true;
            try
            {
                return handler.GetStream(row, fieldDescription);
            }
            catch
            {
                row.IsStreaming = false;
                throw;
            }
        }

        #endregion

        #region Special text getters

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "dataOffset must be between 0 and Int32.MaxValue");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException("bufferOffset must be between 0 and " + (buffer.Length - 1));
            if (buffer != null && length > buffer.Length - bufferOffset)
                throw new ArgumentException("length must not exceed ", "length");
            Contract.Ensures(Contract.Result<long>() >= 0);
            #endregion

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null) {
                throw new InvalidCastException("GetChars() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.CheckNotStreaming();
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
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.Ensures(Contract.Result<TextReader>() != null);
            #endregion

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null)
            {
                throw new InvalidCastException("GetTextReader() not supported for type " + fieldDescription.Name);
            }

            var row = Row;
            row.CheckNotStreaming();
            row.CheckNotNull();
            row.SeekToColumnStart(ordinal);
            row.SeekToColumn(ordinal);

            row.IsStreaming = true;
            try
            {
                return new StreamReader(new ByteaBinaryStream(row));
            }
            catch
            {
                row.IsStreaming = false;
                throw;
            }
        }

        #endregion

        public override bool IsDBNull(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
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
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty", "name");
            Contract.EndContractBlock();
            #endregion

            return _rowDescription.GetFieldIndex(name);
        }

        public override string GetDataTypeName(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return _rowDescription[ordinal].Name;
        }

        public override Type GetFieldType(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return _rowDescription[ordinal].Handler.FieldType;
        }

        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            return _rowDescription[ordinal].Handler.ProviderSpecificFieldType;
        }

        public override object GetValue(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.Ensures(Contract.Result<object>() == DBNull.Value|| GetFieldType(ordinal).IsInstanceOfType(Contract.Result<object>()));
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
            if (_row.Buffer.BytesLeft < _row.ColumnLen && !handler.IsArbitraryLength)
            {
                Contract.Assume(_row.ColumnLen <= _row.Buffer.Size);
                _row.Buffer.Ensure(_row.ColumnLen);
            }
            var result = handler.ReadValueAsObject(_row.Buffer, fieldDescription, _row.ColumnLen);
            _row.PosInColumn += _row.ColumnLen;

            if (IsCaching)
            {
                Contract.Assert(cache != null);
                cache.Value = result;
                cache.IsProviderSpecificValue = false;
            }
            return result;
        }

        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
            Contract.EndContractBlock();
            #endregion

            //return ReadColumn<T>(ordinal);
            var t = typeof(T);
            if (!t.IsArray) {
                return ReadColumn<T>(ordinal);
            }

            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;

            // If the type handler can simply return the requested array, call it as usual. This is the case
            // of reading a bytea as a byte[]
            var tHandler = handler as ITypeHandler<T>;
            if (tHandler != null) {
                return ReadColumn<T>(ordinal);
            }

            // We need to treat this as an actuall array type, these need special treatment because of
            // typing/generics reasons
            var elementType = t.GetElementType();
            var arrayHandler = handler as IArrayHandler;
            if (arrayHandler == null) {
                throw new InvalidCastException(String.Format("Can't cast database type {0} to {1}", fieldDescription.Handler.PgName, typeof(T).Name));
            }

            if (arrayHandler.ElementFieldType == elementType)
            {
                return (T)GetValue(ordinal);
            }
            if (arrayHandler.ElementProviderSpecificFieldType == elementType)
            {
                return (T)GetProviderSpecificValue(ordinal);
            }
            throw new InvalidCastException(String.Format("Can't cast database type {0} to {1}", handler.PgName, typeof(T).Name));
        }

        public override object GetProviderSpecificValue(int ordinal)
        {
            #region Contracts
            if (FieldCount == 0)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            if (ordinal < 0 || ordinal >= FieldCount)
                throw new IndexOutOfRangeException("Column must be between 0 and " + (FieldCount - 1));
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
            if (_row.Buffer.BytesLeft < _row.ColumnLen && !handler.IsArbitraryLength)
            {
                Contract.Assume(_row.ColumnLen <= _row.Buffer.Size);
                _row.Buffer.Ensure(_row.ColumnLen);
            }
            var result = handler.ReadPsvAsObject(_row.Buffer, fieldDescription, _row.ColumnLen);
            _row.PosInColumn += _row.ColumnLen;

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
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void PopulateOutputParameters()
        {
            Contract.Requires(Command.CommandType == CommandType.StoredProcedure);
            Contract.Requires(_row != null);
            Contract.Requires(_rowDescription != null);

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

            var handler = fieldDescription.Handler as ITypeHandler<T>;
            if (handler == null) {
                throw new InvalidCastException(String.Format("Can't cast database type {0} to {1}", fieldDescription.Handler.PgName, typeof (T).Name));
            }
            // The buffer might not contain the entire column in sequential mode.
            // Handlers of arbitrary-length values handle this internally, reading themselves from the buffer.
            // For simple, primitive type handlers we need to handle this here.
            if (_row.Buffer.BytesLeft < _row.ColumnLen && !handler.IsArbitraryLength)
            {
                Contract.Assume(_row.ColumnLen <= _row.Buffer.Size);
                _row.Buffer.Ensure(_row.ColumnLen);
            }
            var result = handler.Read(_row.Buffer, fieldDescription, _row.ColumnLen);
            _row.PosInColumn += _row.ColumnLen;
            return result;
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

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(IsSequential  || _rowCache != null);
            Contract.Invariant(IsCaching     || _rowCache == null);
        }
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
