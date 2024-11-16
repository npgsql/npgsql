using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal;
using Npgsql.Replication;

namespace Npgsql;

/// <summary>
/// Provides a simple way to create and manage the contents of connection strings used by
/// the <see cref="NpgsqlConnection"/> class.
/// </summary>
[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2112:ReflectionToRequiresUnreferencedCode",
    Justification = "Suppressing the same warnings as suppressed in the base DbConnectionStringBuilder. See https://github.com/dotnet/runtime/issues/97057")]
[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2113:ReflectionToRequiresUnreferencedCode",
    Justification = "Suppressing the same warnings as suppressed in the base DbConnectionStringBuilder. See https://github.com/dotnet/runtime/issues/97057")]
public sealed partial class NpgsqlConnectionStringBuilder : DbConnectionStringBuilder, IDictionary<string, object?>
{
    #region Fields

    /// <summary>
    /// Cached DataSource value to reduce allocations on NpgsqlConnection.DataSource.get
    /// </summary>
    string? _dataSourceCached;

    internal string? DataSourceCached
        => _dataSourceCached ??= _host is null || _host.Contains(",")
            ? null
            : IsUnixSocket(_host, _port, out var socketPath, replaceForAbstract: false)
                ? socketPath
                : $"tcp://{_host}:{_port}";

    // Note that we can't cache the result due to nullable's assignment not being thread safe
    internal TimeSpan HostRecheckSecondsTranslated
        => TimeSpan.FromSeconds(HostRecheckSeconds == 0 ? -1 : HostRecheckSeconds);

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the NpgsqlConnectionStringBuilder class.
    /// </summary>
    public NpgsqlConnectionStringBuilder() => Init();

    /// <summary>
    /// Initializes a new instance of the NpgsqlConnectionStringBuilder class, optionally using ODBC rules for quoting values.
    /// </summary>
    /// <param name="useOdbcRules">true to use {} to delimit fields; false to use quotation marks.</param>
    public NpgsqlConnectionStringBuilder(bool useOdbcRules) : base(useOdbcRules) => Init();

    /// <summary>
    /// Initializes a new instance of the NpgsqlConnectionStringBuilder class and sets its <see cref="DbConnectionStringBuilder.ConnectionString"/>.
    /// </summary>
    public NpgsqlConnectionStringBuilder(string? connectionString)
    {
        Init();
        ConnectionString = connectionString;
    }

    // Method fake-returns an int only to make sure it's code-generated
    private partial int Init();

    /// <summary>
    /// GeneratedAction and GeneratedActions exist to be able to produce a streamlined binary footprint for NativeAOT.
    /// An idiomatic approach where each action has its own method would double the binary size of NpgsqlConnectionStringBuilder.
    /// </summary>
    enum GeneratedAction
    {
        Set,
        Get,
        Remove,
        GetCanonical
    }
    private partial bool GeneratedActions(GeneratedAction action, string keyword, ref object? value);

    #endregion

    #region Non-static property handling

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="keyword">The key of the item to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    [AllowNull]
    public override object this[string keyword]
    {
        get
        {
            if (!TryGetValue(keyword, out var value))
                throw new ArgumentException("Keyword not supported: " + keyword, nameof(keyword));
            return value;
        }
        set
        {
            if (value is null)
            {
                Remove(keyword);
                return;
            }

            try
            {
                var val = value;
                GeneratedActions(GeneratedAction.Set, keyword.ToUpperInvariant(), ref val);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Couldn't set " + keyword, keyword, e);
            }
        }
    }

    object? IDictionary<string, object?>.this[string keyword]
    {
        get => this[keyword];
        set => this[keyword] = value!;
    }

    /// <summary>
    /// Adds an item to the <see cref="NpgsqlConnectionStringBuilder"/>.
    /// </summary>
    /// <param name="item">The key-value pair to be added.</param>
    public void Add(KeyValuePair<string, object?> item)
        => this[item.Key] = item.Value!;

    void IDictionary<string, object?>.Add(string keyword, object? value)
        => this[keyword] = value;

    /// <summary>
    /// Removes the entry with the specified key from the DbConnectionStringBuilder instance.
    /// </summary>
    /// <param name="keyword">The key of the key/value pair to be removed from the connection string in this DbConnectionStringBuilder.</param>
    /// <returns><b>true</b> if the key existed within the connection string and was removed; <b>false</b> if the key did not exist.</returns>
    public override bool Remove(string keyword)
    {
        object? value = null;
        return GeneratedActions(GeneratedAction.Remove, keyword.ToUpperInvariant(), ref value);
    }

    /// <summary>
    /// Removes the entry from the DbConnectionStringBuilder instance.
    /// </summary>
    /// <param name="item">The key/value pair to be removed from the connection string in this DbConnectionStringBuilder.</param>
    /// <returns><b>true</b> if the key existed within the connection string and was removed; <b>false</b> if the key did not exist.</returns>
    public bool Remove(KeyValuePair<string, object?> item)
        => Remove(item.Key);

