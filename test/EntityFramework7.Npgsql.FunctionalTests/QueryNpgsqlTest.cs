using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Xunit;

#if DNXCORE50
using System.Threading;
#endif

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class QueryNpgsqlTest : QueryTestBase<NorthwindQueryNpgsqlFixture>
    {
        public QueryNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture) { }

        #region Regular Expressions

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatch()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^A")),
                entryCount: 4);
            Assert.Contains("WHERE \"c\".\"CompanyName\" ~ ('(?p)' || '^A')", Sql);
        }

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatchOptionsNone()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^A", RegexOptions.None)),
                entryCount: 4);
            Assert.Contains("WHERE \"c\".\"CompanyName\" ~ ('(?p)' || '^A')", Sql);
        }

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatchOptionsIgnoreCase()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^a", RegexOptions.IgnoreCase)),
                entryCount: 4);
            Assert.Contains("WHERE \"c\".\"CompanyName\" ~ ('(?ip)' || '^a')", Sql);
        }

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatchOptionsMultiline()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^A", RegexOptions.Multiline)),
                entryCount: 4);
            Assert.Contains("WHERE \"c\".\"CompanyName\" ~ ('(?n)' || '^A')", Sql);
        }

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatchOptionsSingleline()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^A", RegexOptions.Singleline)),
                entryCount: 4);
            Assert.Contains("WHERE \"c\".\"CompanyName\" ~ '^A'", Sql);
        }

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatchOptionsIgnorePatternWhitespace()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^ A", RegexOptions.IgnorePatternWhitespace)),
                entryCount: 4);
            Assert.Contains("WHERE \"c\".\"CompanyName\" ~ ('(?px)' || '^ A')", Sql);
        }

        [Fact(Skip = "Regex support disabled for now")]
        public void Regex_IsMatchOptionsUnsupported()
        {
            AssertQuery<Customer>(
                cs => cs.Where(c => Regex.IsMatch(c.CompanyName, "^A", RegexOptions.RightToLeft)),
                entryCount: 4);
            Assert.DoesNotContain("WHERE \"c\".\"CompanyName\" ~ ", Sql);
        }

        #endregion

        static string Sql => TestSqlLoggerFactory.Sql;
    }
}
