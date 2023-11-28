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
        Assert.AreEqual(new DataTypeName(fullyQualifiedDataTypeName).Value, fullyQualifiedDataTypeName);
    }

    [Test]
    public void TooLongDataTypeName()
    {
        var name = new string('a', DataTypeName.NAMEDATALEN + 1);
        var fullyQualifiedDataTypeName= $"public.{name}";
        var exception = Assert.Throws<ArgumentException>(() => new DataTypeName(fullyQualifiedDataTypeName));
        Assert.That(exception!.Message, Does.EndWith($": public.{new string('a', DataTypeName.NAMEDATALEN)}"));
    }
}
