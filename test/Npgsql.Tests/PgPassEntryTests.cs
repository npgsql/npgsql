using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests;

public class PgPassEntryTests
{
    [Test]
    public void Parses_well_formed_entry()
    {
        var input = "test:1234:test2:test3:test4";
        var entry = PgPassFile.Entry.Parse(input);

        Assert.That(entry, Is.Not.Null);
        Assert.That(entry.Host, Is.EqualTo("test"));
        Assert.That(entry.Port, Is.EqualTo(1234));
        Assert.That(entry.Database, Is.EqualTo("test2"));
        Assert.That(entry.Username, Is.EqualTo("test3"));
        Assert.That(entry.Password, Is.EqualTo("test4"));
    }

    [Test]
    [TestCase("test:1234:test2:test3")]
    [TestCase("test:myport:test2:test3:test4")]
    public void Bad_entry_throws(string input)
    {
        ActualValueDelegate<object> createDelegate = () => PgPassFile.Entry.Parse(input);
        Assert.That(createDelegate, Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Escaped_characters()
    {
        var input = "t\\:est:1234:test2:test3:test\\\\4";
        var entry = PgPassFile.Entry.Parse(input);

        Assert.That(entry, Is.Not.Null);
        Assert.That(entry.Host, Is.EqualTo("t:est"));
        Assert.That(entry.Port, Is.EqualTo(1234));
        Assert.That(entry.Database, Is.EqualTo("test2"));
        Assert.That(entry.Username, Is.EqualTo("test3"));
        Assert.That(entry.Password, Is.EqualTo("test\\4"));
    }

    [Test]
    public void Match_true_for_exact_match()
    {
        var input = "test:1234:test2:test3:test4";
        var entry = PgPassFile.Entry.Parse(input);

        var isMatch = entry.IsMatch("test", 1234, "test2", "test3");

        Assert.That(isMatch, Is.True);
    }

    [Test]
    public void Match_true_for_wildcard_entry()
    {
        var input = "*:1234:test2:test3:test4";
        var entry = PgPassFile.Entry.Parse(input);

        var isMatch = entry.IsMatch("test", 1234, "test2", "test3");

        Assert.That(isMatch, Is.True);
    }

    [Test]
    public void Match_true_for_wildcard_query()
    {
        var input = "test:1234:test2:test3:test4";
        var entry = PgPassFile.Entry.Parse(input);

        var isMatch = entry.IsMatch(null, 1234, "test2", "test3");

        Assert.That(isMatch, Is.True);
    }

    [Test]
    public void Match_false_for_bad_query()
    {
        var input = "test:1234:test2:test3:test4";
        var entry = PgPassFile.Entry.Parse(input);

        var isMatch = entry.IsMatch("notamatch", 1234, "test2", "test3");

        Assert.That(isMatch, Is.False);
    }

    [Test]
    public void Match_true_for_null_query()
    {
        var input = "test:1234:test2:test3:test4";
        var entry = PgPassFile.Entry.Parse(input);

        var isMatch = entry.IsMatch(null, 1234, "test2", "test3");

        Assert.That(isMatch, Is.True);
    }
}
