# Connection String Parameters

To connect to a database, the application provides a connection string which specifies parameters such as the host, the username, the password, etc. Connection strings have the form `keyword1=value; keyword2=value;` and are case-insensitive. Values containing special characters (e.g. semicolons) can be double-quoted. For more information, [see the official doc page on connection strings](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-strings).

Below are the connection string parameters which Npgsql understands, as well as some standard PostgreSQL environment variables.

## Basic connection

Parameter    | Description                                                                        | Default
------------ | ---------------------------------------------------------------------------------- | -------
Host         | Specifies the host name - and optionally port - on which PostgreSQL is running. Multiple hosts may be specified, [see the docs for more info](failover-and-load-balancing.md). If the value begins with a slash, it is used as the directory for the Unix-domain socket (specifying a `Port` is still required).  | *Required*
Port         | The TCP port of the PostgreSQL server.                                             | 5432
Database     | The PostgreSQL database to connect to.                                             | Same as Username
Username     | The username to connect with. Not required if using IntegratedSecurity.            | PGUSER
Password     | The password to connect with. Not required if using IntegratedSecurity.            | PGPASSWORD
Passfile     | Path to a PostgreSQL password file (PGPASSFILE), from which the password is taken. | PGPASSFILE

## Security and encryption

Parameter                    | Description                                                                                                                                 | Default
---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- | -------
SSL Mode                     | Controls whether SSL is used, depending on server support. [See docs for possible values and more info](security.md).                       | Prefer in 6.0, Disable previously
Trust Server Certificate     | Whether to trust the server certificate without validating it. [See docs for more info](security.md).                                       | false
SSL Certificate              | Location of a client certificate to be sent to the server. Introduced in 6.0. [See docs](security.md)                                       | PGSSLCERT
SSL Key                      | Location of a client key for a client certificate to be sent to the server. Introduced in 6.0.                                              | PGSSLKEY
SSL Password                 | Password for a key for a client certificate. Introduced in 6.0.                                                                             |
Root Certificate             | Location of a CA certificate used to validate the server certificate.                                                                       | PGSSLROOTCERT
Check Certificate Revocation | Whether to check the certificate revocation list during authentication.                                                                     | false
Integrated Security          | Whether to use integrated security to log in (GSS/SSPI). [See docs for more info](security.md).                                             | false
Persist Security Info        | Gets or sets a Boolean value that indicates if security-sensitive information, such as the password, is not returned as part of the connection if the connection is open or has ever been in an open state. Introduced in 3.1. | false
Kerberos Service Name        | The Kerberos service name to be used for authentication. [See docs for more info](security.md).                                             | postgres
Include Realm                | The Kerberos realm to be used for authentication. [See docs for more info](security.md).
Include Error Detail         | When enabled, PostgreSQL error and notice details are included on <xref:Npgsql.PostgresException.Detail?displayProperty=nameWithType> and <xref:Npgsql.PostgresNotice.Detail?displayProperty=nameWithType>. These can contain sensitive data. | false
Log Parameters               | When enabled, parameter values are logged when commands are executed.                                                                       | false

## Pooling

Parameter                   | Description                                | Default
--------------------------- | ------------------------------------------ | -------
Pooling                     | Whether connection pooling should be used. | true
Minimum Pool Size           | The minimum connection pool size.          | 0
Maximum Pool Size           | The maximum connection pool size.          | 100 since 3.1, 20 previously
Connection Idle Lifetime    | The time (in seconds) to wait before closing idle connections in the pool if the count of all connections exceeds `Minimum Pool Size`. Introduced in 3.1. | 300
Connection Pruning Interval | How many seconds the pool waits before attempting to prune idle connections that are beyond idle lifetime (see `Connection Idle Lifetime`). Introduced in 3.1. | 10
Connection Lifetime         | The total maximum lifetime of connections (in seconds). Connections which have exceeded this value will be destroyed instead of returned from the pool. This is useful in clustered configurations to force load balancing between a running server and a server just brought online. | 0 (disabled)

## Timeouts and keepalive

Parameter                | Description                                                  | Default
------------------------ | ------------------------------------------------------------ | -------
Timeout                  | The time to wait (in seconds) while trying to establish a connection before terminating the attempt and generating an error. | 15
Command Timeout          | The time to wait (in seconds) while trying to execute a command before terminating the attempt and generating an error. Set to zero for infinity. | 30
Internal Command Timeout | The time to wait (in seconds) while trying to execute an internal command before terminating the attempt and generating an error. -1 uses CommandTimeout, 0 means no timeout. | -1
Cancellation Timeout     | The time to wait (in milliseconds) while trying to read a response for a cancellation request for a timed out or cancelled query, before terminating the attempt and generating an error. -1 skips the wait, 0 means infinite wait. Introduced in 5.0. | 2000
Keepalive                | The number of seconds of connection inactivity before Npgsql sends a keepalive query. | 0 (disabled)
Tcp Keepalive            | Whether to use TCP keepalive with system defaults if overrides isn't specified. | false
Tcp Keepalive Time       | The number of seconds of connection inactivity before a TCP keepalive query is sent. Use of this option is discouraged, use KeepAlive instead if possible. | 0 (disabled)
Tcp Keepalive Interval   | The interval, in seconds, between when successive keep-alive packets are sent if no acknowledgement is received. `Tcp KeepAlive Time` must be non-zero as well. | value of `Tcp Keepalive Time`

## Performance

