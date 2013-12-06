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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Web.Hosting;
using System.Web.Security;
using NpgsqlTypes;

/*

CREATE TABLE Role
(
  role_name Varchar (255) NOT NULL,
  application_name Varchar (255) NOT NULL,
  PRIMARY KEY (role_name, application_name)
);

CREATE TABLE UsersInRoles
(
  user_name Varchar (255) NOT NULL,
  role_name Varchar (255) NOT NULL,
  application_name Text NOT NULL,
  PRIMARY KEY ( user_name , role_name , application_name )
);

*/

namespace Npgsql.Web
{
    public sealed class NpgsqlRoleProvider : RoleProvider
    {
        //
        // Global connection string, generic exception message, event log info.
        //

        private readonly string rolesTable = "roles";
        private readonly string usersInRolesTable = "users_in_roles";

        private readonly string eventSource = "NpgsqlRoleProvider";
        private readonly string eventLog = "Application";
        private readonly string exceptionMessage = "An exception occurred. Please check the Event Log.";

        private ConnectionStringSettings pConnectionStringSettings;
        private string connectionString;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog = false;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }

        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (name == null || name.Length == 0)
            {
                name = "NpgsqlRoleProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Npgsql Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            if (string.IsNullOrEmpty((config["applicationName"] ?? string.Empty).Trim()))
            //if (config["applicationName"] == null || config["applicationName"].Trim() == "")
            {
                pApplicationName = HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                pApplicationName = config["applicationName"];
            }

            if (config["writeExceptionsToEventLog"] != null)
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                {
                    pWriteExceptionsToEventLog = true;
                }
            }

            //
            // Initialize NpgsqlConnection.
            //

            pConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (pConnectionStringSettings == null ||
                string.IsNullOrEmpty((pConnectionStringSettings.ConnectionString ?? string.Empty).Trim()))
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = pConnectionStringSettings.ConnectionString;
        }

        //
        // System.Web.Security.RoleProvider properties.
        //

        private string pApplicationName;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        //
        // System.Web.Security.RoleProvider methods.
        //

        //
        // RoleProvider.AddUsersToRoles
        //

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                if (username.IndexOf(',') > 0)
                {
                    throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is already in role.");
                    }
                }
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "INSERT INTO " + usersInRolesTable + "" + " (user_name, role_name, application_name) " +
                    " Values(@user_name, @role_name, @application_name)", conn);

            NpgsqlParameter userParm = cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255);
            NpgsqlParameter roleParm = cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255);
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Transaction = tran;

                foreach (string username in usernames)
                {
                    foreach (string rolename in rolenames)
                    {
                        userParm.Value = username;
                        roleParm.Value = rolename;
                        cmd.ExecuteBlind();
                    }
                }

                tran.Commit();
            }
            catch (NpgsqlException e)
            {
                try
                {
                    if (tran != null)
                    {
                        tran.Rollback();
                    }
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "AddUsersToRoles");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }

        //
        // RoleProvider.CreateRole
        //

        public override void CreateRole(string rolename)
        {
            if (rolename.IndexOf(',') > 0)
            {
                throw new ArgumentException("Role names cannot contain commas.");
            }

            if (RoleExists(rolename))
            {
                throw new ProviderException("Role name already exists.");
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "INSERT INTO " + rolesTable + "" + " (role_name, application_name) " + " Values(@role_name, @application_name)",
                    conn);

            cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255).Value = rolename;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            try
            {
                conn.Open();

                cmd.ExecuteBlind();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateRole");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }

        //
        // RoleProvider.DeleteRole
        //

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
            {
                throw new ProviderException("Role does not exist.");
            }

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
            {
                throw new ProviderException("Cannot delete a populated role.");
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "DELETE FROM " + rolesTable + "" + " WHERE role_name = @role_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255).Value = rolename;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlCommand cmd2 =
                new NpgsqlCommand(
                    "DELETE FROM " + usersInRolesTable + "" + " WHERE role_name = @role_name AND application_name = @application_name",
                    conn);

            cmd2.Parameters.Add("@role_name", NpgsqlDbType.Text, 255).Value = rolename;
            cmd2.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                cmd2.Transaction = tran;

                cmd2.ExecuteBlind();
                cmd.ExecuteBlind();

                tran.Commit();
            }
            catch (NpgsqlException e)
            {
                try
                {
                    if (tran != null)
                    {
                        tran.Rollback();
                    }
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteRole");

                    return false;
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return true;
        }

        //
        // RoleProvider.GetAllRoles
        //

        public override string[] GetAllRoles()
        {
            string tmpRoleNames = "";

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand("SELECT role_name FROM " + rolesTable + "" + " WHERE application_name = @application_name", conn);

            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tmpRoleNames += reader.GetString(0) + ",";
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllRoles");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Dispose();
                conn.Close();
            }

            if (tmpRoleNames.Length > 0)
            {
                // Remove trailing comma.
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }

            return new string[0];
        }

        //
        // RoleProvider.GetRolesForUser
        //

        public override string[] GetRolesForUser(string username)
        {
            string tmpRoleNames = "";

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT role_name FROM " + usersInRolesTable + "" +
                    " WHERE user_name = @user_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tmpRoleNames += reader.GetString(0) + ",";
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetRolesForUser");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Dispose();
                conn.Close();
            }

            if (tmpRoleNames.Length > 0)
            {
                // Remove trailing comma.
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }

            return new string[0];
        }

        //
        // RoleProvider.GetUsersInRole
        //

        public override string[] GetUsersInRole(string rolename)
        {
            string tmpUserNames = "";

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT user_name FROM " + usersInRolesTable + "" +
                    " WHERE role_name = @role_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255).Value = rolename;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tmpUserNames += reader.GetString(0) + ",";
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUsersInRole");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                cmd.Dispose();
                conn.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                // Remove trailing comma.
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

        //
        // RoleProvider.IsUserInRole
        //

        public override bool IsUserInRole(string username, string rolename)
        {
            bool userIsInRole = false;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT COUNT(*) FROM " + usersInRolesTable + "" +
                    " WHERE user_name = @user_name AND role_name = @role_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255).Value = rolename;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            try
            {
                conn.Open();

                long numRecs = Convert.ToInt64(cmd.ExecuteScalar());

                if (numRecs > 0)
                {
                    userIsInRole = true;
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "IsUserInRole");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return userIsInRole;
        }

        //
        // RoleProvider.RemoveUsersFromRoles
        //

        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    if (!IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is not in role.");
                    }
                }
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "DELETE FROM " + usersInRolesTable + "" +
                    " WHERE user_name = @user_name AND role_name = @role_name AND application_name = @application_name", conn);

            NpgsqlParameter userParm = cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255);
            NpgsqlParameter roleParm = cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255);
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            NpgsqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Transaction = tran;

                foreach (string username in usernames)
                {
                    foreach (string rolename in rolenames)
                    {
                        userParm.Value = username;
                        roleParm.Value = rolename;
                        cmd.ExecuteBlind();
                    }
                }

                tran.Commit();
            }
            catch (NpgsqlException e)
            {
                try
                {
                    if (tran != null)
                    {
                        tran.Rollback();
                    }
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RemoveUsersFromRoles");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }

        //
        // RoleProvider.RoleExists
        //

        public override bool RoleExists(string rolename)
        {
            bool exists = false;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT COUNT(*) FROM " + rolesTable + "" +
                    " WHERE role_name = @role_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@role_name", NpgsqlDbType.Text, 255).Value = rolename;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            try
            {
                conn.Open();

                long numRecs = Convert.ToInt64(cmd.ExecuteScalar());

                if (numRecs > 0)
                {
                    exists = true;
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RoleExists");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return exists;
        }

        //
        // RoleProvider.FindUsersInRole
        //

        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT user_name FROM " + usersInRolesTable + " " +
                    "WHERE user_name LIKE @UsernameSearch AND role_name = @role_name AND application_name = @application_name", conn);
            cmd.Parameters.Add("@UsernameSearch", NpgsqlDbType.Text, 255).Value = usernameToMatch;
            cmd.Parameters.Add("@RoleName", NpgsqlDbType.Text, 255).Value = rolename;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            string tmpUserNames = "";
            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tmpUserNames += reader.GetString(0) + ",";
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersInRole");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                cmd.Dispose();
                conn.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                // Remove trailing comma.
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also
        // thrown by the caller.
        //

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = exceptionMessage + "\n\n";
            message += "Action: " + action + "\n\n";
            if (e != null)
            {
                message += "Exception: " + e;
            }

            log.WriteEntry(message);
        }
    }
}
