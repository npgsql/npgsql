using System;
using Npgsql.NameTranslation;
using NpgsqlTypes;

namespace Npgsql.Tests.Types
{
    public partial class CompositeHandlerTests : TestBase
    {
        NpgsqlConnection OpenAndMapComposite<T>(T composite, string? schema, string nameSuffix, out string nameQualified)
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

            var connection = OpenConnection();

            try
            {
                connection.ExecuteNonQuery(schema is null ? $"DROP TYPE IF EXISTS {name}" : $"DROP SCHEMA IF EXISTS {schema} CASCADE; CREATE SCHEMA {schema}");
                connection.ExecuteNonQuery($"CREATE TYPE {nameQualified} AS ({composite.GetAttributes()})");

                connection.ReloadTypes();
                connection.TypeMapper.MapComposite<T>(nameQualified, nameTranslator);

                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
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

        public class TypeWithExplicitParameterName : SimpleComposite
        {
            public TypeWithExplicitParameterName([PgName("value")] string myValue) => Value = myValue;
            public string Value { get; }
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
        public class TypeWithMoreParametersThanAttributes : IComposite
        {
            public string GetAttributes() => "int_value integer";
            public string GetValues() => $"{IntValue}";

            public TypeWithMoreParametersThanAttributes(int intValue, string? stringValue)
            {
                IntValue = intValue;
                StringValue = stringValue;
            }

            public int IntValue { get; set; }
            public string? StringValue { get; set; }
        }

        public class TypeWithLessParametersThanAttributes : IComposite
        {
            public string GetAttributes() => "int_value integer, string_value text";
            public string GetValues() => $"{IntValue}, NULL";

            public TypeWithLessParametersThanAttributes(int intValue) =>
                IntValue = intValue;

            public int IntValue { get; }
        }

        public class TypeWithOneParameter : IComposite
        {
            public string GetAttributes() => "value1 integer";
            public string GetValues() => $"{Value1}";

            public TypeWithOneParameter(int value1) => Value1 = value1;
            public int Value1 { get; }
        }

        public class TypeWithTwoParameters : IComposite
        {
            public string GetAttributes() => "int_value integer, string_value text";
            public string GetValues() => $"{IntValue}, '{StringValue}'";

            public TypeWithTwoParameters(int intValue, string stringValue) =>
                (IntValue, StringValue) = (intValue, stringValue);

            public int IntValue { get; }
            public string? StringValue { get; }
        }

        public class TypeWithTwoParametersReversed : IComposite
        {
            public string GetAttributes() => "int_value integer, string_value text";
            public string GetValues() => $"{IntValue}, '{StringValue}'";

            public TypeWithTwoParametersReversed(string stringValue, int intValue) =>
                (StringValue, IntValue) = (stringValue, intValue);

            public int IntValue { get; }
            public string? StringValue { get; }
        }

        public class TypeWithNineParameters : IComposite
        {
            public string GetAttributes() => "value1 integer, value2 integer, value3 integer, value4 integer, value5 integer, value6 integer, value7 integer, value8 integer, value9 integer";
            public string GetValues() => $"{Value1}, {Value2}, {Value3}, {Value4}, {Value5}, {Value6}, {Value7}, {Value8}, {Value9}";

            public TypeWithNineParameters(int value1, int value2, int value3, int value4, int value5, int value6, int value7, int value8, int value9)
                => (Value1, Value2, Value3, Value4, Value5, Value6, Value7, Value8, Value9) = (value1, value2, value3, value4, value5, value6, value7, value8, value9);

            public int Value1 { get; }
            public int Value2 { get; }
            public int Value3 { get; }
            public int Value4 { get; }
            public int Value5 { get; }
            public int Value6 { get; }
            public int Value7 { get; }
            public int Value8 { get; }
            public int Value9 { get; }
        }
    }
}
