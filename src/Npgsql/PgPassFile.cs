using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Npgsql
{
    /// <summary>
    /// Represents a .pgpass file, which contains passwords for noninteractive connections
    /// </summary>
    class PgPassFile
    {
        #region Properties

        /// <summary>
        /// File name being parsed for credentials
        /// </summary>
        internal string FileName { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="PgPassFile"/> class
        /// </summary>
        /// <param name="fileName"></param>
        internal PgPassFile(string fileName)
        {
            FileName = fileName;
        }

        #endregion

        /// <summary>
        /// Parses file content and gets all credentials from the file
        /// </summary>
        /// <returns><see cref="IEnumerable{PgPassEntry}"/> corresponding to all lines in the .pgpass file</returns>
        internal IEnumerable<PgPassEntry> Entries => File.ReadLines(FileName)
                                                         .Select(line => line.Trim())
                                                         .Where(line => line.Any() && line[0] != '#')
                                                         .Select(PgPassEntry.Create);

        /// <summary>
        /// Searches queries loaded from .PGPASS file to find first entry matching the provided parameters.
        /// </summary>
        /// <param name="hostName">Hostname to query. Use * to match any. </param>
        /// <param name="port">Port to query. Use * to match any. </param>
        /// <param name="database">Database to query. Use * to match any. </param>
        /// <param name="userName">User name to query. Use * to match any. </param>
        /// <returns>Matching <see cref="PgPassEntry"/> if match was found. Otherwise, returns null.</returns>
        internal PgPassEntry GetFirstMatchingEntry(string hostName = null, int? port = null, string database=null, string userName = null) => Entries.FirstOrDefault(entry => entry.IsMatch(hostName, port, database, userName));

        /// <summary>
        /// Retrieves the full expected path to the .PGPASS file
        /// </summary>
        /// <returns>Path to the .PGPASS file</returns>
        internal static string GetPgPassFilePath()
        {
            // .pgpass path from https://www.postgresql.org/docs/current/static/libpq-pgpass.html
            var pgpassEnv = Environment.GetEnvironmentVariable("PGPASSFILE");
            if (pgpassEnv != null && File.Exists(pgpassEnv))
            {
                return pgpassEnv;
            }

            if (PGUtil.IsWindows)
            {
                Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), "postgresql", "pgpass.conf");
            }

            return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".pgpass");
        }
    }
}
