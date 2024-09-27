using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

public class CompositeTests : MultiplexingTestBase
{
    [Test]
    public async Task Basic()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, some_text text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeComposite { SomeText = "foo", X = 8 },
            "(8,foo)",
            type,
            npgsqlDbType: null);
    }

    [Test]
    public async Task Basic_with_custom_default_translator()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, s text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.DefaultNameTranslator = new CustomTranslator();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeComposite { SomeText = "foo", X = 8 },
            "(8,foo)",
            type,
            npgsqlDbType: null);
    }

    [Test]
    public async Task Basic_with_custom_translator()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, s text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type, new CustomTranslator());
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeComposite { SomeText = "foo", X = 8 },
            "(8,foo)",
            type,
            npgsqlDbType: null);
    }

    class CustomTranslator : INpgsqlNameTranslator
    {
        public string TranslateTypeName(string clrName) => throw new NotImplementedException();

        public string TranslateMemberName(string clrName) => clrName[0].ToString().ToLowerInvariant();
    }

#pragma warning disable CS0618 // GlobalTypeMapper is obsolete
    [Test, NonParallelizable]
    public async Task Global_mapping()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, some_text text)");
        NpgsqlConnection.GlobalTypeMapper.MapComposite<SomeComposite>(type);

        try
        {
            var dataSourceBuilder = CreateDataSourceBuilder();
            dataSourceBuilder.MapComposite<SomeComposite>(type);
            await using var dataSource = dataSourceBuilder.Build();
            await using var connection = await dataSource.OpenConnectionAsync();
            await connection.ReloadTypesAsync();

            await AssertType(
                connection,
                new SomeComposite { SomeText = "foo", X = 8 },
                "(8,foo)",
                type,
                npgsqlDbType: null);
        }
        finally
        {
            NpgsqlConnection.GlobalTypeMapper.Reset();
        }
    }
#pragma warning restore CS0618 // GlobalTypeMapper is obsolete

    [Test]
    public async Task Nested()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var containerType = await GetTempTypeName(adminConnection);
        var containeeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {containeeType} AS (x int, some_text text);
