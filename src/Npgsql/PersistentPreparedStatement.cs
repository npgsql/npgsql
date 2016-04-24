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
        public PersistentPreparedStatement(
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

            if (rowDescription == null)
                throw new ArgumentNullException("rowDescription");

            PreparedStatementName = name;
            StatementSQL = statementSQL;
            Parameters = parameters;
            RowDescription = rowDescription;
        }

        /// <summary>
        /// Name of the prepared statement
        /// </summary>
        public string PreparedStatementName { get; private set; }

        /// <summary>
        /// Statement SQL
        /// </summary>
        public string StatementSQL { get; private set; }

        /// <summary>
        /// Parameters of the statement
        /// </summary>
        public IReadOnlyList<PersistentStatementParameter> Parameters { get; private set; }

        /// <summary>
        /// Row description of the prepared statement
        /// </summary>
        public RowDescriptionMessage RowDescription { get; private set; }
    }
}
