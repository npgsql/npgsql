using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
    public sealed class NpgsqlDereferencingDataReader : NpgsqlDataReader
    {
        private NpgsqlDataReader _wrappedReader = default!;

        private NpgsqlDataReader _originalReader;
        private CommandBehavior _behavior;
        NpgsqlConnector _connector;

        // internally to this class, <= 0 will FETCH ALL; externally in settings > 0 or -1 are the legal values
        ////private int _fetchSize;

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed => _wrappedReader.IsClosed;

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        public override int RecordsAffected => _wrappedReader.RecordsAffected;

        /// <summary>
        /// Returns details about each statement that this reader will or has executed.
        /// </summary>
        /// <remarks>
        /// Note that some fields (i.e. rows and oid) are only populated as the reader
        /// traverses the result.
        ///
        /// For commands with multiple queries, this exposes the number of rows affected on
        /// a statement-by-statement basis, unlike <see cref="DbDataReader.RecordsAffected"/>
        /// which exposes an aggregation across all statements.
        /// </remarks>
        public override IReadOnlyList<NpgsqlStatement> Statements => _wrappedReader.Statements;

        /// <summary>
        /// Gets a value that indicates whether this DbDataReader contains one or more rows.
        /// </summary>
        public override bool HasRows => _wrappedReader.HasRows;

        /// <summary>
        /// Indicates whether the reader is currently positioned on a row, i.e. whether reading a
        /// column is possible.
        /// This property is different from <see cref="DbDataReader.HasRows"/> in that <see cref="DbDataReader.HasRows"/> will
        /// return true even if attempting to read a column will fail, e.g. before <see cref="DbDataReader.Read()"/>
        /// has been called
        /// </summary>
        [PublicAPI]
        public override bool IsOnRow => _wrappedReader.IsOnRow;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
#pragma warning disable CS0067 // The event 'NpgsqlDereferencingDataReader.ReaderClosed' is never used
        public override event EventHandler? ReaderClosed;
#pragma warning restore CS0067

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlDereferencingDataReader));

        /// <summary>
        /// Create a safe, sensible dereferencing reader; <see cref="CanDereference"/> has already been called to check
        /// that there are at least some cursors to dereference before this constructor is called.
        /// </summary>
        /// <param name="reader">The original reader for the undereferenced query.</param>
        /// <param name="behavior">The required <see cref="CommandBehavior"/></param>
        /// <param name="connector">The connector to use</param>
        internal NpgsqlDereferencingDataReader(NpgsqlDataReader reader, CommandBehavior behavior, NpgsqlConnector connector)
        {
            _originalReader = reader;
            _behavior = behavior;
            _connector = connector;
            ////_fetchSize = connector.Settings.DereferenceFetchSize;
        }

        /// <summary>
        /// True iff current reader has cursors in its output types.
        /// </summary>
        /// <param name="reader">The reader to check</param>
        /// <returns>Are there cursors?</returns>
        /// <remarks>Really a part of NpgsqlDereferencingReader</remarks>
        public static bool CanDereference(DbDataReader reader)
        {
            return true;
        }

        /// <summary>
        /// Initialise the reader
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal async Task Init(bool async, CancellationToken cancellationToken)
#pragma warning restore CS1998
        {
            _wrappedReader = _originalReader;
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
            _wrappedReader.Read();

        /// <summary>
        /// This is the asynchronous version of <see cref="Read()"/> The cancellation token is currently ignored.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) =>
            _wrappedReader.ReadAsync(cancellationToken);

        #endregion

        #region NextResult

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of statements.
        /// </summary>
        /// <returns></returns>
        public override bool NextResult() =>
            _wrappedReader.NextResult();

        /// <summary>
        /// This is the asynchronous version of NextResult.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <remarks>Note: the <paramref name="cancellationToken"/> parameter need not be and is not ignored in this variant.</remarks>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            using (NoSynchronizationContextScope.Enter())
                return _wrappedReader.NextResultAsync(cancellationToken);
        }

        #endregion

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal) => _wrappedReader.GetName(ordinal);

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override int FieldCount => _wrappedReader.FieldCount;

        #region Cleanup / Dispose

        internal override async Task Close(bool connectionClosing, bool async) =>
            await _wrappedReader.Close(connectionClosing, async);

        internal override async Task Cleanup(bool async, bool connectionClosing = false) =>
            await _wrappedReader.Cleanup(async, connectionClosing);

        #endregion

        #region Simple value getters

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
        public override int GetValues(object[] values) => _wrappedReader.GetValues(values);

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
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) =>
            _wrappedReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override Stream GetStream(int ordinal) => _wrappedReader.GetStream(ordinal);

        /// <summary>
        /// Retrieves data as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The returned object.</returns>
        public override Task<Stream> GetStreamAsync(int ordinal, CancellationToken cancellationToken = default) =>
            _wrappedReader.GetStreamAsync(ordinal, cancellationToken);

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
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) =>
            _wrappedReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The returned object.</returns>
        public override TextReader GetTextReader(int ordinal) => _wrappedReader.GetTextReader(ordinal);

        /// <summary>
        /// Retrieves data as a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The returned object.</returns>
        public override Task<TextReader> GetTextReaderAsync(int ordinal, CancellationToken cancellationToken = default) =>
            _wrappedReader.GetTextReaderAsync(ordinal, cancellationToken);

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

        #region Other public accessors

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name) => _wrappedReader.GetOrdinal(name);

        /// <summary>
        /// Gets a representation of the PostgreSQL data type for the specified field.
        /// The returned representation can be used to access various information about the field.
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        [PublicAPI]
        public override PostgresType GetPostgresType(int ordinal) => _wrappedReader.GetPostgresType(ordinal);

        /// <summary>
        /// Gets the data type information for the specified field.
        /// This will be the PostgreSQL type name (e.g. double precision), not the .NET type
        /// (see <see cref="GetFieldType"/> for that).
        /// </summary>
        /// <param name="ordinal">The zero-based column index.</param>
        public override string GetDataTypeName(int ordinal) => _wrappedReader.GetDataTypeName(ordinal);

        /// <summary>
        /// Gets the OID for the PostgreSQL type for the specified field, as it appears in the pg_type table.
        /// </summary>
        /// <remarks>
        /// This is a PostgreSQL-internal value that should not be relied upon and should only be used for
        /// debugging purposes.
        /// </remarks>
        /// <param name="ordinal">The zero-based column index.</param>
        public override uint GetDataTypeOID(int ordinal) => _wrappedReader.GetDataTypeOID(ordinal);

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        public override Type GetFieldType(int ordinal) => _wrappedReader.GetFieldType(ordinal);

        /// <summary>
        /// Returns the provider-specific field type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The Type object that describes the data type of the specified column.</returns>
        public override Type GetProviderSpecificFieldType(int ordinal) => _wrappedReader.GetProviderSpecificFieldType(ordinal);

        /// <summary>
        /// Gets all provider-specific attribute columns in the collection for the current row.
        /// </summary>
        /// <param name="values">An array of Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
        public override int GetProviderSpecificValues(object[] values) => GetProviderSpecificValues(values);

        /// <summary>
        /// Returns schema information for the columns in the current resultset.
        /// </summary>
        /// <returns></returns>
        public override ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema() => _wrappedReader.GetColumnSchema();

        #endregion
    }
}
