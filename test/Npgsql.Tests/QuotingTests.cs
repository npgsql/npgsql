using Npgsql.Util;
using NUnit.Framework;
using System.Text;

namespace Npgsql.Tests;

class QuotingTests
{
    [TestCase("my_slot_1", ExpectedResult = "\"my_slot_1\"")]
    [TestCase("MySlot", ExpectedResult = "\"myslot\"")]
    [TestCase("1abc", ExpectedResult = "\"1abc\"", Description = "Valid slot names may start with a digit, which doesn't lex as an identifier when unquoted")]
    [TestCase("x TEMPORARY", ExpectedResult = "\"x temporary\"", Description = "A name can't add keywords to a command")]
    [TestCase("x\" WAIT", ExpectedResult = "\"x\"\" wait\"", Description = "A name can't end the quoted identifier")]
    [TestCase(" myslot\n", ExpectedResult = "\"myslot\"", Description = "The server skipped surrounding whitespace when names were sent unquoted")]
    [TestCase("\"1abc\"", ExpectedResult = "\"1abc\"", Description = "Names that callers quoted themselves keep working")]
    [TestCase("\"a\" WAIT \"b\"", ExpectedResult = "\"\"\"a\"\" wait \"\"b\"\"\"", Description = "A pre-quoted name can't contain quotes that aren't doubled")]
    [TestCase("\"a\"\"b\"", ExpectedResult = "\"a\"\"b\"", Description = "Doubled quotes keep a pre-quoted name one identifier")]
    [TestCase("\"a\"\"\"b\"", ExpectedResult = "\"\"\"a\"\"\"\"\"\"b\"\"\"", Description = "Three quotes in a row aren't all doubled")]
    [TestCase("\"a\"\"", ExpectedResult = "\"\"\"a\"\"\"\"\"", Description = "Neither is a quote right before the closing one")]
    [TestCase("ÄBC_Ü", ExpectedResult = "ÄBC_Ü", Description = "The server folds non-ASCII letters by its encoding and locale, so an identifier with them stays unquoted")]
    [TestCase("Ä$1", ExpectedResult = "Ä$1", Description = "Digits and $ can follow the first character of an identifier")]
    [TestCase("1Ä", ExpectedResult = "\"1Ä\"", Description = "A name with non-ASCII letters that isn't an identifier is quoted")]
    [TestCase("Ä WAIT", ExpectedResult = "\"Ä wait\"", Description = "So that it can't add keywords to a command either")]
    public string Slot_and_plugin_names_are_lowercased_and_quoted(string name)
        => Statics.QuoteName(name);

    [TestCase(" MySlot ", ExpectedResult = "myslot")]
    [TestCase("\"a\"\"b\"", ExpectedResult = "a\"b", Description = "A pre-quoted name loses its quotes, and doubled quotes become one")]
    public string Names_as_the_server_reads_them(string name)
        => Statics.UnquoteName(Statics.QuoteName(name));

    [Test, Description("A name with a non-ASCII letter is sent unquoted, and it can only find a slot if the server cuts the letter off")]
    public void Slot_name_with_a_non_ASCII_letter_beyond_the_limit()
        => Assert.That(
            Statics.TruncateIdentifier(Statics.UnquoteName(Statics.QuoteName(new string('A', 63) + "é")), 63),
            Is.EqualTo(new string('a', 63)));

    [Test, Description("A physical walsender cuts after max_identifier_length bytes, but if that splits a character, the command fails")]
    public void Identifiers_are_truncated_without_splitting_a_character()
    {
        var a63 = new string('a', 63);
        Assert.That(Statics.TruncateIdentifier(a63 + "b", 63), Is.EqualTo(a63));
        Assert.That(Statics.TruncateIdentifier(a63, 63), Is.SameAs(a63));
        Assert.That(Statics.TruncateIdentifier(new string('a', 62) + "ÄÖ", 63), Is.EqualTo(new string('a', 62)));
    }

    [TestCase("proto_version", ExpectedResult = "\"proto_version\"")]
    [TestCase("Mixed\"Case", ExpectedResult = "\"Mixed\"\"Case\"")]
    public string Option_names_are_quoted_identifiers(string name)
        => Statics.QuoteIdentifier(name);

    [TestCase("on", ExpectedResult = "'on'")]
    [TestCase("it's", ExpectedResult = "'it''s'")]
    [TestCase(@"a \path", ExpectedResult = @"'a \path'", Description = "The replication command scanner doesn't support backslash escapes")]
    public string Option_values_are_literals(string value)
        => QuoteLiteralBothWays(value, replicationParser: true);

    [TestCase("on", ExpectedResult = "'on'")]
    [TestCase("it's", ExpectedResult = "'it''s'")]
    [TestCase(@"a \path", ExpectedResult = @" E'a \\path'", Description = "An escape string reads the same whatever standard_conforming_strings is")]
    [TestCase(@"it's \", ExpectedResult = @" E'it''s \\'")]
    public string Sql_literals(string value)
        => QuoteLiteralBothWays(value, replicationParser: false);

    // AppendQuotedLiteral must append exactly what QuoteLiteral returns
    static string QuoteLiteralBothWays(string value, bool replicationParser)
    {
        var quoted = Statics.QuoteLiteral(value, replicationParser);
        Assert.That(new StringBuilder("x").AppendQuotedLiteral(value, replicationParser).ToString(), Is.EqualTo("x" + quoted));
        return quoted;
    }

    [Test, Description("Publication names are case-sensitive quoted identifiers; commas and quotes inside a name are preserved")]
    public void Publication_names()
    {
        Assert.That(Statics.QuoteIdentifierList(["pub"]), Is.EqualTo("\"pub\""));
        Assert.That(Statics.QuoteIdentifierList(["Pub", "a,b", "with \" and '"]),
            Is.EqualTo("\"Pub\",\"a,b\",\"with \"\" and '\""));
    }
}
