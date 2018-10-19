# FAQ

### <a name="stored_procedures">How can I call a PostgreSQL 11 stored procedure? I tried doing so with CommandType.StoredProcedure and got an error...</a>

PostgreSQL 11 stored procedures can be called, but unfortunately not with `CommandType.StoredProcedure`. PostgreSQL has supported stored *functions* for a long while, and since these have acted as replacements for non-existing procedures, Npgsql's `CommandType.StoredProcedure` has been implemented to invoke them; this means that `CommandType.StoredProcedure` translates into `SELECT * FROM my_stored_function()`. The new stored procedures introduce a special invocation syntax - `CALL my_stored_procedure()` - which is incompatible with the existing stored function syntax.

On the brighter side, it's very easy to invoke stored procedures (or functions) yourself - you don't really need `CommandType.StoredProcedure`. Simply create a regular command and set `CommandText` to `CALL my_stored_procedure(@p1, @p2)`, handling parameters like you would any other statement. In fact, with Npgsql and PostgreSQL, `CommandType.StoredProcedure` doesn't really have any added value over constructing the command yourself.

### <a name="unknown_type">I get an exception "The field field1 has a type currently unknown to Npgsql (OID XXXXX). You can retrieve it as a string by marking it as unknown".</a>

Npgsql has to implement support for each PostgreSQL type, and it seems you've stumbled upon an unsupported type.

First, head over to our [issues page](https://github.com/npgsql/npgsql/issues) and check if an issue already exists on your type,
otherwise please open one to let us know.

Then, as a workaround, you can have your type treated as text - it will be up to you to parse it in your program.
One simple way to do this is to append ::TEXT in your query (e.g. `SELECT 3::TEXT`).

If you don't want to modify your query, Npgsql also includes an API for requesting types as text.
The fetch all the columns in the resultset as text, 

```c#
using (var cmd = new NpgsqlCommand(...)) {
  cmd.AllResultTypesAreUnknown = true;
  var reader = cmd.ExecuteReader();
  // Read everything as strings
}
```

You can also specify text only for some columns in your resultset:

```c#
using (var cmd = new NpgsqlCommand(...)) {
  // Only the second field will be fetched as text
  cmd.UnknownResultTypeList = new[] { false, true };
  var reader = cmd.ExecuteReader();
  // Read everything as strings
}
```

---

### <a name="jsonb">I'm trying to write a JSONB type and am getting 'column "XXX" is of type jsonb but expression is of type text'</a>

When sending a JSONB parameter, you must explicitly specify its type to be JSONB with NpgsqlDbType:

```c#
using (var cmd = new NpgsqlCommand("INSERT INTO foo (col) VALUES (@p)", conn)) {
  cmd.Parameters.AddWithValue("p", NpgsqlDbType.Jsonb, jsonText);
}
```

---

### I'm trying to apply an Entity Framework 6 migration and I get `Type is not resolved for member 'Npgsql.NpgsqlException,Npgsql'`!

Unfortunately, a shortcoming of EF6 requires you to have Npgsql.dll in the Global Assembly Cache (GAC), otherwise you can't see
migration-triggered exceptions. You can add Npgsql.dll to the GAC by opening a VS Developer Command Prompt as administator and
running the command `gacutil /i Npgsql.dll`. You can remove it from the GAC with `gacutil /u Npgsql`.
