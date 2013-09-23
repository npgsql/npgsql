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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Text;
using System.Web.Hosting;
using System.Web.Profile;
using NpgsqlTypes;

namespace Npgsql.Web
{
    //using System.Web.DataAccess;

    /// <summary>
    /// custom Profile provider class
    /// </summary>
    public class NpgsqlProfileProvider : ProfileProvider
    {
        #region private properties

        private string _appName;
        private Guid _appId;
        private string _NpgsqlConnectionString;
        private int _commandTimeout;
        private string _table;

        /// <summary>
        /// gets application name
        /// </summary>
        private Guid AppId
        {
            get
            {
                NpgsqlConnection conn = null;
                NpgsqlCommand cmd = null;
                try
                {
                    conn = new NpgsqlConnection(_NpgsqlConnectionString);
                    conn.Open();

                    cmd =
                        new NpgsqlCommand(
                            "SELECT ApplicationId " + " FROM aspnet_applications " +
                            " WHERE LOWER(@applicationname) = LoweredApplicationName ", conn);
                    cmd.Parameters.Add("@applicationname", NpgsqlDbType.Text, 255).Value = ApplicationName;

                    string tmpAppId = null;
                    try
                    {
                        tmpAppId = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                    }

                    if (string.IsNullOrEmpty(tmpAppId)) // == null || tmpAppId == "")
                    {
                        using (
                            NpgsqlCommand cmd1 =
                                new NpgsqlCommand(
                                    " INSERT INTO aspnet_applications (ApplicationId, ApplicationName, LoweredApplicationName) " +
                                    " VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName)) ", conn))
                        {
                            _appId = Guid.NewGuid();
                            cmd1.Parameters.Add("@ApplicationName", NpgsqlDbType.Text, 255).Value = ApplicationName;
                            cmd1.Parameters.Add("@ApplicationId", NpgsqlDbType.Text, 36).Value = _appId.ToString();
                            cmd1.ExecuteBlind();
                        }
                    }
                    else
                    {
                        _appId = new Guid(tmpAppId);
                    }
                }
                finally
                {
                    if (conn != null)
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        conn.Close();
                    }
                }

                return _appId;
            }
        }

        private int CommandTimeout
        {
            get { return _commandTimeout; }
        }

        /// <summary>
        /// Container struct for use in aggregating columns for queries
        /// </summary>
        private struct ProfileColumnData
        {
            public readonly string ColumnName;
            public readonly SettingsPropertyValue PropertyValue;
            public readonly object Value;
            public readonly NpgsqlDbType DataType;

            public ProfileColumnData(string col, SettingsPropertyValue pv, object val, NpgsqlDbType type)
            {
                EnsureValidTableOrColumnName(col);
                ColumnName = col;
                PropertyValue = pv;
                Value = val;
                DataType = type;
            }
        }

        #endregion private properties

        #region Initialization method

        /// <summary>
        /// Initializes settings
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (String.IsNullOrEmpty(name))
            {
                name = "NpgsqlProfileProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "NpgsqlProfileProvider");
            }
            base.Initialize(name, config);

