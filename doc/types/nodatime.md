# NodaTime Type Plugin

Since 4.0, Npgsql supports *type plugins*, which are external nuget packages that modify how Npgsql maps PostgreSQL values to CLR types. One of these is the NodaTime plugin, which makes Npgsql read and write [NodaTime](http://nodatime.org) types. The NodaTime plugin is now the recommended way to interact with PostgreSQL date/time types, and isn't the default only because of the added dependency on the NodaTime library.

# What is NodaTime?

By default, [the PostgreSQL date/time types](https://www.postgresql.org/docs/current/static/datatype-datetime.html) are mapped to the built-in .NET types (`DateTime`, `TimeSpan`). Unfortunately, these built-in types are flawed in many ways. The [NodaTime library](http://nodatime.org/) was created to solve many of these problems, and if your application handles dates and times in anything but the most basic way, you should consider using it. To learn more [read this blog post by Jon Skeet](http://blog.nodatime.org/2011/08/what-wrong-with-datetime-anyway.html).

Beyond NodaTime's general advantages, some specific advantages NodaTime for PostgreSQL date/time mapping include:

* NodaTime defines some types which are missing from the BCL, such as `LocalDate`, `LocalTime`, and `OffsetTime`. These cleanly correspond to PostgreSQL `date`, `time` and `timetz`.
* `Period` is much more suitable for mapping PostgreSQL `interval` than `TimeSpan`.
* NodaTime types can fully represent PostgreSQL's microsecond precision, and can represent dates outside the BCL's date limit (1AD-9999AD).

## Setup

To use the NodaTime plugin, simply add a dependency on [Npgsql.NodaTime](https://www.nuget.org/packages/Npgsql.NodaTime) and set it up:

```c#
using Npgsql;

// Place this at the beginning of your program to use NodaTime everywhere (recommended)
NpgsqlConnection.GlobalTypeMapper.UseNodaTime();

// Or to temporarily use NodaTime on a single connection only:
conn.TypeMapper.UseNodaTime();
```

## Mapping Table

> [!Warning]
> A common mistake is for users to think that the PostgreSQL `timestamp with timezone` type stores the timezone in the database. This is not the case: only the t
imestamp is stored. There is no single PostgreSQL type that stores both a date/time and a timezone, similar to [.NET DateTimeOffset](https://msdn.microsoft.com/en-us/library/system.datetimeoffset(v=vs.110).aspx).

PostgreSQL Type 		| Default NodaTime Type | Additional NodaTime Type      | Notes
--------------------------------|-----------------------|-------------------------------|-------
timestamp       		| Instant               | LocalDateTime                 | It's common to store UTC timestamps in databases - you can simply do so and read/write Instant values. You also have the option of readin/writing LocalDateTime, which is a date/time with no information about timezones; this makes sense if you're storing the timezone in a different column and want to read both into a NodaTime ZonedDateTime.
timestamp with time zone	| Instant               | ZonedDateTime, OffsetDateTime | This PostgreSQL type stores only a timestamp, assumed to be in UTC. If you read/write this as an Instant, it will be provided as stored with no timezone conversions whatsoever. If, however, you read/write as a ZonedDateTime or OffsetDateTime, the plugin will automatically convert to and from UTC according to your PostgreSQL session's timezone.
date				| LocalDate             |                               | A simple date with no timezone or offset information.
time				| LocalTime             |                               | A simple time-of-day, with no timezone or offset information.
time with time zone		| OffsetTime            |                               | This is a PostgreSQL type that stores a time and an offset.
interval        		| Period                |                               | This is a human interval which does not have a fixed absolute length ("two months" can vary depending on the months in question), and so it is mapped to NodaTime's Period (and not Duration or TimeSpan).

## Additional Notes

* The plugin automatically converts `timestamp with time zone` to and from your PostgreSQL session's configured timezone; this is unlike Npgsql's default mapping which uses your machine's local timezone instead. The NodaTime plugin behavior matches the regular PostgreSQL behavior when interacting with `timestamptz` values.
* To read and write `timestamp` or `date` infinity values, set the `Convert Infinity DateTime` connection string parameter to true and read/write MaxValue/MinValue.
