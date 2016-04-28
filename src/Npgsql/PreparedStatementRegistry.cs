using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;

namespace Npgsql
{
    internal class PreparedStatementRegistry
    {
        /// <summary>
        /// Contains allocated non persisted prepared statement names
        /// </summary>
        readonly HashSet<string> _nonPersistedPreparedStmtNames;

        /// <summary>
        /// Maps command SQLs to corresponding persisted prepared statements
        /// </summary>
        readonly Dictionary<string, PersistentPreparedCommand> _persistedPreparedCommandsBySql;

        internal PreparedStatementRegistry()
        {
            _nonPersistedPreparedStmtNames = new HashSet<string>();
            _persistedPreparedCommandsBySql = new Dictionary<string, PersistentPreparedCommand>();
        }

        /// <summary>
        /// Gets persisted command
        /// </summary>
        /// <param name="commandSQL">Command SQL</param>
        /// <returns>Persisted command or null if persisted command does not exists</returns>
        internal PersistentPreparedCommand GetPersistentPreparedCommand(string commandSQL)
        {
            PersistentPreparedCommand persistent;
            if (_persistedPreparedCommandsBySql.TryGetValue(commandSQL, out persistent))
                return persistent;

            return null;
        }

        /// <summary>
        /// Persists prepared command
        /// </summary>
        /// <param name="persistCommand">Persisted command</param>
        internal void PersistPreparedCommand(PersistentPreparedCommand persistCommand)
        {
            for (int i = 0; i < persistCommand.Statements.Count; i++)
            {
                _nonPersistedPreparedStmtNames.Remove(persistCommand.Statements[i].PreparedStatementName);
            }

            _persistedPreparedCommandsBySql[persistCommand.CommandSQL] = persistCommand;
        }

        /// <summary>
        /// Removes persisted prepared command from collection
        /// </summary>
        /// <param name="commandSQL">Command SQL</param>
        /// <returns>True if persisted command existed otherwise false</returns>
        internal bool RemovePersistedPreparedCommand(string commandSQL)
        {
            return _persistedPreparedCommandsBySql.Remove(commandSQL);
        }

        /// <summary>
        /// Removes non persisted prepared statement from collection
        /// </summary>
        /// <param name="statementName">Statement name</param>
        /// <returns>True if statement existed otherwise false</returns>
        internal bool RemoveNonPersistedPreparedStatement(string statementName)
        {
            return _nonPersistedPreparedStmtNames.Remove(statementName);
        }

        /// <summary>
        /// Clears non persistent statements and returns removed statement names
        /// </summary>
        internal List<string> ClearNonPersistedPreparedStatements()
        {
            var names = new List<string>(_nonPersistedPreparedStmtNames.Count);

            foreach (string statementName in _nonPersistedPreparedStmtNames)
            {
                names.Add(statementName);
            }

            _nonPersistedPreparedStmtNames.Clear();

            return names;
        }

        /// <summary>
        /// Clears all prepared statements and returns removed statement names
        /// </summary>
        internal List<string> ClearAllPreparedStatements()
        {
            var names = new List<string>();

            foreach (string statementName in _nonPersistedPreparedStmtNames)
            {
                names.Add(statementName);
            }

            _nonPersistedPreparedStmtNames.Clear();

            foreach (var kvp in _persistedPreparedCommandsBySql)
            {
                PersistentPreparedCommand persistentCommand = kvp.Value;
                for (int i = 0; i < persistentCommand.Statements.Count; i++)
                {
                    string statementName = persistentCommand.Statements[i].PreparedStatementName;
                    names.Add(statementName);
                }
            }

            _persistedPreparedCommandsBySql.Clear();

            return names;
        }

        /// <summary>
        /// Returns next plan index.
        /// </summary>
        internal string NextPreparedStatementName()
        {
            string statementName = PreparedStatementNamePrefix + Guid.NewGuid().ToString("N");

            _nonPersistedPreparedStmtNames.Add(statementName);

            return statementName;
        }

        const string PreparedStatementNamePrefix = "s";
    }
}
