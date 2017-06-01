using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
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

        /// <summary>
        /// The input parameters sent with this statement.
        /// </summary>
        public List<NpgsqlParameter> InputParameters { get; } = new List<NpgsqlParameter>();

        /// <summary>
        /// The RowDescription message for this query. If null, the query does not return rows (e.g. INSERT)
        /// </summary>
        [CanBeNull]
        internal RowDescriptionMessage Description
        {
            get { return PreparedStatement == null ? _description : PreparedStatement.Description; }
            set
            {
                if (PreparedStatement == null)
                    _description = value;
                else
                    PreparedStatement.Description = value;
            }
        }

        [CanBeNull]
        RowDescriptionMessage _description;

        /// <summary>
        /// If this statement has been automatically prepared, references the <see cref="PreparedStatement"/>.
        /// Null otherwise.
        /// </summary>
        [CanBeNull]
        internal PreparedStatement PreparedStatement
        {
            get
            {
                if (_preparedStatement != null && _preparedStatement.State == PreparedState.Unprepared)
                    _preparedStatement = null;
                return _preparedStatement;
            }
            set => _preparedStatement = value;
        }

        [CanBeNull]
        PreparedStatement _preparedStatement;

        /// <summary>
        /// Holds the server-side (prepared) statement name. Empty string for non-prepared statements.
        /// </summary>
        internal string StatementName => PreparedStatement?.Name ?? "";

        /// <summary>
        /// Whether this statement has already been prepared (including automatic preparation).
        /// </summary>
        internal bool IsPrepared => PreparedStatement?.IsPrepared == true;

        internal void Reset()
        {
            SQL = string.Empty;
            StatementType = StatementType.Select;
            _description = null;
            Rows = 0;
            OID = 0;
            InputParameters.Clear();
            PreparedStatement = null;
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
