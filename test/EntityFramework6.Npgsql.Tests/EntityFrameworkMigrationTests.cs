#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Npgsql.Tests;

namespace EntityFramework6.Npgsql.Tests
{
    public class EntityFrameworkMigrationTests : TestBase
    {
        #region Helper method

        /// <summary>
        /// Helper function which I put behind var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, BackendVersion.ToString());
        /// after filling operations list run test and then copy-paste content from D:\temp\printedCode.txt after var statments = new Np..
        /// Ofcourse check if generated code makes sense or UnitTests would be useless :D
        /// </summary>
        /// <param name="statments"></param>
        private void PrintCode(IEnumerable<System.Data.Entity.Migrations.Sql.MigrationStatement> statments)
        {

            using (var SW = new System.IO.StreamWriter(@"D:\temp\printedCode.txt"))
            {
                SW.WriteLine("Assert.AreEqual(" + statments.Count() + ", statments.Count());");
                int i = 0;
                foreach (var statement in statments)
                    SW.WriteLine("Assert.AreEqual(\"" + statement.Sql.Replace("\"", "\\\"") + "\", statments.ElementAt(" + i++ + ").Sql);");
            }
            Assert.Fail();
        }

        #endregion

        #region Actual test against database

        [Test]
        public void CreateBloggingContext()
        {
            using (var db = new BloggingContext(new NpgsqlConnection(ConnectionStringEF)))
            {
                if (!(db.Database.Connection is NpgsqlConnection))
                {
                    Assert.Fail(
                           "Connection type is \"" + db.Database.Connection.GetType().FullName + "\" should be \"" + typeof(NpgsqlConnection).FullName + "\"." + Environment.NewLine +
                           "Most likely wrong configuration in App.config file of Tests project.");
                }
                db.Database.Delete();
                var blog = new Blog { Name = "blogNameTest1" };
                db.Blogs.Add(blog);
                db.SaveChanges();

                var query = from b in db.Blogs
                            where b.Name == "blogNameTest1"
                            select b;
                Assert.AreEqual(1, query.Count());

                db.Database.Connection.Open();
                using (var command = db.Database.Connection.CreateCommand())
                {
                    command.CommandText = "select column_name, data_type, is_nullable, column_default from information_schema.columns where table_name = 'Blogs';";
                    List<string> expectedColumns = new List<string>(new string[] { "Name", "BlogId" });
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine((string)reader[0] + " " + (string)reader[1] + " " + (string)reader[2] + " " + (reader[3] ?? "").ToString());
                            switch ((string)reader[0])
                            {
                                case "Name":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("text", (string)reader[1]);
                                    Assert.AreEqual("YES", (string)reader[2]);
                                    Assert.That(string.IsNullOrEmpty(reader[3] as string));
                                    break;
                                case "BlogId":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("integer", (string)reader[1]);
                                    Assert.AreEqual("NO", (string)reader[2]);
                                    Assert.AreEqual("nextval('dbo.\"Blogs_BlogId_seq\"'::regclass)", reader[3] as string);
                                    break;
                                default:
                                    Assert.Fail("Unknown column '" + (string)reader[0] + "' in Blogs table.");
                                    break;
                            }
                        }
                    }
                    foreach (var columnName in expectedColumns)
                    {
                        Assert.Fail("Column '" + columnName + "' was not created in Blogs table.");
                    }
                }

