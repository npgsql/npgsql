﻿using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Large object manager. This class can be used to store very large files in a PostgreSQL database.
    /// </summary>
    public class NpgsqlLargeObjectManager
    {
        const int InvWrite = 0x00020000;
        const int InvRead = 0x00040000;

        internal NpgsqlConnection Connection { get; }

        /// <summary>
        /// The largest chunk size (in bytes) read and write operations will read/write each roundtrip to the network. Default 4 MB.
        /// </summary>
        public int MaxTransferBlockSize { get; set; }

        /// <summary>
        /// Creates an NpgsqlLargeObjectManager for this connection. The connection must be opened to perform remote operations.
        /// </summary>
        /// <param name="connection"></param>
        public NpgsqlLargeObjectManager(NpgsqlConnection connection)
        {
            Connection = connection;
            MaxTransferBlockSize = 4 * 1024 * 1024; // 4MB
        }

        /// <summary>
        /// Execute a function
        /// </summary>
        internal async Task<T> ExecuteFunction<T>(string function, bool async, params object[] arguments)
        {
            using var command = new NpgsqlCommand(function, Connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = function
            };

            foreach (var argument in arguments)
                command.Parameters.Add(new NpgsqlParameter { Value = argument });

            return (T)(async ? await command.ExecuteScalarAsync() : command.ExecuteScalar());
        }

        /// <summary>
        /// Execute a function that returns a byte array
        /// </summary>
        /// <returns></returns>
        internal async Task<int> ExecuteFunctionGetBytes(string function, byte[] buffer, int offset, int len, bool async, params object[] arguments)
        {
            using var command = new NpgsqlCommand(function, Connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            foreach (var argument in arguments)
                command.Parameters.Add(new NpgsqlParameter { Value = argument });

            using var reader = async
                ? await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess)
                : command.ExecuteReader(CommandBehavior.SequentialAccess);

            if (async)
                await reader.ReadAsync();
            else
                reader.Read();

            return (int)reader.GetBytes(0, 0, buffer, offset, len);
        }

        /// <summary>
        /// Create an empty large object in the database. If an oid is specified but is already in use, an PostgresException will be thrown.
        /// </summary>
        /// <param name="preferredOid">A preferred oid, or specify 0 if one should be automatically assigned</param>
        /// <returns>The oid for the large object created</returns>
        /// <exception cref="PostgresException">If an oid is already in use</exception>
        public uint Create(uint preferredOid = 0) => Create(preferredOid, false).GetAwaiter().GetResult();

        // Review unused parameters
        /// <summary>
        /// Create an empty large object in the database. If an oid is specified but is already in use, an PostgresException will be thrown.
        /// </summary>
        /// <param name="preferredOid">A preferred oid, or specify 0 if one should be automatically assigned</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The oid for the large object created</returns>
        /// <exception cref="PostgresException">If an oid is already in use</exception>
        public Task<uint> CreateAsync(uint preferredOid, CancellationToken cancellationToken = default)
            => cancellationToken.IsCancellationRequested
                ? Task.FromCanceled<uint>(cancellationToken)
                : Create(preferredOid, true);

        Task<uint> Create(uint preferredOid, bool async)
            => ExecuteFunction<uint>("lo_create", async, (int)preferredOid);

        /// <summary>
        /// Opens a large object on the backend, returning a stream controlling this remote object.
        /// A transaction snapshot is taken by the backend when the object is opened with only read permissions.
        /// When reading from this object, the contents reflects the time when the snapshot was taken.
        /// Note that this method, as well as operations on the stream must be wrapped inside a transaction.
        /// </summary>
        /// <param name="oid">Oid of the object</param>
        /// <returns>An NpgsqlLargeObjectStream</returns>
        public NpgsqlLargeObjectStream OpenRead(uint oid)
            => OpenRead(oid, false).GetAwaiter().GetResult();

        /// <summary>
        /// Opens a large object on the backend, returning a stream controlling this remote object.
        /// A transaction snapshot is taken by the backend when the object is opened with only read permissions.
        /// When reading from this object, the contents reflects the time when the snapshot was taken.
        /// Note that this method, as well as operations on the stream must be wrapped inside a transaction.
        /// </summary>
        /// <param name="oid">Oid of the object</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>An NpgsqlLargeObjectStream</returns>
        public Task<NpgsqlLargeObjectStream> OpenReadAsync(uint oid, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<NpgsqlLargeObjectStream>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return OpenRead(oid, true);
        }

        async Task<NpgsqlLargeObjectStream> OpenRead(uint oid, bool async)
        {
            var fd = await ExecuteFunction<int>("lo_open", async, (int)oid, InvRead);
            return new NpgsqlLargeObjectStream(this, fd, false);
        }

        /// <summary>
        /// Opens a large object on the backend, returning a stream controlling this remote object.
        /// Note that this method, as well as operations on the stream must be wrapped inside a transaction.
        /// </summary>
        /// <param name="oid">Oid of the object</param>
        /// <returns>An NpgsqlLargeObjectStream</returns>
        public NpgsqlLargeObjectStream OpenReadWrite(uint oid)
            => OpenReadWrite(oid, false).GetAwaiter().GetResult();

        /// <summary>
        /// Opens a large object on the backend, returning a stream controlling this remote object.
        /// Note that this method, as well as operations on the stream must be wrapped inside a transaction.
        /// </summary>
        /// <param name="oid">Oid of the object</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>An NpgsqlLargeObjectStream</returns>
        public Task<NpgsqlLargeObjectStream> OpenReadWriteAsync(uint oid, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<NpgsqlLargeObjectStream>(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return OpenReadWrite(oid, true);
        }

        async Task<NpgsqlLargeObjectStream> OpenReadWrite(uint oid, bool async)
        {
            var fd = await ExecuteFunction<int>("lo_open", async, (int)oid, InvRead | InvWrite);
            return new NpgsqlLargeObjectStream(this, fd, true);
        }

        /// <summary>
        /// Deletes a large object on the backend.
        /// </summary>
        /// <param name="oid">Oid of the object to delete</param>
        public void Unlink(uint oid)
            => ExecuteFunction<object>("lo_unlink", false, (int)oid).GetAwaiter().GetResult();

        /// <summary>
        /// Deletes a large object on the backend.
        /// </summary>
        /// <param name="oid">Oid of the object to delete</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public Task UnlinkAsync(uint oid, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ExecuteFunction<object>("lo_unlink", true, (int)oid);
        }

        /// <summary>
        /// Exports a large object stored in the database to a file on the backend. This requires superuser permissions.
        /// </summary>
        /// <param name="oid">Oid of the object to export</param>
        /// <param name="path">Path to write the file on the backend</param>
        public void ExportRemote(uint oid, string path)
            => ExecuteFunction<object>("lo_export", false, (int)oid, path).GetAwaiter().GetResult();

        /// <summary>
        /// Exports a large object stored in the database to a file on the backend. This requires superuser permissions.
        /// </summary>
        /// <param name="oid">Oid of the object to export</param>
        /// <param name="path">Path to write the file on the backend</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public Task ExportRemoteAsync(uint oid, string path, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ExecuteFunction<object>("lo_export", true, (int)oid, path);
        }

        /// <summary>
        /// Imports a large object to be stored as a large object in the database from a file stored on the backend. This requires superuser permissions.
        /// </summary>
        /// <param name="path">Path to read the file on the backend</param>
        /// <param name="oid">A preferred oid, or specify 0 if one should be automatically assigned</param>
        public void ImportRemote(string path, uint oid = 0)
            => ExecuteFunction<object>("lo_import", false, path, (int)oid).GetAwaiter().GetResult();

        /// <summary>
        /// Imports a large object to be stored as a large object in the database from a file stored on the backend. This requires superuser permissions.
        /// </summary>
        /// <param name="path">Path to read the file on the backend</param>
        /// <param name="oid">A preferred oid, or specify 0 if one should be automatically assigned</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        public Task ImportRemoteAsync(string path, uint oid, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return ExecuteFunction<object>("lo_import", true, path, (int)oid);
        }

        /// <summary>
        /// Since PostgreSQL 9.3, large objects larger than 2GB can be handled, up to 4TB.
        /// This property returns true whether the PostgreSQL version is >= 9.3.
        /// </summary>
        public bool Has64BitSupport => Connection.PostgreSqlVersion >= new Version(9, 3);

        /*
        internal enum Function : uint
        {
            lo_open = 952,
            lo_close = 953,
            loread = 954,
            lowrite = 955,
            lo_lseek = 956,
            lo_lseek64 = 3170, // Since PostgreSQL 9.3
            lo_creat = 957,
            lo_create = 715,
            lo_tell = 958,
            lo_tell64 = 3171, // Since PostgreSQL 9.3
            lo_truncate = 1004,
            lo_truncate64 = 3172, // Since PostgreSQL 9.3

            // These four are available since PostgreSQL 9.4
            lo_from_bytea = 3457,
            lo_get = 3458,
            lo_get_fragment = 3459,
            lo_put = 3460,

            lo_unlink = 964,

            lo_import = 764,
            lo_import_with_oid = 767,
            lo_export = 765,
        }
        */
    }
}
