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

For PostgreSQL `timestamp without time zone` and time (without time zone), the database value's timezone is unknown or undefined.
Therefore, Npgsql does no timezone conversion whatsoever and always sends your DateTime as-is, regardless of its Kind.
For DateTimeOffset, the timestamp component is sent as-is (i.e. the timezone component is discarded).

For PostgreSQL `timestamp with time zone`, the database value's timezone is expected to always be UTC (the timezone isn't saved in the database).
As a result, a DateTime Offset as well as a DateTime with Kind=Local Npgsql will be converted to UTC before being sent to the database.
When reading a timestamptz, the database UTC timestamp will be returned as a DateTime with Kind=Local or a DateTimeOffset in the local timezone (*UNIMPLEMENTED?*).

PostgreSQL `time with time zone` is the only date/time type which stores a timezone in the database.
Accordingly, your DateTime's Kind will determine the the timezone sent to the database.

## Detailed Behavior: Sending values to the database

.NET value                     | PG type               | Action
-------------------------------|-----------------------|--------------------------------------------------
DateTime                       | timestamp (default)   | Send as-is
                               |                       |
DateTime(Kind=UTC,Unspecified) | timestamptz           | Send as-is
DateTime(Kind=Local)           | timestamptz           | Convert to UTC locally before sending
DateTimeOffset                 | timestamptz (default) | Convert to UTC locally before sending
                               |                       |
TimeSpan                       | time                  | Send as-is
                               |                       |
DateTime(Kind=UTC)             | timetz                | Send time and UTC timezone
DateTime(Kind=Local)           | timetz                | Send time and local system timezone
DateTime(Kind=Unspecified)     | timetz                | Assume local, send time and local system timezone
DateTimeOffset                 | timetz                | Send time and timezone

## Detailed Behavior: Reading values from the database

PG type     | .NET value               | Action
------------|--------------------------|--------------------------------------------------
timestamp   | DateTime (default)       | Kind=Unspecified
            |                          |
timestamptz | DateTime (default)       | Kind=Local (according to system timezone)
timestamptz | DateTimeOffset           | In local timezone offset
            |                          |
time        | TimeSpan (default)       | As-is
time        | DateTime                 | **Use only time component**
time        | DateTimeOffset           | **Exception?**
            |                          |
timetz      | TimeSpan                 | Strip offset, read as-is
timetz      | DateTime                 | **Use only time component, throw away time zone**
timetz      | DateTimeOffset (default) | **Use only time- and time zone component**

## Further Reading

If you're really interested in some of the mapping decisions above, check out [this issue](https://github.com/npgsql/npgsql/issues/347).

