using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public partial class CompositeHandlerTests
{
    async Task Read<T>(Action<Func<T>, T> assert, string? schema = null)
        where T : IComposite, IInitializable, new()
    {
        var composite = new T();
        composite.Initialize();
        await Read(composite, assert, schema);
    }

    async Task Read<T>(T composite, Action<Func<T>, T> assert, string? schema = null)
        where T : IComposite
    {
        await using var dataSource = await OpenAndMapComposite(composite, schema, nameof(Read), out var name);
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommandOrig($"SELECT ROW({composite.GetValues()})::{name}", connection);
        await using var reader = command.ExecuteReader();

        await reader.ReadAsync();
        assert(() => reader.GetFieldValue<T>(0), composite);
    }

    [Test]
    public Task Read_class_with_property() =>
        Read<ClassWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

    [Test]
    public Task Read_class_with_field() =>
        Read<ClassWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

    [Test]
    public Task Read_struct_with_property() =>
        Read<StructWithProperty>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

    [Test]
    public Task Read_struct_with_field() =>
        Read<StructWithField>((execute, expected) => Assert.AreEqual(expected.Value, execute().Value));

    [Test]
    public Task Read_type_with_two_properties() =>
        Read<TypeWithTwoProperties>((execute, expected) =>
        {
            var actual = execute();
            Assert.AreEqual(expected.IntValue, actual.IntValue);
            Assert.AreEqual(expected.StringValue, actual.StringValue);
        });

    [Test]
    public Task Read_type_with_two_properties_inverted() =>
        Read<TypeWithTwoPropertiesReversed>((execute, expected) =>
        {
            var actual = execute();
            Assert.AreEqual(expected.IntValue, actual.IntValue);
            Assert.AreEqual(expected.StringValue, actual.StringValue);
        });

    [Test]
    public Task Read_type_with_private_property_throws() =>
        Read(new TypeWithPrivateProperty(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

    [Test]
    public Task Read_type_with_private_getter() =>
        Read(new TypeWithPrivateGetter(), (execute, expected) => execute());

    [Test]
    public Task Read_type_with_private_setter_throws() =>
        Read(new TypeWithPrivateSetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

    [Test]
    public Task Read_type_without_getter() =>
        Read(new TypeWithoutGetter(), (execute, expected) => execute());

    [Test]
    public Task Read_type_without_setter_throws() =>
        Read(new TypeWithoutSetter(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

    [Test]
    public Task Read_type_with_explicit_property_name() =>
        Read(new TypeWithExplicitPropertyName { MyValue = HelloSlonik }, (execute, expected) => Assert.That(execute().MyValue, Is.EqualTo(expected.MyValue)));

    [Test]
    public Task Read_type_with_explicit_parameter_name() =>
        Read(new TypeWithExplicitParameterName(HelloSlonik), (execute, expected) => Assert.That(execute().Value, Is.EqualTo(expected.Value)));

    [Test]
    public Task Read_type_with_more_properties_than_attributes() =>
        Read(new TypeWithMorePropertiesThanAttributes(), (execute, expected) =>
        {
            var actual = execute();
            Assert.That(actual.IntValue, Is.Not.Null);
            Assert.That(actual.StringValue, Is.Null);
        });

    [Test]
    public Task Read_type_with_less_properties_than_attributes_throws() =>
        Read(new TypeWithLessPropertiesThanAttributes(), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

    [Test]
    public Task Read_type_with_less_parameters_than_attributes_throws() =>
        Read(new TypeWithLessParametersThanAttributes(TheAnswer), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

    [Test]
    public Task Read_type_with_more_parameters_than_attributes_throws() =>
        Read(new TypeWithMoreParametersThanAttributes(TheAnswer, HelloSlonik), (execute, expected) => Assert.Throws<InvalidOperationException>(() => execute()));

    [Test]
    public Task Read_type_with_one_parameter() =>
        Read(new TypeWithOneParameter(1), (execute, expected) => Assert.That(execute().Value1, Is.EqualTo(expected.Value1)));

    [Test]
    public Task Read_type_with_two_parameters() =>
        Read(new TypeWithTwoParameters(TheAnswer, HelloSlonik), (execute, expected) =>
        {
            var actual = execute();
            Assert.That(actual.IntValue, Is.EqualTo(expected.IntValue));
            Assert.That(actual.StringValue, Is.EqualTo(expected.StringValue));
        });

    [Test]
    public Task Read_type_with_two_parameters_reversed() =>
        Read(new TypeWithTwoParametersReversed(HelloSlonik, TheAnswer), (execute, expected) =>
        {
            var actual = execute();
            Assert.That(actual.IntValue, Is.EqualTo(expected.IntValue));
            Assert.That(actual.StringValue, Is.EqualTo(expected.StringValue));
        });

    [Test]
    public Task Read_type_with_nine_parameters() =>
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
