---
layout: doc
title: Continuous Processing
---

## PostgreSQL Asynchronous messages

PostgreSQL has a feature whereby arbitrary notification messages can be sent between clients. For example, one client may wait until it is
notified by another client of a task that it is supposed to perform. Notifications are, by their nature, asynchronous - they can arrive
at any point. For more detail about this feature, see the PostgreSQL [NOTIFY command](http://www.postgresql.org/docs/current/static/sql-notify.html).
Some other asynchronous message types are notices (e.g. database shutdown imminent) and parameter changes, see the
[PostgreSQL protocol docs](http://www.postgresql.org/docs/current/static/protocol-flow.html#PROTOCOL-ASYNC) for more details.

Note that despite the word "asynchronous", this page has nothing to do with ADO.NET async operations.

---

## Processing of Asynchronous Messages

Npgsql exposes notification messages via the Notification event on NpgsqlConnection.

Since asynchronous notifications are rarely used and processing them is complex, by default Npgsql only processes notification messages as
part of regular (synchronous) query interaction. That is, if an asynchronous notification is sent, Npgsql will only process it and emit an
event to the user the next time a command is sent and processed. To make Npgsql process messages at any time, even if no query is in
progress, enable the [Continuous Processing](connection-string-parameters.html#continuous-processing) flag in your connection string. 

{% highlight C# %}

using (var conn = new NpgsqlConnection(ConnectionString + ";ContinuousProcessing=true"))
{
    conn.Open();
    conn.Notification += (o, e) => Console.WriteLine("Received notification");

    using (var cmd = new NpgsqlCommand("LISTEN notifytest", conn))
    {
        cmd.CommandTimeout = 0;   // Disable timeout

        cmd.ExecuteNonQuery();
        // The program will hang until someone else does "NOTIFY notifytest".
        // At this point the LISTEN command will complete successfully.
    }
}

{% endhighlight %}

---

## Keepalive

Clients in continuous processing mode can sometimes wait for hours, even days before getting a single notification. In this scenario,
how can the client know the connection is still up, and hasn't been broken by a server or network outage? For this purpose, Npgsql
has a keepalive feature, which makes it send periodic `SELECT 1` queries. This feature is by default disabled, and must be enabled via
the [Keepalive](connection-string-parameters.html#keepalive) connection string parameter, setting the number of seconds between each
keepalive.

When keepalive is enabled, Npgsql will emit an
[`NpgsqlConnection.StateChange`](https://msdn.microsoft.com/en-us/library/system.data.common.dbconnection.statechange(v=vs.110).aspx)
event if the keepalive fails.

Note that it does not really make sense to turn on this feature unless you're going to be waiting for asynchronous notifications (and
turning on ContinuousProcessing). For normal operations, simply perform your queries and you'll get an exception if the connection
fails.

Note: Npgsql does not turn on TCP keepalive because that feature isn't universally reliable across all network
equipment.

