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
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace Npgsql
{
    /// <summary>
    /// Provides the underlying mechanism for reading schema information.
    /// </summary>
    static class NpgsqlSchema
    {
        const string MetaDataResourceName = "Npgsql.NpgsqlMetaData.xml";

        /// <summary>
        /// Returns the MetaDataCollections that lists all possible collections.
        /// </summary>
        /// <returns>The MetaDataCollections</returns>
        internal static DataTable GetMetaDataCollections()
        {
            var ds = new DataSet { Locale = CultureInfo.InvariantCulture };
            LoadMetaDataXmlResource(ds);
            return ds.Tables["MetaDataCollections"].Copy();
        }

        /// <summary>
        /// Returns the Restrictions that contains the meaning and position of the values in the restrictions array.
        /// </summary>
        /// <returns>The Restrictions</returns>
        internal static DataTable GetRestrictions()
        {
            var ds = new DataSet { Locale = CultureInfo.InvariantCulture };
            LoadMetaDataXmlResource(ds);
            return ds.Tables["Restrictions"].Copy();
        }

        static NpgsqlCommand BuildCommand(NpgsqlConnection conn, StringBuilder query, [CanBeNull] string[] restrictions, [CanBeNull] params string[] names)
            => BuildCommand(conn, query, restrictions, true, names);

        static NpgsqlCommand BuildCommand(NpgsqlConnection conn, StringBuilder query, [CanBeNull] string[] restrictions, bool addWhere, [CanBeNull] params string[] names)
        {
            var command = new NpgsqlCommand();

            if (restrictions != null && names != null)
            {
                for (var i = 0; i < restrictions.Length && i < names.Length; ++i)
                {
                    if (restrictions[i] != null && restrictions[i].Length != 0)
                    {
                        if (addWhere)
                        {
                            query.Append(" WHERE ");
                            addWhere = false;
                        }
                        else
                        {
                            query.Append(" AND ");
                        }

                        string paramName = RemoveSpecialChars(names[i]);

                        query.AppendFormat("{0} = :{1}", names[i], paramName);

                        command.Parameters.Add(new NpgsqlParameter(paramName, restrictions[i]));
                    }
                }
            }
            command.CommandText = query.ToString();
            command.Connection = conn;

            return command;
        }

        static string RemoveSpecialChars(string paramName)
            => paramName.Replace("(", "").Replace(")", "").Replace(".", "");

        /// <summary>
        /// Returns the Databases that contains a list of all accessable databases.
        /// </summary>
        /// <param name="conn">The database connection on which to run the metadataquery.</param>
        /// <param name="restrictions">The restrictions to filter the collection.</param>
        /// <returns>The Databases</returns>
        internal static DataTable GetDatabases(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var databases = new DataTable("Databases") { Locale = CultureInfo.InvariantCulture };

            databases.Columns.AddRange(new [] {
                new DataColumn("database_name"),
                new DataColumn("owner"),
                new DataColumn("encoding")
            });

            var getDatabases = new StringBuilder();

            getDatabases.Append("SELECT d.datname AS database_name, u.usename AS owner, pg_catalog.pg_encoding_to_char(d.encoding) AS encoding FROM pg_catalog.pg_database d LEFT JOIN pg_catalog.pg_user u ON d.datdba = u.usesysid");

            using (var command = BuildCommand(conn, getDatabases, restrictions, "datname"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(databases);

            return databases;
        }

        internal static DataTable GetSchemata(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var schemata = new DataTable("Schemata") { Locale = CultureInfo.InvariantCulture };

            schemata.Columns.AddRange(new [] {
                new DataColumn("catalog_name"),
                new DataColumn("schema_name"),
                new DataColumn("schema_owner")
            });

            var getSchemata = new StringBuilder();

            getSchemata.Append(
@"select * from(
    select current_database() as catalog_name,
        nspname AS schema_name,
        r.rolname AS schema_owner
    from
        pg_catalog.pg_namespace left join pg_catalog.pg_roles r on r.oid = nspowner
    ) tmp");

            using (var command = BuildCommand(conn, getSchemata, restrictions, "catalog_name", "schema_name", "schema_owner"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(schemata);

            return schemata;
        }

        /// <summary>
        /// Returns the Tables that contains table and view names and the database and schema they come from.
        /// </summary>
        /// <param name="conn">The database connection on which to run the metadataquery.</param>
        /// <param name="restrictions">The restrictions to filter the collection.</param>
        /// <returns>The Tables</returns>
        internal static DataTable GetTables(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var tables = new DataTable("Tables") { Locale = CultureInfo.InvariantCulture };

            tables.Columns.AddRange(new[] {
                new DataColumn("table_catalog"),
                new DataColumn("table_schema"),
                new DataColumn("table_name"),
                new DataColumn("table_type")
            });

            var getTables = new StringBuilder();

            //getTables.Append("SELECT * FROM (SELECT table_catalog, table_schema, table_name, table_type FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')) tmp");
            getTables.Append(@"
SELECT table_catalog, table_schema, table_name, table_type
FROM information_schema.tables
WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')");

            using (var command = BuildCommand(conn, getTables, restrictions, false, "table_catalog", "table_schema", "table_name", "table_type"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(tables);

            return tables;
        }

        /// <summary>
        /// Returns the Columns that contains information about columns in tables.
        /// </summary>
        /// <param name="conn">The database connection on which to run the metadataquery.</param>
        /// <param name="restrictions">The restrictions to filter the collection.</param>
        /// <returns>The Columns.</returns>
        internal static DataTable GetColumns(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var columns = new DataTable("Columns") { Locale = CultureInfo.InvariantCulture };

            columns.Columns.AddRange(new [] {
                new DataColumn("table_catalog"), new DataColumn("table_schema"), new DataColumn("table_name"),
                new DataColumn("column_name"), new DataColumn("ordinal_position", typeof(int)), new DataColumn("column_default"),
                new DataColumn("is_nullable"), new DataColumn("data_type"),
                new DataColumn("character_maximum_length", typeof(int)), new DataColumn("character_octet_length", typeof(int)),
                new DataColumn("numeric_precision", typeof(int)), new DataColumn("numeric_precision_radix", typeof(int)),
                new DataColumn("numeric_scale", typeof(int)), new DataColumn("datetime_precision", typeof(int)),
                new DataColumn("character_set_catalog"), new DataColumn("character_set_schema"),
                new DataColumn("character_set_name"), new DataColumn("collation_catalog")
            });

            var getColumns = new StringBuilder();

            getColumns.Append(
                "SELECT table_catalog, table_schema, table_name, column_name, ordinal_position, column_default, is_nullable, udt_name AS data_type, character_maximum_length, character_octet_length, numeric_precision, numeric_precision_radix, numeric_scale, datetime_precision, character_set_catalog, character_set_schema, character_set_name, collation_catalog FROM information_schema.columns");

            using (var command = BuildCommand(conn, getColumns, restrictions, "table_catalog", "table_schema", "table_name", "column_name"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(columns);

            return columns;
        }

        /// <summary>
        /// Returns the Views that contains view names and the database and schema they come from.
        /// </summary>
        /// <param name="conn">The database connection on which to run the metadataquery.</param>
        /// <param name="restrictions">The restrictions to filter the collection.</param>
        /// <returns>The Views</returns>
        internal static DataTable GetViews(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var views = new DataTable("Views") { Locale = CultureInfo.InvariantCulture };

            views.Columns.AddRange(new[] {
                new DataColumn("table_catalog"), new DataColumn("table_schema"), new DataColumn("table_name"),
                new DataColumn("check_option"), new DataColumn("is_updatable")
            });

            var getViews = new StringBuilder();

            //getViews.Append("SELECT table_catalog, table_schema, table_name, check_option, is_updatable FROM information_schema.views WHERE table_schema NOT IN ('pg_catalog', 'information_schema')");
            getViews.Append(@"
SELECT table_catalog, table_schema, table_name, check_option, is_updatable
FROM information_schema.views
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')");

            using (var command = BuildCommand(conn, getViews, restrictions, false, "table_catalog", "table_schema", "table_name"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(views);

            return views;
        }

        /// <summary>
        /// Returns the Users containing user names and the sysid of those users.
        /// </summary>
        /// <param name="conn">The database connection on which to run the metadataquery.</param>
        /// <param name="restrictions">The restrictions to filter the collection.</param>
        /// <returns>The Users.</returns>
        internal static DataTable GetUsers(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var users = new DataTable("Users") { Locale = CultureInfo.InvariantCulture };

            users.Columns.AddRange(new[] {new DataColumn("user_name"), new DataColumn("user_sysid", typeof(int))});

            var getUsers = new StringBuilder();

            getUsers.Append("SELECT usename as user_name, usesysid as user_sysid FROM pg_catalog.pg_user");

            using (var command = BuildCommand(conn, getUsers, restrictions, "usename"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(users);

            return users;
        }

        internal static DataTable GetIndexes(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var indexes = new DataTable("Indexes") { Locale = CultureInfo.InvariantCulture };

            indexes.Columns.AddRange(new[] {
                new DataColumn("table_catalog"), new DataColumn("table_schema"), new DataColumn("table_name"),
                new DataColumn("index_name")
            });

            var getIndexes = new StringBuilder();

            getIndexes.Append(
@"select current_database() as table_catalog,
    n.nspname as table_schema,
    t.relname as table_name,
    i.relname as index_name
from
    pg_catalog.pg_class i join
    pg_catalog.pg_index ix ON ix.indexrelid = i.oid join
    pg_catalog.pg_class t ON ix.indrelid = t.oid join
    pg_attribute a on t.oid = a.attrelid left join
    pg_catalog.pg_user u ON u.usesysid = i.relowner left join
    pg_catalog.pg_namespace n ON n.oid = i.relnamespace
where
    i.relkind = 'i'
    and n.nspname not in ('pg_catalog', 'pg_toast')
    and pg_catalog.pg_table_is_visible(i.oid)
    and a.attnum = ANY(ix.indkey)
    and t.relkind = 'r'");

            using (var command = BuildCommand(conn, getIndexes, restrictions, false, "current_database()", "n.nspname", "t.relname", "i.relname"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(indexes);

            return indexes;
        }

        internal static DataTable GetIndexColumns(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var indexColumns = new DataTable("IndexColumns") { Locale = CultureInfo.InvariantCulture };

            indexColumns.Columns.AddRange(new[] {
                new DataColumn("table_catalog"), new DataColumn("table_schema"), new DataColumn("table_name"),
                new DataColumn("index_name"), new DataColumn("column_name")
            });

            var getIndexColumns = new StringBuilder();

            getIndexColumns.Append(
@"select current_database() as table_catalog,
    n.nspname as table_schema,
    t.relname as table_name,
    i.relname as index_name,
    a.attname as column_name
from
    pg_class t join
    pg_index ix on t.oid = ix.indrelid join
    pg_class i on ix.indexrelid = i.oid join
    pg_attribute a on t.oid = a.attrelid left join
    pg_namespace n on i.relnamespace = n.oid
where
    i.relkind = 'i'
    and n.nspname not in ('pg_catalog', 'pg_toast')
    and pg_catalog.pg_table_is_visible(i.oid)
    and a.attnum = ANY(ix.indkey)
    and t.relkind = 'r'");

            using (var command = BuildCommand(conn, getIndexColumns, restrictions, false, "current_database()", "n.nspname", "t.relname", "i.relname", "a.attname"))
            using (var adapter = new NpgsqlDataAdapter(command))
                adapter.Fill(indexColumns);

            return indexColumns;
        }

        internal static DataTable GetConstraints(NpgsqlConnection conn, [CanBeNull] string[] restrictions, [CanBeNull] string constraintType)
        {
            var getConstraints = new StringBuilder();

            getConstraints.Append(
@"select
  current_database() as ""CONSTRAINT_CATALOG"",
  pgn.nspname as ""CONSTRAINT_SCHEMA"",
  pgc.conname as ""CONSTRAINT_NAME"",
  current_database() as ""TABLE_CATALOG"",
  pgtn.nspname as ""TABLE_SCHEMA"",
  pgt.relname as ""TABLE_NAME"",
  ""CONSTRAINT_TYPE"",
  pgc.condeferrable as ""IS_DEFERRABLE"",
  pgc.condeferred as ""INITIALLY_DEFERRED""
from pg_catalog.pg_constraint pgc
inner join pg_catalog.pg_namespace pgn on pgc.connamespace = pgn.oid
inner join pg_catalog.pg_class pgt on pgc.conrelid = pgt.oid
inner join pg_catalog.pg_namespace pgtn on pgt.relnamespace = pgtn.oid
inner join (
select 'PRIMARY KEY' as ""CONSTRAINT_TYPE"", 'p' as ""contype"" union all
select 'FOREIGN KEY' as ""CONSTRAINT_TYPE"", 'f' as ""contype"" union all
select 'UNIQUE KEY' as ""CONSTRAINT_TYPE"", 'u' as ""contype""
) mapping_table on mapping_table.contype = pgc.contype");
            if ("ForeignKeys".Equals(constraintType))
                getConstraints.Append(" and pgc.contype='f'");
            else if ("PrimaryKey".Equals(constraintType))
                getConstraints.Append(" and pgc.contype='p'");
            else if ("UniqueKeys".Equals(constraintType))
                getConstraints.Append(" and pgc.contype='u'");
            else
                constraintType = "Constraints";

            using (var command = BuildCommand(conn, getConstraints, restrictions, false, "current_database()", "pgtn.nspname", "pgt.relname", "pgc.conname"))
            using (var adapter = new NpgsqlDataAdapter(command))
            {
                var table = new DataTable(constraintType) { Locale = CultureInfo.InvariantCulture };
                adapter.Fill(table);
                return table;
            }
        }

        internal static DataTable GetConstraintColumns(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
        {
            var getConstraintColumns = new StringBuilder();

            getConstraintColumns.Append(
@"select current_database() as constraint_catalog,
    n.nspname as constraint_schema,
    c.conname as constraint_name,
    current_database() as table_catalog,
    n.nspname as table_schema,
    t.relname as table_name,
    a.attname as column_name,
    a.attnum as ordinal_number,
    mapping_table.constraint_type
from pg_constraint c
inner join pg_namespace n on n.oid = c.connamespace
inner join pg_class t on t.oid = c.conrelid and t.relkind = 'r'
inner join pg_attribute a on t.oid = a.attrelid and a.attnum = ANY(c.conkey)
inner join (
select 'PRIMARY KEY' as constraint_type, 'p' as contype union all
select 'FOREIGN KEY' as constraint_type, 'f' as contype union all
select 'UNIQUE KEY' as constraint_type, 'u' as contype
) mapping_table on mapping_table.contype = c.contype
and n.nspname not in ('pg_catalog', 'pg_toast')");

            using (var command = BuildCommand(conn, getConstraintColumns, restrictions, false, "current_database()", "n.nspname", "t.relname", "c.conname", "a.attname"))
            using (var adapter = new NpgsqlDataAdapter(command))
            {
                var table = new DataTable("ConstraintColumns") { Locale = CultureInfo.InvariantCulture };
                adapter.Fill(table);
                return table;
            }
        }

        internal static DataTable GetDataSourceInformation()
        {
            var ds = new DataSet { Locale = CultureInfo.InvariantCulture };
            LoadMetaDataXmlResource(ds);
            return ds.Tables["DataSourceInformation"].Copy();
        }

        public static DataTable GetReservedWords()
        {
            var table = new DataTable("ReservedWords") { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("ReservedWord", typeof(string));
            foreach (var keyword in ReservedKeywords)
                table.Rows.Add(keyword);
            return table;
        }

        static void LoadMetaDataXmlResource(DataSet dataSet)
        {
            var xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(MetaDataResourceName);
            if (xmlStream == null)
                throw new Exception($"Couldn't load {MetaDataResourceName} resource from assembly, Npgsql was compiled badly");

            using (xmlStream)
                dataSet.ReadXml(xmlStream);
        }

        #region Reserved Keywords

        /// <summary>
        /// List of keywords taken from PostgreSQL 9.0 reserved words documentation.
        /// </summary>
        static readonly string[] ReservedKeywords =
        {
            "ALL",
            "ANALYSE",
            "ANALYZE",
            "AND",
            "ANY",
            "ARRAY",
            "AS",
            "ASC",
            "ASYMMETRIC",
            "AUTHORIZATION",
            "BINARY",
            "BOTH",
            "CASE",
            "CAST",
            "CHECK",
            "COLLATE",
            "COLUMN",
            "CONCURRENTLY",
            "CONSTRAINT",
            "CREATE",
            "CROSS",
            "CURRENT_CATALOG",
            "CURRENT_DATE",
            "CURRENT_ROLE",
            "CURRENT_SCHEMA",
            "CURRENT_TIME",
            "CURRENT_TIMESTAMP",
            "CURRENT_USER",
            "DEFAULT",
            "DEFERRABLE",
            "DESC",
            "DISTINCT",
            "DO",
            "ELSE",
            "END",
            "EXCEPT",
            "FALSE",
            "FETCH",
            "FOR",
            "FOREIGN",
            "FREEZE",
            "FROM",
            "FULL",
            "GRANT",
            "GROUP",
            "HAVING",
            "ILIKE",
            "IN",
            "INITIALLY",
            "INNER",
            "INTERSECT",
            "INTO",
            "IS",
            "ISNULL",
            "JOIN",
            "LATERAL",
            "LEADING",
            "LEFT",
            "LIKE",
            "LIMIT",
            "LOCALTIME",
            "LOCALTIMESTAMP",
            "NATURAL",
            "NOT",
            "NOTNULL",
            "NULL",
            "OFFSET",
            "ON",
            "ONLY",
            "OR",
            "ORDER",
            "OUTER",
            "OVER",
            "OVERLAPS",
            "PLACING",
            "PRIMARY",
            "REFERENCES",
            "RETURNING",
            "RIGHT",
            "SELECT",
            "SESSION_USER",
            "SIMILAR",
            "SOME",
            "SYMMETRIC",
            "TABLE",
            "THEN",
            "TO",
            "TRAILING",
            "TRUE",
            "UNION",
            "UNIQUE",
            "USER",
            "USING",
            "VARIADIC",
            "VERBOSE",
            "WHEN",
            "WHERE",
            "WINDOW",
            "WITH"
        };

        #endregion Reserved Keywords
    }
}
