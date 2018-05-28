# Supported Types and their Mappings

The following lists the built-in mappings when reading and writing CLR types to PostgreSQL types.

Note that in addition to the below, enum and composite mappings are documented [in a separate page](enums_and_composites.md). Note also that several plugins exist to add support for more mappings (e.g. spatial support for PostGIS), these are listed in the Types menu.

## Read mappings

The following shows the mappings used when reading values.
* The default type is returned when using `NpgsqlCommand.ExecuteScalar()`, `NpgsqlDataReader.GetValue()` and similar methods.
* You can read as other types by calling `NpgsqlDataReader.GetFieldValue<T>()`.
* Provider-specific types are returne by `NpgsqlDataReader.GetProviderSpecificValue()`.

PostgreSQL type			| Default .NET type		| Provider-specific type	| Other .NET types
--------------------------------|-------------------------------|-------------------------------|-----------------
boolean				| bool				|				|
smallint			| short				|				| byte, sbyte, int, long, float, double, decimal
integer				| int				|				| byte, short, long, float, double, decimal
bigint				| long				|				| long, byte, short, int, float, double, decimal
real				| float				|				| double
double precision		| double			|				|
numeric				| decimal			|				| byte, short, int, long, float, double
money				| decimal			|				|
text				| string			|				| char[]
character varying		| string			|				| char[]
character			| string			|				| char[]
citext				| string			|				| char[]
json				| string			|				| char[]
jsonb				| string			|				| char[]
xml				| string			|				| char[]
point				| NpgsqlPoint			|				|
lseg				| NpgsqlLSeg			|				|
path				| NpgsqlPath			|				|
polygon				| NpgsqlPolygon			|				|
line				| NpgsqlLine			|				|
circle				| NpgsqlCircle			|				|
box				| NpgsqlBox			|				|
bit(1)				| bool				|				| BitArray
bit(n)				| BitArray			|				|
bit varying			| BitArray			|				|
hstore				| IDictionary<string, string>	|				|
uuid				| Guid				|				|
cidr				| ValueTuple<IPAddress, int>	|				| NpgsqlInet
inet				| IPAddress			| ValueTuple<IPAddress, int>	| NpgsqlInet
macaddr				| PhysicalAddress		|				|
tsquery				| NpgsqlTsQuery			|				|
tsvector			| NpgsqlTsVector		|				|
date				| DateTime			| NpgsqlDate			|
interval			| TimeSpan			| NpgsqlTimeSpan		|
timestamp			| DateTime			| NpgsqlDateTime		|
timestamp with time zone	| DateTime			| NpgsqlDateTime		| DateTimeOffset
time				| TimeSpan			|				|
time with time zone		| DateTimeOffset		|				| DateTimeOffset, DateTime, TimeSpan
bytea				| byte[]			|				|
oid				| uint				|				|
xid				| uint				|				|
cid				| uint				|				|
oidvector			| uint[]			|				|
name				| string			|				| char[]
(internal) char			| char				|				| byte, short, int, long
geometry (PostGIS)		| PostgisGeometry		|				|
record				| object[]			|				|
composite types			| T				|				|
range subtypes			| NpgsqlRange\<TElement>	|				|
enum types			| TEnum				|				|
array types			| Array	(of child element type)	|				|

The Default .NET type column specifies the data type `NpgsqlDataReader.GetValue()` will return.

`NpgsqlDataReader.GetProviderSpecificValue` will return a value of a data type specified in the Provider-specific type column, or the Default .NET type if there is no specialization.

Finally, the third column specifies other CLR types which Npgsql supports for the PostgreSQL data type. These can be retrieved by calling `NpgsqlDataReader.GetBoolean()`, `GetByte()`, `GetDouble()` etc. or via `GetFieldValue<T>()`.

## Write mappings

There are three rules that determine the PostgreSQL type sent for a parameter:

1. If the parameter's `NpgsqlDbType` is set, it is used.
2. If the parameter's `DataType` is set, it is used.
3. If the parameter's `DbType` is set, it is used.
4. If none of the above is set, the backend type will be inferred from the CLR value type.

Note that for `DateTime` and `NpgsqlDateTime`, the `Kind` attribute determines whether to use `timestamp` or `timestamptz`.

