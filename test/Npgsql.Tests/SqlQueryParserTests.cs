using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;

namespace Npgsql.Tests;

class SqlQueryParserTests
{
    [Test]
    public void Parameter_simple()
    {
        var parameters = new NpgsqlParameter[] { new(":p1", "foo"), new(":p2", "foo") };
        var result = ParseCommand("SELECT :p1, :p2", parameters).Single();
        Assert.That(result.FinalCommandText, Is.EqualTo("SELECT $1, $2"));
        Assert.That(result.PositionalParameters, Is.EquivalentTo(parameters));
    }

    [Test]
    public void Parameter_name_with_dot()
    {
        var p = new NpgsqlParameter(":a.parameter", "foo");
        var results = ParseCommand("INSERT INTO data (field_char5) VALUES (:a.parameter)", p);
        Assert.That(results.Single().PositionalParameters.Single(), Is.SameAs(p));
    }

    [Test, Description("Checks several scenarios in which the SQL is supposed to pass untouched")]
    [TestCase(@"SELECT to_tsvector('fat cats ate rats') @@ to_tsquery('cat & rat')", TestName="AtAt")]
    [TestCase(@"SELECT 'cat'::tsquery @> 'cat & rat'::tsquery", TestName = "AtGt")]
    [TestCase(@"SELECT 'cat'::tsquery <@ 'cat & rat'::tsquery", TestName = "AtLt")]
    [TestCase(@"SELECT 'b''la'", TestName = "DoubleTicks")]
    [TestCase(@"SELECT 'type(''m.response'')#''O''%'", TestName = "DoubleTicks2")]
    [TestCase(@"SELECT 'abc'':str''a:str'", TestName = "DoubleTicks3")]
    [TestCase(@"SELECT 1 FROM "":str""", TestName = "DoubleQuoted")]
    [TestCase(@"SELECT 1 FROM 'yo'::str", TestName = "DoubleColons")]
    [TestCase("SELECT $\u00ffabc0$literal string :str :int$\u00ffabc0 $\u00ffabc0$", TestName = "DollarQuotes")]
    [TestCase("SELECT $$:str$$", TestName = "DollarQuotesNoTag")]
    public void Untouched(string sql)
    {
        var results = ParseCommand(sql, new NpgsqlParameter(":param", "foo"));
        Assert.That(results.Single().FinalCommandText, Is.EqualTo(sql));
        Assert.That(results.Single().PositionalParameters, Is.Empty);
    }

