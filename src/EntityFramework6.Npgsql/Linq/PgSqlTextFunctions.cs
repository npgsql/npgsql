using System;
using System.Data.Entity;

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
        [DbFunction("Npgsql", "Match")]
        public static bool Match(string tsvector, string tsquery)
        {
            throw new NotSupportedException();
        }
        
        [DbFunction("Npgsql", "ToTsVector")]
        public static string ToTsVector(string text)
        {
            throw new NotSupportedException();
        }

        [DbFunction("Npgsql", "PlainToTsQuery")]
        public static string PlainToTsQuery(string text)
        {
            throw new NotSupportedException();
        }
    }
}