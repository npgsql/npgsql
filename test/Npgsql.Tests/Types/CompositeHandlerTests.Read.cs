using System;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public partial class CompositeHandlerTests
    {
        void Read<T>(Action<Func<T>, T> assert, string? schema = null)
            where T: IComposite, IInitializable, new()
        {
            var composite = new T();
            composite.Initialize();
            Read(composite, assert, schema);
        }

        void Read<T>(T composite, Action<Func<T>, T> assert, string? schema = null)
            where T : IComposite, new()
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
            Read<TypeWithTwoPropertiesInverted>((execute, expected) =>
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
    }
}
