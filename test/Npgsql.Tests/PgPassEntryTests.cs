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
        Assert.That("test", Is.EqualTo(entry.Host));
        Assert.That(1234, Is.EqualTo(entry.Port));
        Assert.That("test2", Is.EqualTo(entry.Database));
        Assert.That("test3", Is.EqualTo(entry.Username));
        Assert.That("test4", Is.EqualTo(entry.Password));
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
        Assert.That("t:est", Is.EqualTo(entry.Host));
        Assert.That(1234, Is.EqualTo(entry.Port));
        Assert.That("test2", Is.EqualTo(entry.Database));
        Assert.That("test3", Is.EqualTo(entry.Username));
        Assert.That("test\\4", Is.EqualTo(entry.Password));
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
