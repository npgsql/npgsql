using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Npgsql
{
    /// <summary>
    /// Represents a hostname, port, database, username, and password combination that has been retrieved from a .pgpass file
    /// </summary>
    class PgPassEntry
    {
        const string PGPASS_WILDCARD = "*";

        #region Fields and Properties

        /// <summary>
        /// Hostname parsed from the .pgpass file
        /// </summary>
        internal string Hostname { get; }
        /// <summary>
        /// Port parsed from the .pgpass file
        /// </summary>
        internal int? Port { get; }
        /// <summary>
        /// Database parsed from the .pgpass file
        /// </summary>
        internal string Database { get; }
        /// <summary>
        /// User name parsed from the .pgpass file
        /// </summary>
        internal string UserName { get; }
        /// <summary>
        /// Password parsed from the .pgpass file
        /// </summary>
        internal string Password { get; }

        #endregion

        #region Construction / Initialization

        /// <summary>
        /// This class represents an entry from the .pgpass file
        /// </summary>
        /// <param name="hostname">Hostname parsed from the .pgpass file</param>
        /// <param name="port">Port parsed from the .pgpass file</param>
        /// <param name="database">Database parsed from the .pgpass file</param>
        /// <param name="userName">User name parsed from the .pgpass file</param>
        /// <param name="password">Password parsed from the .pgpass file</param>
        PgPassEntry(string hostname, int? port, string database, string userName, string password)
        {
            Hostname = hostname;
            Port = port;
            Database = database;
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Creates new <see cref="PgPassEntry"/> based on string in the format hostname:port:database:username:password. The : and \ characters should be escaped with a \.
        /// </summary>
        /// <param name="serializedEntry">string for the entry from the pgpass file</param>
        /// <returns>New instance of <see cref="PgPassEntry"/> for the string</returns>
        /// <exception cref="FormatException">Entry is not formatted as hostname:port:database:username:password or non-wildcard port is not a number</exception>
        internal static PgPassEntry Create(string serializedEntry)
        {
            var parts = Regex.Split(serializedEntry, @"(?<!\\):"); // split on any colons that aren't preceded by a \ (\ indicates that the colon is part of the content and not a separator)
            if (parts == null || parts.Length != 5)
            {
                throw new FormatException(".PGPASS entry was not well-formed. Please ensure all non-comment entries are formatted as hostname:port:database:username:password. If colon is included, it must be escaped like \\:.");
            }

            parts = parts.ToList().Select(part => part.Replace("\\:", ":").Replace("\\\\", "\\")).ToArray();  // remember to un-escape any escaped characters

            int port;
            var wasPortParsed = int.TryParse(parts[1], out port);
            if (!wasPortParsed && parts[1] != PGPASS_WILDCARD)
            {
                throw new FormatException(".PGPASS entry was not formatted correctly. Port must be a valid integer.");
            }

            return new PgPassEntry(parts[0], wasPortParsed ? (int?)port : null, parts[2], parts[3], parts[4]);
        }

        #endregion


        /// <summary>
        /// Checks whether this <see cref="PgPassEntry"/> matches the parameters supplied
        /// </summary>
        /// <param name="hostName">Hostname to check against this entry</param>
        /// <param name="port">Port to check against this entry</param>
        /// <param name="database">Database to check against this entry</param>
        /// <param name="userName">Username to check against this entry</param>
        /// <returns>True if the entry is a match. False otherwise.</returns>
        internal bool IsMatch([CanBeNull]string hostName, [CanBeNull]int? port, [CanBeNull]string database, [CanBeNull]string userName) => 
            AreValuesMatched(hostName, Hostname) && AreValuesMatched(port, Port) && AreValuesMatched(database, Database) && AreValuesMatched(userName, UserName);

        /// <summary>
        /// Checks if 2 strings are a match for a <see cref="PgPassEntry"/> considering that either value can be a wildcard (*)
        /// </summary>
        /// <param name="query">Value being searched</param>
        /// <param name="actual">Value from the PGPASS entry</param>
        /// <returns>True if the values are a match. False otherwise.</returns>
        bool AreValuesMatched(string query, string actual) => 
            query == PGPASS_WILDCARD || actual == PGPASS_WILDCARD || query == actual || query == null || actual == null;

        bool AreValuesMatched(int? query, int? actual) => query == null || actual == null || query == actual;
    }
}
