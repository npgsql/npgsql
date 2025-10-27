using System;
using System.Net;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests;

/// <summary>
/// Tests NpgsqlTypes.* independent of a database
/// </summary>
public class TypesTests
{
#pragma warning disable CS0618 // {NpgsqlTsVector,NpgsqlTsQuery}.Parse are obsolete
    [Test]
    public void TsVector()
    {
        NpgsqlTsVector vec;

        vec = NpgsqlTsVector.Parse("a");
        Assert.That(vec.ToString(), Is.EqualTo("'a'"));

        vec = NpgsqlTsVector.Parse("a ");
        Assert.That(vec.ToString(), Is.EqualTo("'a'"));

        vec = NpgsqlTsVector.Parse("a:1A");
        Assert.That(vec.ToString(), Is.EqualTo("'a':1A"));

        vec = NpgsqlTsVector.Parse(@"\abc\def:1a ");
        Assert.That(vec.ToString(), Is.EqualTo("'abcdef':1A"));

        vec = NpgsqlTsVector.Parse(@"abc:3A 'abc' abc:4B 'hello''yo' 'meh\'\\':5");
        Assert.That(vec.ToString(), Is.EqualTo(@"'abc':3A,4B 'hello''yo' 'meh''\\':5"));

        vec = NpgsqlTsVector.Parse(" a:12345C  a:24D a:25B b c d 1 2 a:25A,26B,27,28");
        Assert.That(vec.ToString(), Is.EqualTo("'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'"));
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
        Assert.That(query.ToString(), Is.EqualTo("'a' & 'b' | 'c'"));

        query = NpgsqlTsQuery.Parse("'a''':*ab&d:d&!c");
        Assert.That(query.ToString(), Is.EqualTo("'a''':*AB & 'd':D & !'c'"));

        query = NpgsqlTsQuery.Parse("(a & !(c | d)) & (!!a&b) | c | d | e");
        Assert.That(query.ToString(), Is.EqualTo("( ( 'a' & !( 'c' | 'd' ) & !( !'a' ) & 'b' | 'c' ) | 'd' ) | 'e'"));
        Assert.That(NpgsqlTsQuery.Parse(query.ToString()).ToString(), Is.EqualTo(query.ToString()));

        query = NpgsqlTsQuery.Parse("(((a:*)))");
        Assert.That(query.ToString(), Is.EqualTo("'a':*"));

        query = NpgsqlTsQuery.Parse(@"'a\\b''cde'");
        Assert.That(((NpgsqlTsQueryLexeme)query).Text, Is.EqualTo(@"a\b'cde"));
        Assert.That(query.ToString(), Is.EqualTo(@"'a\\b''cde'"));

        query = NpgsqlTsQuery.Parse(@"a <-> b");
        Assert.That(query.ToString(), Is.EqualTo("'a' <-> 'b'"));

        query = NpgsqlTsQuery.Parse("((a & b) <5> c) <-> !d <0> e");
        Assert.That(query.ToString(), Is.EqualTo("( ( 'a' & 'b' <5> 'c' ) <-> !'d' ) <0> 'e'"));

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
#pragma warning restore CS0618 // {NpgsqlTsVector,NpgsqlTsQuery}.Parse are obsolete

    [Test]
    public void TsVector_empty()
    {
        Assert.That(NpgsqlTsVector.Empty, Is.Empty);
        Assert.That(NpgsqlTsVector.Empty.ToString(), Is.Empty);
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
            Assert.That(left == right);
            Assert.That(left != right, Is.False);
            Assert.That(right, Is.EqualTo(left));
            Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        }

        void AreNotEqual(NpgsqlTsQuery left, NpgsqlTsQuery right)
        {
            Assert.That(left == right, Is.False);
            Assert.That(left != right);
            Assert.That(right, Is.Not.EqualTo(left));
            Assert.That(right.GetHashCode(), Is.Not.EqualTo(left.GetHashCode()));
        }
    }

#pragma warning disable CS0618 // {NpgsqlTsVector,NpgsqlTsQuery}.Parse are obsolete
    [Test]
    public void TsQueryOperatorPrecedence()
    {
        var query = NpgsqlTsQuery.Parse("!a <-> b & c | d & e");
        var expectedGrouping = NpgsqlTsQuery.Parse("((!(a) <-> b) & c) | (d & e)");
        Assert.That(query.ToString(), Is.EqualTo(expectedGrouping.ToString()));
    }
#pragma warning restore CS0618 // {NpgsqlTsVector,NpgsqlTsQuery}.Parse are obsolete

