#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class SqlQueryParserTests
    {
        [Test]
        public void ParamSimple()
        {
            _params.AddWithValue(":p1", "foo");
            _params.AddWithValue(":p2", "bar");
            _parser.ParseRawQuery("SELECT :p1, :p2", true, _params, _queries);
            Assert.That(_queries.Single().InputParameters, Is.EqualTo(_params));
        }

        [Test]
        public void ParamNameWithDot()
        {
            _params.AddWithValue(":a.parameter", "foo");
            _parser.ParseRawQuery("INSERT INTO data (field_char5) VALUES (:a.parameter)", true, _params, _queries);
            Assert.That(_queries.Single().InputParameters.Single(), Is.SameAs(_params.Single()));
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
            _params.AddWithValue(":param", "foo");
            _parser.ParseRawQuery(sql, true, _params, _queries);
            Assert.That(_queries.Single().SQL, Is.EqualTo(sql));
            Assert.That(_queries.Single().InputParameters, Is.Empty);
        }

        [Test]
        [TestCase(@"SELECT 1<:param", TestName = "LessThan")]
        [TestCase(@"SELECT 1>:param", TestName = "GreaterThan")]
        [TestCase(@"SELECT 1<>:param", TestName = "NotEqual")]
        [TestCase("SELECT--comment\r:param", TestName="LineComment")]
        public void ParamGetsBound(string sql)
        {
            _params.AddWithValue(":param", "foo");
            _parser.ParseRawQuery(sql, true, _params, _queries);
            Assert.That(_queries.Single().InputParameters.Single(), Is.SameAs(_params.Single()));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1177")]
        public void ParamGetsBoundNonAscii()
        {
            _params.AddWithValue("漢字", "foo");
            _parser.ParseRawQuery("SELECT @漢字", true, _params, _queries);
            Assert.That(_queries.Single().InputParameters.Single(), Is.SameAs(_params.Single()));
        }

        [Test]
        [TestCase(@"SELECT e'ab\'c:param'", TestName = "Estring")]
        [TestCase(@"SELECT/*/* -- nested comment :int /*/* *//*/ **/*/*/*/1")]
        [TestCase(@"SELECT 1,
-- Comment, @param and also :param
2", TestName = "LineComment")]
        public void ParamDoesntGetBound(string sql)
        {
            _params.AddWithValue(":param", "foo");
            _parser.ParseRawQuery(sql, true, _params, _queries);
            Assert.That(_queries.Single().InputParameters, Is.Empty);
        }

        [Test]
        public void NonConformantStrings()
        {
            _parser.ParseRawQuery(@"SELECT 'abc\':str''a:str'", false, _params, _queries);
            Assert.That(_queries.Single().SQL, Is.EqualTo(@"SELECT 'abc\':str''a:str'"));
            Assert.That(_queries.Single().InputParameters, Is.Empty);
        }

        [Test]
        public void MultiqueryWithParams()
        {
            var p1 = new NpgsqlParameter("p1", DbType.String);
            _params.Add(p1);
            var p2 = new NpgsqlParameter("p2", DbType.String);
            _params.Add(p2);
            var p3 = new NpgsqlParameter("p3", DbType.String);
            _params.Add(p3);

            _parser.ParseRawQuery("SELECT @p3, @p1; SELECT @p2, @p3", true, _params, _queries);

            Assert.That(_queries, Has.Count.EqualTo(2));
            Assert.That(_queries[0].InputParameters[0], Is.SameAs(p3));
            Assert.That(_queries[0].InputParameters[1], Is.SameAs(p1));
            Assert.That(_queries[1].InputParameters[0], Is.SameAs(p2));
            Assert.That(_queries[1].InputParameters[1], Is.SameAs(p3));
        }

        [Test]
        public void NoOutputParameters()
        {
            var p = new NpgsqlParameter("p", DbType.String) { Direction = ParameterDirection.Output };
            _params.Add(p);
            Assert.That(() => _parser.ParseRawQuery("SELECT @p", true, _params, _queries), Throws.Exception);
        }

        [Test]
        public void MissingParamIsIgnored()
        {
            _parser.ParseRawQuery("SELECT @p; SELECT 1", true, _params, _queries);
            Assert.That(_queries[0].SQL, Is.EqualTo("SELECT @p"));
            Assert.That(_queries[1].SQL, Is.EqualTo("SELECT 1"));
            Assert.That(_queries[0].InputParameters, Is.Empty);
            Assert.That(_queries[1].InputParameters, Is.Empty);
        }

        [Test]
        public void ConsecutiveSemicolons()
        {
            _parser.ParseRawQuery(";;SELECT 1", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(1));
        }

        [Test]
        public void TrailingSemicolon()
        {
            _parser.ParseRawQuery("SELECT 1;", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(1));
        }

        [Test]
        public void Empty()
        {
            _parser.ParseRawQuery("", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(1));
        }

        [Test]
        public void SemicolonInParentheses()
        {
            _parser.ParseRawQuery("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1; SELECT 1)", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(1));
        }

        [Test]
        public void SemicolonAfterParentheses()
        {
            _parser.ParseRawQuery("CREATE OR REPLACE RULE test AS ON UPDATE TO test DO (SELECT 1); SELECT 1", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(2));
        }

        [Test]
        public void ReduceNumberOfStatements()
        {
            _parser.ParseRawQuery("SELECT 1; SELECT 2", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(2));
            _parser.ParseRawQuery("SELECT 1", true, _params, _queries);
            Assert.That(_queries, Has.Count.EqualTo(1));
        }

#if TODO
        [Test]
        public void TrimWhitespace()
        {
            _parser.ParseRawQuery("   SELECT 1\t", true, _params, _queries);
            Assert.That(_queries.Single().Sql, Is.EqualTo("SELECT 1"));
        }
#endif

        #region Setup / Teardown / Utils

        SqlQueryParser _parser;
        List<NpgsqlStatement> _queries;
        NpgsqlParameterCollection _params;

        [SetUp]
        public void SetUp()
        {
            _parser = new SqlQueryParser();
            _queries = new List<NpgsqlStatement>();
            _params = new NpgsqlParameterCollection();
        }

        #endregion
    }
}
