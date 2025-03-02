# Supported Types and their Mappings

The following lists the built-in mappings when reading and writing CLR types to PostgreSQL types.

Note that in addition to the below, enum and composite mappings are documented [in a separate page](enums_and_composites.md). Note also that several plugins exist to add support for more mappings (e.g. spatial support for PostGIS), these are listed in the Types menu.

## Read mappings

The following shows the mappings used when reading values.

* The default type is returned when using `NpgsqlCommand.ExecuteScalar()`, `NpgsqlDataReader.GetValue()` and similar methods.
* You can read as other types by calling `NpgsqlDataReader.GetFieldValue<T>()`.

PostgreSQL type             | Default .NET type          | Non-default .NET types
--------------------------- | -------------------------- | ----------------------
boolean                     | bool                       |
smallint                    | short                      | byte, sbyte, int, long, float, double, decimal
integer                     | int                        | byte, short, long, float, double, decimal
bigint                      | long                       | long, byte, short, int, float, double, decimal
real                        | float                      | double
double precision            | double                     |
numeric                     | decimal                    | byte, short, int, long, float, double, BigInteger (6.0+)
money                       | decimal                    |
text                        | string                     | char[], byte[]<sup>1</sup>
character varying           | string                     | char[], byte[]<sup>1</sup>
character                   | string                     | char[], byte[]<sup>1</sup>
citext                      | string                     | char[], byte[]<sup>1</sup>
json                        | string                     | char[], byte[]<sup>1</sup>
jsonb                       | string                     | char[], byte[]<sup>1</sup>
xml                         | string                     | char[], byte[]<sup>1</sup>
uuid                        | Guid                       |
bytea                       | byte[]<sup>1</sup>         |
timestamp without time zone | DateTime (Unspecified)     |
timestamp with time zone    | DateTime (Utc<sup>2</sup>) | DateTimeOffset (Offset=0)<sup>2</sup>
date                        | DateTime                   | DateOnly (6.0+)
time without time zone      | TimeSpan                   | TimeOnly (6.0+)
time with time zone         | DateTimeOffset             |
interval                    | TimeSpan<sup>3</sup>       | <xref:NpgsqlTypes.NpgsqlInterval>
cidr                        | <xref:NpgsqlTypes.NpgsqlCidr><sup>4</sup> |
inet                        | IPAddress                  | <xref:NpgsqlTypes.NpgsqlInet>
macaddr                     | PhysicalAddress            |
tsquery                     | NpgsqlTsQuery              |
tsvector                    | NpgsqlTsVector             |
bit(1)                      | bool                       | BitArray
bit(n)                      | BitArray                   |
bit varying                 | BitArray                   |
point                       | NpgsqlPoint                |
lseg                        | NpgsqlLSeg                 |
path                        | NpgsqlPath                 |
polygon                     | NpgsqlPolygon              |
line                        | NpgsqlLine                 |
circle                      | NpgsqlCircle               |
box                         | NpgsqlBox                  |
hstore                      | Dictionary<string, string> |
oid                         | uint                       |
xid                         | uint                       |
cid                         | uint                       |
oidvector                   | uint[]                     |
name                        | string                     | char[]
(internal) char             | char                       | byte, short, int, long
geometry (PostGIS)          | PostgisGeometry            |
record                      | object[]                   |
composite types             | T                          |
range types                 | NpgsqlRange\<TElement>     |
multirange types (PG14)     | NpgsqlRange\<TElement>[]   |
enum types                  | TEnum                      |
array types                 | Array (of element type)    |

