# Supported Types and their Mappings

The following lists the built-in mappings when reading and writing CLR types to PostgreSQL types.

Note that in addition to the below, enum and composite mappings are documented [in a separate page](enums_and_composites.md). Note also that several plugins exist to add support for more mappings (e.g. spatial support for PostGIS), these are listed in the Types menu.

## Read mappings

The following shows the mappings used when reading values.
* The default type is returned when using `NpgsqlCommand.ExecuteScalar()`, `NpgsqlDataReader.GetValue()` and similar methods.
* You can read as other types by calling `NpgsqlDataReader.GetFieldValue<T>()`.
* Provider-specific types are returne by `NpgsqlDataReader.GetProviderSpecificValue()`.

PostgreSQL type         | Default .NET type             | Provider-specific type	| Other .NET types
------------------------|-------------------------------|-------------------------------|-----------------
bool			| bool				|				|
int2			| short				|				| byte, sbyte, int, long, float, double, decimal
int4			| int				|				| byte, short, long, float, double, decimal
int8			| long				|				| long, byte, short, int, float, double, decimal
float4			| float				|				| double
float8			| double			|				|
numeric			| decimal			|				| byte, short, int, long, float, double
money			| decimal			|				|
text			| string			|				| char[]
varchar			| string			|				| char[]
bpchar			| string			|				| char[]
citext			| string			|				| char[]
json			| string			|				| char[]
jsonb			| string			|				| char[]
xml			| string			|				| char[]
point			| NpgsqlPoint			|				| string
lseg			| NpgsqlLSeg			|				| string
path			| NpgsqlPath			|				|
polygon			| NpgsqlPolygon			|				|
line			| NpgsqlLine			|				| string
circle			| NpgsqlCircle			|				| string
box			| NpgsqlBox			|				| string
bit(1)			| bool				|				| BitArray
bit(n)			| BitArray			|				|
varbit			| BitArray			|				|
hstore			| IDictionary<string, string>	|				| string
uuid			| Guid				|				| string
cidr			| NpgsqlInet			|				| string
inet			| IPAddress			| NpgsqlInet			| string
macaddr			| PhysicalAddress		|				| string
tsquery			| NpgsqlTsQuery			|				|
tsvector		| NpgsqlTsVector		|				|
date			| DateTime			| NpgsqlDate			|
interval		| TimeSpan			| NpgsqlTimeSpan		|
timestamp		| DateTime			| NpgsqlDateTime		|
timestamptz		| DateTime			| NpgsqlDateTime		| DateTimeOffset
time			| TimeSpan			|				|
timetz			| DateTimeOffset		|				| DateTimeOffset, DateTime, TimeSpan
bytea			| byte[]			|				|
oid			| uint				|				|
xid			| uint				|				|
cid			| uint				|				|
oidvector		| uint[]			|				|
name			| string			|				| char[]
(internal) char		| char				|				| byte, short, int, long
geometry (PostGIS)	| PostgisGeometry		|				|
record			| object[]			|				|
composite types		| T				|				|
range subtypes		| NpgsqlRange<TElement>		|				|
enum types		| TEnum				|				|
array types		| Array	(of child element type)	|				|

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

NpgsqlDbType	| DbType		| PostgreSQL type	| Accepted .NET types
----------------|-----------------------|-----------------------|--------------------
Boolean		| Boolean		| bool			| bool, IConvertible
Smallint	| Int16			| int2			| short, IConvertible
Integer		| Int32			| int4			| int, IConvertible
Bigint		| Int64			| int8			| long, IConvertible
Real		| Single		| float4		| float, IConvertible
Double		| Double		| float8		| double, IConvertible
Numeric		| Decimal, VarNumeric	| numeric		| decimal, IConvertible
Money		| Currency		| money			| decimal, IConvertible
Text		| String, StringFixedLength, AnsiString, AnsiStringFixedLength	| text	| string, char[], char, IConvertible
Varchar		| 			| varchar		| string, char[], char, IConvertible
Char		|			| char			| string, char[], char, IConvertible
Citext		|			| citext		| string, char[], char, IConvertible
Json		|			| json			| string, char[], char, IConvertible
Jsonb		|			| jsonb			| string, char[], char, IConvertible
Xml		|			| xml			| string, char[], char, IConvertible
Point		|			| point			| NpgsqlPoint
LSeg		|			| lseg			| NpgsqlLSeg
Path		|			| path			| NpgsqlPath
Polygon		|			| polygon		| NpgsqlPolygon
Line		|			| line			| NpgsqlLine
Circle		|			| circle		| NpgsqlCircle
Box		|			| box			| NpgsqlBox
Bit		|			| bit			| BitArray, bool, string
Varbit		|			| varbit		| BitArray, bool, string
Hstore		|			| hstore		| IDictionary<string, string>
Uuid		|			| uuid			| Guid, string
Cidr		|			| cidr			| IPAddress, NpgsqlInet
Inet		|			| inet			| IPAddress, NpgsqlInet
MacAddr		|			| macaddr		| PhysicalAddress
TsQuery		|			| tsquery		| NpgsqlTsQuery
TsVector	|			| tsvector		| NpgsqlTsVector
Date		| Date			| date			| DateTime, NpgsqlDate, IConvertible
Interval	|			| interval		| TimeSpan, NpgsqlTimeSpan, string
Timestamp	| DateTime, DateTime2	| timestamp		| DateTime, DateTimeOffset, NpgsqlDateTime, IConvertible
TimestampTz	| DateTimeOffset	| timestamptz		| DateTime, DateTimeOffset, NpgsqlDateTime, IConvertible
Time		| Time			| time			| TimeSpan, string
TimeTz		|			| timetz		| DateTimeOffset, DateTime, TimeSpan
Bytea		| Binary		| bytea			| byte[], ArraySegment<byte>
Oid		|			| oid			| uint, IConvertible
Xid		|			| xid			| uint, IConvertible
Cid		|			| cid			| uint, IConvertible
Oidvector	|			| oidvector		| uint[]
Name		|			| name			| string, char[], char, IConvertible
InternalChar	|			| (internal) char	| byte, IConvertible
Composite	|			| composite types	| T
Range \| (other NpgsqlDbType) |		| range types		| NpgsqlRange<TElement>
Enum		|			| enum types		| TEnum
Array \| (other NpgsqlDbType) | 	| array types		| Array, IList<T>, IList

Notes when using Range and Array, bitwise-or NpgsqlDbType.Range or NpgsqlDbType.Array with the child type. For example, to construct the NpgsqlDbType for a `int4range`, write `NpgsqlDbType.Range | NpgsqlDbType.Integer`. To construct the NpgsqlDbType for an `int[]`, write `NpgsqlDbType.Array | NpgsqlDbType.Integer`.

For information about enums, [see the Enums and Composites page](enums_and_composites.md).

| .NET type					| Auto-inferred PostgreSQL type
|-----------------------------------------------|------------------------------
| bool						| bool
| byte						| int2
| sbyte						| int2
| short						| int2
| int						| int4
| long						| int8
| float						| float4
| double					| float8
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
| BitArray					| varbit
| Guid						| uuid
| IPAddress					| inet
| NpgsqlInet					| inet
| PhysicalAddress				| macaddr
| NpgsqlTsQuery					| tsquery
| NpgsqlTsVector				| tsvector
| NpgsqlDate					| date
| NpgsqlDateTime				| timestamp
| DateTime					| timestamp
| DateTimeOffset				| timestamptz
| TimeSpan					| time
| byte[]					| bytea
| Custom composite type				| composite types
| NpgsqlRange<TElement>				| range types
| Enum types					| enum types
| Array types					| array types
