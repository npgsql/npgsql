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
        public void Write_ClassWithProperty_Succeeds() =>
            Write<ClassWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_ClassWithField_Succeeds() =>
            Write<ClassWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_StructWithProperty_Succeeds() =>
            Write<StructWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_StructWithField_Succeeds() =>
            Write<StructWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().GetString(0)));

        [Test]
        public void Write_TypeWithTwoProperties_Succeeds() =>
            Write<TypeWithTwoProperties>((execute, expected) =>
            {
                var actual = execute();
                Assert.AreEqual(expected.IntValue, actual.GetInt32(0));
                Assert.AreEqual(expected.StringValue, actual.GetString(1));
            });

        [Test]
        public void Write_TypeWithTwoPropertiesInverted_Succeeds() =>
            Write<TypeWithTwoPropertiesReversed>((execute, expected) =>
            {
                var actual = execute();
                Assert.AreEqual(expected.IntValue, actual.GetInt32(1));
                Assert.AreEqual(expected.StringValue, actual.GetString(0));
            });

        [Test]
        public void Write_TypeWithPrivateProperty_ThrowsInvalidOperationException() =>
            Write(new TypeWithPrivateProperty(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_TypeWithPrivateGetter_ThrowsInvalidOperationException() =>
            Write(new TypeWithPrivateGetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_TypeWithPrivateSetter_Succeeds() =>
            Write(new TypeWithPrivateSetter(), (execute, expected) => execute());

        [Test]
        public void Write_TypeWithoutGetter_ThrowsInvalidOperationException() =>
            Write(new TypeWithoutGetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_TypeWithoutSetter_Succeeds() =>
            Write(new TypeWithoutSetter(), (execute, expected) => execute());

        [Test]
        public void Write_TypeWithExplicitPropertyName_Succeeds() =>
            Write(new TypeWithExplicitPropertyName { MyValue = HelloSlonik }, (execute, expected) => Assert.That(execute().GetString(0), Is.EqualTo(expected.MyValue)));

        [Test]
        public void Write_TypeWithExplicitParameterName_Succeeds() =>
            Write(new TypeWithExplicitParameterName(HelloSlonik), (execute, expected) => Assert.That(execute().GetString(0), Is.EqualTo(expected.Value)));

        [Test]
        public void Write_TypeWithMorePropertiesThanAttributes_Succeeds() =>
            Write(new TypeWithMorePropertiesThanAttributes(), (execute, expected) => execute());

        [Test]
        public void Write_TypeWithLessPropertiesThanAttributes_ThrowsInvalidOperationException() =>
            Write(new TypeWithLessPropertiesThanAttributes(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_TypeWithLessParametersThanAttributes_ThrowsInvalidOperationException() =>
            Write(new TypeWithLessParametersThanAttributes(TheAnswer), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Write_TypeWithMoreParametersThanAttributes_ThrowsInvalidOperationException() =>
            Write(new TypeWithMoreParametersThanAttributes(TheAnswer, HelloSlonik), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));
    }
}
