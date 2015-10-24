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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
#if NET45 || NET452 || DNX452
using System.DirectoryServices;
using System.Security.Principal;
#endif

namespace Npgsql
{
    /// <summary>
    /// Provides a simple way to create and manage the contents of connection strings used by
    /// the <see cref="NpgsqlConnection"/> class.
    /// </summary>
    public sealed class NpgsqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Fields

        /// <summary>
        /// Makes all valid keywords for a property to that property (e.g. User Name -> Username, UserId -> Username...)
        /// </summary>
        static readonly Dictionary<string, PropertyInfo> PropertiesByKeyword;

        /// <summary>
        /// Maps CLR property names (e.g. BufferSize) to their canonical keyword name, which is the
        /// property's [DisplayName] (e.g. Buffer Size)
        /// </summary>
        static readonly Dictionary<string, string> PropertyNameToCanonicalKeyword;

        /// <summary>
        /// Maps each property to its [DefaultValue]
        /// </summary>
        static readonly Dictionary<PropertyInfo, object> PropertyDefaults;

        static readonly string[] Empty = new string[0];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the NpgsqlConnectionStringBuilder class.
        /// </summary>
        public NpgsqlConnectionStringBuilder() { Init(); }

#if NET45 || NET452 || DNX452
        /// <summary>
        /// Initializes a new instance of the NpgsqlConnectionStringBuilder class, optionally using ODBC rules for quoting values.
        /// </summary>
        /// <param name="useOdbcRules">true to use {} to delimit fields; false to use quotation marks.</param>
        public NpgsqlConnectionStringBuilder(bool useOdbcRules) : base(useOdbcRules) { Init(); }
#endif

        /// <summary>
        /// Initializes a new instance of the NpgsqlConnectionStringBuilder class and sets its <see cref="DbConnectionStringBuilder.ConnectionString"/>.
        /// </summary>
        public NpgsqlConnectionStringBuilder(string connectionString)
        {
            Init();
            ConnectionString = connectionString;
        }

        void Init()
        {
            foreach (var kv in PropertyDefaults) {
                kv.Key.SetValue(this, kv.Value);
                base.Clear();
            }
        }

        #endregion

        #region Static initialization

        static NpgsqlConnectionStringBuilder()
        {
            var properties = typeof(NpgsqlConnectionStringBuilder)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<NpgsqlConnectionStringPropertyAttribute>() != null)
                .ToArray();

            Contract.Assume(properties.All(p => p.CanRead && p.CanWrite));
            Contract.Assume(properties.All(p => p.GetCustomAttribute<DisplayNameAttribute>() != null));

            PropertiesByKeyword = (
                from p in properties
                let displayName = p.GetCustomAttribute<DisplayNameAttribute>().DisplayName.ToUpperInvariant()
                let propertyName = p.Name.ToUpperInvariant()
                from k in new[] { displayName }
                  .Concat(propertyName != displayName ? new[] { propertyName } : Empty )
                  .Concat(p.GetCustomAttribute<NpgsqlConnectionStringPropertyAttribute>().Aliases
                    .Select(a => a.ToUpperInvariant())
                  )
                  .Select(k => new { Property = p, Keyword = k })
                select k
            ).ToDictionary(t => t.Keyword, t => t.Property);

            PropertyNameToCanonicalKeyword = properties.ToDictionary(
                p => p.Name,
                p => p.GetCustomAttribute<DisplayNameAttribute>().DisplayName
            );

