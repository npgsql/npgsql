#if NET45 || NET452 || DNX452

#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
#endregion

using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NpgsqlTypes;

namespace Npgsql
{
    ///<summary>
    /// This class is responsible to create database commands for automatic insert, update and delete operations.
    ///</summary>
    [System.ComponentModel.DesignerCategory("")]
    public sealed class NpgsqlCommandBuilder : DbCommandBuilder
    {
        // Commented out because SetRowUpdatingHandler() is commented, and causes an "is never used" warning
        // private NpgsqlRowUpdatingEventHandler rowUpdatingHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommandBuilder"/> class.
        /// </summary>
        public NpgsqlCommandBuilder()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlCommandBuilder"/> class.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        public NpgsqlCommandBuilder(NpgsqlDataAdapter adapter)
        {
            DataAdapter = adapter;
            QuotePrefix = "\"";
            QuoteSuffix = "\"";
        }

        /// <summary>
        /// Gets or sets the beginning character or characters to use when specifying database objects (for example, tables or columns) whose names contain characters such as spaces or reserved tokens.
        /// </summary>
        /// <returns>
        /// The beginning character or characters to use. The default is an empty string.
        ///   </returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        ///   </PermissionSet>
        public override string QuotePrefix
        {
            get { return base.QuotePrefix; }
            // TODO: Why should it be possible to remove the QuotePrefix?
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    base.QuotePrefix = value;
                }
                else
                {
                    base.QuotePrefix = "\"";
                }
            }
        }

        /// <summary>
        /// Gets or sets the ending character or characters to use when specifying database objects (for example, tables or columns) whose names contain characters such as spaces or reserved tokens.
        /// </summary>
        /// <returns>
        /// The ending character or characters to use. The default is an empty string.
        ///   </returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        ///   </PermissionSet>
        public override string QuoteSuffix
        {
            get { return base.QuoteSuffix; }
            // TODO: Why should it be possible to remove the QuoteSuffix?
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    base.QuoteSuffix = value;
                }
                else
                {
                    base.QuoteSuffix = "\"";
                }
            }
        }

        ///<summary>
        ///
        /// This method is reponsible to derive the command parameter list with values obtained from function definition.
        /// It clears the Parameters collection of command. Also, if there is any parameter type which is not supported by Npgsql, an InvalidOperationException will be thrown.
        /// Parameters name will be parameter1, parameter2, ...
        ///</summary>
        /// <param name="command">NpgsqlCommand whose function parameters will be obtained.</param>
        public static void DeriveParameters(NpgsqlCommand command)
        {
            try
            {
                DoDeriveParameters(command);
            }
            catch
            {
                command.Parameters.Clear();
                throw;
            }
        }

        private static void DoDeriveParameters(NpgsqlCommand command)
        {
            // See http://www.postgresql.org/docs/current/static/catalog-pg-proc.html
            command.Parameters.Clear();
            // Updated after 0.99.3 to support the optional existence of a name qualifying schema and case insensitivity when the schema ror procedure name do not contain a quote.
            // This fixed an incompatibility with NpgsqlCommand.CheckFunctionReturn(String ReturnType)
            var serverVersion = command.Connection.Connector.ServerVersion;
            string query;
            string procedureName;
            string schemaName = null;
            var fullName = command.CommandText.Split('.');
            if (fullName.Length > 1 && fullName[0].Length > 0)
            {
                // proargsmodes is supported for Postgresql 8.1 and above
                query = serverVersion >= new Version(8, 1, 0)
                    ? "select proargnames, proargtypes, proallargtypes, proargmodes from pg_proc p left join pg_namespace n on p.pronamespace = n.oid where proname=:proname and n.nspname=:nspname"
                    : "select proargnames, proargtypes from pg_proc p left join pg_namespace n on p.pronamespace = n.oid where proname=:proname and n.nspname=:nspname";
                schemaName = (fullName[0].IndexOf("\"") != -1) ? fullName[0] : fullName[0].ToLower();
                procedureName = (fullName[1].IndexOf("\"") != -1) ? fullName[1] : fullName[1].ToLower();

                // The pg_temp pseudo-schema is special - it's an alias to a real schema name (e.g. pg_temp_2).
                // We get the real name with pg_my_temp_schema().
                if (schemaName == "pg_temp")
                {
                    using (var c = new NpgsqlCommand("SELECT nspname FROM pg_namespace WHERE oid=pg_my_temp_schema()", command.Connection))
                    {
                        schemaName = (string)c.ExecuteScalar();
                    }
                }
            }
            else
            {
                // proargsmodes is supported for Postgresql 8.1 and above
                query = serverVersion >= new Version(8, 1, 0)
                    ? "select proargnames, proargtypes, proallargtypes, proargmodes from pg_proc where proname = :proname"
                    : "select proargnames, proargtypes from pg_proc where proname = :proname";
                procedureName = (fullName[0].IndexOf("\"") != -1) ? fullName[0] : fullName[0].ToLower();
            }

            using (var c = new NpgsqlCommand(query, command.Connection))
            {
                c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Text));
                c.Parameters[0].Value = procedureName.Replace("\"", "").Trim();
                if (fullName.Length > 1 && !string.IsNullOrEmpty(schemaName))
                {
                    var prm = c.Parameters.Add(new NpgsqlParameter("nspname", NpgsqlDbType.Text));
                    prm.Value = schemaName.Replace("\"", "").Trim();
                }

                string[] names = null;
                uint[] types = null;
                char[] modes = null;

                using (var rdr = c.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                            names = rdr.GetValue(0) as string[];
                        if (serverVersion >= new Version("8.1.0"))
                        {
                            if (!rdr.IsDBNull(2))
                                types = rdr.GetValue(2) as uint[];
                            if (!rdr.IsDBNull(3))
                                modes = rdr.GetValue(3) as char[];
                        }
                        if (types == null)
                        {
                            if (rdr.IsDBNull(1) || rdr.GetFieldValue<uint[]>(1).Length == 0)
                                return;  // Parameterless function
                            types = rdr.GetFieldValue<uint[]>(1);
                        }
                    }
                    else
                        throw new InvalidOperationException($"{command.CommandText} does not exist in pg_proc");
                }

                command.Parameters.Clear();
                for (var i = 0; i < types.Length; i++)
                {
                    var param = new NpgsqlParameter();

                    // TODO: Fix enums, composite types
                    var npgsqlDbType = c.Connection.Connector.TypeHandlerRegistry[types[i]].NpgsqlDbType;
                    if (npgsqlDbType == NpgsqlDbType.Unknown)
                        throw new InvalidOperationException($"Invalid parameter type: {types[i]}");
                    param.NpgsqlDbType = npgsqlDbType;

                    if (names != null && i < names.Length)
                        param.ParameterName = ":" + names[i];
                    else
                        param.ParameterName = "parameter" + (i + 1);

                    if (modes == null) // All params are IN, or server < 8.1.0 (and only IN is supported)
                        param.Direction = ParameterDirection.Input;
                    else
                    {
                        switch (modes[i])
                        {
                            case 'i':
                                param.Direction = ParameterDirection.Input;
                                break;
                            case 'o':
                                param.Direction = ParameterDirection.Output;
                                break;
                            case 'b':
                                param.Direction = ParameterDirection.InputOutput;
                                break;
                            case 'v':
                                throw new NotImplementedException("Cannot derive function parameter of type VARIADIC");
                            case 't':
                                throw new NotImplementedException("Cannot derive function parameter of type TABLE");
                            default:
                                throw new ArgumentOutOfRangeException("proargmode", modes[i],
                                    "Unknown code in proargmodes while deriving: " + modes[i]);
                        }
                    }

                    command.Parameters.Add(param);
                }
            }
        }

        /// <summary>
        /// Gets the automatically generated <see cref="NpgsqlCommand"/> object required
        /// to perform insertions at the data source.
        /// </summary>
        /// <returns>
        /// The automatically generated <see cref="NpgsqlCommand"/> object required to perform insertions.
        /// </returns>
        public new NpgsqlCommand GetInsertCommand()
        {
            return GetInsertCommand(false);
        }

        /// <summary>
        /// Gets the automatically generated <see cref="NpgsqlCommand"/> object required to perform insertions
        /// at the data source, optionally using columns for parameter names.
        /// </summary>
        /// <param name="useColumnsForParameterNames">
        /// If <c>true</c>, generate parameter names matching column names, if possible.
        /// If <c>false</c>, generate @p1, @p2, and so on.
        /// </param>
        /// <returns>
        /// The automatically generated <see cref="NpgsqlCommand"/> object required to perform insertions.
        /// </returns>
        public new NpgsqlCommand GetInsertCommand(bool useColumnsForParameterNames)
        {
            NpgsqlCommand cmd = (NpgsqlCommand) base.GetInsertCommand(useColumnsForParameterNames);
            cmd.UpdatedRowSource = UpdateRowSource.None;
            return cmd;
        }

        /// <summary>
        /// Gets the automatically generated System.Data.Common.DbCommand object required
        /// to perform updates at the data source.
        /// </summary>
        /// <returns>
        /// The automatically generated System.Data.Common.DbCommand object required to perform updates.
        /// </returns>
        public new NpgsqlCommand GetUpdateCommand()
        {
            return GetUpdateCommand(false);
        }

        /// <summary>
        /// Gets the automatically generated <see cref="NpgsqlCommand"/> object required to perform updates
        /// at the data source, optionally using columns for parameter names.
        /// </summary>
        /// <param name="useColumnsForParameterNames">
        /// If <c>true</c>, generate parameter names matching column names, if possible.
        /// If <c>false</c>, generate @p1, @p2, and so on.
        /// </param>
        /// <returns>
        /// The automatically generated <see cref="NpgsqlCommand"/> object required to perform updates.
        /// </returns>
        public new NpgsqlCommand GetUpdateCommand(bool useColumnsForParameterNames)
        {
            NpgsqlCommand cmd = (NpgsqlCommand)base.GetUpdateCommand(useColumnsForParameterNames);
            cmd.UpdatedRowSource = UpdateRowSource.None;
            return cmd;
        }

        /// <summary>
        /// Gets the automatically generated System.Data.Common.DbCommand object required
        /// to perform deletions at the data source.
        /// </summary>
        /// <returns>
        /// The automatically generated System.Data.Common.DbCommand object required to perform deletions.
        /// </returns>
        public new NpgsqlCommand GetDeleteCommand()
        {
            return GetDeleteCommand(false);
        }

        /// <summary>
        /// Gets the automatically generated <see cref="NpgsqlCommand"/> object required to perform deletions
        /// at the data source, optionally using columns for parameter names.
        /// </summary>
        /// <param name="useColumnsForParameterNames">
        /// If <c>true</c>, generate parameter names matching column names, if possible.
        /// If <c>false</c>, generate @p1, @p2, and so on.
        /// </param>
        /// <returns>
        /// The automatically generated <see cref="NpgsqlCommand"/> object required to perform deletions.
        /// </returns>
        public new NpgsqlCommand GetDeleteCommand(bool useColumnsForParameterNames)
        {
            NpgsqlCommand cmd = (NpgsqlCommand) base.GetDeleteCommand(useColumnsForParameterNames);
            cmd.UpdatedRowSource = UpdateRowSource.None;
            return cmd;
        }

        //never used
        //private string QualifiedTableName(string schema, string tableName)
        //{
        //    if (schema == null || schema.Length == 0)
        //    {
        //        return tableName;
        //    }
        //    else
        //    {
        //        return schema + "." + tableName;
        //    }
        //}

