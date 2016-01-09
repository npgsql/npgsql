using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;

namespace Npgsql
{
    /// <summary>
    /// Represents a single SQL statement within Npgsql.
    ///
    /// Instances aren't constructed directly; users should construct an <see cref="NpgsqlCommand"/>
    /// object and populate its <see cref="NpgsqlCommand.CommandText"/> property as in standard ADO.NET.
    /// Npgsql will analyze that property and constructed instances of <see cref="NpgsqlStatement"/>
    /// internally.
    ///
    /// Users can retrieve instances from <see cref="NpgsqlDataReader.Statements"/>
    /// and access information about statement execution (e.g. affected rows).
    /// </summary>
    public class NpgsqlStatement
    {
        internal NpgsqlStatement(string sql, List<NpgsqlParameter> inputParameters, string preparedStatementName = null)
        {
            SQL = sql;
            InputParameters = inputParameters;
            PreparedStatementName = preparedStatementName;
        }

        /// <summary>
        /// The SQL text of the statement.
        /// </summary>
        public string SQL { get; }

        /// <summary>
        /// Specifies the type of query, e.g. SELECT.
        /// </summary>
        public StatementType StatementType { get; internal set; }

        /// <summary>
        /// The number of rows affected or retrieved.
        /// </summary>
        /// <remarks>
        /// See the command tag in the CommandComplete message,
        /// http://www.postgresql.org/docs/current/static/protocol-message-formats.html
        /// </remarks>
        public uint Rows { get; internal set; }

        /// <summary>
        /// For an INSERT, the object ID of the inserted row if <see cref="Rows"/> is 1 and
        /// the target table has OIDs; otherwise 0.
        /// </summary>
        public uint OID { get; internal set; }

        internal readonly List<NpgsqlParameter> InputParameters;

        /// <summary>
        /// The RowDescription message for this query. If null, the query does not return rows (e.g. INSERT)
        /// </summary>
        internal RowDescriptionMessage Description;

        /// <summary>
        /// For prepared statements, holds the server-side prepared statement name.
        /// </summary>
        internal string PreparedStatementName;

        /// <summary>
        /// Returns the SQL text of the statement.
        /// </summary>
        public override string ToString() { return SQL; }
    }
}
