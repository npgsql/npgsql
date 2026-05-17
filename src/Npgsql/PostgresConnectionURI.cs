using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Npgsql;

/// <summary>
/// Represents options for parsing and validating PostgreSQL connection URIs.
/// </summary>
public sealed record PostgresConnectionURIParseOptions
{
    /// <summary>
    /// Gets the default set of options for parsing PostgreSQL connection URIs.
    /// </summary>
    internal static PostgresConnectionURIParseOptions s_default { get; }
        = new PostgresConnectionURIParseOptions();

    /// <summary>
    /// Throws an exception when it encounters unsupported keys in the connection URI.
    /// </summary>
    /// <remarks> Default is <see langword="true"/>.</remarks>
    public bool ThrowsOnUnsupportedKeys { get; init; } = true;

    /// <summary>
    /// Throws an exception when it encounters parameter overwrite in the connection URI.
    /// </summary>
    /// <remarks> Default is <see langword="true"/>.</remarks>
    public bool ThrowsOnParameterOverwrite { get; init; } = true;

    /// <summary>
    /// Throws an exception when it encounters multple ports in the connection URI.
    /// </summary>
    /// <remarks> Default is <see langword="true"/>.</remarks>
    public bool ThrowsOnMultiplePorts { get; init; } = true;
}

