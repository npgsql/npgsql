#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Globalization;
using System.Text;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Spatial;
using System.Linq;

namespace Npgsql
{
    /// <summary>
    /// Used to generate migration sql
    /// </summary>
    public class NpgsqlMigrationSqlGenerator : MigrationSqlGenerator
    {
        List<MigrationStatement> migrationStatments;
        private List<string> addedSchemas;
        private List<string> addedExtensions;
        private Version serverVersion;

        /// <summary>
        /// Generates the migration sql.
        /// </summary>
        /// <param name="migrationOperations">The operations in the migration</param>
        /// <param name="providerManifestToken">The provider manifest token used for server versioning.</param>
        public override IEnumerable<MigrationStatement> Generate(
            IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            migrationStatments = new List<MigrationStatement>();
            addedSchemas = new List<string>();
            addedExtensions = new List<string>();
            serverVersion = new Version(providerManifestToken);
            Convert(migrationOperations);
            return migrationStatments;
        }

        #region MigrationOperation to MigrationStatement converters

        #region General

        protected virtual void Convert(IEnumerable<MigrationOperation> operations)
        {
            foreach (var migrationOperation in operations)
            {
                if (migrationOperation is AddColumnOperation)
                {
                    Convert(migrationOperation as AddColumnOperation);
                }
                else if (migrationOperation is AlterColumnOperation)
                {
                    Convert(migrationOperation as AlterColumnOperation);
                }
                else if (migrationOperation is CreateTableOperation)
                {
                    Convert(migrationOperation as CreateTableOperation);
                }
                else if (migrationOperation is DropForeignKeyOperation)
                {
                    Convert(migrationOperation as DropForeignKeyOperation);
                }
                else if (migrationOperation is DropTableOperation)
                {
                    Convert(migrationOperation as DropTableOperation);
                }
                else if (migrationOperation is MoveTableOperation)
                {
                    Convert(migrationOperation as MoveTableOperation);
                }
                else if (migrationOperation is RenameTableOperation)
                {
                    Convert(migrationOperation as RenameTableOperation);
                }
                else if (migrationOperation is AddForeignKeyOperation)
                {
                    Convert(migrationOperation as AddForeignKeyOperation);
                }
                else if (migrationOperation is DropIndexOperation)
                {
                    Convert(migrationOperation as DropIndexOperation);
                }
                else if (migrationOperation is SqlOperation)
                {
                    AddStatment((migrationOperation as SqlOperation).Sql, (migrationOperation as SqlOperation).SuppressTransaction);
                }
                else if (migrationOperation is AddPrimaryKeyOperation)
                {
                    Convert(migrationOperation as AddPrimaryKeyOperation);
                }
                else if (migrationOperation is CreateIndexOperation)
                {
                    Convert(migrationOperation as CreateIndexOperation);
                }
                else if (migrationOperation is DropColumnOperation)
                {
                    Convert(migrationOperation as DropColumnOperation);
                }
                else if (migrationOperation is DropPrimaryKeyOperation)
                {
                    Convert(migrationOperation as DropPrimaryKeyOperation);
                }
                else if (migrationOperation is HistoryOperation)
                {
                    Convert(migrationOperation as HistoryOperation);
                }
                else if (migrationOperation is RenameColumnOperation)
                {
                    Convert(migrationOperation as RenameColumnOperation);
                }
                else if (migrationOperation is UpdateDatabaseOperation)
                {
                    Convert((migrationOperation as UpdateDatabaseOperation).Migrations as IEnumerable<MigrationOperation>);
                }
                else
                {
                    throw new NotImplementedException("Unhandled MigrationOperation " + migrationOperation.GetType().Name + " in " + GetType().Name);
                }
            }
        }

        private void AddStatment(string sql, bool suppressTransacion = false)
        {
            migrationStatments.Add(new MigrationStatement
            {
                Sql = sql,
                SuppressTransaction = suppressTransacion,
                BatchTerminator = ";"
            });
        }

        private void AddStatment(StringBuilder sql, bool suppressTransacion = false)
        {
            AddStatment(sql.ToString(), suppressTransacion);
        }

        #endregion

        #region History