            PropertyDefaults = properties
                .Where(p => p.GetCustomAttribute<ObsoleteAttribute>() == null)
                .ToDictionary(
                p => p,
                p => p.GetCustomAttribute<DefaultValueAttribute>() != null
                    ? p.GetCustomAttribute<DefaultValueAttribute>().Value
                    : (p.PropertyType.GetTypeInfo().IsValueType ? Activator.CreateInstance(p.PropertyType) : null)
            );
        }

        #endregion

        #region Non-static property handling

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="keyword">The key of the item to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public override object this[string keyword]
        {
            get
            {
                object value;
                if (!TryGetValue(keyword, out value)) {
                    throw new ArgumentException("Keyword not supported: " + keyword, nameof(keyword));
                }
                return value;
            }
            set
            {
                if (value == null) {
                    Remove(keyword);
                    return;
                }

                var p = GetProperty(keyword);
                try {
                    object convertedValue;
                    if (p.PropertyType.GetTypeInfo().IsEnum && value is string) {
                        convertedValue = Enum.Parse(p.PropertyType, (string)value);
                    } else {
                        convertedValue = Convert.ChangeType(value, p.PropertyType);
                    }
                    p.SetValue(this, convertedValue);
                } catch (Exception e) {
                    throw new ArgumentException("Couldn't set " + keyword, keyword, e);
                }
            }
        }

        /// <summary>
        /// Removes the entry with the specified key from the DbConnectionStringBuilder instance.
        /// </summary>
        /// <param name="keyword">The key of the key/value pair to be removed from the connection string in this DbConnectionStringBuilder.</param>
        /// <returns><b>true</b> if the key existed within the connection string and was removed; <b>false</b> if the key did not exist.</returns>
        public override bool Remove(string keyword)
        {
            var p = GetProperty(keyword);
            var removed = base.ContainsKey(p.Name);
            // Note that string property setters call SetValue, which itself calls base.Remove().
            p.SetValue(this, PropertyDefaults[p]);
            base.Remove(p.Name);
            return removed;
        }

        /// <summary>
        /// Clears the contents of the <see cref="NpgsqlConnectionStringBuilder"/> instance.
        /// </summary>
        public override void Clear()
        {
            Contract.Assert(Keys != null);
            foreach (var k in Keys.Cast<string>().ToArray()) {
                Remove(k);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="NpgsqlConnectionStringBuilder"/> contains a specific key.
        /// </summary>
        /// <param name="keyword">The key to locate in the <see cref="NpgsqlConnectionStringBuilder"/>.</param>
        /// <returns><b>true</b> if the <see cref="NpgsqlConnectionStringBuilder"/> contains an entry with the specified key; otherwise <b>false</b>.</returns>
        public override bool ContainsKey(string keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));
            Contract.EndContractBlock();

            return PropertiesByKeyword.ContainsKey(keyword.ToUpperInvariant());
        }

        PropertyInfo GetProperty(string keyword)
        {
            PropertyInfo p;
            if (!PropertiesByKeyword.TryGetValue(keyword.ToUpperInvariant(), out p)) {
                throw new ArgumentException("Keyword not supported: " + keyword, nameof(keyword));
            }
            return p;
        }

        /// <summary>
        /// Retrieves a value corresponding to the supplied key from this <see cref="NpgsqlConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="keyword">The key of the item to retrieve.</param>
        /// <param name="value">The value corresponding to the key.</param>
        /// <returns><b>true</b> if keyword was found within the connection string, <b>false</b> otherwise.</returns>
        public override bool TryGetValue(string keyword, [CanBeNull] out object value)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));
            Contract.EndContractBlock();

            PropertyInfo p;
            if (!PropertiesByKeyword.TryGetValue(keyword.ToUpperInvariant(), out p))
            {
                value = null;
                return false;
            }

            value = GetProperty(keyword).GetValue(this) ?? "";
            return true;

        }

        void SetValue(string propertyName, [CanBeNull] object value)
        {
            var canonicalKeyword = PropertyNameToCanonicalKeyword[propertyName];
            if (value == null) {
                base.Remove(canonicalKeyword);
            } else {
                base[canonicalKeyword] = value;
            }
        }

        #endregion

        #region Properties - Connection

        /// <summary>
        /// The hostname or IP address of the PostgreSQL server to connect to.
        /// </summary>
        [Category("Connection")]
        [Description("The hostname or IP address of the PostgreSQL server to connect to.")]
        [DisplayName("Host")]
        [NpgsqlConnectionStringProperty("Server")]
        public string Host
        {
            get { return _host; }
            set
            {
                _host = value;
                SetValue(nameof(Host), value);
            }
        }
        string _host;

        /// <summary>
        /// The TCP/IP port of the PostgreSQL server.
        /// </summary>
        [Category("Connection")]
        [Description("The TCP port of the PostgreSQL server.")]
        [DisplayName("Port")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(NpgsqlConnection.DefaultPort)]
        public int Port
        {
            get { return _port; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid port: " + value);
                Contract.EndContractBlock();

                _port = value;
                SetValue(nameof(Port), value);
            }
        }
        int _port;

        ///<summary>
        /// The PostgreSQL database to connect to.
        /// </summary>
        [Category("Connection")]
        [Description("The PostgreSQL database to connect to.")]
        [DisplayName("Database")]
        [NpgsqlConnectionStringProperty("DB")]
        public string Database
        {
            get { return _database; }
            set
            {
                _database = value;
                SetValue(nameof(Database), value);
            }
        }
        string _database;

        /// <summary>
        /// The username to connect with. Not required if using IntegratedSecurity.
        /// </summary>
        [Category("Connection")]
        [Description("The username to connect with. Not required if using IntegratedSecurity.")]
        [DisplayName("Username")]
        [NpgsqlConnectionStringProperty("User Name", "UserId", "User Id", "UID")]
        public string Username
        {
            get
            {
#if NET45 || NET452 || DNX452
                if ((_integratedSecurity) && (String.IsNullOrEmpty(_username))) {
                    _username = GetIntegratedUserName();
                }
#endif

                return _username;
            }

            set
            {
                _username = value;
                SetValue(nameof(Username), value);
            }
        }
        string _username;

        /// <summary>
        /// The password to connect with. Not required if using IntegratedSecurity.
        /// </summary>
        [Category("Connection")]
        [Description("The password to connect with. Not required if using IntegratedSecurity.")]
        [PasswordPropertyText(true)]
        [DisplayName("Password")]
        [NpgsqlConnectionStringProperty("PSW", "PWD")]
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                SetValue(nameof(Password), value);
            }
        }
        string _password;

        /// <summary>
        /// The optional application name parameter to be sent to the backend during connection initiation.
        /// </summary>
        [Category("Connection")]
        [Description("The optional application name parameter to be sent to the backend during connection initiation")]
        [DisplayName("Application Name")]
        [NpgsqlConnectionStringProperty]
        public string ApplicationName
        {
            get { return _applicationName; }
            set
            {
                _applicationName = value;
                SetValue(nameof(ApplicationName), value);
            }
        }
        string _applicationName;

        /// <summary>
        /// Whether to enlist in an ambient TransactionScope.
        /// </summary>
        [Category("Connection")]
        [Description("Whether to enlist in an ambient TransactionScope.")]
        [DisplayName("Enlist")]
        [NpgsqlConnectionStringProperty]
        public bool Enlist
        {
            get { return _enlist; }
            set
            {
                _enlist = value;
                SetValue(nameof(Enlist), value);
            }
        }
        bool _enlist;

        /// <summary>
        /// Gets or sets the schema search path.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets the schema search path.")]
        [DisplayName("Search Path")]
        [NpgsqlConnectionStringProperty]
        public string SearchPath
        {
            get { return _searchpath; }
            set
            {
                _searchpath = value;
                SetValue(nameof(SearchPath), value);
            }
        }
        string _searchpath;

        #endregion

        #region Properties - Security

        /// <summary>
        /// Controls whether SSL is required, disabled or preferred, depending on server support.
        /// </summary>
        [Category("Security")]
        [Description("Controls whether SSL is required, disabled or preferred, depending on server support.")]
        [DisplayName("SSL Mode")]
        [NpgsqlConnectionStringProperty]
        public SslMode SslMode
        {
            get { return _sslmode; }
            set
            {
                _sslmode = value;
                SetValue(nameof(SslMode), value);
            }
        }
        SslMode _sslmode;

        /// <summary>
        /// Whether to trust the server certificate without validating it.
        /// </summary>
        [Category("Security")]
        [Description("Whether to trust the server certificate without validating it.")]
        [DisplayName("Trust Server Certificate")]
        [NpgsqlConnectionStringProperty]
        public bool TrustServerCertificate
        {
            get { return _trustServerCertificate; }
            set
            {
                _trustServerCertificate = value;
                SetValue(nameof(TrustServerCertificate), value);
            }
        }
        bool _trustServerCertificate;

        /// <summary>
        /// Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead.
        /// </summary>
        [Category("Security")]
        [Description("Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead.")]
        [DisplayName("Use SSL Stream")]
        [NpgsqlConnectionStringProperty]
        public bool UseSslStream
        {
            get { return _useSslStream; }
            set
            {
                _useSslStream = value;
                SetValue(nameof(UseSslStream), value);
            }
        }
        bool _useSslStream;

        /// <summary>
        /// Whether to use Windows integrated security to log in.
        /// </summary>
        [Category("Security")]
        [Description("Whether to use Windows integrated security to log in.")]
        [DisplayName("Integrated Security")]
        [NpgsqlConnectionStringProperty]
        public bool IntegratedSecurity
        {
            get { return _integratedSecurity; }
            set
            {
#if !NET40
                if (value)
                    CheckIntegratedSecuritySupport();
#endif
                _integratedSecurity = value;
                SetValue(nameof(IntegratedSecurity), value);
            }
        }
        bool _integratedSecurity;

        /// <summary>
        /// The Kerberos service name to be used for authentication.
        /// </summary>
        [Category("Security")]
        [Description("The Kerberos service name to be used for authentication.")]
        [DisplayName("Kerberos Service Name")]
        [NpgsqlConnectionStringProperty("Krbsrvname")]
        public string KerberosServiceName
        {
            get { return _kerberosServiceName; }
            set
            {
                _kerberosServiceName = value;
                SetValue(nameof(KerberosServiceName), value);
            }
        }
        string _kerberosServiceName;

        /// <summary>
        /// The Kerberos realm to be used for authentication.
        /// </summary>
        [Category("Security")]
        [Description("The Kerberos realm to be used for authentication.")]
        [DisplayName("Include Realm")]
        [NpgsqlConnectionStringProperty]
        public bool IncludeRealm
        {
            get { return _includeRealm; }
            set
            {
                _includeRealm = value;
                SetValue(nameof(IncludeRealm), value);
            }
        }
        bool _includeRealm;

        /// <summary>
        /// Gets or sets a Boolean value that indicates if security-sensitive information, such as the password, is not returned as part of the connection if the connection is open or has ever been in an open state.
        /// </summary>
        [Category("Security")]
        [Description("Gets or sets a Boolean value that indicates if security-sensitive information, such as the password, is not returned as part of the connection if the connection is open or has ever been in an open state.")]
        [DisplayName("Persist Security Info")]
        [NpgsqlConnectionStringProperty]
        public bool PersistSecurityInfo
        {
            get { return _persistSecurityInfo; }
            set
            {
                _persistSecurityInfo = value;
                SetValue(nameof(PersistSecurityInfo), value);
            }
        }
        bool _persistSecurityInfo;

        #endregion

        #region Properties - Pooling

        /// <summary>
        /// Whether connection pooling should be used.
        /// </summary>
        [Category("Pooling")]
        [Description("Whether connection pooling should be used.")]
        [DisplayName("Pooling")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(true)]
        public bool Pooling
        {
            get { return _pooling; }
            set
            {
                _pooling = value;
                SetValue(nameof(Pooling), value);
            }
        }
        bool _pooling;

        /// <summary>
        /// The minimum connection pool size.
        /// </summary>
        [Category("Pooling")]
        [Description("The minimum connection pool size.")]
        [DisplayName("Minimum Pool Size")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(1)]
        public int MinPoolSize
        {
            get { return _minPoolSize; }
            set
            {
                if (value < 0 || value > NpgsqlConnectorPool.PoolSizeLimit)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MinPoolSize must be between 0 and " + NpgsqlConnectorPool.PoolSizeLimit);
                Contract.EndContractBlock();

                _minPoolSize = value;
                SetValue(nameof(MinPoolSize), value);
            }
        }
        int _minPoolSize;

        /// <summary>
        /// The maximum connection pool size.
        /// </summary>
        [Category("Pooling")]
        [Description("The maximum connection pool size.")]
        [DisplayName("Maximum Pool Size")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(20)]
        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set
            {
                if (value < 0 || value > NpgsqlConnectorPool.PoolSizeLimit)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxPoolSize must be between 0 and " + NpgsqlConnectorPool.PoolSizeLimit);
                Contract.EndContractBlock();

                _maxPoolSize = value;
                SetValue(nameof(MaxPoolSize), value);
            }
        }
        int _maxPoolSize;

        /// <summary>
        /// The time to wait before closing unused connections in the pool if the count
        /// of all connections exeeds MinPoolSize.
        /// </summary>
        /// <remarks>
        /// If connection pool contains unused connections for ConnectionLifeTime seconds,
        /// the half of them will be closed. If there will be unused connections in a second
        /// later then again the half of them will be closed and so on.
        /// This strategy provide smooth change of connection count in the pool.
        /// </remarks>
        /// <value>The time (in seconds) to wait. The default value is 15 seconds.</value>
        [Category("Pooling")]
        [Description("The time to wait before closing unused connections in the pool if the count of all connections exeeds MinPoolSize.")]
        [DisplayName("Connection Lifetime")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(15)]
        public int ConnectionLifeTime
        {
            get { return _connectionLifeTime; }
            set
            {
                _connectionLifeTime = value;
                SetValue(nameof(ConnectionLifeTime), value);
            }
        }
        int _connectionLifeTime;

        #endregion

        #region Properties - Timeouts

        /// <summary>
        /// The time to wait (in seconds) while trying to establish a connection before terminating the attempt and generating an error.
        /// Defaults to 15 seconds.
        /// </summary>
        [Category("Timeouts")]
        [Description("The time to wait (in seconds) while trying to establish a connection before terminating the attempt and generating an error.")]
        [DisplayName("Timeout")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(15)]
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (value < 0 || value > NpgsqlConnection.TimeoutLimit)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Timeout must be between 0 and " + NpgsqlConnection.TimeoutLimit);
                Contract.EndContractBlock();

                _timeout = value;
                SetValue(nameof(Timeout), value);
            }
        }
        int _timeout;

        /// <summary>
        /// The time to wait (in seconds) while trying to execute a command before terminating the attempt and generating an error.
        /// Defaults to 30 seconds.
        /// </summary>
        [Category("Timeouts")]
        [Description("The time to wait (in seconds) while trying to execute a command before terminating the attempt and generating an error. Set to zero for infinity.")]
        [DisplayName("Command Timeout")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(NpgsqlCommand.DefaultTimeout)]
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "CommandTimeout can't be negative");
                Contract.EndContractBlock();

                _commandTimeout = value;
                SetValue(nameof(CommandTimeout), value);
            }
        }
        int _commandTimeout;

        /// <summary>
        /// The time to wait (in seconds) while trying to execute a an internal command before terminating the attempt and generating an error.
        /// </summary>
        [Category("Timeouts")]
        [Description("The time to wait (in seconds) while trying to execute a an internal command before terminating the attempt and generating an error. -1 uses CommandTimeout, 0 means no timeout.")]
        [DisplayName("Internal Command Timeout")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(-1)]
        public int InternalCommandTimeout
        {
            get { return _internalCommandTimeout; }
            set
            {
                if (value != 0 && value != -1 && value < NpgsqlConnector.MinimumInternalCommandTimeout)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"InternalCommandTimeout must be >= {NpgsqlConnector.MinimumInternalCommandTimeout}, 0 (infinite) or -1 (use CommandTimeout)");
                Contract.EndContractBlock();

                _internalCommandTimeout = value;
                SetValue(nameof(InternalCommandTimeout), value);
            }
        }
        int _internalCommandTimeout;

        /// <summary>
        /// Whether to have the backend enforce <see cref="CommandTimeout"/> and <see cref="InternalCommandTimeout"/>
        /// via the statement_timeout variable. Defaults to true.
        /// </summary>
        [Category("Timeouts")]
        [Description("Whether to have the backend enforce CommandTimeout and InternalCommandTimeout via the statement_timeout variable.")]
        [DisplayName("Backend Timeouts")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(true)]
        public bool BackendTimeouts
        {
            get { return _backendTimeouts; }
            set
            {
                _backendTimeouts = value;
                SetValue(nameof(BackendTimeouts), value);
            }
        }
        bool _backendTimeouts;

        #endregion

        #region Properties - Entity Framework

        /// <summary>
        /// The database template to specify when creating a database in Entity Framework. If not specified,
        /// PostgreSQL defaults to "template1".
        /// </summary>
        /// <remarks>
        /// http://www.postgresql.org/docs/current/static/manage-ag-templatedbs.html
        /// </remarks>
        [Category("Entity Framework")]
        [Description("The database template to specify when creating a database in Entity Framework. If not specified, PostgreSQL defaults to \"template1\".")]
        [DisplayName("EF Template Database")]
        [NpgsqlConnectionStringProperty]
        public string EntityTemplateDatabase
        {
            get { return _entityTemplateDatabase; }
            set
            {
                _entityTemplateDatabase = value;
                SetValue(nameof(EntityTemplateDatabase), value);
            }
        }
        string _entityTemplateDatabase;

        /// <summary>
        /// The database admin to specify when creating and dropping a database in Entity Framework. This is needed because
        /// Npgsql needs to connect to a database in order to send the create/drop database command.
        /// If not specified, defaults to "template1". Check NpgsqlServices.UsingPostgresDBConnection for more information.
        /// </summary>
        [Category("Entity Framework")]
        [Description("The database admin to specify when creating and dropping a database in Entity Framework. If not specified, defaults to \"template1\".")]
        [DisplayName("EF Admin Database")]
        [NpgsqlConnectionStringProperty]
        public string EntityAdminDatabase
        {
            get { return _entityAdminDatabase; }
            set
            {
                _entityAdminDatabase = value;
                SetValue(nameof(EntityAdminDatabase), value);
            }
        }
        string _entityAdminDatabase;

        #endregion

        #region Properties - Advanced

        /// <summary>
        /// Whether to process messages that arrive between command activity.
        /// </summary>
        [Category("Advanced")]
        [Description("Whether to process messages that arrive between command activity.")]
        [DisplayName("Continuous Processing")]
        [NpgsqlConnectionStringProperty("SyncNotification")]
        public bool ContinuousProcessing
        {
            get { return _continuousProcessing; }
            set
            {
                _continuousProcessing = value;
                SetValue(nameof(ContinuousProcessing), value);
            }
        }
        bool _continuousProcessing;

        /// <summary>
        /// The number of seconds of connection inactivity before Npgsql sends a keepalive query.
        /// Set to 0 (the default) to disable.
        /// </summary>
        [Category("Advanced")]
        [Description("The number of seconds of connection inactivity before Npgsql sends a keepalive query.")]
        [DisplayName("Keepalive")]
        [NpgsqlConnectionStringProperty]
        public int KeepAlive
        {
            get { return _keepAlive; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "KeepAlive can't be negative");
                Contract.EndContractBlock();

                _keepAlive = value;
                SetValue(nameof(KeepAlive), value);
            }
        }
        int _keepAlive;

        /// <summary>
        /// Gets or sets the buffer size.
        /// </summary>
        [Category("Advanced")]
        [Description("Determines the size of the internal buffer Npgsql uses when reading or writing. Increasing may improve performance if transferring large values from the database.")]
        [DisplayName("Buffer Size")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(NpgsqlBuffer.DefaultBufferSize)]
        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                _bufferSize = value;
                SetValue(nameof(BufferSize), value);
            }
        }
        int _bufferSize;

        #endregion

        #region Properties - Compatibility

        /// <summary>
        /// A compatibility mode for special PostgreSQL server types.
        /// </summary>
        [Category("Compatibility")]
        [Description("A compatibility mode for special PostgreSQL server types.")]
        [DisplayName("Server Compatibility Mode")]
        [NpgsqlConnectionStringProperty]
        public ServerCompatibilityMode ServerCompatibilityMode
        {
            get { return _serverCompatibilityMode; }
            set
            {
                _serverCompatibilityMode = value;
                SetValue(nameof(ServerCompatibilityMode), value);
            }
        }
        ServerCompatibilityMode _serverCompatibilityMode;

        /// <summary>
        /// Makes MaxValue and MinValue timestamps and dates readable as infinity and negative infinity.
        /// </summary>
        [Category("Compatibility")]
        [Description("Makes MaxValue and MinValue timestamps and dates readable as infinity and negative infinity.")]
        [DisplayName("Convert Infinity DateTime")]
        [NpgsqlConnectionStringProperty]
        public bool ConvertInfinityDateTime
        {
            get { return _convertInfinityDateTime; }
            set
            {
                _convertInfinityDateTime = value;
                SetValue(nameof(ConvertInfinityDateTime), value);
            }
        }
        bool _convertInfinityDateTime;

        #endregion

        #region Properties - Obsolete

        /// <summary>
        /// Obsolete, see https://github.com/npgsql/Npgsql/wiki/PreloadReader-Removal
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see https://github.com/npgsql/Npgsql/wiki/PreloadReader-Removal")]
        [DisplayName("Preload Reader")]
        [NpgsqlConnectionStringProperty]
        [Obsolete]
        public bool PreloadReader
        {
            get { return false; }
            set { throw new NotSupportedException("The PreloadReader parameter is no longer supported. Please see https://github.com/npgsql/Npgsql/wiki/PreloadReader-Removal"); }
        }

        /// <summary>
        /// Obsolete, see https://github.com/npgsql/Npgsql/wiki/UseExtendedTypes-Removal
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see https://github.com/npgsql/Npgsql/wiki/UseExtendedTypes-Removal")]
        [DisplayName("Use Extended Types")]
        [NpgsqlConnectionStringProperty]
        [Obsolete]
        public bool UseExtendedTypes
        {
            get { return false; }
            set { throw new NotSupportedException("The UseExtendedTypes parameter is no longer supported. Please see https://github.com/npgsql/Npgsql/wiki/UseExtendedTypes-Removal"); }
        }

        #endregion

        #region Integrated security support

        /// <summary>
        /// No integrated security if we're on mono and .NET 4.5 because of ClaimsIdentity,
        /// see https://github.com/npgsql/Npgsql/issues/133
        /// </summary>
        static void CheckIntegratedSecuritySupport()
        {
            if (Type.GetType("Mono.Runtime") != null)
                throw new NotSupportedException("IntegratedSecurity is currently unsupported on mono and .NET 4.5 (see https://github.com/npgsql/Npgsql/issues/133)");
        }

