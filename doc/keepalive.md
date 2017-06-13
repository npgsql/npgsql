# Keepalive

Some clients keep idle connections for long periods of time - this is especially frequent when waiting for PostgreSQL notifications.
In this scenario, how can the client know the connection is still up, and hasn't been broken by a server or network outage?
For this purpose, Npgsql has a keepalive feature, which makes it send periodic `SELECT NULL` queries.
This feature is by default disabled, and must be enabled via the
[Keepalive](connection-string-parameters.md#timeouts-and-keepalive) connection string parameter, setting the number of seconds between each keepalive.

When keepalive is enabled, Npgsql will emit an
[`NpgsqlConnection.StateChange`](https://msdn.microsoft.com/en-us/library/system.data.common.dbconnection.statechange(v=vs.110).aspx)
event if the keepalive fails.

Note that you should only turn this feature on if you need it. Unless you know you'll have long-lived idle connections, and that your
backend (or network equipment) will interfere with these connections, it's better to leave this off.

## TCP Keepalives

Npgsql also supports TCP keepalive, which is different from the application-level keepalive described above. To better understand the different kinds of keepalives, see [this blog post](http://blog.stephencleary.com/2009/05/detection-of-half-open-dropped.html). In a nutshell, while application-level keepalive will always work, TCP keepalive depends on your networking stack properly supporting it, etc.

One known case in which TCP keepalive may be necessary, is when you need keepalives during query processing, as opposed to between queries; this is the case if you send a query for processing, and plan to spend a lot of time before getting any response from the server (see [#1596](https://github.com/npgsql/npgsql/issues/1596) for an example). TCP keepalive may also be helpful in some other cases.
