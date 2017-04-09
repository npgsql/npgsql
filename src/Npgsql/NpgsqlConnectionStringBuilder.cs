#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Npgsql
{
    /// <summary>
    /// Provides a simple way to create and manage the contents of connection strings used by
    /// the <see cref="NpgsqlConnection"/> class.
    /// </summary>
    public sealed class NpgsqlConnectionStringBuilder : DbConnectionStringBuilder, IDictionary<string, object>
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the NpgsqlConnectionStringBuilder class.
        /// </summary>
        public NpgsqlConnectionStringBuilder() { Init(); }

#if NET45 || NET451
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
            // Set the strongly-typed properties to their default values
            foreach (var kv in PropertyDefaults)
                kv.Key.SetValue(this, kv.Value);
            // Setting the strongly-typed properties here also set the string-based properties in the base class.
            // Clear them (default settings = empty connection string)
            base.Clear();
        }

        #endregion

        #region Static initialization

        static NpgsqlConnectionStringBuilder()
        {
            var properties = typeof(NpgsqlConnectionStringBuilder)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<NpgsqlConnectionStringPropertyAttribute>() != null)
                .ToArray();

            Debug.Assert(properties.All(p => p.CanRead && p.CanWrite));
            Debug.Assert(properties.All(p => p.GetCustomAttribute<DisplayNameAttribute>() != null));

            PropertiesByKeyword = (
                from p in properties
                let displayName = p.GetCustomAttribute<DisplayNameAttribute>().DisplayName.ToUpperInvariant()
                let propertyName = p.Name.ToUpperInvariant()
                from k in new[] { displayName }
                  .Concat(propertyName != displayName ? new[] { propertyName } : EmptyStringArray )
                  .Concat(p.GetCustomAttribute<NpgsqlConnectionStringPropertyAttribute>().Synonyms
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
        public override object this[[NotNull] string keyword]
        {
            get
            {
                if (!TryGetValue(keyword, out var value))
                    throw new ArgumentException("Keyword not supported: " + keyword, nameof(keyword));
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
        /// Adds an item to the <see cref="NpgsqlConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="item">The key-value pair to be added.</param>
        public void Add(KeyValuePair<string, object> item)
            => this[item.Key] = item.Value;

        /// <summary>
        /// Removes the entry with the specified key from the DbConnectionStringBuilder instance.
        /// </summary>
        /// <param name="keyword">The key of the key/value pair to be removed from the connection string in this DbConnectionStringBuilder.</param>
        /// <returns><b>true</b> if the key existed within the connection string and was removed; <b>false</b> if the key did not exist.</returns>
        public override bool Remove([NotNull] string keyword)
        {
            var p = GetProperty(keyword);
            var cannonicalName = PropertyNameToCanonicalKeyword[p.Name];
            var removed = base.ContainsKey(cannonicalName);
            // Note that string property setters call SetValue, which itself calls base.Remove().
            p.SetValue(this, PropertyDefaults[p]);
            base.Remove(cannonicalName);
            return removed;
        }

        /// <summary>
        /// Removes the entry from the DbConnectionStringBuilder instance.
        /// </summary>
        /// <param name="item">The key/value pair to be removed from the connection string in this DbConnectionStringBuilder.</param>
        /// <returns><b>true</b> if the key existed within the connection string and was removed; <b>false</b> if the key did not exist.</returns>
        public bool Remove(KeyValuePair<string, object> item)
            => Remove(item.Key);

        /// <summary>
        /// Clears the contents of the <see cref="NpgsqlConnectionStringBuilder"/> instance.
        /// </summary>
        public override void Clear()
        {
            Debug.Assert(Keys != null);
            foreach (var k in Keys.ToArray()) {
                Remove(k);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="NpgsqlConnectionStringBuilder"/> contains a specific key.
        /// </summary>
        /// <param name="keyword">The key to locate in the <see cref="NpgsqlConnectionStringBuilder"/>.</param>
        /// <returns><b>true</b> if the <see cref="NpgsqlConnectionStringBuilder"/> contains an entry with the specified key; otherwise <b>false</b>.</returns>
        public override bool ContainsKey([CanBeNull] string keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));

            return PropertiesByKeyword.ContainsKey(keyword.ToUpperInvariant());
        }

        /// <summary>
        /// Determines whether the <see cref="NpgsqlConnectionStringBuilder"/> contains a specific key-value pair.
        /// </summary>
        /// <param name="item">The itemto locate in the <see cref="NpgsqlConnectionStringBuilder"/>.</param>
        /// <returns><b>true</b> if the <see cref="NpgsqlConnectionStringBuilder"/> contains the entry; otherwise <b>false</b>.</returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return TryGetValue(item.Key, out var value) &&
                ((value == null && item.Value == null) || (value != null && value.Equals(item.Value)));
        }

        PropertyInfo GetProperty(string keyword)
        {
            if (!PropertiesByKeyword.TryGetValue(keyword.ToUpperInvariant(), out var p))
                throw new ArgumentException("Keyword not supported: " + keyword, nameof(keyword));
            return p;
        }

        /// <summary>
        /// Retrieves a value corresponding to the supplied key from this <see cref="NpgsqlConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="keyword">The key of the item to retrieve.</param>
        /// <param name="value">The value corresponding to the key.</param>
        /// <returns><b>true</b> if keyword was found within the connection string, <b>false</b> otherwise.</returns>
        public override bool TryGetValue([NotNull] string keyword, [CanBeNull] out object value)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));

            if (!PropertiesByKeyword.ContainsKey(keyword.ToUpperInvariant()))
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
        [CanBeNull]
        public string Host
        {
            get => _host;
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
            get => _port;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid port: " + value);

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
        [CanBeNull]
        public string Database
        {
            get => _database;
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
        [CanBeNull]
        public string Username
        {
            get => _username;
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
        [CanBeNull]
        public string Password
        {
            get => _password;
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
            get => _applicationName;
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
            get => _enlist;
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
            get => _searchpath;
            set
            {
                _searchpath = value;
                SetValue(nameof(SearchPath), value);
            }
        }
        string _searchpath;

        /// <summary>
        /// Gets or sets the client_encoding parameter.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets the client_encoding parameter.")]
        [DisplayName("Client Encoding")]
        [NpgsqlConnectionStringProperty]
        [CanBeNull]
        public string ClientEncoding
        {
            get => _clientEncoding;
            set
            {
                _clientEncoding = value;
                SetValue(nameof(ClientEncoding), value);
            }
        }
        string _clientEncoding;

        /// <summary>
        /// Gets or sets the .NET encoding that will be used to encode/decode PostgreSQL string data.
        /// </summary>
        [Category("Connection")]
        [Description("Gets or sets the .NET encoding that will be used to encode/decode PostgreSQL string data.")]
        [DisplayName("Encoding")]
        [DefaultValue("UTF8")]
        [NpgsqlConnectionStringProperty]
        public string Encoding
        {
            get => _encoding;
            set
            {
                _encoding = value;
                SetValue(nameof(Encoding), value);
            }
        }
        string _encoding;

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
            get => _sslmode;
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
            get => _trustServerCertificate;
            set
            {
                _trustServerCertificate = value;
                SetValue(nameof(TrustServerCertificate), value);
            }
        }
        bool _trustServerCertificate;

        /// <summary>
        /// Whether to check the certificate revocation list during authentication.
        /// False by default.
        /// </summary>
        [Category("Security")]
        [Description("Whether to check the certificate revocation list during authentication.")]
        [DisplayName("Check Certificate Revocation")]
        [NpgsqlConnectionStringProperty]
        public bool CheckCertificateRevocation
        {
            get => _checkCertificateRevocation;
            set
            {
                _checkCertificateRevocation = value;
                SetValue(nameof(CheckCertificateRevocation), value);
            }
        }
        bool _checkCertificateRevocation;

        /// <summary>
        /// Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead.
        /// </summary>
        [Category("Security")]
        [Description("Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead.")]
        [DisplayName("Use SSL Stream")]
        [NpgsqlConnectionStringProperty]
        public bool UseSslStream
        {
            get => _useSslStream;
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
            get => _integratedSecurity;
            set
            {
                // No integrated security if we're on mono and .NET 4.5 because of ClaimsIdentity,
                // see https://github.com/npgsql/Npgsql/issues/133
                if (value && Type.GetType("Mono.Runtime") != null)
                    throw new NotSupportedException("IntegratedSecurity is currently unsupported on mono and .NET 4.5 (see https://github.com/npgsql/Npgsql/issues/133)");
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
        [DefaultValue("postgres")]
        public string KerberosServiceName
        {
            get => _kerberosServiceName;
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
            get => _includeRealm;
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
            get => _persistSecurityInfo;
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
            get => _pooling;
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
        [DefaultValue(0)]
        public int MinPoolSize
        {
            get => _minPoolSize;
            set
            {
                if (value < 0 || value > PoolManager.PoolSizeLimit)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MinPoolSize must be between 0 and " + PoolManager.PoolSizeLimit);

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
        [DefaultValue(100)]
        public int MaxPoolSize
        {
            get => _maxPoolSize;
            set
            {
                if (value < 0 || value > PoolManager.PoolSizeLimit)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxPoolSize must be between 0 and " + PoolManager.PoolSizeLimit);

                _maxPoolSize = value;
                SetValue(nameof(MaxPoolSize), value);
            }
        }
        int _maxPoolSize;

        /// <summary>
        /// The time to wait before closing idle connections in the pool if the count
        /// of all connections exceeds MinPoolSize.
        /// </summary>
        /// <value>The time (in seconds) to wait. The default value is 300.</value>
        [Category("Pooling")]
        [Description("The time to wait before closing unused connections in the pool if the count of all connections exeeds MinPoolSize.")]
        [DisplayName("Connection Idle Lifetime")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(300)]
        public int ConnectionIdleLifetime
        {
            get => _connectionIdleLifetime;
            set
            {
                _connectionIdleLifetime = value;
                SetValue(nameof(ConnectionIdleLifetime), value);
            }
        }
        int _connectionIdleLifetime;

        /// <summary>
        /// How many seconds the pool waits before attempting to prune idle connections that are beyond
        /// idle lifetime (<see cref="ConnectionIdleLifetime"/>.
        /// </summary>
        /// <value>The interval (in seconds). The default value is 10.</value>
        [Category("Pooling")]
        [Description("How many seconds the pool waits before attempting to prune idle connections that are beyond idle lifetime.")]
        [DisplayName("Connection Pruning Interval")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(10)]
        public int ConnectionPruningInterval
        {
            get => _connectionPruningInterval;
            set
            {
                _connectionPruningInterval = value;
                SetValue(nameof(ConnectionPruningInterval), value);
            }
        }
        int _connectionPruningInterval;

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
            get => _timeout;
            set
            {
                if (value < 0 || value > NpgsqlConnection.TimeoutLimit)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Timeout must be between 0 and " + NpgsqlConnection.TimeoutLimit);

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
            get => _commandTimeout;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "CommandTimeout can't be negative");

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
            get => _internalCommandTimeout;
            set
            {
                if (value != 0 && value != -1 && value < NpgsqlConnector.MinimumInternalCommandTimeout)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"InternalCommandTimeout must be >= {NpgsqlConnector.MinimumInternalCommandTimeout}, 0 (infinite) or -1 (use CommandTimeout)");

                _internalCommandTimeout = value;
                SetValue(nameof(InternalCommandTimeout), value);
            }
        }
        int _internalCommandTimeout;

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
            get => _entityTemplateDatabase;
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
            get => _entityAdminDatabase;
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
        /// The number of seconds of connection inactivity before Npgsql sends a keepalive query.
        /// Set to 0 (the default) to disable.
        /// </summary>
        [Category("Advanced")]
        [Description("The number of seconds of connection inactivity before Npgsql sends a keepalive query.")]
        [DisplayName("Keepalive")]
        [NpgsqlConnectionStringProperty]
        public int KeepAlive
        {
            get => _keepAlive;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "KeepAlive can't be negative");

                _keepAlive = value;
                SetValue(nameof(KeepAlive), value);
            }
        }
        int _keepAlive;

        /// <summary>
        /// Determines the size of the internal buffer Npgsql uses when reading. Increasing may improve performance if transferring large values from the database.
        /// </summary>
        [Category("Advanced")]
        [Description("Determines the size of the internal buffer Npgsql uses when reading. Increasing may improve performance if transferring large values from the database.")]
        [DisplayName("Read Buffer Size")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(ReadBuffer.DefaultSize)]
        public int ReadBufferSize
        {
            get => _readBufferSize;
            set
            {
                _readBufferSize = value;
                SetValue(nameof(ReadBufferSize), value);
            }
        }
        int _readBufferSize;

        /// <summary>
        /// Determines the size of the internal buffer Npgsql uses when writing. Increasing may improve performance if transferring large values to the database.
        /// </summary>
        [Category("Advanced")]
        [Description("Determines the size of the internal buffer Npgsql uses when writing. Increasing may improve performance if transferring large values to the database.")]
        [DisplayName("Write Buffer Size")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(WriteBuffer.DefaultSize)]
        public int WriteBufferSize
        {
            get => _writeBufferSize;
            set
            {
                _writeBufferSize = value;
                SetValue(nameof(WriteBufferSize), value);
            }
        }
        int _writeBufferSize;

        /// <summary>
        /// Determines the size of socket read buffer.
        /// </summary>
        [Category("Advanced")]
        [Description("Determines the size of socket receive buffer.")]
        [DisplayName("Socket Receive Buffer Size")]
        [NpgsqlConnectionStringProperty]
        [CanBeNull]
        public int SocketReceiveBufferSize
        {
            get => _socketReceiveBufferSize;
            set
            {
                _socketReceiveBufferSize = value;
                SetValue(nameof(SocketReceiveBufferSize), value);
            }
        }
        int _socketReceiveBufferSize;

        /// <summary>
        /// Determines the size of socket send buffer.
        /// </summary>
        [Category("Advanced")]
        [Description("Determines the size of socket send buffer.")]
        [DisplayName("Socket Send Buffer Size")]
        [NpgsqlConnectionStringProperty]
        public int SocketSendBufferSize
        {
            get => _socketSendBufferSize;
            set
            {
                _socketSendBufferSize = value;
                SetValue(nameof(SocketSendBufferSize), value);
            }
        }
        int _socketSendBufferSize;

        /// <summary>
        /// The maximum number SQL statements that can be automatically prepared at any given point.
        /// Beyond this number the least-recently-used statement will be recycled.
        /// Zero (the default) disables automatic preparation.
        /// </summary>
        [Category("Advanced")]
        [Description("The maximum number SQL statements that can be automatically prepared at any given point. Beyond this number the least-recently-used statement will be recycled. Zero (the default) disables automatic preparation.")]
        [DisplayName("Max Auto Prepare")]
        [NpgsqlConnectionStringProperty]
        public int MaxAutoPrepare
        {
            get => _maxAutoPrepare;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxAutoPrepare)} cannot be negative");

                _maxAutoPrepare = value;
                SetValue(nameof(MaxAutoPrepare), value);
            }
        }
        int _maxAutoPrepare;

        /// <summary>
        /// The minimum number of usages an SQL statement is used before it's automatically prepared.
        /// Defaults to 5.
        /// </summary>
        [Category("Advanced")]
        [Description("The minimum number of usages an SQL statement is used before it's automatically prepared. Defaults to 5.")]
        [DisplayName("Auto Prepare Min Usages")]
        [NpgsqlConnectionStringProperty]
        [DefaultValue(5)]
        public int AutoPrepareMinUsages
        {
            get => _autoPrepareMinUsages;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(AutoPrepareMinUsages)} must be 1 or greater");

                _autoPrepareMinUsages = value;
                SetValue(nameof(AutoPrepareMinUsages), value);
            }
        }
        int _autoPrepareMinUsages;

        /// <summary>
        /// Writes connection performance information to performance counters.
        /// </summary>
        [Category("Advanced")]
        [Description("Writes connection performance information to performance counters.")]
        [DisplayName("Use Perf Counters")]
        [NpgsqlConnectionStringProperty]
        public bool UsePerfCounters
        {
            get => _usePerfCounters;
            set
            {
                _usePerfCounters = value;
                SetValue(nameof(UsePerfCounters), value);
            }
        }
        bool _usePerfCounters;

        /// <summary>
        /// If set to true, a pool connection's state won't be reset when it is closed (improves performance).
        /// Do not specify this unless you know what you're doing.
        /// </summary>
        [Category("Advanced")]
        [Description("If set to true, a pool connection's state won't be reset when it is closed (improves performance). Do not specify this unless you know what you're doing.")]
        [DisplayName("No Reset On Close")]
        [NpgsqlConnectionStringProperty]
        public bool NoResetOnClose
        {
            get => _noResetOnClose;
            set
            {
                _noResetOnClose = value;
                SetValue(nameof(NoResetOnClose), value);
            }
        }
        bool _noResetOnClose;

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
            get => _serverCompatibilityMode;
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
            get => _convertInfinityDateTime;
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
        /// Obsolete, see http://www.npgsql.org/doc/migration/3.1.html
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see http://www.npgsql.org/doc/migration/3.1.html")]
        [DisplayName("Connection Lifetime")]
        [NpgsqlConnectionStringProperty]
        [Obsolete("The ConnectionLifeTime parameter is no longer supported")]
        public int ConnectionLifeTime
        {
            get => 0;
            set => throw new NotSupportedException("The ConnectionLifeTime parameter is no longer supported. Please see http://www.npgsql.org/doc/migration/3.1.html");
        }

        /// <summary>
        /// Obsolete, see http://www.npgsql.org/doc/migration/3.1.html
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see http://www.npgsql.org/doc/migration/3.1.html")]
        [DisplayName("Continuous Processing")]
        [NpgsqlConnectionStringProperty]
        [Obsolete("The ContinuousProcessing parameter is no longer supported.")]
        public bool ContinuousProcessing
        {
            get => false;
            set => throw new NotSupportedException("The ContinuousProcessing parameter is no longer supported. Please see http://www.npgsql.org/doc/migration/3.1.html");
        }

        /// <summary>
        /// Obsolete, see http://www.npgsql.org/doc/migration/3.1.html
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see http://www.npgsql.org/doc/migration/3.1.html")]
        [DisplayName("Backend Timeouts")]
        [NpgsqlConnectionStringProperty]
        [Obsolete("The BackendTimeouts parameter is no longer supported")]
        public bool BackendTimeouts
        {
            get => false;
            set => throw new NotSupportedException("The BackendTimeouts parameter is no longer supported. Please see http://www.npgsql.org/doc/migration/3.1.html");
        }

        /// <summary>
        /// Obsolete, see http://www.npgsql.org/doc/migration/3.0.html
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see http://www.npgsql.org/doc/migration/3.0.html")]
        [DisplayName("Preload Reader")]
        [NpgsqlConnectionStringProperty]
        [Obsolete("The PreloadReader parameter is no longer supported")]
        public bool PreloadReader
        {
            get => false;
            set => throw new NotSupportedException("The PreloadReader parameter is no longer supported. Please see http://www.npgsql.org/doc/migration/3.0.html");
        }

        /// <summary>
        /// Obsolete, see http://www.npgsql.org/doc/migration/3.0.html
        /// </summary>
        [Category("Obsolete")]
        [Description("Obsolete, see http://www.npgsql.org/doc/migration/3.0.html")]
        [DisplayName("Use Extended Types")]
        [NpgsqlConnectionStringProperty]
        [Obsolete("The UseExtendedTypes parameter is no longer supported")]
        public bool UseExtendedTypes
        {
            get => false;
            set => throw new NotSupportedException("The UseExtendedTypes parameter is no longer supported. Please see http://www.npgsql.org/doc/migration/3.0.html");
        }

        #endregion

        #region Misc

        internal string ToStringWithoutPassword()
        {
            var clone = Clone();
            clone.Password = null;
            return clone.ToString();
        }

        internal NpgsqlConnectionStringBuilder Clone() => new NpgsqlConnectionStringBuilder(ConnectionString);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            var o = obj as NpgsqlConnectionStringBuilder;
            return o != null && EquivalentTo(o);
        }

        /// <summary>
        /// Hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Host?.GetHashCode() ?? 0;

        #endregion

        #region IDictionary<string, object>

        /// <summary>
        /// Gets an ICollection{string} containing the keys of the <see cref="NpgsqlConnectionStringBuilder"/>.
        /// </summary>
        public new ICollection<string> Keys => new List<string>(base.Keys.Cast<string>());

        /// <summary>
        /// Gets an ICollection{string} containing the values in the <see cref="NpgsqlConnectionStringBuilder"/>.
        /// </summary>
        public new ICollection<object> Values => base.Values.Cast<object>().ToList();

        /// <summary>
        /// Copies the elements of the <see cref="NpgsqlConnectionStringBuilder"/> to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from <see cref="NpgsqlConnectionStringBuilder"/>.
        /// The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo([NotNull] KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (var kv in this)
                array[arrayIndex++] = kv;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="NpgsqlConnectionStringBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var k in Keys)
                yield return new KeyValuePair<string, object>(k, this[k]);
        }

