using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    /// Cursor dereferencing data reader derived originally from removed Npgsql v2 code (see
    /// https://github.com/npgsql/npgsql/issues/438); but now with safer, more consistent behaviour.
    /// </summary>
    /// <remarks>
    /// Does not aim to do the actual reading work, rather, it wraps the creation of as many real instances
    /// of <see cref="NpgsqlDataReader"/> as required to read from the cursors in the original result set.
    ///
    /// NOTE:
    /// 
    /// No longer FETCH ALL by default as this is dangerous for large result sets (http://stackoverflow.com/q/42292341/);
    /// FETCH ALL can be enabled with DereferenceFetchSize=-1 and will be more efficient (less round trips) for small to
    /// medium result sets. Not being FETCH ALL by default means we don't know in advance how many FETCH commands we're going
    /// to issue per cursor, and so can't do the *much* simpler solution of pre-generating one FETCH ALL for each cursor and
    /// sending them all as a normal batch of commands to a standard reader.
    /// 
    /// This dereferences multiple cursors in any arrangement (n x 1; 1 x m; n x m) in the first result set of the original reader.
    /// 
    /// c.f. Oracle:
    ///  - Oracle always does the equivalent of this automatic dereferencing in its ADO.NET driver
    ///  - Since this is always on in Oracle you can never read a cursor from a result set directly; however you can
    ///    still code additonal SQL with output parameters in order to get at the original cursors if you need to
    ///  - In Oracle, if just some values are cursors then these are dereferenced and other data is ignored
    ///  - The rest of Oracle's pattern is to only ever try to dereference on Query and Scalar, never on Execute
    ///  - We copy these latter two behaviours since presumably: a) they work in practice, and b) this will be the most
    ///    useful thing to do for any cross-DB developers
    /// </remarks>
    public sealed class NpgsqlDereferencingReader : NpgsqlWrappingReaderBase
    {
        // internally to this class, zero or any negative value will FETCH ALL; externally in settings, -1 is the currently chosen trigger value
        int _fetchSize;

        // rows read so far on current FETCH
        int _rowsRead;

        // list of names of cursors to dereference
        readonly List<string> _cursors = new List<string>();

        // current cursor index
        int _cursorIndex;

        // name of current cursor
        string? _cursor;

        /// <summary>
        /// Holds the list of statements being executed by this reader.
        /// </summary>
        List<NpgsqlStatement> _statements = default!;

        /// <summary>
        /// Returns details about each statement that this reader will or has executed.
        /// </summary>
        /// <remarks>
        /// Unlike a standard reader, the statements in here must be added to as the reader progresses and works
        /// out what it needs to do.
        /// </remarks>
        public override IReadOnlyList<NpgsqlStatement> Statements => _statements.AsReadOnly();

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
        public override event EventHandler? ReaderClosed;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlDereferencingReader));

        internal NpgsqlDereferencingReader(NpgsqlConnector connector) : base(connector)
        {
            _fetchSize = connector.Settings.DereferenceFetchSize;
        }

        /// <summary>
        /// True iff passed reader has cursors in its output types.
        /// </summary>
        /// <param name="reader">The reader to check</param>
        /// <returns>Are there cursors?</returns>
        public static bool CanDereference(DbDataReader reader)
        {
            var hasCursors = false;
#if true
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetDataTypeName(i) == "refcursor")
                {
                    hasCursors = true;
                    break;
                }
            }
