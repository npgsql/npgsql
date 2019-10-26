using Npgsql.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Wraps another data reader.
    /// </summary>
    /// <remarks>
    /// Check that wrapping works fully and passes all Npgsql tests.
    /// </remarks>
    public sealed class NpgsqlWrappingReader : NpgsqlWrappingReaderBase
    {
        internal const bool TestWrapEverything = false;

        /// <summary>
        /// Is raised whenever Close() is called.
        /// </summary>
#pragma warning disable CS0067 // The event 'NpgsqlDereferencingDataReader.ReaderClosed' is never used
        public override event EventHandler? ReaderClosed;
#pragma warning restore CS0067

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlWrappingReader));

        internal NpgsqlWrappingReader(NpgsqlConnector connector) : base(connector) { }

        internal void Init(NpgsqlDataReader originalReader)
        {
            _wrappedReader = originalReader;
            Command = originalReader.Command;
            originalReader.ReaderClosed += (sender, args) => ReaderClosed?.Invoke(sender, args);
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
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) =>
            _wrappedReader.NextResultAsync(cancellationToken);

        #endregion

        #region Cleanup / Dispose

        internal override async Task Close(bool connectionClosing, bool async) =>
            await _wrappedReader.Close(connectionClosing, async);

        internal override async Task Cleanup(bool async, bool connectionClosing = false) =>
            await _wrappedReader.Cleanup(async, connectionClosing);

        #endregion

    }
}