CREATE TYPE {containerType} AS (a int, containee {containeeType});");

        var dataSourceBuilder = CreateDataSourceBuilder();
        // Registration in inverse dependency order should work
        dataSourceBuilder
            .MapComposite<SomeCompositeContainer>(containerType)
            .MapComposite<SomeComposite>(containeeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeCompositeContainer { A = 8, Containee = new() { SomeText = "foo", X = 9 } },
            @"(8,""(9,foo)"")",
            containerType,
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1168")]
    public async Task With_schema()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var schema = await CreateTempSchema(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {schema}.some_composite AS (x int, some_text text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>($"{schema}.some_composite");
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeComposite { SomeText = "foo", X = 8 },
            "(8,foo)",
            $"{schema}.some_composite",
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4365")]
    public async Task In_different_schemas_same_type_with_nested()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var firstSchemaName = await CreateTempSchema(adminConnection);
        var secondSchemaName = await CreateTempSchema(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {firstSchemaName}.containee AS (x int, some_text text);
CREATE TYPE {firstSchemaName}.container AS (a int, containee {firstSchemaName}.containee);
CREATE TYPE {secondSchemaName}.containee AS (x int, some_text text);
CREATE TYPE {secondSchemaName}.container AS (a int, containee {secondSchemaName}.containee);");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder
            .MapComposite<SomeComposite>($"{firstSchemaName}.containee")
            .MapComposite<SomeCompositeContainer>($"{firstSchemaName}.container")
            .MapComposite<SomeComposite>($"{secondSchemaName}.containee")
            .MapComposite<SomeCompositeContainer>($"{secondSchemaName}.container");
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeCompositeContainer { A = 8, Containee = new() { SomeText = "foo", X = 9 } },
            @"(8,""(9,foo)"")",
            $"{secondSchemaName}.container",
            npgsqlDbType: null,
            isDefaultForWriting: false);

        await AssertType(
            connection,
            new SomeCompositeContainer { A = 8, Containee = new() { SomeText = "foo", X = 9 } },
            @"(8,""(9,foo)"")",
            $"{firstSchemaName}.container",
            npgsqlDbType: null,
            isDefaultForWriting: true);
    }

    [Test]
    public async Task Struct()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, some_text text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeCompositeStruct>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeCompositeStruct { SomeText = "foo", X = 8 },
            "(8,foo)",
            type,
            npgsqlDbType: null);
    }

    [Test]
    public async Task Array()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, some_text text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeComposite[] { new() { SomeText = "foo", X = 8 }, new() { SomeText = "bar", X = 9 }},
            @"{""(8,foo)"",""(9,bar)""}",
            type + "[]",
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
    public async Task Name_translation()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync(@$"
CREATE TYPE {type} AS (simple int, two_words int, some_database_name int)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<NameTranslationComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new NameTranslationComposite { Simple = 2, TwoWords = 3, SomeClrName = 4 },
            "(2,3,4)",
            type,
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/856")]
    public async Task Composite_containing_domain_type()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var domainType = await GetTempTypeName(adminConnection);
        var compositeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE DOMAIN {domainType} AS TEXT;
CREATE TYPE {compositeType} AS (street TEXT, postal_code {domainType})");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<Address>(compositeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new Address { PostalCode = "12345", Street = "Main St." },
            @"(""Main St."",12345)",
            compositeType,
            npgsqlDbType: null);
    }

    [Test]
    public async Task Composite_containing_array_type()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var compositeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {compositeType} AS (ints int4[])");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeCompositeWithArray>(compositeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeCompositeWithArray { Ints = new[] { 1, 2, 3, 4 } },
            @"(""{1,2,3,4}"")",
            compositeType,
            npgsqlDbType: null,
            comparer: (actual, expected) => actual.Ints!.SequenceEqual(expected.Ints!));
    }

    [Test]
    public async Task Composite_containing_IPAddress()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var compositeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {compositeType} AS (address inet)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeCompositeWithIPAddress>(compositeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeCompositeWithIPAddress { Address = IPAddress.Loopback },
            @"(127.0.0.1)",
            compositeType,
            npgsqlDbType: null,
            comparer: (actual, expected) => actual.Address!.Equals(expected.Address));
    }

    [Test]
    public async Task Composite_containing_converter_resolver_type()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var compositeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {compositeType} AS (date_times timestamp[])");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.Timezone = "Europe/Berlin";
        dataSourceBuilder.MapComposite<SomeCompositeWithConverterResolverType>(compositeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeCompositeWithConverterResolverType { DateTimes = new [] { new DateTime(DateTime.UnixEpoch.Ticks, DateTimeKind.Unspecified), new DateTime(DateTime.UnixEpoch.Ticks, DateTimeKind.Unspecified).AddDays(1) } },
            """("{""1970-01-01 00:00:00"",""1970-01-02 00:00:00""}")""",
            compositeType,
            npgsqlDbType: null,
            comparer: (actual, expected) => actual.DateTimes!.SequenceEqual(expected.DateTimes!));
    }

    [Test]
    public async Task Composite_containing_converter_resolver_type_throws()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var compositeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {compositeType} AS (date_times timestamp[])");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.Timezone = "Europe/Berlin";
        dataSourceBuilder.MapComposite<SomeCompositeWithConverterResolverType>(compositeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        Assert.ThrowsAsync<ArgumentException>(() => AssertType(
            connection,
            new SomeCompositeWithConverterResolverType { DateTimes = new[] { DateTime.UnixEpoch } }, // UTC DateTime
            """("{""1970-01-01 01:00:00"",""1970-01-02 01:00:00""}")""",
            compositeType,
            npgsqlDbType: null,
            comparer: (actual, expected) => actual.DateTimes!.SequenceEqual(expected.DateTimes!)));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/990")]
    public async Task Table_as_composite([Values] bool enabled)
    {
        await using var adminConnection = await OpenConnectionAsync();
        var table = await CreateTempTable(adminConnection, "x int, some_text text");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(table);
        if (enabled)
            dataSourceBuilder.ConnectionStringBuilder.LoadTableComposites = true;
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        if (enabled)
            await DoAssertion();
        else
        {
            Assert.ThrowsAsync<NotSupportedException>(DoAssertion);
            // Start a transaction specifically for multiplexing (to bind a connector to the connection)
            await using var tx = await connection.BeginTransactionAsync();
            Assert.Null(connection.Connector!.DatabaseInfo.CompositeTypes.SingleOrDefault(c => c.Name.Contains(table)));
            Assert.Null(connection.Connector!.DatabaseInfo.ArrayTypes.SingleOrDefault(c => c.Name.Contains(table)));

        }

        Task DoAssertion()
            => AssertType(
            connection,
            new SomeComposite { SomeText = "foo", X = 8 },
            "(8,foo)",
            table,
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1267")]
    public async Task Table_as_composite_with_deleted_columns()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var table = await CreateTempTable(adminConnection, "x int, some_text text, bar int");
        await adminConnection.ExecuteNonQueryAsync($"ALTER TABLE {table} DROP COLUMN bar;");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.LoadTableComposites = true;
        dataSourceBuilder.MapComposite<SomeComposite>(table);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new SomeComposite { SomeText = "foo", X = 8 },
            "(8,foo)",
            table,
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1125")]
    public async Task Nullable_property_in_class_composite()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (foo INT)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<ClassWithNullableProperty>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new ClassWithNullableProperty { Foo = 8 },
            "(8)",
            type,
            npgsqlDbType: null);

        await AssertType(
            connection,
            new ClassWithNullableProperty { Foo = null },
            "()",
            type,
            npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1125")]
    public async Task Nullable_property_in_struct_composite()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (foo INT)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<StructWithNullableProperty>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new StructWithNullableProperty { Foo = 8 },
            "(8)",
            type,
            npgsqlDbType: null);

        await AssertType(
            connection,
            new StructWithNullableProperty { Foo = null },
            "()",
            type,
            npgsqlDbType: null);
    }

    [Test]
    public async Task PostgresType()
    {
        await using var connection = await OpenConnectionAsync();
        var type1 = await GetTempTypeName(connection);
        var type2 = await GetTempTypeName(connection);

        await connection.ExecuteNonQueryAsync(@$"
CREATE TYPE {type1} AS (x int, some_text text);
CREATE TYPE {type2} AS (comp {type1}, comps {type1}[]);");
        await connection.ReloadTypesAsync();

        await using var cmd = new NpgsqlCommand(
            $"SELECT ROW(ROW(8, 'foo')::{type1}, ARRAY[ROW(9, 'bar')::{type1}, ROW(10, 'baz')::{type1}])::{type2}",
            connection);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        var comp2Type = (PostgresCompositeType)reader.GetPostgresType(0);
        Assert.That(comp2Type.Name, Is.EqualTo(type2));
        Assert.That(comp2Type.FullName, Is.EqualTo($"public.{type2}"));
        Assert.That(comp2Type.Fields, Has.Count.EqualTo(2));
        var field1 = comp2Type.Fields[0];
        var field2 = comp2Type.Fields[1];
        Assert.That(field1.Name, Is.EqualTo("comp"));
        Assert.That(field2.Name, Is.EqualTo("comps"));
        var comp1Type = (PostgresCompositeType)field1.Type;
        Assert.That(comp1Type.Name, Is.EqualTo(type1));
        var arrType = (PostgresArrayType)field2.Type;
        Assert.That(arrType.Name, Is.EqualTo(type1 + "[]"));
        var elemType = arrType.Element;
        Assert.That(elemType, Is.SameAs(comp1Type));
    }

    [Test]
    public async Task DuplicateConstructorParameters()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (long int8, boolean bool)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<DuplicateOneLongOneBool>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var ex = Assert.ThrowsAsync<InvalidCastException>(async () => await AssertType(
            connection,
            new DuplicateOneLongOneBool(true, 1),
            "(1,t)",
            type,
            npgsqlDbType: null));
        Assert.That(ex!.InnerException, Is.TypeOf<AmbiguousMatchException>());
    }

    [Test]
    public async Task PartialConstructorMissingSetter()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (long int8, boolean bool)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<MissingSetterOneLongOneBool>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await AssertTypeRead(
            connection,
            "(1,t)",
            type,
            new MissingSetterOneLongOneBool(true, 1)));
        Assert.That(ex, Is.TypeOf<InvalidOperationException>().With.Message.Contains("No (public) setter for"));
    }

    [Test]
    public async Task PartialConstructorWorks()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (long int8, boolean bool)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<OneLongOneBool>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertType(
            connection,
            new OneLongOneBool(1) { BooleanValue = true },
            "(1,t)",
            type,
            npgsqlDbType: null);
    }

    [Test]
    public async Task CompositeOverRange()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        var rangeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, some_text text); CREATE TYPE {rangeType} AS RANGE(subtype={type})");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        dataSourceBuilder.EnableUnmappedTypes();
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var composite1 = new SomeComposite
        {
            SomeText = "foo",
            X = 8
        };

        var composite2 = new SomeComposite
        {
            SomeText = "bar",
            X = 42
        };

        await AssertType(
            connection,
            new NpgsqlRange<SomeComposite>(composite1, composite2),
            "[\"(8,foo)\",\"(42,bar)\"]",
            rangeType,
            npgsqlDbType: null,
            isDefaultForWriting: false);
    }

    #region Test Types

    readonly struct DuplicateOneLongOneBool
    {
        public DuplicateOneLongOneBool(bool boolean, [PgName("boolean")]int @bool)
        {
        }

        [PgName("long")]
        public long LongValue { get; }

        [PgName("boolean")]
        public bool BooleanValue { get; }
    }

    readonly struct MissingSetterOneLongOneBool
    {
        public MissingSetterOneLongOneBool(long @long)
        {
            LongValue = @long;
        }

        public MissingSetterOneLongOneBool(bool boolean, [PgName("boolean")]int @bool)
        {
        }

        [PgName("long")]
        public long LongValue { get; }

        [PgName("boolean")]
        public bool BooleanValue { get; }
    }

    struct OneLongOneBool
    {
        public OneLongOneBool(bool boolean, [PgName("boolean")]int @bool)
        {
        }

        public OneLongOneBool(long @long)
        {
            LongValue = @long;
        }

        public OneLongOneBool(double other)
        {
        }

        public OneLongOneBool(int boolean, [PgName("boolean")]bool @bool)
        {
        }

        [PgName("long")]
        public long LongValue { get; }

        [PgName("boolean")]
        public bool BooleanValue { get; set; }
    }


    record SomeComposite
    {
        public int X { get; set; }
        public string SomeText { get; set; } = null!;
    }

    record SomeCompositeContainer
    {
        public int A { get; set; }
        public SomeComposite Containee { get; set; }  = null!;
    }

    struct SomeCompositeStruct
    {
        public int X { get; set; }
        public string SomeText { get; set; }
    }

    class SomeCompositeWithArray
    {
        public int[]? Ints { get; set; }
    }

    class SomeCompositeWithIPAddress
    {
        public IPAddress? Address { get; set; }
    }

    class SomeCompositeWithConverterResolverType
    {
        public DateTime[]? DateTimes { get; set; }
    }

    record NameTranslationComposite
    {
        public int Simple { get; set; }
        public int TwoWords { get; set; }
        [PgName("some_database_name")]
        public int SomeClrName { get; set; }
    }

    record Address
    {
        public string Street { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
    }

    record ClassWithNullableProperty
    {
        public int? Foo { get; set; }
    }

    struct StructWithNullableProperty
    {
        public int? Foo { get; set; }
    }

    public CompositeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}

    #endregion
}
