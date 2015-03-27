using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Logging
{
    /// Used to create logger instances of the given name.
    public interface INpgsqlLoggingProvider
    {
        /// <summary>
        /// Creates a new INpgsqlLogger instance of the given name.
        /// </summary>
        NpgsqlLogger CreateLogger(string name);
    }
}