        protected virtual void Convert(HistoryOperation historyOperation)
        {
            foreach (var command in historyOperation.CommandTrees)
            {
                var npgsqlCommand = new NpgsqlCommand();
                NpgsqlServices.Instance.TranslateCommandTree(serverVersion, command, npgsqlCommand, false);
                AddStatment(npgsqlCommand.CommandText);
            }
        }

        #endregion

        #region Tables

        protected virtual void Convert(CreateTableOperation createTableOperation)
        {
            StringBuilder sql = new StringBuilder();
            int dotIndex = createTableOperation.Name.IndexOf('.');
            if (dotIndex != -1)
            {
                CreateSchema(createTableOperation.Name.Remove(dotIndex));
            }

            sql.Append("CREATE TABLE ");
            AppendTableName(createTableOperation.Name, sql);
            sql.Append('(');
            foreach (var column in createTableOperation.Columns)
            {
                AppendColumn(column, sql);
                sql.Append(",");
            }
            if (createTableOperation.Columns.Any())
                sql.Remove(sql.Length - 1, 1);
            if (createTableOperation.PrimaryKey != null)
            {
                sql.Append(",");
                sql.Append("CONSTRAINT ");
                sql.Append('"');
                sql.Append(createTableOperation.PrimaryKey.Name);
                sql.Append('"');
                sql.Append(" PRIMARY KEY ");
                sql.Append("(");
                foreach (var column in createTableOperation.PrimaryKey.Columns)
                {
                    sql.Append('"');
                    sql.Append(column);
                    sql.Append("\",");
                }
                sql.Remove(sql.Length - 1, 1);
                sql.Append(")");
            }
            sql.Append(")");
            AddStatment(sql);
        }

        protected virtual void Convert(DropTableOperation dropTableOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("DROP TABLE ");
            AppendTableName(dropTableOperation.Name, sql);
            AddStatment(sql);
        }

        protected virtual void Convert(RenameTableOperation renameTableOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(renameTableOperation.Name, sql);
            sql.Append(" RENAME TO ");
            AppendTableName(renameTableOperation.NewName, sql);
            AddStatment(sql);
        }

        private void CreateSchema(string schemaName)
        {
            if (schemaName == "public" || addedSchemas.Contains(schemaName))
                return;
            addedSchemas.Add(schemaName);
            if (serverVersion.Major > 9 || (serverVersion.Major == 9 && serverVersion.Minor >= 3))
            {
                AddStatment("CREATE SCHEMA IF NOT EXISTS " + schemaName);
            }
            else
            {
                //TODO: CREATE PROCEDURE that checks if schema already exists on servers < 9.3
                AddStatment("CREATE SCHEMA " + schemaName);
            }
        }

        //private void CreateExtension(string exensionName)
        //{
        //    //This is compatible only with server 9.1+
        //    if (serverVersion.Major > 9 || (serverVersion.Major == 9 && serverVersion.Minor >= 1))
        //    {
        //        if (addedExtensions.Contains(exensionName))
        //            return;
        //        addedExtensions.Add(exensionName);
        //        AddStatment("CREATE EXTENSION IF NOT EXISTS \"" + exensionName + "\"");
        //    }
        //}

        protected virtual void Convert(MoveTableOperation moveTableOperation)
        {
            StringBuilder sql = new StringBuilder();
            var newSchema = moveTableOperation.NewSchema ?? "dbo";
            CreateSchema(newSchema);
            sql.Append("ALTER TABLE ");
            AppendTableName(moveTableOperation.Name, sql);
            sql.Append(" SET SCHEMA ");
            sql.Append(newSchema);
            AddStatment(sql);
        }

        #endregion

        #region Columns
        protected virtual void Convert(AddColumnOperation addColumnOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(addColumnOperation.Table, sql);
            sql.Append(" ADD ");
            AppendColumn(addColumnOperation.Column, sql);
            AddStatment(sql);
        }

        protected virtual void Convert(DropColumnOperation dropColumnOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(dropColumnOperation.Table, sql);
            sql.Append(" DROP COLUMN \"");
            sql.Append(dropColumnOperation.Name);
            sql.Append('"');
            AddStatment(sql);
        }

