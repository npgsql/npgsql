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
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Security;
using NpgsqlTypes;

/*

You will have to change NpgsqlMembershipProvider::encryptionKey to a random hexadecimal value of your choice

CREATE TABLE users
(
  userid char(36) NOT NULL,
  user_name varchar(255) NOT NULL,
  application_name varchar(100) NOT NULL,
  email varchar(100) NOT NULL,
  comment varchar(255),
  password varchar(128) NOT NULL,
  password_question varchar(255),
  password_answer varchar(255),
  is_approved bool,
  last_activity_date timestamp,
  last_login_date timestamp,
  last_password_changed_date timestamp,
  creation_date timestamp,
  is_online bool,
  is_locked_out bool,
  last_locked_out_date timestamp,
  failed_password_attempt_count integer,
  failed_password_attempt_window_start timestamp,
  failed_password_answer_attempt_count integer,
  failed_password_answer_attempt_window_start timestamp,
  PRIMARY KEY  (userid)
)

*/

namespace Npgsql.Web
{
    public sealed class NpgsqlMembershipProvider : MembershipProvider
    {
        //
        // Global connection string, generated password length, generic exception message, event log info.
        //

        private readonly int newPasswordLength = 8;
        private readonly string eventSource = "NpgsqlMembershipProvider";
        private readonly string eventLog = "Application";
        private readonly string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private readonly string tableName = "Users";
        private string connectionString;

        private const string encryptionKey = "AE09F72B007CAAB5";

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog;

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

            if (string.IsNullOrEmpty(name))
            {
                name = "NpgsqlMembershipProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Npgsql Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters =
                Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression =
                Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException("Password format not supported.");
            }

            //
            // Initialize NpgsqlConnection.
            //

            ConnectionStringSettings ConnectionStringSettings =
                ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (ConnectionStringSettings == null || string.IsNullOrEmpty(ConnectionStringSettings.ConnectionString.Trim()))
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;
        }

        //
        // A helper function to retrieve config values from the configuration file.
        //

