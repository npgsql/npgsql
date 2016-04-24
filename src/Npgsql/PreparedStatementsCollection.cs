using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;

namespace Npgsql
{
    internal class PreparedStatementsCollection
    {
        /// <summary>
        /// Contains allocated non persisted prepared statement names
        /// </summary>
        readonly HashSet<string> _nonPersistedPreparedStmtNames;

        /// <summary>
        /// Maps command SQLs to corresponding persisted prepared statements
        /// </summary>
        readonly Dictionary<string, PersistentPreparedCommand> _persistedPreparedCommands;

        public PreparedStatementsCollection()
        {
            _nonPersistedPreparedStmtNames = new HashSet<string>();
            _persistedPreparedCommands = new Dictionary<string, PersistentPreparedCommand>();
        }

        /// <summary>
        /// Gets persisted command
        /// </summary>
        /// <param name="commandSQL">Command SQL</param>
        /// <returns>Persisted command or null if persisted command does not exists</returns>
        internal PersistentPreparedCommand GetPersistentPreparedCommand(string commandSQL)
        {
            PersistentPreparedCommand persistent;
            if (_persistedPreparedCommands.TryGetValue(commandSQL, out persistent))
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

            _persistedPreparedCommands[persistCommand.CommandSQL] = persistCommand;
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

            foreach (var kvp in _persistedPreparedCommands)
            {
                PersistentPreparedCommand persistentCommand = kvp.Value;
                for (int i = 0; i < persistentCommand.Statements.Count; i++)
                {
                    string statementName = persistentCommand.Statements[i].PreparedStatementName;
                    names.Add(statementName);
                }
            }

            _persistedPreparedCommands.Clear();

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
