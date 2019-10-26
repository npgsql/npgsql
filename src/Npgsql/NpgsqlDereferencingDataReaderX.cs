#if false
//#define TEST_WRAPPING_ONLY
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Cursor dereferencing data reader derived originally from removed Npgsql v2 code (see
    /// https://github.com/npgsql/npgsql/issues/438); but now with safer, more consistent behaviour.
    /// </summary>
    /// <remarks>
    /// Does not aim to do the actual reading work, rather, it wraps the creation of as many real instances
    /// of <see cref="NpgsqlDataReader"/> as are required to read from the cursors in the original
    /// result set.
    ///
    /// No longer FETCH ALL by default as this is dangerous for large result sets (http://stackoverflow.com/q/42292341/);
    /// FETCH ALL can be enabled with DereferenceFetchSize=-1 and will be more efficient (less round trips) for small to
    /// medium result sets.
    /// 
    /// Works for multiple cursors in any arrangement in the original result set (n x 1; 1 x m; n x m).
    /// 
    /// c.f. Oracle:
    ///  - Oracle always does the equivalent of this automatic dereferencing in its ADO.NET driver
    ///  - Since it's always on in Oracle, you can never read a cursor from a result set directly; however you can
    ///    still code additonal SQL with output parameters in order to get at the original cursors if you need to
    ///  - In Oracle, if just some values are cursors then these are dereferenced and other data is ignored
    ///  - The rest of Oracle's pattern is to only ever try to dereference on Query and Scalar, never on Execute
    ///  - We copy these latter behaviours since presumably: a) they work in practice, and b) this will be the most
    ///    useful thing to do for any cross-DB developers
    /// </remarks>
    public sealed class NpgsqlDereferencingDataReaderX : NpgsqlDataReader
    {
        private NpgsqlDataReader _originalReader;

#if !TEST_WRAPPING_ONLY
        private CommandBehavior _behavior;

        // internally to this class, zero or any negative value will FETCH ALL; externally in settings, -1 is the currently chosen trigger value
        private int _fetchSize;
#endif

        // current FETCH reader
        private NpgsqlDataReader _wrappedReader = default!;

#if !TEST_WRAPPING_ONLY
        // # read on current FETCH
        private int Count;

        // cursors to dereference
        private readonly List<string> Cursors = new List<string>();

        // current cursor index
        private int CursorIndex = 0;

        // current cursor
        private string? Cursor = null;
#endif

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlDereferencingDataReaderX));

        /// <summary>
        /// Create a safe, sensible dereferencing reader; <see cref="CanDereference"/> has already been called to check
        /// that there are at least some cursors to dereference before this constructor is called.
        /// </summary>
        /// <param name="reader">The original reader for the undereferenced query.</param>
        /// <param name="behavior">The required <see cref="CommandBehavior"/></param>
        /// <param name="connector">The connector to use</param>
        /// <remarks>
        /// FETCH ALL is genuinely useful in some situations (if using cursors to return small or medium sized multiple
        /// result sets then we can and do save one round trip to the database overall: n_cursors round trips, rather
        /// than n_cursors + 1), but since it is badly problematic in the case of large cursors we force the user to
        /// request it explicitly.
        /// https://github.com/npgsql/npgsql/issues/438
        /// http://stackoverflow.com/q/42292341/
        /// </remarks>
        internal NpgsqlDereferencingDataReaderX(NpgsqlDataReader reader, CommandBehavior behavior, NpgsqlConnector connector) : base(connector)
        {
            _originalReader = reader;
#if !TEST_WRAPPING_ONLY
            _behavior = behavior;
            _fetchSize = connector.Settings.DereferenceFetchSize;
#endif
        }

        /// <summary>
        /// True iff current reader has cursors in its output types.
        /// </summary>
        /// <param name="reader">The reader to check</param>
        /// <returns>Are there cursors?</returns>
        /// <remarks>Really a part of NpgsqlDereferencingReader</remarks>
        static public bool CanDereference(DbDataReader reader)
        {
#if TEST_WRAPPING_ONLY
            return false;
#else
            var hasCursors = false;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetDataTypeName(i) == "refcursor")
                {
                    hasCursors = true;
                    break;
                }
            }
            return hasCursors;
#endif
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Initialise the reader
        /// </summary>
        /// <returns></returns>
        internal async Task Init(bool async, CancellationToken cancellationToken)
