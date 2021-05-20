using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Replication
{
    [TestFixture(ReplicationDataMode.DefaultReplicationData, Buffering.Unbuffered, AsyncMode.Async)]
    [TestFixture(ReplicationDataMode.BinaryReplicationData, Buffering.Unbuffered, AsyncMode.Async)]
    [TestFixture(ReplicationDataMode.DefaultReplicationData, Buffering.Unbuffered, AsyncMode.Sync)]
    [TestFixture(ReplicationDataMode.BinaryReplicationData, Buffering.Unbuffered, AsyncMode.Sync)]
    [TestFixture(ReplicationDataMode.DefaultReplicationData, Buffering.Buffered, AsyncMode.Async)]
    [TestFixture(ReplicationDataMode.BinaryReplicationData, Buffering.Buffered, AsyncMode.Async)]
    [TestFixture(ReplicationDataMode.DefaultReplicationData, Buffering.Buffered, AsyncMode.Sync)]
    [TestFixture(ReplicationDataMode.BinaryReplicationData, Buffering.Buffered, AsyncMode.Sync)]
    public class ReplicationDataRecordTests : PgOutputReplicationTestBase
    {
        #region GetFieldValueTestCases

        public delegate Task SqlCommandDelegate(NpgsqlConnection connection);

        static IEnumerable GetFieldValueTestCases()
        {
            foreach (var t in TestCases())
            {
                var testCase = new TestCaseData(t.TypeName, t.BinaryValue, t.StringValue, t.InputString, t.TypeDelegates).SetName(t.TestCaseName);
                testCase = t.ExplicitReason is null ? testCase : testCase.Explicit(t.ExplicitReason);
                testCase = t.IgnoreReason is null ? testCase : testCase.Ignore(t.IgnoreReason);
                yield return testCase;
            }

            static IEnumerable<GetFieldValueTestCase> TestCases()
            {
                #region Numeric Types

                yield return new("smallint", (short)42);
                yield return new("smallint[]", new short[]{1,2});
                yield return new("integer", 42);
                yield return new("integer[]", new []{1,2});
                yield return new("bigint", 42L);
                yield return new("bigint[]", new []{1L,2L});
                yield return new("numeric", 42.42M);
                yield return new("numeric[]", new []{1.1M,2.2M});
                yield return new("real", 42.42f);
                yield return new("real[]", new []{1.1f,2.2f});
                yield return new("double precision", 42.42D);
                yield return new("double precision[]", new []{1.1D,2.2D});

                #endregion Numeric Types

                #region Monetary Types

                yield return new("money", 42.42M, "$42.42");
                yield return new("money[]", new []{1.1M,2.2M}, "{$1.10,$2.20}");

                #endregion Monetary Types

                #region Character Types

                yield return new("character varying(10)", "Test");
                yield return new("character varying(10)[]", new []{"Test 1","Test 2"}, "{\"Test 1\",\"Test 2\"}");
                yield return new("character(4)", "Test");
                yield return new("character(6)[]", new []{"Test 1","Test 2"}, "{\"Test 1\",\"Test 2\"}");
                yield return new("text", "Test");
                yield return new("text[]", new []{"Test 1","Test 2"}, "{\"Test 1\",\"Test 2\"}");
                yield return new("\"char\"", 'T');
                yield return new("\"char\"[]", new []{'A','B'});
                yield return new("name", "Test");
                yield return new("name[]", new []{"Test 1","Test 2"}, "{\"Test 1\",\"Test 2\"}");

                #endregion Character Types

                #region Binary Data Types

                static byte[] B(string text) => Encoding.UTF8.GetBytes(text);
                yield return new("bytea", B("Test"), "Test");
                yield return new("bytea[]", new []{B("Test 1"), B("Test 2")}, "{\"Test 1\",\"Test 2\"}");

                #endregion Binary Data Types

                #region Date/Time Types

                var ts = new DateTime(2021, 06, 25, 19, 16, 48, DateTimeKind.Unspecified);
                yield return new("timestamp", ts);
                yield return new("timestamp[]", new []{ts, ts}, "{\"2021-06-25 19:16:48\",\"2021-06-25 19:16:48\"}");
                var tstz = new DateTime(2021, 06, 25, 21, 16, 48, DateTimeKind.Local);
                yield return new("timestamp with time zone", tstz);
                yield return new("timestamp with time zone[]", new []{tstz, tstz}, "{\"2021-06-25 21:16:48+02\",\"2021-06-25 21:16:48+02\"}");
                var date = new DateTime(2021, 06, 25);
                yield return new("date", date);
                yield return new("date[]", new []{date, date});
                var time = new TimeSpan(19, 16, 48);
                yield return new("time", time);
                yield return new("time[]", new []{time, time});
                // Mind you, the date has to be 0001-01-02 for the test to pass since that's the date we adjust
                // the returned DateTimeOffset to.
                var timetz = new DateTimeOffset(1, 1, 2, 19, 16, 48, TimeSpan.FromHours(2));
                yield return new("time with time zone", timetz);
                yield return new("time with time zone[]", new []{timetz, timetz});
                var interval = new TimeSpan(456, 17, 6, 34);
                yield return new("interval", interval);
                yield return new("interval[]", new []{interval, interval}, "{\"456 days 17:06:34\",\"456 days 17:06:34\"}");

                #endregion Date/Time Types

                #region Boolean Type

                yield return new("bool", true, "t");
                yield return new("bool[]", new []{true, false}, "{t,f}");

                #endregion Boolean Type

                #region Enumerated Types

                (SqlCommandDelegate createType, SqlCommandDelegate dropType) enumTypeDelegates = (
                    static async c =>
                    {
                        var t = NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator;
                        await c.ExecuteNonQueryAsync(@$"DROP TYPE IF EXISTS {t.TranslateTypeName(nameof(RDRTGFVEnum))} CASCADE;
                                                        CREATE TYPE {t.TranslateTypeName(nameof(RDRTGFVEnum))}
                                                        AS ENUM (
                                                            '{t.TranslateMemberName(nameof(RDRTGFVEnum.Happy))}',
                                                            '{t.TranslateMemberName(nameof(RDRTGFVEnum.Ok))}',
                                                            '{t.TranslateMemberName(nameof(RDRTGFVEnum.Sad))}')");
                        c.ReloadTypes();
                        NpgsqlConnection.GlobalTypeMapper.MapEnum<RDRTGFVEnum>();
                    },
                    static async c =>
                    {
                        var t = NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator;
                        NpgsqlConnection.GlobalTypeMapper.UnmapEnum<RDRTGFVEnum>();
                        await c.ExecuteNonQueryAsync(@$"DROP TYPE IF EXISTS {t.TranslateTypeName(nameof(RDRTGFVEnum))} CASCADE");
                    }
                );

                var enumTypeName = NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator.TranslateTypeName(nameof(RDRTGFVEnum));
                yield return new(enumTypeName, RDRTGFVEnum.Sad, typeDelegates: enumTypeDelegates);
                yield return new($"{enumTypeName}[]", new []
                {
                    RDRTGFVEnum.Ok, RDRTGFVEnum.Happy
                }, typeDelegates: enumTypeDelegates);

                #endregion Enumerated Types

                #region Geometric Types

                var point = new NpgsqlPoint(4.2D, 42D);
                yield return new("point", point);
                yield return new("point[]", new []{point, point}, "{\"(4.2,42)\",\"(4.2,42)\"}");
                var line = new NpgsqlLine(0.42D, 4.2D, 42D);
                yield return new("line", line);
                yield return new("line[]", new []{line, line}, "{\"{0.42,4.2,42}\",\"{0.42,4.2,42}\"}");
                var lseg = new NpgsqlLSeg(0.42D, 4.2D, 42D, 420D);
                yield return new("lseg", lseg);
                yield return new("lseg[]", new []{lseg, lseg}, "{\"[(0.42,4.2),(42,420)]\",\"[(0.42,4.2),(42,420)]\"}");
                var box = new NpgsqlBox(new(420D, 42D), new(4.2D, 0.42D));
                yield return new("box", box);
                yield return new("box[]", new []{box, box}, "{(420,42),(4.2,0.42);(420,42),(4.2,0.42)}");
                var path = new NpgsqlPath(new NpgsqlPoint(420D, 42D), new NpgsqlPoint(4.2D, 0.42D));
                yield return new("path", path);
                yield return new("path[]", new []{path, path}, "{\"((420,42),(4.2,0.42))\",\"((420,42),(4.2,0.42))\"}");
                var polygon = new NpgsqlPolygon(new NpgsqlPoint(420D, 42D), new NpgsqlPoint(4.2D, 0.42D));
                yield return new("polygon", polygon);
                yield return new("polygon[]", new []{polygon, polygon}, "{\"((420,42),(4.2,0.42))\",\"((420,42),(4.2,0.42))\"}");
                var circle = new NpgsqlCircle(0.42D, 4.2D, 42D);
                yield return new("circle", circle);
                yield return new("circle[]", new []{circle, circle}, "{\"<(0.42,4.2),42>\",\"<(0.42,4.2),42>\"}");

                #endregion Geometric Types

                #region Network Address Types

                var inet = IPAddress.Parse("127.0.0.1");
                yield return new("inet", inet);
                yield return new("inet[]", new []{inet, inet});
                var cidr = (IPAddress.Parse("2001:4f8:3:ba:2e0:81ff:fe22:d1f1"), 128);
                yield return new("cidr", cidr, testCaseName: $"{nameof(GetFieldValue)}<({nameof(IPAddress)},{nameof(Int32)})>({{0}})");
                yield return new("cidr[]", new []{cidr, cidr}, testCaseName: $"{nameof(GetFieldValue)}<({nameof(IPAddress)},{nameof(Int32)})[]>({{0}})");
                var macaddr = PhysicalAddress.Parse("08-00-2B-01-02-03");
                yield return new("macaddr", macaddr);
                yield return new("macaddr[]", new []{macaddr, macaddr});
                var macaddr8 = PhysicalAddress.Parse("08-00-2B-01-02-03-04-05");
                yield return new("macaddr8", macaddr8);
                yield return new("macaddr8[]", new []{macaddr8, macaddr8});

                #endregion Network Address Types

                #region Bit String Types

                yield return new("bit", true, "1");
                yield return new("bit[]", new []{true, false}, "{1,0}");
                var bit2 = new BitArray(2) { [0] = true };
                yield return new("bit(2)", bit2);
                yield return new("bit(2)[]", new []{bit2, bit2});
                var varbit4 = new BitArray(2) { [0] = true };
                yield return new("bit varying(4)", varbit4);
                yield return new("bit varying(4)[]", new []{varbit4, varbit4});

                #endregion Bit String Types

                #region Text Search Types

                var tsvector = NpgsqlTsVector.Parse("fat cat");
                yield return new("tsvector", tsvector);
                yield return new("tsvector[]", new []{tsvector, tsvector}, "{\"'cat' 'fat'\",\"'cat' 'fat'\"}");
                var tsquery = NpgsqlTsQuery.Parse("fat & (rat | cat)");
                yield return new("tsquery", tsquery, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlTsQuery)}>({{0}})");
                yield return new("tsquery[]", new []{tsquery, tsquery}, "{\"'fat' & ( 'rat' | 'cat' )\",\"'fat' & ( 'rat' | 'cat' )\"}");

                #endregion Text Search Types

                #region UUID Type

                var uuid = Guid.NewGuid();
                yield return new("uuid", uuid);
                yield return new("uuid[]", new []{uuid, uuid});

                #endregion UUID Type

                #region XML Type

                var xml = "<foo>bar</foo>";
                yield return new("xml", xml);
                yield return new("xml[]", new []{xml, xml});

                #endregion XML Type

                #region JSON Types

                var json = "{\"foo\": [true, \"bar\"], \"tags\": {\"a\": 1, \"b\": null}}";
                yield return new("json", json);
                yield return new("json[]", new []{json, json},
                    "{\"{\\\"foo\\\": [true, \\\"bar\\\"], \\\"tags\\\": {\\\"a\\\": 1, \\\"b\\\": null}}\",\"{\\\"foo\\\": [true, \\\"bar\\\"], \\\"tags\\\": {\\\"a\\\": 1, \\\"b\\\": null}}\"}");
                var jsonb = "{\"foo\": [true, \"bar\"], \"tags\": {\"a\": 1, \"b\": null}}";
                yield return new("jsonb", jsonb);
                yield return new("jsonb[]", new []{jsonb, jsonb},
                    "{\"{\\\"foo\\\": [true, \\\"bar\\\"], \\\"tags\\\": {\\\"a\\\": 1, \\\"b\\\": null}}\",\"{\\\"foo\\\": [true, \\\"bar\\\"], \\\"tags\\\": {\\\"a\\\": 1, \\\"b\\\": null}}\"}");
                var jsonpath = "$.\"id\"?(@ == 42)";
                yield return new("jsonpath", jsonpath);
                yield return new("jsonpath[]", new []{jsonpath, jsonpath}, "{\"$.\\\"id\\\"?(@ == 42)\",\"$.\\\"id\\\"?(@ == 42)\"}");

                #endregion JSON Types

                #region Composite Types

                var composite = new RDRTGFVComposite(42, "Answer to the Ultimate Question of Life, the Universe, and Everything");
                (SqlCommandDelegate createType, SqlCommandDelegate dropType) compositeTypeDelegates = (
                    static async c =>
                    {
                        var compositeTypeName =
                            NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator.TranslateTypeName(nameof(RDRTGFVComposite));
                        await c.ExecuteNonQueryAsync(@$"DROP TYPE IF EXISTS {compositeTypeName} CASCADE;
                                                        CREATE TYPE {compositeTypeName} AS (id int, value text)");
                        c.ReloadTypes();
                        NpgsqlConnection.GlobalTypeMapper.MapComposite<RDRTGFVComposite>();
                    },
                    static async c =>
                    {
                        var compositeTypeName =
                            NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator.TranslateTypeName(nameof(RDRTGFVComposite));
                        NpgsqlConnection.GlobalTypeMapper.UnmapComposite<RDRTGFVComposite>();
                        await c.ExecuteNonQueryAsync(@$"DROP TYPE IF EXISTS {compositeTypeName} CASCADE");
                    }
                );
                var compositeTypeName = NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator.TranslateTypeName(nameof(RDRTGFVComposite));
                yield return new(compositeTypeName, composite, typeDelegates: compositeTypeDelegates);
                yield return new($"{compositeTypeName}[]", new[] { composite, composite },
                    "{\"(42,\\\"Answer to the Ultimate Question of Life, the Universe, and Everything\\\")\",\"(42,\\\"Answer to the Ultimate Question of Life, the Universe, and Everything\\\")\"}",
                    typeDelegates: compositeTypeDelegates);

                #endregion Composite Types

                #region Range Types

                var int4Range = new NpgsqlRange<int>(-99, true, 99, false);
                yield return new("int4range", int4Range, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<int>)}<{nameof(Int32)}>>({{0}})");
                //yield return new("int4range[]", new []{int4range, int4range}, "{\"[-99,99)\",\"[-99,99)\"}", testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<int>)}<{nameof(Int32)}>[]>({{0}})");

                var int8Range = new NpgsqlRange<long>(-99L, true, 99L, false);
                yield return new("int8range", int8Range, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<long>)}<{nameof(Int64)}>>({{0}})");
                //yield return new("int8range[]", new []{int8range, int8range}, "{\"[-99,99)\",\"[-99,99)\"}", testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<long>)}<{nameof(Int64)}>[]>({{0}})");

                var numRange = new NpgsqlRange<decimal>(-99M, true, 99M, false);
                yield return new("numrange", numRange, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<decimal>)}<{nameof(Decimal)}>>({{0}})");
                //yield return new("numrange[]", new []{numrange, numrange}, "{\"[-99,99)\",\"[-99,99)\"}", testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<decimal>)}<{nameof(Decimal)}>[]>({{0}})");

                var tsRange = new NpgsqlRange<DateTime>(new DateTime(2021, 08, 08, 17, 15, 56, DateTimeKind.Unspecified), true, new DateTime(2021, 08, 08, 17, 16, 15, DateTimeKind.Unspecified), false);
                yield return new("tsrange", tsRange, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<DateTime>)}<{nameof(DateTime)}>>({{0}})");
                //yield return new("tsrange[]", new []{tsrange, tsrange}, "{\"[\\\"2021-08-08 17:15:56\\\",\\\"2021-08-08 17:16:15\\\")\",\"[\\\"2021-08-08 17:15:56\\\",\\\"2021-08-08 17:16:15\\\")\"}", testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<DateTime>)}<{nameof(DateTime)}>[]>({{0}})");

                var tstzRange = new NpgsqlRange<DateTime>(new DateTime(2021, 08, 08, 17, 15, 56, DateTimeKind.Local), true, new DateTime(2021, 08, 08, 17, 16, 15, DateTimeKind.Local), false);
                yield return new("tstzrange", tstzRange, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<DateTime>)}<{nameof(DateTime)}>>({{0}})");
                //yield return new("tstzrange[]", new []{tstzrange, tstzrange}, "{\"[\\\"2021-08-08 17:15:56+02\\\",\\\"2021-08-08 17:16:15+02\\\")\",\"[\\\"2021-08-08 17:15:56+02\\\",\\\"2021-08-08 17:16:15+02\\\")\"}", testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<DateTime>)}<{nameof(DateTime)}>[]>({{0}})");

                var dateRange = new NpgsqlRange<DateTime>(new DateTime(2021, 08, 07), true, new DateTime(2021, 08, 08), false);
                yield return new("daterange", dateRange, testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<DateTime>)}<{nameof(DateTime)}>>({{0}})");
                //yield return new("daterange[]", new []{daterange, daterange}, "{\"[2021-08-07,2021-08-08)\",\"[2021-08-07,2021-08-08)\"}", testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<DateTime>)}<{nameof(DateTime)}>[]>({{0}})");

                #endregion Range Types

                #region Domain Types

                (SqlCommandDelegate createType, SqlCommandDelegate dropType) posintDelegates = (
                    static async c =>
                    {
                        await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS nullableposint_domain CASCADE;
                                                        CREATE DOMAIN nullableposint_domain AS integer CHECK (VALUE >= 0)");
                        c.ReloadTypes();
                    },
                    static async c =>
                    {
                        await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS nullableposint_domain CASCADE");
                    }
                );
                // Domain over base type
                yield return new("nullableposint_domain", 42, typeDelegates: posintDelegates);

                // Array of domain over base type
                yield return new("nullableposint_domain[]", new []{1,2}, typeDelegates: posintDelegates);

                // Domain over array of base type
                yield return new("intarray_domain", new[] { 1, 2 }, typeDelegates: (
                    static async c =>
                    {
                        await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS intarray_domain CASCADE;
                                                        CREATE DOMAIN intarray_domain AS integer[]");
                        c.ReloadTypes();
                    },
                    static async c =>
                    {
                        await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS intarray_domain CASCADE");
                    }
                ));

                // Domain over range of base type
                yield return new("intrange_domain", int4Range,
                    testCaseName: $"{nameof(GetFieldValue)}<{nameof(NpgsqlRange<int>)}<{nameof(Int32)}>>({{0}})",
                    typeDelegates: (
                        static async c =>
                        {
                            await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS intrange_domain CASCADE;
                                                        CREATE DOMAIN intrange_domain AS int4range");
                            c.ReloadTypes();
                        },
                        static async c =>
                        {
                            await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS intrange_domain CASCADE");
                        }
                    ));

                // Domain over domain of base type is currently not supported by Npgsql
                // yield return new("posint_domain", 42,
                //     typeDelegates: (
                //         static async c =>
                //         {
                //             await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS nullableposint_domain CASCADE;
                //                                         CREATE DOMAIN nullableposint_domain AS integer CHECK (VALUE >= 0);
                //                                         CREATE DOMAIN posint_domain AS nullableposint_domain NOT NULL");
                //             c.ReloadTypes();
                //         },
                //         static async c =>
                //         {
                //             await c.ExecuteNonQueryAsync(@"DROP TYPE IF EXISTS nullableposint_domain CASCADE");
                //         }
                //     ));

                // ToDo: Domains over enums and composite types

                #endregion Domain Types

                #region Object Identifier Types

                yield return new("oid", 42U);
                yield return new("oid[]", new []{1U,2U});

                // Currently only the only Object Identifier Type that's mapped to uint is OID
                // We might want to support others (https://www.postgresql.org/docs/9.4/datatype-oid.html)
                // too but we currently don't.
                // yield return new("regclass", 1247U, "pg_type");
                // yield return new("regclass[]", new []{1247U,1259U},"{pg_type,pg_class}");

                #endregion Object Identifier Types

                #region pg_lsn Type

                // ToDo

                #endregion pg_lsn Type
            }
        }

        class GetFieldValueTestCase
        {
            readonly string? _stringValue;
            readonly string? _inputString;
            public string TypeName { get; }
            public object BinaryValue { get; }
            public (SqlCommandDelegate createType, SqlCommandDelegate dropType)? TypeDelegates { get; }
            public string? ExplicitReason { get; }
            public string? IgnoreReason { get; }
            public string TestCaseName { get; } = "{m}{a}";

            public string StringValue => _stringValue
                                         ?? (BinaryValue is Array a
                                             ? $"{{{string.Join(',', a.Cast<object>().Select(Format))}}}"
                                             : Format(BinaryValue));
            public string InputString => _inputString
                                         ?? $"$${StringValue}$$::{TypeName}";

            static string Format(object o)
                => o switch
                {
                    DateTime { Hour: 0, Minute: 0, Second: 0, Millisecond: 0 } date => date.ToString("yyyy-MM-dd"),
                    DateTime { Hour: >0, Kind: DateTimeKind.Unspecified } ts => ts.ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime { Hour: >0, Kind: DateTimeKind.Local } tstz => tstz.ToString("yyyy-MM-dd HH:mm:sszz"),
                    DateTimeOffset timetz => timetz.ToString("HH:mm:sszz"),
                    TimeSpan { Days: 0 } time => time.ToString("hh\\:mm\\:ss"),
                    TimeSpan interval => interval.ToString("%d' days 'hh\\:mm\\:ss"),
                    ValueTuple<IPAddress, int> cidr => string.Format(CultureInfo.InvariantCulture, "{0}/{1}", cidr.Item1, cidr.Item2),
                    PhysicalAddress macaddr => macaddr.GetAddressBytes()
                        switch
                        {
                            {Length: 6} m6 => string.Join(':', m6.Select(e => e.ToString("x2"))),
                            {Length: 8} m8 => string.Join(':', m8.Select(e => e.ToString("x2"))),
                            _ => throw new Exception()
                        },
                    BitArray bit => string.Concat(bit.Cast<bool>().Select(e => e ? '1' : '0')),
                    RDRTGFVEnum enum_type => NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator.TranslateMemberName(enum_type.ToString("g")),
                    RDRTGFVComposite composite => $"({composite.Id},\"{composite.Value}\")",
                    NpgsqlRange<DateTime> { LowerBound: { Hour: 0, Minute: 0, Second: 0, Millisecond: 0 }, UpperBound: { Hour: 0, Minute: 0, Second: 0, Millisecond: 0 } } dtr
                        => $"{(dtr.LowerBoundIsInclusive ? "[" : "(")}{Format(dtr.LowerBound)},{Format(dtr.UpperBound)}{(dtr.UpperBoundIsInclusive ? "]" : ")")}",
                    NpgsqlRange<DateTime> dtr => $"{(dtr.LowerBoundIsInclusive ? "[" : "(")}\"{Format(dtr.LowerBound)}\",\"{Format(dtr.UpperBound)}\"{(dtr.UpperBoundIsInclusive ? "]" : ")")}",
                    _ => string.Format(CultureInfo.InvariantCulture, "{0}", o),
                };

            public GetFieldValueTestCase(string typeName, object binaryValue, string? stringValue = null, string? inputString = null, (SqlCommandDelegate createType, SqlCommandDelegate dropType)? typeDelegates = null, string? testCaseName = "{m}({0})", string? explicitReason = null, string? ignoreReason = null)
            {
                _stringValue = stringValue;
                _inputString = inputString;
                TypeName = typeName;
                BinaryValue = binaryValue;
                TypeDelegates = typeDelegates;
                ExplicitReason = explicitReason;
                IgnoreReason = ignoreReason;
                if (testCaseName is not null)
                    TestCaseName = testCaseName;
            }
        }

        enum RDRTGFVEnum { Sad, Ok, Happy }

        record RDRTGFVComposite(int Id, string Value);

        #endregion

        [TestCaseSource(nameof(GetFieldValueTestCases)), NonParallelizable]
        public Task GetFieldValue<T>(string typeName, T binaryValue, string stringValue, string inputString, (SqlCommandDelegate createType, SqlCommandDelegate dropType)? typeDelegates)
            => SafePgOutputReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    var adjustedConnectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Options = "-c lc_monetary=C -c bytea_output=escape"
                    };

                    var triggersTypeMessage = false;
                    await using var c = await OpenConnectionAsync(adjustedConnectionStringBuilder);
                    if (typeDelegates?.createType is not null)
                    {
                        triggersTypeMessage = true;
                        await typeDelegates.Value.createType(c);
                    }

                    await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (value1 {typeName}, value2 {typeName});
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                    var rc = await OpenReplicationConnectionAsync(adjustedConnectionStringBuilder);
                    var slot = await rc.CreatePgOutputReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync(@$"INSERT INTO {tableName} VALUES ({inputString}, {inputString})");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                        .GetAsyncEnumerator(streamingCts.Token);

                    // Begin Transaction, Type Relation
                    await AssertTransactionStart(messages);
                    if (triggersTypeMessage)
                    {
                        await NextMessage<TypeMessage>(messages);
                        await NextMessage<TypeMessage>(messages);
                    }
                    await NextMessage<RelationMessage>(messages);

                    // Insert value
                    var insertMsg = await NextMessageBuffered<InsertMessage>(messages);
                    await AssertFieldValue(insertMsg, 1, binaryValue, stringValue);
                    if (Buffered)
                        await AssertFieldValue(insertMsg, 0, binaryValue, stringValue);
                    else
                        Assert.That(async () =>
                        {
                            var _ = Async
                                ? await insertMsg.NewRow.GetFieldValueAsync<T>(0)
                                : insertMsg.NewRow.GetFieldValue<T>(0);
                        }, Throws.InvalidOperationException);

                    // Commit Transaction
                    await AssertTransactionCommit(messages);
                    streamingCts.Cancel();
                    await AssertReplicationCancellation(messages);
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);

                    if (typeDelegates?.dropType is not null)
                        await typeDelegates.Value.dropType(c);

                    [SuppressMessage("ReSharper", "HeapView.PossibleBoxingAllocation")]
                    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
                    async Task AssertFieldValue(InsertMessage message, int ordinal, T expectedValue, string expectedStringValue)
                    {
                        object? value = Async
                            ? IsBinaryMode
                                ? await message.NewRow.GetFieldValueAsync<T>(ordinal)
                                : await message.NewRow.GetFieldValueAsync<string>(ordinal)
                            : IsBinaryMode
                                ? message.NewRow.GetFieldValue<T>(ordinal)
                                : message.NewRow.GetFieldValue<string>(ordinal);

                        Assert.That(value, IsBinaryMode ? Is.EqualTo(expectedValue) : Is.EqualTo(expectedStringValue));
                    }
                });

        async Task<T> NextMessageBuffered<T>(IAsyncEnumerator<PgOutputReplicationMessage> messages)
            where T : PgOutputReplicationMessage
        {
            var message = await NextMessage<T>(messages);
            if (message is InsertMessage insertMessage)
                return Buffered
                    ? Async
                        ? (T)(PgOutputReplicationMessage)await insertMessage.CloneAsync()
                        // ReSharper disable once MethodHasAsyncOverload
                        : (T)(PgOutputReplicationMessage)insertMessage.Clone()
                    : (T)(PgOutputReplicationMessage)insertMessage;

            return Buffered ? (T)message.Clone() : message;
        }

        public ReplicationDataRecordTests(ReplicationDataMode dataMode, Buffering buffering, AsyncMode asyncMode)
            : base(ProtocolVersionMode.ProtocolV1, dataMode, TransactionStreamingMode.DefaultTransaction)
        {
            Buffered = buffering == Buffering.Buffered;
            Async = asyncMode == AsyncMode.Async;
        }

        bool Buffered { get; }
        bool Async { get; }

        public enum Buffering
        {
            Unbuffered,
            Buffered
        }

        public enum AsyncMode
        {
            Sync,
            Async
        }
    }
}
