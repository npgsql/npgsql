# Multiple Hosts, Failover and Load Balancing

> [!NOTE]
> The functionality described in this page was introduced in Npgsql 6.0.

Npgsql 6.0 allows specifying multiple hosts in your application's connection strings, allowing various failover and load balancing scenarios to be supported without the need for any additional component such as pgpool or pgbouncer. This typically requires setting up replication between your multiple PostgreSQL servers, to keep your standby servers in sync with your primary; this can be done with the help of PostgreSQL logical or physical replication, and some cloud providers provide this out of the box. Whatever the solution chosen, it's important to understand that this is out of Npgsql's scope - Npgsql is only responsible for connecting to your multiple servers as described below, and not for keeping your servers in sync.

## Multiple servers and failover

Npgsql allows you to specify multiple servers in your connection string as follows:

```text
Host=server1,server2;Username=test;Password=test
```

Different ports may be specified per host with the standard colon syntax: `Host=server1:5432,server2:5433`.

By default, Npgsql will try to connect to the servers in the order in which they were specified. In the above example, `server2` is only used if a connection could not be established to `server1` (or if the connection pool for `server1` has been exhausted). This allows a simple *failover* setup, where Npgsql always connects to a single, primary server, but can connect to a standby in case the primary is down; this improves the reliability of your application. In this configuration, we sometimes refer to the standby as "warm" - it is always up and in sync with the primary, but is only used when the primary is down.

> [!NOTE]
> Using failover as described above does not mean you don't have to worry about errors when your primary server is down. When opening a connection, you may get a broken connection from the pool: Npgsql has no way of knowing whether the connection is working without actually executing something on it, which would negate the perf advantages of pooling. Also, once you have an open connection, Npgsql will never implicitly retry a failed command on a failover server, since that command may be in a transaction (or otherwise depend on some state in the first connection). In other words, you must always be prepared to catch I/O-related exceptions when interacting with the database, and possibly implement a retrying strategy, opening a new connection and re-executing the series of commands.

## Specifying server types

In the failover scenario above, if `server1` goes down, `server2` is typically promoted to being the new primary. However, `server1` may be brought back up and assume the role of standby - the servers will have switched roles - and Npgsql will continue to connect to `server1` whenever possible. To mitigate this, you can tell Npgsql which server type you wish to connect to:

```text
Host=server1,server2;Username=test;Password=test;Target Session Attributes=primary
```

This will make Npgsql return connections only to the primary server, regardless of where it's located in the host list you provide.

## Load distribution

Going a step further, it's important to understand that applications don't always make use of the database in the same way; some parts of your application only need to read data from the database, while others need to write data. If you have one or more standby servers, Npgsql can dispatch read-only queries to those servers to reduce the load on your primary. While the failover setup described above improves *reliability*, this technique improves *performance*.

The `Target Session Attributes` parameter can be used to ask for a connection to a Standby, whenever possible:

```text
Host=server1,server2;Username=test;Password=test;Target Session Attributes=prefer-standby
```

With `prefer-standby`, as long as at least one standby server is available, Npgsql will return connections to that server. However, if all standby servers are down (or have exhausted their `Max Pool Size` setting), a connection to the primary will be returned instead.

`Target Session Attributes` supports the following options:

Option         | Description
-------------- | -----------
any            | Any successful connection is acceptable.
primary        | Server must not be in hot standby mode (`pg_is_in_recovery()` must return false).
standby        | Server must be in hot standby mode (`pg_is_in_recovery()` must return true).
prefer-primary | First try to find a primary server, but if none of the listed hosts is a primary server, try again in `Any` mode.
prefer-standby | First try to find a standby server, but if none of the listed hosts is a standby server, try again in `Any` mode.
read-write     | Session must accept read-write transactions by default (that is, the server must not be in hot standby mode and the `default_transaction_read_only` parameter must be off).
read-only      | Session must not accept read-write transactions by default (the converse).

Npgsql detects whether a server is a primary or a standby by occasionally querying `pg_is_in_recovery()`, and whether a server is read-write or read-only by querying [`default_transaction_read_only`](TODO) - this is consistent with how PostgreSQL's libpq implements `target_session_attributes`. Servers are queried just before a connection is returned from the pool; the query intervals can be controlled via the `Host Recheck Seconds` parameter (10 seconds by default). PostgreSQL 14 reports state changes automatically, so querying isn't needed (except when a host is down).

> [!NOTE]
> If you choose to distribute load across multiple servers, make sure you understand what consistency guarantees are provided by PostgreSQL in your particular setup. In some cases, hot standbys lag behind their primary servers, and will therefore return slightly out-of-date results. This is usually OK, but if you require up-to-date results at all times, synchronous commit may provide a good solution (but has a performance cost).

## Load balancing

We have seen how to select servers based on the type of workload we want to execute. However, in the above examples, Npgsql still attempts to return connections based on the host order specified in the connection string; this concentrates load on a single primary and possibly a single secondary, and doesn't balance load across multiple servers of the same type.

You can specify `Load Balance Hosts=true` in the connection string to instruct Npgsql to load balance across all servers, by returning connections in round-robbin fashion:

```text
Host=server1,server2,server3,server4,server5;Username=test;Password=test;Load Balance Hosts=true;Target Session Attributes=prefer-standby
```

With this connection string, every time a connection is opened, Npgsql will start at a different point in the list. For example, in the 3rd connection attempt, Npgsql will first try to return a connection to `server3`; if that server is reachable and is a standby, it will be selected. This allows spreading your (typically read-only) application load across all available servers, and can greatly improve your scalability.
