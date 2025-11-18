using System;
using Npgsql.Internal.Postgres;
using NUnit.Framework;

namespace Npgsql.Tests;

public class DataTypeNameTests
{
    [Test]
    public void MaxLengthDataTypeName()
    {
        var name = new string('a', DataTypeName.NAMEDATALEN);
        var fullyQualifiedDataTypeName= $"public.{name}";
        Assert.DoesNotThrow(() => new DataTypeName(fullyQualifiedDataTypeName));
        Assert.That(fullyQualifiedDataTypeName, Is.EqualTo(new DataTypeName(fullyQualifiedDataTypeName).Value));
    }

    [Test]
    public void TooLongDataTypeName()
    {
        var name = new string('a', DataTypeName.NAMEDATALEN + 1);
        var fullyQualifiedDataTypeName= $"public.{name}";
        var exception = Assert.Throws<ArgumentException>(() => new DataTypeName(fullyQualifiedDataTypeName));
        Assert.That(exception!.Message, Does.EndWith($": public.{new string('a', DataTypeName.NAMEDATALEN)}"));
    }

    [TestCase("public.name", ExpectedResult = "public._name")]
    [TestCase("public._name", ExpectedResult = "public._name")]
    [TestCase("public.zzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa123", ExpectedResult = "public._zzzaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa12")]
    public string ToArrayName(string name)
        => new DataTypeName(name).ToArrayName();

    [TestCase("public.multirange", ExpectedResult = "public.multirange")]
    [TestCase("public.abcmultirange123", ExpectedResult = "public.abcmultirange123")]
    [TestCase("public.multiRANGE", ExpectedResult = "public.multiRANGE_multirange")]
    public string ToDefaultMultirangeNameHasMultiRange(string name)
        => new DataTypeName(name).ToDefaultMultirangeName();

    [TestCase("public.range", ExpectedResult = "public.multirange")]
    [TestCase("public.abcrange123", ExpectedResult = "public.abcmultirange123")]
    [TestCase("public.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaarange", ExpectedResult = "public.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaamultirange")] // Replace goes to max length
    [TestCase("public.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaarange1", ExpectedResult = "public.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaamultir")] // Replace goes over max length
    [TestCase("public.RANGE", ExpectedResult = "public.RANGE_multirange")]
    public string ToDefaultMultirangeNameHasRange(string name)
        => new DataTypeName(name).ToDefaultMultirangeName();

    [TestCase("public.name", null, ExpectedResult = "public.name")]
    [TestCase("public._name", null, ExpectedResult = "public._name")]
    [TestCase("public.name[]", null, ExpectedResult = "public._name")]
    [TestCase("public.integer", null, ExpectedResult = "public.integer")]
    [TestCase("name", null, ExpectedResult = "pg_catalog.name")]
    [TestCase("_name", null, ExpectedResult = "pg_catalog._name")]
    [TestCase("name[]", null, ExpectedResult = "pg_catalog._name")]
    [TestCase("mytype", null, ExpectedResult = "-.mytype")]
    [TestCase("_mytype", null, ExpectedResult = "-._mytype")]
    [TestCase("mytype[]", null, ExpectedResult = "-._mytype")]
    [TestCase("character varying", null, ExpectedResult = "pg_catalog.varchar")]
    [TestCase("decimal(facet_name)", null, ExpectedResult = "pg_catalog.numeric")]
    [TestCase("name", "public", ExpectedResult = "public.name")]
    [TestCase("name ", "public", ExpectedResult = "public.name")]
    [TestCase("_name", "public", ExpectedResult = "public._name")]
    [TestCase("name[]", "public", ExpectedResult = "public._name")]
    [TestCase("timestamp with time zone", "public", ExpectedResult = "public.timestamptz")]
    [TestCase("boolean(facet_name)", "public", ExpectedResult = "public.bool")]
    [TestCase(" public.name ", null, ExpectedResult = "public.name")]
    public string FromDisplayName(string name, string? schema)
    => DataTypeName.FromDisplayName(name, schema).Value;
}
