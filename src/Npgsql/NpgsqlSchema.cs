using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Provides the underlying mechanism for reading schema information.
    /// </summary>
    static class NpgsqlSchema
    {
        public static DataTable GetSchema(NpgsqlConnection conn, [CanBeNull] string collectionName, [CanBeNull] string[] restrictions)
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentException("Collection name cannot be null or empty", nameof(collectionName));

            switch (collectionName.ToUpperInvariant())
            {
            case "METADATACOLLECTIONS":
                return GetMetaDataCollections();
            case "RESTRICTIONS":
                return GetRestrictions();
            case "DATASOURCEINFORMATION":
                return GetDataSourceInformation(conn);
            case "DATATYPES":
                return GetDataTypes(conn);
            case "RESERVEDWORDS":
                return GetReservedWords();
            // custom collections for npgsql
            case "DATABASES":
                return GetDatabases(conn, restrictions);
            case "SCHEMATA":
                return GetSchemata(conn, restrictions);
            case "TABLES":
                return GetTables(conn, restrictions);
            case "COLUMNS":
                return GetColumns(conn, restrictions);
            case "VIEWS":
                return GetViews(conn, restrictions);
            case "USERS":
                return GetUsers(conn, restrictions);
            case "INDEXES":
                return GetIndexes(conn, restrictions);
            case "INDEXCOLUMNS":
                return GetIndexColumns(conn, restrictions);
            case "CONSTRAINTS":
            case "PRIMARYKEY":
            case "UNIQUEKEYS":
            case "FOREIGNKEYS":
                return GetConstraints(conn, restrictions, collectionName);
            case "CONSTRAINTCOLUMNS":
                return GetConstraintColumns(conn, restrictions);
            default:
                throw new ArgumentOutOfRangeException(nameof(collectionName), collectionName, "Invalid collection name");
            }
        }

        /// <summary>
        /// Returns the MetaDataCollections that lists all possible collections.
        /// </summary>
        /// <returns>The MetaDataCollections</returns>
        static DataTable GetMetaDataCollections()
        {
            var table = new DataTable("MetaDataCollections");
            table.Columns.Add("CollectionName", typeof(string));
            table.Columns.Add("NumberOfRestrictions", typeof(int));
            table.Columns.Add("NumberOfIdentifierParts", typeof(int));

            table.Rows.Add("MetaDataCollections", 0, 0);
            table.Rows.Add("DataSourceInformation", 0, 0);
            table.Rows.Add("Restrictions", 0, 0);
            table.Rows.Add("DataTypes", 0, 0);  // TODO: Support type name restriction
            table.Rows.Add("Databases", 1, 1);
            table.Rows.Add("Tables", 4, 3);
            table.Rows.Add("Columns", 4, 4);
            table.Rows.Add("Views", 3, 3);
            table.Rows.Add("Users", 1, 1);
            table.Rows.Add("Indexes", 4, 4);
            table.Rows.Add("IndexColumns", 5, 5);

            return table;
        }

        /// <summary>
        /// Returns the Restrictions that contains the meaning and position of the values in the restrictions array.
        /// </summary>
        /// <returns>The Restrictions</returns>
        static DataTable GetRestrictions()
        {
            var table = new DataTable("Restrictions");

            table.Columns.Add("CollectionName", typeof(string));
            table.Columns.Add("RestrictionName", typeof(string));
            table.Columns.Add("RestrictionDefault", typeof(string));
            table.Columns.Add("RestrictionNumber", typeof(int));

            table.Rows.Add("Database", "Name", "Name", 1);
            table.Rows.Add("Tables", "Catalog", "table_catalog", 1);
            table.Rows.Add("Tables", "Schema", "schema_catalog", 2);
            table.Rows.Add("Tables", "Table", "table_name", 3);
            table.Rows.Add("Tables", "TableType", "table_type", 4);
            table.Rows.Add("Columns", "Catalog", "table_catalog", 1);
            table.Rows.Add("Columns", "Schema", "table_schema", 2);
            table.Rows.Add("Columns", "TableName", "table_name", 3);
            table.Rows.Add("Columns", "Column", "column_name", 4);
            table.Rows.Add("Views", "Catalog", "table_catalog", 1);
            table.Rows.Add("Views", "Schema", "table_schema", 2);
            table.Rows.Add("Views", "Table", "table_name", 3);
            table.Rows.Add("Users", "User", "user_name", 1);
            table.Rows.Add("Indexes", "Catalog", "table_catalog", 1);
            table.Rows.Add("Indexes", "Schema", "table_schema", 2);
            table.Rows.Add("Indexes", "Table", "table_name", 3);
            table.Rows.Add("Indexes", "Index", "index_name", 4);
            table.Rows.Add("IndexColumns", "Catalog", "table_catalog", 1);
            table.Rows.Add("IndexColumns", "Schema", "table_schema", 2);
            table.Rows.Add("IndexColumns", "Table", "table_name", 3);
            table.Rows.Add("IndexColumns", "Index", "index_name", 4);
            table.Rows.Add("IndexColumns", "Column", "column_name", 5);

            return table;
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
        static DataTable GetDatabases(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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

        static DataTable GetSchemata(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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
        static DataTable GetTables(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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
        static DataTable GetColumns(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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
        static DataTable GetViews(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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
        static DataTable GetUsers(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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

        static DataTable GetIndexes(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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

        static DataTable GetIndexColumns(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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

        static DataTable GetConstraints(NpgsqlConnection conn, [CanBeNull] string[] restrictions, [CanBeNull] string constraintType)
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

        static DataTable GetConstraintColumns(NpgsqlConnection conn, [CanBeNull] string[] restrictions)
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

        static DataTable GetDataSourceInformation(NpgsqlConnection conn)
        {
            var table = new DataTable("DataSourceInformation");
            var row = table.Rows.Add();

            table.Columns.Add("CompositeIdentifierSeparatorPattern", typeof(string));
            // TODO: DefaultCatalog? Was in XML (unfilled) but isn't in docs
            table.Columns.Add("DataSourceProductName", typeof(string));
            table.Columns.Add("DataSourceProductVersion", typeof(string));
            table.Columns.Add("DataSourceProductVersionNormalized", typeof(string));
            table.Columns.Add("GroupByBehavior", typeof(GroupByBehavior));
            table.Columns.Add("IdentifierPattern", typeof(string));
            table.Columns.Add("IdentifierCase", typeof(IdentifierCase));
            table.Columns.Add("OrderByColumnsInSelect", typeof(bool));
            table.Columns.Add("ParameterMarkerFormat", typeof(string));
            table.Columns.Add("ParameterMarkerPattern", typeof(string));
            table.Columns.Add("ParameterNameMaxLength", typeof(int));
            table.Columns.Add("QuotedIdentifierPattern", typeof(string));
            table.Columns.Add("QuotedIdentifierCase", typeof(IdentifierCase));
            table.Columns.Add("ParameterNamePattern", typeof(string));
            table.Columns.Add("StatementSeparatorPattern", typeof(string));
            table.Columns.Add("StringLiteralPattern", typeof(string));
            table.Columns.Add("SupportedJoinOperators", typeof(SupportedJoinOperators));

            var version = conn.PostgreSqlVersion;
            var normalizedVersion = $"{version.Major:00}.{version.Minor:00}";
            if (version.Build >= 0)
                normalizedVersion += $".{version.Build:00}";

            row["CompositeIdentifierSeparatorPattern"] = @"\.";
            row["DataSourceProductName"] = "Npgsql";
            row["DataSourceProductVersion"] = version.ToString();
            row["DataSourceProductVersionNormalized"] = normalizedVersion;
            row["GroupByBehavior"] = GroupByBehavior.Unrelated;
            row["IdentifierPattern"] = @"(^\[\p{Lo}\p{Lu}\p{Ll}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Nd}@$#_]*$)|(^\[[^\]\0]|\]\]+\]$)|(^\""[^\""\0]|\""\""+\""$)";
            row["IdentifierCase"] = IdentifierCase.Insensitive;
            row["OrderByColumnsInSelect"] = false;
            row["ParameterMarkerFormat"] = @"{0}";  // TODO: Not sure
            row["ParameterMarkerPattern"] = @"@[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            row["ParameterNameMaxLength"] = 63; // For function out parameters
            row["QuotedIdentifierPattern"] = @"""(([^\""]|\""\"")*)""";
            row["QuotedIdentifierCase"] = IdentifierCase.Sensitive;
            row["ParameterNamePattern"] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            row["StatementSeparatorPattern"] = ";";
            row["StringLiteralPattern"] = @"'(([^']|'')*)'";
            row["SupportedJoinOperators"] =
                SupportedJoinOperators.FullOuter |
                SupportedJoinOperators.Inner |
                SupportedJoinOperators.LeftOuter |
                SupportedJoinOperators.RightOuter;

            return table;
        }

        #region DataTypes

        static DataTable GetDataTypes(NpgsqlConnection conn)
        {
            var connector = conn.Connector;

            var table = new DataTable("DataTypes");

            table.Columns.Add("TypeName", typeof(string));
            table.Columns.Add("ColumnSize", typeof(long));
            table.Columns.Add("CreateFormat", typeof(string));
            table.Columns.Add("CreateParameters", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("IsAutoIncrementable", typeof(bool));
            table.Columns.Add("IsBestMatch", typeof(bool));
            table.Columns.Add("IsCaseSensitive", typeof(bool));
            table.Columns.Add("IsConcurrencyType", typeof(bool));
            table.Columns.Add("IsFixedLength", typeof(bool));
            table.Columns.Add("IsFixedPrecisionAndScale", typeof(bool));
            table.Columns.Add("IsLiteralSupported", typeof(bool));
            table.Columns.Add("IsLong", typeof(bool));
            table.Columns.Add("IsNullable", typeof(bool));
            table.Columns.Add("IsSearchable", typeof(bool));
            table.Columns.Add("IsSearchableWithLike", typeof(bool));
            table.Columns.Add("IsUnsigned", typeof(bool));
            table.Columns.Add("LiteralPrefix", typeof(string));
            table.Columns.Add("LiteralSuffix", typeof(string));
            table.Columns.Add("MaximumScale", typeof(short));
            table.Columns.Add("MinimumScale", typeof(short));
            table.Columns.Add("NativeDataType", typeof(string));
            table.Columns.Add("ProviderDbType", typeof(int));

            // Npgsql-specific
            table.Columns.Add("OID", typeof(uint));

            // TODO: Support type name restriction

            foreach (var baseType in connector.DatabaseInfo.BaseTypes)
            {
                if (!connector.TypeMapper.Mappings.TryGetValue(baseType.Name, out var mapping) &&
                    !connector.TypeMapper.Mappings.TryGetValue(baseType.FullName, out mapping))
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, baseType);
                PopulateHardcodedDataTypeInfo(row, baseType);

                if (mapping.ClrTypes.Length > 0)
                    row["DataType"] = mapping.ClrTypes[0].FullName;
                if (mapping.NpgsqlDbType.HasValue)
                    row["ProviderDbType"] = (int)mapping.NpgsqlDbType.Value;
            }

            foreach (var arrayType in connector.DatabaseInfo.ArrayTypes)
            {
                if (!connector.TypeMapper.Mappings.TryGetValue(arrayType.Element.Name, out var elementMapping) &&
                    !connector.TypeMapper.Mappings.TryGetValue(arrayType.Element.FullName, out elementMapping))
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, arrayType.Element);
                // Populate hardcoded values based on the element type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, arrayType.Element);

                row["TypeName"] = arrayType.DisplayName;
                row["OID"] = arrayType.OID;
                row["CreateFormat"] += "[]";
                if (elementMapping.ClrTypes.Length > 0)
                    row["DataType"] = elementMapping.ClrTypes[0].MakeArrayType().FullName;
                if (elementMapping.NpgsqlDbType.HasValue)
                    row["ProviderDbType"] = (int)(elementMapping.NpgsqlDbType.Value | NpgsqlDbType.Array);
            }

            foreach (var rangeType in connector.DatabaseInfo.RangeTypes)
            {
                if (!connector.TypeMapper.Mappings.TryGetValue(rangeType.Subtype.Name, out var elementMapping) &&
                    !connector.TypeMapper.Mappings.TryGetValue(rangeType.Subtype.FullName, out elementMapping))
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, rangeType.Subtype);
                // Populate hardcoded values based on the element type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, rangeType.Subtype);

                row["TypeName"] = rangeType.DisplayName;
                row["OID"] = rangeType.OID;
                row["CreateFormat"] = rangeType.DisplayName.ToUpperInvariant();
                if (elementMapping.ClrTypes.Length > 0)
                    row["DataType"] = typeof(NpgsqlRange<>).MakeGenericType(elementMapping.ClrTypes[0]).FullName;
                if (elementMapping.NpgsqlDbType.HasValue)
                    row["ProviderDbType"] = (int)(elementMapping.NpgsqlDbType.Value | NpgsqlDbType.Range);
            }

            foreach (var enumType in connector.DatabaseInfo.EnumTypes)
            {
                if (!connector.TypeMapper.Mappings.TryGetValue(enumType.Name, out var mapping) &&
                    !connector.TypeMapper.Mappings.TryGetValue(enumType.FullName, out mapping))
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, enumType);
                PopulateHardcodedDataTypeInfo(row, enumType);

                if (mapping.ClrTypes.Length > 0)
                    row["DataType"] = mapping.ClrTypes[0].FullName;
            }

            foreach (var compositeType in connector.DatabaseInfo.CompositeTypes)
            {
                if (!connector.TypeMapper.Mappings.TryGetValue(compositeType.Name, out var mapping) &&
                    !connector.TypeMapper.Mappings.TryGetValue(compositeType.FullName, out mapping))
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, compositeType);
                PopulateHardcodedDataTypeInfo(row, compositeType);

                if (mapping.ClrTypes.Length > 0)
                    row["DataType"] = mapping.ClrTypes[0].FullName;
            }

            foreach (var domainType in connector.DatabaseInfo.DomainTypes)
            {
                if (!connector.TypeMapper.Mappings.TryGetValue(domainType.BaseType.Name, out var baseMapping) &&
                    !connector.TypeMapper.Mappings.TryGetValue(domainType.BaseType.FullName, out baseMapping))
                    continue;

                var row = table.Rows.Add();

                PopulateDefaultDataTypeInfo(row, domainType.BaseType);
                // Populate hardcoded values based on the element type (e.g. citext[] is case-insensitive).
                PopulateHardcodedDataTypeInfo(row, domainType.BaseType);
                row["TypeName"] = domainType.DisplayName;
                row["OID"] = domainType.OID;
                // A domain is never the best match, since its underlying base type is
                row["IsBestMatch"] = false;

                if (baseMapping.ClrTypes.Length > 0)
                    row["DataType"] = baseMapping.ClrTypes[0].FullName;
                if (baseMapping.NpgsqlDbType.HasValue)
                    row["ProviderDbType"] = (int)baseMapping.NpgsqlDbType.Value;
            }

            return table;
        }

        /// <summary>
        /// Populates some generic type information that is common for base types, arrays, enums, etc. Some will
        /// be overridden later.
        /// </summary>
        static void PopulateDefaultDataTypeInfo(DataRow row, PostgresType type)
        {
            row["TypeName"] = type.DisplayName;
            // Skipping ColumnSize at least for now, not very meaningful
            row["CreateFormat"] = type.DisplayName.ToUpperInvariant();
            row["CreateParameters"] = "";
            row["IsAutoIncrementable"] = false;
            // We populate the DataType above from mapping.ClrTypes, which means we take the .NET type from
            // which we *infer* the PostgreSQL type. Since only a single PostgreSQL type gets inferred from a given
            // .NET type, we never have the same DataType in more than one row - so the mapping is always the
            // best match. See the hardcoding override  below for some exceptions.
            row["IsBestMatch"] = true;
            row["IsCaseSensitive"] = true;
            row["IsConcurrencyType"] = false;
            row["IsFixedLength"] = false;
            row["IsFixedPrecisionAndScale"] = false;
            row["IsLiteralSupported"] = false;  // See hardcoding override below
            row["IsLong"] = false;
            row["IsNullable"] = true;
            row["IsSearchable"] = true;
            row["IsSearchableWithLike"] = false;
            row["IsUnsigned"] = DBNull.Value; // See hardcoding override below
            // LiteralPrefix/Suffix: no literal for now except for strings, see hardcoding override below
            row["MaximumScale"] = DBNull.Value;
            row["MinimumScale"] = DBNull.Value;
            // NativeDataType is unset
            row["OID"] = type.OID;
        }

        /// <summary>
        /// Sets some custom, hardcoded info on a DataType row that cannot be loaded/inferred from PostgreSQL
        /// </summary>
        static void PopulateHardcodedDataTypeInfo(DataRow row, PostgresType type)
        {
            switch (type.Name)
            {
            case "varchar":
            case "char":
                row["DataType"] = "String";
                row["IsBestMatch"] = false;
                goto case "text";
            case "text":
                row["CreateFormat"] += "({0})";
                row["CreateParameters"] = "size";
                row["IsSearchableWithLike"] = true;
                row["IsLiteralSupported"] = true;
                row["LiteralPrefix"] = "'";
                row["LiteralSuffix"] = "'";
                return;
            case "numeric":
                row["CreateFormat"] += "({0},{1})";
                row["CreateParameters"] = "precision, scale";
                row["MaximumScale"] = 16383;
                row["MinimumScale"] = 16383;
                row["IsUnsigned"] = false;
                return;
            case "bytea":
                row["IsLong"] = true;
                return;
            case "citext":
                row["IsCaseSensitive"] = false;
                return;
            case "integer":
            case "smallint":
            case "bigint":
            case "double precision":
            case "real":
            case "money":
                row["IsUnsigned"] = false;
                return;
            case "oid":
            case "cid":
            case "regtype":
            case "regconfig":
                row["IsUnsigned"] = true;
                return;
            case "xid":
                row["IsUnsigned"] = true;
                row["IsConcurrencyType"] = true;
                return;
            }
        }

        #endregion DataTypes

        #region Reserved Keywords

        static DataTable GetReservedWords()
        {
            var table = new DataTable("ReservedWords") { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("ReservedWord", typeof(string));
            foreach (var keyword in ReservedKeywords)
                table.Rows.Add(keyword);
            return table;
        }

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