            string temp = config["connectionStringName"];
            if (String.IsNullOrEmpty(temp))
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("connectionStringName not specified");
            }

            //
            // Initialize NpgNpgsqlConnection.
            //
            ConnectionStringSettings pConnectionStringSettings =
                ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (pConnectionStringSettings == null ||
                string.IsNullOrEmpty((pConnectionStringSettings.ConnectionString ?? string.Empty).Trim()))
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("Connection string cannot be blank.");
            }

            _NpgsqlConnectionString = pConnectionStringSettings.ConnectionString;

            _appName = config["applicationName"];
            if (string.IsNullOrEmpty(_appName))
            {
                _appName = HostingEnvironment.ApplicationVirtualPath;
            }

            if (_appName.Length > 256)
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("Application name too long");
            }

            _table = config["table"];
            if (string.IsNullOrEmpty(_table))
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("No table specified");
            }
            EnsureValidTableOrColumnName(_table);

            string timeout = config["commandTimeout"];
            if (string.IsNullOrEmpty(timeout) || !Int32.TryParse(timeout, out _commandTimeout))
            {
                _commandTimeout = 30;
            }

            config.Remove("commandTimeout");
            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("table");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                {
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(string.Format("Unrecognized config attribute:{0}", attribUnrecognized));
                }
            }
        }

        #endregion Initialization method

        #region public properties

        /// <summary>
        /// Gets or sets application name
        /// </summary>
        public override string ApplicationName
        {
            get { return _appName; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ApplicationName");
                }
                if (value.Length > 256)
                {
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException("Application name too long");
                }
                _appName = value;
            }
        }

        #endregion public properties

        #region public methods

        /// <summary>
        /// Gets property values
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
                                                                          SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection svc = new SettingsPropertyValueCollection();

            if (collection == null || collection.Count < 1 || context == null)
            {
                return svc;
            }

            string username = (string) context["UserName"];
            if (String.IsNullOrEmpty(username))
            {
                return svc;
            }

            NpgsqlConnection conn = null;
            try
            {
                conn = new NpgsqlConnection(_NpgsqlConnectionString);
                conn.Open();

                GetProfileDataFromTable(collection, svc, username, conn);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return svc;
        }

        /// <summary>
        /// Sets property values
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection"></param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            string username = (string) context["UserName"];
            bool userIsAuthenticated = (bool) context["IsAuthenticated"];

            if (username == null || username.Length < 1 || collection.Count < 1)
            {
                return;
            }

            NpgsqlConnection conn = null;
            NpgsqlDataReader reader = null;
            NpgsqlCommand cmd = null;
            try
            {
                bool anyItemsToSave = false;

                // First make sure we have at least one item to save
                foreach (SettingsPropertyValue pp in collection)
                {
                    if (pp.IsDirty)
                    {
                        if (!userIsAuthenticated)
                        {
                            bool allowAnonymous = (bool) pp.Property.Attributes["AllowAnonymous"];
                            if (!allowAnonymous)
                            {
                                continue;
                            }
                        }
                        anyItemsToSave = true;
                        break;
                    }
                }

                if (!anyItemsToSave)
                {
                    return;
                }

                conn = new NpgsqlConnection(_NpgsqlConnectionString);
                conn.Open();

                List<ProfileColumnData> columnData = new List<ProfileColumnData>(collection.Count);

                foreach (SettingsPropertyValue pp in collection)
                {
                    if (!userIsAuthenticated)
                    {
                        bool allowAnonymous = (bool) pp.Property.Attributes["AllowAnonymous"];
                        if (!allowAnonymous)
                        {
                            continue;
                        }
                    }

                    //Normal logic for original SQL provider
                    //if (!pp.IsDirty && pp.UsingDefaultValue) // Not fetched from DB and not written to

                    //Can eliminate unnecessary updates since we are using a table though
                    if (!pp.IsDirty)
                    {
                        continue;
                    }

                    string persistenceData = pp.Property.Attributes["CustomProviderData"] as string;
                    // If we can't find the table/column info we will ignore this data
                    if (String.IsNullOrEmpty(persistenceData))
                    {
                        continue;
                    }
                    string[] chunk = persistenceData.Split(new char[] {';'});
                    if (chunk.Length != 2)
                    {
                        continue;
                    }
                    string columnName = chunk[0];
                    NpgsqlDbType datatype = (NpgsqlDbType) Enum.Parse(typeof (NpgsqlDbType), chunk[1], true);

                    object value = null;

                    if (pp.Deserialized && pp.PropertyValue == null)
                    {
                        // is value null?
                        value = DBNull.Value;
                    }
                    else
                    {
                        value = pp.PropertyValue;
                    }

                    columnData.Add(new ProfileColumnData(columnName, pp, value, datatype));
                }

                // Figure out userid, if we don't find a userid, go ahead and create a user in the aspnetUsers table
                Guid userId = Guid.Empty;
                string tmpUserId = null;
                cmd =
                    new NpgsqlCommand(
                        string.Format(
                            "SELECT u.UserId FROM vw_aspnet_Users u WHERE u.ApplicationId = '{0}' AND u.LoweredUserName = LOWER(@Username)",
                            AppId), conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@Username", NpgsqlDbType.Text, 255).Value = username;
                try
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        tmpUserId = reader.GetString(0);
                        if (!string.IsNullOrEmpty(tmpUserId)) // != null && tmpUserId != "")
                        {
                            userId = new Guid(tmpUserId);
                        }
                        else
                        {
                            userId = Guid.NewGuid();
                        }
                    }
                    else
                    {
                        reader.Close();
                        cmd.Dispose();
                        reader = null;

                        cmd =
                            new NpgsqlCommand(
                                " INSERT INTO aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate) " +
                                " VALUES (@ApplicationId, @UserId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate) ", conn);
                        userId = Guid.NewGuid();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@UserId", NpgsqlDbType.Text, 36).Value = userId.ToString();
                        cmd.Parameters.Add("@ApplicationId", NpgsqlDbType.Text, 36).Value = AppId.ToString();
                        cmd.Parameters.Add("@UserName", NpgsqlDbType.Text, 255).Value = username;
                        cmd.Parameters.Add("@IsUserAnonymous", NpgsqlDbType.Boolean).Value = !userIsAuthenticated;
                        cmd.Parameters.Add("@LastActivityDate", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;

                        cmd.ExecuteBlind();
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                    cmd.Dispose();
                }

                // Figure out if the row already exists in the table and use appropriate SELECT/UPDATE
                cmd = new NpgsqlCommand("SELECT * FROM " + _table + " WHERE UserId = @UserId", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@UserId", NpgsqlDbType.Text, 36).Value = userId.ToString();
                NpgsqlDataReader readerSelect = null;
                bool IfExists = false;
                try
                {
                    using (readerSelect = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (readerSelect.HasRows)
                        {
                            // IF EXISTS (SELECT * FROM aspnet_users WHERE UserId = '')
                            IfExists = true;
                        }
                        else
                        {
                            IfExists = false;
                        }
                        readerSelect.Close();
                    }
                }
                catch (NpgsqlException)
                {
                }
                finally
                {
                    if (readerSelect != null)
                    {
                        readerSelect.Close();
                    }
                }

                cmd = new NpgsqlCommand(String.Empty, conn);
                StringBuilder NpgsqlCommand = new StringBuilder();
                // Build up strings used in the query
                StringBuilder columnStr = new StringBuilder();
                StringBuilder valueStr = new StringBuilder();
                StringBuilder setStr = new StringBuilder();
                int count = 0;
                foreach (ProfileColumnData data in columnData)
                {
                    columnStr.Append(", ");
                    valueStr.Append(", ");
                    columnStr.Append(data.ColumnName);
                    string valueParam = "@Value" + count;
                    valueStr.Append(valueParam);
                    cmd.Parameters.Add(valueParam, data.Value);

                    if (data.DataType != NpgsqlDbType.Timestamp)
                    {
                        if (count > 0)
                        {
                            setStr.Append(",");
                        }
                        setStr.Append(data.ColumnName);
                        setStr.Append("=");
                        setStr.Append(valueParam);
                    }

                    ++count;
                }
                columnStr.Append(",LastUpdatedDate ");
                valueStr.Append(",@LastUpdatedDate");
                setStr.Append(",LastUpdatedDate=@LastUpdatedDate");
                cmd.Parameters.Add("@LastUpdatedDate", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;

                if (setStr.ToString().StartsWith(","))
                {
                    setStr.Remove(0, 1);
                }

                if (IfExists)
                {
                    NpgsqlCommand.Append("UPDATE ").Append(_table).Append(" SET ").Append(setStr.ToString());
                    NpgsqlCommand.Append(" WHERE UserId = '").Append(userId).Append("'");
                }
                else
                {
                    NpgsqlCommand.Append("INSERT INTO ").Append(_table).Append(" (UserId").Append(columnStr.ToString());
                    NpgsqlCommand.Append(") VALUES ('").Append(userId).Append("'").Append(valueStr.ToString()).Append(")");
                }

                cmd.CommandText = NpgsqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteBlind();

                // Need to close reader before we try to update
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                UpdateLastActivityDate(conn, userId);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException("profiles");
            }

            if (profiles.Count < 1)
            {
                throw new ArgumentException("Profiles collection is empty");
            }

            string[] usernames = new string[profiles.Count];

            int iter = 0;
            foreach (ProfileInfo profile in profiles)
            {
                usernames[iter++] = profile.UserName;
            }

            return DeleteProfiles(usernames);
        }

        /// <summary>
        /// Deletes Profiles
        /// </summary>
        /// <param name="usernames"></param>
        /// <returns></returns>
        public override int DeleteProfiles(string[] usernames)
        {
            if (usernames == null || usernames.Length < 1)
            {
                return 0;
            }

            int numProfilesDeleted = 0;
            bool beginTranCalled = false;
            NpgsqlConnection conn = null;
            try
            {
                conn = new NpgsqlConnection(_NpgsqlConnectionString);
                conn.Open();

                NpgsqlCommand cmd;
                int numUsersRemaing = usernames.Length;
                while (numUsersRemaing > 0)
                {
                    cmd = new NpgsqlCommand(String.Empty, conn);
                    cmd.Parameters.Add("@UserName0", usernames[usernames.Length - numUsersRemaing]);
                    StringBuilder allUsers = new StringBuilder("@UserName0");
                    numUsersRemaing--;

                    int userIndex = 1;
                    for (int iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                    {
                        // REVIEW: Should we check length of command string instead of parameter lengths?
                        if (allUsers.Length + usernames[iter].Length + 3 >= 4000)
                        {
                            break;
                        }
                        string userNameParam = "@UserName" + userIndex;
                        allUsers.Append(",");
                        allUsers.Append(userNameParam);
                        cmd.Parameters.Add(userNameParam, usernames[iter]);
                        numUsersRemaing--;
                        ++userIndex;
                    }

                    // We don't need to start a transaction if we can finish this in one sql command
                    if (!beginTranCalled && numUsersRemaing > 0)
                    {
                        NpgsqlCommand beginCmd = new NpgsqlCommand("BEGIN TRANSACTION", conn);
                        beginCmd.ExecuteBlind();
                        beginTranCalled = true;
                    }

                    cmd.CommandText =
                        string.Format(
                            "DELETE FROM {0} WHERE UserId IN ( SELECT u.UserId FROM vw_aspnet_Users u WHERE u.ApplicationId = '{1}' AND u.UserName IN ({2}))",
                            _table, AppId, allUsers.ToString());
                    cmd.CommandTimeout = CommandTimeout;
                    numProfilesDeleted += cmd.ExecuteNonQuery();
                }

                if (beginTranCalled)
                {
                    cmd = new NpgsqlCommand("COMMIT TRANSACTION", conn);
                    cmd.ExecuteBlind();
                    beginTranCalled = false;
                }
            }
            catch
            {
                if (beginTranCalled)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("ROLLBACK TRANSACTION", conn);
                    cmd.ExecuteBlind();
                    beginTranCalled = false;
                }
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
            return numProfilesDeleted;
        }

        /// <summary>
        /// Deletes inactive Profiles
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <returns></returns>
        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                   DateTime userInactiveSinceDate)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand cmd = null;
            try
            {
                conn = new NpgsqlConnection(_NpgsqlConnectionString);
                conn.Open();

                cmd = new NpgsqlCommand(GenerateQuery(true, authenticationOption), conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.Parameters.Add("@InactiveSinceDate", NpgsqlDbType.Timestamp).Value = userInactiveSinceDate.ToUniversalTime();

                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        /// <summary>
        /// Gets number of inactive Profiles
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <returns></returns>
        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                        DateTime userInactiveSinceDate)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand cmd = null;
            try
            {
                conn = new NpgsqlConnection(_NpgsqlConnectionString);
                conn.Open();

                cmd = new NpgsqlCommand(GenerateQuery(false, authenticationOption), conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.Parameters.Add("@InactiveSinceDate", NpgsqlDbType.Timestamp).Value = userInactiveSinceDate.ToUniversalTime();

                object o = cmd.ExecuteScalar();
                if (o == null || !(o is int))
                {
                    return 0;
                }
                return (int) o;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        /// <summary>
        /// Gets all Profiles
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex,
                                                             int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            return GetProfilesForQuery(null, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        /// <summary>
        /// Gets all inactive Profiles
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                                     DateTime userInactiveSinceDate, int pageIndex,
                                                                     int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.LastActivityDate <= @InactiveSinceDate");
            NpgsqlParameter[] args = new NpgsqlParameter[1];
            args[0] = CreateInputParam("@InactiveSinceDate", NpgsqlDbType.Timestamp, userInactiveSinceDate.ToUniversalTime());
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        /// <summary>
        /// Finds Profiles by user name
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
                                                                     string usernameToMatch, int pageIndex, int pageSize,
                                                                     out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.UserName LIKE LOWER(@UserName)");
            NpgsqlParameter[] args = new NpgsqlParameter[1];
            args[0] = CreateInputParam("@UserName", NpgsqlDbType.Text, usernameToMatch);
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        /// <summary>
        /// Finds inactive Profiles by user name
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="usernameToMatch"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption,
                                                                             string usernameToMatch,
                                                                             DateTime userInactiveSinceDate, int pageIndex,
                                                                             int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.UserName LIKE LOWER(@UserName) AND u.LastActivityDate <= @InactiveSinceDate");
            NpgsqlParameter[] args = new NpgsqlParameter[2];
            args[0] = CreateInputParam("@InactiveSinceDate", NpgsqlDbType.Timestamp, userInactiveSinceDate.ToUniversalTime());
            args[1] = CreateInputParam("@UserName", NpgsqlDbType.Text, usernameToMatch);
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);
        }

        #endregion public methods

        #region Private methods

        private static readonly string s_legalChars = "_@#$";

        private static void EnsureValidTableOrColumnName(string name)
        {
            for (int i = 0; i < name.Length; ++i)
            {
                if (!Char.IsLetterOrDigit(name[i]) && s_legalChars.IndexOf(name[i]) == -1)
                {
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException("Table and column names cannot contain: " + name[i]);
                }
            }
        }

        /// <summary>
        /// Gets Profile data from table
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="svc"></param>
        /// <param name="username"></param>
        /// <param name="conn"></param>
        private void GetProfileDataFromTable(SettingsPropertyCollection properties, SettingsPropertyValueCollection svc,
                                             string username, NpgsqlConnection conn)
        {
            List<ProfileColumnData> columnData = new List<ProfileColumnData>(properties.Count);
            StringBuilder commandText = new StringBuilder("SELECT u.UserID");
            NpgsqlCommand cmd = new NpgsqlCommand(String.Empty, conn);

            int columnCount = 0;
            foreach (SettingsProperty prop in properties)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(prop);
                svc.Add(value);

                string persistenceData = prop.Attributes["CustomProviderData"] as string;
                // If we can't find the table/column info we will ignore this data
                if (String.IsNullOrEmpty(persistenceData))
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string[] chunk = persistenceData.Split(new char[] {';'});
                if (chunk.Length != 2)
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string columnName = chunk[0];
                // REVIEW: Should we ignore case?
                NpgsqlDbType datatype = (NpgsqlDbType) Enum.Parse(typeof (NpgsqlDbType), chunk[1], true);

                columnData.Add(new ProfileColumnData(columnName, value, null /* not needed for get */, datatype));
                commandText.Append(", ");
                commandText.Append("t." + columnName);
                ++columnCount;
            }

            commandText.Append(" FROM " + _table + " t, vw_aspnet_Users u WHERE u.ApplicationId = '").Append(AppId);
            commandText.Append("' AND u.LoweredUserName = LOWER(@Username) AND t.UserID = u.UserID");
            cmd.CommandText = commandText.ToString();
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@Username", NpgsqlDbType.Text, 255).Value = username;
            NpgsqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
                //If no row exists in the database, then the default Profile values
                //from configuration are used.
                if (reader.Read())
                {
                    Guid userId = reader.GetGuid(0);
                    for (int i = 0; i < columnData.Count; ++i)
                    {
                        object val = reader.GetValue(i + 1);
                        ProfileColumnData colData = columnData[i];
                        SettingsPropertyValue propValue = colData.PropertyValue;

                        //Only initialize a SettingsPropertyValue for non-null values
                        if (!(val is DBNull || val == null))
                        {
                            propValue.PropertyValue = val;
                            propValue.IsDirty = false;
                            propValue.Deserialized = true;
                        }
                    }

                    // need to close reader before we try to update the user
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }

                    UpdateLastActivityDate(conn, userId);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Updates last activity date
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        private static void UpdateLastActivityDate(NpgsqlConnection conn, Guid userId)
        {
            NpgsqlCommand cmd =
                new NpgsqlCommand("UPDATE aspnet_Users SET LastActivityDate = @LastUpdatedDate WHERE UserId = '" + userId + "'",
                                  conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@LastUpdatedDate", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;
            try
            {
                cmd.ExecuteBlind();
            }
            finally
            {
                cmd.Dispose();
            }
        }

        /// <summary>
        /// Creates input parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="dbType"></param>
        /// <param name="objValue"></param>
        /// <returns></returns>
        private static NpgsqlParameter CreateInputParam(string paramName, NpgsqlDbType dbType, object objValue)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, dbType);
            if (objValue == null)
            {
                objValue = String.Empty;
            }
            param.Value = objValue;
            return param;
        }

        /*
                private static NpgsqlParameter CreateOutputParam(string paramName, NpgsqlTypes.NpgsqlDbType dbType, int size) {
                    NpgsqlParameter param = new NpgsqlParameter(paramName, dbType);
                    param.Direction = ParameterDirection.Output;
                    param.Size = size;
                    return param;
                }
        */

        //
        // Mangement APIs from ProfileProvider class
        //

        /// <summary>
        /// Generates Query
        /// </summary>
        /// <param name="delete"></param>
        /// <param name="authenticationOption"></param>
        /// <returns></returns>
        private string GenerateQuery(bool delete, ProfileAuthenticationOption authenticationOption)
        {
            StringBuilder cmdStr = new StringBuilder(200);
            if (delete)
            {
                cmdStr.Append("DELETE FROM ");
            }
            else
            {
                cmdStr.Append("SELECT COUNT(*) FROM ");
            }
            cmdStr.Append(_table);
            cmdStr.Append(" WHERE UserId IN (SELECT u.UserId FROM vw_aspnet_Users u WHERE u.ApplicationId = '").Append(AppId);
            cmdStr.Append("' AND (u.LastActivityDate <= @InactiveSinceDate)");
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    cmdStr.Append(" AND u.IsAnonymous = 1");
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    cmdStr.Append(" AND u.IsAnonymous = 0");
                    break;
                case ProfileAuthenticationOption.All:
                    // Want to delete all profiles here, so nothing more needed
                    break;
            }
            cmdStr.Append(")");
            return cmdStr.ToString();
        }

        // TODO: Implement size
        /// <summary>
        /// Generates temporary insert Query for Get Profiles
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <returns></returns>
        private StringBuilder GenerateTempInsertQueryForGetProfiles(ProfileAuthenticationOption authenticationOption)
        {
            StringBuilder cmdStr = new StringBuilder(200);
            cmdStr.Append("INSERT INTO #PageIndexForProfileUsers (UserId) ");
            cmdStr.Append("SELECT u.UserId FROM vw_aspnet_Users u, ").Append(_table);
            cmdStr.Append(" p WHERE ApplicationId = '").Append(AppId);
            cmdStr.Append("' AND u.UserId = p.UserId");
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    cmdStr.Append(" AND u.IsAnonymous = 1");
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    cmdStr.Append(" AND u.IsAnonymous = 0");
                    break;
                case ProfileAuthenticationOption.All:
                    // Want to delete all profiles here, so nothing more needed
                    break;
            }
            return cmdStr;
        }

        /// <summary>
        /// Gets Profiles for Query
        /// </summary>
        /// <param name="insertArgs"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="insertQuery"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        private ProfileInfoCollection GetProfilesForQuery(NpgsqlParameter[] insertArgs, int pageIndex, int pageSize,
                                                          StringBuilder insertQuery, out int totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("pageIndex");
            }
            if (pageSize < 1)
            {
                throw new ArgumentException("pageSize");
            }

            long lowerBound = (long) pageIndex*pageSize;
            long upperBound = lowerBound + pageSize - 1;
            if (upperBound > Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex and pageSize");
            }

            NpgsqlConnection conn = null;
            NpgsqlDataReader reader = null;
            NpgsqlCommand cmd = null;
            try
            {
                conn = new NpgsqlConnection(_NpgsqlConnectionString);
                conn.Open();

                StringBuilder cmdStr = new StringBuilder(200);
                // Create a temp table TO store the select results
                cmd =
                    new NpgsqlCommand(
                        "CREATE TABLE #PageIndexForProfileUsers(IndexId int IDENTITY (0, 1) NOT NULL, UserId varchar(36))", conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.ExecuteBlind();
                cmd.Dispose();

                insertQuery.Append(" ORDER BY UserName");
                cmd = new NpgsqlCommand(insertQuery.ToString(), conn);
                cmd.CommandTimeout = CommandTimeout;
                if (insertArgs != null)
                {
                    foreach (NpgsqlParameter arg in insertArgs)
                    {
                        cmd.Parameters.Add(arg);
                    }
                }

                cmd.ExecuteBlind();
                cmd.Dispose();

                cmdStr = new StringBuilder(200);
                cmdStr.Append("SELECT u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate FROM vw_aspnet_Users u, ").
                    Append(_table);
                cmdStr.Append(" p, #PageIndexForProfileUsers i WHERE u.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= ");
                cmdStr.Append(lowerBound).Append(" AND i.IndexId <= ").Append(upperBound);
                cmd = new NpgsqlCommand(cmdStr.ToString(), conn);
                cmd.CommandTimeout = CommandTimeout;

                reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                ProfileInfoCollection profiles = new ProfileInfoCollection();
                while (reader.Read())
                {
                    string username;
                    DateTime dtLastActivity, dtLastUpdated = DateTime.UtcNow;
                    bool isAnon;

                    username = reader.GetString(0);
                    isAnon = reader.GetBoolean(1);
                    dtLastActivity = DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc);
                    dtLastUpdated = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc);
                    profiles.Add(new ProfileInfo(username, isAnon, dtLastActivity, dtLastUpdated, 0));
                }
                totalRecords = profiles.Count;

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                cmd.Dispose();

                cmd = new NpgsqlCommand("DROP TABLE #PageIndexForProfileUsers", conn);
                cmd.ExecuteBlind();

                return profiles;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        #endregion Private methods
    }
}