Parameter                  | Description                                                  | Default
-------------------------- | ------------------------------------------------------------ | -------
Max Auto Prepare           | The maximum number SQL statements that can be automatically prepared at any given point. Beyond this number the least-recently-used statement will be recycled. Zero disables automatic preparation. | 0
Auto Prepare Min Usages    | The minimum number of usages an SQL statement is used before it's automatically prepared. | 5
Read Buffer Size           | Determines the size of the internal buffer Npgsql uses when reading. Increasing may improve performance if transferring large values from the database. | 8192
Write Buffer Size          | Determines the size of the internal buffer Npgsql uses when writing. Increasing may improve performance if transferring large values to the database. | 8192
Socket Receive Buffer Size | Determines the size of socket receive buffer. | System-dependent
Socket Send Buffer Size    | Determines the size of socket send buffer. | System-dependent
No Reset On Close          | Improves performance in some cases by not resetting the connection state when it is returned to the pool, at the cost of leaking state. Use only if benchmarking shows a performance improvement | false

## Failover and load balancing

For more information, [see the dedicated docs page](failover-and-load-balancing.md).

Parameter                 | Description                                                                | Default
------------------------- | -------------------------------------------------------------------------- | -------------------------
Target Session Attributes | Determines the preferred PostgreSQL target server type.                    | PGTARGETSESSIONATTRS, Any
Load Balance Hosts        | Enables balancing between multiple hosts by round-robin.                   | false
Host Recheck Seconds      | Controls for how long the host's cached state will be considered as valid. | 10

## Misc

Parameter                | Description                                                                                          | Default
------------------------ | ---------------------------------------------------------------------------------------------------- | ----------
Options<sup>[1](#misc_sup_1)</sup> | Specifies any valid [PostgreSQL connection options](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-OPTIONS) (e.g. `Options=-c synchronous_commit=local`). Introduced in 5.0. | PGOPTIONS
Application Name         | The optional application name parameter to be sent to the backend during connection initiation.      |
Enlist                   | Whether to enlist in an ambient TransactionScope.                                                    | true
Search Path              | Sets the schema search path.                                                                         |
Client Encoding          | Gets or sets the client_encoding parameter.                                                          | PGCLIENTENCODING, UTF8
Encoding                 | Gets or sets the .NET encoding that will be used to encode/decode PostgreSQL string data.            | UTF8
Timezone                 | Gets or sets the session timezone.                                                                   | PGTZ
EF Template Database     | The database template to specify when creating a database in Entity Framework.                       | template1
EF Admin Database        | The database admin to specify when creating and dropping a database in Entity Framework.             | template1
Load Table Composites    | Load table composite type definitions, and not just free-standing composite types.                   | false
Array Nullability Mode   | Configure the way arrays of value types are returned when requested as object instances. Possible values are: `Never` (arrays of value types are always returned as non-nullable arrays), `Always` (arrays of value types are always returned as nullable arrays) and `PerInstance` (the type of array that gets returned is determined at runtime).             | Never

<sup id="misc_sup_1">1</sup>The `Options` connection string parameter is essentially the string of command line options that get passed to the [`postgres`](https://www.postgresql.org/docs/current/app-postgres.html) program when the process is started.
It is most commonly used to set named run-time parameters via the `-c` option but other options can be used too (although not all of them make sense in that context).
Setting multiple options is possible by separating them with a space character. Space and backslash characters in option values need to be escaped by prefixing a backslash character.
Example: `Options=-c default_transaction_isolation=serializable -c default_transaction_deferrable=on -c foo.bar=My\\ Famous\\\\Thing`

## Compatibility

Parameter                 | Description                                                                                       | Default
------------------------- | ------------------------------------------------------------------------------------------------- | -------
Server Compatibility Mode | A compatibility mode for special PostgreSQL server types. Currently "Redshift" is supported, as well as "NoTypeLoading", which will bypass the normal type loading mechanism from the PostgreSQL catalog tables and supports a hardcoded list of basic types. | none

## Obsolete

Parameter                 | Description                                                                                                      | Default
------------------------- | ---------------------------------------------------------------------------------------------------------------- | -------
Client Certificate        | Location of a client certificate to be sent to the server. Deprecated in 6.0.                                    | PGSSLCERT
Client Certificate Key    | Location of a client key for a client certificate to be sent to the server. Deprecated in 6.0.                   | PGSSLKEY
Use Perf Counters         | Makes Npgsql write performance information about connection use to Windows Performance Counters. Removed in 5.0. | false

## Environment variables

In addition to the connection string parameters above, Npgsql also recognizes the standard PostgreSQL environment variables below. This helps Npgsql-based applications behave similar to other, non-.NET PostgreSQL client applications. The PostgreSQL doc page on environment variables recognized by libpq can be found [here](https://www.postgresql.org/docs/current/libpq-envars.html).

Environment variable | Description
-------------------- | -----------
PGUSER               | Behaves the same as the [user](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-USER) connection parameter.
PGPASSWORD           | Behaves the same as the [password](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-PASSWORD) connection parameter. Use of this environment variable is not recommended for security reasons, as some operating systems allow non-root users to see process environment variables via ps; instead consider using a password file (see [Section 33.15](https://www.postgresql.org/docs/current/libpq-pgpass.html)).
PGPASSFILE           | Behaves the same as the [passfile](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-PASSFILE) connection parameter.
PGSSLCERT            | Behaves the same as the [sslcert](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-SSLCERT) connection parameter.
PGSSLKEY             | Behaves the same as the [sslkey](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-SSLKEY) connection parameter.
PGSSLROOTCERT        | Behaves the same as the [sslrootcert](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-SSLROOTCERT) connection parameter.
PGCLIENTENCODING     | Behaves the same as the [client_encoding](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-CLIENT-ENCODING) connection parameter.
PGTZ                 | Sets the default time zone. (Equivalent to SET timezone TO ....)
PGOPTIONS            | Behaves the same as the [options](https://www.postgresql.org/docs/current/libpq-connect.html#LIBPQ-CONNECT-OPTIONS) connection parameter.