        private static string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            return configValue;
        }

        //
        // System.Web.Security.MembershipProvider properties.
        //

        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }

        //
        // System.Web.Security.MembershipProvider methods.
        //

        //
        // MembershipProvider.ChangePassword
        //

        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!ValidateUser(username, oldPwd))
            {
                return false;
            }

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
                }
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("UPDATE {0} SET Password = @Password, last_password_changed_date = @last_password_changed_date  WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@Password", NpgsqlDbType.Text, 255).Value = EncodePassword(newPwd);
            cmd.Parameters.Add("@last_password_changed_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePassword");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
                }
                else
                {
                    throw;// e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return (rowsAffected > 0);        }

        //
        // MembershipProvider.ChangePasswordQuestionAndAnswer
        //

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPwdQuestion,
                                                             string newPwdAnswer)
        {
            if (!ValidateUser(username, password))
            {
                return false;
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("UPDATE {0} SET password_question = @Question, password_answer = @Answer WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@Question", NpgsqlDbType.Text, 255).Value = newPwdQuestion;
            cmd.Parameters.Add("@Answer", NpgsqlDbType.Text, 255).Value = EncodePassword(newPwdAnswer);
            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
                }
                else
                {
                    throw;// e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return (rowsAffected > 0);
        }

        //
        // MembershipProvider.CreateUser
        //

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion,
                                                  string passwordAnswer, bool isApproved, object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !string.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                NpgsqlCommand cmd =
                    new NpgsqlCommand(
                        string.Format("INSERT INTO {0} (UserId, user_name, Password, Email, password_question,  password_answer, is_approved, Comment, creation_date, last_password_changed_date, last_activity_date, application_name, is_locked_out, last_locked_out_date, failed_password_attempt_count, failed_password_attempt_window_start,  failed_password_answer_attempt_count, failed_password_answer_attempt_window_start) Values(@UserId, @user_name, @Password, @Email, @password_question,  @password_answer, @is_approved, @Comment, @creation_date, @last_password_changed_date,  @last_activity_date, @application_name, @is_locked_out, @last_locked_out_date,  @failed_password_attempt_count, @failed_password_attempt_window_start,  @failed_password_answer_attempt_count, @failed_password_answer_attempt_window_start)", tableName), conn);

                cmd.Parameters.Add("@UserId", NpgsqlDbType.Text).Value = providerUserKey.ToString();
                cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                cmd.Parameters.Add("@Password", NpgsqlDbType.Text, 255).Value = EncodePassword(password);
                cmd.Parameters.Add("@Email", NpgsqlDbType.Text, 128).Value = email;
                cmd.Parameters.Add("@password_question", NpgsqlDbType.Text, 255).Value = passwordQuestion;
                cmd.Parameters.Add("@password_answer", NpgsqlDbType.Text, 255).Value = passwordAnswer == null
                                                                                           ? null
                                                                                           : EncodePassword(passwordAnswer);
                cmd.Parameters.Add("@is_approved", NpgsqlDbType.Boolean).Value = isApproved;
                cmd.Parameters.Add("@Comment", NpgsqlDbType.Text, 255).Value = "";
                cmd.Parameters.Add("@creation_date", NpgsqlDbType.Timestamp).Value = createDate;
                cmd.Parameters.Add("@last_password_changed_date", NpgsqlDbType.Timestamp).Value = createDate;
                cmd.Parameters.Add("@last_activity_date", NpgsqlDbType.Timestamp).Value = createDate;
                cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;
                cmd.Parameters.Add("@is_locked_out", NpgsqlDbType.Boolean).Value = false; //false
                cmd.Parameters.Add("@last_locked_out_date", NpgsqlDbType.Timestamp).Value = createDate;
                cmd.Parameters.Add("@failed_password_attempt_count", NpgsqlDbType.Integer).Value = 0;
                cmd.Parameters.Add("@failed_password_attempt_window_start", NpgsqlDbType.Timestamp).Value = createDate;
                cmd.Parameters.Add("@failed_password_answer_attempt_count", NpgsqlDbType.Integer).Value = 0;
                cmd.Parameters.Add("@failed_password_answer_attempt_window_start", NpgsqlDbType.Timestamp).Value = createDate;

                try
                {
                    conn.Open();

                    int recAdded = cmd.ExecuteNonQuery();

                    if (recAdded > 0)
                    {
                        status = MembershipCreateStatus.Success;
                    }
                    else
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
                catch (NpgsqlException e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "CreateUser");
                    }

                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }

                return GetUser(username, false);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            return null;
        }

        //
        // MembershipProvider.DeleteUser
        //

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("DELETE FROM {0} WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();

                if (deleteAllRelatedData)
                {
                    // Process commands to delete all data for the user in the database.
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteUser");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
                }
                else
                {
                    throw;//e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return (rowsAffected > 0);        }

        //
        // MembershipProvider.GetAllUsers
        //

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(string.Format("SELECT Count(*) FROM {0} WHERE application_name = @application_name", tableName), conn);
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;
            MembershipUserCollection users = new MembershipUserCollection();

            NpgsqlDataReader reader = null;
            totalRecords = 0;
            try
            {
                conn.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalRecords <= 0)
                {
                    return users;
                }

                cmd.CommandText = string.Format("SELECT UserId, user_name, Email, password_question, Comment, is_approved, is_locked_out, creation_date, last_login_date, last_activity_date, last_password_changed_date, last_locked_out_date  FROM {0}  WHERE application_name = @application_name  ORDER BY user_name Asc", tableName);

                using (reader = cmd.ExecuteReader())
                {
                    int counter = 0;
                    int startIndex = pageSize*pageIndex;
                    int endIndex = startIndex + pageSize - 1;

                    while (reader.Read())
                    {
                        if (counter >= startIndex)
                        {
                            MembershipUser u = GetUserFromReader(reader);
                            users.Add(u);
                        }

                        if (counter >= endIndex)
                        {
                            cmd.Cancel();
                        }

                        counter++;
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllUsers");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
                }
                else
                {
                    throw;// e;
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

            return users;
        }

        //
        // MembershipProvider.GetNumberOfUsersOnline
        //

        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT Count(*) FROM {0} WHERE last_activity_date > @CompareDate AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@CompareDate", NpgsqlDbType.Timestamp).Value = compareTime;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            int numOnline = 0;

            try
            {
                conn.Open();

                numOnline = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetNumberOfUsersOnline");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
                }
                else
                {
                    throw;// e;
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }

            return numOnline;
        }

        //
        // MembershipProvider.GetPassword
        //

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("Cannot retrieve Hashed passwords.");
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT Password, password_answer, is_locked_out FROM {0} WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            string password = "";
            string passwordAnswer = "";
            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        if (reader.GetBoolean(2))
                        {
                            throw new MembershipPasswordException("The supplied user is locked out.");
                        }

                        password = reader.GetString(0);
                        passwordAnswer = reader.GetString(1);
                    }
                    else
                    {
                        throw new MembershipPasswordException("The supplied user name is not found.");
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }

            return password;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string GetUserNameById(string Id)
        {
            NpgsqlMembershipProvider _provider = null;
            ProviderCollection _providers = null;

            // Get a reference to the <imageService> section
            MembershipSection section = (MembershipSection) WebConfigurationManager.GetSection("system.web/membership");

            // Load registered providers and point _provider
            // to the default provider
            _providers = new ProviderCollection();
            ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof (NpgsqlMembershipProvider));
            _provider = (NpgsqlMembershipProvider) _providers[section.DefaultProvider];

            NpgsqlConnection conn = new NpgsqlConnection(_provider.connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT user_name FROM " + tableName + " WHERE userid = @user_id AND application_name = @application_name", conn);

            cmd.Parameters.Add("@user_id", NpgsqlDbType.Text, 50).Value = Id;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = _provider.ApplicationName;

            string UserName = "";
            try
            {
                conn.Open();
                UserName = cmd.ExecuteScalar().ToString();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameById(Guid Id)");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return UserName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            NpgsqlMembershipProvider _provider = null;
            ProviderCollection _providers = null;

            // Get a reference to the <imageService> section
            MembershipSection section = (MembershipSection) WebConfigurationManager.GetSection("system.web/membership");

            // Load registered providers and point _provider
            // to the default provider
            _providers = new ProviderCollection();
            ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof (NpgsqlMembershipProvider));
            _provider = (NpgsqlMembershipProvider) _providers[section.DefaultProvider];

            HttpContext currentContext = HttpContext.Current;

            NpgsqlConnection conn = new NpgsqlConnection(_provider.connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT UserId FROM " + tableName + " WHERE user_name = @user_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = currentContext.User.Identity.Name;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = _provider.ApplicationName;

            string UserId = "";
            try
            {
                conn.Open();
                UserId = cmd.ExecuteScalar().ToString();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserId()");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return UserId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public MembershipUser GetCustomUser(string username)
        {
            NpgsqlMembershipProvider _provider = null;
            ProviderCollection _providers = null;

            // Get a reference to the <imageService> section
            MembershipSection section = (MembershipSection) WebConfigurationManager.GetSection("system.web/membership");

            // Load registered providers and point _provider
            // to the default provider
            _providers = new ProviderCollection();
            ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof (NpgsqlMembershipProvider));
            _provider = (NpgsqlMembershipProvider) _providers[section.DefaultProvider];

            NpgsqlConnection conn = new NpgsqlConnection(_provider.connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT UserId, user_name, Email, password_question," +
                    " Comment, is_approved, is_locked_out, creation_date, last_login_date," +
                    " last_activity_date, last_password_changed_date, last_locked_out_date" + " FROM " + tableName +
                    " WHERE user_name = @user_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = _provider.ApplicationName;

            MembershipUser u = null;
            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        u = GetUserFromReader(reader);
                        reader.Close();
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return u;
        }

        /// <summary>
        /// MembershipProvider.GetUser(string, bool)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT UserId, user_name, Email, password_question, Comment, is_approved, is_locked_out, creation_date, last_login_date, last_activity_date, last_password_changed_date, last_locked_out_date FROM {0} WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            MembershipUser u = null;
            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        u = GetUserFromReader(reader);
                        reader.Close();

                        if (userIsOnline)
                        {
                            NpgsqlCommand updateCmd =
                                new NpgsqlCommand(
                                    string.Format("UPDATE {0} SET last_activity_date = @last_activity_date WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

                            updateCmd.Parameters.Add("@last_activity_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                                // fixed by Alex .ToString("yyyy/MM/dd HH:mm:ss");
                            updateCmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                            updateCmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

                            updateCmd.ExecuteBlind();
                        }
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return u;
        }

        //
        // MembershipProvider.GetUser(object, bool)
        //

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT UserId, user_name, Email, password_question, Comment, is_approved, is_locked_out, creation_date, last_login_date, last_activity_date, last_password_changed_date, last_locked_out_date FROM {0} WHERE UserId = @UserId", tableName), conn);

            cmd.Parameters.Add("@UserId", NpgsqlDbType.Text).Value = providerUserKey;

            MembershipUser u = null;
            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        u = GetUserFromReader(reader);

                        reader.Close();

                        if (userIsOnline)
                        {
                            NpgsqlCommand updateCmd =
                                new NpgsqlCommand(
                                    string.Format("UPDATE {0} SET last_activity_date = @last_activity_date WHERE UserId = @UserId", tableName), conn);

                            updateCmd.Parameters.Add("@last_activity_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                                // fixed by Alex .ToString("yyyy/MM/dd HH:mm:ss");
                            updateCmd.Parameters.Add("@UserId", NpgsqlDbType.Text).Value = providerUserKey;

                            updateCmd.ExecuteBlind();
                        }
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return u;
        }

        //
        // GetUserFromReader
        //    A helper function that takes the current row from the NpgsqlDataReader
        // and hydrates a MembershiUser from the values. Called by the
        // MembershipUser.GetUser implementation.
        //

        private MembershipUser GetUserFromReader(NpgsqlDataReader reader)
        {
            object providerUserKey = new Guid(reader.GetValue(0).ToString());
            string username = reader.IsDBNull(1) ? "" : reader.GetString(1);
            string email = reader.IsDBNull(2) ? "" : reader.GetString(2);
            string passwordQuestion = reader.IsDBNull(3) ? "" : reader.GetString(3);
            string comment = reader.IsDBNull(4) ? "" : reader.GetString(4);
            bool isApproved = reader.IsDBNull(5) ? false : reader.GetBoolean(5);
            bool isLockedOut = reader.IsDBNull(6) ? false : reader.GetBoolean(6);
            DateTime creationDate = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7);
            DateTime lastLoginDate = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8);
            DateTime lastActivityDate = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9);
            DateTime lastPasswordChangedDate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10);
            DateTime lastLockedOutDate = reader.IsDBNull(11) ? DateTime.Now : reader.GetDateTime(11);

            MembershipUser u =
                new MembershipUser(this.Name, username, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut,
                                   creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockedOutDate);

            return u;
        }

        //
        // MembershipProvider.UnlockUser
        //

        public override bool UnlockUser(string username)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "UPDATE " + tableName + " " + " SET is_locked_out = false, last_locked_out_date = @last_locked_out_date " +
                    " WHERE user_name = @user_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@last_locked_out_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UnlockUser");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        //
        // MembershipProvider.GetUserNameByEmail
        //

        public override string GetUserNameByEmail(string email)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT user_name" + " FROM " + tableName + " WHERE Email = @Email AND application_name = @application_name", conn);

            cmd.Parameters.Add("@Email", NpgsqlDbType.Text, 128).Value = email;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            string username = "";

            try
            {
                conn.Open();

                username = (string) cmd.ExecuteScalar();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            if (username == null)
            {
                username = "";
            }

            return username;
        }

        //
        // MembershipProvider.ResetPassword
        //

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");

                // use fully qualified name so as not to conflict with System.Data.ProviderException
                // in System.Data.Entity assembly
                throw new System.Configuration.Provider.ProviderException("Password answer required for password reset.");
            }

            string newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
                }
            }

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT password_answer, is_locked_out FROM " + tableName + "" +
                    " WHERE user_name = @user_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            int rowsAffected = 0;
            string passwordAnswer = "";
            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        if (reader.GetBoolean(1))
                        {
                            throw new MembershipPasswordException("The supplied user is locked out.");
                        }

                        passwordAnswer = reader.GetString(0);
                    }
                    else
                    {
                        throw new MembershipPasswordException("The supplied user name is not found.");
                    }
                    reader.Close();
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");

                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                NpgsqlCommand updateCmd =
                    new NpgsqlCommand(
                        "UPDATE " + tableName + "" + " SET Password = @Password, last_password_changed_date = @last_password_changed_date" +
                        " WHERE user_name = @user_name AND application_name = @application_name AND is_locked_out = false", conn);

                updateCmd.Parameters.Add("@Password", NpgsqlDbType.Text, 255).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("@last_password_changed_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                updateCmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                updateCmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            if (rowsAffected > 0)
            {
                return newPassword;
            }
            else
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }

        //
        // MembershipProvider.UpdateUser
        //

        public override void UpdateUser(MembershipUser user)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "UPDATE " + tableName + "" + " SET Email = @Email, Comment = @Comment," + " is_approved = @is_approved" +
                    " WHERE user_name = @user_name AND application_name = @application_name", conn);

            cmd.Parameters.Add("@Email", NpgsqlDbType.Text, 128).Value = user.Email;
            cmd.Parameters.Add("@Comment", NpgsqlDbType.Text, 255).Value = user.Comment;
            cmd.Parameters.Add("@is_approved", NpgsqlDbType.Boolean).Value = user.IsApproved;
            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = user.UserName;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            try
            {
                conn.Open();

                cmd.ExecuteBlind();
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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
        // MembershipProvider.ValidateUser
        //

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    "SELECT Password, is_approved FROM " + tableName + "" +
                    " WHERE user_name = @user_name AND application_name = @application_name AND is_locked_out = false", conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            NpgsqlDataReader reader = null;
            bool isApproved = false;
            string pwd = "";

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        pwd = reader.GetString(0);
                        isApproved = reader.GetBoolean(1);
                    }
                    else
                    {
                        return false;
                    }
                    reader.Close();
                }

                if (CheckPassword(password, pwd))
                {
                    if (isApproved)
                    {
                        isValid = true;

                        NpgsqlCommand updateCmd =
                            new NpgsqlCommand(
                                "UPDATE " + tableName + " SET last_login_date = @last_login_date, last_activity_date = @last_activity_date" +
                                " WHERE user_name = @user_name AND application_name = @application_name", conn);

                        updateCmd.Parameters.Add("@last_login_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@last_activity_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                            // fixed by Alex .ToString("yyyy/MM/dd HH:mm:ss");
                        updateCmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                        updateCmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

                        updateCmd.ExecuteBlind();
                    }
                }
                else
                {
                    cmd.Dispose();
                    conn.Close();

                    UpdateFailureCount(username, "password");
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return isValid;
        }

        //
        // UpdateFailureCount
        //   A helper method that performs the checks and updates associated with
        // password failure tracking.
        //

        private void UpdateFailureCount(string username, string failureType)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT failed_password_attempt_count,   failed_password_attempt_window_start,   failed_password_answer_attempt_count,   failed_password_answer_attempt_window_start   FROM {0}   WHERE user_name = @user_name AND application_name = @application_name", tableName), conn);

            cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            NpgsqlDataReader reader = null;
            DateTime windowStart = new DateTime();
            int failureCount = 0;

            try
            {
                conn.Open();

                using (reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        if (failureType == "password")
                        {
                            failureCount = reader.GetInt32(0);
                            windowStart = reader.GetDateTime(1);
                        }

                        if (failureType == "passwordAnswer")
                        {
                            failureCount = reader.GetInt32(2);
                            windowStart = reader.GetDateTime(3);
                        }
                    }
                    reader.Close();
                }

                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow.
                    // Start a new password failure count from 1 and a new window starting now.

                    if (failureType == "password")
                    {
                        cmd.CommandText = string.Format("UPDATE {0}   SET failed_password_attempt_count = @Count,       failed_password_attempt_window_start = @WindowStart   WHERE user_name = @user_name AND application_name = @application_name", tableName);
                    }

                    if (failureType == "passwordAnswer")
                    {
                        cmd.CommandText = string.Format("UPDATE {0}   SET failed_password_answer_attempt_count = @Count,       failed_password_answer_attempt_window_start = @WindowStart   WHERE user_name = @user_name AND application_name = @application_name", tableName);
                    }

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("@Count", NpgsqlDbType.Integer).Value = 1;
                    cmd.Parameters.Add("@WindowStart", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                    cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                    cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

                    if (cmd.ExecuteNonQuery() < 0)
                    {
                        // use fully qualified name so as not to conflict with System.Data.ProviderException
                        // in System.Data.Entity assembly
                        throw new System.Configuration.Provider.ProviderException("Unable to update failure count and window start.");
                    }
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.

                        cmd.CommandText = string.Format("UPDATE {0}   SET is_locked_out = @is_locked_out, last_locked_out_date = @last_locked_out_date   WHERE user_name = @user_name AND application_name = @application_name", tableName);

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@is_locked_out", NpgsqlDbType.Boolean).Value = true;
                        cmd.Parameters.Add("@last_locked_out_date", NpgsqlDbType.Timestamp).Value = DateTime.Now;
                        cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                        cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                        {
                            // use fully qualified name so as not to conflict with System.Data.ProviderException
                            // in System.Data.Entity assembly
                            throw new System.Configuration.Provider.ProviderException("Unable to lock out user.");
                        }
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.

                        if (failureType == "password")
                        {
                            cmd.CommandText = string.Format("UPDATE {0}   SET failed_password_attempt_count = @Count  WHERE user_name = @user_name AND application_name = @application_name", tableName);
                        }

                        if (failureType == "passwordAnswer")
                        {
                            cmd.CommandText = string.Format("UPDATE {0}   SET failed_password_answer_attempt_count = @Count  WHERE user_name = @user_name AND application_name = @application_name", tableName);
                        }

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@Count", NpgsqlDbType.Integer).Value = failureCount;
                        cmd.Parameters.Add("@user_name", NpgsqlDbType.Text, 255).Value = username;
                        cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                        {
                            // use fully qualified name so as not to conflict with System.Data.ProviderException
                            // in System.Data.Entity assembly
                            throw new System.Configuration.Provider.ProviderException("Unable to update failure count.");
                        }
                    }
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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
        }

        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //

        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }

        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //

        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte(encryptionKey);
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException("Cannot unencode a hashed password.");
                default:
                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException("Unsupported password format.");
            }

            return password;
        }

        //
        // HexToByte
        //   Converts a hexadecimal string to a byte array. Used to convert encryption
        // key values from the configuration.
        //

        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length/2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
            }
            return returnBytes;
        }

        //
        // MembershipProvider.FindUsersByName
        //

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT Count(*) FROM {0} WHERE user_name LIKE @UsernameSearch AND application_name = @application_name", tableName), conn);
            cmd.Parameters.Add("@UsernameSearch", NpgsqlDbType.Text, 255).Value = usernameToMatch;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = pApplicationName;

            MembershipUserCollection users = new MembershipUserCollection();

            NpgsqlDataReader reader = null;

            try
            {
                conn.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

                if (totalRecords <= 0)
                {
                    return users;
                }

                cmd.CommandText = string.Format("SELECT UserId, user_name, Email, password_question, Comment, is_approved, is_locked_out, creation_date, last_login_date, last_activity_date, last_password_changed_date, last_locked_out_date  FROM {0}  WHERE user_name LIKE @UsernameSearch AND application_name = @application_name  ORDER BY user_name Asc", tableName);

                using (reader = cmd.ExecuteReader())
                {
                    int counter = 0;
                    int startIndex = pageSize*pageIndex;
                    int endIndex = startIndex + pageSize - 1;

                    while (reader.Read())
                    {
                        if (counter >= startIndex)
                        {
                            MembershipUser u = GetUserFromReader(reader);
                            users.Add(u);
                        }

                        if (counter >= endIndex)
                        {
                            cmd.Cancel();
                        }

                        counter++;
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return users;
        }

        //
        // MembershipProvider.FindUsersByEmail
        //

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            NpgsqlCommand cmd =
                new NpgsqlCommand(
                    string.Format("SELECT Count(*) FROM {0} WHERE Email LIKE @EmailSearch AND application_name = @application_name", tableName), conn);
            cmd.Parameters.Add("@EmailSearch", NpgsqlDbType.Text, 255).Value = emailToMatch;
            cmd.Parameters.Add("@application_name", NpgsqlDbType.Text, 255).Value = ApplicationName;

            MembershipUserCollection users = new MembershipUserCollection();

            NpgsqlDataReader reader = null;
            totalRecords = 0;

            try
            {
                conn.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

                if (totalRecords <= 0)
                {
                    return users;
                }

                cmd.CommandText = string.Format("SELECT UserId, user_name, Email, password_question, Comment, is_approved, is_locked_out, creation_date, last_login_date, last_activity_date, last_password_changed_date, last_locked_out_date  FROM {0}  WHERE Email LIKE @user_name AND application_name = @application_name  ORDER BY user_name Asc", tableName);

                using (reader = cmd.ExecuteReader())
                {
                    int counter = 0;
                    int startIndex = pageSize*pageIndex;
                    int endIndex = startIndex + pageSize - 1;

                    while (reader.Read())
                    {
                        if (counter >= startIndex)
                        {
                            MembershipUser u = GetUserFromReader(reader);
                            users.Add(u);
                        }

                        if (counter >= endIndex)
                        {
                            cmd.Cancel();
                        }

                        counter++;
                    }
                    reader.Close();
                }
            }
            catch (NpgsqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByEmail");

                    // use fully qualified name so as not to conflict with System.Data.ProviderException
                    // in System.Data.Entity assembly
                    throw new System.Configuration.Provider.ProviderException(exceptionMessage);
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

            return users;
        }

        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also
        // thrown by the caller.
        //

        private void WriteToEventLog(NpgsqlException e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = string.Format("An exception occurred communicating with the data source.\n\nAction: {0}\n\nException: {1}", action, e);

            log.WriteEntry(message);
        }
    }
}
