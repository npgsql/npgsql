using System.Collections;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    [TestFixture(MultiplexingMode.NonMultiplexing, false)]
    [TestFixture(MultiplexingMode.NonMultiplexing, true)]
    [TestFixture(MultiplexingMode.Multiplexing, false)]
    [TestFixture(MultiplexingMode.Multiplexing, true)]
    public sealed class TsQueryTests : TypeHandlerTestBase<NpgsqlTsQuery>
    {
        public TsQueryTests(MultiplexingMode multiplexingMode, bool useTypeName) : base(
            multiplexingMode,
            useTypeName ? null : NpgsqlDbType.TsQuery,
            useTypeName ? "tsquery" : null)
        { }

        public static IEnumerable TestCases() => new[]
        {
            new object[]
            {
                "$$'a'$$::tsquery",
                new NpgsqlTsQueryLexeme("a")
            },
            new object[]
            {
                "$$!'a'$$::tsquery",
                new NpgsqlTsQueryNot(
                    new NpgsqlTsQueryLexeme("a"))
            },
            new object[]
            {
                "$$'a' | 'b'$$::tsquery",
                new NpgsqlTsQueryOr(
                    new NpgsqlTsQueryLexeme("a"),
                    new NpgsqlTsQueryLexeme("b"))
            },
            new object[]
            {
                "$$'a' & 'b'$$::tsquery",
                new NpgsqlTsQueryAnd(
                    new NpgsqlTsQueryLexeme("a"),
                    new NpgsqlTsQueryLexeme("b"))
            },
            new object[]
            {
                "$$'a' <-> 'b'$$::tsquery",
                new NpgsqlTsQueryFollowedBy(
                    new NpgsqlTsQueryLexeme("a"), 1, new NpgsqlTsQueryLexeme("b"))
            },
            new object[]
            {
                "$$('a' & !('c' | 'd')) & (!!'a' & 'b') | 'ä' | 'x' <-> 'y' | 'x' <10> 'y' | 'd' <0> 'e' | 'f'$$::tsquery",
                new NpgsqlTsQueryOr(
                    new NpgsqlTsQueryOr(
                        new NpgsqlTsQueryOr(
                            new NpgsqlTsQueryOr(
                                new NpgsqlTsQueryOr(
                                    new NpgsqlTsQueryAnd(
                                        new NpgsqlTsQueryAnd(
                                            new NpgsqlTsQueryLexeme("a"),
                                            new NpgsqlTsQueryNot(
                                                new NpgsqlTsQueryOr(
                                                    new NpgsqlTsQueryLexeme("c"),
                                                    new NpgsqlTsQueryLexeme("d")))),
                                        new NpgsqlTsQueryAnd(
                                            new NpgsqlTsQueryNot(
                                                new NpgsqlTsQueryNot(
                                                    new NpgsqlTsQueryLexeme("a"))),
                                            new NpgsqlTsQueryLexeme("b"))),
                                    new NpgsqlTsQueryLexeme("ä")),
                                new NpgsqlTsQueryFollowedBy(
                                    new NpgsqlTsQueryLexeme("x"), 1, new NpgsqlTsQueryLexeme("y"))),
                            new NpgsqlTsQueryFollowedBy(
                                new NpgsqlTsQueryLexeme("x"), 10, new NpgsqlTsQueryLexeme("y"))),
                        new NpgsqlTsQueryFollowedBy(
                            new NpgsqlTsQueryLexeme("d"), 0, new NpgsqlTsQueryLexeme("e"))),
                    new NpgsqlTsQueryLexeme("f"))
            }
        };

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using var conn = await OpenConnectionAsync();
            await TestUtil.EnsureExtensionAsync(conn, "ltree");
        }
    }
}