#if NET45 || NET452 || DNX452
        class CachedUpn
        {
            public string Upn;
            public DateTime ExpiryTimeUtc;
        }

        static readonly Dictionary<SecurityIdentifier, CachedUpn> CachedUpns = new Dictionary<SecurityIdentifier, CachedUpn>();

        private string GetWindowsIdentityUserName()
        {
            var identity = WindowsIdentity.GetCurrent();
            return identity == null ? string.Empty : identity.Name.Split('\\')[1];
        }

        [CanBeNull]
        string GetIntegratedUserName()
        {
            // Side note: This maintains the hack fix mentioned before for https://github.com/npgsql/Npgsql/issues/133.
            // In a nutshell, starting with .NET 4.5 WindowsIdentity inherits from ClaimsIdentity
            // which doesn't exist in mono, and calling a WindowsIdentity method bombs.
            // The workaround is that this function that actually deals with WindowsIdentity never
            // gets called on mono, so never gets JITted and the problem goes away.

            // Gets the current user's username for integrated security purposes
            var identity = WindowsIdentity.GetCurrent();
            if (identity?.User == null) {
                return null;
            }
            CachedUpn cachedUpn;
            string upn = null;

            // Check to see if we already have this UPN cached
            lock (CachedUpns) {
                if (CachedUpns.TryGetValue(identity.User, out cachedUpn)) {
                    if (cachedUpn.ExpiryTimeUtc > DateTime.UtcNow)
                        upn = cachedUpn.Upn;
                    else
                        CachedUpns.Remove(identity.User);
                }
            }

            try {
                if (upn == null) {
                    // Try to get the user's UPN in its correct case; this is what the
                    // server will need to verify against a Kerberos/SSPI ticket

                    // If the computer does not belong to a domain, returns Empty.
                    string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    if (domainName.Equals(string.Empty))
                    {
                        return GetWindowsIdentityUserName();
                    }

                    // First, find a domain server we can talk to
                    string domainHostName;

                    using (DirectoryEntry rootDse = new DirectoryEntry("LDAP://rootDSE") { AuthenticationType = AuthenticationTypes.Secure }) {
                        domainHostName = (string)rootDse.Properties["dnsHostName"].Value;
                    }

                    // Query the domain server by the current user's SID
                    using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + domainHostName) { AuthenticationType = AuthenticationTypes.Secure }) {
                        DirectorySearcher search = new DirectorySearcher(entry,
                            "(objectSid=" + identity.User.Value + ")", new[] { "userPrincipalName" });

                        SearchResult result = search.FindOne();

                        upn = (string)result.Properties["userPrincipalName"][0];
                    }
                }

                if (cachedUpn == null) {
                    // Save this value
                    cachedUpn = new CachedUpn() { Upn = upn, ExpiryTimeUtc = DateTime.UtcNow.AddHours(3.0) };

                    lock (CachedUpns) {
                        CachedUpns[identity.User] = cachedUpn;
                    }
                }

                string[] upnParts = upn.Split('@');

                if (_includeRealm) {
                    // Make it Kerberos-y by uppercasing the realm part
                    return upnParts[0] + "@" + upnParts[1].ToUpperInvariant();
                } else {
                    return upnParts[0];
                }
            } catch {
                // Querying the directory failed, so return the SAM name
                // (which probably won't work, but it's better than nothing)
                return GetWindowsIdentityUserName();
            }
        }
