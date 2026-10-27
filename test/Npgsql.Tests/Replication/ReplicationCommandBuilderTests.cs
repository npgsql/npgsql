using Npgsql.Replication;
using NUnit.Framework;

namespace Npgsql.Tests.Replication;

class ReplicationCommandBuilderTests
{
    [TestCase("my_slot_1", ExpectedResult = "\"my_slot_1\"")]
    [TestCase("MySlot", ExpectedResult = "\"myslot\"")]
    [TestCase("1abc", ExpectedResult = "\"1abc\"", Description = "Valid slot names may start with a digit, which doesn't lex as an identifier when unquoted")]
    [TestCase("x TEMPORARY", ExpectedResult = "\"x temporary\"", Description = "A name can't add keywords to a command")]
    [TestCase("x\" WAIT", ExpectedResult = "\"x\"\" wait\"", Description = "A name can't end the quoted identifier")]
    [TestCase(" myslot\n", ExpectedResult = "\"myslot\"", Description = "The server skipped surrounding whitespace when names were sent unquoted")]
    [TestCase("\"1abc\"", ExpectedResult = "\"1abc\"", Description = "Names that callers quoted themselves keep working")]
    [TestCase("\"a\" WAIT \"b\"", ExpectedResult = "\"\"\"a\"\" wait \"\"b\"\"\"", Description = "A pre-quoted name can't contain other quotes")]
    [TestCase("ÄBC_Ü", ExpectedResult = "\"Äbc_Ü\"", Description = "Only A-Z are lowercased; valid slot names are ASCII-only anyway")]
    public string Slot_and_plugin_names_are_lowercased_and_quoted(string name)
        => ReplicationCommandBuilder.QuoteName(name);

    [TestCase("proto_version", ExpectedResult = "\"proto_version\"")]
    [TestCase("Mixed\"Case", ExpectedResult = "\"Mixed\"\"Case\"")]
    public string Option_names_are_quoted_identifiers(string name)
        => ReplicationCommandBuilder.QuoteIdentifier(name);

    [TestCase("on", ExpectedResult = "'on'")]
    [TestCase("it's", ExpectedResult = "'it''s'")]
    [TestCase(@"a \path", ExpectedResult = @"'a \path'", Description = "The replication command scanner doesn't support backslash escapes")]
    public string Option_values_are_literals(string value)
        => ReplicationCommandBuilder.QuoteLiteral(value);

    [Test, Description("Publication names are case-sensitive quoted identifiers; commas and quotes inside a name are preserved")]
    public void Publication_names()
    {
        Assert.That(ReplicationCommandBuilder.PublicationNames(["pub"]), Is.EqualTo("\"pub\""));
        Assert.That(ReplicationCommandBuilder.PublicationNames(["Pub", "a,b", "with \" and '"]),
            Is.EqualTo("\"Pub\",\"a,b\",\"with \"\" and '\""));
    }
}
