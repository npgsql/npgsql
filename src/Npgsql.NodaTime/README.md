Npgsql is the open source .NET data provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.

This package is an Npgsql plugin which allows you to use the [NodaTime](https://nodatime.org) date/time library when interacting with PostgreSQL; this provides a better and safer API for dealing with date and time data. 

To use the NodaTime plugin, simply add a dependency on this package and set it up at program startup:

```csharp
NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
```

Once this is done, you can simply use NodaTime types when interacting with PostgreSQL, just as you would use e.g. `DateTime`:

```csharp
// Write NodaTime Instant to PostgreSQL "timestamp with time zone" (UTC)
using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_timestamptz) VALUES (@p)", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter("p", Instant.FromUtc(2011, 1, 1, 10, 30)));
    cmd.ExecuteNonQuery();
}

// Read timestamp back from the database as an Instant
using (var cmd = new NpgsqlCommand(@"SELECT my_timestamptz FROM mytable", conn))
using (var reader = cmd.ExecuteReader())
{
    reader.Read();
    var instant = reader.GetFieldValue<Instant>(0);
}
```

For more information, [visit the NodaTime plugin documentation page](https://www.npgsql.org/doc/types/nodatime.html).
