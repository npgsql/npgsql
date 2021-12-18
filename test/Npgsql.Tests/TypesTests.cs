using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests;

/// <summary>
/// Tests NpgsqlTypes.* independent of a database
/// </summary>
[TestFixture]
public class TypesTests
{
    [Test]
    public void TsVector()
    {
        NpgsqlTsVector vec;

        vec = NpgsqlTsVector.Parse("a");
        Assert.AreEqual("'a'", vec.ToString());

        vec = NpgsqlTsVector.Parse("a ");
        Assert.AreEqual("'a'", vec.ToString());

        vec = NpgsqlTsVector.Parse("a:1A");
        Assert.AreEqual("'a':1A", vec.ToString());

        vec = NpgsqlTsVector.Parse(@"\abc\def:1a ");
        Assert.AreEqual("'abcdef':1A", vec.ToString());

        vec = NpgsqlTsVector.Parse(@"abc:3A 'abc' abc:4B 'hello''yo' 'meh\'\\':5");
        Assert.AreEqual(@"'abc':3A,4B 'hello''yo' 'meh''\\':5", vec.ToString());

        vec = NpgsqlTsVector.Parse(" a:12345C  a:24D a:25B b c d 1 2 a:25A,26B,27,28");
        Assert.AreEqual("'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'", vec.ToString());
    }

