// created on 21/5/2002 at 20:03

// Npgsql.NpgsqlCommand.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
// 
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// 
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using NpgsqlTypes;

#if WITHDESIGN

#endif

namespace Npgsql
{
    /// <summary>
    /// Represents a SQL statement or function (stored procedure) to execute
    /// against a PostgreSQL database. This class cannot be inherited.
    /// </summary>
#if WITHDESIGN
    [System.Drawing.ToolboxBitmapAttribute(typeof(NpgsqlCommand)), ToolboxItem(true)]
#endif

    public sealed class NpgsqlCommand : DbCommand, ICloneable
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Regex parameterReplace = new Regex(@"([:@][\w\.]*)", RegexOptions.Singleline|RegexOptions.Compiled);
        private static readonly Regex POSTGRES_TEXT_ARRAY = new Regex(@"^array\[+'", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private NpgsqlConnection connection;
        private NpgsqlConnector m_Connector; //renamed to account for hiding it in a local function
        //if all locals were named with this prefix, it would solve LOTS of issues.
        private NpgsqlTransaction transaction;
        private String text;
        private Int32 timeout;
        private CommandType type;
        private readonly NpgsqlParameterCollection parameters = new NpgsqlParameterCollection();
        private String planName;
        private Boolean designTimeVisible;

        private NpgsqlParse parse;
        private NpgsqlBind bind;

        private Int64 lastInsertedOID = 0;
        
        // locals about function support so we don`t need to check it everytime a function is called.
        
        private Boolean functionChecksDone = false;
        
        private Boolean addProcedureParenthesis = false; // Do not add procedure parenthesis by default.

        private Boolean functionNeedsColumnListDefinition = false; // Functions don't return record by default.

        private Boolean commandTimeoutSet = false;

        private UpdateRowSource updateRowSource = UpdateRowSource.Both;


        // Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> class.
        /// </summary>
        public NpgsqlCommand()
            : this(String.Empty, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> class with the text of the query.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        public NpgsqlCommand(String cmdText)
            : this(cmdText, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> class with the text of the query and a <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        public NpgsqlCommand(String cmdText, NpgsqlConnection connection)
            : this(cmdText, connection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> class with the text of the query, a <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>, and the <see cref="Npgsql.NpgsqlTransaction">NpgsqlTransaction</see>.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">A <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see> that represents the connection to a PostgreSQL server.</param>
        /// <param name="transaction">The <see cref="Npgsql.NpgsqlTransaction">NpgsqlTransaction</see> in which the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> executes.</param>
        public NpgsqlCommand(String cmdText, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);

            planName = String.Empty;
            text = cmdText;
            this.connection = connection;

            if (this.connection != null)
            {
                this.m_Connector = connection.Connector;
            }

            type = CommandType.Text;
            this.Transaction = transaction;

            SetCommandTimeout();
        }

        /// <summary>
        /// Used to execute internal commands.
        /// </summary>
        internal NpgsqlCommand(String cmdText, NpgsqlConnector connector)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);


            planName = String.Empty;
            text = cmdText;
            this.m_Connector = connector;
            type = CommandType.Text;

            // Removed this setting. It was causing too much problem.
            // Do internal commands really need different timeout setting?
            // Internal commands aren't affected by command timeout value provided by user.
            // timeout = 20;
        }

        // Public properties.
        /// <summary>
        /// Gets or sets the SQL statement or function (stored procedure) to execute at the data source.
        /// </summary>
        /// <value>The Transact-SQL statement or stored procedure to execute. The default is an empty string.</value>
        [Category("Data"), DefaultValue("")]
        public override String CommandText
        {
            get { return text; }

            set
            {
                // [TODO] Validate commandtext.
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "CommandText", value);
                text = value;
                planName = String.Empty;
                parse = null;
                bind = null;
                
                functionChecksDone = false;
            }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt
        /// to execute a command and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for the command to execute.
        /// The default is 20 seconds.</value>
        [DefaultValue(20)]
        public override Int32 CommandTimeout
        {
            get { return timeout; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(resman.GetString("Exception_CommandTimeoutLessZero"));
                }

                timeout = value;
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "CommandTimeout", value);

                commandTimeoutSet = true;

            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the
        /// <see cref="Npgsql.NpgsqlCommand.CommandText">CommandText</see> property is to be interpreted.
        /// </summary>
        /// <value>One of the <see cref="System.Data.CommandType">CommandType</see> values. The default is <see cref="System.Data.CommandType">CommandType.Text</see>.</value>
        [Category("Data"), DefaultValue(CommandType.Text)]
        public override CommandType CommandType
        {
            get { return type; }

            set
            {
                type = value;
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "CommandType", value);
            }
        }

        protected override DbConnection DbConnection
        {
            get { return Connection; }

            set
            {
                Connection = (NpgsqlConnection)value;
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "DbConnection", value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Npgsql.NpgsqlConnection">NpgsqlConnection</see>
        /// used by this instance of the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <value>The connection to a data source. The default value is a null reference.</value>
        [Category("Behavior"), DefaultValue(null)]
        public new NpgsqlConnection Connection
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "Connection");
                return connection;
            }