    [Test]
    [TestCase(@"SELECT 1<:param", TestName = "LessThan")]
    [TestCase(@"SELECT 1>:param", TestName = "GreaterThan")]
    [TestCase(@"SELECT 1<>:param", TestName = "NotEqual")]
    [TestCase("SELECT--comment\r:param", TestName="LineComment")]
    public void Parameter_gets_bound(string sql)
    {
        var p = new NpgsqlParameter(":param", "foo");
        var results = ParseCommand(sql, p);
        Assert.That(results.Single().PositionalParameters.Single(), Is.SameAs(p));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1177")]
    public void Parameter_gets_bound_non_ascii()
    {
        var p = new NpgsqlParameter("漢字", "foo");
        var results = ParseCommand("SELECT @漢字", p);
        Assert.That(results.Single().PositionalParameters.Single(), Is.SameAs(p));
    }

    [Test]
    [TestCase(@"SELECT e'ab\'c:param'", TestName = "Estring")]
    [TestCase(@"SELECT/*/* -- nested comment :int /*/* *//*/ **/*/*/*/1")]
    [TestCase(@"SELECT 1,
-- Comment, @param and also :param
2", TestName = "LineComment")]
    public void Parameter_does_not_get_bound(string sql)
    {
        var p = new NpgsqlParameter(":param", "foo");
        var results = ParseCommand(sql, p);
        Assert.That(results.Single().PositionalParameters, Is.Empty);
    }

    [Test]
    public void Non_conforming_string()
    {
        var result = ParseCommand(@"SELECT 'abc\':str''a:str'").Single();
        Assert.That(result.FinalCommandText, Is.EqualTo(@"SELECT 'abc\':str''a:str'"));
        Assert.That(result.PositionalParameters, Is.Empty);
    }

    [Test]
    public void Multiquery_with_parameters()
    {
        var parameters = new NpgsqlParameter[]
        {
            new("p1", DbType.String),
            new("p2", DbType.String),
            new("p3", DbType.String),
        };

        var results = ParseCommand("SELECT @p3, @p1; SELECT @p2, @p3", parameters);

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].FinalCommandText, Is.EqualTo("SELECT $1, $2"));
        Assert.That(results[0].PositionalParameters[0], Is.SameAs(parameters[2]));
        Assert.That(results[0].PositionalParameters[1], Is.SameAs(parameters[0]));
        Assert.That(results[1].FinalCommandText, Is.EqualTo("SELECT $1, $2"));
        Assert.That(results[1].PositionalParameters[0], Is.SameAs(parameters[1]));
        Assert.That(results[1].PositionalParameters[1], Is.SameAs(parameters[2]));
    }

    [Test]
    public void No_output_parameters()
    {
        var p = new NpgsqlParameter("p", DbType.String) { Direction = ParameterDirection.Output };
        Assert.That(() => ParseCommand("SELECT @p", p), Throws.Exception);
    }

    [Test]
    public void Missing_parameter_is_ignored()
    {
        var results = ParseCommand("SELECT @p; SELECT 1");
        Assert.That(results[0].FinalCommandText, Is.EqualTo("SELECT @p"));
        Assert.That(results[1].FinalCommandText, Is.EqualTo("SELECT 1"));
        Assert.That(results[0].PositionalParameters, Is.Empty);
        Assert.That(results[1].PositionalParameters, Is.Empty);
    }

    [Test]
    public void Consecutive_semicolons()
    {
        var results = ParseCommand(";;SELECT 1");
        Assert.That(results, Has.Count.EqualTo(3));
        Assert.That(results[0].FinalCommandText, Is.Empty);
        Assert.That(results[1].FinalCommandText, Is.Empty);
        Assert.That(results[2].FinalCommandText, Is.EqualTo("SELECT 1"));
    }

    [Test]
    public void Trailing_semicolon()
    {
        var results = ParseCommand("SELECT 1;");
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FinalCommandText, Is.EqualTo("SELECT 1"));
    }

    [Test]
    public void Empty()
    {
        var results = ParseCommand("");
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FinalCommandText, Is.Empty);
    }

    [Test]
    public void Semicolon_in_parentheses()
    {
        var results = ParseCommand("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1; SELECT 1)");
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].FinalCommandText, Is.EqualTo("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1; SELECT 1)"));
    }

    [Test]
    public void Semicolon_after_parentheses()
    {
        var results = ParseCommand("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1); SELECT 1");
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].FinalCommandText, Is.EqualTo("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1)"));
        Assert.That(results[1].FinalCommandText, Is.EqualTo("SELECT 1"));
    }

    [Test]
    public void Reduce_number_of_statements()
    {
        var parser = new SqlQueryParser();

        var cmd = new NpgsqlCommand("SELECT 1; SELECT 2");
        parser.ParseRawQuery(cmd);
        Assert.That(cmd.InternalBatchCommands, Has.Count.EqualTo(2));

        cmd.CommandText = "SELECT 1";
        parser.ParseRawQuery(cmd);
        Assert.That(cmd.InternalBatchCommands, Has.Count.EqualTo(1));
    }

#if TODO
    [Test]
    public void Trim_whitespace()
    {
        _parser.ParseRawQuery("   SELECT 1\t", _params, _queries, standardConformingStrings: true);
        Assert.That(_queries.Single().Sql, Is.EqualTo("SELECT 1"));
    }
#endif

    #region Setup / Teardown / Utils

    List<NpgsqlBatchCommand> ParseCommand(string sql, params NpgsqlParameter[] parameters)
        => ParseCommand(sql, parameters, standardConformingStrings: true);

    List<NpgsqlBatchCommand> ParseCommand(string sql, NpgsqlParameter[] parameters, bool standardConformingStrings)
    {
        var cmd = new NpgsqlCommand(sql);
        cmd.Parameters.AddRange(parameters);
        var parser = new SqlQueryParser();
        parser.ParseRawQuery(cmd, standardConformingStrings);
        return cmd.InternalBatchCommands;
    }

    #endregion
}