#endif

        #endregion

        #region Misc

        internal NpgsqlConnectionStringBuilder Clone()
        {
            return new NpgsqlConnectionStringBuilder(ConnectionString);
        }

        #endregion

        #region Attributes

        [AttributeUsage(AttributeTargets.Property)]
        [MeansImplicitUse]
        class NpgsqlConnectionStringPropertyAttribute : Attribute
        {
            internal string[] Aliases { get; }

            internal NpgsqlConnectionStringPropertyAttribute()
            {
                Aliases = Empty;
            }

            internal NpgsqlConnectionStringPropertyAttribute(params string[] aliases)
            {
                Aliases = aliases;
            }
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// An option specified in the connection string that activates special compatibility features.
    /// </summary>
    [PublicAPI]
    public enum ServerCompatibilityMode
    {
        /// <summary>
        /// No special server compatibility mode is active
        /// </summary>
        None,
        /// <summary>
        /// The server is an Amazon Redshift instance.
        /// </summary>
        Redshift,
    }

    /// <summary>
    /// Specifies how to manage SSL.
    /// </summary>
    [PublicAPI]
    public enum SslMode
    {
        /// <summary>
        /// SSL is disabled. If the server requires SSL, the connection will fail.
        /// </summary>
        Disable,
        /// <summary>
        /// Prefer SSL connections if the server allows them, but allow connections without SSL.
        /// </summary>
        Prefer,
        /// <summary>
        /// Fail the connection if the server doesn't suppotr SSL.
        /// </summary>
        Require,
    }

    #endregion
}
