﻿using System;
using System.Collections;
using System.Threading.Tasks;
using Npgsql.Properties;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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

    [Test]
    public async Task Full_text_search_supported_only_with_EnableFullTextSearch([Values] bool enableFullTextSearch)
    {
        var errorMessage = string.Format(NpgsqlStrings.FullTextSearchNotEnabled, "EnableFullTextSearch", "NpgsqlSlimDataSourceBuilder");

        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        if (enableFullTextSearch)
            dataSourceBuilder.EnableFullTextSearch();
        await using var dataSource = dataSourceBuilder.Build();

        if (enableFullTextSearch)
        {
            await AssertType<NpgsqlTsQuery>(new NpgsqlTsQueryLexeme("a"), "'a'", "tsquery", NpgsqlDbType.TsQuery);
            await AssertType(NpgsqlTsVector.Parse("'1'"), "'1'", "tsvector", NpgsqlDbType.TsVector);
        }
        else
        {
            var exception = await AssertTypeUnsupportedRead<NpgsqlTsQuery, NotSupportedException>("a", "tsquery", dataSource);
            Assert.AreEqual(errorMessage, exception.Message);
            exception = await AssertTypeUnsupportedWrite<NpgsqlTsQuery, NotSupportedException>(new NpgsqlTsQueryLexeme("a"), pgTypeName: null, dataSource);
            Assert.AreEqual(errorMessage, exception.Message);

            exception = await AssertTypeUnsupportedRead<NpgsqlTsVector, NotSupportedException>("1", "tsvector", dataSource);
            Assert.AreEqual(errorMessage, exception.Message);
            exception = await AssertTypeUnsupportedWrite<NpgsqlTsVector, NotSupportedException>(NpgsqlTsVector.Parse("'1'"), pgTypeName: null, dataSource);
            Assert.AreEqual(errorMessage, exception.Message);
        }
    }
}
