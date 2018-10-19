# Connection String Parameters

To connect to a database, the application provides a connection string which specifies parameters such as the host, the username, the password, etc. Connection strings have the form `keyword1=value; keyword2=value;` and are case-insensitive. Values containing special characters (e.g. semicolons) can be double-quoted. For more information, [see the official doc page on connection strings](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-strings).

Below are the connection string parameters which Npgsql understands.

## Basic Connection

| Parameter    | Description                                                                        | Default          |
|--------------|------------------------------------------------------------------------------------|------------------|
| Host         | Specifies the host name of the machine on which the server is running. If the value begins with a slash, it is used as the directory for the Unix-domain socket (specifying a `Port` is still required). | *Required*       |
| Port         | The TCP port of the PostgreSQL server.                                             | 5432             |
| Database     | The PostgreSQL database to connect to.                                             | Same as Username |
| Username     | The username to connect with. Not required if using IntegratedSecurity.            |                  |
| Password     | The password to connect with. Not required if using IntegratedSecurity.            |                  |
| Passfile     | Path to a PostgreSQL password file (PGPASSFILE), from which the password is taken. |                  |

## Security and Encryption

| Parameter                | Description                                                             | Default          |
|--------------------------|-------------------------------------------------------------------------|------------------|
| SSL Mode                 | Controls whether SSL is used, depending on server support. Can be `Require`, `Disable`, or `Prefer`. [See docs for more info](security.md). | Disable |
| Trust Server Certificate | Whether to trust the server certificate without validating it. [See docs for more info](security.md). | false |
| Use SSL Stream           | Npgsql uses its own internal implementation of TLS/SSL. Turn this on to use .NET SslStream instead. | false |
| Check Certificate Revocation | Whether to check the certificate revocation list during authentication. False by default. | false |
| Integrated Security      | Whether to use integrated security to log in (GSS/SSPI), currently supported on Windows only. [See docs for more info](security.md). | false |
| Persist Security Info    | Gets or sets a Boolean value that indicates if security-sensitive information, such as the password, is not returned as part of the connection if the connection is open or has ever been in an open state. Since 3.1 only. | false |
| Kerberos Service Name    | The Kerberos service name to be used for authentication. [See docs for more info](security.md). | postgres |
| Include Realm            | The Kerberos realm to be used for authentication. [See docs for more info](security.md). | |

## Pooling

| Parameter | Description                                                  | Default                      |
|-----------|--------------------------------------------------------------|------------------------------|
| Pooling                     | Whether connection pooling should be used. | true                         |
| Minimum Pool Size           | The minimum connection pool size.          | 1                            |
| Maximum Pool Size           | The maximum connection pool size.          | 100 since 3.1, 20 previously |
| Connection Idle Lifetime    | The time (in seconds) to wait before closing idle connections in the pool if the count of all connections exceeds MinPoolSize. Since 3.1 only. | 300 |
| Connection Pruning Interval | How many seconds the pool waits before attempting to prune idle connections that are beyond idle lifetime (see `ConnectionIdleLifetime`). Since 3.1 only. | 10 |

## Timeouts and Keepalive

| Parameter                | Description                                                  | Default                      |
|--------------------------|--------------------------------------------------------------|------------------------------|
| Timeout                  | The time to wait (in seconds) while trying to establish a connection before terminating the attempt and generating an error. | 15 |
| Command Timeout          | The time to wait (in seconds) while trying to execute a command before terminating the attempt and generating an error. Set to zero for infinity. | 30 |
| Internal Command Timeout | The time to wait (in seconds) while trying to execute a an internal command before terminating the attempt and generating an error. -1 uses CommandTimeout, 0 means no timeout. | -1 |
| Keepalive                | The number of seconds of connection inactivity before Npgsql sends a keepalive query. | disabled |
| Tcp Keepalive            | Whether to use TCP keepalive with system defaults if overrides isn't specified. | disabled |
| Tcp Keepalive Time       | The number of milliseconds of connection inactivity before a TCP keepalive query is sent. Use of this option is discouraged, use KeepAlive instead if possible. Supported only on Windows. | disabled |
| Tcp Keepalive Interval   | The interval, in milliseconds, between when successive keep-alive packets are sent if no acknowledgement is received. TcpKeepAliveTime must be non-zero as well. Supported only on Windows. | value of TcpKeepAliveTime |

## Performance

| Parameter                  | Description                                                  | Default                      |
|----------------------------|--------------------------------------------------------------|------------------------------|
| Max Auto Prepare           | The maximum number SQL statements that can be automatically prepared at any given point. Beyond this number the least-recently-used statement will be recycled. Zero disables automatic preparation. | 0 |
| Auto Prepare Min Usages    | The minimum number of usages an SQL statement is used before it's automatically prepared. | 5 |
| Use Perf Counters          | Makes Npgsql write performance information about connection use to Windows Performance Counters. [Read the docs](performance.md#performance-counters) for more info.
| Read Buffer Size           | Determines the size of the internal buffer Npgsql uses when reading. Increasing may improve performance if transferring large values from the database. | 8192 |
| Write Buffer Size          | Determines the size of the internal buffer Npgsql uses when writing. Increasing may improve performance if transferring large values to the database. | 8192 |
| Socket Receive Buffer Size | Determines the size of socket receive buffer. | System-dependent |
| Socket Send Buffer Size    | Determines the size of socket send buffer. | System-dependent |
| No Reset On Close          | Improves performance in some cases by not resetting the connection state when it is returned to the pool, at the cost of leaking state. Use only if benchmarking shows a performance improvement | false |

## Misc

| Parameter                | Description                                                                                     | Default   |
|--------------------------|-------------------------------------------------------------------------------------------------|-----------|
| Application Name         | The optional application name parameter to be sent to the backend during connection initiation. |           |
| Enlist                   | Whether to enlist in an ambient TransactionScope.                                               | true      |
| Search Path              | Sets the schema search path.                                                                    |           |
| Client Encoding          | Gets or sets the client_encoding parameter. Since 3.1.                                          |           |
| Timezone                 | Gets or sets the session timezone, PGTZ environment variable can be used instead. Since 3.3.    |           |
| EF Template Database     | The database template to specify when creating a database in Entity Framework.                  | template1 |
| Load Table Composites    | Load table composite type definitions, and not just free-standing composite types.              | false     |

## Compatibility

| Parameter                 | Description                                                                                       | Default |
|---------------------------|---------------------------------------------------------------------------------------------------|---------|
| Server Compatibility Mode | A compatibility mode for special PostgreSQL server types. Currently "Redshift" is supported, as well as "NoTypeLoading", which will bypass the normal type loading mechanism from the PostgreSQL catalog tables and supports a hardcoded list of basic types . | none    |
| Convert Infinity DateTime | Makes MaxValue and MinValue timestamps and dates readable as infinity and negative infinity.      | false   |