            set
            {
                if (this.Connection == value)
                {
                    return;
                }

                //if (this.transaction != null && this.transaction.Connection == null)
                //  this.transaction = null;

                // All this checking needs revising. It should be simpler.
                // This this.Connector != null check was added to remove the nullreferenceexception in case
                // of the previous connection has been closed which makes Connector null and so the last check would fail.
                // See bug 1000581 for more details.
                if (this.transaction != null && this.connection != null && this.Connector != null && this.Connector.Transaction != null)
                {
                    throw new InvalidOperationException(resman.GetString("Exception_SetConnectionInTransaction"));
                }

                this.connection = value;
                Transaction = null;
                if (this.connection != null)
                {
                    m_Connector = this.connection.Connector;
                }

                SetCommandTimeout();

                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "Connection", value);
            }
        }

        internal NpgsqlConnector Connector
        {
            get
            {
                if (this.connection != null)
                {
                    m_Connector = this.connection.Connector;
                }

                return m_Connector;
            }
        }

        internal Type[] ExpectedTypes { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return Parameters; }
        }

        /// <summary>
        /// Gets the <see cref="Npgsql.NpgsqlParameterCollection">NpgsqlParameterCollection</see>.
        /// </summary>
        /// <value>The parameters of the SQL statement or function (stored procedure). The default is an empty collection.</value>
#if WITHDESIGN
        [Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif

        public new NpgsqlParameterCollection Parameters
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "Parameters");
                return parameters;
            }
        }


        protected override DbTransaction DbTransaction
        {
            get { return Transaction; }
            set
            {
                Transaction = (NpgsqlTransaction)value;
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "IDbCommand.Transaction", value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Npgsql.NpgsqlTransaction">NpgsqlTransaction</see>
        /// within which the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see> executes.
        /// </summary>
        /// <value>The <see cref="Npgsql.NpgsqlTransaction">NpgsqlTransaction</see>.
        /// The default value is a null reference.</value>
#if WITHDESIGN
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif

        public new NpgsqlTransaction Transaction
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "Transaction");

                if (this.transaction != null && this.transaction.Connection == null)
                {
                    this.transaction = null;
                }
                return this.transaction;
            }

            set
            {
                NpgsqlEventLog.LogPropertySet(LogLevel.Debug, CLASSNAME, "Transaction", value);

                this.transaction = value;
            }
        }

        /// <summary>
        /// Gets or sets how command results are applied to the <see cref="System.Data.DataRow">DataRow</see>
        /// when used by the <see cref="System.Data.Common.DbDataAdapter.Update(DataSet)">Update</see>
        /// method of the <see cref="System.Data.Common.DbDataAdapter">DbDataAdapter</see>.
        /// </summary>
        /// <value>One of the <see cref="System.Data.UpdateRowSource">UpdateRowSource</see> values.</value>
#if WITHDESIGN
        [Category("Behavior"), DefaultValue(UpdateRowSource.Both)]
