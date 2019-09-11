using System;
using Npgsql.NameTranslation;

namespace Npgsql.Tests.Types
{
    public partial class CompositeHandlerTests : TestBase
    {
        NpgsqlConnection OpenAndMapComposite<T>(T composite, string? schema, string nameSuffix, out string nameQualified)
            where T : IComposite, new()
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
                connection.ExecuteNonQuery(schema is null? $"DROP TYPE IF EXISTS {name}" : $"DROP SCHEMA IF EXISTS {schema} CASCADE; CREATE SCHEMA {schema}");
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

        public class TypeWithTwoPropertiesInverted : IComposite, IInitializable
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
            public string GetValues() => $"'Value'";
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
    }
}
