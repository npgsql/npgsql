using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.BackendMessages;

namespace Npgsql
{
    internal class PersistentPreparedStatement
    {
        /// <summary>
        /// Initializes PersistentPreparedStatement
        /// </summary>
        /// <param name="name">Name of the statement in server side</param>
        /// <param name="statementSQL">Statement SQL</param>
        /// <param name="parameters">Parameters of the statement</param>
        /// <param name="rowDescription">Statement row description constructed from server messages</param>
        internal PersistentPreparedStatement(
            string name, 
            string statementSQL, 
            IReadOnlyList<PersistentStatementParameter> parameters, 
            RowDescriptionMessage rowDescription)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (statementSQL == null)
                throw new ArgumentNullException("statementSQL");

            if (parameters == null)
                throw new ArgumentNullException("parameters");

            PreparedStatementName = name;
            StatementSQL = statementSQL;
            Parameters = parameters;
            RowDescription = rowDescription;
        }

        /// <summary>
        /// Name of the prepared statement
        /// </summary>
        internal string PreparedStatementName { get; private set; }

        /// <summary>
        /// Statement SQL
        /// </summary>
        internal string StatementSQL { get; private set; }

        /// <summary>
        /// Parameters of the statement
        /// </summary>
        internal IReadOnlyList<PersistentStatementParameter> Parameters { get; private set; }

        /// <summary>
        /// Row description of the prepared statement
        /// </summary>
        internal RowDescriptionMessage RowDescription { get; private set; }
    }
}