                using (var command = db.Database.Connection.CreateCommand())
                {
                    command.CommandText = "select column_name, data_type, is_nullable, column_default from information_schema.columns where table_name = 'Posts';";
                    List<string> expectedColumns = new List<string>(new string[] { "PostId", "Title", "Content", "BlogId" });
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine((string)reader[0] + " " + (string)reader[1] + " " + (string)reader[2] + " " + (reader[3] ?? "").ToString());
                            switch ((string)reader[0])
                            {
                                case "PostId":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("integer", (string)reader[1]);
                                    Assert.AreEqual("NO", (string)reader[2]);
                                    Assert.AreEqual("nextval('dbo.\"Posts_PostId_seq\"'::regclass)", (string)reader[3]);
                                    break;
                                case "Title":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("text", (string)reader[1]);
                                    Assert.AreEqual("YES", (string)reader[2]);
                                    Assert.That(string.IsNullOrEmpty(reader[3] as string));
                                    break;
                                case "Content":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("text", (string)reader[1]);
                                    Assert.AreEqual("YES", (string)reader[2]);
                                    Assert.That(string.IsNullOrEmpty(reader[3] as string));
                                    break;
                                case "BlogId":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("integer", (string)reader[1]);
                                    Assert.AreEqual("NO", (string)reader[2]);
                                    Assert.AreEqual("0", (string)reader[3]);
                                    break;
                                case "UniqueId":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("uuid", (string)reader[1]);
                                    Assert.AreEqual("NO", (string)reader[2]);
                                    Assert.AreEqual("'00000000-0000-0000-0000-000000000000'::uuid", reader[3] as string);
                                    //Assert.AreEqual("uuid_generate_v4()", reader[3] as string);
                                    break;
                                case "Rating":
                                    expectedColumns.Remove((string)reader[0]);
                                    Assert.AreEqual("smallint", (string)reader[1]);
                                    Assert.AreEqual("YES", (string)reader[2]);
                                    Assert.That(string.IsNullOrEmpty(reader[3] as string));
                                    break;
                                default:
                                    Assert.Fail("Unknown column '" + (string)reader[0] + "' in Post table.");
                                    break;
                            }
                        }
                    }
                    foreach (var columnName in expectedColumns)
                    {
                        Assert.Fail("Column '" + columnName + "' was not created in Posts table.");
                    }
                }
            }
        }



        public class Blog
        {
            public int BlogId { get; set; }
            public string Name { get; set; }

            public virtual List<Post> Posts { get; set; }
        }

        public class Post
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public Guid UniqueId { get; set; }
            public byte? Rating { get; set; }

            public int BlogId { get; set; }
            public virtual Blog Blog { get; set; }
        }

        public class BloggingContext : DbContext
        {
            public BloggingContext(DbConnection connection)
                : base(connection, true)
            {
            }

            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }
        }

        #endregion

        [Test]
        public void DatabaseExistsCreateDelete()
        {
            using (var db = new BloggingContext(new NpgsqlConnection(ConnectionStringEF)))
            {
                if (db.Database.Exists())
                {
                    db.Database.Delete();
                    Assert.IsFalse(db.Database.Exists());
                    db.Database.Create();
                }
                else
                {
                    db.Database.Create();
                }
                Assert.IsTrue(db.Database.Exists());
                db.Database.Delete();
                Assert.IsFalse(db.Database.Exists());
            }
        }

        [Test]
        public void AddColumnOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new AddColumnOperation("tableName", new ColumnModel(PrimitiveTypeKind.Double)
            {
                Name = "columnName"
            }));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"tableName\" ADD \"columnName\" float8", statments.ElementAt(0).Sql);
        }

        [Test]
        public void AddColumnOperationDefaultValue()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new AddColumnOperation("tableName", new ColumnModel(PrimitiveTypeKind.Single)
            {
                Name = "columnName",
                DefaultValue = 4.4f
            }));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"tableName\" ADD \"columnName\" float4 DEFAULT 4.4", statments.ElementAt(0).Sql);
        }

        [Test]
        public void AddColumnOperationDefaultValueSql()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new AddColumnOperation("tableName", new ColumnModel(PrimitiveTypeKind.Single)
            {
                Name = "columnName",
                DefaultValueSql = "4.6"
            }));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"tableName\" ADD \"columnName\" float4 DEFAULT 4.6", statments.ElementAt(0).Sql);
        }

        [Test]
        public void AlterColumnOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new AlterColumnOperation("tableName", new ColumnModel(PrimitiveTypeKind.Double)
            {
                Name = "columnName"
            }, false));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(3, statments.Count());
            Assert.AreEqual("ALTER TABLE \"tableName\" ALTER COLUMN \"columnName\" TYPE float8", statments.ElementAt(0).Sql);
            Assert.AreEqual("ALTER TABLE \"tableName\" ALTER COLUMN \"columnName\" DROP NOT NULL", statments.ElementAt(1).Sql);
            Assert.AreEqual("ALTER TABLE \"tableName\" ALTER COLUMN \"columnName\" DROP DEFAULT", statments.ElementAt(2).Sql);
        }

        [Test]
        public void AlterColumnOperationDefaultAndNullable()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new AlterColumnOperation("tableName", new ColumnModel(PrimitiveTypeKind.Double)
            {
                Name = "columnName",
                DefaultValue = 2.3,
                IsNullable = false
            }, false));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(3, statments.Count());
            Assert.AreEqual("ALTER TABLE \"tableName\" ALTER COLUMN \"columnName\" TYPE float8", statments.ElementAt(0).Sql);
            Assert.AreEqual("ALTER TABLE \"tableName\" ALTER COLUMN \"columnName\" SET NOT NULL", statments.ElementAt(1).Sql);
            Assert.AreEqual("ALTER TABLE \"tableName\" ALTER COLUMN \"columnName\" SET DEFAULT 2.3", statments.ElementAt(2).Sql);
        }

        [Test]
        public void CreateTableOperation()
        {
            var operations = new List<MigrationOperation>();
            var operation = new CreateTableOperation("someSchema.someTable");

            operation.Columns.Add(
                new ColumnModel(PrimitiveTypeKind.String)
                {
                    Name = "SomeString",
                    MaxLength = 233,
                    IsNullable = false
                });

            operation.Columns.Add(
                new ColumnModel(PrimitiveTypeKind.String)
                {
                    Name = "AnotherString",
                    IsNullable = true
                });

            operation.Columns.Add(
                new ColumnModel(PrimitiveTypeKind.Binary)
                {
                    Name = "SomeBytes"
                });

            operation.Columns.Add(
                new ColumnModel(PrimitiveTypeKind.Int64)
                {
                    Name = "SomeLong",
                    IsIdentity = true
                });

            operation.Columns.Add(
                new ColumnModel(PrimitiveTypeKind.DateTime)
                {
                    Name = "SomeDateTime"
                });

            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(2, statments.Count());
            if (_backendVersion.Major > 9 || (_backendVersion.Major == 9 && _backendVersion.Minor > 2))
                Assert.AreEqual("CREATE SCHEMA IF NOT EXISTS someSchema", statments.ElementAt(0).Sql);
            else
                Assert.AreEqual("CREATE SCHEMA someSchema", statments.ElementAt(0).Sql);
            Assert.AreEqual("CREATE TABLE \"someSchema\".\"someTable\"(\"SomeString\" varchar(233) NOT NULL DEFAULT '',\"AnotherString\" text,\"SomeBytes\" bytea,\"SomeLong\" serial8,\"SomeDateTime\" timestamp)", statments.ElementAt(1).Sql);
        }

        [Test]
        public void CreateTableWithoutSchema()
        {
            var statements = new NpgsqlMigrationSqlGenerator().Generate(new List<MigrationOperation> { new CreateTableOperation("some_table") }, _backendVersion.ToString()).ToList();
            Assert.That(statements.Count, Is.EqualTo(1));
            Assert.That(statements[0].Sql, Is.EqualTo("CREATE TABLE \"some_table\"()"));
        }

        [Test]
        public void CreateTableInPublicSchema()
        {
            var statements = new NpgsqlMigrationSqlGenerator().Generate(new List<MigrationOperation> { new CreateTableOperation("public.some_table") }, _backendVersion.ToString()).ToList();
            Assert.That(statements.Count, Is.EqualTo(1));
            Assert.That(statements[0].Sql, Is.EqualTo("CREATE TABLE \"public\".\"some_table\"()"));
        }

        [Test]
        public void DropColumnOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new DropColumnOperation("someTable", "someColumn"));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someTable\" DROP COLUMN \"someColumn\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void DropTableOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new DropTableOperation("someTable"));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("DROP TABLE \"someTable\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void RenameTableOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new RenameTableOperation("schema.someOldTableName", "someNewTablename"));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"schema\".\"someOldTableName\" RENAME TO \"someNewTablename\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void HistoryOperation()
        {
            var operations = new List<MigrationOperation>();
            //TODO: fill operations
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            //TODO: check statments
        }

        [Test]
        public void DropIndexOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new DropIndexOperation()
            {
                Name = "someIndex",
                Table = "someTable"
            });
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("DROP INDEX IF EXISTS dto.\"someTable_someIndex\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void DropIndexOperationTableNameWithSchema()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new DropIndexOperation()
            {
                Name = "someIndex",
                Table = "someSchema.someTable"
            });
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("DROP INDEX IF EXISTS someSchema.\"someTable_someIndex\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void CreateIndexOperation()
        {
            var operations = new List<MigrationOperation>();
            var operation = new CreateIndexOperation();
            operation.Table = "someTable";
            operation.Name = "someIndex";
            operation.Columns.Add("column1");
            operation.Columns.Add("column2");
            operation.Columns.Add("column3");
            operation.IsUnique = false;
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("CREATE INDEX \"someTable_someIndex\" ON \"someTable\" (\"column1\",\"column2\",\"column3\")", statments.ElementAt(0).Sql);
        }

        [Test]
        public void CreateIndexOperationUnique()
        {
            var operations = new List<MigrationOperation>();
            var operation = new CreateIndexOperation();
            operation.Table = "someTable";
            operation.Name = "someIndex";
            operation.Columns.Add("column1");
            operation.Columns.Add("column2");
            operation.Columns.Add("column3");
            operation.IsUnique = true;
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("CREATE UNIQUE INDEX \"someTable_someIndex\" ON \"someTable\" (\"column1\",\"column2\",\"column3\")", statments.ElementAt(0).Sql);
        }

        [Test]
        public void RenameIndexOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new RenameIndexOperation("someSchema.someTable", "someOldIndexName", "someNewIndexName"));
            var statements = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statements.Count());
            if (_backendVersion.Major > 9 || (_backendVersion.Major == 9 && _backendVersion.Minor >= 2))
            {
                Assert.AreEqual("ALTER INDEX IF EXISTS someSchema.\"someOldIndexName\" RENAME TO \"someNewIndexName\"", statements.ElementAt(0).Sql);
            }
            else
            {
                Assert.AreEqual("ALTER INDEX someSchema.\"someOldIndexName\" RENAME TO \"someNewIndexName\"", statements.ElementAt(0).Sql);    
            }
        }

        [Test]
        public void MoveTableOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new MoveTableOperation("someOldSchema.someTable", "someNewSchema"));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(2, statments.Count());
            if (_backendVersion.Major > 9 || (_backendVersion.Major == 9 && _backendVersion.Minor > 2))
                Assert.AreEqual("CREATE SCHEMA IF NOT EXISTS someNewSchema", statments.ElementAt(0).Sql);
            else
                Assert.AreEqual("CREATE SCHEMA someNewSchema", statments.ElementAt(0).Sql);
            Assert.AreEqual("ALTER TABLE \"someOldSchema\".\"someTable\" SET SCHEMA someNewSchema", statments.ElementAt(1).Sql);
        }

        [Test]
        public void MoveTableOperationNewSchemaIsNull()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new MoveTableOperation("someOldSchema.someTable", null));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(2, statments.Count());
            if (_backendVersion.Major > 9 || (_backendVersion.Major == 9 && _backendVersion.Minor > 2))
                Assert.AreEqual("CREATE SCHEMA IF NOT EXISTS dbo", statments.ElementAt(0).Sql);
            else
                Assert.AreEqual("CREATE SCHEMA dbo", statments.ElementAt(0).Sql);
            Assert.AreEqual("ALTER TABLE \"someOldSchema\".\"someTable\" SET SCHEMA dbo", statments.ElementAt(1).Sql);
        }

        [Test]
        public void AddPrimaryKeyOperation()
        {
            var operations = new List<MigrationOperation>();
            var operation = new AddPrimaryKeyOperation();
            operation.Table = "someTable";
            operation.Name = "somePKName";
            operation.Columns.Add("column1");
            operation.Columns.Add("column2");
            operation.Columns.Add("column3");
            operation.IsClustered = false;
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD CONSTRAINT \"somePKName\" PRIMARY KEY (\"column1\",\"column2\",\"column3\")", statments.ElementAt(0).Sql);
        }

        [Test]
        public void AddPrimaryKeyOperationClustered()
        {
            var operations = new List<MigrationOperation>();
            var operation = new AddPrimaryKeyOperation();
            operation.Table = "someTable";
            operation.Name = "somePKName";
            operation.Columns.Add("column1");
            operation.Columns.Add("column2");
            operation.Columns.Add("column3");
            operation.IsClustered = true;
            //TODO: PostgreSQL support something like IsClustered?
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD CONSTRAINT \"somePKName\" PRIMARY KEY (\"column1\",\"column2\",\"column3\")", statments.ElementAt(0).Sql);
        }

        [Test]
        public void DropPrimaryKeyOperation()
        {
            var operations = new List<MigrationOperation>();
            var operation = new DropPrimaryKeyOperation();
            operation.Table = "someTable";
            operation.Name = "somePKName";
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someTable\" DROP CONSTRAINT \"somePKName\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void RenameColumnOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new RenameColumnOperation("someTable", "someOldColumnName", "someNewColumnName"));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someTable\" RENAME COLUMN \"someOldColumnName\" TO \"someNewColumnName\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void SqlOperation()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new SqlOperation("SELECT someColumn FROM someTable"));
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("SELECT someColumn FROM someTable", statments.ElementAt(0).Sql);
        }

        [Test]
        public void UpdateDatabaseOperation()
        {
            var operations = new List<MigrationOperation>();
            //TODO: fill operations
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            //TODO: check statments
        }

        [Test]
        public void AddForeignKeyOperation()
        {
            var operations = new List<MigrationOperation>();
            var operation = new AddForeignKeyOperation();
            operation.Name = "someFK";
            operation.PrincipalTable = "somePrincipalTable";
            operation.DependentTable = "someDependentTable";
            operation.DependentColumns.Add("column1");
            operation.DependentColumns.Add("column2");
            operation.DependentColumns.Add("column3");
            operation.CascadeDelete = false;
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someDependentTable\" ADD CONSTRAINT \"someFK\" FOREIGN KEY (\"column1\",\"column2\",\"column3\") REFERENCES \"somePrincipalTable\" )", statments.ElementAt(0).Sql);
        }

        [Test]
        public void AddForeignKeyOperationCascadeDelete()
        {
            var operations = new List<MigrationOperation>();
            var operation = new AddForeignKeyOperation();
            operation.Name = "someFK";
            operation.PrincipalTable = "somePrincipalTable";
            operation.DependentTable = "someDependentTable";
            operation.DependentColumns.Add("column1");
            operation.DependentColumns.Add("column2");
            operation.DependentColumns.Add("column3");
            operation.CascadeDelete = true;
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someDependentTable\" ADD CONSTRAINT \"someFK\" FOREIGN KEY (\"column1\",\"column2\",\"column3\") REFERENCES \"somePrincipalTable\" ) ON DELETE CASCADE", statments.ElementAt(0).Sql);
        }


        [Test]
        public void DropForeignKeyOperation()
        {
            var operations = new List<MigrationOperation>();
            var operation = new DropForeignKeyOperation();
            operation.Name = "someFK";
            operation.DependentTable = "someTable";
            operations.Add(operation);
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(1, statments.Count());
            if (_backendVersion.Major > 8)
                Assert.AreEqual("ALTER TABLE \"someTable\" DROP CONSTRAINT IF EXISTS \"someFK\"", statments.ElementAt(0).Sql);
            else
                Assert.AreEqual("ALTER TABLE \"someTable\" DROP CONSTRAINT \"someFK\"", statments.ElementAt(0).Sql);
        }

        [Test]
        public void DefaultTypes()
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Binary)
                {
                    Name = "someByteaColumn",
                    DefaultValue = new byte[6] { 1, 2, 127, 128, 254, 255 }
                }, false)
            );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Boolean)
                    {
                        Name = "someFalseBooleanColumn",
                        DefaultValue = false
                    }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Boolean)
                {
                    Name = "someTrueBooleanColumn",
                    DefaultValue = true
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Byte)
                {
                    Name = "someByteColumn",
                    DefaultValue = 15
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.DateTime)
                {
                    Name = "someDateTimeColumn",
                    DefaultValue = new DateTime(2014, 1, 31, 5, 15, 23, 435)
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.DateTimeOffset)
                {
                    Name = "someDateTimeOffsetColumn",
                    DefaultValue = new DateTimeOffset(new DateTime(2014, 1, 31, 5, 18, 43, 186), TimeSpan.FromHours(1))
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Decimal)
                {
                    Name = "someDecimalColumn",
                    DefaultValue = 23432423.534534m
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Double)
                {
                    Name = "someDoubleColumn",
                    DefaultValue = 44.66
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Guid)
                {
                    Name = "someGuidColumn",
                    DefaultValue = new Guid("de303070-afb8-4ec1-bcb0-c637f3316501")
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Int16)
                {
                    Name = "someInt16Column",
                    DefaultValue = 16
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Int32)
                {
                    Name = "someInt32Column",
                    DefaultValue = 32
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Int64)
                {
                    Name = "someInt64Column",
                    DefaultValue = 64
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.SByte)
                {
                    Name = "someSByteColumn",
                    DefaultValue = -24
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Single)
                {
                    Name = "someSingleColumn",
                    DefaultValue = 12.4f
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.String)
                {
                    Name = "someStringColumn",
                    DefaultValue = "Hello EF"
                }, false)
                );
            operations.Add(new AddColumnOperation("someTable",
                new ColumnModel(PrimitiveTypeKind.Time)
                {
                    Name = "someColumn",
                    DefaultValue = new TimeSpan(937840050067)//1 day, 2 hours, 3 minutes, 4 seconds, 5 miliseconds, 6 microseconds, 700 nanoseconds
                }, false)
                );
            var statments = new NpgsqlMigrationSqlGenerator().Generate(operations, _backendVersion.ToString());
            Assert.AreEqual(16, statments.Count());
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someByteaColumn\" bytea DEFAULT E'\\\\01027F80FEFF'", statments.ElementAt(0).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someFalseBooleanColumn\" boolean DEFAULT FALSE", statments.ElementAt(1).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someTrueBooleanColumn\" boolean DEFAULT TRUE", statments.ElementAt(2).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someByteColumn\" int2 DEFAULT 15", statments.ElementAt(3).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someDateTimeColumn\" timestamp DEFAULT '2014-01-31 05:15:23.4350000'", statments.ElementAt(4).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someDateTimeOffsetColumn\" timestamptz DEFAULT '2014-01-31 04:18:43.1860000'", statments.ElementAt(5).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someDecimalColumn\" numeric DEFAULT 23432423.534534", statments.ElementAt(6).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someDoubleColumn\" float8 DEFAULT 44.66", statments.ElementAt(7).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someGuidColumn\" uuid DEFAULT 'de303070-afb8-4ec1-bcb0-c637f3316501'", statments.ElementAt(8).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someInt16Column\" int2 DEFAULT 16", statments.ElementAt(9).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someInt32Column\" int4 DEFAULT 32", statments.ElementAt(10).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someInt64Column\" int8 DEFAULT 64", statments.ElementAt(11).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someSByteColumn\" int2 DEFAULT -24", statments.ElementAt(12).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someSingleColumn\" float4 DEFAULT 12.4", statments.ElementAt(13).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someStringColumn\" text DEFAULT 'Hello EF'", statments.ElementAt(14).Sql);
            Assert.AreEqual("ALTER TABLE \"someTable\" ADD \"someColumn\" interval DEFAULT '1 day 02:03:04.005007'", statments.ElementAt(15).Sql);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var conn = OpenConnection(ConnectionStringEF))
                _backendVersion = conn.PostgreSqlVersion;
        }

        Version _backendVersion;
    }
}
