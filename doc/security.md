# Security and Encryption

## Logging in

The simplest way to log into PostgreSQL is by specifying a `Username` and a `Password` in your connection string. Depending on how your PostgreSQL is configured (in the `pg_hba.conf` file), Npgsql will send the password in MD5 or in cleartext (not recommended).

For documentation about all auth methods supported by PostgreSQL, [see this page](http://www.postgresql.org/docs/current/static/auth-methods.html). Note that Npgsql supports Unix-domain sockets (auth method `local`), simply set your `Host` parameter to the absolute path of your PostgreSQL socket directory, as configred in your `postgresql.conf`.

## Integrated Security (GSS/SSPI/Kerberos)

Logging in with a username and password isn't recommended, since your application must have access to your password. An alternate way of authenticating is "Integrated Security", which uses GSS or SSPI to negotiate Kerberos. The advantage of this method is that authentication is handed off to your operating system, using your already-open login session. Your application never needs to handle a password. You can use this method for a Kerberos login, Windows Active Directory or a local Windows session. Note that since 3.2, this method of authentication also works on non-Windows platforms.

Instructions on setting up Kerberos and SSPI are available in the [PostgreSQL auth methods docs](http://www.postgresql.org/docs/current/static/auth-methods.html). Some more instructions for SSPI are [available here](https://wiki.postgresql.org/wiki/Configuring_for_single_sign-on_using_SSPI_on_Windows).

Once your PostgreSQL is configured correctly, simply include `Integrated Security=true` in your connection string and drop the Password parameter. However, Npgsql must still send a username to PostgreSQL. If you specify a `Username` connection string parameter, Npgsql will send that as usual. If you omit it, Npgsql will attempt to detect your system username, including the Kerberos realm. Note that by default, PostgreSQL expects your Kerberos realm to be sent in your username (e.g. `username@REALM`); you can have Npgsql detect the realm by setting `Include Realm` to true in your connection string. Alternatively, you can disable add `include_realm=0` in your PostgreSQL's pg_hba.conf entry, which will make it strip the realm. You always have the possibility of explicitly specifying the username sent to PostgreSQL yourself.

## Encryption (SSL/TLS)

By default PostgreSQL connections are unencrypted, but you can turn on SSL/TLS encryption if you wish. First, you have to set up your PostgreSQL to receive SSL/TLS connections [as described here](http://www.postgresql.org/docs/current/static/ssl-tcp.html).

Once that's done, specify `SSL Mode` in your connection string, setting it to either `Require` (connection will fail if the server isn't set up for encryption), or `Prefer` (use encryption if possible but fallback to unencrypted otherwise).

Note that by default, Npgsql will verify that your server's certificate is valid. If you're using a self-signed certificate this will fail. You can instruct Npgsql to ignore this by specifying
`Trust Server Certificate=true` in the connection string. To precisely control how the server's certificate is validated, you can register `UserCertificateValidationCallback` on `NpgsqlConnection` (this works just like on .NET's [`SSLStream`](https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx)).

You can also have Npgsql provide client certificates to the server by registering the `ProvideClientCertificatesCallback` on `NpgsqlConnection` (this works just like on .NET's [`SSLStream`](https://msdn.microsoft.com/en-us/library/system.net.security.localcertificateselectioncallback(v=vs.110).aspx)).
