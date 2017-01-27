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

Note: Npgsql does not turn on TCP keepalive because that feature isn't universally reliable across all network
equipment.

