# FAQ

## <a name="stored_procedures">How can I call a PostgreSQL 11 stored procedure? I tried doing so with CommandType.StoredProcedure and got an error...</a>

PostgreSQL 11 stored procedures can be called, but unfortunately not with `CommandType.StoredProcedure`. PostgreSQL has supported stored *functions* for a long while, and since these have acted as replacements for non-existing procedures, Npgsql's `CommandType.StoredProcedure` has been implemented to invoke them; this means that `CommandType.StoredProcedure` translates into `SELECT * FROM my_stored_function()`. The new stored procedures introduce a special invocation syntax - `CALL my_stored_procedure()` - which is incompatible with the existing stored function syntax.

On the brighter side, it's very easy to invoke stored procedures (or functions) yourself - you don't really need `CommandType.StoredProcedure`. Simply create a regular command and set `CommandText` to `CALL my_stored_procedure(@p1, @p2)`, handling parameters like you would any other statement. In fact, with Npgsql and PostgreSQL, `CommandType.StoredProcedure` doesn't really have any added value over constructing the command yourself.

## <a name="broken_connection_from_pool">I opened a pooled connection, and it throws right away when I use it! What gives?</a>

We know it's frustrating and seems weird, but this behavior is by-design.

While your connection is idle in the pool, any number of things could happen to it - a timeout could cause it to break, or some other similar network problem. Unfortunately, with the way networking works, there is no reliable way for us to know on the client if a connection is still alive; the only thing we can do is send something to PostgreSQL, and wait for the response to arrive. Doing this whenever a connection is handed out from the pool would kill the very reason pooling exists - it would dramatically slow down pooling, which is there precisely to avoid unneeded network roundtrips.

But the reality is even more grim than that. Even if Npgsql checked whether a connection is live before handing it out of the pool, there's nothing guaranteeing that the connection won't break 1 millisecond after that check - it's a total race condition. So the check wouldn't just degrade performance, it would also be largely useless. The reality of network programming is that I/O errors can occur at any point, and your code must take that into account if it has high reliability requirements. Resilience/retrying systems can help you with this; take a look at [Polly](https://github.com/App-vNext/Polly) as an example.

One thing which Npgsql can do to help a bit, is the [keepalive feature](https://www.npgsql.org/doc/keepalive.html); this does a roundtrip with PostgreSQL every e.g. 1 second - including when the connection is idle in the pool - and destroys it if an I/O error occurs. However, depending on timing, you may still get a broken connection out of the pool - unfortunately there's simply no way around that.

## <a name="unknown_type">I get an exception "The field field1 has a type currently unknown to Npgsql (OID XXXXX). You can retrieve it as a string by marking it as unknown".</a>

Npgsql has to implement support for each PostgreSQL type, and it seems you've stumbled upon an unsupported type.

First, head over to our [issues page](https://github.com/npgsql/npgsql/issues) and check if an issue already exists on your type,
otherwise please open one to let us know.

Then, as a workaround, you can have your type treated as text - it will be up to you to parse it in your program.
One simple way to do this is to append ::TEXT in your query (e.g. `SELECT 3::TEXT`).

If you don't want to modify your query, Npgsql also includes an API for requesting types as text.
The following code returns all the columns in the resultset as text:

```csharp
await using (var cmd = new NpgsqlCommand(...)) {
  cmd.AllResultTypesAreUnknown = true;
  await using var reader = await cmd.ExecuteReaderAsync();
  // Read everything as strings
}
```

You can also specify text only for some columns in your resultset:

```csharp
await using (var cmd = new NpgsqlCommand(...)) {
  // Only the second field will be fetched as text
  cmd.UnknownResultTypeList = new[] { false, true };
  await using var reader = await cmd.ExecuteReaderAsync();
  // Read everything as strings
}
```

## <a name="jsonb">I'm trying to write a JSONB type and am getting 'column "XXX" is of type jsonb but expression is of type text'</a>

When sending a JSONB parameter, you must explicitly specify its type to be JSONB with NpgsqlDbType:

```csharp
await using (var cmd = new NpgsqlCommand("INSERT INTO foo (col) VALUES (@p)", conn)) {
  cmd.Parameters.AddWithValue("p", NpgsqlDbType.Jsonb, jsonText);
}
```

## I'm trying to apply an Entity Framework 6 migration and I get `Type is not resolved for member 'Npgsql.NpgsqlException,Npgsql'`

Unfortunately, a shortcoming of EF6 requires you to have Npgsql.dll in the Global Assembly Cache (GAC), otherwise you can't see
migration-triggered exceptions. You can add Npgsql.dll to the GAC by opening a VS Developer Command Prompt as administator and
running the command `gacutil /i Npgsql.dll`. You can remove it from the GAC with `gacutil /u Npgsql`.
