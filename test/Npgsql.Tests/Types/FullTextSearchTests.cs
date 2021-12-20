using System.Collections;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public class FullTextSearchTests : MultiplexingTestBase
{
    public FullTextSearchTests(MultiplexingMode multiplexingMode)
        : base(multiplexingMode) { }

    [Test]
    public Task TsVector()
        => AssertType(
            NpgsqlTsVector.Parse("'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'"),
            "'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'",
            "tsvector",
            NpgsqlDbType.TsVector);

    public static IEnumerable TsQueryTestCases() => new[]
    {
        new object[]
        {
            "'a'",
            new NpgsqlTsQueryLexeme("a")
        },
        new object[]
        {
            "!'a'",
            new NpgsqlTsQueryNot(
                new NpgsqlTsQueryLexeme("a"))
        },
        new object[]
        {
            "'a' | 'b'",
            new NpgsqlTsQueryOr(
                new NpgsqlTsQueryLexeme("a"),
                new NpgsqlTsQueryLexeme("b"))
        },
        new object[]
        {
            "'a' & 'b'",
            new NpgsqlTsQueryAnd(
                new NpgsqlTsQueryLexeme("a"),
                new NpgsqlTsQueryLexeme("b"))
        },
        new object[]
        {
            "'a' <-> 'b'",
            new NpgsqlTsQueryFollowedBy(
                new NpgsqlTsQueryLexeme("a"), 1, new NpgsqlTsQueryLexeme("b"))
        }
    };

    [Test]
    [TestCaseSource(nameof(TsQueryTestCases))]
    public Task TsQuery(string sqlLiteral, NpgsqlTsQuery query)
        => AssertType(query, sqlLiteral, "tsquery", NpgsqlDbType.TsQuery);
}