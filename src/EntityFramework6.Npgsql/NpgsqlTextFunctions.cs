using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql
{
    /// <summary>
    /// Use this class in LINQ queries to generate full-text search expressions using tsvector and tsquery. 
    /// None of these functions can be called directly.
    /// See http://www.postgresql.org/docs/current/static/functions-textsearch.html for the latest documentation.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public static class NpgsqlTextFunctions
    {
        /// <summary>
        /// Cast <paramref name="vector" /> to the tsvector data type.
        /// </summary>
        [DbFunction("Npgsql", "as_tsvector")]
        public static string AsTsVector(string vector)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reduce <paramref name="document" /> to tsvector.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-PARSING-DOCUMENTS
        /// </summary>
        [DbFunction("Npgsql", "to_tsvector")]
        public static string ToTsVector(string document)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reduce <paramref name="document" /> to tsvector using the text search configuration specified
        /// by <paramref name="config" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-PARSING-DOCUMENTS
        /// </summary>
        [DbFunction("Npgsql", "to_tsvector")]
        public static string ToTsVector(string config, string document)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Cast <paramref name="query" /> to the tsquery data type.
        /// </summary>
        [DbFunction("Npgsql", "as_tsquery")]
        public static string AsTsQuery(string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Produce tsquery from <paramref name="query" /> ignoring punctuation.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-PARSING-QUERIES
        /// </summary>
        [DbFunction("Npgsql", "plainto_tsquery")]
        public static string PlainToTsQuery(string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Produce tsquery from <paramref name="query" /> ignoring punctuation and using the text search
        /// configuration specified by <paramref name="config" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-PARSING-QUERIES
        /// </summary>
        [DbFunction("Npgsql", "plainto_tsquery")]
        public static string PlainToTsQuery(string config, string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Normalize words in <paramref name="query" /> and convert to tsquery. If your input
        /// contains punctuation that should not be treated as text search operators, use 
        /// <see cref="PlainToTsQuery(string)" /> instead.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-PARSING-QUERIES
        /// </summary>
        [DbFunction("Npgsql", "to_tsquery")]
        public static string ToTsQuery(string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Normalize words in <paramref name="query" /> and convert to tsquery using the text search
        /// configuration specified by <paramref name="config" />. If your input contains punctuation 
        /// that should not be treated as text search operators, use <see cref="PlainToTsQuery(string, string)" /> 
        /// instead.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-PARSING-QUERIES
        /// </summary>
        [DbFunction("Npgsql", "to_tsquery")]
        public static string ToTsQuery(string config, string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// AND tsquerys together. Generates the "&amp;&amp;" operator.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
        /// </summary>
        [DbFunction("Npgsql", "operator_tsquery_and")]
        public static string QueryAnd(string tsquery1, string tsquery2)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// OR tsquerys together. Generates the "||" operator.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
        /// </summary>
        [DbFunction("Npgsql", "operator_tsquery_or")]
        public static string QueryOr(string tsquery1, string tsquery2)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Negate a tsquery. Generates the "!!" operator.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
        /// </summary>
        [DbFunction("Npgsql", "operator_tsquery_negate")]
        public static string QueryNot(string tsquery)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns whether <paramref name="tsquery1" /> contains <paramref name="tsquery2" />.
        /// Generates the "@&gt;" operator.
        /// http://www.postgresql.org/docs/current/static/functions-textsearch.html
        /// </summary>
        [DbFunction("Npgsql", "operator_tsquery_contains")]
        public static bool QueryContains(string tsquery1, string tsquery2)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns whether <paramref name="tsquery1" /> is contained within <paramref name="tsquery2" />.
        /// Generates the "&lt;@" operator.
        /// http://www.postgresql.org/docs/current/static/functions-textsearch.html
        /// </summary>
        [DbFunction("Npgsql", "operator_tsquery_is_contained")]
        public static bool QueryIsContained(string tsquery1, string tsquery2)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method generates the "@@" match operator.
        /// http://www.postgresql.org/docs/current/static/textsearch-intro.html#TEXTSEARCH-MATCHING
        /// </summary>
        [DbFunction("Npgsql", "@@")]
        public static bool Match(string tsvector, string tsquery)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Assign weight to each element of <paramref name="tsvector" /> and return a new
        /// weighted tsvector.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
        /// </summary>
        [DbFunction("Npgsql", "setweight")]
        public static string SetWeight(string tsvector, NpgsqlWeightLabel label)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the number of lexemes in <paramref name="tsvector" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
        /// </summary>
        [DbFunction("Npgsql", "length")]
        public static int Length(string tsvector)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the number of lexemes plus operators in <paramref name="tsquery" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
        /// </summary>
        [DbFunction("Npgsql", "numnode")]
        public static int NumNode(string tsquery)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes weights and positions from <paramref name="tsvector" /> and returns
        /// a new stripped tsvector.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
        /// </summary>
        [DbFunction("Npgsql", "strip")]
        public static string Strip(string tsvector)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get indexable part of <paramref name="query" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
        /// </summary>
        [DbFunction("Npgsql", "querytree")]
        public static string QueryTree(string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a string suitable for display containing a query match.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-HEADLINE
        /// </summary>
        [DbFunction("Npgsql", "ts_headline")]
        public static string TsHeadline(string document, string tsquery, string options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a string suitable for display containing a query match using the text
        /// search configuration specified by <paramref name="config" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-HEADLINE
        /// </summary>
        [DbFunction("Npgsql", "ts_headline")]
        public static string TsHeadline(string config, string document, string tsquery, string options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank")]
        public static float TsRank(
            string vector,
            string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> while normalizing 
        /// the result according to the behaviors specified by <paramref name="normalization" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank")]
        public static float TsRank(
            string vector,
            string query,
            NpgsqlRankingNormalization normalization)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> with custom 
        /// weighting for word instances depending on their labels (D, C, B or A).
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank")]
        public static float TsRank(
            float weightD,
            float weightC,
            float weightB,
            float weightA,
            string vector,
            string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> while normalizing 
        /// the result according to the behaviors specified by <paramref name="normalization" /> 
        /// and using custom weighting for word instances depending on their labels (D, C, B or A).
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank")]
        public static float TsRank(
            float weightD,
            float weightC,
            float weightB,
            float weightA,
            string vector,
            string query,
            NpgsqlRankingNormalization normalization)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover 
        /// density method.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank_cd")]
        public static float TsRankCd(
            string vector,
            string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover
        /// density method while normalizing the result according to the behaviors specified by 
        /// <paramref name="normalization" />.
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank_cd")]
        public static float TsRankCd(
            string vector,
            string query,
            NpgsqlRankingNormalization normalization)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover 
        /// density method with custom weighting for word instances depending on their labels (D, C, B or A).
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank_cd")]
        public static float TsRankCd(
            float weightD,
            float weightC,
            float weightB,
            float weightA,
            string vector,
            string query)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover density
        /// method while normalizing the result according to the behaviors specified by <paramref name="normalization" /> 
        /// and using custom weighting for word instances depending on their labels (D, C, B or A).
        /// http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
        /// </summary>
        [DbFunction("Npgsql", "ts_rank_cd")]
        public static float TsRankCd(
            float weightD,
            float weightC,
            float weightB,
            float weightA,
            string vector,
            string query,
            NpgsqlRankingNormalization normalization)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Searchs <paramref name="query" /> for occurrences of <paramref name="target" />, and replaces 
        /// each occurrence with a <paramref name="substitute" />. All parameters are of type tsquery.
        /// http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
        /// </summary>
        [DbFunction("Npgsql", "ts_rewrite")]
        public static string TsRewrite(string query, string target, string substitute)
        {
            throw new NotSupportedException();
        }
    }   
}