#pragma warning restore CS1998
        {
#if TEST_WRAPPING_ONLY
            _wrappedReader = _originalReader;
#else
            // Behavior is only saved to be used here in this Init method; we don't need to check or enforce it again
            // elsewhere since the read logic below is already enforcing it.
            // For SingleRow we additionally rely on the user to only read one row and then dispose of everything.
            var earlyQuit = (_behavior == CommandBehavior.SingleResult || _behavior == CommandBehavior.SingleRow);

            // Save all the cursors from the original reader here, then dispose it
            // NB the original reader is never returned to the end user, instead this dereferencing reader is
            // NB CanDereference has already been called and determined that there are some cursors
            using (_originalReader)
            {
                while (async ? await _originalReader.ReadAsync(cancellationToken) : _originalReader.Read())
                {
                    for (var i = 0; i < _originalReader.FieldCount; i++)
                    {
                        if (_originalReader.GetDataTypeName(i) == "refcursor")
                        {
                            // cursor name can potentially contain " so stop that breaking us
                            Cursors.Add(_originalReader.GetString(i).Replace(@"""", @""""""));
                            if (earlyQuit) break;
                        }
                    }
                    if (earlyQuit) break;
                }
            }

            // initialize
            if (async)
                await NextResultAsync(cancellationToken);
            else
                NextResult();
#endif
        }

#if !TEST_WRAPPING_ONLY
        /// <summary>
        /// Fetch next N rows from current cursor.
        /// </summary>
        /// <param name="closePreviousSQL">
        /// SQL to prepend to close the previous cursor in a single round trip (send <see cref="string.Empty"/> when
        /// not required).
        /// </param>
        /// <param name="async">True to operate asynchronously.</param>
        /// <param name="cancellationToken">Async <see cref="CancellationToken"/>.</param>
        async Task FetchNextNFromCursor(string closePreviousSQL, bool async, CancellationToken cancellationToken)
        {
            // close and dispose previous fetch reader for this cursor
            if (FetchReader != null && !FetchReader.IsClosed)
            {
                FetchReader.Dispose();
            }

            // fetch next n from cursor;
            // optionally close previous cursor;
            // iff we're fetching all, we can close this cursor in this command
            using (var fetchCmd = CreateCommand(closePreviousSQL + FetchSQL() + (_fetchSize <= 0 ? CloseSQL() : "")))
            {
                // make this dereferencing reader expose its current command to the connector
                Command = fetchCmd;

                if (async)
                    FetchReader = await fetchCmd.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
                else
                    FetchReader = fetchCmd.ExecuteReader(CommandBehavior.SingleResult);
            }

            Count = 0;
        }
#endif

        #region NextResult

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of statements.
        /// </summary>
        /// <returns></returns>
        public override bool NextResult()
        {
#if TEST_WRAPPING_ONLY
            return _wrappedReader.NextResult();
#else
            return NextResult(false, CancellationToken.None)
                .GetAwaiter().GetResult();
#endif
        }

        /// <summary>
        /// This is the asynchronous version of NextResult.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <remarks>Note: the <paramref name="cancellationToken"/> parameter need not be and is not ignored in this variant.</remarks>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            using (NoSynchronizationContextScope.Enter())
#if TEST_WRAPPING_ONLY
                return _wrappedReader.NextResultAsync(cancellationToken);
#else
                return NextResult(true, cancellationToken);
#endif
        }

#if !TEST_WRAPPING_ONLY
        async Task<bool> NextResult(bool async, CancellationToken cancellationToken)
        {
            var closeImmediately = CursorIndex >= Cursors.Count;
            var closeSql = async ?
                await CloseCursor(true, cancellationToken, closeImmediately) :
                CloseCursor(false, CancellationToken.None, closeImmediately).GetAwaiter().GetResult();
            if (closeImmediately)
            {
                return false;
            }
            Cursor = Cursors[CursorIndex++];
            await FetchNextNFromCursor(closeSql, async, cancellationToken);
            return true;
        }
#endif

        #endregion

        #region Read

        /// <summary>
        /// This is an optimized execution path that avoids calling any async methods for the (usual)
        /// case where the next row (or CommandComplete) is already in memory.
        /// </summary>
        /// <returns></returns>
        internal override bool? TryFastRead() =>
            _wrappedReader.TryFastRead();

        /// <summary>
        /// Internal implementation of Read
        /// </summary>
        /// <param name="async"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<bool> Read(bool async, CancellationToken cancellationToken)
        {
#if TEST_WRAPPING_ONLY
            return async ? await _wrappedReader.ReadAsync(cancellationToken) : _wrappedReader.Read();
#else
            if (FetchReader != null)
            {
                var cursorHasNextRow = async ? await FetchReader.ReadAsync(cancellationToken) : FetchReader.Read();
                if (cursorHasNextRow)
                {
                    Count++;
                    return true;
                }

                // if we did FETCH ALL or if rows ended before requested count, there is nothing more to fetch on this cursor
                if (_fetchSize <= 0 || Count < _fetchSize)
                {
                    return false;
                }
            }

            // NB if rows ended at requested count, there may or may not be more rows
            await FetchNextNFromCursor(string.Empty, async, cancellationToken);

            // recursive self-call
            return await Read(async, cancellationToken);
#endif
        }

        #endregion

#if !TEST_WRAPPING_ONLY
        /// <summary>
        /// SQL to fetch required count from current cursor
        /// </summary>
        /// <returns>SQL</returns>
        private string FetchSQL()
        {
            return string.Format(@"FETCH {0} FROM ""{1}"";", (_fetchSize <= 0 ? "ALL" : _fetchSize.ToString()), Cursor);
        }

        /// <summary>
        /// SQL to close current cursor
        /// </summary>
        /// <returns>SQL</returns>
        private string CloseSQL()
        {
            return string.Format(@"CLOSE ""{0}"";", Cursor);
        }

        /// <summary>
        /// Close current FETCH cursor on the database
        /// </summary>
        /// <param name="async">True to operate asynchronously</param>
        /// <param name="cancellationToken">Async <see cref="CancellationToken"/></param>
        /// <param name="ExecuteNow">Iff false then return the SQL but don't execute the command</param>
        /// <returns>The SQL to close the cursor, if there is one and this has not already been executed.</returns>
        private async Task<string> CloseCursor(bool async, CancellationToken cancellationToken, bool ExecuteNow = true)
        {
            // close and dispose current fetch reader for this cursor
            if (FetchReader != null && !FetchReader.IsClosed)
            {
                FetchReader.Dispose();
            }
            // close cursor itself
            if (_fetchSize > 0 && !string.IsNullOrEmpty(Cursor))
            {
                var closeSql = CloseSQL();
                if (!ExecuteNow)
                {
                    return closeSql;
                }
                using (var closeCmd = CreateCommand(closeSql))
                {
                    if (async)
                        await closeCmd.ExecuteNonQueryAsync();
                    else
                        closeCmd.ExecuteNonQuery();
                }
                Cursor = null;
            }
            return "";
        }

        private NpgsqlCommand CreateCommand(string sql)
        {
            var command = Connector.Connection!.CreateCommand();
            command.CommandText = sql;
            return command;
        }
#endif

        #region DbDataReader abstract interface

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) =>
            _wrappedReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

        /// <summary>
        /// Internal implementation of GetStream
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        internal override ValueTask<Stream> GetStreamInternal(int ordinal, bool async) =>
            _wrappedReader.GetStreamInternal(ordinal, async);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldoffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) =>
            _wrappedReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override string GetDataTypeName(int i) =>
            _wrappedReader.GetDataTypeName(i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override Type GetFieldType(int i) =>
            _wrappedReader.GetFieldType(i);

        #endregion

        #region GetValue

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal) =>
            _wrappedReader.GetValue(ordinal);

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetProviderSpecificValue(int ordinal) =>
            _wrappedReader.GetProviderSpecificValue(ordinal);

        #endregion

        #region IsDBNull

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public override bool IsDBNull(int ordinal) =>
            _wrappedReader.IsDBNull(ordinal);

        /// <summary>
        /// An asynchronous version of <see cref="IsDBNull(int)"/>, which gets a value that indicates whether the column contains non-existent or missing values.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns><b>true</b> if the specified column value is equivalent to <see cref="DBNull"/> otherwise <b>false</b>.</returns>
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) =>
            _wrappedReader.IsDBNullAsync(ordinal, cancellationToken);

        #endregion

        #region Cleanup / Dispose

        /// <summary>
        /// Consumes all result sets for this reader, leaving the connector ready for sending and processing further
        /// queries
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async Task Consume(bool async)
#pragma warning restore CS1998
        {
#if !TEST_WRAPPING_ONLY
            await CloseCursor(async, CancellationToken.None, true);
#endif
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal override async Task _Cleanup(bool async, bool connectionClosing = false)
#pragma warning restore CS1998
        {
        }

        #endregion

        #region GetFieldValue

        /// <summary>
        /// Asynchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        /// <param name="ordinal">The type of the value to be returned.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) =>
            _wrappedReader.GetFieldValueAsync<T>(ordinal, cancellationToken);

        /// <summary>
        /// Synchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
        /// <param name="ordinal">The column to be retrieved.</param>
        /// <returns>The column to be retrieved.</returns>
        public override T GetFieldValue<T>(int ordinal) =>
           _wrappedReader.GetFieldValue<T>(ordinal);

        #endregion

        #region Other public accessors

        /// <summary>
        /// Returns schema information for the columns in the current resultset.
        /// </summary>
        /// <returns></returns>
        public override ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema() =>
            _wrappedReader.GetColumnSchema();

        #endregion
    }
}
#endif