        protected virtual void Convert(AlterColumnOperation alterColumnOperation)
        {
            StringBuilder sql = new StringBuilder();

            //TYPE
            AppendAlterColumn(alterColumnOperation, sql);
            sql.Append(" TYPE ");
            AppendColumnType(alterColumnOperation.Column, sql, false);
            AddStatment(sql);
            sql.Clear();

            //NOT NULL
            AppendAlterColumn(alterColumnOperation, sql);
            if (alterColumnOperation.Column.IsNullable != null && !alterColumnOperation.Column.IsNullable.Value)
                sql.Append(" SET NOT NULL");
            else
                sql.Append(" DROP NOT NULL");
            AddStatment(sql);
            sql.Clear();

            //DEFAULT
            AppendAlterColumn(alterColumnOperation, sql);
            if (alterColumnOperation.Column.DefaultValue != null)
            {
                sql.Append(" SET DEFAULT ");
                AppendValue(alterColumnOperation.Column.DefaultValue, sql);
            }
            else if (!string.IsNullOrWhiteSpace(alterColumnOperation.Column.DefaultValueSql))
            {
                sql.Append(" SET DEFAULT ");
                sql.Append(alterColumnOperation.Column.DefaultValueSql);
            }
            else if (alterColumnOperation.Column.IsIdentity)
            {
                sql.Append(" SET DEFAULT ");
                switch (alterColumnOperation.Column.Type)
                {
                    case PrimitiveTypeKind.Byte:
                    case PrimitiveTypeKind.SByte:
                    case PrimitiveTypeKind.Int16:
                    case PrimitiveTypeKind.Int32:
                    case PrimitiveTypeKind.Int64:
                        //TODO: need function CREATE SEQUENCE IF NOT EXISTS and set to it...
                        //Until this is resolved changing IsIdentity from false to true
                        //on types int2, int4 and int8 won't switch to type serial2, serial4 and serial8
                        throw new NotImplementedException("Not supporting creating sequence for integer types");
                    case PrimitiveTypeKind.Guid:
                        //CreateExtension("uuid-ossp");
                        //If uuid-ossp is not enabled migrations throw exception
                        AddStatment("select * from uuid_generate_v4()");
                        sql.Append("uuid_generate_v4()");
                        break;
                    default:
                        throw new NotImplementedException("Not supporting creating IsIdentity for " + alterColumnOperation.Column.Type);
                }
            }
            else
            {
                sql.Append(" DROP DEFAULT");
            }
            AddStatment(sql);
        }

        private void AppendAlterColumn(AlterColumnOperation alterColumnOperation, StringBuilder sql)
        {
            sql.Append("ALTER TABLE ");
            AppendTableName(alterColumnOperation.Table, sql);
            sql.Append(" ALTER COLUMN \"");
            sql.Append(alterColumnOperation.Column.Name);
            sql.Append('"');
        }

        protected virtual void Convert(RenameColumnOperation renameColumnOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(renameColumnOperation.Table, sql);
            sql.Append(" RENAME COLUMN \"");
            sql.Append(renameColumnOperation.Name);
            sql.Append("\" TO \"");
            sql.Append(renameColumnOperation.NewName);
            sql.Append('"');
            AddStatment(sql);
        }

        #endregion

        #region Keys and indexes

