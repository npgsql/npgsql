using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public partial class CompositeHandlerTests
{
    async Task Write<T>(Action<NpgsqlDataReader, T> assert, string? schema = null)
        where T : IComposite, IInitializable, new()
    {
        var composite = new T();
        composite.Initialize();
        await Write(composite, assert, schema);
    }

    async Task Write<T>(T composite, Action<NpgsqlDataReader, T>? assert = null, string? schema = null)
        where T : IComposite
    {
        await using var dataSource = await OpenAndMapComposite(composite, schema, nameof(Write), out var _);
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand("SELECT (@c).*", connection);

        command.Parameters.AddWithValue("c", composite);
        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (assert is not null)
            assert(reader, composite);
    }

    [Test]
    public Task Write_class_with_property()
        => Write<ClassWithProperty>((reader, expected) => Assert.AreEqual(expected.Value, reader.GetString(0)));

    [Test]
    public Task Write_class_with_field()
        => Write<ClassWithField>((reader, expected) => Assert.AreEqual(expected.Value, reader.GetString(0)));

    [Test]
    public Task Write_struct_with_property()
        => Write<StructWithProperty>((reader, expected) => Assert.AreEqual(expected.Value, reader.GetString(0)));

    [Test]
    public Task Write_struct_with_field()
        => Write<StructWithField>((reader, expected) => Assert.AreEqual(expected.Value, reader.GetString(0)));

    [Test]
    public Task Write_type_with_two_properties()
        => Write<TypeWithTwoProperties>((reader, expected) =>
        {
            Assert.AreEqual(expected.IntValue, reader.GetInt32(0));
            Assert.AreEqual(expected.StringValue, reader.GetString(1));
        });

    [Test]
    public Task Write_type_with_two_properties_inverted()
        => Write<TypeWithTwoPropertiesReversed>((reader, expected) =>
        {
            Assert.AreEqual(expected.IntValue, reader.GetInt32(1));
            Assert.AreEqual(expected.StringValue, reader.GetString(0));
        });

    [Test]
    public void Write_type_with_private_property_throws()
        => Assert.ThrowsAsync<InvalidOperationException>(async () => await Write(new TypeWithPrivateProperty()));

    [Test]
    public void Write_type_with_private_getter_throws()
        => Assert.ThrowsAsync<InvalidOperationException>(async () => await Write(new TypeWithPrivateGetter()));

    [Test]
    public Task Write_type_with_private_setter()
        => Write(new TypeWithPrivateSetter());

    [Test]
    public void Write_type_without_getter_throws()
        => Assert.ThrowsAsync<InvalidOperationException>(async () => await Write(new TypeWithoutGetter()));

    [Test]
    public Task Write_type_without_setter() =>
        Write(new TypeWithoutSetter());

    [Test]
    public Task Write_type_with_explicit_property_name()
        => Write(new TypeWithExplicitPropertyName { MyValue = HelloSlonik }, (reader, expected) => Assert.That(reader.GetString(0), Is.EqualTo(expected.MyValue)));

    [Test]
    public Task Write_type_with_explicit_parameter_name()
        => Write(new TypeWithExplicitParameterName(HelloSlonik), (reader, expected) => Assert.That(reader.GetString(0), Is.EqualTo(expected.Value)));

    [Test]
    public Task Write_type_with_more_properties_than_attributes()
        => Write(new TypeWithMorePropertiesThanAttributes());

    [Test]
    public void Write_type_with_less_properties_than_attributes_throws()
        => Assert.ThrowsAsync<InvalidOperationException>(async () => await Write(new TypeWithLessPropertiesThanAttributes()));

    [Test]
    public void Write_type_with_less_parameters_than_attributes_throws()
        => Assert.ThrowsAsync<InvalidOperationException>(async () => await Write(new TypeWithMoreParametersThanAttributes(TheAnswer, HelloSlonik)));

    [Test]
    public void Write_type_with_more_parameters_than_attributes_throws()
        => Assert.ThrowsAsync<InvalidOperationException>(async () => await Write(new TypeWithLessParametersThanAttributes(TheAnswer)));
}
