---
layout: page
title: PostgreSQL Types
---

## Overview

The following are notes by Emil Lenngren on PostgreSQL wire representation of types:

~~~
bool:
	text: t or f
	binary: a byte: 1 or 0

bytea:
	text:
		either \x followed by hex-characters (lowercase by default),
		or plain characters, where non-printable characters (between 0x20 and 0x7e, inclusive) are written as \nnn (octal) and \ is written as \\
	binary: the bytes as they are

char:
	This type holds a single char/byte. (Not to be confused with bpchar (blank-padded char) which is PostgreSQL's alias to the SQL standard's char).
	The char may be the null-character
	text: the char as a byte, encoding seems to be ignored
	binary: the char as a byte

name:
	A null-padded string of NAMEDATALEN (currently 64) bytes (the last byte must be a null-character). Used in pg catalog.
	text: the name as a string
	binary: the name as a string

int2/int4/int8:
	text: text representation in base 10
	binary: binary version of the integer

int2vector:
	non-null elements, 0-indexed, 1-dim
	text: 1 2 3 4
	binary: same as int2[]

oidvector:
	non-null elements, 0-indexed, 1-dim
	text: 1 2 3 4
	binary: same as oid[]

regproc:
	internally just an OID (UInt32)
	text: -, name of procedure, or numeric if not found
	binary: only the OID in binary

regprocedure/regoper/regoperator/regclass/regconfig/regdictionary:
	similar to regproc

text:
	text: the string as it is
	binary: the string as it is

oid:
	A 32-bit unsigned integer used for internal object identification.
	text: the text-representation of this integer in base 10
	binary: the UInt32

tid:
	tuple id
	Internally a tuple of a BlockNumber (UInt32) and an OffsetNumber (UInt16)
	text: (blockNumber,offsetNumber)
	binary: the block number in binary followed by offset number in binary

xid:
	transaction id
	Internally just a TransactionId (UInt32)
	text: the number
	binary: the number in binary

cid:
	command id
	Internally just a CommandId (UInt32)
	text: the number
	binary: the number in binary

json:
	json
	text: the json an text
	binary: the json as text

jsonb:
	json internally stored in an efficient binary format
	text: the json as text
	binary: An Int32 (version number, currently 1), followed by data (currently just json as text)