    [Test]
    public void NpgsqlPath_empty()
        => Assert.That(new NpgsqlPath { new(1, 2) }, Is.EqualTo(new NpgsqlPath(new NpgsqlPoint(1, 2))));

    [Test]
    public void NpgsqlPolygon_empty()
        => Assert.That(new NpgsqlPolygon { new(1, 2) }, Is.EqualTo(new NpgsqlPolygon(new NpgsqlPoint(1, 2))));

    [Test]
    public void NpgsqlPath_default()
    {
        NpgsqlPath defaultPath = default;
        Assert.That(defaultPath.Equals([new(1, 2)]), Is.False);
    }

    [Test]
    public void NpgsqlPolygon_default()
    {
        NpgsqlPolygon defaultPolygon = default;
        Assert.That(defaultPolygon.Equals([new(1, 2)]), Is.False);
    }

    [Test]
    public void Bug1011018()
    {
        var p = new NpgsqlParameter();
        p.NpgsqlDbType = NpgsqlDbType.Time;
        p.Value = DateTime.Now;
        var o = p.Value;
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/750")]
    public void NpgsqlInet()
    {
        var v = new NpgsqlInet(IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"), 32);
        Assert.That(v.ToString(), Is.EqualTo("2001:1db8:85a3:1142:1000:8a2e:1370:7334/32"));
    }

    [Test]
    public void NpgsqlInet_parse_ipv4()
    {
        var ipv4 = new NpgsqlInet("192.168.1.1/8");
        Assert.That(ipv4.Address, Is.EqualTo(IPAddress.Parse("192.168.1.1")));
        Assert.That(ipv4.Netmask, Is.EqualTo(8));

        ipv4 = new NpgsqlInet("192.168.1.1/32");
        Assert.That(ipv4.Address, Is.EqualTo(IPAddress.Parse("192.168.1.1")));
        Assert.That(ipv4.Netmask, Is.EqualTo(32));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/5638")]
    public void NpgsqlInet_parse_ipv6()
    {
        var ipv6 = new NpgsqlInet("2001:0000:130F:0000:0000:09C0:876A:130B/32");
        Assert.That(ipv6.Address, Is.EqualTo(IPAddress.Parse("2001:0000:130F:0000:0000:09C0:876A:130B")));
        Assert.That(ipv6.Netmask, Is.EqualTo(32));

        ipv6 = new NpgsqlInet("2001:0000:130F:0000:0000:09C0:876A:130B");
        Assert.That(ipv6.Address, Is.EqualTo(IPAddress.Parse("2001:0000:130F:0000:0000:09C0:876A:130B")));
        Assert.That(ipv6.Netmask, Is.EqualTo(128));
    }

    [Test]
    public void NpgsqlInet_ToString_ipv4()
    {
        Assert.That(new NpgsqlInet("192.168.1.1/8").ToString(), Is.EqualTo("192.168.1.1/8"));
        Assert.That(new NpgsqlInet("192.168.1.1/32").ToString(), Is.EqualTo("192.168.1.1"));
    }

    [Test]
    public void NpgsqlInet_ToString_ipv6()
    {
        Assert.That(new NpgsqlInet("2001:0:130f::9c0:876a:130b/32").ToString(), Is.EqualTo("2001:0:130f::9c0:876a:130b/32"));
        Assert.That(new NpgsqlInet("2001:0:130f::9c0:876a:130b/128").ToString(), Is.EqualTo("2001:0:130f::9c0:876a:130b"));
    }
}