    /// <summary>
    /// Clears the contents of the <see cref="NpgsqlConnectionStringBuilder"/> instance.
    /// </summary>
    public override void Clear()
    {
        Debug.Assert(Keys != null);
        foreach (var k in (string[])Keys)
            Remove(k);
    }

    /// <summary>
    /// Determines whether the <see cref="NpgsqlConnectionStringBuilder"/> contains a specific key.
    /// </summary>
    /// <param name="keyword">The key to locate in the <see cref="NpgsqlConnectionStringBuilder"/>.</param>
    /// <returns><b>true</b> if the <see cref="NpgsqlConnectionStringBuilder"/> contains an entry with the specified key; otherwise <b>false</b>.</returns>
    public override bool ContainsKey(string keyword)
    {
        object? value = null;
        return GeneratedActions(GeneratedAction.GetCanonical, (keyword ?? throw new ArgumentNullException(nameof(keyword))).ToUpperInvariant(), ref value);
    }

    /// <summary>
    /// Determines whether the <see cref="NpgsqlConnectionStringBuilder"/> contains a specific key-value pair.
    /// </summary>
    /// <param name="item">The item to locate in the <see cref="NpgsqlConnectionStringBuilder"/>.</param>
    /// <returns><b>true</b> if the <see cref="NpgsqlConnectionStringBuilder"/> contains the entry; otherwise <b>false</b>.</returns>
    public bool Contains(KeyValuePair<string, object?> item)
        => TryGetValue(item.Key, out var value) &&
           ((value == null && item.Value == null) || (value != null && value.Equals(item.Value)));

    /// <summary>
    /// Retrieves a value corresponding to the supplied key from this <see cref="NpgsqlConnectionStringBuilder"/>.
    /// </summary>
    /// <param name="keyword">The key of the item to retrieve.</param>
    /// <param name="value">The value corresponding to the key.</param>
    /// <returns><b>true</b> if keyword was found within the connection string, <b>false</b> otherwise.</returns>
    public override bool TryGetValue(string keyword, [NotNullWhen(true)] out object? value)
    {
        object? v = null;
        var result = GeneratedActions(GeneratedAction.Get, (keyword ?? throw new ArgumentNullException(nameof(keyword))).ToUpperInvariant(), ref v);
        value = v;
        return result;
    }