#if !(NET45 || NET451)
        /// <summary>
        /// Gets a value indicating whether the ICollection{T} is read-only.
        /// </summary>
        public bool IsReadOnly => false;
#endif
        #endregion IDictionary<string, object>

        #region ICustomTypeDescriptor

#if NET45 || NET451
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void GetProperties(Hashtable propertyDescriptors)
        {
            // Tweak which properties are exposed via TypeDescriptor. This affects the VS DDEX
            // provider, for example.
            base.GetProperties(propertyDescriptors);

            var toRemove = propertyDescriptors.Values
                .Cast<PropertyDescriptor>()
                .Where(d =>
                    !d.Attributes.Cast<Attribute>().Any(a => a is NpgsqlConnectionStringPropertyAttribute) ||
                    d.Attributes.Cast<Attribute>().Any(a => a is ObsoleteAttribute)
                )
                .ToList();
            foreach (var o in toRemove)
                propertyDescriptors.Remove(o.DisplayName);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif

        #endregion

        #region Attributes

#if NETSTANDARD1_3
        [AttributeUsage(AttributeTargets.Property)]
        class DescriptionAttribute : Attribute
        {
            internal DescriptionAttribute(string description) { }
        }
#endif

        #endregion

        internal static readonly string[] EmptyStringArray = new string[0];
    }

    #region Attributes

    /// <summary>
    /// Marks on <see cref="NpgsqlConnectionStringBuilder"/> which participate in the connection
    /// string. Optionally holds a set of synonyms for the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [MeansImplicitUse]
    public class NpgsqlConnectionStringPropertyAttribute : Attribute
    {
        /// <summary>
        /// Holds a list of synonyms for the property.
        /// </summary>
        public string[] Synonyms { get; }

        /// <summary>
        /// Creates a <see cref="NpgsqlConnectionStringPropertyAttribute"/>.
        /// </summary>
        public NpgsqlConnectionStringPropertyAttribute()
        {
            Synonyms = NpgsqlConnectionStringBuilder.EmptyStringArray;
        }

        /// <summary>
        /// Creates a <see cref="NpgsqlConnectionStringPropertyAttribute"/>.
        /// </summary>
        public NpgsqlConnectionStringPropertyAttribute(params string[] synonyms)
        {
            Synonyms = synonyms;
        }
    }

    #endregion

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
