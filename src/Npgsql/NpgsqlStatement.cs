using System.Collections.Generic;
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
    public sealed class NpgsqlStatement
    {
        /// <summary>
        /// The SQL text of the statement.
        /// </summary>
        public string SQL { get; set; } = string.Empty;

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

        internal List<NpgsqlParameter> InputParameters { get; } = new List<NpgsqlParameter>();

        /// <summary>
        /// The RowDescription message for this query. If null, the query does not return rows (e.g. INSERT)
        /// </summary>
        internal RowDescriptionMessage Description;

        /// <summary>
        /// For prepared statements, holds the server-side prepared statement name.
        /// </summary>
        internal string PreparedStatementName;

        /// <summary>
        /// Whether this statement has already been prepared.
        /// </summary>
        internal bool IsPrepared;

        internal void Reset()
        {
            SQL = string.Empty;
            StatementType = StatementType.Select;
            Rows = 0;
            OID = 0;
            InputParameters.Clear();
            Unprepare();
        }

        internal void Unprepare()
        {
            Description = null;
            PreparedStatementName = null;
            IsPrepared = false;
        }

        internal void ApplyCommandComplete(CommandCompleteMessage msg)
        {
            StatementType = msg.StatementType;
            Rows = msg.Rows;
            OID = msg.OID;
        }

        /// <summary>
        /// Returns the SQL text of the statement.
        /// </summary>
        public override string ToString() => SQL ?? "<none>";
    }
}
