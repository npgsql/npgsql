# Waiting for Notifications

Note: *This functionality replaces Npgsql 3.0's "Continuous processing mode"*.

## PostgreSQL Asynchronous messages

PostgreSQL has a feature whereby arbitrary notification messages can be sent between clients. For example, one client may wait until it is
notified by another client of a task that it is supposed to perform. Notifications are, by their nature, asynchronous - they can arrive
at any point. For more detail about this feature, see the PostgreSQL [NOTIFY command](http://www.postgresql.org/docs/current/static/sql-notify.html).
Some other asynchronous message types are notices (e.g. database shutdown imminent) and parameter changes, see the
[PostgreSQL protocol docs](http://www.postgresql.org/docs/current/static/protocol-flow.html#PROTOCOL-ASYNC) for more details.

Note that despite the word "asynchronous", this page has nothing to do with ADO.NET async operations (e.g. ExecuteReaderAsync).

---

## Processing of Notifications

Npgsql exposes notification messages via the `Notification` event on NpgsqlConnection.

Since asynchronous notifications are rarely used and processing can be complex, Npgsql only processes notification messages as
part of regular (synchronous) query interaction. That is, if an asynchronous notification is sent, Npgsql will only process it and emit an
event to the user the next time a command is sent and processed.

To receive notifications outside a synchronous request-response cycle, call `NpgsqlConnection.Wait()`. This will make your thread block
until a single notification is received (note that a version with a timeout as well as an async version exist). Note that the notification
is still delivered via the `Notification` event as before.

```c#
var conn = new NpgsqlConnection(ConnectionString);
conn.Open();
conn.Notification += (o, e) => Console.WriteLine("Received notification");

using (var cmd = new NpgsqlCommand("LISTEN channel_name", conn)) {
  cmd.ExecuteNonQuery();
}

while (true) {
  conn.Wait();   // Thread will block here
}
```

---

## Keepalive

You may want to turn on [keepalives](keepalive.md).