#endif
            return hasCursors;
        }

        /// <summary>
        /// Initialise <see cref="NpgsqlDereferencingReader"/>
        /// </summary>
        /// <remarks>
        /// Can't safely throw within the initialiser of this wrapping reader until after closing original reader, to avoid
        /// triggering the <see cref="Debug.Assert(bool)"/> in <see cref="NpgsqlConnector.EndUserAction"/>.
        /// </remarks>
        internal async Task Init(NpgsqlDataReader originalReader, CommandBehavior behavior, bool async, CancellationToken cancellationToken)
        {
            // reset vars
            _cursors.Clear();
            _cursorIndex = 0;
            _cursor = null;

            _statements = new List<NpgsqlStatement>();

            // Behavior is not required outside this Init method; we don't need to check or enforce it again
            // elsewhere since the read logic below is already enforcing it.
            // For SingleRow we additionally rely on the user to only read one row and then dispose of everything.
            var earlyQuit = (behavior == CommandBehavior.SingleResult || behavior == CommandBehavior.SingleRow);

            // Save all the cursors from the original reader here, then dispose it
            // NB the original reader is never returned to the end user, instead this dereferencing reader is
            // NB CanDereference has already been called and determined that there are some cursors
            using (originalReader)
            {
                while (async ? await originalReader.ReadAsync(cancellationToken) : originalReader.Read())
                {
                    for (var i = 0; i < originalReader.FieldCount; i++)
                    {
                        if (originalReader.GetDataTypeName(i) == "refcursor")
                        {
                            // cursor name can potentially contain " so stop that breaking us
                            _cursors.Add(originalReader.GetString(i).Replace(@"""", @""""""));
                            if (earlyQuit) break;
                        }
                    }
                    if (earlyQuit) break;
                }
            }

            // see remarks above
            if (!Connector.InTransaction)
                throw new InvalidOperationException("Cursor dereferencing requires a transaction. Please add one, or consider using TABLE return values where possible as these are generally more efficient than cursors.");

            // initialize
            if (async)
                await NextResultAsync(cancellationToken);
            else
                NextResult();
        }

        #region Read

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns><b>true</b> if there are more rows; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// The default position of a data reader is before the first record. Therefore, you must call Read to begin accessing data.
        /// </remarks>
        public override bool Read() =>
               Read(false, CancellationToken.None).GetAwaiter().GetResult();

        /// <summary>
        /// This is the asynchronous version of <see cref="Read()"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);

            using (NoSynchronizationContextScope.Enter())
                return Read(true, cancellationToken);
        }

        async Task<bool> Read(bool async, CancellationToken cancellationToken)
        {
            if (_wrappedReader != null)
            {
                var cursorHasNextRow = async ? await _wrappedReader.ReadAsync(cancellationToken) : _wrappedReader.Read();
                if (cursorHasNextRow)
                {
                    _rowsRead++;
                    return true;
                }

                // if we did FETCH ALL or if rows ended before requested count, there is nothing more to fetch on this cursor
                if (_fetchSize <= 0 || _rowsRead < _fetchSize)
                {
                    return false;
                }
            }

            // if rows ended exactly at requested count, there may or may not be more rows
            await FetchNextNFromCursor(string.Empty, async, cancellationToken);
            // recursive self-call
            return await Read(async, cancellationToken);
        }

        #endregion

        #region NextResult

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of statements.
        /// </summary>
        /// <returns></returns>
        public override bool NextResult() =>
            NextResult(false, CancellationToken.None)
                .GetAwaiter().GetResult();

        /// <summary>
        /// This is the asynchronous version of NextResult.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<bool>(cancellationToken);

            using (NoSynchronizationContextScope.Enter())
                return NextResult(true, cancellationToken);
        }

        /// <summary>
        /// Internal implementation of NextResult
        /// </summary>
        async Task<bool> NextResult(bool async, CancellationToken cancellationToken)
        {
            var closeImmediately = _cursorIndex >= _cursors.Count;
            var closeSql = async ?
                await CloseCursor(true, cancellationToken, closeImmediately) :
                CloseCursor(false, CancellationToken.None, closeImmediately).GetAwaiter().GetResult();
            if (closeImmediately)
            {
                return false;
            }
            _cursor = _cursors[_cursorIndex++];
            await FetchNextNFromCursor(closeSql, async, cancellationToken);
            return true;
        }

        #endregion

        #region Cleanup / Dispose

        internal override async Task Close(bool connectionClosing, bool async)
        {
            await CloseCursor(async, CancellationToken.None, true);

            ReaderClosed?.Invoke(this, EventArgs.Empty);
        }

        internal override Task Cleanup(bool async, bool connectionClosing = false)
        {
            if (ReaderClosed != null)
            {
                ReaderClosed(this, EventArgs.Empty);
                ReaderClosed = null;
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Private fetch methods

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
            if (_wrappedReader != null && !_wrappedReader.IsClosed)
            {
                _wrappedReader.Dispose();
            }

            // fetch next n from cursor;
            // optionally close previous cursor;
            // iff we're fetching all, we can close this cursor in this command
            using (var fetchCmd = CreateCommand(closePreviousSQL + FetchSQL() + (_fetchSize <= 0 ? CloseSQL() : "")))
            {
                if (async)
                    _wrappedReader = await fetchCmd.ExecuteReaderAsync(CommandBehavior.SingleResult, cancellationToken);
                else
                    _wrappedReader = fetchCmd.ExecuteReader(CommandBehavior.SingleResult);

                _statements.AddRange(fetchCmd.Statements);
            }

            _rowsRead = 0;
        }

        /// <summary>
        /// Close current FETCH cursor on the database
        /// </summary>
        /// <param name="async">True to operate asynchronously</param>
        /// <param name="cancellationToken">Async <see cref="CancellationToken"/></param>
        /// <param name="ExecuteNow">Iff false then return the SQL but don't execute the command</param>
        /// <returns>The SQL to close the cursor, if there is one and this has not already been executed.</returns>
        async Task<string> CloseCursor(bool async, CancellationToken cancellationToken, bool ExecuteNow = true)
        {
            // close and dispose current fetch reader for this cursor
            if (_wrappedReader != null && !_wrappedReader.IsClosed)
            {
                _wrappedReader.Dispose();
            }
            // close cursor itself
            if (_fetchSize > 0 && !string.IsNullOrEmpty(_cursor))
            {
                var closeSql = CloseSQL();
                if (!ExecuteNow)
                {
                    return closeSql;
                }
                using (var closeCmd = CreateCommand(closeSql))
                {
                    if (async)
                        await closeCmd.ExecuteNonQueryAsync(cancellationToken);
                    else
                        closeCmd.ExecuteNonQuery();

                    _statements.AddRange(closeCmd.Statements);
                }
                _cursor = null;
            }
            return "";
        }

        #endregion

        #region Create command

        NpgsqlCommand CreateCommand(string sql)
        {
            var command = Connector.Connection!.CreateCommand();
            command.CommandText = sql;

            // make this dereferencing reader expose its current command to the connector
            Command = command;

            return command;
        }

        #endregion

        #region Cursor SQL

        /// <summary>
        /// SQL to fetch required count from current cursor
        /// </summary>
        /// <returns>SQL</returns>
        string FetchSQL()
        {
            return string.Format(@"FETCH {0} FROM ""{1}"";", (_fetchSize <= 0 ? "ALL" : _fetchSize.ToString()), _cursor);
        }

        /// <summary>
        /// SQL to close current cursor
        /// </summary>
        /// <returns>SQL</returns>
        string CloseSQL()
        {
            return string.Format(@"CLOSE ""{0}"";", _cursor);
        }

        #endregion
    }
}
