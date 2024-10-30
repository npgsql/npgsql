using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests;

public class NpgsqlSelectValueFixture : NpgsqlDbFactoryFixture, ISelectValueFixture, IDeleteFixture, IDisposable
{
    public NpgsqlSelectValueFixture()
        => Utility.ExecuteNonQuery(this, @"
DROP TABLE IF EXISTS select_value;
CREATE TABLE select_value
(
	id INTEGER NOT NULL PRIMARY KEY,
	""Binary"" bytea,
	""Boolean"" boolean,
	""Date"" date,
	""DateTime"" timestamp,
	""DateTimeOffset"" timestamp with time zone,
	""Decimal"" numeric(57,28),
	""Double"" double precision,
	""Guid"" uuid,
	""Int16"" smallint,
	""Int32"" integer,
	""Int64"" bigint,
	""Single"" real,
	""String"" varchar,
	""Time"" time
);
INSERT INTO select_value VALUES
(0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(1, E''::bytea, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '', NULL),
(2, E'\\000'::bytea, false, NULL, NULL, NULL, 0, 0, '00000000-0000-0000-0000-000000000000', 0, 0, 0, 0, '0', '00:00:00'),
(3, E'\\021'::bytea, true, '1111-11-11', '1111-11-11 11:11:11.111', '1111-11-11 11:11:11.111 +11:11', 1, 1, '11111111-1111-1111-1111-111111111111', 1, 1, 1, 1, '1', '11:11:11.111'),
(4, NULL, false, '0001-01-01', '0001-01-01', '0001-01-01', 0.000000000000001, 2.23e-308, '33221100-5544-7766-9988-aabbccddeeff', -32768, -2147483648, -9223372036854775808, 1.18e-38, NULL, '00:00:00'),
(5, NULL, true, '9999-12-31', '9999-12-31 23:59:59.999', '9999-12-31 23:59:59.999 +14:00', 99999999999999999999.999999999999999, 1.79e308, 'ccddeeff-aabb-8899-7766-554433221100', 32767, 2147483647, 9223372036854775807, 3.40e38, NULL, '23:59:59.999');
");

    public void Dispose() => Utility.ExecuteNonQuery(this, "DROP TABLE IF EXISTS select_value;");

    public string CreateSelectSql(DbType dbType, ValueKind kind) =>
        $"SELECT \"{dbType.ToString()}\" FROM select_value WHERE id = {(int)kind};";

    public string CreateSelectSql(byte[] value) =>
        $@"SELECT E'{string.Join("", value.Select(x => @"\x" + x.ToString("X2")))}'::bytea";

    public string SelectNoRows => "SELECT 1 WHERE 0 = 1;";

    public IReadOnlyCollection<DbType> SupportedDbTypes { get; } = new ReadOnlyCollection<DbType>([
        DbType.Binary,
        DbType.Boolean,
        DbType.Date,
        DbType.DateTime,
        DbType.DateTimeOffset,
        DbType.Decimal,
        DbType.Double,
        DbType.Guid,
        DbType.Int16,
        DbType.Int32,
        DbType.Int64,
        DbType.Single,
        DbType.String,
        DbType.Time
    ]);

    public Type NullValueExceptionType => typeof(InvalidCastException);

    public string DeleteNoRows => "DELETE FROM select_value WHERE 1 = 0";
}
