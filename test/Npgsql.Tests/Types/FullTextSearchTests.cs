using System;
using System.Collections;
using System.Threading.Tasks;
using Npgsql.Properties;
using NpgsqlTypes;
using NUnit.Framework;

#pragma warning disable CS0618 // NpgsqlTsVector.Parse is obsolete

namespace Npgsql.Tests.Types;

public class FullTextSearchTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public Task TsVector()
        => AssertType(
            NpgsqlTsVector.Parse("'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'"),
            "'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'",
            "tsvector",
            NpgsqlDbType.TsVector);

    public static IEnumerable TsQueryTestCases() => new[]
    {
        [
            "'a'",
            new NpgsqlTsQueryLexeme("a")
        ],
        [
            "!'a'",
            new NpgsqlTsQueryNot(
                new NpgsqlTsQueryLexeme("a"))
        ],
        [
            "'a' | 'b'",
            new NpgsqlTsQueryOr(
                new NpgsqlTsQueryLexeme("a"),
                new NpgsqlTsQueryLexeme("b"))
        ],
        [
            "'a' & 'b'",
            new NpgsqlTsQueryAnd(
                new NpgsqlTsQueryLexeme("a"),
                new NpgsqlTsQueryLexeme("b"))
        ],
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

    [Test]
    public async Task Full_text_search_not_supported_by_default_on_NpgsqlSlimSourceBuilder()
    {
        var errorMessage = string.Format(
            NpgsqlStrings.FullTextSearchNotEnabled,
            nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch),
            nameof(NpgsqlSlimDataSourceBuilder));

        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        await using var dataSource = dataSourceBuilder.Build();

        var exception = await AssertTypeUnsupportedRead<NpgsqlTsQuery, InvalidCastException>("a", "tsquery", dataSource);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.AreEqual(errorMessage, exception.InnerException!.Message);

        exception = await AssertTypeUnsupportedWrite<NpgsqlTsQuery, InvalidCastException>(new NpgsqlTsQueryLexeme("a"), pgTypeName: null, dataSource);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.AreEqual(errorMessage, exception.InnerException!.Message);

        exception = await AssertTypeUnsupportedRead<NpgsqlTsVector, InvalidCastException>("1", "tsvector", dataSource);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.AreEqual(errorMessage, exception.InnerException!.Message);

        exception = await AssertTypeUnsupportedWrite<NpgsqlTsVector, InvalidCastException>(NpgsqlTsVector.Parse("'1'"), pgTypeName: null, dataSource);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.AreEqual(errorMessage, exception.InnerException!.Message);
    }

    [Test]
    public async Task NpgsqlSlimSourceBuilder_EnableFullTextSearch()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableFullTextSearch();
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType<NpgsqlTsQuery>(new NpgsqlTsQueryLexeme("a"), "'a'", "tsquery", NpgsqlDbType.TsQuery);
        await AssertType(NpgsqlTsVector.Parse("'1'"), "'1'", "tsvector", NpgsqlDbType.TsVector);
    }
}
