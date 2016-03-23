using System;
using System.Data.Entity;
using NpgsqlTypes;

namespace Npgsql.Linq
{
    /// <summary>
    /// Use this class in LINQ queries to generate
    /// full-text search expressions using tsvector 
    /// and tsquery.
    /// </summary>
    public static class PgSqlTextFunctions
    {
        /// <summary>
        /// This method generates the "@@" match operator.
        /// See http://www.postgresql.org/docs/9.1/static/textsearch-intro.html#TEXTSEARCH-MATCHING
        /// </summary>
        /// <remarks>
        /// Generated SQL: &lt;<paramref name="tsvector" />&gt; @@ &lt;<paramref name="tsquery" />&gt;
        /// 
        /// &lt;<paramref name="tsvector" />&gt; and &lt;<paramref name="tsquery" />&gt; are replaced 
        /// with their respective resolved expressions.
        /// </remarks>
        [DbFunction("Npgsql", "Match"), DbFunctionStoreName("OperatorMatch")]
        public static bool Match(string tsvector, string tsquery)
        {
            throw new NotSupportedException();
        }
        
        [DbFunction("Npgsql", "ToTsVector"), DbFunctionStoreName("to_tsvector")]
        public static string ToTsVector(string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "ToTsVector"), DbFunctionStoreName("to_tsvector")]
        public static string ToTsVector(string config, string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "PlainToTsQuery"), DbFunctionStoreName("plainto_tsquery")]
        public static string PlainToTsQuery(string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "PlainToTsQuery"), DbFunctionStoreName("plainto_tsquery")]
        public static string PlainToTsQuery(string config, string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "ToTsQuery"), DbFunctionStoreName("to_tsquery")]
        public static string ToTsQuery(string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "ToTsQuery"), DbFunctionStoreName("to_tsquery")]
        public static string ToTsQuery(string config, string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "SetWeight"), DbFunctionStoreName("setweight")]
        public static string SetWeight(string tsvector, string weightAbcd)
        {
            throw new NotSupportedException();
        }
    }
}