    [Test]
    public void TsQuery()
    {
        NpgsqlTsQuery query;

        query = new NpgsqlTsQueryLexeme("a", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B);
        query = new NpgsqlTsQueryOr(query, query);
        query = new NpgsqlTsQueryOr(query, query);

        var str = query.ToString();

        query = NpgsqlTsQuery.Parse("a & b | c");
        Assert.AreEqual("'a' & 'b' | 'c'", query.ToString());

        query = NpgsqlTsQuery.Parse("'a''':*ab&d:d&!c");
        Assert.AreEqual("'a''':*AB & 'd':D & !'c'", query.ToString());

        query = NpgsqlTsQuery.Parse("(a & !(c | d)) & (!!a&b) | c | d | e");
        Assert.AreEqual("( ( 'a' & !( 'c' | 'd' ) & !( !'a' ) & 'b' | 'c' ) | 'd' ) | 'e'", query.ToString());
        Assert.AreEqual(query.ToString(), NpgsqlTsQuery.Parse(query.ToString()).ToString());

        query = NpgsqlTsQuery.Parse("(((a:*)))");
        Assert.AreEqual("'a':*", query.ToString());

        query = NpgsqlTsQuery.Parse(@"'a\\b''cde'");
        Assert.AreEqual(@"a\b'cde", ((NpgsqlTsQueryLexeme)query).Text);
        Assert.AreEqual(@"'a\\b''cde'", query.ToString());

        query = NpgsqlTsQuery.Parse(@"a <-> b");
        Assert.AreEqual("'a' <-> 'b'", query.ToString());

        query = NpgsqlTsQuery.Parse("((a & b) <5> c) <-> !d <0> e");
        Assert.AreEqual("( ( 'a' & 'b' <5> 'c' ) <-> !'d' ) <0> 'e'", query.ToString());

        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a b c & &"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("&"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("|"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("!"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("("));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse(")"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("()"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<-"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<->"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a <->"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<>"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a <a> b"));
        Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a <-1> b"));
    }

    [Test]
    public void TsQueryEquatibility()
    {
        //Debugger.Launch();
        AreEqual(
            new NpgsqlTsQueryLexeme("lexeme"),
            new NpgsqlTsQueryLexeme("lexeme"));

        AreEqual(
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B),
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B));

        AreEqual(
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B, true),
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B, true));

        AreEqual(
            new NpgsqlTsQueryNot(new NpgsqlTsQueryLexeme("not")),
            new NpgsqlTsQueryNot(new NpgsqlTsQueryLexeme("not")));

        AreEqual(
            new NpgsqlTsQueryAnd(new NpgsqlTsQueryLexeme("left"), new NpgsqlTsQueryLexeme("right")),
            new NpgsqlTsQueryAnd(new NpgsqlTsQueryLexeme("left"), new NpgsqlTsQueryLexeme("right")));

        AreEqual(
            new NpgsqlTsQueryOr(new NpgsqlTsQueryLexeme("left"), new NpgsqlTsQueryLexeme("right")),
            new NpgsqlTsQueryOr(new NpgsqlTsQueryLexeme("left"), new NpgsqlTsQueryLexeme("right")));

        AreEqual(
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 0, new NpgsqlTsQueryLexeme("right")),
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 0, new NpgsqlTsQueryLexeme("right")));

        AreEqual(
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 1, new NpgsqlTsQueryLexeme("right")),
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 1, new NpgsqlTsQueryLexeme("right")));

        AreEqual(
            new NpgsqlTsQueryEmpty(),
            new NpgsqlTsQueryEmpty());

        AreNotEqual(
            new NpgsqlTsQueryLexeme("lexeme a"),
            new NpgsqlTsQueryLexeme("lexeme b"));

        AreNotEqual(
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.D),
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B));

        AreNotEqual(
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B, true),
            new NpgsqlTsQueryLexeme("lexeme", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B, false));

        AreNotEqual(
            new NpgsqlTsQueryNot(new NpgsqlTsQueryLexeme("not")),
            new NpgsqlTsQueryNot(new NpgsqlTsQueryLexeme("ton")));

        AreNotEqual(
            new NpgsqlTsQueryAnd(new NpgsqlTsQueryLexeme("right"), new NpgsqlTsQueryLexeme("left")),
            new NpgsqlTsQueryAnd(new NpgsqlTsQueryLexeme("left"), new NpgsqlTsQueryLexeme("right")));

        AreNotEqual(
            new NpgsqlTsQueryOr(new NpgsqlTsQueryLexeme("right"), new NpgsqlTsQueryLexeme("left")),
            new NpgsqlTsQueryOr(new NpgsqlTsQueryLexeme("left"), new NpgsqlTsQueryLexeme("right")));

        AreNotEqual(
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("right"), 0, new NpgsqlTsQueryLexeme("left")),
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 0, new NpgsqlTsQueryLexeme("right")));

        AreNotEqual(
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 0, new NpgsqlTsQueryLexeme("right")),
            new NpgsqlTsQueryFollowedBy(new NpgsqlTsQueryLexeme("left"), 1, new NpgsqlTsQueryLexeme("right")));

        void AreEqual(NpgsqlTsQuery left, NpgsqlTsQuery right)
        {
            Assert.True(left == right);
            Assert.False(left != right);
            Assert.AreEqual(left, right);
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }

        void AreNotEqual(NpgsqlTsQuery left, NpgsqlTsQuery right)
        {
            Assert.False(left == right);
            Assert.True(left != right);
            Assert.AreNotEqual(left, right);
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }
    }

    [Test]
    public void TsQueryOperatorPrecedence()
    {
        var query = NpgsqlTsQuery.Parse("!a <-> b & c | d & e");
        var expectedGrouping = NpgsqlTsQuery.Parse("((!(a) <-> b) & c) | (d & e)");
        Assert.AreEqual(expectedGrouping.ToString(), query.ToString());
    }

    [Test]
    public void Bug1011018()
    {
        var p = new NpgsqlParameter();
        p.NpgsqlDbType = NpgsqlDbType.Time;
        p.Value = DateTime.Now;
        var o = p.Value;
    }

#pragma warning disable 618
    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/750")]
    public void NpgsqlInet()
    {
        var v = new NpgsqlInet(IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"), 32);
        Assert.That(v.ToString(), Is.EqualTo("2001:1db8:85a3:1142:1000:8a2e:1370:7334/32"));

#pragma warning disable CS8625
        Assert.That(v != null);  // #776
#pragma warning disable CS8625
    }
#pragma warning restore 618
}