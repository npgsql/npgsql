# NodaTime Type Plugin

Npgsql provides a plugin that allows mapping the [NodaTime](http://nodatime.org) date/time library; this is the recommended way to interact with PostgreSQL date/time types, rather than the built-in .NET types.

## What is NodaTime

By default, [the PostgreSQL date/time types](https://www.postgresql.org/docs/current/static/datatype-datetime.html) are mapped to the built-in .NET types (`DateTime`, `TimeSpan`). Unfortunately, these built-in types are flawed in many ways. The [NodaTime library](http://nodatime.org/) was created to solve many of these problems, and if your application handles dates and times in anything but the most basic way, you should consider using it. To learn more [read this blog post by Jon Skeet](http://blog.nodatime.org/2011/08/what-wrong-with-datetime-anyway.html).

Beyond NodaTime's general advantages, some specific advantages NodaTime for PostgreSQL date/time mapping include:

* NodaTime's types map very cleanly to the PostgreSQL types. For example `Instant` corresponds to `timestamptz`, and `LocalDateTime` corresponds to `timestamp without time zone`. The BCL's DateTime can correspond to both, depending on its type; this can create confusion and errors.
* `Period` is much more suitable for mapping PostgreSQL `interval` than `TimeSpan`.
* NodaTime types can fully represent PostgreSQL's microsecond precision, and can represent dates outside the BCL's date limit (1AD-9999AD).

## Setup

To avoid forcing a dependency on the NetTopologySuite library for users not using spatial, NodaTime support is delivered as a separate plugin. To use the plugin, simply add a dependency on [Npgsql.NodaTime](https://www.nuget.org/packages/Npgsql.NodaTime) and set it up in one of the following ways:

### [NpgsqlDataSource](#tab/datasource)

> [!NOTE]
> `NpgsqlDataSource` was introduced in Npgsql 7.0, and is the recommended way to manage type mapping. If you're using an older version, see the other methods.

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.UseNodaTime();
await using var dataSource = dataSourceBuilder.Build();
```

### [Global mapping](#tab/global)

If you're using an older version of Npgsql which doesn't yet support `NpgsqlDataSource`, you can configure mappings globally for all connections in your application:

```csharp
NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
```

For this to work, you must place this code at the beginning of your application, before any other Npgsql API is called. Note that in Npgsql 7.0, global type mappings are obsolete (but still supported) - `NpgsqlDataSource` is the recommended way to manage type mappings.

### [Connection mapping](#tab/connection)

> [!NOTE]
> This mapping method has been removed in Npgsql 7.0.

Older versions of Npgsql supported configuring a type mapping on an individual connection, as follows:

```csharp
var conn = new NpgsqlConnection(...);
conn.TypeMapper.UseNodaTime();
```

***

## Reading and Writing Values

Once the plugin is set up, you can transparently read and write NodaTime objects:

```csharp
// Write NodaTime Instant to PostgreSQL "timestamp with time zone" (UTC)
await using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_timestamptz) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new() { Value = Instant.FromUtc(2011, 1, 1, 10, 30) });
    await cmd.ExecuteNonQueryAsync();
}

// Read timestamp back from the database as an Instant
await using (var cmd = new NpgsqlCommand(@"SELECT my_timestamptz FROM mytable", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    await reader.ReadAsync();
    var instant = reader.GetFieldValue<Instant>(0);
}
```

## Mapping Table

> [!Warning]
> A common mistake is for users to think that the PostgreSQL `timestamp with time zone` type stores the timezone in the database. This is not the case: only a UTC timestamp is stored. There is no single PostgreSQL type that stores both a date/time and a timezone, similar to [.NET DateTimeOffset](https://msdn.microsoft.com/en-us/library/system.datetimeoffset(v=vs.110).aspx). To store a timezone in the database, add a separate text column containing the timezone ID.

PostgreSQL Type                 | Default NodaTime Type                                                                   | Additional NodaTime Type      | Notes
------------------------------- | --------------------------------------------------------------------------------------- | ----------------------------- | ------
timestamp with time zone        | [Instant](https://nodatime.org/3.0.x/api/NodaTime.Instant.html)                         | [ZonedDateTime](https://nodatime.org/3.0.x/api/NodaTime.ZonedDateTime.html)<sup>1</sup>, [OffsetDateTime](https://nodatime.org/3.0.x/api/NodaTime.OffsetDateTime.html)<sup>1</sup> | A UTC timestamp in the database. Only UTC ZonedDateTime and OffsetDateTime are supported.
timestamp without time zone     | [LocalDateTime](https://nodatime.org/3.0.x/api/NodaTime.LocalDateTime.html)<sup>2</sup> |                               | A timestamp in an unknown or implicit time zone.
date                            | [LocalDate](https://nodatime.org/3.0.x/api/NodaTime.LocalDate.html)                     |                               | A simple date with no timezone or offset information.
time without time zone          | [LocalTime](https://nodatime.org/3.0.x/api/NodaTime.LocalTime.html)                     |                               | A simple time-of-day, with no timezone or offset information.
time with time zone             | [OffsetTime](https://nodatime.org/3.0.x/api/NodaTime.OffsetTime.html)                   |                               | A type that stores a time and an offset. It's use is generally discouraged.
interval                        | [Period](https://nodatime.org/3.0.x/api/NodaTime.Period.html)                           | [Duration](https://nodatime.org/3.0.x/api/NodaTime.Duration.html) | An interval of time, from sub-second units to years. NodaTime `Duration` is supported for intervals with days and smaller, but not with years or months (as these have no absolute duration). `Period` can be used with any interval unit.
tstzrange                       | [Interval](https://nodatime.org/3.0.x/api/NodaTime.Interval.html)                       | `NpgsqlRange<Instant>` etc.   | An interval between two instants in time (start and end).
tsrange                         | `NpgsqlRange<LocalDateTime>`                                                            |                               | An interval between two timestamps in an unknown or implicit time zone.
daterange                       | [DateInterval](https://nodatime.org/3.0.x/api/NodaTime.DateInterval.html)               | `NpgsqlRange<LocalDate>` etc. | An interval between two dates.

<sup>1</sup> In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), writing or reading ZonedDateTime or OffsetDateTime automatically converted to or from UTC. [See the breaking change note for more info](../release-notes/6.0.md#major-changes-to-timestamp-mapping).

<sup>2</sup> In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), `timestamp without time zone` was mapped to Instant by default, instead of LocalDateTime. [See the breaking change note for more info](../release-notes/6.0.md#major-changes-to-timestamp-mapping).

## Infinity values

PostgreSQL supports the special values `-infinity` and `infinity` for the timestamp and date types ([see docs](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-DATETIME-SPECIAL-VALUES)); these can be useful to represent a value which is earlier or later than any other value. Starting with Npgsql 6.0, these special values are mapped to the `MinValue` and `MaxValue` value on the corresponding .NET types (`Instant` and `LocalDate`). To opt out of this behavior, set the following AppContext switch at the start of your application:

```csharp
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
```

Note: in versions prior to 6.0, the connection string parameter `Convert Infinity DateTime` could be used to opt into these infinity conversions. That connection string parameter has been removed.
