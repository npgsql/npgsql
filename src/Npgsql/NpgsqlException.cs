using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// The exception that is thrown when server-related issues occur.
    /// </summary>
    /// <remarks>
    /// PostgreSQL errors (e.g. query SQL issues, constraint violations) are raised via
    /// <see cref="PostgresException"/> which is a subclass of this class.
    /// Purely Npgsql-related issues which aren't related to the server will be raised
    /// via the standard CLR exceptions (e.g. ArgumentException).
    /// </remarks>
#if NET45 || NET451
    [Serializable]
#endif
    public class NpgsqlException : DbException
    {
        internal NpgsqlException() {}

        internal NpgsqlException(string message, Exception innerException) 
            : base(message, innerException) {}

        internal NpgsqlException(string message)
            : base(message) { }
    }
}
