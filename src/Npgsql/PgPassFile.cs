using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

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
        PgPassFile(string fileName)
        {
            FileName = fileName;
        }

        [CanBeNull]
        internal static PgPassFile Load([CanBeNull] string pgPassFile)
        {
            var path = pgPassFile ?? GetSystemPgPassFilePath();
            return path == null || !File.Exists(path) ? null : new PgPassFile(path);
        }

        #endregion

        /// <summary>
        /// Parses file content and gets all credentials from the file
        /// </summary>
        /// <returns><see cref="IEnumerable{PgPassEntry}"/> corresponding to all lines in the .pgpass file</returns>
        internal IEnumerable<Entry> Entries => File.ReadLines(FileName)
                                                         .Select(line => line.Trim())
                                                         .Where(line => line.Any() && line[0] != '#')
                                                         .Select(Entry.Parse);

        /// <summary>
        /// Searches queries loaded from .PGPASS file to find first entry matching the provided parameters.
        /// </summary>
        /// <param name="host">Hostname to query. Use null to match any.</param>
        /// <param name="port">Port to query. Use null to match any.</param>
        /// <param name="database">Database to query. Use null to match any.</param>
        /// <param name="username">User name to query. Use null to match any.</param>
        /// <returns>Matching <see cref="Entry"/> if match was found. Otherwise, returns null.</returns>
        [CanBeNull]
        internal Entry GetFirstMatchingEntry(string host = null, int? port = null, string database=null, string username=null)
            => Entries.FirstOrDefault(entry => entry.IsMatch(host, port, database, username));

        /// <summary>
        /// Retrieves the full system path to the pgpass file. Does not check whether the
        /// file actually exist.
        /// </summary>
        /// <remarks>
        /// See https://www.postgresql.org/docs/current/static/libpq-pgpass.html
        /// </remarks>
        /// <returns>Path to the pgpass file</returns>
        [CanBeNull]
        internal static string GetSystemPgPassFilePath()
        {
            var pgpassEnv = Environment.GetEnvironmentVariable("PGPASSFILE");
            if (pgpassEnv != null)
                return pgpassEnv;

            // ReSharper disable AssignNullToNotNullAttribute
            return PGUtil.IsWindows
                ? Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "postgresql", "pgpass.conf")
                : Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".pgpass");
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Represents a hostname, port, database, username, and password combination that has been retrieved from a .pgpass file
        /// </summary>
        internal class Entry
        {
            const string PgPassWildcard = "*";

            #region Fields and Properties

            /// <summary>
            /// Hostname parsed from the .pgpass file
            /// </summary>
            [CanBeNull] internal string Host { get; }
            /// <summary>
            /// Port parsed from the .pgpass file
            /// </summary>
            [CanBeNull] internal int? Port { get; }
            /// <summary>
            /// Database parsed from the .pgpass file
            /// </summary>
            [CanBeNull] internal string Database { get; }
            /// <summary>
            /// User name parsed from the .pgpass file
            /// </summary>
            [CanBeNull] internal string Username { get; }
            /// <summary>
            /// Password parsed from the .pgpass file
            /// </summary>
            internal string Password { get; }

            #endregion

            #region Construction / Initialization

            /// <summary>
            /// This class represents an entry from the .pgpass file
            /// </summary>
            /// <param name="host">Hostname parsed from the .pgpass file</param>
            /// <param name="port">Port parsed from the .pgpass file</param>
            /// <param name="database">Database parsed from the .pgpass file</param>
            /// <param name="username">User name parsed from the .pgpass file</param>
            /// <param name="password">Password parsed from the .pgpass file</param>
            Entry(string host, int? port, string database, string username, string password)
            {
                Host = host;
                Port = port;
                Database = database;
                Username = username;
                Password = password;
            }

            /// <summary>
            /// Creates new <see cref="Entry"/> based on string in the format hostname:port:database:username:password. The : and \ characters should be escaped with a \.
            /// </summary>
            /// <param name="serializedEntry">string for the entry from the pgpass file</param>
            /// <returns>New instance of <see cref="Entry"/> for the string</returns>
            /// <exception cref="FormatException">Entry is not formatted as hostname:port:database:username:password or non-wildcard port is not a number</exception>
            internal static Entry Parse(string serializedEntry)
            {
                var parts = Regex.Split(serializedEntry, @"(?<!\\):"); // split on any colons that aren't preceded by a \ (\ indicates that the colon is part of the content and not a separator)
                if (parts == null || parts.Length != 5)
                    throw new FormatException("pgpass entry was not well-formed. Please ensure all non-comment entries are formatted as hostname:port:database:username:password. If colon is included, it must be escaped like \\:.");

                parts = parts
                    .Select(part => part.Replace("\\:", ":").Replace("\\\\", "\\")) // unescape any escaped characters
                    .Select(part => part == PgPassWildcard ? null : part)
                    .ToArray();

                int? port = null;
                if (parts[1] != null)
                {
                    if (!int.TryParse(parts[1], out var tempPort))
                        throw new FormatException("pgpass entry was not formatted correctly. Port must be a valid integer.");
                    port = tempPort;
                }

                return new Entry(parts[0], port, parts[2], parts[3], parts[4]);
            }

            #endregion


            /// <summary>
            /// Checks whether this <see cref="Entry"/> matches the parameters supplied
            /// </summary>
            /// <param name="host">Hostname to check against this entry</param>
            /// <param name="port">Port to check against this entry</param>
            /// <param name="database">Database to check against this entry</param>
            /// <param name="username">Username to check against this entry</param>
            /// <returns>True if the entry is a match. False otherwise.</returns>
            internal bool IsMatch([CanBeNull]string host, [CanBeNull]int? port, [CanBeNull]string database, [CanBeNull]string username) =>
                AreValuesMatched(host, Host) && AreValuesMatched(port, Port) && AreValuesMatched(database, Database) && AreValuesMatched(username, Username);

            /// <summary>
            /// Checks if 2 strings are a match for a <see cref="Entry"/> considering that either value can be a wildcard (*)
            /// </summary>
            /// <param name="query">Value being searched</param>
            /// <param name="actual">Value from the PGPASS entry</param>
            /// <returns>True if the values are a match. False otherwise.</returns>
            bool AreValuesMatched([CanBeNull] string query, [CanBeNull] string actual)
                => query == actual || actual == null || query == null;

            bool AreValuesMatched([CanBeNull] int? query, [CanBeNull] int? actual)
                => query == actual || actual == null || query == null;
        }
    }
}
