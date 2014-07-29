using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Npgsql.Npgsql
{
    /// <summary>
    /// Represents an exclusive advisory lock managed in the Postgresql backend. Can be used for
    /// cooperative application synchronization.
    /// See <a href="http://www.postgresql.org/docs/9.3/static/explicit-locking.html">the Postgresql documentation</a>
    /// </summary>
    public class NpgsqlExclusiveAdvisoryLock
    {
        readonly NpgsqlConnection _conn;
        readonly long? _key;
        readonly int _key1;
        readonly int _key2;

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the single key <paramref name="key"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key">a single 64-bit key identifying this lock</param>
        public NpgsqlExclusiveAdvisoryLock(NpgsqlConnection conn, long key)
        {
            _conn = conn;
            _key = key;
        }

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the dual keys <paramref name="key1"/>
        /// and <paramref name="key2"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key1">a 32-bit key identifying this lock along with <paramref name="key2"/></param>
        /// <param name="key2">a 32-bit key identifying this lock along with <paramref name="key1"/></param>
        public NpgsqlExclusiveAdvisoryLock(NpgsqlConnection conn, int key1, int key2)
        {
            _conn = conn;
            _key1 = key1;
            _key2 = key2;
        }

        /// <summary>
        /// Attempts to acquire the lock.
        /// If another session already holds a lock on the same resource identifier, this method will
        /// block until the resource becomes available. Multiple lock requests stack, so that if the
        /// same resource is locked three times it must then be unlocked three times to be released
        /// for other sessions' use.
        /// </summary>
        /// <returns>a handle that, when disposed, frees the lock</returns>
        public IDisposable Acquire()
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_advisory_lock(" + key + ")", _conn))
                cmd.ExecuteNonQuery();
            return new NpgsqlAdvisoryLockHandle(_conn, "SELECT pg_advisory_unlock(" + key + ")");
        }

        /// <summary>
        /// Similar to <see cref="Acquire"/>, except the method will not block for the lock to become
        /// available. It will either obtain the lock immediately and return true, or return false if
        /// the lock cannot be acquired immediately.
        /// </summary>
        /// <param name="handle">if the method returned true, will contain a handle that, when disposed,
        /// frees the lock. Otherwise null.</param>
        /// <returns>whether the lock was acquired</returns>
        public bool TryAcquire(out IDisposable handle)
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_try_advisory_lock(" + key + ")", _conn))
            {
                var result = (bool)cmd.ExecuteScalar();
                handle = result ? new NpgsqlAdvisoryLockHandle(_conn, "SELECT pg_advisory_unlock(" + key + ")") : null;
                return result;
            }
        }
    }

    public class NpgsqlSharedAdvisoryLock
    {
        readonly NpgsqlConnection _conn;
        readonly long? _key;
        readonly int _key1;
        readonly int _key2;

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the single key <paramref name="key"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key">a single 64-bit key identifying this lock</param>
        public NpgsqlSharedAdvisoryLock(NpgsqlConnection conn, long key)
        {
            _conn = conn;
            _key = key;
        }

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the dual keys <paramref name="key1"/>
        /// and <paramref name="key2"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key1">a 32-bit key identifying this lock along with <paramref name="key2"/></param>
        /// <param name="key2">a 32-bit key identifying this lock along with <paramref name="key1"/></param>
        public NpgsqlSharedAdvisoryLock(NpgsqlConnection conn, int key1, int key2)
        {
            _conn = conn;
            _key1 = key1;
            _key2 = key2;
        }

        /// <summary>
        /// Attempts to acquire the lock.
        /// If another session already holds a lock on the same resource identifier, this method will
        /// block until the resource becomes available. Multiple lock requests stack, so that if the
        /// same resource is locked three times it must then be unlocked three times to be released
        /// for other sessions' use.
        /// </summary>
        /// <returns>a handle that, when disposed, frees the lock</returns>
        public IDisposable Acquire()
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_advisory_lock_shared(" + key + ")", _conn))
                cmd.ExecuteNonQuery();
            return new NpgsqlAdvisoryLockHandle(_conn, "SELECT pg_advisory_unlock_shared(" + key + ")");
        }

        /// <summary>
        /// Similar to <see cref="Acquire"/>, except the method will not block for the lock to become
        /// available. It will either obtain the lock immediately and return true, or return false if
        /// the lock cannot be acquired immediately.
        /// </summary>
        /// <param name="handle">if the method returned true, will contain a handle that, when disposed,
        /// frees the lock. Otherwise null.</param>
        /// <returns>whether the lock was acquired</returns>
        public bool TryAcquire(out IDisposable handle)
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_try_advisory_lock_shared(" + key + ")", _conn))
            {
                var result = (bool)cmd.ExecuteScalar();
                handle = result ? new NpgsqlAdvisoryLockHandle(_conn, "SELECT pg_advisory_unlock_shared(" + key + ")") : null;
                return result;
            }
        }
    }

    public class NpgsqlExclusiveTransactionAdvisoryLock
    {
        readonly NpgsqlConnection _conn;
        readonly long? _key;
        readonly int _key1;
        readonly int _key2;

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the single key <paramref name="key"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key">a single 64-bit key identifying this lock</param>
        public NpgsqlExclusiveTransactionAdvisoryLock(NpgsqlConnection conn, long key)
            : this(conn)
        {
            _key = key;
        }

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the dual keys <paramref name="key1"/>
        /// and <paramref name="key2"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key1">a 32-bit key identifying this lock along with <paramref name="key2"/></param>
        /// <param name="key2">a 32-bit key identifying this lock along with <paramref name="key1"/></param>
        public NpgsqlExclusiveTransactionAdvisoryLock(NpgsqlConnection conn, int key1, int key2)
            : this(conn)
        {
            _key1 = key1;
            _key2 = key2;
        }

        NpgsqlExclusiveTransactionAdvisoryLock(NpgsqlConnection conn)
        {
            if (conn.PostgreSqlVersion < new Version(9, 1, 0))
                throw new NotSupportedException("Transaction advisory locks aren't supported prior to Postgresql 9.1");
            _conn = conn;            
        }

        /// <summary>
        /// Attempts to acquire the lock.
        /// If another session already holds a lock on the same resource identifier, this method will
        /// block until the resource becomes available. Multiple lock requests stack, so that if the
        /// same resource is locked three times it must then be unlocked three times to be released
        /// for other sessions' use.
        /// </summary>
        /// <returns>a handle that, when disposed, frees the lock</returns>
        public void Acquire()
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_advisory_xact_lock(" + key + ")", _conn))
                cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Similar to <see cref="Acquire"/>, except the method will not block for the lock to become
        /// available. It will either obtain the lock immediately and return true, or return false if
        /// the lock cannot be acquired immediately.
        /// </summary>
        /// <param name="handle">if the method returned true, will contain a handle that, when disposed,
        /// frees the lock. Otherwise null.</param>
        /// <returns>whether the lock was acquired</returns>
        public bool TryAcquire()
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_try_advisory_xact_lock(" + key + ")", _conn))
                return (bool)cmd.ExecuteScalar();
        }
    }

    public class NpgsqlSharedTransactionAdvisoryLock
    {
        readonly NpgsqlConnection _conn;
        readonly long? _key;
        readonly int _key1;
        readonly int _key2;

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the single key <paramref name="key"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key">a single 64-bit key identifying this lock</param>
        public NpgsqlSharedTransactionAdvisoryLock(NpgsqlConnection conn, long key)
            : this(conn)
        {
            _key = key;
        }

        /// <summary>
        /// Create the lock on <paramref name="conn"/> with the dual keys <paramref name="key1"/>
        /// and <paramref name="key2"/>.
        /// </summary>
        /// <param name="conn">the connection on which to manage the lock</param>
        /// <param name="key1">a 32-bit key identifying this lock along with <paramref name="key2"/></param>
        /// <param name="key2">a 32-bit key identifying this lock along with <paramref name="key1"/></param>
        public NpgsqlSharedTransactionAdvisoryLock(NpgsqlConnection conn, int key1, int key2)
            : this(conn)
        {
            _key1 = key1;
            _key2 = key2;
        }

        NpgsqlSharedTransactionAdvisoryLock(NpgsqlConnection conn)
        {
            if (conn.PostgreSqlVersion < new Version(9, 1, 0))
                throw new NotSupportedException("Transaction advisory locks aren't supported prior to Postgresql 9.1");
            _conn = conn;            
        }

        /// <summary>
        /// Attempts to acquire the lock.
        /// If another session already holds a lock on the same resource identifier, this method will
        /// block until the resource becomes available. Multiple lock requests stack, so that if the
        /// same resource is locked three times it must then be unlocked three times to be released
        /// for other sessions' use.
        /// </summary>
        /// <returns>a handle that, when disposed, frees the lock</returns>
        public void Acquire()
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_advisory_xact_lock_shared(" + key + ")", _conn))
                cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Similar to <see cref="Acquire"/>, except the method will not block for the lock to become
        /// available. It will either obtain the lock immediately and return true, or return false if
        /// the lock cannot be acquired immediately.
        /// </summary>
        /// <param name="handle">if the method returned true, will contain a handle that, when disposed,
        /// frees the lock. Otherwise null.</param>
        /// <returns>whether the lock was acquired</returns>
        public bool TryAcquire()
        {
            var key = _key.HasValue ? _key.ToString() : _key1 + "," + _key2;
            using (var cmd = new NpgsqlCommand("SELECT pg_try_advisory_xact_lock_shared(" + key + ")", _conn))
                return (bool)cmd.ExecuteScalar();
        }
    }

    class NpgsqlAdvisoryLockHandle : IDisposable
    {
        readonly NpgsqlConnection _conn;
        readonly string _unlockCmdText;

        internal NpgsqlAdvisoryLockHandle(NpgsqlConnection conn, string unlockCmdText)
        {
            _conn = conn;
            _unlockCmdText = unlockCmdText;
        }

        public void Dispose()
        {
            // Note that the Postgresql unlock function returns false if the unlock failed, and
            // issues a warning - but never an error.
            using (var cmd = new NpgsqlCommand(_unlockCmdText, _conn))
                cmd.ExecuteNonQuery();
        }

        // TODO: Implement destructor
    }
}
