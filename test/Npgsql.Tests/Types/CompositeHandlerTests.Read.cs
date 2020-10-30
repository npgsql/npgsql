using System;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public partial class CompositeHandlerTests
    {
        void Read<T>(Action<Func<T>, T> assert, string? schema = null)
            where T : IComposite, IInitializable, new()
        {
            var composite = new T();
            composite.Initialize();
            Read(composite, assert, schema);
        }

        void Read<T>(T composite, Action<Func<T>, T> assert, string? schema = null)
            where T : IComposite
        {
            using var connection = OpenAndMapComposite(composite, schema, nameof(Read), out var name);
            using var command = new NpgsqlCommand($"SELECT ROW({composite.GetValues()})::{name}", connection);
            using var reader = command.ExecuteReader();

            reader.Read();
            assert(() => reader.GetFieldValue<T>(0), composite);
        }

        [Test]
        public void Read_ClassWithProperty_Succeeds() =>
            Read<ClassWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

        [Test]
        public void Read_ClassWithField_Succeeds() =>
            Read<ClassWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

        [Test]
        public void Read_StructWithProperty_Succeeds() =>
            Read<StructWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

        [Test]
        public void Read_StructWithField_Succeeds() =>
            Read<StructWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

        [Test]
        public void Read_TypeWithTwoProperties_Succeeds() =>
            Read<TypeWithTwoProperties>((execute, expected) =>
            {
                var actual = execute();
                Assert.AreEqual(expected.IntValue, actual.IntValue);
                Assert.AreEqual(expected.StringValue, actual.StringValue);
            });

        [Test]
        public void Read_TypeWithTwoPropertiesInverted_Succeeds() =>
            Read<TypeWithTwoPropertiesReversed>((execute, expected) =>
            {
                var actual = execute();
                Assert.AreEqual(expected.IntValue, actual.IntValue);
                Assert.AreEqual(expected.StringValue, actual.StringValue);
            });

        [Test]
        public void Read_TypeWithPrivateProperty_ThrowsInvalidOperationException() =>
            Read(new TypeWithPrivateProperty(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Read_TypeWithPrivateGetter_Succeeds() =>
            Read(new TypeWithPrivateGetter(), (execute, expected) => execute());

        [Test]
        public void Read_TypeWithPrivateSetter_ThrowsInvalidOperationException() =>
            Read(new TypeWithPrivateSetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Read_TypeWithoutGetter_Succeeds() =>
            Read(new TypeWithoutGetter(), (execute, expected) => execute());

        [Test]
        public void Read_TypeWithoutSetter_ThrowsInvalidOperationException() =>
            Read(new TypeWithoutSetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Read_TypeWithExplicitPropertyName_Succeeds() =>
            Read(new TypeWithExplicitPropertyName { MyValue = HelloSlonik }, (execute, expected) => Assert.That(execute().MyValue, Is.EqualTo(expected.MyValue)));

        [Test]
        public void Read_TypeWithExplicitParameterName_Succeeds() =>
            Read(new TypeWithExplicitParameterName(HelloSlonik), (execute, expected) => Assert.That(execute().Value, Is.EqualTo(expected.Value)));

        [Test]
        public void Read_TypeWithMorePropertiesThanAttributes_Succeeds() =>
             Read(new TypeWithMorePropertiesThanAttributes(), (execute, expected) =>
             {
                 var actual = execute();
                 Assert.That(actual.IntValue, Is.Not.Null);
                 Assert.That(actual.StringValue, Is.Null);
             });

        [Test]
        public void Read_TypeWithLessPropertiesThanAttributes_ThrowsInvalidOperationException() =>
            Read(new TypeWithLessPropertiesThanAttributes(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Read_TypeWithLessParametersThanAttributes_ThrowsInvalidOperationException() =>
            Read(new TypeWithLessParametersThanAttributes(TheAnswer), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Read_TypeWithMoreParametersThanAttributes_ThrowsInvalidOperationException() =>
            Read(new TypeWithMoreParametersThanAttributes(TheAnswer, HelloSlonik), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

        [Test]
        public void Read_TypeWithOneParameter_Succeeds() =>
            Read(new TypeWithOneParameter(1), (execute, expected) => Assert.That(execute().Value1, Is.EqualTo(expected.Value1)));

        [Test]
        public void Read_TypeWithTwoParameters_Succeeds() =>
            Read(new TypeWithTwoParameters(TheAnswer, HelloSlonik), (execute, expected) =>
            {
                var actual = execute();
                Assert.That(actual.IntValue, Is.EqualTo(expected.IntValue));
                Assert.That(actual.StringValue, Is.EqualTo(expected.StringValue));
            });

        [Test]
        public void Read_TypeWithTwoParametersReversed_Succeeds() =>
            Read(new TypeWithTwoParametersReversed(HelloSlonik, TheAnswer), (execute, expected) =>
            {
                var actual = execute();
                Assert.That(actual.IntValue, Is.EqualTo(expected.IntValue));
                Assert.That(actual.StringValue, Is.EqualTo(expected.StringValue));
            });

        [Test]
        public void Read_TypeWithNineParameters_Succeeds() =>
            Read(new TypeWithNineParameters(1, 2, 3, 4, 5, 6, 7, 8, 9), (execute, expected) =>
            {
                var actual = execute();
                Assert.That(actual.Value1, Is.EqualTo(expected.Value1));
                Assert.That(actual.Value2, Is.EqualTo(expected.Value2));
                Assert.That(actual.Value3, Is.EqualTo(expected.Value3));
                Assert.That(actual.Value4, Is.EqualTo(expected.Value4));
                Assert.That(actual.Value5, Is.EqualTo(expected.Value5));
                Assert.That(actual.Value6, Is.EqualTo(expected.Value6));
                Assert.That(actual.Value7, Is.EqualTo(expected.Value7));
                Assert.That(actual.Value8, Is.EqualTo(expected.Value8));
                Assert.That(actual.Value9, Is.EqualTo(expected.Value9));
            });
    }
}
