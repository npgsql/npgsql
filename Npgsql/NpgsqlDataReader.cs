using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
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

namespace Npgsql
{
    public class NpgsqlDataReader : DbDataReader
    {
        internal NpgsqlCommand Command { get; private set; }
        NpgsqlConnector _connector;
        NpgsqlConnection _connection;
        CommandBehavior _behavior;

        // TODO: Protect with Interlocked?
        internal ReaderState State { get; private set; }

        RowDescriptionMessage _rowDescription;
        DataRowMessageBase _row;
        int _recordsAffected;
        internal long? LastInsertedOID { get; private set; }

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
        IServerMessage _pendingMessage;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior, RowDescriptionMessage rowDescription = null)
        {
            Command = command;
            _connector = command.Connector;
            _connection = command.Connection;
            _behavior = behavior;
            _rowDescription = rowDescription;
            _recordsAffected = -1;
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

        ReadResult ProcessMessage(IServerMessage msg)
        {
            switch (msg.Code)
            {
                case BackEndMessageCode.RowDescription:
                    _rowDescription = (RowDescriptionMessage)msg;
                    return ReadResult.ReadAgain;

                case BackEndMessageCode.DataRow:
                    if (_rowDescription == null) {
                        throw new Exception("Got DataRow but have no RowDescription");
                    }
                    _row = (DataRowMessageBase)msg;
                    _row.Description = _rowDescription;
                    _readOneRow = true;
                    _hasRows = true;
                    _connector.State = ConnectorState.Fetching;
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
                        LastInsertedOID = completed.LastInsertedOID;
                    }
                    goto case BackEndMessageCode.EmptyQueryResponse;

                case BackEndMessageCode.EmptyQueryResponse:
                    _row = null;
                    if (!_hasRows.HasValue) {
                        _hasRows = false;
                    }
                    _rowDescription = null;
                    State = ReaderState.BetweenResults;
                    return ReadResult.ReadAgain;

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
            switch (State)
            {
                case ReaderState.InResult:
                    if (_row != null)
                    {
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

            Debug.Assert(State == ReaderState.BetweenResults);

            IServerMessage msg;
            if ((_behavior & CommandBehavior.SingleResult) != 0)
            {
                Consume();
                return false;
            }

            msg = ReadMessage();
            if (msg is ReadyForQueryMessage) {
                State = ReaderState.Consumed;
                return false;
            }
            Debug.Assert(msg is RowDescriptionMessage);
            _rowDescription = (RowDescriptionMessage)msg;
            _hasRows = null;
            State = ReaderState.InResult;
            return true;
        }

        #endregion

        IServerMessage ReadMessage()
        {
            if (_pendingMessage != null) {
                var msg = _pendingMessage;
                _pendingMessage = null;
                return msg;
            }
            return _connector.ReadSingleMessage((_behavior & CommandBehavior.SequentialAccess) != 0);
        }

        IServerMessage SkipUntil(params BackEndMessageCode[] stopAt)
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
                // We read msdn documentation and bug report #1010649 that the common return value is 0.
                return _rowDescription == null ? 0 : _rowDescription.NumFields;
            }
        }

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

        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        void CheckHasRow()
        {
            if (_row == null) {
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            }            
        }

        #region Value getters

        public override bool GetBoolean(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Boolean;
        }

        public override byte GetByte(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Byte;
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Int16;
        }

        public override int GetInt32(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Int32;
        }

        public override long GetInt64(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Int64;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).String;
        }

        public override decimal GetDecimal(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Decimal;
        }

        public override double GetDouble(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Double;
        }

        public override float GetFloat(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).Float;
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override object this[int ordinal]
        {
            get
            {
                CheckHasRow();
                return _row.Get(ordinal).Object;
            }
        }

#if OLD
        public override object this[int ordinal]
        {
            get { return GetValueInternal(ordinal); }
        }

        object GetValueInternal(int ordinal)
        {
            object providerValue = GetProviderSpecificValue(ordinal);
            NpgsqlBackendTypeInfo backendTypeInfo;
            if (Command.ExpectedTypes != null && Command.ExpectedTypes.Length > ordinal && Command.ExpectedTypes[ordinal] != null)
            {
                return ExpectedTypeConverter.ChangeType(providerValue, Command.ExpectedTypes[ordinal]);
            }
            else if ((_connection == null || !_connection.UseExtendedTypes) && TryGetTypeInfo(ordinal, out backendTypeInfo))
                return backendTypeInfo.ConvertToFrameworkType(providerValue);
            return providerValue;
        }

        public override object GetProviderSpecificValue(int ordinal)
        {
            throw new NotImplementedException();
            var field = SeekToColumn(ordinal);
            var len = _row.Buffer.ReadInt32();

            if (field.FormatCode == FormatCode.Text)
            {
                //return
                    //NpgsqlTypesHelper.ConvertBackendStringToSystemType(field.TypeInfo, _row.Buffer, len,
                    //                                                   field.TypeModifier);
            }
            else
            {
                //return
                    //NpgsqlTypesHelper.ConvertBackendBytesToSystemType(field.TypeInfo, _row.Buffer, len,
                    //                                                  field.TypeModifier);
            }
        }

        internal bool TryGetTypeInfo(int fieldIndex, out NpgsqlBackendTypeInfo backendTypeInfo)
        {
            if (_rowDescription == null)
            {
                throw new IndexOutOfRangeException(); //Essentially, all indices are out of range.
            }
            return (backendTypeInfo = _rowDescription[fieldIndex].TypeInfo) != null;
        }
#endif

        #endregion

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            CheckHasRow();

            if (dataOffset < 0 || dataOffset > int.MaxValue) {
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "dataOffset must be between 0 and Int32.MaxValue");
            }

            return _row.GetBytes(ordinal, (int)dataOffset, buffer, bufferOffset, length);
        }

#if NET45
        public override Stream GetStream(int ordinal)
#else
        public Stream GetStream(int ordinal)
#endif
        {
            CheckHasRow();
            return _row.GetStream(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

#if NET45
        public override TextReader GetTextReader(int ordinal)
#else
        public TextReader GetTextReader(int ordinal)
#endif
        {
            throw new NotImplementedException();
        }

        #region Non-standard value getters

        public NpgsqlDate GetDate(int ordinal)
        {
            throw new NotImplementedException();
        }

        public NpgsqlTime GetTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public NpgsqlTimeTZ GetTimeTZ(int ordinal)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetTimeSpan(int ordinal)
        {
            throw new NotImplementedException();
        }

        public NpgsqlTimeStamp GetTimeStamp(int ordinal)
        {
            throw new NotImplementedException();
        }

        public NpgsqlTimeStampTZ GetTimeStampTZ(int ordinal)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override bool IsDBNull(int ordinal)
        {
            CheckHasRow();
            return _row.Get(ordinal).IsNull;
        }

        public override object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            return this[ordinal];
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
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