NpgsqlDbType	| DbType		| PostgreSQL type		| Accepted .NET types
----------------|-----------------------|-------------------------------|--------------------
Boolean		| Boolean		| boolean			| bool
Smallint	| Int16			| smallint			| short
Integer		| Int32			| integer			| int
Bigint		| Int64			| bigint			| long
Real		| Single		| real				| float
Double		| Double		| double precision		| double
Numeric		| Decimal, VarNumeric	| numeric			| decimal
Money		| Currency		| money				| decimal
Text		| String, StringFixedLength, AnsiString, AnsiStringFixedLength	| text	| string, char[], char
Varchar		| 			| character varying		| string, char[], char
Char		|			| character			| string, char[], char
Citext		|			| citext			| string, char[], char
Json		|			| json				| string, char[], char
Jsonb		|			| jsonb				| string, char[], char
Xml		|			| xml				| string, char[], char
Point		|			| point				| NpgsqlPoint
LSeg		|			| lseg				| NpgsqlLSeg
Path		|			| path				| NpgsqlPath
Polygon		|			| polygon			| NpgsqlPolygon
Line		|			| line				| NpgsqlLine
Circle		|			| circle			| NpgsqlCircle
Box		|			| box				| NpgsqlBox
Bit		|			| bit				| BitArray, bool, string
Varbit		|			| bit varying			| BitArray, bool, string
Hstore		|			| hstore			| IDictionary<string, string>
Uuid		|			| uuid				| Guid
Cidr		|			| cidr				| ValueTuple<IPAddress, int>, IPAddress, NpgsqlInet
Inet		|			| inet				| ValueTuple<IPAddress, int>, IPAddress, NpgsqlInet
MacAddr		|			| macaddr			| PhysicalAddress
TsQuery		|			| tsquery			| NpgsqlTsQuery
TsVector	|			| tsvector			| NpgsqlTsVector
Date		| Date			| date				| DateTime, NpgsqlDate
Interval	|			| interval			| TimeSpan, NpgsqlTimeSpan
Timestamp	| DateTime, DateTime2	| timestamp			| DateTime, DateTimeOffset, NpgsqlDateTime
TimestampTz	| DateTimeOffset	| timestamp with time zone	| DateTime, DateTimeOffset, NpgsqlDateTime
Time		| Time			| time				| TimeSpan
TimeTz		|			| time with time zone		| DateTimeOffset, DateTime, TimeSpan
Bytea		| Binary		| bytea				| byte[], ArraySegment\<byte>
Oid		|			| oid				| uint
Xid		|			| xid				| uint
Cid		|			| cid				| uint
Oidvector	|			| oidvector			| uint[]
Name		|			| name				| string, char[], char
InternalChar	|			| (internal) char		| byte
Composite	|			| composite types		| T
Range \| (other NpgsqlDbType) |		| range types			| NpgsqlRange\<TElement>
Enum		|			| enum types			| TEnum
Array \| (other NpgsqlDbType) | 	| array types			| Array, IList\<T>, IList

Notes when using Range and Array, bitwise-or NpgsqlDbType.Range or NpgsqlDbType.Array with the child type. For example, to construct the NpgsqlDbType for a `int4range`, write `NpgsqlDbType.Range | NpgsqlDbType.Integer`. To construct the NpgsqlDbType for an `int[]`, write `NpgsqlDbType.Array | NpgsqlDbType.Integer`.

For information about enums, [see the Enums and Composites page](enums_and_composites.md).

| .NET type					| Auto-inferred PostgreSQL type
|-----------------------------------------------|------------------------------
| bool						| boolean
| byte						| smallint
| sbyte						| smallint
| short						| smallint
| int						| integer
| long						| bigint
| float						| real
| double					| double precision
| decimal					| numeric
| string					| text
| char[]					| text
| char						| text
| NpgsqlPoint					| point
| NpgsqlLSeg					| lseg
| NpgsqlPath					| path
| NpgsqlPolygon					| polygon
| NpgsqlLine					| line
| NpgsqlCircle					| circle
| NpgsqlBox					| box
| BitArray					| bit varying
| Guid						| uuid
| IPAddress					| inet
| NpgsqlInet					| inet
| PhysicalAddress				| macaddr
| NpgsqlTsQuery					| tsquery
| NpgsqlTsVector				| tsvector
| NpgsqlDate					| date
| NpgsqlDateTime				| timestamp
| DateTime					| timestamp
| DateTimeOffset				| timestamp with time zone
| TimeSpan					| time
| byte[]					| bytea
| Custom composite type				| composite types
| NpgsqlRange\<TElement>			| range types
| Enum types					| enum types
| Array types					| array types
