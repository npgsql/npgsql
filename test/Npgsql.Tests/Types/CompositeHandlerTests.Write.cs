using System;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public partial class CompositeHandlerTests
    {
        void Write<T>(Action<Func<NpgsqlDataReader>, T> assert, string? schema = null)
            where T : IComposite, IInitializable, new()
        {
            var composite = new T();
            composite.Initialize();
            Write(composite, assert, schema);
        }

        void Write<T>(T composite, Action<Func<NpgsqlDataReader>, T> assert, string? schema = null)
            where T : IComposite
        {
            using var connection = OpenAndMapComposite<T>(composite, schema, nameof(Write), out var _);
            using var command = new NpgsqlCommand("SELECT (@c).*", connection);

            command.Parameters.AddWithValue("c", composite);
            assert(() =>
            {
                var reader = command.ExecuteReader();
                reader.Read();
                return reader;
            }, composite);
        }

        [Test]
        public void Write_class_with_property() =>
            Write<ClassWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_class_with_field() =>
            Write<ClassWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_struct_with_property() =>
            Write<StructWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_struct_with_field() =>
            Write<StructWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_type_with_two_properties() =>
            Write<TypeWithTwoProperties>((execute, expected) =>
            {
                var actual = execute();
                Assert.AreEqual(expected.IntValue, actual.GetInt32(0));
                Assert.AreEqual(expected.StringValue, actual.GetString(1));
            });

        [Test]
        public void Write_type_with_two_properties_inverted() =>
            Write<TypeWithTwoPropertiesReversed>((execute, expected) =>
            {
                var actual = execute();
                Assert.AreEqual(expected.IntValue, actual.GetInt32(1));
                Assert.AreEqual(expected.StringValue, actual.GetString(0));
            });

        [Test]
        public void Write_type_with_private_property_throws() =>
            Write(new TypeWithPrivateProperty(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_type_with_private_getter_throws() =>
            Write(new TypeWithPrivateGetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_type_with_private_setter() =>
            Write(new TypeWithPrivateSetter(), (execute, expected) => execute());

        [Test]
        public void Write_type_without_getter_throws() =>
            Write(new TypeWithoutGetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_type_without_setter() =>
            Write(new TypeWithoutSetter(), (execute, expected) => execute());

        [Test]
        public void Write_type_with_explicit_property_name() =>
            Write(new TypeWithExplicitPropertyName { MyValue = HelloSlonik }, (execute, expected) => Assert.That(execute().GetString(0), Is.EqualTo(expected.MyValue)));

        [Test]
        public void Write_type_with_explicit_parameter_name() =>
            Write(new TypeWithExplicitParameterName(HelloSlonik), (execute, expected) => Assert.That(execute().GetString(0), Is.EqualTo(expected.Value)));

        [Test]
        public void Write_type_with_more_properties_than_attributes() =>
            Write(new TypeWithMorePropertiesThanAttributes(), (execute, expected) => execute());

        [Test]
        public void Write_type_with_less_properties_than_attributes_throws() =>
            Write(new TypeWithLessPropertiesThanAttributes(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_type_with_less_parameters_than_attributes_throws() =>
            Write(new TypeWithLessParametersThanAttributes(TheAnswer), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_type_with_more_parameters_than_attributes_throws() =>
            Write(new TypeWithMoreParametersThanAttributes(TheAnswer, HelloSlonik), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));
    }
}
