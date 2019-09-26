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

The keepalive mechanism above is ideal for long-standing idle connections, but it cannot be used during query processing. With some PostgreSQL-like data warehouse products such as [Amazon Redshift](http://docs.aws.amazon.com/redshift/latest/mgmt/welcome.html) or [Greenplum](http://greenplum.org/), it is not uncommon for a single SQL statement to take a long time to execute, and during that time it is not possible to send `SELECT NULL`. For these cases you may want to turn on *TCP keepalive*, which is quite different from the application-level keepalive described above. To better understand the different kinds of keepalives, see [this blog post](http://blog.stephencleary.com/2009/05/detection-of-half-open-dropped.html). As that article explains, TCP keepalive depends on networking stack support and might not always work, but it is your only option during query processing.

On Linux, you turn keepalives simply by specifying `Tcp Keepalive=true` in your connection string. The default system-wide settings will be used (for interval, count...) - it is currently impossible to specify these at the application level. On Windows, you can also specify `Tcp Keepalive Time` and `Tcp Keepalive Interval` to tweak these settings.
