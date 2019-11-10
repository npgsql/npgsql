using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Npgsql
{
    /// <summary>
    /// Represents a connection URI and easy access to its contents.
    /// </summary>
    internal class NpgsqlConnectionUri
    {
        #region Fields

        /// <summary>
        /// Maps URI parameter name (e.g. connect_timeout), which is set by the property's [NpgsqlConnectionURIParameter], 
        /// to their property info (e.g. Timeout)
        /// </summary>
        static readonly Dictionary<string, PropertyInfo?> PropertiesByParameter;

        /// <summary>
        /// Maps CLR property names (e.g. CommandTimeout) to their canonical keyword name, which is the
        /// property's [DisplayName] (e.g. Command Timeout)
        /// </summary>
        static readonly Dictionary<string, string> PropertyNameToCanonicalKeyword;

        #endregion

        #region Properties

        /// <summary>
        /// The hostname or IP address of the PostgreSQL server to connect to.
        /// </summary>
        public string? Scheme { get; protected set; }

        /// <summary>
        /// The hostname or IP address of the PostgreSQL server to connect to.
        /// </summary>
        public string? Host { get; protected set; }

        /// <summary>
        /// The TCP/IP port of the PostgreSQL server.
        /// </summary>
        public int? Port { get; protected set; }

        ///<summary>
        /// The PostgreSQL database to connect to.
        /// </summary>
        public string? Database { get; protected set; }

        /// <summary>
        /// The user name to connect with.
        /// </summary>
        public string? Username { get; protected set; }

        /// <summary>
        /// The password to connect with.
        /// </summary>
        public string? Password { get; protected set; }

        public IReadOnlyList<KeyValuePair<string, string?>> Parameters => _parameters;
        List<KeyValuePair<string, string?>> _parameters;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the NpgsqlConnectionUri class.
        /// </summary>
        /// <param name="connectionUri">The connection URI used to open the PostgreSQL database.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public NpgsqlConnectionUri(string connectionUri)
        {
            if (connectionUri == null)
                throw new ArgumentNullException(nameof(connectionUri));

            var preprocessedUri = ReplaceHostComponent(connectionUri, out var host);
            var uri = new Uri(preprocessedUri);

            Scheme = uri.Scheme;
            switch (Scheme.ToLower())
            {
                case "postgres":
                case "postgresql":
                    break;
                default:
                    throw new UriFormatException("Invalid Connection URI: The provided connection URI had unsupported scheme.");
            }

            var userInfo = uri.UserInfo.Split(new[] { ':' }, 2);
            if (userInfo[0].Length > 0)
                Username = Uri.UnescapeDataString(userInfo[0]);
            if (userInfo.Length > 1 && userInfo[1].Length > 0)
                Password = Uri.UnescapeDataString(userInfo[1]);

            if (host.Length > 0)
            {
                if (host.Contains(','))
                    throw new NotSupportedException("Specifying multiple hosts in a connection URI is not supported.");
                var match = Regex.Match(host, @"^\[([a-fA-F0-9:]+)\]$");
                if (match.Length > 0)
                    Host = match.Groups[1].Value;
                else if (host.Length > 0)
                    Host = Uri.UnescapeDataString(host);
            }

            if (uri.Port >= 0)
                Port = uri.Port;

            var path = new string(uri.AbsolutePath.Skip(1).ToArray());
            if (path.Length > 0)
                Database = Uri.UnescapeDataString(path);

            _parameters = uri.Query
                .Split(new[] { '?', '&' })
                .Where(p => p.Length > 0)
                .Select(p =>
                {
                    var param = p.Split(new[] { '=' }, 2);
                    if (param.Length < 2)
                        throw new UriFormatException($"Invalid Connection URI: URI parameter {p} was not followed by value.");
                    return new KeyValuePair<string, string?>(Uri.UnescapeDataString(param[0]), p.Length < 2 || param[1].Length == 0 ? null : Uri.UnescapeDataString(param[1]));
                })
                .ToList();
        }

        /// <summary>
        /// Extract the host component from a URI and replace it with a dummy host name.
        /// </summary>
        /// <param name="uri">Connection URI</param>
        /// <param name="host"></param>
        /// <returns>The connection URI which host component is replaced with "x".</returns>
        /// <remarks>
        /// The forms of URI listed below are all valid for psql, but a System.UriFormatException is thrown in the parse of System.Uri class.
        /// This method separates such part from URI and allows to handle it outside the System.Uri parse.
        /// <list type="bullet">
        /// <item><description>The authority part exists but the host component is empty. (e.g. postgresql://user@/dbname)</description></item>
        /// <item><description>A percent-encoded path is specified in the host component, which is the case of a non-standard Unix-domain socket directory. (e.g. postgresql://%2Fvar%2Flib%2Fpostgresql/dbname)</description></item>
        /// <item><description>Multiple sets of host and port are specified in the authrity part. (e.g. postgresql://host1,host2/dbname)</description></item>
        /// </list>
        /// </remarks>
        static string ReplaceHostComponent(string uri, out string host)
        {
            var authorityStart = uri.IndexOf("://", StringComparison.InvariantCulture);
            if (authorityStart < 0)
            {
                host = "";
                return uri;
            }
            authorityStart += 3;

            var queryStart = uri.IndexOf('?', authorityStart);
            var userinfoEnd = uri.IndexOf('@', authorityStart) + 1;
            if (queryStart >= 0 && userinfoEnd > queryStart)
                userinfoEnd = -1;
            var pathStart = uri.IndexOf('/', authorityStart);
            var fragmentStart = uri.IndexOf('#');

            var hostStart = Math.Max(authorityStart, userinfoEnd);
            var hostEnd = uri.Length;
            if (0 < hostStart && hostStart < uri.Length && uri[hostStart] == '[')
                hostEnd = uri.IndexOf(']', hostStart + 1) + 1;
            else
            {
                var portStart = uri.IndexOf(':', hostStart);
                var hostEndCandidates = new[] { portStart, pathStart, queryStart, fragmentStart }
                    .Where(i => i >= 0);
                if (hostEndCandidates.Count() > 0)
                    hostEnd = hostEndCandidates.Min();
            }
            host = uri.Substring(hostStart, hostEnd - hostStart);
            return uri.Substring(0, hostStart) + "x" + uri.Substring(hostEnd, uri.Length - hostEnd);
        }

        #endregion

        #region Static initialization

        static NpgsqlConnectionUri()
        {
            var properties = typeof(NpgsqlConnectionStringBuilder)
                .GetProperties()
                .Select(p => new
                {
                    Property = p,
                    UriParameter = p.GetCustomAttribute<NpgsqlConnectionURIParameterAttribute>(),
                    DisplayName = p.GetCustomAttribute<DisplayNameAttribute>()
                })
                .Where(p => p.UriParameter != null)
                .ToArray();

            PropertiesByParameter = (
                from p in properties
                from k in p.UriParameter.Keywords
                select new { Keyword = k, p.Property }
            ).ToDictionary(p => p.Keyword, p => (PropertyInfo?)p.Property);

            PropertyNameToCanonicalKeyword = properties.ToDictionary(
                p => p.Property.Name,
                p => p.DisplayName!.DisplayName
            );
        }

        #endregion

        /// <summary>
        /// Returns an instance of <see cref="NpgsqlConnectionStringBuilder"/> that represents this connection URI.
        /// </summary>
        public NpgsqlConnectionStringBuilder ToSettings()
        {
            var components = new Dictionary<string, string?>
            {
                { "host", Host },
                { "port", Port?.ToString() },
                { "dbname", Database },
                { "user", Username },
                { "password", Password },
                { "keepalives", "1" },
                { "sslmode", "prefer" },
            }
            .ToList();
            _parameters.ForEach(p => components.Add(new KeyValuePair<string, string?>(p.Key, p.Value)));

            var respectedComponents = (
                from param in components
                from prop in PropertiesByParameter.Where(p => param.Key == p.Key)
                    .DefaultIfEmpty(new KeyValuePair<string, PropertyInfo?>(param.Key, null))
                group new { param, prop } by (prop.Value?.Name ?? param.Key) into g
                select g.Last()
            ).ToDictionary(p => p.param.Key, p => new { p.param.Value, Property = p.prop.Value });

            if (respectedComponents.ContainsKey("hostaddr"))
                respectedComponents.Remove("host");

            if (respectedComponents.ContainsKey("fallback_application_name")
                && respectedComponents.ContainsKey("application_name"))
                respectedComponents.Remove("fallback_application_name");

            var settings = new NpgsqlConnectionStringBuilder();
            foreach (var component in respectedComponents)
            {
                if (component.Value.Property == null)
                    throw new NotSupportedException("URI parameter not supported: " + component.Key);
                var canonicalKeyword = PropertyNameToCanonicalKeyword[component.Value.Property.Name];
                settings[canonicalKeyword] = ConvertParameter(component.Key, component.Value.Value, component.Value.Property);
            }

            return settings;
        }

        object? ConvertParameter(string key, string? value, PropertyInfo property)
        {
            if (property.Name == "Host")
            {
                if (string.IsNullOrEmpty(value))
                    return "localhost";
                if (value!.Split(',').Count() > 1)
                    throw new NotSupportedException("The host parameter does not support multiple hosts in Npgsql.");
            }
            else if (property.Name == "Port")
            {
                if (string.IsNullOrEmpty(value))
                    return null;
                if (value!.Split(',').Count() > 1)
                    throw new NotSupportedException("The port parameter does not support multiple hosts in Npgsql.");
                if (int.TryParse(value, out var port))
                    return port;
                throw new UriFormatException("Invalid Connection URI: The format of port number in the connection URI parameter was invalid.");
            }
            else if (property.Name == "SslMode")
            {
                switch (value?.ToLower())
                {
                    case "true":
                    case "1":
                        return SslMode.Require;
                    case "false":
                    case "0":
                        return SslMode.Disable;
                }
            }
            else if (property.Name == "TcpKeepAliveTime" || property.Name == "TcpKeepAliveInterval")
            {
                return value + "000";
            }
            else if (property.PropertyType == typeof(bool))
            {
                if (int.TryParse(value, out var n))
                    switch (n)
                    {
                        case 0:
                            return false;
                        case 1:
                            return true;
                    }

                throw new UriFormatException("Invalid Connection URI: Couldn't set parameter " + key);
            }

            return value;
        }
    }
}