    void SetValue(string propertyName, object? value)
    {
        object? canonicalKeyword = null;
        var result = GeneratedActions(GeneratedAction.GetCanonical, (propertyName ?? throw new ArgumentNullException(nameof(propertyName))).ToUpperInvariant(), ref canonicalKeyword);
        if (!result)
            throw new KeyNotFoundException();
        if (value == null)
            base.Remove((string)canonicalKeyword!);
        else
            base[(string)canonicalKeyword!] = value;
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
    public string? Host
    {
        get => _host;
        set
        {
            _host = value;
            SetValue(nameof(Host), value);
            _dataSourceCached = null;
        }
    }
    string? _host;

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
            _dataSourceCached = null;
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
    public string? Database
    {
        get => _database;
        set
        {
            _database = value;
            SetValue(nameof(Database), value);
        }
    }
    string? _database;

    /// <summary>
    /// The username to connect with.
    /// </summary>
    [Category("Connection")]
    [Description("The username to connect with.")]
    [DisplayName("Username")]
    [NpgsqlConnectionStringProperty("User Name", "UserId", "User Id", "UID")]
    public string? Username
    {
        get => _username;
        set
        {
            _username = value;
            SetValue(nameof(Username), value);
        }
    }
    string? _username;

    /// <summary>
    /// The password to connect with.
    /// </summary>
    [Category("Connection")]
    [Description("The password to connect with.")]
    [PasswordPropertyText(true)]
    [DisplayName("Password")]
    [NpgsqlConnectionStringProperty("PSW", "PWD")]
    public string? Password
    {
        get => _password;
        set
        {
            _password = value;
            SetValue(nameof(Password), value);
        }
    }
    string? _password;

    /// <summary>
    /// Path to a PostgreSQL password file (PGPASSFILE), from which the password would be taken.
    /// </summary>
    [Category("Connection")]
    [Description("Path to a PostgreSQL password file (PGPASSFILE), from which the password would be taken.")]
    [DisplayName("Passfile")]
    [NpgsqlConnectionStringProperty]
    public string? Passfile
    {
        get => _passfile;
        set
        {
            _passfile = value;
            SetValue(nameof(Passfile), value);
        }
    }

    string? _passfile;

    /// <summary>
    /// The optional application name parameter to be sent to the backend during connection initiation.
    /// </summary>
    [Category("Connection")]
    [Description("The optional application name parameter to be sent to the backend during connection initiation")]
    [DisplayName("Application Name")]
    [NpgsqlConnectionStringProperty]
    public string? ApplicationName
    {
        get => _applicationName;
        set
        {
            _applicationName = value;
            SetValue(nameof(ApplicationName), value);
        }
    }
    string? _applicationName;

    /// <summary>
    /// Whether to enlist in an ambient TransactionScope.
    /// </summary>
    [Category("Connection")]
    [Description("Whether to enlist in an ambient TransactionScope.")]
    [DisplayName("Enlist")]
    [DefaultValue(true)]
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
    public string? SearchPath
    {
        get => _searchPath;
        set
        {
            _searchPath = value;
            SetValue(nameof(SearchPath), value);
        }
    }
    string? _searchPath;

    /// <summary>
    /// Gets or sets the client_encoding parameter.
    /// </summary>
    [Category("Connection")]
    [Description("Gets or sets the client_encoding parameter.")]
    [DisplayName("Client Encoding")]
    [NpgsqlConnectionStringProperty]
    public string? ClientEncoding
    {
        get => _clientEncoding;
        set
        {
            _clientEncoding = value;
            SetValue(nameof(ClientEncoding), value);
        }
    }
    string? _clientEncoding;

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
    string _encoding = "UTF8";

    /// <summary>
    /// Gets or sets the PostgreSQL session timezone, in Olson/IANA database format.
    /// </summary>
    [Category("Connection")]
    [Description("Gets or sets the PostgreSQL session timezone, in Olson/IANA database format.")]
    [DisplayName("Timezone")]
    [NpgsqlConnectionStringProperty]
    public string? Timezone
    {
        get => _timezone;
        set
        {
            _timezone = value;
            SetValue(nameof(Timezone), value);
        }
    }
    string? _timezone;

    #endregion

    #region Properties - Security

    /// <summary>
    /// Controls whether SSL is required, disabled or preferred, depending on server support.
    /// </summary>
    [Category("Security")]
    [Description("Controls whether SSL is required, disabled or preferred, depending on server support.")]
    [DisplayName("SSL Mode")]
    [DefaultValue(SslMode.Prefer)]
    [NpgsqlConnectionStringProperty]
    public SslMode SslMode
    {
        get => _sslMode;
        set
        {
            _sslMode = value;
            SetValue(nameof(SslMode), value);
        }
    }
    SslMode _sslMode;

    /// <summary>
    /// Controls how SSL encryption is negotiated with the server, if SSL is used.
    /// </summary>
    [Category("Security")]
    [Description("Controls how SSL encryption is negotiated with the server, if SSL is used.")]
    [DisplayName("SSL Negotiation")]
    [NpgsqlConnectionStringProperty]
    public SslNegotiation SslNegotiation
    {
        get => UserProvidedSslNegotiation ?? SslNegotiation.Postgres;
        set
        {
            UserProvidedSslNegotiation = value;
            SetValue(nameof(SslNegotiation), value);
        }
    }

    internal SslNegotiation? UserProvidedSslNegotiation { get; private set; }

    /// <summary>
    /// Location of a client certificate to be sent to the server.
    /// </summary>
    [Category("Security")]
    [Description("Location of a client certificate to be sent to the server.")]
    [DisplayName("SSL Certificate")]
    [NpgsqlConnectionStringProperty]
    public string? SslCertificate
    {
        get => _sslCertificate;
        set
        {
            _sslCertificate = value;
            SetValue(nameof(SslCertificate), value);
        }
    }
    string? _sslCertificate;

    /// <summary>
    /// Location of a client key for a client certificate to be sent to the server.
    /// </summary>
    [Category("Security")]
    [Description("Location of a client key for a client certificate to be sent to the server.")]
    [DisplayName("SSL Key")]
    [NpgsqlConnectionStringProperty]
    public string? SslKey
    {
        get => _sslKey;
        set
        {
            _sslKey = value;
            SetValue(nameof(SslKey), value);
        }
    }
    string? _sslKey;

    /// <summary>
    /// Password for a key for a client certificate.
    /// </summary>
    [Category("Security")]
    [Description("Password for a key for a client certificate.")]
    [DisplayName("SSL Password")]
    [NpgsqlConnectionStringProperty]
    public string? SslPassword
    {
        get => _sslPassword;
        set
        {
            _sslPassword = value;
            SetValue(nameof(SslPassword), value);
        }
    }
    string? _sslPassword;

    /// <summary>
    /// Location of a CA certificate used to validate the server certificate.
    /// </summary>
    [Category("Security")]
    [Description("Location of a CA certificate used to validate the server certificate.")]
    [DisplayName("Root Certificate")]
    [NpgsqlConnectionStringProperty]
    public string? RootCertificate
    {
        get => _rootCertificate;
        set
        {
            _rootCertificate = value;
            SetValue(nameof(RootCertificate), value);
        }
    }
    string? _rootCertificate;

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
    string _kerberosServiceName = "postgres";

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

    /// <summary>
    /// When enabled, parameter values are logged when commands are executed. Defaults to false.
    /// </summary>
    [Category("Security")]
    [Description("When enabled, parameter values are logged when commands are executed. Defaults to false.")]
    [DisplayName("Log Parameters")]
    [NpgsqlConnectionStringProperty]
    public bool LogParameters
    {
        get => _logParameters;
        set
        {
            _logParameters = value;
            SetValue(nameof(LogParameters), value);
        }
    }
    bool _logParameters;

    internal const string IncludeExceptionDetailDisplayName = "Include Error Detail";

    /// <summary>
    /// When enabled, PostgreSQL error details are included on <see cref="PostgresException.Detail" /> and
    /// <see cref="PostgresNotice.Detail" />. These can contain sensitive data.
    /// </summary>
    [Category("Security")]
    [Description("When enabled, PostgreSQL error and notice details are included on PostgresException.Detail and PostgresNotice.Detail. These can contain sensitive data.")]
    [DisplayName(IncludeExceptionDetailDisplayName)]
    [NpgsqlConnectionStringProperty]
    public bool IncludeErrorDetail
    {
        get => _includeErrorDetail;
        set
        {
            _includeErrorDetail = value;
            SetValue(nameof(IncludeErrorDetail), value);
        }
    }
    bool _includeErrorDetail;

    /// <summary>
    /// Controls whether channel binding is required, disabled or preferred, depending on server support.
    /// </summary>
    [Category("Security")]
    [Description("Controls whether channel binding is required, disabled or preferred, depending on server support.")]
    [DisplayName("Channel Binding")]
    [DefaultValue(ChannelBinding.Prefer)]
    [NpgsqlConnectionStringProperty]
    public ChannelBinding ChannelBinding
    {
        get => _channelBinding;
        set
        {
            _channelBinding = value;
            SetValue(nameof(ChannelBinding), value);
        }
    }
    ChannelBinding _channelBinding;

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
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "MinPoolSize can't be negative");

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
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "MaxPoolSize can't be negative");

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
    [Description("The time to wait before closing unused connections in the pool if the count of all connections exceeds MinPoolSize.")]
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

    /// <summary>
    /// The total maximum lifetime of connections (in seconds). Connections which have exceeded this value will be
    /// destroyed instead of returned from the pool. This is useful in clustered configurations to force load
    /// balancing between a running server and a server just brought online. It can also be useful to prevent
    /// runaway memory growth of connections at the PostgreSQL server side, because in some cases very long lived
    /// connections slowly consume more and more memory over time.
    /// Defaults to 3600 seconds (1 hour).
    /// </summary>
    /// <value>The time (in seconds) to wait, or 0 to to make connections last indefinitely.</value>
    [Category("Pooling")]
    [Description("The total maximum lifetime of connections (in seconds).")]
    [DisplayName("Connection Lifetime")]
    [NpgsqlConnectionStringProperty("Load Balance Timeout")]
    [DefaultValue(3600)]
    public int ConnectionLifetime
    {
        get => _connectionLifetime;
        set
        {
            _connectionLifetime = value;
            SetValue(nameof(ConnectionLifetime), value);
        }
    }
    int _connectionLifetime;

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
    [DefaultValue(DefaultTimeout)]
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

    internal const int DefaultTimeout = 15;

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
    /// The time to wait (in milliseconds) while trying to read a response for a cancellation request for a timed out or cancelled query, before terminating the attempt and generating an error.
    /// Zero for infinity, -1 to skip the wait.
    /// Defaults to 2000 milliseconds.
    /// </summary>
    [Category("Timeouts")]
    [Description("After Command Timeout is reached (or user supplied cancellation token is cancelled) and command cancellation is attempted, Npgsql waits for this additional timeout (in milliseconds) before breaking the connection. Defaults to 2000, set to zero for infinity.")]
    [DisplayName("Cancellation Timeout")]
    [NpgsqlConnectionStringProperty]
    [DefaultValue(2000)]
    public int CancellationTimeout
    {
        get => _cancellationTimeout;
        set
        {
            if (value < -1)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(CancellationTimeout)} can't less than -1");

            _cancellationTimeout = value;
            SetValue(nameof(CancellationTimeout), value);
        }
    }
    int _cancellationTimeout;

    #endregion

    #region Properties - Failover and load balancing

    /// <summary>
    /// Determines the preferred PostgreSQL target server type.
    /// </summary>
    [Category("Failover and load balancing")]
    [Description("Determines the preferred PostgreSQL target server type.")]
    [DisplayName("Target Session Attributes")]
    [NpgsqlConnectionStringProperty]
    public string? TargetSessionAttributes
    {
        get => TargetSessionAttributesParsed switch
        {
            Npgsql.TargetSessionAttributes.Any           => "any",
            Npgsql.TargetSessionAttributes.Primary       => "primary",
            Npgsql.TargetSessionAttributes.Standby       => "standby",
            Npgsql.TargetSessionAttributes.PreferPrimary => "prefer-primary",
            Npgsql.TargetSessionAttributes.PreferStandby => "prefer-standby",
            Npgsql.TargetSessionAttributes.ReadWrite     => "read-write",
            Npgsql.TargetSessionAttributes.ReadOnly      => "read-only",
            null => null,

            _ => throw new ArgumentException($"Unhandled enum value '{TargetSessionAttributesParsed}'")
        };

        set
        {
            TargetSessionAttributesParsed = value is null ? null : ParseTargetSessionAttributes(value);
            SetValue(nameof(TargetSessionAttributes), value);
        }
    }

    internal TargetSessionAttributes? TargetSessionAttributesParsed { get; set; }

    internal static TargetSessionAttributes ParseTargetSessionAttributes(string s)
        => s switch
        {
            "any"            => Npgsql.TargetSessionAttributes.Any,
            "primary"        => Npgsql.TargetSessionAttributes.Primary,
            "standby"        => Npgsql.TargetSessionAttributes.Standby,
            "prefer-primary" => Npgsql.TargetSessionAttributes.PreferPrimary,
            "prefer-standby" => Npgsql.TargetSessionAttributes.PreferStandby,
            "read-write"     => Npgsql.TargetSessionAttributes.ReadWrite,
            "read-only"      => Npgsql.TargetSessionAttributes.ReadOnly,

            _ => throw new ArgumentException($"TargetSessionAttributes contains an invalid value '{s}'")
        };

    /// <summary>
    /// Enables balancing between multiple hosts by round-robin.
    /// </summary>
    [Category("Failover and load balancing")]
    [Description("Enables balancing between multiple hosts by round-robin.")]
    [DisplayName("Load Balance Hosts")]
    [NpgsqlConnectionStringProperty]
    public bool LoadBalanceHosts
    {
        get => _loadBalanceHosts;
        set
        {
            _loadBalanceHosts = value;
            SetValue(nameof(LoadBalanceHosts), value);
        }
    }
    bool _loadBalanceHosts;

    /// <summary>
    /// Controls for how long the host's cached state will be considered as valid.
    /// </summary>
    [Category("Failover and load balancing")]
    [Description("Controls for how long the host's cached state will be considered as valid.")]
    [DisplayName("Host Recheck Seconds")]
    [DefaultValue(10)]
    [NpgsqlConnectionStringProperty]
    public int HostRecheckSeconds
    {
        get => _hostRecheckSeconds;
        set
        {
            if (value < 0)
                throw new ArgumentException($"{HostRecheckSeconds} cannot be negative", nameof(HostRecheckSeconds));
            _hostRecheckSeconds = value;
            SetValue(nameof(HostRecheckSeconds), value);
        }
    }
    int _hostRecheckSeconds;

    #endregion Properties - Failover and load balancing

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
    /// Whether to use TCP keepalive with system defaults if overrides isn't specified.
    /// </summary>
    [Category("Advanced")]
    [Description("Whether to use TCP keepalive with system defaults if overrides isn't specified.")]
    [DisplayName("TCP Keepalive")]
    [NpgsqlConnectionStringProperty]
    public bool TcpKeepAlive
    {
        get => _tcpKeepAlive;
        set
        {
            _tcpKeepAlive = value;
            SetValue(nameof(TcpKeepAlive), value);
        }
    }
    bool _tcpKeepAlive;

    /// <summary>
    /// The number of seconds of connection inactivity before a TCP keepalive query is sent.
    /// Use of this option is discouraged, use <see cref="KeepAlive"/> instead if possible.
    /// Set to 0 (the default) to disable.
    /// </summary>
    [Category("Advanced")]
    [Description("The number of seconds of connection inactivity before a TCP keepalive query is sent.")]
    [DisplayName("TCP Keepalive Time")]
    [NpgsqlConnectionStringProperty]
    public int TcpKeepAliveTime
    {
        get => _tcpKeepAliveTime;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "TcpKeepAliveTime can't be negative");

            _tcpKeepAliveTime = value;
            SetValue(nameof(TcpKeepAliveTime), value);
        }
    }
    int _tcpKeepAliveTime;

    /// <summary>
    /// The interval, in seconds, between when successive keep-alive packets are sent if no acknowledgement is received.
    /// Defaults to the value of <see cref="TcpKeepAliveTime"/>. <see cref="TcpKeepAliveTime"/> must be non-zero as well.
    /// </summary>
    [Category("Advanced")]
    [Description("The interval, in seconds, between when successive keep-alive packets are sent if no acknowledgement is received.")]
    [DisplayName("TCP Keepalive Interval")]
    [NpgsqlConnectionStringProperty]
    public int TcpKeepAliveInterval
    {
        get => _tcpKeepAliveInterval;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "TcpKeepAliveInterval can't be negative");

            _tcpKeepAliveInterval = value;
            SetValue(nameof(TcpKeepAliveInterval), value);
        }
    }
    int _tcpKeepAliveInterval;

    /// <summary>
    /// Determines the size of the internal buffer Npgsql uses when reading. Increasing may improve performance if transferring large values from the database.
    /// </summary>
    [Category("Advanced")]
    [Description("Determines the size of the internal buffer Npgsql uses when reading. Increasing may improve performance if transferring large values from the database.")]
    [DisplayName("Read Buffer Size")]
    [NpgsqlConnectionStringProperty]
    [DefaultValue(NpgsqlReadBuffer.DefaultSize)]
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
    [DefaultValue(NpgsqlWriteBuffer.DefaultSize)]
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

    /// <summary>
    /// Set the replication mode of the connection
    /// </summary>
    /// <remarks>
    /// This property and its corresponding enum are intentionally kept internal as they
    /// should not be set by users or even be visible in their connection strings.
    /// Replication connections are a special kind of connection that is encapsulated in
    /// <see cref="PhysicalReplicationConnection"/>
    /// and <see cref="LogicalReplicationConnection"/>.
    /// </remarks>
    [NpgsqlConnectionStringProperty]
    [DisplayName("Replication Mode")]
    internal ReplicationMode ReplicationMode
    {
        get => _replicationMode;
        set
        {
            _replicationMode = value;
            SetValue(nameof(ReplicationMode), value);
        }
    }
    ReplicationMode _replicationMode;

    /// <summary>
    /// Set PostgreSQL configuration parameter default values for the connection.
    /// </summary>
    [Category("Advanced")]
    [Description("Set PostgreSQL configuration parameter default values for the connection.")]
    [DisplayName("Options")]
    [NpgsqlConnectionStringProperty]
    public string? Options
    {
        get => _options;
        set
        {
            _options = value;
            SetValue(nameof(Options), value);
        }
    }

    string? _options;

    /// <summary>
    /// Configure the way arrays of value types are returned when requested as object instances.
    /// </summary>
    [Category("Advanced")]
    [Description("Configure the way arrays of value types are returned when requested as object instances.")]
    [DisplayName("Array Nullability Mode")]
    [NpgsqlConnectionStringProperty]
    public ArrayNullabilityMode ArrayNullabilityMode
    {
        get => _arrayNullabilityMode;
        set
        {
            _arrayNullabilityMode = value;
            SetValue(nameof(ArrayNullabilityMode), value);
        }
    }

    ArrayNullabilityMode _arrayNullabilityMode;

    #endregion

    #region Multiplexing

    /// <summary>
    /// Enables multiplexing, which allows more efficient use of connections.
    /// </summary>
    [Category("Multiplexing")]
    [Description("Enables multiplexing, which allows more efficient use of connections.")]
    [DisplayName("Multiplexing")]
    [NpgsqlConnectionStringProperty]
    [DefaultValue(false)]
    public bool Multiplexing
    {
        get => _multiplexing;
        set
        {
            _multiplexing = value;
            SetValue(nameof(Multiplexing), value);
        }
    }
    bool _multiplexing;

    /// <summary>
    /// When multiplexing is enabled, determines the maximum number of outgoing bytes to buffer before
    /// flushing to the network.
    /// </summary>
    [Category("Multiplexing")]
    [Description("When multiplexing is enabled, determines the maximum number of outgoing bytes to buffer before " +
                 "flushing to the network.")]
    [DisplayName("Write Coalescing Buffer Threshold Bytes")]
    [NpgsqlConnectionStringProperty]
    [DefaultValue(1000)]
    public int WriteCoalescingBufferThresholdBytes
    {
        get => _writeCoalescingBufferThresholdBytes;
        set
        {
            _writeCoalescingBufferThresholdBytes = value;
            SetValue(nameof(WriteCoalescingBufferThresholdBytes), value);
        }
    }
    int _writeCoalescingBufferThresholdBytes;

    #endregion

    #region Properties - Obsolete

    /// <summary>
    /// Load table composite type definitions, and not just free-standing composite types.
    /// </summary>
    [Category("Advanced")]
    [Description("Load table composite type definitions, and not just free-standing composite types.")]
    [DisplayName("Load Table Composites")]
    [NpgsqlConnectionStringProperty]
    [Obsolete("Specifying type loading options through the connection string is obsolete, use the DataSource builder instead. See the 9.0 release notes for more information.")]
    public bool LoadTableComposites
    {
        get => _loadTableComposites;
        set
        {
            _loadTableComposites = value;
            SetValue(nameof(LoadTableComposites), value);
        }
    }
    bool _loadTableComposites;

    /// <summary>
    /// A compatibility mode for special PostgreSQL server types.
    /// </summary>
    [Category("Compatibility")]
    [Description("A compatibility mode for special PostgreSQL server types.")]
    [DisplayName("Server Compatibility Mode")]
    [NpgsqlConnectionStringProperty]
    [Obsolete("Specifying type loading options through the connection string is obsolete, use the DataSource builder instead. See the 9.0 release notes for more information.")]
    public ServerCompatibilityMode ServerCompatibilityMode
    {
        // Physical replication connections don't allow regular queries, so we can't load types from PG
        get => ReplicationMode is ReplicationMode.Physical ? ServerCompatibilityMode.NoTypeLoading : _serverCompatibilityMode;
        set
        {
            _serverCompatibilityMode = value;
            SetValue(nameof(ServerCompatibilityMode), value);
        }
    }
    ServerCompatibilityMode _serverCompatibilityMode;

    /// <summary>
    /// Whether to trust the server certificate without validating it.
    /// </summary>
    [Category("Security")]
    [Description("Whether to trust the server certificate without validating it.")]
    [DisplayName("Trust Server Certificate")]
    [Obsolete("The TrustServerCertificate parameter is no longer needed and does nothing.")]
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
    /// The time to wait (in seconds) while trying to execute a an internal command before terminating the attempt and generating an error.
    /// </summary>
    [Category("Obsolete")]
    [Description("The time to wait (in seconds) while trying to execute a an internal command before terminating the attempt and generating an error. -1 uses CommandTimeout, 0 means no timeout.")]
    [DisplayName("Internal Command Timeout")]
    [NpgsqlConnectionStringProperty]
    [DefaultValue(-1)]
    [Obsolete("The InternalCommandTimeout parameter is no longer needed and does nothing.")]
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

    #region Misc

    internal void PostProcessAndValidate()
    {
        if (string.IsNullOrWhiteSpace(Host))
            throw new ArgumentException("Host can't be null");
        if (Multiplexing && !Pooling)
            throw new ArgumentException("Pooling must be on to use multiplexing");
        if (SslNegotiation == SslNegotiation.Direct && SslMode is not SslMode.Require and not SslMode.VerifyCA and not SslMode.VerifyFull)
            throw new ArgumentException("SSL Mode has to be Require or higher to be used with direct SSL Negotiation");

        if (!Host.Contains(','))
        {
            if (TargetSessionAttributesParsed is not null &&
                TargetSessionAttributesParsed != Npgsql.TargetSessionAttributes.Any)
            {
                throw new NotSupportedException("Target Session Attributes other then Any is only supported with multiple hosts");
            }

            // Support single host:port format in Host
            if (!IsUnixSocket(Host, Port, out _) &&
                TrySplitHostPort(Host.AsSpan(), out var newHost, out var newPort))
            {
                Host = newHost;
                Port = newPort;
            }
        }
    }

    internal string ToStringWithoutPassword()
    {
        var clone = Clone();
        clone.Password = null;
        return clone.ToString();
    }

    internal string ConnectionStringForMultipleHosts
    {
        get
        {
            var clone = Clone();
            clone[nameof(TargetSessionAttributes)] = null;
            return clone.ConnectionString;
        }
    }

    internal NpgsqlConnectionStringBuilder Clone() => new(ConnectionString);

    internal static bool TrySplitHostPort(ReadOnlySpan<char> originalHost, [NotNullWhen(true)] out string? host, out int port)
    {
        var portSeparator = originalHost.LastIndexOf(':');
        if (portSeparator != -1)
        {
            var otherColon = originalHost.Slice(0, portSeparator).LastIndexOf(':');
            var ipv6End = originalHost.LastIndexOf(']');
            if (otherColon == -1 || portSeparator > ipv6End && otherColon < ipv6End)
            {
                port = int.Parse(originalHost.Slice(portSeparator + 1));
                host = originalHost.Slice(0, portSeparator).ToString();
                return true;
            }
        }

        port = -1;
        host = null;
        return false;
    }

    internal static bool IsUnixSocket(string host, int port, [NotNullWhen(true)] out string? socketPath, bool replaceForAbstract = true)
    {
        socketPath = null;
        if (string.IsNullOrEmpty(host))
            return false;

        var isPathRooted = Path.IsPathRooted(host);

        if (host[0] == '@')
        {
            if (replaceForAbstract)
                host = $"\0{host.Substring(1)}";
            isPathRooted = true;
        }

        if (isPathRooted)
        {
            socketPath = Path.Combine(host, $".s.PGSQL.{port}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj)
        => obj is NpgsqlConnectionStringBuilder o && EquivalentTo(o);

    /// <summary>
    /// Hash function.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Host?.GetHashCode() ?? 0;

    #endregion

    #region IDictionary<string, object>

    /// <summary>
    /// Gets an <see cref="ICollection" /> containing the keys of the <see cref="NpgsqlConnectionStringBuilder"/>.
    /// </summary>
    public new ICollection<string> Keys
    {
        get
        {
            var result = new string[base.Keys.Count];
            var i = 0;
            foreach (var key in base.Keys)
                result[i++] = (string)key;
            return result;
        }
    }

    /// <summary>
    /// Gets an <see cref="ICollection" /> containing the values in the <see cref="NpgsqlConnectionStringBuilder"/>.
    /// </summary>
    public new ICollection<object?> Values
    {
        get
        {
            var result = new object?[base.Keys.Count];
            var i = 0;
            foreach (var key in base.Values)
                result[i++] = (object?)key;
            return result;
        }
    }

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
    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        foreach (var kv in this)
            array[arrayIndex++] = kv;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="NpgsqlConnectionStringBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        foreach (var k in Keys)
            yield return new KeyValuePair<string, object?>(k, this[k]);
    }

    #endregion IDictionary<string, object>

    #region ICustomTypeDescriptor

    /// <inheritdoc />
    [RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
    protected override void GetProperties(Hashtable propertyDescriptors)
    {
        // Tweak which properties are exposed via TypeDescriptor. This affects the VS DDEX
        // provider, for example.
        base.GetProperties(propertyDescriptors);

        var toRemove = new List<PropertyDescriptor>();
        foreach (var value in propertyDescriptors.Values)
        {
            var d = (PropertyDescriptor)value;
            foreach (var attribute in d.Attributes)
                if (attribute is NpgsqlConnectionStringPropertyAttribute or ObsoleteAttribute)
                    toRemove.Add(d);
        }

        foreach (var o in toRemove)
            propertyDescriptors.Remove(o.DisplayName);
    }

    #endregion
}

#region Attributes

/// <summary>
/// Marks on <see cref="NpgsqlConnectionStringBuilder"/> which participate in the connection
/// string. Optionally holds a set of synonyms for the property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
sealed class NpgsqlConnectionStringPropertyAttribute : Attribute
{
    /// <summary>
    /// Holds a list of synonyms for the property.
    /// </summary>
    public string[] Synonyms { get; }

    /// <summary>
    /// Creates a <see cref="NpgsqlConnectionStringPropertyAttribute"/>.
    /// </summary>
    public NpgsqlConnectionStringPropertyAttribute()
        => Synonyms = [];

    /// <summary>
    /// Creates a <see cref="NpgsqlConnectionStringPropertyAttribute"/>.
    /// </summary>
    public NpgsqlConnectionStringPropertyAttribute(params string[] synonyms)
        => Synonyms = synonyms;
}

#endregion

#region Enums

/// <summary>
/// Specifies how to manage SSL.
/// </summary>
public enum SslMode
{
    /// <summary>
    /// SSL is disabled. If the server requires SSL, the connection will fail.
    /// </summary>
    Disable,
    /// <summary>
    /// Prefer non-SSL connections if the server allows them, but allow SSL connections.
    /// </summary>
    Allow,
    /// <summary>
    /// Prefer SSL connections if the server allows them, but allow connections without SSL.
    /// </summary>
    Prefer,
    /// <summary>
    /// Fail the connection if the server doesn't support SSL.
    /// </summary>
    Require,
    /// <summary>
    /// Fail the connection if the server doesn't support SSL. Also verifies server certificate.
    /// </summary>
    VerifyCA,
    /// <summary>
    /// Fail the connection if the server doesn't support SSL. Also verifies server certificate with host's name.
    /// </summary>
    VerifyFull
}

/// <summary>
/// Specifies how to initialize SSL session.
/// </summary>
public enum SslNegotiation
{
    /// <summary>
    /// Perform PostgreSQL protocol negotiation.
    /// </summary>
    Postgres,
    /// <summary>
    /// Start SSL handshake directly after establishing the TCP/IP connection.
    /// </summary>
    Direct
}

/// <summary>
/// Specifies how to manage channel binding.
/// </summary>
public enum ChannelBinding
{
    /// <summary>
    /// Channel binding is disabled. If the server requires channel binding, the connection will fail.
    /// </summary>
    Disable,
    /// <summary>
    /// Prefer channel binding if the server allows it, but connect without it if not.
    /// </summary>
    Prefer,
    /// <summary>
    /// Fail the connection if the server doesn't support channel binding.
    /// </summary>
    Require
}

/// <summary>
/// Specifies how the mapping of arrays of
/// <a href="https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-types">value types</a>
/// behaves with respect to nullability when they are requested via an API returning an <see cref="object"/>.
/// </summary>
public enum ArrayNullabilityMode
{
    /// <summary>
    /// Arrays of value types are always returned as non-nullable arrays (e.g. <c>int[]</c>).
    /// If the PostgreSQL array contains a NULL value, an exception is thrown. This is the default mode.
    /// </summary>
    Never,
    /// <summary>
    /// Arrays of value types are always returned as nullable arrays (e.g. <c>int?[]</c>).
    /// </summary>
    Always,
    /// <summary>
    /// The type of array that gets returned is determined at runtime.
    /// Arrays of value types are returned as non-nullable arrays (e.g. <c>int[]</c>)
    /// if the actual instance that gets returned doesn't contain null values
    /// and as nullable arrays (e.g. <c>int?[]</c>) if it does.
    /// </summary>
    /// <remarks>When using this setting, make sure that your code is prepared to the fact
    /// that the actual type of array instances returned from APIs like <see cref="NpgsqlDataReader.GetValue"/>
    /// may change on a row by row base.</remarks>
    PerInstance,
}

/// <summary>
/// Specifies whether the connection shall be initialized as a physical or
/// logical replication connection
/// </summary>
/// <remarks>
/// This enum and its corresponding property are intentionally kept internal as they
/// should not be set by users or even be visible in their connection strings.
/// Replication connections are a special kind of connection that is encapsulated in
/// <see cref="PhysicalReplicationConnection"/>
/// and <see cref="LogicalReplicationConnection"/>.
/// </remarks>
enum ReplicationMode
{
    /// <summary>
    /// Replication disabled. This is the default
    /// </summary>
    Off,
    /// <summary>
    /// Physical replication enabled
    /// </summary>
    Physical,
    /// <summary>
    /// Logical replication enabled
    /// </summary>
    Logical
}

#endregion