/*
        private static void SetParameterValuesFromRow(NpgsqlCommand command, DataRow row)
        {
            foreach (NpgsqlParameter parameter in command.Parameters)
            {
                parameter.Value = row[parameter.SourceColumn, parameter.SourceVersion];
            }
        }
*/

        /// <summary>
        /// Applies the parameter information.
        /// </summary>
        /// <param name="p">The parameter.</param>
        /// <param name="row">The row.</param>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="whereClause">if set to <c>true</c> [where clause].</param>
        protected override void ApplyParameterInfo(DbParameter p, DataRow row, System.Data.StatementType statementType, bool whereClause)
        {
            // TODO: We may need to set NpgsqlDbType, as well as other properties, on p
        }

        /// <summary>
        /// Returns the name of the specified parameter in the format of @p#.
        /// </summary>
        /// <param name="parameterOrdinal">The number to be included as part of the parameter's name..</param>
        /// <returns>
        /// The name of the parameter with the specified number appended as part of the parameter name.
        /// </returns>
        protected override string GetParameterName(int parameterOrdinal)
        {
            return String.Format(CultureInfo.InvariantCulture, "@p{0}", parameterOrdinal);
        }

        /// <summary>
        /// Returns the full parameter name, given the partial parameter name.
        /// </summary>
        /// <param name="parameterName">The partial name of the parameter.</param>
        /// <returns>
        /// The full parameter name corresponding to the partial parameter name requested.
        /// </returns>
        protected override string GetParameterName(string parameterName)
        {
            return String.Format(CultureInfo.InvariantCulture, "@{0}", parameterName);
        }

        /// <summary>
        /// Returns the placeholder for the parameter in the associated SQL statement.
        /// </summary>
        /// <param name="parameterOrdinal">The number to be included as part of the parameter's name.</param>
        /// <returns>
        /// The name of the parameter with the specified number appended.
        /// </returns>
        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
            return GetParameterName(parameterOrdinal);
        }

        /// <summary>
        /// Registers the <see cref="T:NpgsqlCommandBuilder" /> to handle the <see cref="E:NpgsqlDataAdapter.RowUpdating"/> event for a <see cref="T:NpgsqlDataAdapter" />.
        /// </summary>
        /// <param name="adapter">The <see cref="T:System.Data.Common.DbDataAdapter" /> to be used for the update.</param>
        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            var npgsqlAdapter = adapter as NpgsqlDataAdapter;
            if (npgsqlAdapter == null)
                throw new ArgumentException("adapter needs to be a NpgsqlDataAdapter", nameof(adapter));

            // Being called twice for the same adapter means unregister
            if (adapter == DataAdapter)
                npgsqlAdapter.RowUpdating -= RowUpdatingHandler;
            else
                npgsqlAdapter.RowUpdating += RowUpdatingHandler;
        }

        /// <summary>
        /// Adds an event handler for the <see cref="NpgsqlDataAdapter.RowUpdating"/> event.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">A <see cref="NpgsqlRowUpdatingEventArgs"/> instance containing information about the event.</param>
        private void RowUpdatingHandler(object sender, NpgsqlRowUpdatingEventArgs e)
        {
            base.RowUpdatingHandler(e);
        }

        /// <summary>
        /// Given an unquoted identifier in the correct catalog case, returns the correct quoted form of that identifier, including properly escaping any embedded quotes in the identifier.
        /// </summary>
        /// <param name="unquotedIdentifier">The original unquoted identifier.</param>
        /// <returns>
        /// The quoted version of the identifier. Embedded quotes within the identifier are properly escaped.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        ///   </PermissionSet>
        /// <exception cref="System.ArgumentNullException">Unquoted identifier parameter cannot be null</exception>
        public override string QuoteIdentifier(string unquotedIdentifier)

        {
            if (unquotedIdentifier == null)

            {
                throw new ArgumentNullException(nameof(unquotedIdentifier), "Unquoted identifier parameter cannot be null");
            }

            return $"{QuotePrefix}{unquotedIdentifier.Replace(QuotePrefix, QuotePrefix + QuotePrefix)}{QuoteSuffix}";
        }

        /// <summary>
        /// Given a quoted identifier, returns the correct unquoted form of that identifier, including properly un-escaping any embedded quotes in the identifier.
        /// </summary>
        /// <param name="quotedIdentifier">The identifier that will have its embedded quotes removed.</param>
        /// <returns>
        /// The unquoted identifier, with embedded quotes properly un-escaped.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        ///   </PermissionSet>
        /// <exception cref="System.ArgumentNullException">Quoted identifier parameter cannot be null</exception>
        public override string UnquoteIdentifier(string quotedIdentifier)

        {
            if (quotedIdentifier == null)

            {
                throw new ArgumentNullException(nameof(quotedIdentifier), "Quoted identifier parameter cannot be null");
            }

            var unquotedIdentifier = quotedIdentifier.Trim().Replace(QuotePrefix + QuotePrefix, QuotePrefix);

            if (unquotedIdentifier.StartsWith(QuotePrefix))

            {
                unquotedIdentifier = unquotedIdentifier.Remove(0, 1);
            }

            if (unquotedIdentifier.EndsWith(QuoteSuffix))

            {
                unquotedIdentifier = unquotedIdentifier.Remove(unquotedIdentifier.Length - 1, 1);
            }

            return unquotedIdentifier;
        }
    }
}
#endif