#endif

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "UpdatedRowSource");

                return updateRowSource;
            }
            set 
            {
                switch (value)
                {
                        // validate value (required based on base type contract)
                    case UpdateRowSource.None:
                    case UpdateRowSource.OutputParameters:
                    case UpdateRowSource.FirstReturnedRecord:
                    case UpdateRowSource.Both:
                        updateRowSource = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        /// Returns oid of inserted row. This is only updated when using executenonQuery and when command inserts just a single row. If table is created without oids, this will always be 0.
        /// </summary>
        public Int64 LastInsertedOID
        {
            get { return lastInsertedOID; }
        }


        /// <summary>
        /// Attempts to cancel the execution of a <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        /// <remarks>This Method isn't implemented yet.</remarks>
        public override void Cancel()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Cancel");

            try
            {
                // get copy for thread safety of null test
                NpgsqlConnector connector = Connector;
                if (connector != null)
                {
                    connector.CancelRequest();
                }
            }
            catch (IOException)
            {
                Connection.ClearPool();
            }
            catch (NpgsqlException)
            {
                // Cancel documentation says the Cancel doesn't throw on failure
            }
        }

        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        Object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a new command based on this one.
        /// </summary>
        /// <returns>A new NpgsqlCommand object.</returns>
        public NpgsqlCommand Clone()
        {
            // TODO: Add consistency checks.

            NpgsqlCommand clone = new NpgsqlCommand(CommandText, Connection, Transaction);
            clone.CommandTimeout = CommandTimeout;
            clone.CommandType = CommandType;
            clone.DesignTimeVisible = DesignTimeVisible;
            if (ExpectedTypes != null)
            {
                clone.ExpectedTypes = (Type[])ExpectedTypes.Clone();
            }
            foreach (NpgsqlParameter parameter in Parameters)
            {
                clone.Parameters.Add(parameter.Clone());
            }
            return clone;
        }

        /// <summary>
        /// Creates a new instance of an <see cref="System.Data.Common.DbParameter">DbParameter</see> object.
        /// </summary>
        /// <returns>An <see cref="System.Data.Common.DbParameter">DbParameter</see> object.</returns>
        protected override DbParameter CreateDbParameter()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CreateDbParameter");

            return CreateParameter();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="Npgsql.NpgsqlParameter">NpgsqlParameter</see> object.
        /// </summary>
        /// <returns>A <see cref="Npgsql.NpgsqlParameter">NpgsqlParameter</see> object.</returns>
        public new NpgsqlParameter CreateParameter()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CreateParameter");

            return new NpgsqlParameter();
        }

        /// <summary>
        /// Slightly optimised version of ExecuteNonQuery() for internal ues in cases where the number
        /// of affected rows is of no interest.
        /// </summary>
        internal void ExecuteBlind()
        {
            GetReader(CommandBehavior.SequentialAccess).Dispose();
        }

        /// <summary>
        /// Executes a SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <returns>The number of rows affected if known; -1 otherwise.</returns>
        public override Int32 ExecuteNonQuery()
        {
            //We treat this as a simple wrapper for calling ExecuteReader() and then
            //update the records affected count at every call to NextResult();
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ExecuteNonQuery");
            int? ret = null;
            using (NpgsqlDataReader rdr = GetReader(CommandBehavior.SequentialAccess))
            {
                do
                {
                    int thisRecord = rdr.RecordsAffected;
                    if (thisRecord != -1)
                    {
                        ret = (ret ?? 0) + thisRecord;
                    }
                    lastInsertedOID = rdr.LastInsertedOID ?? lastInsertedOID;
                }
                while (rdr.NextResult());
            }
            return ret ?? -1;
        }

        /// <summary>
        /// Sends the <see cref="Npgsql.NpgsqlCommand.CommandText">CommandText</see> to
        /// the <see cref="Npgsql.NpgsqlConnection">Connection</see> and builds a
        /// <see cref="Npgsql.NpgsqlDataReader">NpgsqlDataReader</see>
        /// using one of the <see cref="System.Data.CommandBehavior">CommandBehavior</see> values.
        /// </summary>
        /// <param name="behavior">One of the <see cref="System.Data.CommandBehavior">CommandBehavior</see> values.</param>
        /// <returns>A <see cref="Npgsql.NpgsqlDataReader">NpgsqlDataReader</see> object.</returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return ExecuteReader(behavior);
        }

        /// <summary>
        /// Sends the <see cref="Npgsql.NpgsqlCommand.CommandText">CommandText</see> to
        /// the <see cref="Npgsql.NpgsqlConnection">Connection</see> and builds a
        /// <see cref="Npgsql.NpgsqlDataReader">NpgsqlDataReader</see>.
        /// </summary>
        /// <returns>A <see cref="Npgsql.NpgsqlDataReader">NpgsqlDataReader</see> object.</returns>
        public new NpgsqlDataReader ExecuteReader()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ExecuteReader");

            return ExecuteReader(CommandBehavior.Default);
        }

        /// <summary>
        /// Sends the <see cref="Npgsql.NpgsqlCommand.CommandText">CommandText</see> to
        /// the <see cref="Npgsql.NpgsqlConnection">Connection</see> and builds a
        /// <see cref="Npgsql.NpgsqlDataReader">NpgsqlDataReader</see>
        /// using one of the <see cref="System.Data.CommandBehavior">CommandBehavior</see> values.
        /// </summary>
        /// <param name="cb">One of the <see cref="System.Data.CommandBehavior">CommandBehavior</see> values.</param>
        /// <returns>A <see cref="Npgsql.NpgsqlDataReader">NpgsqlDataReader</see> object.</returns>
        /// <remarks>Currently the CommandBehavior parameter is ignored.</remarks>
        public new NpgsqlDataReader ExecuteReader(CommandBehavior cb)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ExecuteReader", cb);
            
            // Close connection if requested even when there is an error.

                    try
                    {
                        if (connection != null)
                        {
                            if (connection.PreloadReader)
                            {
                                //Adjust behaviour so source reader is sequential access - for speed - and doesn't close the connection - or it'll do so at the wrong time.
                                CommandBehavior adjusted = (cb | CommandBehavior.SequentialAccess) & ~CommandBehavior.CloseConnection;
                                return new CachingDataReader(GetReader(adjusted), cb);
                            }
                        }
                    return GetReader(cb);
                    }
                    catch (Exception)
                    {
                        if ((cb & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
            {
                            connection.Close();
            }
                        throw;
                    }
                
        }

        internal ForwardsOnlyDataReader GetReader(CommandBehavior cb)
        {
            try
            {
                CheckConnectionState();

                // reset any responses just before getting new ones
                Connector.Mediator.ResetResponses();

                // Set command timeout.
                m_Connector.Mediator.CommandTimeout = CommandTimeout;


                using (m_Connector.BlockNotificationThread())
                {
                    ForwardsOnlyDataReader reader;
                    if (parse == null)
                    {
                        reader = new ForwardsOnlyDataReader(m_Connector.QueryEnum(this), cb, this,
                                                            m_Connector.BlockNotificationThread(), false);
                        if (type == CommandType.StoredProcedure
                            && reader.FieldCount == 1
                            && reader.GetDataTypeName(0) == "refcursor")
                        {
                            // When a function returns a sole column of refcursor, transparently
                            // FETCH ALL from every such cursor and return those results.
                            StringBuilder sb = new StringBuilder();
                            while (reader.Read())
                            {
                                sb.Append("fetch all from \"").Append(reader.GetString(0)).Append("\";");
                            }
                            sb.Append(";"); // Just in case the list of cursors is empty.

                            //reader = new NpgsqlCommand(sb.ToString(), Connection).GetReader(reader._behavior);

                            // Passthrough the commandtimeout to the inner command, so user can also control its timeout.
                            // TODO: Check if there is a better way to handle that.

                            NpgsqlCommand c = new NpgsqlCommand(sb.ToString(), Connection);

                            c.CommandTimeout = this.CommandTimeout;

                            reader = c.GetReader(reader._behavior);

                        }
                    }
                    else
                    {
                        BindParameters();
                        reader = new ForwardsOnlyDataReader(m_Connector.ExecuteEnum(new NpgsqlExecute(bind.PortalName, 0)), cb, this,
                                                            m_Connector.BlockNotificationThread(), true);
                    }
                    return reader;
                }
            }
            catch (IOException ex)
            {
                throw ClearPoolAndCreateException(ex);
            }
        }

        ///<summary>
        /// This method binds the parameters from parameters collection to the bind
        /// message.
        /// </summary>
        private void BindParameters()
        {
            if (parameters.Count != 0)
            {
                Object[] parameterValues = new Object[parameters.Count];
                Int16[] parameterFormatCodes = bind.ParameterFormatCodes;

                for (Int32 i = 0; i < parameters.Count; i++)
                {
                    // Do not quote strings, or escape existing quotes - this will be handled by the backend.
                    // DBNull or null values are returned as null.
                    // TODO: Would it be better to remove this null special handling out of ConvertToBackend??

                    // Do special handling of bytea values. They will be send in binary form.
                    // TODO: Add binary format support for all supported types. Not only bytea.
                    if (parameters[i].TypeInfo.NpgsqlDbType != NpgsqlDbType.Bytea)
                    {
                        parameterValues[i] = parameters[i].TypeInfo.ConvertToBackend(parameters[i].Value, true);
                    }
                    else
                    {
                        if (parameters[i].Value != DBNull.Value)
                        {
                            parameterFormatCodes[i] = (Int16)FormatCode.Binary;
                            parameterValues[i] = (byte[])parameters[i].Value;
                        }
                        else
                        {
                            parameterValues[i] = parameters[i].TypeInfo.ConvertToBackend(parameters[i].Value, true);
                        }
                    }
                }
                bind.ParameterValues = parameterValues;
                bind.ParameterFormatCodes = parameterFormatCodes;
            }

            try
            {
                // In case of error when binding parameters, the ReadyForQuery isn't returned.
                // According to docs: "[...] The response is either BindComplete or ErrorResponse."

                Connector.RequireReadyForQuery = false;

                Connector.Bind(bind);

                Connector.Flush();
            }
            catch
            {
                // Check catch{} of Preapre method for discussion about that.
                Connector.Sync();
                throw;
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row
        /// in the result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <returns>The first column of the first row in the result set,
        /// or a null reference if the result set is empty.</returns>
        public override Object ExecuteScalar()
        {
            using (
                NpgsqlDataReader reader =
                    GetReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
                return reader.Read() && reader.FieldCount != 0 ? reader.GetValue(0) : null;
            }
        }

        /// <summary>
        /// Creates a prepared version of the command on a PostgreSQL server.
        /// </summary>
        public override void Prepare()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Prepare");

            // Check the connection state.
            CheckConnectionState();

            // reset any responses just before getting new ones
            Connector.Mediator.ResetResponses();

            // Set command timeout.
            m_Connector.Mediator.CommandTimeout = CommandTimeout;

            if (!m_Connector.SupportsPrepare)
            {
                return; // Do nothing.
            }

            if (m_Connector.BackendProtocolVersion == ProtocolVersion.Version2)
            {
                using (NpgsqlCommand command = new NpgsqlCommand(GetPrepareCommandText(), m_Connector))
                {
                    command.ExecuteBlind();
                }
            }
            else
            {
                using (m_Connector.BlockNotificationThread())
                {
                    try
                    {
                        // Use the extended query parsing...
                        planName = m_Connector.NextPlanName();
                        String portalName = m_Connector.NextPortalName();

                        parse = new NpgsqlParse(planName, GetParseCommandText(), new Int32[] { });

                        m_Connector.Parse(parse);

                        // We need that because Flush() doesn't cause backend to send
                        // ReadyForQuery on error. Without ReadyForQuery, we don't return 
                        // from query extended processing.

                        // We could have used Connector.Flush() which sends us back a
                        // ReadyForQuery, but on postgresql server below 8.1 there is an error
                        // with extended query processing which hinders us from using it.
                        m_Connector.RequireReadyForQuery = false;
                        m_Connector.Flush();


                        // Description...
                        NpgsqlDescribe describe = new NpgsqlDescribe('S', planName);


                        m_Connector.Describe(describe);

                        NpgsqlRowDescription returnRowDesc = m_Connector.Sync();

                        Int16[] resultFormatCodes;


                        if (returnRowDesc != null)
                        {
                            resultFormatCodes = new Int16[returnRowDesc.NumFields];

                            for (int i = 0; i < returnRowDesc.NumFields; i++)
                            {
                                NpgsqlRowDescription.FieldData returnRowDescData = returnRowDesc[i];


                                if (returnRowDescData.TypeInfo != null && returnRowDescData.TypeInfo.NpgsqlDbType == NpgsqlDbType.Bytea)
                                {
                                    // Binary format
                                    resultFormatCodes[i] = (Int16)FormatCode.Binary;
                                }
                                else
                                {
                                    // Text Format
                                    resultFormatCodes[i] = (Int16)FormatCode.Text;
                                }
                            }
                        }
                        else
                        {
                            resultFormatCodes = new Int16[] { 0 };
                        }

                        bind = new NpgsqlBind("", planName, new Int16[Parameters.Count], null, resultFormatCodes);
                    }
                    catch (IOException e)
                    {
                        throw ClearPoolAndCreateException(e);
                    }
                    catch
                    {

                        // As per documentation:

                        // "[...] When an error is detected while processing any extended-query message,

                        // the backend issues ErrorResponse, then reads and discards messages until a

                        // Sync is reached, then issues ReadyForQuery and returns to normal message processing.[...]"

                        // So, send a sync command if we get any problems.



                        m_Connector.Sync();

                        throw;
                    }
                }
            }
        }

        /*
        /// <summary>
        /// Releases the resources used by the <see cref="Npgsql.NpgsqlCommand">NpgsqlCommand</see>.
        /// </summary>
        protected override void Dispose (bool disposing)
        {
            
            if (disposing)
            {
                // Only if explicitly calling Close or dispose we still have access to 
                // managed resources.
                NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Dispose");
                if (connection != null)
                {
                    connection.Dispose();
                }
                base.Dispose(disposing);
                
            }
        }*/

        ///<summary>
        /// This method checks the connection state to see if the connection
        /// is set or it is open. If one of this conditions is not met, throws
        /// an InvalidOperationException
        ///</summary>
        private void CheckConnectionState()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CheckConnectionState");


            // Check the connection state.
            if (Connector == null || Connector.State == ConnectionState.Closed)
            {
                throw new InvalidOperationException(resman.GetString("Exception_ConnectionNotOpen"));
            }
            if (Connector.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "There is already an open DataReader associated with this Command which must be closed first.");
            }
        }

        /// <summary>
        /// This method substitutes the <see cref="Npgsql.NpgsqlCommand.Parameters">Parameters</see>, if exist, in the command
        /// to their actual values.
        /// The parameter name format is <b>:ParameterName</b>.
        /// </summary>
        /// <returns>A version of <see cref="Npgsql.NpgsqlCommand.CommandText">CommandText</see> with the <see cref="Npgsql.NpgsqlCommand.Parameters">Parameters</see> inserted.</returns>
        internal StringBuilder GetCommandText()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetCommandText");

            StringBuilder ret = string.IsNullOrEmpty(planName) ? GetClearCommandText() : GetPreparedCommandText();
            // In constructing the command text, we potentially called internal
            // queries.  Reset command timeout and SQL sent.
            m_Connector.Mediator.ResetResponses();
            m_Connector.Mediator.CommandTimeout = CommandTimeout;
            return ret;

        }

        private static void PassEscapedArray(StringBuilder query, string array)
        {
            bool inTextLiteral = false;
            int endAt = array.Length - 1;//leave last char for separate append as we don't have to continually check we're safe to add the next char too.
            for(int i = 0; i != endAt; ++i)
            {
                if(array[i] == '\'')
                {
                    if(!inTextLiteral)
                    {
                        query.Append("E'");
                        inTextLiteral = true;
                    }
                    else if(array[i + 1] == '\'')//SQL-escaped '
                    {
                        query.Append("''");
                        ++i;
                    }
                    else
                    {
                        query.Append('\'');
                        inTextLiteral = false;
                    }
                }
                else
                    query.Append(array[i]);
            }
            query.Append(array[endAt]);
        }
        
        private void PassParam(StringBuilder query, NpgsqlParameter p)
        {
                string serialised = p.TypeInfo.ConvertToBackend(p.Value, false);

                // Add parentheses wrapping parameter value before the type cast to avoid problems with Int16.MinValue, Int32.MinValue and Int64.MinValue
                // See bug #1010543
                // Check if this parenthesis can be collapsed with the previous one about the array support. This way, we could use
                // only one pair of parentheses for the two purposes instead of two pairs.
                query.Append('(');

                if(Connector.UseConformantStrings)
                    switch(serialised[0])
                    {
                    case '\''://type passed as string or string with type.
                        //We could test to see if \ is used anywhere, but then we could be doing quite an expensive check (if the value is large) for little gain.
                        query.Append("E").Append(serialised);
                        break;
                    case 'a':
                        if(POSTGRES_TEXT_ARRAY.IsMatch(serialised))
                            PassEscapedArray(query, serialised);
                        else
                            query.Append(serialised);
                        break;
                    default:
                        query.Append(serialised);
                        break;
                    }
                else
                    query.Append(serialised);

                query.Append(')');


                if (p.UseCast)
                {
                    query.Append("::").Append(p.TypeInfo.CastName);
                    if (p.TypeInfo.UseSize && (p.Size > 0))
                        query.Append('(').Append(p.Size).Append(')');
                }
        }

        private StringBuilder GetClearCommandText()
        {
            if (NpgsqlEventLog.Level == LogLevel.Debug)
            {
                NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetClearCommandText");
            }

            StringBuilder result = PGUtil.TrimStringBuilder(new StringBuilder(text));

            switch(type)
            {
                case CommandType.TableDirect:
                    return result.Insert(0, "select * from "); // There is no parameter support on table direct.
                case CommandType.StoredProcedure:
                    if (!functionChecksDone)
                    {
                        functionNeedsColumnListDefinition = Parameters.Count != 0 && CheckFunctionNeedsColumnDefinitionList();

                        // Check if just procedure name was passed. If so, does not replace parameter names and just pass parameter values in order they were added in parameters collection. Also check if command text finishes in a ";" which would make Npgsql incorrectly append a "()" when executing this command text.
                        switch(result[result.Length - 1])
                        {
                            case ')' : case ';':
                                addProcedureParenthesis = false;
                                break;
                            default:
                                addProcedureParenthesis = true;
                                break;
                        }
                        
                        functionChecksDone = true;
                    }

                    result.Insert(0,
                        Connector.SupportsPrepare
                        ? "select * from " // This syntax is only available in 7.3+ as well SupportsPrepare.
                        : "select " //Only a single result return supported. 7.2 and earlier.
                       );
                    break;
            }

            if (parameters.Count == 0)
            {
                if (addProcedureParenthesis)
                    result.Append("()");

                if (functionNeedsColumnListDefinition)
                    AddFunctionColumnListSupport(result);
               
                return result;
            }

            // Get parameters in query string to translate them to their actual values.

            // This regular expression gets all the parameters in format :param or @param
            // and everythingelse.
            // This is only needed if query string has parameters. Else, just append the
            // parameter values in order they were put in parameter collection.


            // If parenthesis don't need to be added, they were added by user with parameter names. Replace them.
            if (!addProcedureParenthesis)
            {
                Dictionary<string, NpgsqlParameter> parameterIndex = new Dictionary<string, NpgsqlParameter>(parameters.Count, StringComparer.InvariantCultureIgnoreCase);
                foreach (NpgsqlParameter parameter in parameters)
                    parameterIndex[parameter.CleanName] = parameter;

                StringBuilder sb = new StringBuilder();
                foreach (String s in parameterReplace.Split(result.ToString()))
                    if (s.Length != 0)
                    {
                        NpgsqlParameter p = null;
                        string parameterName = s;
                        if ((parameterName[0] == ':') || (parameterName[0] == '@'))
                        {
                            parameterName = parameterName.Remove(0, 1);
                            parameterIndex.TryGetValue(parameterName, out p);
                        }

                        if (p != null)
                        {
                            switch(p.Direction)
                            {
                                case ParameterDirection.Input: case ParameterDirection.InputOutput:
                                    //Wrap in probably-redundant parentheses. Queries should operate much as if they were in the a parameter or
                                    //variable in a postgres function. Generally this is the case without the parentheses (hence "probably redundant")
                                    //but there are exceptions to this rule. E.g. consider the postgres function:
                                    //
                                    //CREATE FUNCTION first_param(integer[])RETURNS int AS'select $1[1]'LANGUAGE 'sql' STABLE STRICT;
                                    //
                                    //The equivalent commandtext would be "select :param[1]", but this fails without the parentheses.

                                    sb.Append('('); PassParam(sb, p); sb.Append(')');
                                    break;
                            }
                        }
                        else
                        {
                            sb.Append(s);
                        }
                    }
                result = sb;
            }

            else
            {
                result.Append('(');
                
                for (Int32 i = 0; i < parameters.Count; i++)
                {
                    switch(parameters[i].Direction)
                    {
                        case ParameterDirection.Input: case ParameterDirection.InputOutput:
                            PassParam(result, parameters[i]);
                            result.Append(',');
                            break;
                    }
                }

                // Remove a trailing comma added from parameter handling above. If any.
                // Maybe there are only output parameters. If so, there will be no comma.
                if (result[result.Length - 1] == ',')
                    result = result.Remove(result.Length - 1, 1);
                result.Append(')');
            }

            if (functionNeedsColumnListDefinition)
            {
                AddFunctionColumnListSupport(result);
            }

            return result;
        }

        private Boolean CheckFunctionNeedsColumnDefinitionList()
        {
            // If and only if a function returns "record" and has no OUT ("o" in proargmodes), INOUT ("b"), or TABLE
            // ("t") return arguments to characterize the result columns, we must provide a column definition list.
            // See http://pgfoundry.org/forum/forum.php?thread_id=1075&forum_id=519
            // We would use our Output and InputOutput parameters to construct that column definition list.  If we have
            // no such parameters, skip the check: we could only construct "AS ()", which yields a syntax error.

            // Updated after 0.99.3 to support the optional existence of a name qualifying schema and allow for case insensitivity
            // when the schema or procedure name do not contain a quote.
            // The hard-coded schema name 'public' was replaced with code that uses schema as a qualifier, only if it is provided.

            String returnRecordQuery;

            StringBuilder parameterTypes = new StringBuilder("");


            // Process parameters

            Boolean seenDef = false;
            foreach (NpgsqlParameter p in Parameters)
            {
                if ((p.Direction == ParameterDirection.Input) || (p.Direction == ParameterDirection.InputOutput))
                {
                    parameterTypes.Append(Connection.Connector.OidToNameMapping[p.TypeInfo.Name].OID.ToString() + " ");
                }

                if ((p.Direction == ParameterDirection.Output) || (p.Direction == ParameterDirection.InputOutput))
                {
                    seenDef = true;
                }
            }
            
            if (!seenDef)
            {
                return false;
            }


            // Process schema name.

            String schemaName = String.Empty;
            String procedureName = String.Empty;


            String[] fullName = CommandText.Split('.');

            String predicate = "prorettype = ( select oid from pg_type where typname = 'record' ) "
                + "and proargtypes=:proargtypes and proname=:proname "
                // proargmodes && array['o','b','t']::"char"[] performs just as well, but it requires PostgreSQL 8.2.
                + "and ('o' = any (proargmodes) OR 'b' = any (proargmodes) OR 't' = any (proargmodes)) is not true";
            if (fullName.Length == 2)
            {
                returnRecordQuery =
                "select count(*) > 0 from pg_proc p left join pg_namespace n on p.pronamespace = n.oid where " + predicate + " and n.nspname=:nspname";

                schemaName = (fullName[0].IndexOf("\"") != -1) ? fullName[0] : fullName[0].ToLower();
                procedureName = (fullName[1].IndexOf("\"") != -1) ? fullName[1] : fullName[1].ToLower();
            }
            else
            {
                // Instead of defaulting don't use the nspname, as an alternative, query pg_proc and pg_namespace to try and determine the nspname.
                //schemaName = "public"; // This was removed after build 0.99.3 because the assumption that a function is in public is often incorrect.
                returnRecordQuery =
                    "select count(*) > 0 from pg_proc p where " + predicate;

                procedureName = (CommandText.IndexOf("\"") != -1) ? CommandText : CommandText.ToLower();
            }


            bool ret;

            using (NpgsqlCommand c = new NpgsqlCommand(returnRecordQuery, Connection))
            {
                c.Parameters.Add(new NpgsqlParameter("proargtypes", NpgsqlDbType.Oidvector));
                c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Name));

                c.Parameters[0].Value = parameterTypes.ToString();
                c.Parameters[1].Value = procedureName;

                if (schemaName != null && schemaName.Length > 0)
                {
                    c.Parameters.Add(new NpgsqlParameter("nspname", NpgsqlDbType.Name));
                    c.Parameters[2].Value = schemaName;
                }

                ret = (Boolean)c.ExecuteScalar();
            }

            // reset any responses just before getting new ones
            m_Connector.Mediator.ResetResponses();

            // Set command timeout.
            m_Connector.Mediator.CommandTimeout = CommandTimeout;

            return ret;
        }

        private void AddFunctionColumnListSupport(StringBuilder sb)
        {
            sb.Append(" as (");
            foreach (NpgsqlParameter p in Parameters)
                switch(p.Direction)
                {
                    case ParameterDirection.Output: case ParameterDirection.InputOutput:
                        sb.Append(p.CleanName).Append(" ").Append(p.TypeInfo.Name).Append(",");
                        break;
                }
            sb[sb.Length - 1] = ')';
        }


        private StringBuilder GetPreparedCommandText()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetPreparedCommandText");

            StringBuilder result = new StringBuilder("execute ").Append(planName);

            if(parameters.Count != 0)
            {
                result.Append('(');

                foreach(NpgsqlParameter p in parameters)
                {
                    // Add parentheses wrapping parameter value before the type cast to avoid problems with Int16.MinValue, Int32.MinValue and Int64.MinValue
                    // See bug #1010543
                    result.Append('(');
                    result.Append(p.TypeInfo.ConvertToBackend(p.Value, false));
                    result.Append(')');
                    if (p.UseCast)
                    {
                        result.Append("::").Append(p.TypeInfo.CastName);
                        if (p.TypeInfo.UseSize && (p.Size > 0))
                            result.Append('(').Append(p.Size).Append(')');
                    }
                    result.Append(',');
                }

                result[result.Length - 1] = ')';
            }
            return result;
        }


        private String GetParseCommandText()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetParseCommandText");

            Boolean addProcedureParenthesis = false; // Do not add procedure parenthesis by default.

            String parseCommand = text;

            if (type == CommandType.StoredProcedure)
            {
                // Check if just procedure name was passed. If so, does not replace parameter names and just pass parameter values in order they were added in parameters collection.
                if (!parseCommand.Trim().EndsWith(")"))
                {
                    addProcedureParenthesis = true;
                    parseCommand += "(";
                }

                parseCommand = string.Format("select * from {0}", parseCommand); // This syntax is only available in 7.3+ as well SupportsPrepare.
            }
            else
            {
                if (type == CommandType.TableDirect)
                {
                    return string.Format("select * from {0}", parseCommand); // There is no parameter support on TableDirect.
                }
            }
            if (parameters.Count > 0)
            {
                // The ReplaceParameterValue below, also checks if the parameter is present.

                String parameterName;
                Int32 i;

                for (i = 0; i < parameters.Count; i++)
                {
                    if ((parameters[i].Direction == ParameterDirection.Input) ||
                        (parameters[i].Direction == ParameterDirection.InputOutput))
                    {
                        
                        string parameterSize = "";

                        if (parameters[i].TypeInfo.UseSize && (parameters[i].Size > 0))
                        {
                            parameterSize += string.Format("({0})", parameters[i].Size);
                        }
                        

                        if (!addProcedureParenthesis)
                        {
                            //result = result.Replace(":" + parameterName, parameters[i].Value.ToString());
                            parameterName = parameters[i].CleanName;
                            //textCommand = textCommand.Replace(':' + parameterName, "$" + (i+1));
                            
                            // Just add typecast if needed.
                            if (parameters[i].UseCast)
                                parseCommand = ReplaceParameterValue(parseCommand, parameterName, string.Format("${0}::{1}{2}", (i + 1), parameters[i].TypeInfo.CastName, parameterSize));
                            else
                                parseCommand = ReplaceParameterValue(parseCommand, parameterName, string.Format("${0}{1}", (i + 1), parameterSize));
                        }
                        else
                        {
                            if (parameters[i].UseCast)
                                parseCommand += string.Format("${0}::{1}{2}", (i + 1), parameters[i].TypeInfo.CastName, parameterSize);
                            else
                                parseCommand += string.Format("${0}{1}", (i + 1), parameterSize);
                        }
                       
                    
                    }
                }
            }


            return string.Format("{0}{1}", parseCommand, addProcedureParenthesis ? ")" : string.Empty);


            //if (addProcedureParenthesis)
            //{
            //    return parseCommand + ")";
            //}
            //else
            //{
            //    return parseCommand;
            //}
        }


        private String GetPrepareCommandText()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetPrepareCommandText");

            Boolean addProcedureParenthesis = false; // Do not add procedure parenthesis by default.

            planName = Connector.NextPlanName();

            StringBuilder command = new StringBuilder("prepare " + planName);

            String textCommand = text;

            if (type == CommandType.StoredProcedure)
            {
                // Check if just procedure name was passed. If so, does not replace parameter names and just pass parameter values in order they were added in parameters collection.
                if (!textCommand.Trim().EndsWith(")"))
                {
                    addProcedureParenthesis = true;
                    textCommand += "(";
                }

                textCommand = "select * from " + textCommand;
            }
            else if (type == CommandType.TableDirect)
            {
                return "select * from " + textCommand; // There is no parameter support on TableDirect.
            }


            if (parameters.Count > 0)
            {
                // The ReplaceParameterValue below, also checks if the parameter is present.

                String parameterName;
                Int32 i;

                for (i = 0; i < parameters.Count; i++)
                {
                    if ((parameters[i].Direction == ParameterDirection.Input) ||
                        (parameters[i].Direction == ParameterDirection.InputOutput))
                    {
                        if (!addProcedureParenthesis)
                        {
                            //result = result.Replace(":" + parameterName, parameters[i].Value.ToString());
                            parameterName = parameters[i].CleanName;
                            // The space in front of '$' fixes a parsing problem in 7.3 server
                            // which gives errors of operator when finding the caracters '=$' in
                            // prepare text
                            textCommand = ReplaceParameterValue(textCommand, parameterName, " $" + (i + 1).ToString());
                        }
                        else
                        {
                            textCommand += " $" + (i + 1).ToString();
                        }
                    }
                }

                //[TODO] Check if there are any missing parameters in the query.
                // For while, an error is thrown saying about the ':' char.

                command.Append('(');

                for (i = 0; i < parameters.Count; i++)
                {
                    //                    command.Append(NpgsqlTypesHelper.GetDefaultTypeInfo(parameters[i].DbType));
                    if (parameters[i].UseCast)
                        command.Append(parameters[i].TypeInfo.Name);
                    else
                        command.Append("unknown");

                    command.Append(',');
                }

                command = command.Remove(command.Length - 1, 1);
                command.Append(')');
            }

            if (addProcedureParenthesis)
            {
                textCommand += ")";
            }

            command.Append(" as ");
            command.Append(textCommand);


            return command.ToString();
        }


        private static String ReplaceParameterValue(String result, String parameterName, String paramVal)
        {
            String quote_pattern = @"['][^']*[']";
            string parameterMarker = string.Empty;
            // search parameter marker since it is not part of the name
            String pattern = "[- |\n\r\t,)(;=+/<>][:|@]" + parameterMarker + parameterName + "([- |\n\r\t,)(;=+/<>]|$)";
            Int32 start, end;
            String withoutquote = result;
            Boolean found = false;
            // First of all
            // Suppress quoted string from query (because we ave to ignore them)
            MatchCollection results = Regex.Matches(result, quote_pattern);
            foreach (Match match in results)
            {
                start = match.Index;
                end = match.Index + match.Length;
                String spaces = new String(' ', match.Length - 2);
                withoutquote = withoutquote.Substring(0, start + 1) + spaces + withoutquote.Substring(end - 1);
            }
            do
            {
                // Now we look for the searched parameters on the "withoutquote" string
                results = Regex.Matches(withoutquote, pattern);
                if (results.Count == 0)
                {
                    // If no parameter is found, go out!
                    break;
                }
                // We take the first parameter found
                found = true;
                Match match = results[0];
                start = match.Index;
                if ((match.Length - parameterName.Length) == 3)
                {
                    // If the found string is not the end of the string
                    end = match.Index + match.Length - 1;
                }
                else
                {
                    // If the found string is the end of the string
                    end = match.Index + match.Length;
                }
                result = result.Substring(0, start + 1) + paramVal + result.Substring(end);
                withoutquote = withoutquote.Substring(0, start + 1) + paramVal + withoutquote.Substring(end);
            }
            while (true);
            if (!found)
            {
                throw new IndexOutOfRangeException(String.Format(resman.GetString("Exception_ParamNotInQuery"), parameterName));
            }
            return result;
        }

        private void SetCommandTimeout()
        {
            if (commandTimeoutSet)
                return;

            if (Connection != null)
            {
                timeout = Connection.CommandTimeout;
            }
            else
            {
                timeout = (int)NpgsqlConnectionStringBuilder.GetDefaultValue(Keywords.CommandTimeout);
            }
        }

        internal NpgsqlException ClearPoolAndCreateException(Exception e)
        {
            Connection.ClearPool();
            return new NpgsqlException(resman.GetString("Exception_ConnectionBroken"), e);
        }

        public override bool DesignTimeVisible
        {
            get { return designTimeVisible; }
            set { designTimeVisible = value; }
        }
    }
}