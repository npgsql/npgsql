// Npgsql.NpgsqlDataReader.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Provides a means of reading a forward-only stream of rows from a PostgreSQL backend.
    /// </summary>
    public partial class NpgsqlDataReader : DbDataReader, IStreamOwner
    {
        internal NpgsqlConnector _connector;
        internal NpgsqlConnection _connection;
        internal DataTable _currentResultsetSchema;
        internal CommandBehavior _behavior;
        internal NpgsqlCommand Command { get; private set; }

        internal Version Npgsql205 = new Version("2.0.5");

        internal bool _isClosed = false;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        internal NpgsqlRowDescription CurrentDescription { get; private set; }
        private NpgsqlRow _currentRow = null;
        private int? _recordsAffected = null;
        private int? _nextRecordsAffected;
        internal long? LastInsertedOID { get; private set; }
        private long? _nextInsertOID = null;
        internal bool _cleanedUp = false;
        private bool _hasRows = false;
        private readonly NpgsqlConnector.NotificationThreadBlock _threadBlock;

        //Unfortunately we sometimes don't know we're going to be dealing with
        //a description until it comes when we look for a row or a message, and
        //we may also need test if we may have rows for HasRows before the first call
        //to Read(), so we need to be able to cache one of each.
        private NpgsqlRowDescription _pendingDescription = null;
        private NpgsqlRow _pendingRow = null;
        private readonly bool _preparedStatement;

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior,
                                  NpgsqlConnector.NotificationThreadBlock threadBlock,
                                  bool preparedStatement = false, NpgsqlRowDescription rowDescription = null)
        {
            _behavior = behavior;
            _connector = command.Connector;
            Command = command;
            _connection = command.Connection;
            command.Connector.CurrentReader = this;
            _threadBlock = threadBlock;
            _preparedStatement = preparedStatement;
            CurrentDescription = rowDescription;
        }

        internal void UpdateOutputParameters()
        {
            if (CurrentDescription != null)
            {
                IEnumerable<NpgsqlParameter> inputOutputAndOutputParams = Command.Parameters.Cast<NpgsqlParameter>()
                    .Where(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output);
                if (!inputOutputAndOutputParams.Any())
                {
                    return;
                }

                NpgsqlRow row = ParameterUpdateRow;
                if (row == null)
                {
                    return;
                }

                Queue<NpgsqlParameter> pending = new Queue<NpgsqlParameter>();
                List<int> taken = new List<int>();
                foreach (NpgsqlParameter p in inputOutputAndOutputParams)
                {
                    int idx = CurrentDescription.TryFieldIndex(p.CleanName);
                    if (idx == -1)
                    {
                        pending.Enqueue(p);
                    }
                    else
                    {
                        p.Value = row.Get(idx);
                        taken.Add(idx);
                    }
                }
                for (int i = 0; pending.Count != 0 && i != row.NumFields; ++i)
                {
                    if (!taken.Contains(i))
                    {
                        pending.Dequeue().Value = row.Get(i);
                    }
                }
            }
        }

        // We always receive a ForwardsOnlyRow, but if we are not
        // doing SequentialAccess we want the flexibility of CachingRow,
        // so here we either return the ForwardsOnlyRow we received, or
        // build a CachingRow from it, as appropriate.
        private NpgsqlRow BuildRow(ForwardsOnlyRow fo)
        {
            if (fo == null)
            {
                return null;
            }
            else
            {
                fo.SetRowDescription(CurrentDescription);

                if ((_behavior & CommandBehavior.SequentialAccess) == CommandBehavior.SequentialAccess)
                {
                    return fo;
                }
                else
                {
                    return new CachingRow(fo);
                }
            }
        }

        private NpgsqlRow ParameterUpdateRow
        {
            get
            {
                NpgsqlRow ret = CurrentRow ?? _pendingRow ?? GetNextRow(false);
                if (ret is ForwardsOnlyRow)
                {
                    ret = _pendingRow = new CachingRow((ForwardsOnlyRow) ret);
                }
                return ret;
            }
        }

        private NpgsqlRow CurrentRow
        {
            get { return _currentRow; }
            set
            {
                if (_currentRow != null)
                {
                    _currentRow.Dispose();
                }
                _currentRow = value;
            }
        }

        internal void CheckHaveRow()
        {
            if (CurrentRow == null)
            {
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        public override Int32 RecordsAffected
        {
            get { return _recordsAffected ?? -1; }
        }

        #region Result traversal

        /// <summary>
        /// Advances the data reader to the next row.
        /// </summary>
        /// <returns>True if the reader was advanced, otherwise false.</returns>
        public override bool Read()
        {
            return ReadInternal();
        }

        /// <summary>
        /// Asynchronous version of <see cref="Read"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with true if the reader
        /// was advanced, otherwise false.</returns>
#if NET45
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
#else
        public Task<bool> ReadAsync(CancellationToken cancellationToken)
#endif
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO: What is the desired behavior when cancelling here?
            // cancellationToken.Register(...)
            return ReadInternalAsync();
        }

#if !NET45
        public Task<bool> ReadAsync()
        {
            return ReadAsync(CancellationToken.None);
        }
#endif

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [GenerateAsync]
        bool ReadInternal()
        {
            try
            {
                //CurrentRow = null;
                return (CurrentRow = GetNextRow(true)) != null;
            }
            catch (IOException)
            {
                Command.Connection.ClearPool();
                throw;
            }
        }

        /// <summary>
        /// Indicates if NpgsqlDatareader has rows to be read.
        /// </summary>
        public override Boolean HasRows
        {
            get
            {
                // Return true even after the last row has been read in this result.
                // the first call to GetNextRow will set _hasRows to true if rows are found.
                return _hasRows || (GetNextRow(false) != null);
            }
        }

        [GenerateAsync]
        private NpgsqlRow GetNextRow(bool clearPending)
        {
            if (_pendingDescription != null)
            {
                return null;
            }
            if (((_behavior & CommandBehavior.SingleRow) != 0 && CurrentRow != null && _pendingDescription == null) ||
                ((_behavior & CommandBehavior.SchemaOnly) != 0))
            {
                if (!clearPending)
                {
                    return null;
                }
                //We should only have one row, and we've already had it. Move to end
                //of recordset.
                CurrentRow = null;
                for (object skip = GetNextServerMessage();
                     skip != null && (_pendingDescription = skip as NpgsqlRowDescription) == null;
                     skip = GetNextServerMessage())
                {
                    if (skip is NpgsqlRow)
                    {
                        (skip as NpgsqlRow).Dispose();
                    }
                }

                return null;
            }
            if (_pendingRow != null)
            {
                NpgsqlRow ret = _pendingRow;
                if (clearPending)
                {
                    _pendingRow = null;
                }
                if (!_hasRows)
                {
                    // when rows are found, store that this result has rows.
                    _hasRows = (ret != null);
                }
                return ret;
            }
            CurrentRow = null;
            object objNext = GetNextServerMessage();
            if (clearPending)
            {
                _pendingRow = null;
            }
            if (objNext is NpgsqlRowDescription)
            {
                _pendingDescription = objNext as NpgsqlRowDescription;
                return null;
            }
            if (!_hasRows)
            {
                // when rows are found, store that this result has rows.
                _hasRows = objNext is NpgsqlRow;
            }
            return objNext as NpgsqlRow;
        }

        /// <summary>
        /// Advances the data reader to the next result, when multiple result sets were returned by the PostgreSQL backend.
        /// </summary>
        /// <returns>True if the reader was advanced, otherwise false.</returns>
        public override bool NextResult()
        {
            if (_preparedStatement) {
                // Prepared statements can never have multiple results.
                return false;
            }
            return NextResultInternal();
        }

        /// <summary>
        /// Asynchronous version of <see cref="NextResult"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with true if the reader
        /// was advanced, otherwise false.</returns>
#if NET45
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
#else
        public Task<bool> NextResultAsync(CancellationToken cancellationToken)
#endif
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO: What is the desired behavior when cancelling here?
            // cancellationToken.Register(...)
            if (_preparedStatement) {
                // Prepared statements can never have multiple results.
                return PGUtil.TaskFromResult(false);
            }
            return NextResultInternalAsync();
        }

