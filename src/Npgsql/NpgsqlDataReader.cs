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
using Npgsql.TypeHandling;
using NpgsqlTypes;

#pragma warning disable CA2222 // Do not decrease inherited member visibility

namespace Npgsql
{
    /// <summary>
    /// Reads a forward-only stream of rows from a data source.
    /// </summary>
#pragma warning disable CA1010
    public abstract class NpgsqlDataReader : DbDataReader
#pragma warning restore CA1010
#if NETSTANDARD1_3
        , IDbColumnSchemaGenerator
#endif
    {
        internal NpgsqlCommand Command { get; }
        internal readonly NpgsqlConnector Connector;
        readonly NpgsqlConnection _connection;
        readonly CommandBehavior _behavior;
        readonly Task _sendTask;

        internal ReaderState State;

        internal NpgsqlReadBuffer Buffer;

        /// <summary>
        /// Holds the list of statements being executed by this reader.
        /// </summary>
        readonly List<NpgsqlStatement> _statements;

        /// <summary>
        /// The index of the current query resultset we're processing (within a multiquery)
        /// </summary>
        internal int StatementIndex { get; private set; }

        /// <summary>
        /// For streaming types (e.g. bytea), holds the byte length of the column.
        /// Does not include the length prefix.
        /// </summary>
        internal int ColumnLen;

        internal int PosInColumn;

        int _charPos;

        /// <summary>
        /// The RowDescription message for the current resultset being processed
        /// </summary>
        [CanBeNull]
        internal RowDescriptionMessage RowDescription;

        uint? _recordsAffected;

        /// <summary>
        /// Whether the current result set has rows
        /// </summary>
        bool _hasRows;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public event EventHandler ReaderClosed;

        bool IsSchemaOnly => (_behavior & CommandBehavior.SchemaOnly) != 0;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal NpgsqlDataReader(NpgsqlCommand command, CommandBehavior behavior, List<NpgsqlStatement> statements, Task sendTask)
        {
            Command = command;
            _connection = command.Connection;
            Connector = _connection.Connector;
            _behavior = behavior;
            _statements = statements;
            StatementIndex = -1;
            _sendTask = sendTask;
            State = ReaderState.BetweenResults;
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
        {
            using (NoSynchronizationContextScope.Enter())
                return Read(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                if ((_behavior & CommandBehavior.SingleRow) != 0)
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
                var msg = await ReadMessage(async);
                ProcessMessage(msg);
                return msg.Code == BackendMessageCode.DataRow;
            }
            catch (PostgresException)
            {
                State = ReaderState.Consumed;
                throw;
            }
        }

        void ProcessMessage(IBackendMessage msg)
        {
            Debug.Assert(msg != null);

            switch (msg.Code)
            {
            case BackendMessageCode.DataRow:
                Debug.Assert(RowDescription != null);
                if (Connector.State != ConnectorState.Fetching)
                    Connector.State = ConnectorState.Fetching;
                ProcessDataMessage((DataRowMessage)msg);
                _hasRows = true;
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
                return;

            case BackendMessageCode.CompletedResponse:
                var completed = (CommandCompleteMessage) msg;
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

        internal abstract ValueTask<IBackendMessage> ReadMessage(bool async);
        internal abstract void ProcessDataMessage(DataRowMessage dataMsg);
        internal abstract Task SeekToColumn(int column, bool async);
        internal abstract Task SeekInColumn(int posInColumn, bool async);
        internal abstract ValueTask<Stream> GetStreamInternal(int column, bool async);
        internal abstract Task ConsumeRow(bool async);

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
                State = ReaderState.Consumed;
                if ((StatementIndex >= 0) && (StatementIndex < _statements.Count))
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
            try
            {
                using (NoSynchronizationContextScope.Enter())
                    return IsSchemaOnly ? NextResultSchemaOnly(true) : NextResult(true);
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
        protected virtual async Task<bool> NextResult(bool async)
        {
            IBackendMessage msg;
            Debug.Assert(!IsSchemaOnly);

            // If we're in the middle of a resultset, consume it
            switch (State)
            {
            case ReaderState.BeforeResult:
            case ReaderState.InResult:
                await ConsumeRow(async);
                var completedMsg = await Connector.SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.EmptyQueryResponse, async);
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
            _hasRows = false;

            if ((_behavior & CommandBehavior.SingleResult) != 0 && StatementIndex == 0)
            {
                if (State == ReaderState.BetweenResults)
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
                    await Connector.ReadExpecting<BindCompleteMessage>(async);
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
                            await Connector.ReadExpecting<CloseCompletedMessage>(async);
                            pStatement.StatementBeingReplaced.CompleteUnprepare();
                            pStatement.StatementBeingReplaced = null;
                        }
                    }

                    await Connector.ReadExpecting<ParseCompleteMessage>(async);
                    await Connector.ReadExpecting<BindCompleteMessage>(async);
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

                    if (pStatement != null)
                    {
                        Debug.Assert(!pStatement.IsPrepared);
                        pStatement.CompletePrepare();
                    }
                }

                msg = await ReadMessage(async);
                if (RowDescription == null)
                {
                    // Statement did not generate a resultset (e.g. INSERT)
                    // Read and process its completion message and move on to the next statement

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

                switch (msg.Code)
                {
                case BackendMessageCode.DataRow:
                case BackendMessageCode.CompletedResponse:
                    break;
                default:
                    throw Connector.UnexpectedMessageReceived(msg.Code);
                }

                ProcessMessage(msg);
                return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            ProcessMessage(Connector.ReadExpecting<ReadyForQueryMessage>());
            RowDescription = null;
            return false;
        }

        /// <summary>
        /// Note that in SchemaOnly mode there are no resultsets, and we read nothing from the backend (all
        /// RowDescriptions have already been processed and are available)
        /// </summary>
        async Task<bool> NextResultSchemaOnly(bool async)
        {
            Debug.Assert(IsSchemaOnly);

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
                    await Connector.ReadExpecting<ParseCompleteMessage>(async);
                    await Connector.ReadExpecting<ParameterDescriptionMessage>(async);
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
                ProcessMessage(await Connector.ReadExpecting<ReadyForQueryMessage>(async));
                RowDescription = null;
            }
            return false;
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
        public override string GetName(int ordinal)
        {
            CheckResultSet();
            CheckColumn(ordinal);

            return RowDescription[ordinal].Name;
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override int FieldCount => RowDescription?.NumFields ?? 0;

        #region Cleanup / Dispose

        /// <summary>
        /// Consumes all result sets for this reader, leaving the connector ready for sending and processing further
        /// queries
        /// </summary>
        async Task Consume(bool async)
        {
            if (IsSchemaOnly && _statements.All(s => s.IsPrepared))
            {
                State = ReaderState.Consumed;
                return;
            }

            switch (State)
            {
            case ReaderState.BeforeResult:
            case ReaderState.InResult:
                await ConsumeRow(async);
                break;
            }

            // Skip over the other result sets, processing only CommandCompleted for RecordsAffected
            while (true)
            {
                var msg = await Connector.SkipUntil(BackendMessageCode.CompletedResponse, BackendMessageCode.ReadyForQuery, async);
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
        public override long GetBytes(int ordinal, long dataOffset, [CanBeNull] byte[] buffer, int bufferOffset, int length)
        {
            CheckRowAndOrdinal(ordinal);
            if (dataOffset < 0 || dataOffset > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
            if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length))
                throw new IndexOutOfRangeException($"bufferOffset must be between {0} and {(buffer.Length - 1)}");
            if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
                throw new IndexOutOfRangeException($"length must be between {0} and {buffer.Length - bufferOffset}");

            var fieldDescription = RowDescription[ordinal];
            var handler = fieldDescription.Handler;
            if (!(handler is ByteaHandler || handler is PostgisGeometryHandler))
                throw new InvalidCastException("GetBytes() not supported for type " + fieldDescription.Name);

            SeekToColumn(ordinal, false).GetAwaiter().GetResult();
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");
            if (buffer == null)
                return ColumnLen;

            var offset = (int)dataOffset;
            SeekInColumn(offset, false).GetAwaiter().GetResult();

            // Attempt to read beyond the end of the column
            if (offset + length > ColumnLen)
                length = ColumnLen - offset;

            return Buffer.ReadAllBytes(buffer, bufferOffset, length, false, false).Result;
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
            => SynchronizationContextSwitcher.NoContext(async () => await GetStream(ordinal, true));

        ValueTask<Stream> GetStream(int ordinal, bool async)
        {
            CheckRowAndOrdinal(ordinal);

            var fieldDescription = RowDescription[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null)
                throw new InvalidCastException($"GetStream() not supported for type {fieldDescription.Handler.PgDisplayName}");

            return GetStreamInternal(ordinal, async);
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

            var fieldDescription = RowDescription[ordinal];
            var handler = fieldDescription.Handler as TextHandler;
            if (handler == null)
                throw new InvalidCastException("GetChars() not supported for type " + fieldDescription.Name);

            SeekToColumn(ordinal, false).GetAwaiter().GetResult();
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");
            if (PosInColumn == 0)
                _charPos = 0;

            if (buffer == null)
            {
                // Note: Getting the length of a text column means decoding the entire field,
                // very inefficient and also consumes the column in sequential mode. But this seems to
                // be SqlClient's behavior as well.
                Buffer.SkipChars(int.MaxValue, ColumnLen - PosInColumn, out var bytesSkipped, out var charsSkipped);
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
                int bytesSkipped, charsSkipped;
                Buffer.SkipChars(charsToSkip, ColumnLen - PosInColumn, out bytesSkipped, out charsSkipped);
                PosInColumn += bytesSkipped;
                _charPos += charsSkipped;
                if (charsSkipped < charsToSkip)
                {
                    // TODO: What is the actual required behavior here?
                    throw new IndexOutOfRangeException();
                }
            }

            Buffer.ReadAllChars(buffer, bufferOffset, length, ColumnLen - PosInColumn, out var bytesRead, out var charsRead);
            PosInColumn += bytesRead;
            _charPos += charsRead;
            return charsRead;
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
            => SynchronizationContextSwitcher.NoContext(async () => await GetTextReader(ordinal, true));

        async ValueTask<TextReader> GetTextReader(int ordinal, bool async)
        {
            CheckRowAndOrdinal(ordinal);

            var fieldDescription = RowDescription[ordinal];
            var handler = fieldDescription.Handler as ITextReaderHandler;
            if (handler == null)
                throw new InvalidCastException($"GetTextReader() not supported for type {fieldDescription.Handler.PgDisplayName}");

            var stream = async
                ? await GetStreamInternal(ordinal, async)
                : GetStreamInternal(ordinal, async).Result;

            return handler.GetTextReader(stream);
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

            return RowDescription.GetFieldIndex(name);
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
            CheckColumn(ordinal);

            return RowDescription[ordinal].PostgresType;
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
            CheckColumn(ordinal);

            return RowDescription[ordinal].TypeOID;
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
            CheckColumn(ordinal);

            var type = Command.ObjectResultTypes?[ordinal];
            return type ?? RowDescription[ordinal].FieldType;
        }

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The Type object that describes the data type of the specified column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            CheckResultSet();
            CheckColumn(ordinal);

            var fieldDescription = RowDescription[ordinal];
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

        #region New (CoreCLR) schema API

        /// <summary>
        /// Returns schema information for the columns in the current resultset.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema()
            => new DbColumnSchemaGenerator(_connection, RowDescription, (_behavior & CommandBehavior.KeyInfo) != 0)
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
            table.Columns.Add("ProviderType", typeof(Type));
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
                row["DataType"] = row["ProviderType"] = column.DataType; // Non-standard
                row["AllowDBNull"] = (object)column.AllowDBNull ?? DBNull.Value;
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

#endif
        #endregion Schema metadata table

        #region Checks

        internal void CheckRowAndOrdinal(int ordinal)
        {
            CheckRow();
            CheckColumn(ordinal);
        }

        void CheckRow()
        {
            if (!IsOnRow)
                throw new InvalidOperationException("No row is available");
        }

        // ReSharper disable once UnusedParameter.Local
        internal void CheckColumn(int column)
        {
            if (column < 0 || column >= FieldCount)
                throw new IndexOutOfRangeException($"Column must be between {0} and {(FieldCount - 1)}");
        }

        void CheckResultSet()
        {
            if (FieldCount == 0)
                throw new InvalidOperationException("No resultset is currently being traversed");
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
