using System.Threading.Tasks;
using Npgsql.NameTranslation;
using NpgsqlTypes;

namespace Npgsql.Tests.Types;

public partial class CompositeHandlerTests : TestBase
{
    Task<NpgsqlDataSource> OpenAndMapComposite<T>(T composite, string? schema, string nameSuffix, out string nameQualified)
        where T : IComposite
    {
        var nameTranslator = new NpgsqlSnakeCaseNameTranslator();
        var name = nameTranslator.TranslateTypeName(typeof(T).Name + nameSuffix);

        if (schema == null)
            nameQualified = name;
        else
        {
            schema = nameTranslator.TranslateTypeName(schema);
            nameQualified = schema + "." + name;
        }

        return OpenAndMapCompositeCore(nameQualified);

        async Task<NpgsqlDataSource> OpenAndMapCompositeCore(string nameQualified)
        {
            await using var adminConnection = await OpenConnectionAsync();

            await adminConnection.ExecuteNonQueryAsync(schema is null ? $"DROP TYPE IF EXISTS {name}" : $"DROP SCHEMA IF EXISTS {schema} CASCADE; CREATE SCHEMA {schema}");
            await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {nameQualified} AS ({composite.GetAttributes()})");

            var dataSourceBuilder = CreateDataSourceBuilder();
            dataSourceBuilder.MapComposite<T>(nameQualified, nameTranslator);
            var dataSource = dataSourceBuilder.Build();
            await using var connection = await dataSource.OpenConnectionAsync();

            return dataSource;
        }
    }

    interface IComposite
    {
        string GetAttributes();
        string GetValues();
    }

    interface IInitializable
    {
        void Initialize();
    }

    const string HelloSlonik = "Hello, Slonik";
    const int TheAnswer = 42;

    public class ClassWithProperty : IComposite, IInitializable
    {
        public string? Value { get; set; }
        public string GetAttributes() => "value text";
        public string GetValues() => $"'{Value}'";
        public void Initialize() => Value = HelloSlonik;
    }

    public class ClassWithField : IComposite, IInitializable
    {
        public string? Value;
        public string GetAttributes() => "value text";
        public string GetValues() => $"'{Value}'";
        public void Initialize() => Value = HelloSlonik;
    }

    public struct StructWithProperty : IComposite, IInitializable
    {
        public string? Value { get; set; }
        public string GetAttributes() => "value text";
        public string GetValues() => $"'{Value}'";
        public void Initialize() => Value = HelloSlonik;
    }

    public struct StructWithField : IComposite, IInitializable
    {
        public string? Value;
        public string GetAttributes() => "value text";
        public string GetValues() => $"'{Value}'";
        public void Initialize() => Value = HelloSlonik;
    }

    public class TypeWithTwoProperties : IComposite, IInitializable
    {
        public int IntValue { get; set; }
        public string? StringValue { get; set; }

        public string GetAttributes() => "int_value integer, string_value text";
        public string GetValues() => $"{IntValue}, '{StringValue}'";

        public void Initialize()
        {
            IntValue = TheAnswer;
            StringValue = HelloSlonik;
        }
    }

    public class TypeWithTwoPropertiesReversed : IComposite, IInitializable
    {
        public int IntValue { get; set; }
        public string? StringValue { get; set; }

        public string GetAttributes() => "string_value text, int_value integer";
        public string GetValues() => $"'{StringValue}', {IntValue}";

        public void Initialize()
        {
            IntValue = TheAnswer;
            StringValue = HelloSlonik;
        }
    }

    public abstract class SimpleComposite : IComposite
    {
        public string GetAttributes() => "value text";
        public string GetValues() => $"'{GetValue()}'";

        protected virtual string GetValue() => HelloSlonik;
    }

    public class TypeWithPrivateProperty : SimpleComposite
    {
        private string? Value { get; set; }
    }

    public class TypeWithPrivateGetter : SimpleComposite
    {
        public string? Value { private get; set; }
    }

    public class TypeWithPrivateSetter : SimpleComposite
    {
        public string? Value { get; private set; }
    }

    public class TypeWithoutGetter : SimpleComposite
    {

        public string? Value { set { } }
    }

    public class TypeWithoutSetter : SimpleComposite
    {
        public string? Value { get; }
    }

    public class TypeWithExplicitPropertyName : SimpleComposite
    {
        [PgName("value")]
        public string MyValue { get; set; } = string.Empty;
        protected override string GetValue() => MyValue;
    }

    public class TypeWithExplicitParameterName([PgName("value")] string myValue) : SimpleComposite
    {
        public string Value { get; } = myValue;
        protected override string GetValue() => Value;
    }

    public class TypeWithMorePropertiesThanAttributes : IComposite
    {
        public string GetAttributes() => "int_value integer";
        public string GetValues() => $"{IntValue}";

        public int IntValue { get; set; }
        public string? StringValue { get; set; }
    }

    public class TypeWithLessPropertiesThanAttributes : IComposite
    {
        public string GetAttributes() => "int_value integer, string_value text";
        public string GetValues() => $"{IntValue}, NULL";

        public int IntValue { get; set; }
    }
    public class TypeWithMoreParametersThanAttributes(int intValue, string? stringValue) : IComposite
    {
        public string GetAttributes() => "int_value integer";
        public string GetValues() => $"{IntValue}";

        public int IntValue { get; set; } = intValue;
        public string? StringValue { get; set; } = stringValue;
    }

    public class TypeWithLessParametersThanAttributes(int intValue) : IComposite
    {
        public string GetAttributes() => "int_value integer, string_value text";
        public string GetValues() => $"{IntValue}, NULL";

        public int IntValue { get; } = intValue;
    }

    public class TypeWithOneParameter(int value1) : IComposite
    {
        public string GetAttributes() => "value1 integer";
        public string GetValues() => $"{Value1}";

        public int Value1 { get; } = value1;
    }

    public class TypeWithTwoParameters(int intValue, string stringValue) : IComposite
    {
        public string GetAttributes() => "int_value integer, string_value text";
        public string GetValues() => $"{IntValue}, '{StringValue}'";

        public int IntValue { get; } = intValue;
        public string? StringValue { get; } = stringValue;
    }

    public class TypeWithTwoParametersReversed(string stringValue, int intValue) : IComposite
    {
        public string GetAttributes() => "int_value integer, string_value text";
        public string GetValues() => $"{IntValue}, '{StringValue}'";

        public int IntValue { get; } = intValue;
        public string? StringValue { get; } = stringValue;
    }

    public class TypeWithNineParameters(
        int value1,
        int value2,
        int value3,
        int value4,
        int value5,
        int value6,
        int value7,
        int value8,
        int value9)
        : IComposite
    {
        public string GetAttributes() => "value1 integer, value2 integer, value3 integer, value4 integer, value5 integer, value6 integer, value7 integer, value8 integer, value9 integer";
        public string GetValues() => $"{Value1}, {Value2}, {Value3}, {Value4}, {Value5}, {Value6}, {Value7}, {Value8}, {Value9}";

        public int Value1 { get; } = value1;
        public int Value2 { get; } = value2;
        public int Value3 { get; } = value3;
        public int Value4 { get; } = value4;
        public int Value5 { get; } = value5;
        public int Value6 { get; } = value6;
        public int Value7 { get; } = value7;
        public int Value8 { get; } = value8;
        public int Value9 { get; } = value9;
    }
}