#if !NET45
        public Task<bool> NextResultAsync()
        {
            return NextResultAsync(CancellationToken.None);
        }
#endif

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [GenerateAsync]
        internal bool NextResultInternal()
        {
            try
            {
                CurrentRow = null;
                _currentResultsetSchema = null;
                _hasRows = false; // set to false and let the reading code determine if the set has rows.
                return (CurrentDescription = GetNextRowDescription()) != null;
            }
            catch (IOException)
            {
                Command.Connection.ClearPool();
                throw;
            }
        }

        /// <summary>
        /// Iterate through the objects returned through from the server.
        /// If it's a CompletedResponse the rowsaffected count is updated appropriately,
        /// and we iterate again, otherwise we return it (perhaps updating our cache of pending
        /// rows if appropriate).
        /// </summary>
        /// <returns>The next <see cref="IServerMessage"/> we will deal with.</returns>
        [GenerateAsync]
        private IServerMessage GetNextServerMessage(bool cleanup = false)
        {
            if (_cleanedUp)
                return null;
            try
            {
                CurrentRow = null;
                if (_pendingRow != null)
                {
                    _pendingRow.Dispose();
                }
                _pendingRow = null;
                while (true)
                {
                    var msg = _connector.ReadMessage();

                    if (msg is RowReader)
                    {
                        RowReader reader = msg as RowReader;

                        if (cleanup)
                        {
                            // V3 rows can dispose by simply reading MessageLength bytes.
                            reader.Dispose();

                            return reader;
                        }
                        else
                        {
                            return _pendingRow = BuildRow(new ForwardsOnlyRow(reader));
                        }
                    }
                    else if (msg is CompletedResponse)
                    {
                        CompletedResponse cr = msg as CompletedResponse;
                        if (cr.RowsAffected.HasValue)
                        {
                            _nextRecordsAffected = (_nextRecordsAffected ?? 0) + cr.RowsAffected.Value;
                        }
                        _nextInsertOID = cr.LastInsertedOID ?? _nextInsertOID;
                    }
                    // TODO: The check for the three COPY protocol messages slow down our normal
                    // processing. Simply separate the flow (don't use ExecuteNonQuery)
                    else if (msg is ReadyForQueryMsg ||
                             msg is CopyInResponseMsg || msg is CopyOutResponseMsg || msg is CopyDataMsg)
                    {
                        CleanUp(true);
                        return null;                        
                    }
                    else
                    {
                        return msg;
                    }
                }
            }
            catch
            {
                CleanUp(true);
                throw;
            }
        }

        /// <summary>
        /// Advances the data reader to the next result, when multiple result sets were returned by the PostgreSQL backend.
        /// </summary>
        /// <returns>True if the reader was advanced, otherwise false.</returns>
        [GenerateAsync]
        private NpgsqlRowDescription GetNextRowDescription()
        {
            if ((_behavior & CommandBehavior.SingleResult) != 0 && CurrentDescription != null)
            {
                CleanUp(false);
                return null;
            }
            NpgsqlRowDescription rd = _pendingDescription;
            while (rd == null)
            {
                object objNext = GetNextServerMessage();
                if (objNext == null)
                {
                    break;
                }
                if (objNext is NpgsqlRow)
                {
                    (objNext as NpgsqlRow).Dispose();
                }

                rd = objNext as NpgsqlRowDescription;
            }

            _pendingDescription = null;

            // If there were records affected before,  keep track of their values.
            if (_recordsAffected != null)
                _recordsAffected += (_nextRecordsAffected ?? 0);
            else
                _recordsAffected = _nextRecordsAffected;

            _nextRecordsAffected = null;
            LastInsertedOID = _nextInsertOID;
            _nextInsertOID = null;
            return rd;
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }

        #endregion

        #region Cleanup / close

        private void CleanUp(bool finishedMessages)
        {
            lock (this)
            {
                if (_cleanedUp) {
                    return;
                }
                if (!finishedMessages)
                {
                    do
                    {
                        if ((Thread.CurrentThread.ThreadState & (ThreadState.Aborted | ThreadState.AbortRequested)) != 0)
                        {
                            _connection.EmergencyClose();
                            return;
                        }
                    }
                    while (GetNextServerMessage(true) != null);
                }
                _cleanedUp = true;
            }
            _threadBlock.Dispose();
        }

        /// <summary>
        /// Closes the data reader object.
        /// </summary>
        public override void Close()
        {
            CleanUp(false);
            if ((_behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
            {
                _connection.Close();
            }
            _isClosed = true;
            if (ReaderClosed != null) {
                ReaderClosed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override Boolean IsClosed
        {
            get { return _isClosed; }
        }

        #endregion Cleanup / close

        #region Get column data

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// The value of the specified column.
        /// </returns>
        /// <param name="ordinal">The zero-based column ordinal.</param><filterpriority>1</filterpriority>
        public override object GetValue(int ordinal)
        {
            return GetValueInternal(ordinal);
        }

        /// <summary>
        /// Asynchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="ordinal">The type of the value to be returned.</param>
        /// <param name="cancellationToken">Currently not implemented.</param>
        /// <returns></returns>
#if NET45
        public override async Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
#else
        public async Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
#endif
        {
            return (T)await GetValueInternalAsync(ordinal);
        }

#if !NET45
        /// <summary>
        /// Asynchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="ordinal">The type of the value to be returned.</param>
        /// <returns></returns>
        public Task<T> GetFieldValueAsync<T>(int ordinal)
        {
            return GetFieldValueAsync<T>(ordinal, CancellationToken.None);
        }
#endif

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [GenerateAsync]
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

        [GenerateAsync]
        public override object GetProviderSpecificValue(int ordinal)
        {
            if (ordinal < 0 || ordinal >= CurrentDescription.NumFields)
            {
                throw new IndexOutOfRangeException("Column index out of range");
            }

            CheckHaveRow();

            return CurrentRow.Get(ordinal);
        }

        /// <summary>
        /// Gets the value of a column in its native format.
        /// </summary>
        public override Object this[String name]
        {
            get
            {
                return GetValueInternal(GetOrdinal(name));
            }
        }

        /// <summary>
        /// Gets the value of a column as Boolean.
        /// </summary>
        public override Boolean GetBoolean(Int32 i)
        {
            // Should this be done using the GetValue directly and not by converting to String
            // and parsing from there?
            return (Boolean)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Byte.
        /// </summary>
        public override Byte GetByte(Int32 i)
        {
            return (Byte)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Char.
        /// </summary>
        public override Char GetChar(Int32 i)
        {
            //This is an interesting one. In the world of databases we've the idea of chars which is 0 to n characters
            //where n is stated (and can perhaps be infinite) and various variations upon that (postgres is admirable
            //in being relatively consistent and in not generally encouraging limiting n purely for performance reasons,
            //but across many different platforms we'll find such things as text, ntext, char, nchar, varchar, nvarchar,
            //and so on with some platforms not having them all and many implementaiton differences).
            //
            //In the world of .NET, and many other languages, we have the idea of characters and of strings - which are
            //sequences of characters with differing degress of encapsulation from C just having char* through to .NET
            //having full-blown objects
            //
            //Database char, varchar, text, etc. are all generally mapped to strings. There's a bit of a question as to
            //what maps to a .NET char. Interestingly enough, SQLDataReader doesn't support GetChar() and neither do
            //a few other providers (Oracle for example). It would seem that IDataReader.GetChar() was defined largely
            //to have a complete set of .NET base types. Still, the closest thing in the database world to a char value
            //is a char(1) or varchar(1) - that is to say the value of a string of length one, so that's what is used here.
            //
            //In case of the type is "char", we can return the .NET Char directly.
            Object value = GetValueInternal(i);
            if (value is Char)
            {
                return (Char)value;
            }
            String s = (String)value;
            if (s.Length != 1)
            {
                throw new InvalidCastException();
            }
            return s[0];
        }

        /// <summary>
        /// Gets the value of a column as DateTime.
        /// </summary>
        public override DateTime GetDateTime(Int32 i)
        {
            return (DateTime)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column converted to a Guid.
        /// </summary>
        public override Guid GetGuid(Int32 i)
        {
            return (Guid)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Int16.
        /// </summary>
        public override Int16 GetInt16(Int32 i)
        {
            return (Int16)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Int32.
        /// </summary>
        public override Int32 GetInt32(Int32 i)
        {
            return (Int32)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Int64.
        /// </summary>
        public override Int64 GetInt64(Int32 i)
        {
            return (Int64)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Single.
        /// </summary>
        public override Single GetFloat(Int32 i)
        {
            return (Single)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Double.
        /// </summary>
        public override Double GetDouble(Int32 i)
        {
            return (Double)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as String.
        /// </summary>
        public override String GetString(Int32 i)
        {
            return (String)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as Decimal.
        /// </summary>
        public override Decimal GetDecimal(Int32 i)
        {
            return (Decimal)GetValueInternal(i);
        }

        /// <summary>
        /// Gets the value of a column as TimeSpan.
        /// </summary>
        public TimeSpan GetTimeSpan(Int32 i)
        {
            return (TimeSpan)GetValueInternal(i);
        }

        /// <summary>
        /// Get specified field value.
        /// /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public BitString GetBitString(int i)
        {
            object ret = GetValueInternal(i);
            if (ret is bool)
                return new BitString((bool)ret);
            else
                return (BitString)ret;
        }

        /// <summary>
        /// Get the value of a column as a <see cref="NpgsqlInterval"/>.
        /// <remarks>If the differences between <see cref="NpgsqlInterval"/> and <see cref="System.TimeSpan"/>
        /// in handling of days and months is not important to your application, use <see cref="GetTimeSpan(Int32)"/>
        /// instead.</remarks>
        /// </summary>
        /// <param name="i">Index of the field to find.</param>
        /// <returns><see cref="NpgsqlInterval"/> value of the field.</returns>
        public NpgsqlInterval GetInterval(Int32 i)
        {
            return (NpgsqlInterval)GetProviderSpecificValue(i);
        }

        /// <summary>
        /// Get specified field value.
        /// /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NpgsqlTime GetTime(int i)
        {
            return (NpgsqlTime)GetProviderSpecificValue(i);
        }

        /// <summary>
        /// Get specified field value.
        /// /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NpgsqlTimeTZ GetTimeTZ(int i)
        {
            return (NpgsqlTimeTZ)GetProviderSpecificValue(i);
        }

        /// <summary>
        /// Get specified field value.
        /// /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NpgsqlTimeStamp GetTimeStamp(int i)
        {
            return (NpgsqlTimeStamp)GetProviderSpecificValue(i);
        }

        /// <summary>
        /// Get specified field value.
        /// /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NpgsqlTimeStampTZ GetTimeStampTZ(int i)
        {
            return (NpgsqlTimeStampTZ)GetProviderSpecificValue(i);
        }

        /// <summary>
        /// Get specified field value.
        /// /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public NpgsqlDate GetDate(int i)
        {
            return (NpgsqlDate)GetProviderSpecificValue(i);
        }

        /// <summary>
        /// Gets raw data from a column.
        /// </summary>
        public override Int64 GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length)
        {
            return CurrentRow.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Gets raw data from a column.
        /// </summary>
        public override Int64 GetChars(Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length)
        {
            return CurrentRow.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Gets the value of a column in its native format.
        /// </summary>
        public override Object this[Int32 i]
        {
            get { return GetValueInternal(i); }
        }

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <returns>true if the specified column value is equivalent to DBNull otherwise false.</returns>
        public override bool IsDBNull(int ordinal)
        {
            CheckHaveRow();
            return CurrentRow.IsDBNull(ordinal);
        }

        /// <summary>
        /// An asynchronous version of IsDBNull, which gets a value that indicates whether the
        /// column contains non-existent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">Currently not implemented.</param>
        /// <returns>true if the specified column value is equivalent to DBNull otherwise false.</returns>
#if NET45
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
#else
        public Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
#endif
        {
            CheckHaveRow();
            return CurrentRow.IsDBNullAsync(ordinal);
        }

#if !NET45
        /// <summary>
        /// An asynchronous version of IsDBNull, which gets a value that indicates whether the
        /// column contains non-existent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <returns>true if the specified column value is equivalent to DBNull otherwise false.</returns>
        public Task<bool> IsDBNullAsync(int ordinal)
        {
            return IsDBNullAsync(ordinal, CancellationToken.None);
        }
#endif

        /// <summary>
        /// Copy values from each column in the current row into <paramref name="values"/>.
        /// </summary>
        /// <param name="values">Destination for column values.</param>
        /// <returns>The number of column values copied.</returns>
        public override Int32 GetValues(Object[] values)
        {
            return LoadValues(values, GetValue);
        }

        /// <summary>
        /// Copy values from each column in the current row into <paramref name="values"></paramref>.
        /// </summary>
        /// <param name="values">An array appropriately sized to store values from all columns.</param>
        /// <returns>The number of column values copied.</returns>
        public override int GetProviderSpecificValues(object[] values)
        {
            return LoadValues(values, GetProviderSpecificValue);
        }

        private delegate object ValueLoader(int ordinal);
        private int LoadValues(object[] values, ValueLoader getValue)
        {
            CheckHaveRow();

            // Only the number of elements in the array are filled.
            // It's also possible to pass an array with more that FieldCount elements.
            Int32 maxColumnIndex = (values.Length < FieldCount) ? values.Length : FieldCount;

            for (Int32 i = 0; i < maxColumnIndex; i++)
            {
                values[i] = getValue(i);
            }

            return maxColumnIndex;
        }

        #endregion Column data retrieval

        #region Result set metadata

        /// <summary>
        /// Return the column name of the column at index <param name="Index"></param>.
        /// </summary>
        public override String GetName(Int32 Index)
        {
            if (CurrentDescription == null)
            {
                throw new IndexOutOfRangeException(); //Essentially, all indices are out of range.
            }

            return CurrentDescription[Index].Name;
        }

        /// <summary>
        /// Return the data type OID of the column at index <param name="Index"></param>.
        /// </summary>
        /// FIXME: Why this method returns String?
        public String GetDataTypeOID(Int32 Index)
        {
            if (CurrentDescription == null)
            {
                throw new IndexOutOfRangeException(); //Essentially, all indices are out of range.
            }

            return CurrentDescription[Index].TypeOID.ToString();
        }

        internal bool TryGetTypeInfo(int fieldIndex, out NpgsqlBackendTypeInfo backendTypeInfo)
        {
            if (CurrentDescription == null)
            {
                throw new IndexOutOfRangeException(); //Essentially, all indices are out of range.
            }
            return (backendTypeInfo = CurrentDescription[fieldIndex].TypeInfo) != null;
        }

        /// <summary>
        /// Return the data type name of the column at index <param name="Index"></param>.
        /// </summary>
        public override String GetDataTypeName(Int32 Index)
        {
            NpgsqlBackendTypeInfo TI;
            return TryGetTypeInfo(Index, out TI) ? TI.Name : GetDataTypeOID(Index);
        }

        private Type GetFieldType(Int32 Index, Boolean getProviderSpecific)
        {
            if (CurrentDescription == null)
            {
                throw new IndexOutOfRangeException(); //Essentially, all indices are out of range.
            }
            NpgsqlRowDescription.FieldData FD = CurrentDescription[Index];
            NpgsqlBackendTypeInfo TI = CurrentDescription[Index].TypeInfo;
            if (TI == null)
            {
                return typeof(string); //Default type is string.
            }
            return getProviderSpecific ? TI.GetType(FD.TypeModifier) : TI.GetFrameworkType(FD.TypeModifier);
        }

        /// <summary>
        /// Return the data type of the column at index <param name="Index"></param>.
        /// </summary>
        public override Type GetFieldType(Int32 Index)
        {
            return GetFieldType(Index, false);
        }

        /// <summary>
        /// Return the Npgsql specific data type of the column at requested ordinal.
        /// </summary>
        /// <param name="ordinal">column position</param>
        /// <returns>Appropriate Npgsql type for column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            return GetFieldType(ordinal, true);
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override Int32 FieldCount
        {
            get
            {
                // We read msdn documentation and bug report #1010649 that the common return value is 0.
                return CurrentDescription == null ? 0 : CurrentDescription.NumFields;
            }
        }

        /// <summary>
        /// Has ordinal.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool HasOrdinal(string fieldName)
        {
            if (CurrentDescription == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return CurrentDescription.HasOrdinal(fieldName);
        }

        /// <summary>
        /// Return the column name of the column named <param name="Name"></param>.
        /// </summary>
        public override Int32 GetOrdinal(String Name)
        {
            if (CurrentDescription == null)
                throw new InvalidOperationException("Invalid attempt to read when no data is present.");
            return CurrentDescription.FieldIndex(Name);
        }

        /// <summary>
        /// Return the data DbType of the column at index <param name="Index"></param>.
        /// </summary>
        public DbType GetFieldDbType(Int32 Index)
        {
            NpgsqlBackendTypeInfo TI;
            return TryGetTypeInfo(Index, out TI) ? TI.DbType : DbType.String;
        }

        /// <summary>
        /// Return the data NpgsqlDbType of the column at index <param name="Index"></param>.
        /// </summary>
        public NpgsqlDbType GetFieldNpgsqlDbType(Int32 Index)
        {
            NpgsqlBackendTypeInfo TI;
            return TryGetTypeInfo(Index, out TI) ? TI.NpgsqlDbType : NpgsqlDbType.Text;
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.  Always returns zero.
        /// </summary>
        public override Int32 Depth
        {
            get { return 0; }
        }

        #endregion Result set metadata

        #region Schema metadata table

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the DataReader.
        /// </summary>
        public override DataTable GetSchemaTable()
        {
            return _currentResultsetSchema = _currentResultsetSchema ?? GetResultsetSchema();
        }

        private DataTable GetResultsetSchema()
        {
            DataTable result = null;
            if (CurrentDescription != null && CurrentDescription.NumFields > 0)
            {
                result = new DataTable("SchemaTable");

                result.Columns.Add("ColumnName", typeof(string));
                result.Columns.Add("ColumnOrdinal", typeof(int));
                result.Columns.Add("ColumnSize", typeof(int));
                result.Columns.Add("NumericPrecision", typeof(int));
                result.Columns.Add("NumericScale", typeof(int));
                result.Columns.Add("IsUnique", typeof(bool));
                result.Columns.Add("IsKey", typeof(bool));
                result.Columns.Add("BaseCatalogName", typeof(string));
                result.Columns.Add("BaseColumnName", typeof(string));
                result.Columns.Add("BaseSchemaName", typeof(string));
                result.Columns.Add("BaseTableName", typeof(string));
                result.Columns.Add("DataType", typeof(Type));
                result.Columns.Add("AllowDBNull", typeof(bool));
                result.Columns.Add("ProviderType", typeof(string));
                result.Columns.Add("IsAliased", typeof(bool));
                result.Columns.Add("IsExpression", typeof(bool));
                result.Columns.Add("IsIdentity", typeof(bool));
                result.Columns.Add("IsAutoIncrement", typeof(bool));
                result.Columns.Add("IsRowVersion", typeof(bool));
                result.Columns.Add("IsHidden", typeof(bool));
                result.Columns.Add("IsLong", typeof(bool));
                result.Columns.Add("IsReadOnly", typeof(bool));

                FillSchemaTable(result);
            }

            return result;
        }

        private void FillSchemaTable(DataTable schema)
        {
            Dictionary<long, Table> oidTableLookup = new Dictionary<long, Table>();
            KeyLookup keyLookup = new KeyLookup();
            // needs to be null because there is a difference
            // between an empty dictionary and not setting it
            // the default values will be different
            Dictionary<string, Column> columnLookup = null;

            if ((_behavior & CommandBehavior.KeyInfo) == CommandBehavior.KeyInfo)
            {
                List<int> tableOids = new List<int>();
                for (int i = 0; i != CurrentDescription.NumFields; ++i)
                {
                    if (CurrentDescription[i].TableOID != 0 && !tableOids.Contains(CurrentDescription[i].TableOID))
                    {
                        tableOids.Add(CurrentDescription[i].TableOID);
                    }
                }
                oidTableLookup = GetTablesFromOids(tableOids);

                if (oidTableLookup.Count == 1)
                {
                    // only 1, but we can't index into the Dictionary
                    foreach (int key in oidTableLookup.Keys)
                    {
                        keyLookup = GetKeys(key);
                    }
                }

                columnLookup = GetColumns();
            }

            for (Int16 i = 0; i < CurrentDescription.NumFields; i++)
            {
                DataRow row = schema.NewRow();

                string baseColumnName = GetBaseColumnName(columnLookup, i);

                row["ColumnName"] = GetName(i);
                row["ColumnOrdinal"] = i + 1;
                if (CurrentDescription[i].TypeModifier != -1 && CurrentDescription[i].TypeInfo != null &&
                    (CurrentDescription[i].TypeInfo.Name == "varchar" || CurrentDescription[i].TypeInfo.Name == "bpchar"))
                {
                    row["ColumnSize"] = CurrentDescription[i].TypeModifier - 4;
                }
                else if (CurrentDescription[i].TypeModifier != -1 && CurrentDescription[i].TypeInfo != null &&
                         (CurrentDescription[i].TypeInfo.Name == "bit" || CurrentDescription[i].TypeInfo.Name == "varbit"))
                {
                    row["ColumnSize"] = CurrentDescription[i].TypeModifier;
                }
                else
                {
                    row["ColumnSize"] = (int)CurrentDescription[i].TypeSize;
                }
                if (CurrentDescription[i].TypeModifier != -1 && CurrentDescription[i].TypeInfo != null &&
                    CurrentDescription[i].TypeInfo.Name == "numeric")
                {
                    row["NumericPrecision"] = ((CurrentDescription[i].TypeModifier - 4) >> 16) & ushort.MaxValue;
                    row["NumericScale"] = (CurrentDescription[i].TypeModifier - 4) & ushort.MaxValue;
                }
                else
                {
                    row["NumericPrecision"] = 0;
                    row["NumericScale"] = 0;
                }
                row["IsUnique"] = IsUnique(keyLookup, baseColumnName);
                row["IsKey"] = IsKey(keyLookup, baseColumnName);
                if (CurrentDescription[i].TableOID != 0 && oidTableLookup.ContainsKey(CurrentDescription[i].TableOID))
                {
                    row["BaseCatalogName"] = oidTableLookup[CurrentDescription[i].TableOID].Catalog;
                    row["BaseSchemaName"] = oidTableLookup[CurrentDescription[i].TableOID].Schema;
                    row["BaseTableName"] = oidTableLookup[CurrentDescription[i].TableOID].Name;
                }
                else
                {
                    row["BaseCatalogName"] = row["BaseSchemaName"] = row["BaseTableName"] = "";
                }
                row["BaseColumnName"] = baseColumnName;
                row["DataType"] = GetFieldType(i);
                row["AllowDBNull"] = IsNullable(columnLookup, i);
                if (CurrentDescription[i].TypeInfo != null)
                {
                    row["ProviderType"] = CurrentDescription[i].TypeInfo.Name;
                }
                row["IsAliased"] = string.CompareOrdinal((string)row["ColumnName"], baseColumnName) != 0;
                row["IsExpression"] = false;
                row["IsIdentity"] = false;
                row["IsAutoIncrement"] = IsAutoIncrement(columnLookup, i);
                row["IsRowVersion"] = false;
                row["IsHidden"] = false;
                row["IsLong"] = false;
                row["IsReadOnly"] = false;

                schema.Rows.Add(row);
            }
        }

        private static bool IsKey(KeyLookup keyLookup, string fieldName)
        {
            return keyLookup.primaryKey.Contains(fieldName);
        }

        private static bool IsUnique(KeyLookup keyLookup, string fieldName)
        {
            return keyLookup.uniqueColumns.Contains(fieldName);
        }

        private Boolean IsNullable(Dictionary<string, Column> columnLookup, Int32 FieldIndex)
        {
            if (columnLookup == null || CurrentDescription[FieldIndex].TableOID == 0)
            {
                return true;
            }

            string lookupKey = string.Format("{0},{1}", CurrentDescription[FieldIndex].TableOID, CurrentDescription[FieldIndex].ColumnAttributeNumber);
            Column col = null;
            return columnLookup.TryGetValue(lookupKey, out col) ? !col.NotNull : true;
        }

        private bool IsAutoIncrement(Dictionary<string, Column> columnLookup, Int32 FieldIndex)
        {
            if (columnLookup == null || CurrentDescription[FieldIndex].TableOID == 0)
            {
                return false;
            }

            string lookupKey = string.Format("{0},{1}", CurrentDescription[FieldIndex].TableOID, CurrentDescription[FieldIndex].ColumnAttributeNumber);
            Column col = null;
            return
                columnLookup.TryGetValue(lookupKey, out col)
                    ? col.ColumnDefault is string && col.ColumnDefault.ToString().StartsWith("nextval(")
                    : true;
        }

        private string GetBaseColumnName(Dictionary<string, Column> columnLookup, Int32 FieldIndex)
        {
            if (columnLookup == null || CurrentDescription[FieldIndex].TableOID == 0)
            {
                return GetName(FieldIndex);
            }

            string lookupKey = string.Format("{0},{1}", CurrentDescription[FieldIndex].TableOID, CurrentDescription[FieldIndex].ColumnAttributeNumber);
            Column col = null;
            return columnLookup.TryGetValue(lookupKey, out col) ? col.Name : GetName(FieldIndex);
        }

        private KeyLookup GetKeys(Int32 tableOid)
        {
            string getKeys =
                "select a.attname, ci.relname, i.indisprimary from pg_catalog.pg_class ct, pg_catalog.pg_class ci, pg_catalog.pg_attribute a, pg_catalog.pg_index i WHERE ct.oid=i.indrelid AND ci.oid=i.indexrelid AND a.attrelid=ci.oid AND i.indisunique AND ct.oid = :tableOid order by ci.relname";

            KeyLookup lookup = new KeyLookup();

            using (NpgsqlConnection metadataConn = _connection.Clone())
            {
                NpgsqlCommand c = new NpgsqlCommand(getKeys, metadataConn);
                c.Parameters.Add(new NpgsqlParameter("tableOid", NpgsqlDbType.Integer)).Value = tableOid;

                using (NpgsqlDataReader dr = c.GetReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult))
                {
                    string previousKeyName = null;
                    string possiblyUniqueColumn = null;
                    string columnName;
                    string currentKeyName;
                    // loop through adding any column that is primary to the primary key list
                    // add any column that is the only column for that key to the unique list
                    // unique here doesn't mean general unique constraint (with possibly multiple columns)
                    // it means all values in this single column must be unique
                    while (dr.ReadInternal())
                    {
                        columnName = dr.GetString(0);
                        currentKeyName = dr.GetString(1);
                        // if i.indisprimary
                        if (dr.GetBoolean(2))
                        {
                            // add column name as part of the primary key
                            lookup.primaryKey.Add(columnName);
                        }
                        if (currentKeyName != previousKeyName)
                        {
                            if (possiblyUniqueColumn != null)
                            {
                                lookup.uniqueColumns.Add(possiblyUniqueColumn);
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
                        lookup.uniqueColumns.Add(possiblyUniqueColumn);
                    }
                    return lookup;
                }
            }
        }

        private class KeyLookup
        {
            /// <summary>
            /// Contains the column names as the keys
            /// </summary>
            public readonly List<string> primaryKey = new List<string>();

            /// <summary>
            /// Contains all unique columns
            /// </summary>
            public readonly List<string> uniqueColumns = new List<string>();
        }

        private struct Table
        {
            public readonly string Catalog;
            public readonly string Schema;
            public readonly string Name;
            public readonly int Id;

            public Table(IDataReader rdr)
            {
                Catalog = rdr.GetString(0);
                Schema = rdr.GetString(1);
                Name = rdr.GetString(2);
                Id = rdr.GetInt32(3);
            }
        }

        private Dictionary<long, Table> GetTablesFromOids(List<int> oids)
        {
            if (oids.Count == 0)
            {
                return new Dictionary<long, Table>(); //Empty collection is simpler than requiring tests for null;
            }

            // the column index is used to find data.
            // any changes to the order of the columns needs to be reflected in struct Tables
            StringBuilder sb =
                new StringBuilder(
                    "SELECT current_database(), nc.nspname, c.relname, c.oid FROM pg_namespace nc, pg_class c WHERE c.relnamespace = nc.oid AND (c.relkind = 'r' OR c.relkind = 'v') AND c.oid IN (");
            bool first = true;
            foreach (int oid in oids)
            {
                if (!first)
                {
                    sb.Append(',');
                }
                sb.Append(oid);
                first = false;
            }
            sb.Append(')');

            using (NpgsqlConnection connection = _connection.Clone())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sb.ToString(), connection))
                {
                    using (NpgsqlDataReader reader = command.GetReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult)
                        )
                    {
                        Dictionary<long, Table> oidLookup = new Dictionary<long, Table>(oids.Count);
                        int columnCount = reader.FieldCount;
                        while (reader.ReadInternal())
                        {
                            Table t = new Table(reader);
                            oidLookup.Add(t.Id, t);
                        }
                        return oidLookup;
                    }
                }
            }
        }

        private class Column
        {
            public readonly string Name;
            public readonly bool NotNull;
            public readonly int TableId;
            public readonly short ColumnNum;
            public readonly object ColumnDefault;

            public string Key
            {
                get { return string.Format("{0},{1}", TableId, ColumnNum); }
            }

            public Column(IDataReader rdr)
            {
                Name = rdr.GetString(0);
                NotNull = rdr.GetBoolean(1);
                TableId = rdr.GetInt32(2);
                ColumnNum = rdr.GetInt16(3);
                ColumnDefault = rdr.GetValue(4);
            }
        }

        private Dictionary<string, Column> GetColumns()
        {
            StringBuilder sb = new StringBuilder();

            // the column index is used to find data.
            // any changes to the order of the columns needs to be reflected in struct Columns
            sb.Append(
                "SELECT a.attname AS column_name, a.attnotnull AS column_notnull, a.attrelid AS table_id, a.attnum AS column_num, d.adsrc as column_default");
            sb.Append(
                " FROM pg_attribute a LEFT OUTER JOIN pg_attrdef d ON a.attrelid = d.adrelid AND a.attnum = d.adnum WHERE a.attnum > 0 AND (");
            bool first = true;
            for (int i = 0; i < CurrentDescription.NumFields; ++i)
            {
                if (CurrentDescription[i].TableOID != 0)
                {
                    if (!first)
                    {
                        sb.Append(" OR ");
                    }
                    sb.AppendFormat("(a.attrelid={0} AND a.attnum={1})", CurrentDescription[i].TableOID,
                                    CurrentDescription[i].ColumnAttributeNumber);
                    first = false;
                }
            }
            sb.Append(')');

            // if the loop ended without setting first to false, then there will be no results from the query
            if (first)
            {
                return null;
            }

            using (NpgsqlConnection connection = _connection.Clone())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sb.ToString(), connection))
                {
                    using (NpgsqlDataReader reader = command.GetReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult)
                        )
                    {
                        Dictionary<string, Column> columnLookup = new Dictionary<string, Column>();
                        while (reader.ReadInternal())
                        {
                            Column column = new Column(reader);
                            columnLookup.Add(column.Key, column);
                        }
                        return columnLookup;
                    }
                }
            }
        }

        #endregion Schema metadata table

        #region Unused - to be removed

        private static Boolean IsKey(String ColumnName, IEnumerable<string> ListOfKeys)
        {
            foreach (String s in ListOfKeys)
            {
                if (s == ColumnName)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<string> GetPrimaryKeys(String tablename)
        {
            if (string.IsNullOrEmpty(tablename))
            {
                yield break;
            }

            String getPKColumns =
                "select a.attname from pg_catalog.pg_class ct, pg_catalog.pg_class ci, pg_catalog.pg_attribute a, pg_catalog.pg_index i  WHERE ct.oid=i.indrelid AND ci.oid=i.indexrelid  AND a.attrelid=ci.oid AND i.indisprimary AND ct.relname = :tablename";

            using (NpgsqlConnection metadataConn = _connection.Clone())
            {
                using (NpgsqlCommand c = new NpgsqlCommand(getPKColumns, metadataConn))
                {
                    c.Parameters.Add(new NpgsqlParameter("tablename", NpgsqlDbType.Text));
                    c.Parameters["tablename"].Value = tablename;

                    using (NpgsqlDataReader dr = c.GetReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                    {
                        while (dr.ReadInternal())
                        {
                            yield return dr.GetString(0);
                        }
                    }
                }
            }
        }

        ///<summary>
        /// This methods parses the command text and tries to get the tablename
        /// from it.
        ///</summary>
        private String GetTableNameFromQuery()
        {
            Int32 fromClauseIndex = Command.CommandText.ToLowerInvariant().IndexOf("from");

            String tableName = Command.CommandText.Substring(fromClauseIndex + 4).Trim();

            if (string.IsNullOrEmpty(tableName))// == String.Empty)
            {
                return String.Empty;
            }

            /*if (tableName.EndsWith("."));
                return String.Empty;
              */
            foreach (Char c in tableName.Substring(0, tableName.Length - 1))
            {
                if (!Char.IsLetterOrDigit(c) && c != '_' && c != '.')
                {
                    return String.Empty;
                }
            }

            return tableName;
        }

        #endregion Unused - to be removed
    }
}