<sup>1</sup>Types which can be read as `byte[]` can also be written as `ReadOnlyMemory<byte>` and `Stream`., and if they could be big, consider using `Stream`CommandBehavior.SequentialAccess` and `NpgsqlDataReader.GetStream()` to stream the row and column instead of buffering them. Note that if you're reading string data (e.g. `jsonb`) as binary, it's you're responsibility to perform UTF8 decoding (or whatever encoding is configured for your database).

<sup>2</sup> In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), reading a `timestamp with time zone` returns a Local DateTime instead of Utc, and reading it as a DateTimeOffset returns a local offset based on the timezone of the server where Npgsql is running. [See the breaking change note for more info](../release-notes/6.0.md#major-changes-to-timestamp-mapping).

<sup>3</sup> PostgreSQL intervals with month or year components cannot be read as `TimeSpan`. Consider using NodaTime's [Period](https://nodatime.org/3.0.x/api/NodaTime.Period.html) type, or <xref:NpgsqlTypes.NpgsqlInterval>.

<sup>4</sup> Prior to version 8.0, the default mapping for `cidr` was `ValueTuple<IPAddress, int>`.

## Write mappings

There are three rules that determine the PostgreSQL type sent for a parameter:

1. If the parameter's `NpgsqlDbType` is set, it is used.
2. If the parameter's `DataType` is set, it is used.
3. If the parameter's `DbType` is set, it is used.
4. If none of the above is set, the backend type will be inferred from the CLR value type.

PostgreSQL type             | Default .NET types                         | Non-default .NET types                   | NpgsqlDbType          | DbType
--------------------------- | ------------------------------------------ | ---------------------------------------- | --------------------- | ------
boolean                     | bool                                       |                                          | Boolean               | Boolean
smallint                    | short, byte, sbyte                         |                                          | Smallint              | Int16
integer                     | int                                        |                                          | Integer               | Int32
bigint                      | long                                       |                                          | Bigint                | Int64
real                        | float                                      |                                          | Real                  | Single
double precision            | double                                     |                                          | Double                | Double
numeric                     | decimal, BigInteger (6.0+)                 |                                          | Numeric               | Decimal, VarNumeric
money                       |                                            | decimal                                  | Money                 | Currency
text                        | string, char[], char                       | byte[], ReadOnlyMemory\<byte\>           | Text                  | String, StringFixedLength, AnsiString, AnsiStringFixedLength
character varying           |                                            | string, char[], char, byte[]<sup>3</sup> | Varchar               |
character                   |                                            | string, char[], char, byte[]<sup>3</sup> | Char                  |
citext                      |                                            | string, char[], char, byte[]<sup>3</sup> | Citext                |
json                        |                                            | string, char[], char, byte[]<sup>3</sup> | Json                  |
jsonb                       |                                            | string, char[], char, byte[]<sup>3</sup> | Jsonb                 |
xml                         |                                            | string, char[], char, byte[]<sup>3</sup> | Xml                   |
uuid                        | Guid                                       |                                          | Uuid                  |
bytea                       | byte[]                                     | ArraySegment\<byte\>, Stream (7.0+)      | Bytea                 | Binary
timestamp with time zone    | DateTime (Utc)<sup>1</sup>, DateTimeOffset |                                          | TimestampTz           | DateTime<sup>2</sup>, DateTimeOffset
timestamp without time zone | DateTime (Local/Unspecified)<sup>1</sup>   |                                          | Timestamp             | DateTime2
date                        | DateOnly (6.0+)                            | DateTime                                 | Date                  | Date
time without time zone      | TimeOnly (6.0+)                            | TimeSpan                                 | Time                  | Time
time with time zone         |                                            | DateTimeOffset                           | TimeTz                |
interval                    | TimeSpan                                   | <xref:NpgsqlTypes.NpgsqlInterval>        | Interval              |
cidr                        |                                            | ValueTuple\<IPAddress, int\>, IPAddress  | Cidr                  |
inet                        | IPAddress                                  | ValueTuple\<IPAddress, int\>             | Inet                  |
macaddr                     | PhysicalAddress                            |                                          | MacAddr               |
tsquery                     | NpgsqlTsQuery                              |                                          | TsQuery               |
tsvector                    | NpgsqlTsVector                             |                                          | TsVector              |
bit                         |                                            | bool, BitArray, string                   | Bit                   |
bit varying                 | BitArray                                   | bool, BitArray, string                   | Varbit                |
point                       | NpgsqlPoint                                |                                          | Point                 |
lseg                        | NpgsqlLSeg                                 |                                          | LSeg                  |
path                        | NpgsqlPath                                 |                                          | Path                  |
polygon                     | NpgsqlPolygon                              |                                          | Polygon               |
line                        | NpgsqlLine                                 |                                          | Line                  |
circle                      | NpgsqlCircle                               |                                          | Circle                |
box                         | NpgsqlBox                                  |                                          | Box                   |
hstore                      | IDictionary\<string, string\>              |                                          | Hstore                |
oid                         |                                            | uint                                     | Oid                   |
xid                         |                                            | uint                                     | Xid                   |
cid                         |                                            | uint                                     | Cid                   |
oidvector                   |                                            | uint[]                                   | Oidvector             |
name                        |                                            | string, char[], char                     | Name                  |
(internal) char             |                                            | byte                                     | InternalChar          |
composite types             | Pre-mapped type                            |                                          | Composite             |
range types                 | NpgsqlRange\<TSubtype\>                    |                                          | Range \| NpgsqlDbType |
enum types                  | Pre-mapped type                            |                                          | Enum                  |
array types                 | T[], List\<T\>                             |                                          | Array \| NpgsqlDbType |

<sup>1</sup> UTC DateTime is written as `timestamp with time zone`, Local/Unspecified DateTimes are written as `timestamp without time zone`. In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), DateTime is always written as `timestamp without time zone`.

<sup>2</sup>In versions prior to 6.0 (or when `Npgsql.EnableLegacyTimestampBehavior` is enabled), `DbType.DateTime` is mapped to `timestamp without time zone`.

<sup>3</sup>Types which can be read as `byte[]` can also be written as `ReadOnlyMemory<byte>` and `Stream`.

Notes when using Range and Array, bitwise-or NpgsqlDbType.Range or NpgsqlDbType.Array with the child type. For example, to construct the NpgsqlDbType for a `int4range`, write `NpgsqlDbType.Range | NpgsqlDbType.Integer`. To construct the NpgsqlDbType for an `int[]`, write `NpgsqlDbType.Array | NpgsqlDbType.Integer`.

For information about enums, [see the Enums and Composites page](enums_and_composites.md).