/// <summary>
/// Handles PostgreSQL connection URIs. A URI-to-NpgsqlConnectionStringBuilder converter.
/// </summary>
public sealed record PostgresConnectionURI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresConnectionURI"/>
    /// class by parsing the provided PostgreSQL connection URI string.
    /// </summary>
    /// <param name="uri">A PostgreSQL connection URI string.</param>
    /// <param name="options">Optional configuration options for parsing the
    /// connection URI. If omitted, default options will be used.</param>
    public PostgresConnectionURI(string uri, PostgresConnectionURIParseOptions? options = null)
    {
        _originalURI = uri;

        ParseOptions = options ?? PostgresConnectionURIParseOptions.s_default;

        var parser = new PostgresConnectionURIParser(uri);

        Host = parser.Host;
        Port = parser.Port;
        Database = parser.Database;
        Username = parser.Username;
        Password = parser.Password;

        BindParameters(parser.Parameters);
    }

    #region Nested Types

    /// <summary>
    /// Holds URI connection property and its ConnectionParameterAttribute.
    /// </summary>
    /// <param name="Instance">The instance of URI connection property.</param>
    /// <param name="Attribute">The instance of ConnectionParameterAttribute.</param>
    record ConnectionParameterProperty(PropertyInfo Instance, ConnectionParameterAttribute Attribute);

    /// <summary>
    /// The list of <see cref="ConnectionParameterProperty"/> in the <see cref="PostgresConnectionURI"/> class.
    /// </summary>
    static ConnectionParameterProperty[] s_connectionParameters { get; } = [
        ..typeof(PostgresConnectionURI).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
        .Select(prop => Tuple.Create(prop, prop.GetCustomAttribute<ConnectionParameterAttribute>()))
        .Where(tuple => tuple.Item2 is not null)
        .Select(tuple => new ConnectionParameterProperty(tuple.Item1, tuple.Item2!))
    ];

    #endregion

    #region Utilities

    internal static IEnumerable<PropertyInfo> EnumerateConnectionParameters(
        Predicate<ConnectionParameterAttribute> predicate)
    {
        foreach (var prop in s_connectionParameters)
        {
            if (predicate(prop.Attribute))
            {
                yield return prop.Instance;
            }
        }
    }

    void BindParameters(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var unsupportedKeys = new List<string>();

        foreach (var entry in parameters)
        {
            var prop = EnumerateConnectionParameters(attr
                => attr.ParameterKeyWords.Contains(entry.Key)).FirstOrDefault();
            if (prop is null)
            {
                unsupportedKeys.Add(entry.Key);
                continue;
            }

            if (ParseOptions.ThrowsOnParameterOverwrite && prop.GetValue(this) is not null)
            {
                throw new ArgumentException($"Parameter '{entry.Key}' overwrites "
                    + "a previously specified value in the connection URI.");
            }

            BindEntry(prop, entry);
        }

        if (ParseOptions.ThrowsOnUnsupportedKeys && unsupportedKeys.Count > 0)
        {
            var detail = string.Join(", ", unsupportedKeys);
            throw new ArgumentException($"Unsupported connection URI parameters: {detail}");
        }
    }

    void BindEntry(PropertyInfo prop, KeyValuePair<string, string> entry)
    {
        // Handle LoadBalanceHosts separately since it requires special handling of its value.
        if (prop.Name == nameof(LoadBalanceHosts))
        {
            switch (entry.Value)
            {
            case "disable":
                LoadBalanceHosts = false;
                return;

            case "random":
                LoadBalanceHosts = true;
                return;

            default:
                throw new ArgumentException($"Invalid value for {entry.Key}: {entry.Value}. "
                    + "Expected 'disable' or 'random'.");
            }
        }

        if (prop.Name == nameof(Port) && entry.Value.Contains(','))
        {
            if (ParseOptions.ThrowsOnMultiplePorts)
                throw new ArgumentException($"Multiple ports are not supported: {entry.Value}.");

            // To avoid type conversion error, skip binding port if multiple ports are specified.
            return;
        }

        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (type == typeof(string))
        {
            prop.SetValue(this, entry.Value);
            return;
        }

        if (type == typeof(int))
        {
            if (int.TryParse(entry.Value, out var value))
            {
                prop.SetValue(this, value);
            }
            else
            {
                throw new ArgumentException($"Invalid value for {entry.Key}: {entry.Value}. Expected an integer.");
            }
            return;
        }

        if (type == typeof(bool))
        {
            var truePattern = "^(1|t(rue)?|y(es)?|on)$";
            var falsePattern = "^(0|f(alse)?|n(o)?|of(f)?)$";

            if (Regex.IsMatch(entry.Value, truePattern, RegexOptions.IgnoreCase))
            {
                prop.SetValue(this, true);
            }
            else if (Regex.IsMatch(entry.Value, falsePattern, RegexOptions.IgnoreCase))
            {
                prop.SetValue(this, false);
            }
            else
            {
                throw new ArgumentException($"Invalid value for {entry.Key}: {entry.Value}. "
                    + "Expected on, off, true, false, yes, no, 1, 0 (all case-insensitive) or "
                    + "any unambiguous prefix of one of these.");
            }
            return;
        }

        if (type.IsEnum)
        {
            // PostgreSQL allows hyphens in enum values, but C# does not. Remove hyphens before parsing.
            if (Enum.TryParse(type, entry.Value.Replace("-", string.Empty), ignoreCase: true, out var parsed))
            {
                prop.SetValue(this, parsed);
            }
            else
            {
                var enums = string.Join(", ", type.GetEnumNames());
                throw new ArgumentException($"Invalid value for {entry.Key}: {entry.Value}. Expected one of {enums}.");
            }
            return;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a string that represents the original URI.
    /// </summary>
    public override string ToString() => _originalURI;
    readonly string _originalURI;

    /// <summary>
    /// Creates a new <see cref="NpgsqlConnectionStringBuilder"/> instance equivalent to
    /// the connection URI string.
    /// </summary>
    public NpgsqlConnectionStringBuilder ToConnectionStringBuilder()
    {
        var builder = new NpgsqlConnectionStringBuilder();

        // Copy non-null properties to the connection string builder.
        foreach (var prop in EnumerateConnectionParameters(attr => attr.Bindable))
        {
            var value = prop.GetValue(this);
            if (value is null) continue;

            builder[prop.Name] = value;
        }

        return builder;
    }

    #endregion

    #region Properties

    /// <summary>
    /// PostgreSQL connection URI parse options.
    /// </summary>
    public PostgresConnectionURIParseOptions ParseOptions { get; }

    #endregion

    #region Connection URI Properties

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Host"/>.
    /// </summary>
    [ConnectionParameter("host", "hostaddr")]
    internal string? Host { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Port"/>.
    /// </summary>
    [ConnectionParameter("port")]
    internal int? Port { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Database"/>.
    /// </summary>
    [ConnectionParameter("dbname")]
    internal string? Database { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Username"/>.
    /// </summary>
    [ConnectionParameter("user")]
    internal string? Username { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Password"/>.
    /// </summary>
    [ConnectionParameter("password")]
    internal string? Password { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Passfile"/>.
    /// </summary>
    [ConnectionParameter("passfile")]
    internal string? Passfile { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.ApplicationName"/>.
    /// </summary>
    [ConnectionParameter("application_name")]
    internal string? ApplicationName { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.ClientEncoding"/>.
    /// </summary>
    [ConnectionParameter("client_encoding")]
    internal string? ClientEncoding { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.SslMode"/>.
    /// </summary>
    [ConnectionParameter("sslmode")]
    internal SslMode? SslMode { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.SslNegotiation"/>.
    /// </summary>
    [ConnectionParameter("sslnegotiation")]
    internal SslNegotiation? SslNegotiation { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.GssEncryptionMode"/>.
    /// </summary>
    [ConnectionParameter("gssencmode")]
    internal GssEncryptionMode? GssEncryptionMode { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.SslCertificate"/>.
    /// </summary>
    [ConnectionParameter("sslcert")]
    internal string? SslCertificate { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.SslKey"/>.
    /// </summary>
    [ConnectionParameter("sslkey")]
    internal string? SslKey { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.SslPassword"/>.
    /// </summary>
    [ConnectionParameter("sslpassword")]
    internal string? SslPassword { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.RootCertificate"/>.
    /// </summary>
    [ConnectionParameter("sslrootcert")]
    internal string? RootCertificate { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.KerberosServiceName"/>.
    /// </summary>
    [ConnectionParameter("krbsrvname")]
    internal string? KerberosServiceName { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.ChannelBinding"/>.
    /// </summary>
    [ConnectionParameter("channel_binding")]
    internal ChannelBinding? ChannelBinding { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.RequireAuth"/>.
    /// </summary>
    [ConnectionParameter("require_auth")]
    internal string? RequireAuth
    {
        get => _require_auth;
        init
        {
            if (string.IsNullOrEmpty(value)) return;

            var prefix = value.Contains('!') ? "!" : string.Empty;
            var result = new List<string>();

            foreach (var item in value.Split(','))
            {
                var trimmed = item.Trim('!', ' ').Replace("-", string.Empty);

                if (string.IsNullOrEmpty(trimmed)) continue;

                if (Enum.TryParse<RequireAuthMode>(trimmed, ignoreCase: true, out var mode))
                {
                    result.Add(prefix + mode.ToString());
                }
                else
                {
                    throw new ArgumentException($"Invalid value for require_auth: {value}.");
                }
            }

            _require_auth = string.Join(",", result);
        }
    }
    string? _require_auth;

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Timeout"/>.
    /// </summary>
    [ConnectionParameter("connect_timeout")]
    internal int? Timeout { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.TargetSessionAttributes"/>.
    /// </summary>
    [ConnectionParameter("target_session_attrs")]
    internal string? TargetSessionAttributes { get; init; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.LoadBalanceHosts"/>.
    /// </summary>
    /// <remarks>Requires private setter for internal handling.
    /// See also <see cref="BindEntry"/>.</remarks>
    [ConnectionParameter("load_balance_hosts")]
    internal bool? LoadBalanceHosts { get; private set; }

    /// <summary>
    /// Conforms to <see cref="NpgsqlConnectionStringBuilder.Options"/>.
    /// </summary>
    [ConnectionParameter("options")]
    internal string? Options { get; init; }

    #endregion
}

#region Internal Types

/// <summary>
/// Marks on connection parameters in the <see cref="PostgresConnectionURI"/> class.
/// </summary>
/// <remarks>
/// Creates a <see cref="ConnectionParameterAttribute"/>.
/// </remarks>
/// <param name="bindable">Indicates whether the property is bindable to NpgsqlConnectionStringBuilder.</param>
/// <param name="keywords">A set of synonyms for the connection parameter.</param>
[AttributeUsage(AttributeTargets.Property)]
class ConnectionParameterAttribute(bool bindable, params string[] keywords) : Attribute
{
    #region Properties

    /// <summary>
    /// A set of synonyms for the connection parameter.
    /// </summary>
    public string[] ParameterKeyWords { get; } = keywords;

    /// <summary>
    /// Indicate whether the property is bindable to NpgsqlConnectionStringBuilder.
    /// </summary>
    /// <remarks>Default is <see langword="true"/>.</remarks>
    public bool Bindable { get; } = bindable;

    #endregion

    /// <summary>
    /// Creates a <see cref="ConnectionParameterAttribute"/>.
    /// </summary>
    /// <param name="keywords">A set of synonyms for the connection parameter.</param>
    public ConnectionParameterAttribute(params string[] keywords) : this(bindable: true, keywords) { }
}

/// <summary>
/// Internal class responsible for parsing PostgreSQL connection URI
/// strings and extracting connection parameters.
/// </summary>
internal sealed partial record PostgresConnectionURIParser
{
    #region Connection URI Specification

    readonly string? _userspec;
    readonly string? _hostspec;
    readonly string? _dbname;
    readonly string? _paramspec;

    [GeneratedRegex("^postgres(ql)?://"
        + $"((?<{nameof(_userspec)}>[^@]+)@)?"
        + $"(?<{nameof(_hostspec)}>[^/]+)?"
        + $"(/(?<{nameof(_dbname)}>[^?]+))?"
        + $"([?](?<{nameof(_paramspec)}>.+))?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex CreatePostgresUriRegex();

    static readonly Regex s_uriRegex = CreatePostgresUriRegex();

    #endregion

    /// <summary>
    /// Initializes a new instance of the PostgresConnectionURIParser
    /// class by parsing the specified PostgreSQL connection URI.
    /// </summary>
    /// <param name="uri">The PostgreSQL connection URI to parse.
    /// Must be in the format 'postgresql://[user@]host/[database]?[parameters]'.</param>
    /// <exception cref="ArgumentException">Thrown if the specified URI is not
    /// a valid PostgreSQL connection URI.</exception>
    internal PostgresConnectionURIParser(string uri)
    {
        var result = s_uriRegex.Match(uri);

        if (!result.Success) throw new ArgumentException($"Invalid connection URI: {uri}");

        _userspec = GetGroupValue(result, nameof(_userspec));
        _hostspec = GetGroupValue(result, nameof(_hostspec));
        _dbname = GetGroupValue(result, nameof(_dbname));
        _paramspec = GetGroupValue(result, nameof(_paramspec));

        if (_userspec is not null)
        {
            // with password
            if (_userspec.Contains(':'))
            {
                var parts = _userspec.Split(':');

                Username = Uri.UnescapeDataString(parts.First());
                Password = Uri.UnescapeDataString(parts.Last());
            }
            else
            {
                Username = Uri.UnescapeDataString(_userspec);
            }
        }

        if (_hostspec is not null)
        {
            // multiple hosts
            if (_hostspec.Contains(','))
            {
                Host = _hostspec;
            }
            // with port
            else if (_hostspec.Contains(':'))
            {
                var parts = _hostspec.Split(':');

                Host = parts.First();

                if (int.TryParse(parts.Last(), out var port))
                    Port = port;
                else
                    throw new ArgumentException($"Invalid port number: {parts.Last()}");
            }
            else
            {
                Host = _hostspec;
            }
        }
    }

    static string? GetGroupValue(Match match, string groupName)
    {
        var group = match.Groups[groupName];

        return group.Success ? group.Value : null;
    }

    #region Internal Properties

    internal string? Username { get; }
    internal string? Password { get; }
    internal string? Host { get; }
    internal int? Port { get; }
    internal string? Database => _dbname;

    internal IEnumerable<KeyValuePair<string, string>> Parameters
    {
        get
        {
            var pairs = _paramspec?.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs ?? [])
            {
                var kvp = pair.Split('=');
                if (kvp.Length != 2) continue;

                var key = kvp.First();
                var value = Uri.UnescapeDataString(kvp.Last());

                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }

    #endregion
}

#endregion