xml:
	Xml. It is probably most efficient to use the text format, especially when receiving from client.
	text: the xml as text (when sent from the server: encoding removed, when receiving: assuming database encoding)
	binary: the xml as text (when sent from the server: in the client's specified encoding, when receiving: figures out itself)

pg_node_tree:
	used as type for the column typdefaultbin in pg_type
	does not accept input
	text: text
	binary: text

smgr:
	storage manager
	can only have the value "magnetic disk"
	text: magnetic disk
	binary: not available

point:
	A tuple of two float8
	text:
		(x,y)
		The floats are interpreted with the C strtod function.
		The floats are written with the snprintf function, with %.*g format. NaN/-Inf/+Inf can be written, but not interpretability depends on platform. The extra_float_digits setting is honored.
		For linux, NaN, [+-]Infinity, [+-]Inf works, but not on Windows. Windows also have other output syntax for these special numbers. (1.#QNAN for example)
	binary: the two floats

lseg:
	A tuple of two points
	text: [(x1,y1),(x2,y2)] see point for details
	binary: the four floats in the order x1, y1, x2, y2

path:
	A boolean whether the path is opened or closed + a vector of points.
	text: [(x1,y1),...] for open path and ((x1,y1),...) for closed paths. See point for details.
	binary: first a byte indicating open (0) or close (1), then the number of points (Int32), then a vector of points

box:
	A tuple of two points. The coordinates will be reordered so that the first is the upper right and the second is the lower left.
	text: (x1,y1),(x2,y2) see point for details
	binary: the four floats in the order x1, y1, x2, y2 (doesn't really matter since they will be reordered)

polygon:
	Same as path but with two differences: is always closed and internally stores the bounding box.
	text: same as closed path
	binary: the number of points (Int32), then a vector of points

line (version 9.4):
	Ax + By + C = 0. Stored with three float8.
	Constraint: A and B must not both be zero (only checked on text input, not binary).
	text: {A,B,C} see point for details about the string representation of floats. Can also use the same input format as a path with two different points, representing the line between those.
	binary: the three floats

circle:
	<(x,y),r> (center point and radius), stored with three float8.
	text: <(x,y),r> see point for details about the string representation of floats.
	binary: the three floats x, y, r in that order

float4/float8:
	text:
		(leading/trailing whitespace is skipped) interpreted with the C strtod function, but since it has problems with NaN, [+-]Infinity, [+-]Inf, those strings are identified (case-insensitively) separately.
		when outputting: NaN, [+-]Infinity is treated separately, otherwise the string is printed with snprintf %.*g and the extra_float_digits setting is honored.
	binary: the float

abstime:
	A unix timestamp stored as a 32-bit signed integer with seconds-precision (seconds since 1970-01-01 00:00:00), in UTC
	Has three special values: Invalid (2^31-1), infinity (2^31-3), -infinity (-2^31)
	text: same format as timestamptz, or "invalid", "infinity", "-infinity"
	binary: Int32
	

reltime:
	A time interval with seconds-precision (stored as an 32-bit signed integer)
	text: same as interval
	binary: Int32

tinterval:
	Consists of a status (Int32) and two abstimes. Status is valid (1) iff both abstimes are valid, else 0.
	Note that the docs incorrectly states that ' is used as quote instead of "
	text: ["<abstime>" "<abstime>"]
	binary: Int32 (status), Int32 (abstime 1), Int32 (abstime 2)

unknown:
	text: text
	binary: text

money:
	A 64-bit signed integer. For example, $123.45 is stored as the integer 12345. Number of fraction digits is locale-dependent.
	text: a locale-depedent string
	binary: the raw 64-bit integer

macaddr:
	6 bytes
	text: the 6 bytes in hex (always two characters per byte) separated by :
	binary: the 6 bytes appearing in the same order as when written in text

inet/cidr:
	Struct of Family (byte: ipv4=2, ipv6=3), Netmask (byte with number of bits in the netmask), Ipaddr bytes (16)
	Text: The IP-address in text format and /netmask. /netmask is omitted in inet if the netmask is the whole address.
	Binary: family byte, netmask byte, byte (cidr=1, inet=0), number of bytes in address, bytes of the address

aclitem:
	Access list item used in pg_class
	Text: Something like postgres=arwdDxt/postgres
	Binary: not available

bpchar:
	Blank-padded char. The type modifier is used to blank-pad the input.
	text: text
	binary: text

varchar:
	Variable-length char. The type modifier is used to check the input's length.
	text: text
	binary: text

date:
	A signed 32-bit integer of a date. 0 = 2000-01-01.
	Infinity: INT_MAX, -Infinity: INT_MIN
	Text: Date only using the specified date style
	Binary: Int32

time:
	A signed 64-bit integer representing microseconds from 00:00:00.000000. (Legacy uses 64-bit float). Negative values are not allowed.
	Max value is 24:00:00.000000.
	text: hh:mm:ss or hh:mm:ss.ffffff where the fraction part is between 1 and 6 digits (trailing zeros are not written)
	binary: the 64-bit integer

timetz:
	A struct of
	Time: A signed 64-bit integer representing microseconds from 00:00:00.000000. (Legacy uses 64-bit float). Negative values are not allowed.
		Max value is 24:00:00.000000.
	Zone: A signed 32-bit integer representing the zone (in seconds). Note that the sign is inverted. So GMT+1h is stored as -1h.
	text: hh:mm:ss or hh:mm:ss.ffffff where the fraction part is between 1 and 6 digits (trailing zeros are not written)
	binary: the 64-bit integer followed by the 32-bit integer

timestamp:
	A signed 64-bit integer representing microseconds from 2000-01-01 00:00:00.000000
	Infinity is LONG_MAX and -Infinity is LONG_MIN
	(Infinity would be 294277-01-09 04:00:54.775807)
	Earliest possible timestamp is 4714-11-24 00:00:00 BC. Even earlier would be possible, but due to internal calculations those are forbidden.
	text: dependent on date style
	binary: the 64-bit integer

timestamptz:
	A signed 64-bit integer representing microseconds from 2000-01-01 00:00:00.000000 UTC. (Time zone is not stored).
	Infinity is LONG_MAX and -Infinity is LONG_MIN
	text: first converted to the time zone in the db settings, then printed according to the date style
	binary: the 64-bit integer

interval:
	A struct of
	Time (Int64): all time units other than days, months and years (microseconds)
	Day (Int32): days, after time for alignment
	Month (Int32): months and years, after time for alignment
	text: Style dependent, but for example: "-11 mons +15435 days -11111111:53:00"
	binary: all fields in the struct

bit/varbit:
	First a signed 32-bit integer containing the number of bits (negative length not allowed). Then all the bits in big end first.
	So a varbit of length 1 has the first (and only) byte set to either 0x80 or 0x00. Last byte is assumed (and is automatically zero-padded in recv) to be zero-padded.
	text:
		when sending from backend: all the bits, written with 1s and 0s.
		when receiving from client: (optionally b or B followed by) all the bits as 1s and 0s, or a x or X followed by hexadecimal digits (upper- or lowercase), big endian first.
	binary: the 32-bit length followed by the bytes containing the bits

numeric:
	A variable-length numeric value, can be negative.
	text: NaN or first - if it is negative, then the digits with . as decimal separator
	binary:
		first a header of 4 16-bit signed integers:
			number of digits in the digits array that follows (can be 0, but not negative),
			weight of the first digit (10000^weight), can be both negative, positive or 0,
			sign: negative=0x4000, positive=0x0000, NaN=0xC000
			dscale: number of digits (in base 10) to print after the decimal separator
		then the array of digits:
			The digits are stored in base 10000, where each digit is a 16-bit integer.
			Trailing zeros are not stored in this array, to save space.
			The digits are stored such that, if written as base 10000, the decimal separator can be inserted between two digits in base 10000,
				i.e. when this is to be printed in base 10, only the first digit in base 10000 can (possibly) be printed with less than 4 characters.
				Note that this does not apply for the digits after the decimal separator; the digits should be printed out in chunks of 4
				characters and then truncated with the given dscale.

refcursor:
	uses the same routines as text

record:
	Describes a tuple. Is also the "base class" for composite types (i.e. it uses the same i/o functions).
	text:
		( followed by a list of comma-separated text-encoded values followed by ).
		Empty element means null.
		Quoted with " and " if necessary. " is escaped with "" and \ is escaped with \\ (this differs from arrays where " is escaped with \").
		Must be quoted if it is an empty string or contains one of "\,() or a space.
	binary:
		First a 32-bit integer with the number of columns, then for each column:
			An OID indicating the type of the column
			The length of the column (32-bit integer), or -1 if null
			The column data encoded as binary

cstring:
	text/binary: all characters are sent without the trailing null-character

void:
	Used for example as return value in SELECT * FROM func_returning_void()
	text: an empty string
	binary: zero bytes

uuid:
	A 16-byte uuid.
	text: group of 8, 4, 4, 4, 12 hexadecimal lower-case characters, separated by -. The first byte is written first. It is allowed to surround it with {}.
	binary: the 16 bytes

txid_snapshot:
	(txid is a UInt64)
	A struct of
	UInt32 nxip (size of the xip array)
	txid xmin (no values in xip is smaller than this)
	txid xmax (no values in xip is larger than or equal this)
	txid[] xip (is ordered in ascending order)
	text: xmin:xmax:1,2,3,4
	binary: all fields in the structure

tsvector:
	Used for text searching. Example of tsvector: 'a':1,6,10 'on':5 'and':8 'ate':9A 'cat':3 'fat':2,11 'mat':7 'rat':12 'sat':4
	Max length for each lexeme string is 2046 bytes (excluding the trailing null-char)
	The words are sorted when parsed, and only written once. Positions are also sorted and only written once.
	For some reason, the unique check does not seem to be made for binary input, only text input...
	text: As seen above. ' is escaped with '' and \ is escaped with \\.
	binary:
		UInt32 number of lexemes
		for each lexeme:
			lexeme text in client encoding, null-terminated
			UInt16 number of positions
			for each position:
				UInt16 WordEntryPos, where the most significant 2 bits is weight, and the 14 least significant bits is pos (can't be 0). Weights 3,2,1,0 represent A,B,C,D

tsquery:
	A tree with operands and operators (&, |, !). Operands are strings, with optional weight (bitmask of ABCD) and prefix search (yes/no, written with *).
	text: the tree written in infix notation. Example: ( 'abc':*B | 'def' ) & !'ghi'
	binary: the tree written in prefix notation:
		First the number of tokens (a token is an operand or an operator).
		For each token:
			UInt8 type (1 = val, 2 = oper) followed by
			For val: UInt8 weight + UInt8 prefix (1 = yes / 0 = no) + null-terminated string,
			For oper: UInt8 oper (1 = not, 2 = and, 3 = or, 4 = phrase). 
			In case of phrase oper code, an additional UInt16 field is sent (distance value of operator). Default is 1 for <->, otherwise the n value in '<n>'.

enum:
	Simple text

gtsvector:
	GiST for tsvector. Probably internal type.

int4range/numrange/tsrange/tstzrange/daterange/int8range and user-defined range types:
	/* A range's flags byte contains these bits: */
	#define RANGE_EMPTY         0x01    /* range is empty */
	#define RANGE_LB_INC        0x02    /* lower bound is inclusive */
	#define RANGE_UB_INC        0x04    /* upper bound is inclusive */
	#define RANGE_LB_INF        0x08    /* lower bound is -infinity */
	#define RANGE_UB_INF        0x10    /* upper bound is +infinity */
	#define RANGE_LB_NULL       0x20    /* lower bound is null (NOT USED) */
	#define RANGE_UB_NULL       0x40    /* upper bound is null (NOT USED) */
	#define RANGE_CONTAIN_EMPTY 0x80/* marks a GiST internal-page entry whose
	                                 * subtree contains some empty ranges */
	A range has no lower bound if any of RANGE_EMPTY, RANGE_LB_INF (or RANGE_LB_NULL, not used anymore) is set. The same applies for upper bounds.
	text:
		A range with RANGE_EMPTY is just written as the string "empty".
		Inclusive bounds are written with [ and ], else ( and ) is used.
		The two values are comma-separated.
		Missing bounds are written as an empty string (without quotes).
		Each value is quoted with " if necessary. Quotes are necessary if the string is either the empty string or contains "\,()[] or spaces. " is escaped with "" and \ is escaped with \\.
		Example: [18,21]
	binary: First the flag byte. Then, if has lower bound: 32-bit length + binary-encoded data. Then, if has upper bound: 32-bit length + binary-encoded data.

hstore:
	Key/value-store. Both keys and values are strings.
	text:
		Comma-space separated string, where each item is written as "key"=>"value" or "key"=>NULL. " and \ are escaped as \" and \\.
		Example: "a"=>"b", "c"=>NULL, "d"=>"q"
	binary:
		Int32 count
		for each item:
			Int32 keylen
			string of the key (not null-terminated)
			Int32 length of item (or -1 if null)
			the item as a string

ghstore:
	internal type for indexing hstore

domain types:
	mapped types used in information_schema:
		cardinal_number: int4 (must be nonnegative or null)
		character_data: varchar
		sql_identifier: varchar
		time_stamp: timestamptz
		yes_or_no: varchar(3) (must be "YES" or "NO" or null)
	intnotnull: when an int4 is cast to this type, it is checked that the int4 is not null, but it still returns an int4 and not intnotnull...
~~~
