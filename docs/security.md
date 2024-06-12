# Security and Encryption

## Password management

The simplest way to log into PostgreSQL is by specifying a `Username` and a `Password` in your connection string. Depending on how your PostgreSQL is configured (in the `pg_hba.conf` file), Npgsql will send the password in MD5 or in cleartext (not recommended).

If a `Password` is not specified and your PostgreSQL is configured to request a password, Npgsql will look for a [standard PostgreSQL password file](https://www.postgresql.org/docs/current/static/libpq-pgpass.html). If you specify the `Passfile` connection string parameter, the file it specifies will be used. If that parameter isn't defined, Npgsql will look under the path taken from `PGPASSFILE` environment variable. If the environment variable isn't defined, Npgsql will fall back to the system-dependent default directory which is `$HOME/.pgpass` for Unix and `%APPDATA%\postgresql\pgpass.conf` for Windows.

### Auth token rotation and dynamic password

In some cloud scenarios, logging into PostgreSQL is done with an auth token that is rotated every time interval (e.g. one hour). Starting with version 7.0, Npgsql has a built-in periodic password provider mechanism, which allows refreshing the password with zero effort:

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.UsePeriodicPasswordProvider(
    (settings, cancellationToken) =>  /* async code to fetch the new access token */,
    TimeSpan.FromMinutes(55), // Interval for refreshing the token
    TimeSpan.FromSeconds(5)); // Interval for retrying after a refresh failure
await using var dataSource = dataSourceBuilder.Build();
```

This API allows you to provide a minimal async code fragment for fetching the latest auth token, and have Npgsql take care of running it for you as needed.

If, instead, you prefer to manage this yourself, you can simply inject a new password at any time into a working data source:

```csharp
dataSource.Password = <new password>;
```

Any physical connection that's opened after this point will use the newly-injected password.

## Encryption (SSL/TLS)

By default PostgreSQL connections are unencrypted, but you can turn on SSL/TLS encryption if you wish. First, you have to set up your PostgreSQL to receive SSL/TLS connections [as described here](http://www.postgresql.org/docs/current/static/ssl-tcp.html). Once that's done, specify `SSL Mode` in your connection string as detailed below.

### [Version 6.0+](#tab/tabid-1)

Starting with 6.0, the following `SSL Mode` values are supported (see the [PostgreSQL docs](https://www.postgresql.org/docs/current/libpq-ssl.html#LIBPQ-SSL-SSLMODE-STATEMENTS) for more details):

SSL Mode            | Eavesdropping protection | Man-in-the-middle protection | Statement
------------------- | ------------------------ | ---------------------------- | ---------
Disable             | No                       | No                           | I don't care about security, and I don't want to pay the overhead of encryption.
Allow               | Maybe                    | No                           | I don't care about security, but I will pay the overhead of encryption if the server insists on it.
Prefer (default)    | Maybe                    | No                           | I don't care about encryption, but I wish to pay the overhead of encryption if the server supports it.
Require<sup>1</sup> | Yes                      | No                           | I want my data to be encrypted, and I accept the overhead. I trust that the network will make sure I always connect to the server I want.
VerifyCA            | Yes                      | Depends on CA policy         | I want my data encrypted, and I accept the overhead. I want to be sure that I connect to a server that I trust.
VerifyFull          | Yes                      | Yes                          | I want my data encrypted, and I accept the overhead. I want to be sure that I connect to a server I trust, and that it's the one I specify.

<sup>1</sup> Prior to Npgsql 8.0, `SSL Mode=Require` required explicitly setting `Trust Server Certificate=true` as well, to make it explicit that the server certificate isn't validated. Starting with 8.0, `Trust Server Certificate=true` is no longer required and does nothing.

The default mode in 6.0+ is `Prefer`, which allows SSL but does not require it, and does not validate certificates.

### [Older versions](#tab/tabid-2)

Versions prior to 6.0 supported the following `SSL Mode` values:

SSL Mode    | Eavesdropping protection | Man-in-the-middle protection | Statement
----------- | ------------------------ | ---------------------------- | ---------
Disable     | No                       | No                           | I don't care about security, and I don't want to pay the overhead of encryption.
Prefer      | Maybe                    | Maybe                        | I don't care about encryption, but I wish to pay the overhead of encryption if the server supports it.
Require     | Yes                      | Yes                          | I want my data encrypted, and I accept the overhead. I want to be sure that I connect to a server I trust, and that it's the one I specify.

The default mode prior to 6.0 was `Disable`.

To disable certificate validation when using `Require`, set `Trust Server Certificate` to true; this allows connecting to servers with e.g. self-signed certificates, while still requiring encryption.

---

### Advanced server certificate validation

If the root CA of the server certificate isn't installed in your machine's CA store, validation will fail. Either install the certificate in your machine's CA store, or point to it via the `Root Certificate` connection string parameter or via the `PGSSLROOTCERT` environment variable.

Note that Npgsql does not perform certificate revocation validation by default, since this is an optional extension not implemented by all providers and CAs. To turn on certificate revocation validation, specify `Check Certificate Revocation=true` on the connection string.

Finally, if the above options aren't sufficient for your scenario, you can call <xref:Npgsql.NpgsqlDataSourceBuilder.UseUserCertificateValidationCallback(System.Net.Security.RemoteCertificateValidationCallback)?displayProperty=nameWithType> to provide your custom server certificate validation logic (this gets set on the underlying .NET [`SslStream`](https://docs.microsoft.com/dotnet/api/system.net.security.sslstream.-ctor#System_Net_Security_SslStream__ctor_System_IO_Stream_System_Boolean_System_Net_Security_RemoteCertificateValidationCallback_System_Net_Security_LocalCertificateSelectionCallback_)).

### Client certificates

PostgreSQL may be configured to require valid certificates from connecting clients for authentication. Npgsql automatically sends client certificates specified in the following places:

* The `SSL Certificate` connection string parameter.
* The `PGSSLCERT` environment variable.
* The default locations of `~/.postgresql/postgresql.crt` (on Unix) or `%APPDATA%\postgresql\postgresql.crt` (on Windows)

To provide a password for a client certificate, set either the `SSL Password` (6.0 and higher) or `Client Certificate Key` (5.0 and lower) connection string parameter.

Finally, you can call <xref:Npgsql.NpgsqlDataSourceBuilder.UseClientCertificate(System.Security.Cryptography.X509Certificates.X509Certificate)?displayProperty=nameWithType>, <xref:Npgsql.NpgsqlDataSourceBuilder.UseClientCertificates(System.Security.Cryptography.X509Certificates.X509CertificateCollection)?displayProperty=nameWithType> or <xref:Npgsql.NpgsqlDataSourceBuilder.UseClientCertificatesCallback(System.Action{System.Security.Cryptography.X509Certificates.X509CertificateCollection})?displayProperty=nameWithType> to programmatically provide a certificate, multiple certificates or a callback which returns certificates (this works like on the underlying .NET [`SslStream`](https://docs.microsoft.com/dotnet/api/system.net.security.sslstream.-ctor#System_Net_Security_SslStream__ctor_System_IO_Stream_System_Boolean_System_Net_Security_RemoteCertificateValidationCallback_System_Net_Security_LocalCertificateSelectionCallback_)).

> [!NOTE]
> Npgsql supports .PFX and .PEM certificates starting with 6.0. Previously, only .PFX certificates were supported.

## Password-less authentication (GSS/SSPI/Kerberos)

Logging in with a username and password may not be ideal, since your application must have access to your password, and raise questions around secret management. An alternate way of authenticating is to use GSS or SSPI to negotiate Kerberos. The advantage of this method is that authentication is handed off to your operating system, using your already-open login session. Your application never needs to handle a password. You can use this method for a Kerberos login, Windows Active Directory or a local Windows session.

Instructions on setting up Kerberos and SSPI are available in the [PostgreSQL auth methods docs](http://www.postgresql.org/docs/current/static/auth-methods.html). Some more instructions for SSPI are [available here](https://wiki.postgresql.org/wiki/Configuring_for_single_sign-on_using_SSPI_on_Windows).

Once your PostgreSQL is configured correctly, it will require GSS/SSPI authentication from Npgsql at login, and you can simply drop the `Password` parameter from the connection string. However, Npgsql must still send a username to PostgreSQL. If you specify a `Username` connection string parameter, Npgsql will send that as usual. If you omit it, Npgsql will attempt to detect your system username, including the Kerberos realm. Note that by default, PostgreSQL expects your Kerberos realm to be sent in your username (e.g. `username@REALM`); you can have Npgsql detect the realm by setting `Include Realm` to true in your connection string. Alternatively, you can disable add `include_realm=0` in your PostgreSQL's pg_hba.conf entry, which will make it strip the realm. You always have the possibility of explicitly specifying the username sent to PostgreSQL yourself.

Note that in versions of Npgsql prior to 8.0, use of GSS/SSPI authentication requires that `Integrated Security=true` be specified on the connection string. This requirement has been removed in Npgsql 8.0.
