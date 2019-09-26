# Date and Time Handling

> [!Note]
> Since 4.0 the recommended way of working with date/time types is [the NodaTime plugin](nodatime.md).

Handling date and time values usually isn't hard, but you must pay careful attention to differences in how the .NET types and PostgreSQL represent dates.
It's worth reading the [PostgreSQL date/time type documentation](http://www.postgresql.org/docs/current/static/datatype-datetime.html) to familiarize
yourself with PostgreSQL's types.

## .NET types and PostgreSQL types

> [!Warning]
> A common mistake is for users to think that the PostgreSQL `timestamp with timezone` type stores the timezone in the database. This is not the case: only the timestamp is stored. There is no single PostgreSQL type that stores both a date/time and a timezone, similar to [.NET DateTimeOffset](https://msdn.microsoft.com/en-us/library/system.datetimeoffset(v=vs.110).aspx).

The .NET and PostgreSQL types differ in the resolution and range they provide; the .NET type usually have a higher resolution but a lower range than the PostgreSQL types:

PostgreSQL type         | Precision/Range                           | .NET Native Type             | Precision/Range                                | Npgsql .NET Provider-Specific Type
------------------------|-------------------------------------------|------------------------------|------------------------------------------------|-----------------------------------
timestamp               | 1 microsecond, 4713BC-294276AD            | DateTime                     | 100 nanoseconds, 1AD-9999AD                    | NpgsqlDateTime
timestamp with timezone | 1 microsecond, 4713BC-294276AD            | DateTime                     | 100 nanoseconds, 1AD-9999AD                    | NpgsqlDateTime
date                    | 1 day, 4713BC-5874897AD                   | DateTime                     | 100 nanoseconds, 1AD-9999AD                    | NpgsqlDate
time                    | 1 microsecond, 0-24 hours                 | TimeSpan                     | 100 nanoseconds, -10,675,199 - 10,675,199 days | N/A
time with timezone      | 1 microsecond, 0-24 hours                 | DateTimeOffset (ignore date) | 100 nanoseconds, 1AD-9999AD                    | N/A
interval                | 1 microsecond, -178000000-178000000 years | TimeSpan                     | 100 nanoseconds, -10,675,199 - 10,675,199 days | NpgsqlTimeSpan

If your needs are met by the .NET native types, it is best that you use them directly with Npgsql.
If, however, you require the extended range of a PostgreSQL type you can use Npgsql's provider-specific types, which represent PostgreSQL types in an exact way.

## Timezones

It's critical to understand exactly how timezones and timezone conversions are handled between .NET types and PostgreSQL.
In particular, .NET's DateTime has a [Kind](https://msdn.microsoft.com/en-us/library/system.datetime.kind(v=vs.110).aspx) property which impacts how
Npgsql reads and writes the value.

By default, `DateTime` is sent to PostgreSQL as a `timestamp without time zone` - no timezone conversion of any kind will occur, and your `DateTime` instance will be transferred as-is to PostgreSQL. This is the recommended way to store timestamps in the database. Note that you may still send `DateTime` as `timestamp with time zone` by setting `NpgsqlDbType.TimestampTz` on your `NpgsqlParameter`; in this case, if the `Kind` is `Local`, Npgsql will convert the value to UTC before sending it to PostgreSQL. Otherwise, it will be sent as-is.

You can also send `DateTimeOffset` values, which are written as `timestamptz` and are converted to UTC before sending.

PostgreSQL `time with time zone` is the only date/time type which actually stores a timezone in the database. You can use a `DateTimeOffset` to send one to PostgreSQL, in which case the date component is dropped and the time and timezone are preserved. You can also send a `DateTime`, in which case the `Kind` will determine the the timezone sent to the database.

## Detailed Behavior: Sending values to the database

.NET value                     | NpgsqlDbType                       | Action
-------------------------------|------------------------------------|--------------------------------------------------
DateTime                       | NpgsqlDbType.Timestamp (default)   | Send as-is
DateTime(Kind=UTC,Unspecified) | NpgsqlDbType.TimestampTz           | Send as-is
DateTime(Kind=Local)           | NpgsqlDbType.TimestampTz           | Convert to UTC locally before sending
                               |                                    |
DateTimeOffset                 | NpgsqlDbType.TimestampTz (default) | Convert to UTC locally before sending
                               |                                    |
TimeSpan                       | NpgsqlDbType.Time (default)        | Send as-is
                               |                                    |
DateTimeOffset                 | NpgsqlDbType.TimeTz                | Send time and timezone
DateTime(Kind=UTC)             | NpgsqlDbType.TimeTz                | Send time and UTC timezone
DateTime(Kind=Local)           | NpgsqlDbType.TimeTz                | Send time and local system timezone
DateTime(Kind=Unspecified)     | NpgsqlDbType.TimeTz                | Assume local, send time and local system timezone

## Detailed Behavior: Reading values from the database

PG type     | .NET value               | Action
------------|--------------------------|--------------------------------------------------
timestamp   | DateTime (default)       | Kind=Unspecified
            |                          |
timestamptz | DateTime (default)       | Kind=Local (according to system timezone)
timestamptz | DateTimeOffset           | In local timezone offset
            |                          |
time        | TimeSpan (default)       | As-is
            |                          |
timetz      | DateTimeOffset (default) | Date component is empty
timetz      | TimeSpan                 | Strip offset, read as-is
timetz      | DateTime                 | Strip offset, date is empty

## Further Reading

If you're really interested in some of the mapping decisions above, check out [this issue](https://github.com/npgsql/npgsql/issues/347).