        protected virtual void Convert(AddForeignKeyOperation addForeignKeyOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(addForeignKeyOperation.DependentTable, sql);
            sql.Append(" ADD CONSTRAINT \"");
            sql.Append(addForeignKeyOperation.Name);
            sql.Append("\" FOREIGN KEY (");
            foreach (var column in addForeignKeyOperation.DependentColumns)
            {
                sql.Append('"');
                sql.Append(column);
                sql.Append("\",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(") REFERENCES ");
            AppendTableName(addForeignKeyOperation.PrincipalTable, sql);
            sql.Append(" (");
            foreach (var column in addForeignKeyOperation.PrincipalColumns)
            {
                sql.Append('"');
                sql.Append(column);
                sql.Append("\",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            if (addForeignKeyOperation.CascadeDelete)
            {
                sql.Append(" ON DELETE CASCADE");
            }
            AddStatment(sql);
        }

        protected virtual void Convert(DropForeignKeyOperation dropForeignKeyOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(dropForeignKeyOperation.DependentTable, sql);
            if (serverVersion.Major < 9)
                sql.Append(" DROP CONSTRAINT \"");//TODO: http://piecesformthewhole.blogspot.com/2011/04/dropping-foreign-key-if-it-exists-in.html ?
            else
                sql.Append(" DROP CONSTRAINT IF EXISTS \"");
            sql.Append(dropForeignKeyOperation.Name);
            sql.Append('"');
            AddStatment(sql);
        }

        protected virtual void Convert(CreateIndexOperation createIndexOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE ");

            if (createIndexOperation.IsUnique)
                sql.Append("UNIQUE ");

            sql.Append("INDEX \"");
            sql.Append(GetTableNameFromFullTableName(createIndexOperation.Table) + "_" + createIndexOperation.Name);
            sql.Append("\" ON ");
            AppendTableName(createIndexOperation.Table, sql);
            sql.Append(" (");
            foreach (var column in createIndexOperation.Columns)
            {
                sql.Append('"');
                sql.Append(column);
                sql.Append("\",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            AddStatment(sql);
        }

        private string GetSchemaNameFromFullTableName(string tableFullName)
        {
            int dotIndex = tableFullName.IndexOf('.');
            if (dotIndex != -1)
                return tableFullName.Remove(dotIndex);
            else
                return "dto";//TODO: Check always setting dto schema if no schema in table name is not bug
        }

        /// <summary>
        /// Removes schema prefix e.g. "dto.Blogs" returns "Blogs" and "Posts" returns "Posts"
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTableNameFromFullTableName(string tableName)
        {
            int dotIndex = tableName.IndexOf('.');
            if (dotIndex != -1)
                return tableName.Substring(dotIndex + 1);
            else
                return tableName;
        }

        protected virtual void Convert(DropIndexOperation dropIndexOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("DROP INDEX IF EXISTS ");
            sql.Append(GetSchemaNameFromFullTableName(dropIndexOperation.Table));
            sql.Append(".\"");
            sql.Append(GetTableNameFromFullTableName(dropIndexOperation.Table) + "_" + dropIndexOperation.Name);
            sql.Append('"');
            AddStatment(sql);
        }

        protected virtual void Convert(AddPrimaryKeyOperation addPrimaryKeyOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(addPrimaryKeyOperation.Table, sql);
            sql.Append(" ADD CONSTRAINT \"");
            sql.Append(addPrimaryKeyOperation.Name);
            sql.Append("\" PRIMARY KEY ");

            sql.Append("(");
            foreach (var column in addPrimaryKeyOperation.Columns)
            {
                sql.Append('"');
                sql.Append(column);
                sql.Append("\",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            AddStatment(sql);
        }

        protected virtual void Convert(DropPrimaryKeyOperation dropPrimaryKeyOperation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("ALTER TABLE ");
            AppendTableName(dropPrimaryKeyOperation.Table, sql);
            sql.Append(" DROP CONSTRAINT \"");
            sql.Append(dropPrimaryKeyOperation.Name);
            sql.Append('"');
            AddStatment(sql);
        }

        #endregion

        #endregion

        #region Misc functions

        private void AppendColumn(ColumnModel column, StringBuilder sql)
        {
            sql.Append('"');
            sql.Append(column.Name);
            sql.Append("\" ");
            AppendColumnType(column, sql, true);

            if (column.IsNullable != null && !column.IsNullable.Value)
                sql.Append(" NOT NULL");

            if (column.DefaultValue != null)
            {
                sql.Append(" DEFAULT ");
                AppendValue(column.DefaultValue, sql);
            }
            else if (!string.IsNullOrWhiteSpace(column.DefaultValueSql))
            {
                sql.Append(" DEFAULT ");
                sql.Append(column.DefaultValueSql);
            }
            else if (column.IsIdentity)
            {
                switch (column.Type)
                {
                    case PrimitiveTypeKind.Guid:
                        //CreateExtension("uuid-ossp");
                        //If uuid-ossp is not enabled migrations throw exception
                        AddStatment("select * from uuid_generate_v4()");
                        sql.Append(" DEFAULT uuid_generate_v4()");
                        break;
                    case PrimitiveTypeKind.Byte:
                    case PrimitiveTypeKind.SByte:
                    case PrimitiveTypeKind.Int16:
                    case PrimitiveTypeKind.Int32:
                    case PrimitiveTypeKind.Int64:
                        //TODO: Add support for setting "SERIAL"
                        break;
                }
            }
            else if (column.IsNullable != null
                && !column.IsNullable.Value
                && (column.StoreType == null ||
                (column.StoreType.IndexOf("rowversion", StringComparison.OrdinalIgnoreCase) == -1)))
            {
                sql.Append(" DEFAULT ");
                AppendValue(column.ClrDefaultValue, sql);
            }
        }

        private void AppendColumnType(ColumnModel column, StringBuilder sql, bool setSerial)
        {
            switch (column.Type)
            {
                case PrimitiveTypeKind.Binary:
                    sql.Append("bytea");
                    break;
                case PrimitiveTypeKind.Boolean:
                    sql.Append("boolean");
                    break;
                case PrimitiveTypeKind.DateTime:
                    if (column.Precision != null)
                        sql.Append("timestamp(" + column.Precision + ")");
                    else
                        sql.Append("timestamp");
                    break;
                case PrimitiveTypeKind.Decimal:
                    //TODO: Check if inside min/max
                    if (column.Precision == null && column.Scale == null)
                    {
                        sql.Append("numeric");
                    }
                    else
                    {
                        sql.Append("numeric(");
                        sql.Append(column.Precision ?? 19);
                        sql.Append(',');
                        sql.Append(column.Scale ?? 4);
                        sql.Append(')');
                    }
                    break;
                case PrimitiveTypeKind.Double:
                    sql.Append("float8");
                    break;
                case PrimitiveTypeKind.Guid:
                    sql.Append("uuid");
                    break;
                case PrimitiveTypeKind.Single:
                    sql.Append("float4");
                    break;
                case PrimitiveTypeKind.Byte://postgres doesn't support sbyte :(
                case PrimitiveTypeKind.SByte://postgres doesn't support sbyte :(
                case PrimitiveTypeKind.Int16:
                    if (setSerial)
                        sql.Append(column.IsIdentity ? "serial2" : "int2");
                    else
                        sql.Append("int2");
                    break;
                case PrimitiveTypeKind.Int32:
                    if (setSerial)
                        sql.Append(column.IsIdentity ? "serial4" : "int4");
                    else
                        sql.Append("int4");
                    break;
                case PrimitiveTypeKind.Int64:
                    if (setSerial)
                        sql.Append(column.IsIdentity ? "serial8" : "int8");
                    else
                        sql.Append("int8");
                    break;
                case PrimitiveTypeKind.String:
                    if (column.IsFixedLength.HasValue &&
                        column.IsFixedLength.Value &&
                        column.MaxLength.HasValue)
                    {
                        sql.AppendFormat("char({0})",column.MaxLength.Value);
                    }
                    else if (column.MaxLength.HasValue)
                    {
                        sql.AppendFormat("varchar({0})", column.MaxLength);
                    }
                    else
                    {
                        sql.Append("text");
                    }
                    break;
                case PrimitiveTypeKind.Time:
                    if (column.Precision != null)
                    {
                        sql.Append("interval(");
                        sql.Append(column.Precision);
                        sql.Append(')');
                    }
                    else
                    {
                        sql.Append("interval");
                    }
                    break;
                case PrimitiveTypeKind.DateTimeOffset:
                    if (column.Precision != null)
                    {
                        sql.Append("timestamptz(");
                        sql.Append(column.Precision);
                        sql.Append(')');
                    }
                    else
                    {
                        sql.Append("timestamptz");
                    }
                    break;
                case PrimitiveTypeKind.Geometry:
                    sql.Append("point");
                    break;
                //case PrimitiveTypeKind.Geography:
                //    break;
                //case PrimitiveTypeKind.GeometryPoint:
                //    break;
                //case PrimitiveTypeKind.GeometryLineString:
                //    break;
                //case PrimitiveTypeKind.GeometryPolygon:
                //    break;
                //case PrimitiveTypeKind.GeometryMultiPoint:
                //    break;
                //case PrimitiveTypeKind.GeometryMultiLineString:
                //    break;
                //case PrimitiveTypeKind.GeometryMultiPolygon:
                //    break;
                //case PrimitiveTypeKind.GeometryCollection:
                //    break;
                //case PrimitiveTypeKind.GeographyPoint:
                //    break;
                //case PrimitiveTypeKind.GeographyLineString:
                //    break;
                //case PrimitiveTypeKind.GeographyPolygon:
                //    break;
                //case PrimitiveTypeKind.GeographyMultiPoint:
                //    break;
                //case PrimitiveTypeKind.GeographyMultiLineString:
                //    break;
                //case PrimitiveTypeKind.GeographyMultiPolygon:
                //    break;
                //case PrimitiveTypeKind.GeographyCollection:
                //    break;
                default:
                    throw new ArgumentException("Unhandled column type:" + column.Type);
            }
        }

        private void AppendTableName(string tableName, StringBuilder sql)
        {
            int dotIndex = tableName.IndexOf('.');
            if (dotIndex == -1)
            {
                sql.Append('"');
                sql.Append(tableName);
                sql.Append('"');
            }
            else
            {
                sql.Append('"');
                sql.Append(tableName.Remove(dotIndex));
                sql.Append("\".\"");
                sql.Append(tableName.Substring(dotIndex + 1));
                sql.Append('"');
            }
        }

        #endregion

        #region Value appenders

        private void AppendValue(byte[] values, StringBuilder sql)
        {
            if (values.Length == 0)
            {
                sql.Append("''");
            }
            else
            {
                sql.Append("E'\\\\");
                foreach (var value in values)
                    sql.Append(value.ToString("X2"));
                sql.Append("'");
            }
        }

        private void AppendValue(bool value, StringBuilder sql)
        {
            sql.Append(value ? "TRUE" : "FALSE");
        }

        private void AppendValue(DateTime value, StringBuilder sql)
        {
            sql.Append("'");
            sql.Append(new NpgsqlTypes.NpgsqlDateTime(value));
            sql.Append("'");
        }

        private void AppendValue(DateTimeOffset value, StringBuilder sql)
        {
            sql.Append("'");
            sql.Append(new NpgsqlTypes.NpgsqlDateTime(value.UtcDateTime));
            sql.Append("'");
        }

        private void AppendValue(Guid value, StringBuilder sql)
        {
            sql.Append("'");
            sql.Append(value);
            sql.Append("'");
        }

        private void AppendValue(string value, StringBuilder sql)
        {
            sql.Append("'");
            sql.Append(value);
            sql.Append("'");
        }

        private void AppendValue(TimeSpan value, StringBuilder sql)
        {
            sql.Append("'");
            sql.Append(new NpgsqlTypes.NpgsqlTimeSpan(value).ToString());
            sql.Append("'");
        }

        private void AppendValue(DbGeometry value, StringBuilder sql)
        {
            sql.Append("'");
            sql.Append(value);
            sql.Append("'");
        }

        private void AppendValue(object value, StringBuilder sql)
        {
            if (value is byte[])
            {
                AppendValue((byte[])value, sql);
            }
            else if (value is bool)
            {
                AppendValue((bool)value, sql);
            }
            else if (value is DateTime)
            {
                AppendValue((DateTime)value, sql);
            }
            else if (value is DateTimeOffset)
            {
                AppendValue((DateTimeOffset)value, sql);
            }
            else if (value is Guid)
            {
                AppendValue((Guid)value, sql);
            }
            else if (value is string)
            {
                AppendValue((string)value, sql);
            }
            else if (value is TimeSpan)
            {
                AppendValue((TimeSpan)value, sql);
            }
            else if (value is DbGeometry)
            {
                AppendValue((DbGeometry)value, sql);
            }
            else
            {
                sql.Append(string.Format(CultureInfo.InvariantCulture, "{0}", value));
            }
        }

        #endregion
    }
}
