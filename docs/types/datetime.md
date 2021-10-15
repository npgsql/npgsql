# Date and Time Handling

> [!WARNING]
> Npgsql 6.0 introduced some important changes to how timestamps are mapped, [see the release notes for more information](../release-notes/6.0.html).

> [!NOTE]
> The recommended way of working with date/time types is [the NodaTime plugin](nodatime.md): the NodaTime types are much better-designed, avoid the flaws in the built-in BCL types, and are fully supported by Npgsql.

Handling date and time values usually isn't hard, but you must pay careful attention to differences in how the .NET types and PostgreSQL represent dates. It's worth reading the [PostgreSQL date/time type documentation](http://www.postgresql.org/docs/current/static/datatype-datetime.html) to familiarize yourself with PostgreSQL's types.

## .NET types and PostgreSQL types

The .NET and PostgreSQL types differ in the resolution and range they provide; the .NET type usually have a higher resolution but a lower range than the PostgreSQL types:

PostgreSQL type             | Precision/Range                           | .NET Native Type             | Precision/Range
----------------------------|-------------------------------------------|------------------------------|----------------
timestamp without time zone | 1 microsecond, 4713BC-294276AD            | DateTime                     | 100 nanoseconds, 1AD-9999AD
timestamp with time zone    | 1 microsecond, 4713BC-294276AD            | DateTime                     | 100 nanoseconds, 1AD-9999AD
date                        | 1 day, 4713BC-5874897AD                   | DateOnly (6.0+), DateTime    | 100 nanoseconds, 1AD-9999AD
time without time zone      | 1 microsecond, 0-24 hours                 | TimeOnly (6.0+), TimeSpan    | 100 nanoseconds, -10,675,199 - 10,675,199 days
time with time zone         | 1 microsecond, 0-24 hours                 | DateTimeOffset (ignore date) | 100 nanoseconds, 1AD-9999AD
interval                    | 1 microsecond, -178000000-178000000 years | TimeSpan                     | 100 nanoseconds, -10,675,199 - 10,675,199 days

For almost all applications, the range of the .NET native types (or the NodaTime types) are more than sufficient. In the rare cases where you need to access values outside these ranges, timestamps can be accessed as `long`, dates as `int`, and intervals as `NpgsqlInterval`. These are the raw PostgreSQL binary representations of these type, so you'll have to deal with encoding/decoding yourself.

## Timestamps and timezones

> [!Warning]
> A common mistake is for users to think that the PostgreSQL `timestamp with time zone` type stores the timezone in the database. This is not the case: only a UTC timestamp is stored. There is no single PostgreSQL type that stores both a date/time and a timezone, similar to [.NET DateTimeOffset](https://msdn.microsoft.com/en-us/library/system.datetimeoffset(v=vs.110).aspx). To store a timezone in the database, add a separate text column containing the timezone ID.

In PostgreSQL, `timestamp with time zone` represents a UTC timestamp, while `timestamp without time zone` represents a local or unspecified time zone. Starting with 6.0, Npgsql maps UTC DateTime to `timestamp with time zone`, and Local/Unspecified DateTime to `timestamp without time zone`; trying to send a non-UTC DateTime as `timestamptz` will throw an exception, etc. Npgsql also supports reading and writing DateTimeOffset to `timestamp with time zone`, but only with Offset=0. Prior to 6.0, `timestamp with time zone` would be converted to a local timestamp when read - see below for more details. The precise improvements and breaking changes are detailed in the [6.0 breaking changes](../release-notes/6.0.html#timestamp-rationalization-and-improvements); to revert to the pre-6.0 behavior, add the following at the start of your application, before any Npgsql operations are invoked:

```c#
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

Use of the `time with time zone` type is discouraged, [see the PostgreSQL documentation](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-TIMEZONES). You can use a `DateTimeOffset` to read and write values - the date component will be ignored.

## Infinity values

PostgreSQL supports the special values `-infinity` and `infinity` for the timestamp and date types ([see docs](https://www.postgresql.org/docs/current/datatype-datetime.html#DATATYPE-DATETIME-SPECIAL-VALUES)); these can be useful to represent a value which is earlier or later than any other value. Starting with Npgsql 6.0, these special values are mapped to the `MinValue` and `MaxValue` value on the corresponding .NET types (`DateTime` and `DateOnly`, NodaTime `Instant` and `LocalDate`). To opt out of this behavior, set the following AppContext switch at the start of your application:

```c#
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
```

Note: in versions prior to 6.0, the connection string parameter `Convert Infinity DateTime` could be used to opt into these infinity conversions. That connection string parameter has been removed.

## Detailed Behavior: Reading values from the database

PostgreSQL type             | Default .NET type          | Non-default .NET types
--------------------------- | -------------------------- | ----------------------
timestamp without time zone | DateTime (Unspecified)     |
timestamp with time zone    | DateTime (Utc<sup>1</sup>) | DateTimeOffset (Offset=0)<sup>2</sup>
date                        | DateTime                   | DateOnly (6.0+)
time without time zone      | TimeSpan                   | TimeOnly (6.0+)
time with time zone         | DateTimeOffset             |
interval                    | TimeSpan                   |

<sup>1</sup> In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), reading a `timestamp with time zone` returns a Local DateTime instead of Utc. [See the breaking change note for more info](../release-notes/6.0.html#major-changes-to-timestamp-mapping).

<sup>2</sup> In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), reading a `timestamp with time zone` as a DateTimeOffset returns a local offset based on the timezone of the server where Npgsql is running.

## Detailed Behavior: Sending values to the database

PostgreSQL type             | Default .NET types                         | Non-default .NET types                  | NpgsqlDbType          | DbType
--------------------------- | ------------------------------------------ | --------------------------------------- | --------------------- | ------
timestamp without time zone | DateTime (Local/Unspecified)<sup>1</sup>   |                                         | Timestamp             | DateTime, DateTime2
timestamp with time zone    | DateTime (Utc)<sup>1</sup>, DateTimeOffset |                                         | TimestampTz           | DateTimeOffset
date                        | DateOnly (6.0+)                            | DateTime                                | Date                  | Date
time without time zone      | TimeOnly (6.0+)                            | TimeSpan                                | Time                  | Time
time with time zone         |                                            | DateTimeOffset                          | TimeTz                |
interval                    | TimeSpan                                   |                                         | Interval              |

<sup>1</sup> UTC DateTime is written as `timestamp with time zone`, Local/Unspecified DateTimes are written as `timestamp without time zone`. In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), DateTime is always written as `timestamp without time zone`.
