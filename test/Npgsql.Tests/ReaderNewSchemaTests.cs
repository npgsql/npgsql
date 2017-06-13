#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    /// <summary>
    /// This tests the new CoreCLR schema/metadata API, which returns ReadOnlyCollection&lt;DbColumn&gt;.
    /// Note that this API is also available on .NET Framework.
    /// For the old DataTable-based API, see <see cref="ReaderOldSchemaTests"/>.
    /// </summary>
    public class ReaderNewSchemaTests : TestBase
    {
        // ReSharper disable once InconsistentNaming
        [Test]
        public void AllowDBNull()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (nullable INTEGER, non_nullable INTEGER NOT NULL)");

                using (var cmd = new NpgsqlCommand("SELECT nullable,non_nullable,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].AllowDBNull, Is.True);
                    Assert.That(columns[1].AllowDBNull, Is.False);
                    Assert.That(columns[2].AllowDBNull, Is.Null);
                }
            }
        }

        [Test]
        public void BaseCatalogName()
        {
            var dbName = new NpgsqlConnectionStringBuilder(ConnectionString).Database;
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseCatalogName, Is.EqualTo(dbName));
                    Assert.That(columns[1].BaseCatalogName, Is.EqualTo(dbName));
                }
            }
        }

        [Test]
        public void BaseColumnName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 AS bar,8,'8'::VARCHAR(10) FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[1].BaseColumnName, Is.EqualTo("bar"));
                    Assert.That(columns[2].BaseColumnName, Is.Null);
                    Assert.That(columns[3].BaseColumnName, Is.EqualTo("varchar"));
                }
            }
        }

        [Test]
        public void BaseSchemaName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseSchemaName, Does.StartWith("pg_temp"));
                    Assert.That(columns[1].BaseSchemaName, Is.Null);
                }
            }
        }

        [Test]
        public void BaseServerName()
        {
            var host = new NpgsqlConnectionStringBuilder(ConnectionString).Host;
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseServerName, Is.EqualTo(host));
                    Assert.That(columns[1].BaseServerName, Is.EqualTo(host));
                }
            }
        }

        [Test]
        public void BaseTableName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseTableName, Is.EqualTo("data"));
                    Assert.That(columns[1].BaseTableName, Is.Null);
                }
            }
        }

        [Test]
        public void ColumnName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 AS bar,8,'8'::VARCHAR(10) FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("bar"));
                    Assert.That(columns[2].ColumnName, Is.Null);
                    Assert.That(columns[3].ColumnName, Is.EqualTo("varchar"));
                }
            }
        }

        [Test]
        public void ColumnOrdinal()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (first INTEGER, second INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT second,first FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("second"));
                    Assert.That(columns[0].ColumnOrdinal, Is.EqualTo(0));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("first"));
                    Assert.That(columns[1].ColumnOrdinal, Is.EqualTo(1));
                }
            }
        }

        [Test]
        public void ColumnAttributeNumber()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (first INTEGER, second INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT second,first FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("second"));
                    Assert.That(columns[0].ColumnAttributeNumber, Is.EqualTo(1));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("first"));
                    Assert.That(columns[1].ColumnAttributeNumber, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void ColumnSize()
        {
            if (IsRedshift)
                Assert.Ignore("Column size is never unlimited on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (bounded VARCHAR(30), unbounded VARCHAR)");

                using (var cmd = new NpgsqlCommand("SELECT bounded,unbounded,'a'::VARCHAR(10),'b'::VARCHAR FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnSize, Is.EqualTo(30));
                    Assert.That(columns[1].ColumnSize, Is.Null);
                    Assert.That(columns[2].ColumnSize, Is.EqualTo(10));
                    Assert.That(columns[3].ColumnSize, Is.Null);
                }
            }
        }

        [Test]
        public void IsAutoIncrement()
        {
            if (IsRedshift)
                Assert.Ignore("Serial columns not support on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (inc SERIAL, non_inc INT)");

                using (var cmd = new NpgsqlCommand("SELECT inc,non_inc,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsAutoIncrement, Is.True);
                    Assert.That(columns[1].IsAutoIncrement, Is.False);
                    Assert.That(columns[2].IsAutoIncrement, Is.Null);
                }
            }
        }

        [Test]
        public void IsKey()
        {
            if (IsRedshift)
                Assert.Ignore("Key not supported in reader schema on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id INT PRIMARY KEY, non_id INT, uniq INT UNIQUE)");

                using (var cmd = new NpgsqlCommand("SELECT id,non_id,uniq,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsKey, Is.True);
                    Assert.That(columns[1].IsKey, Is.False);

                    // Note: according to the old API docs any unique column is considered key.
                    // https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
                    // But in the new API we have a separate IsUnique so IsKey should be false
                    Assert.That(columns[2].IsKey, Is.False);

                    Assert.That(columns[3].IsKey, Is.Null);
                }
            }
        }

        [Test]
        public void IsKeyComposite()
        {
            if (IsRedshift)
                Assert.Ignore("Key not supported in reader schema on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id1 INT, id2 INT, PRIMARY KEY (id1, id2))");

                using (var cmd = new NpgsqlCommand("SELECT id1,id2 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsKey, Is.True);
                    Assert.That(columns[1].IsKey, Is.True);
                }
            }
        }

        [Test]
        public void IsLong()
        {
            if (IsRedshift)
                Assert.Ignore("bytea not supported on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (long BYTEA, non_long INT)");

                using (var cmd = new NpgsqlCommand("SELECT long, non_long, 8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsLong, Is.True);
                    Assert.That(columns[1].IsLong, Is.False);
                    Assert.That(columns[2].IsLong, Is.False);
                }
            }
        }

        [Test]
        public void IsReadOnlyOnView()
        {
            using (var conn = OpenConnection())
            {
                // We can't use temporary tables because GetColumnSchema opens another connection
                // internally, which doesn't have our temporary schema
                conn.ExecuteNonQuery("DROP VIEW IF EXISTS view; DROP TABLE IF EXISTS data");
                conn.ExecuteNonQuery("CREATE VIEW view AS SELECT 8 AS foo; CREATE TABLE data (bar INTEGER)");

                try
                {
                    using (var cmd = new NpgsqlCommand("SELECT foo,bar FROM view,data", conn))
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                    {
                        var columns = reader.GetColumnSchema();
                        Assert.That(columns[0].IsReadOnly, Is.True);
                        Assert.That(columns[1].IsReadOnly, Is.False);
                    }
                }
                finally
                {
                    conn.ExecuteNonQuery("DROP VIEW IF EXISTS view; DROP TABLE IF EXISTS data");
                }
            }
        }

        [Test]
        public void IsReadOnlyOnNonColumn()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 8", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsReadOnly, Is.True);
            }
        }

        [Test]
        public void IsUnique()
        {
            if (IsRedshift)
                Assert.Ignore("Unique not supported in reader schema on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id INT PRIMARY KEY, non_id INT, uniq INT UNIQUE)");

                using (var cmd = new NpgsqlCommand("SELECT id,non_id,uniq,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsUnique, Is.True);
                    Assert.That(columns[1].IsUnique, Is.False);
                    Assert.That(columns[2].IsUnique, Is.True);
                    Assert.That(columns[3].IsUnique, Is.Null);
                }
            }
        }

        [Test]
        public void NumericPrecision()
        {
            if (IsRedshift)
                Assert.Ignore("Precision is never unlimited on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (a NUMERIC(8), b NUMERIC, c INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT a,b,c,8.3::NUMERIC(8) FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].NumericPrecision, Is.EqualTo(8));
                    Assert.That(columns[1].NumericPrecision, Is.Null);
                    Assert.That(columns[2].NumericPrecision, Is.Null);
                    Assert.That(columns[3].NumericPrecision, Is.EqualTo(8));
                }
            }
        }

        [Test]
        public void NumericScale()
        {
            if (IsRedshift)
                Assert.Ignore("Scale is never unlimited on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (a NUMERIC(8,5), b NUMERIC, c INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT a,b,c,8.3::NUMERIC(8,5) FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].NumericScale, Is.EqualTo(5));
                    Assert.That(columns[1].NumericScale, Is.Null);
                    Assert.That(columns[2].NumericScale, Is.Null);
                    Assert.That(columns[3].NumericScale, Is.EqualTo(5));
                }
            }
        }

        [Test]
        public void DataType()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8::INTEGER FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DataType, Is.SameAs(typeof(int)));
                    Assert.That(columns[1].DataType, Is.SameAs(typeof(int)));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1305")]
        public void DataTypeUnknownType()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo::INTEGER FROM data", conn))
                {
                    cmd.AllResultTypesAreUnknown = true;
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        var columns = reader.GetColumnSchema();
                        Assert.That(columns[0].DataType, Is.SameAs(typeof(int)));
                    }
                }
            }
        }

        [Test]
        public void DataTypeWithComposite()
        {
            if (IsRedshift)
                Assert.Ignore("Composite types not support on Redshift");
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(DataTypeWithComposite),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.some_composite AS (foo int)");
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>();
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (comp pg_temp.some_composite)");

                using (var cmd = new NpgsqlCommand("SELECT comp,'(4)'::some_composite FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DataType, Is.SameAs(typeof(SomeComposite)));
                    Assert.That(columns[0].UdtAssemblyQualifiedName, Is.EqualTo(typeof(SomeComposite).AssemblyQualifiedName));
                    Assert.That(columns[1].DataType, Is.SameAs(typeof(SomeComposite)));
                    Assert.That(columns[1].UdtAssemblyQualifiedName, Is.EqualTo(typeof(SomeComposite).AssemblyQualifiedName));
                }
            }
        }

        [Test]
        public void UdtAssemblyQualifiedName()
        {
            // Also see DataTypeWithComposite
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].UdtAssemblyQualifiedName, Is.Null);
                    Assert.That(columns[1].UdtAssemblyQualifiedName, Is.Null);
                }
            }
        }

        [Test]
        public void PostgresType()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    var intType = columns[0].PostgresType;
                    Assert.That(columns[1].PostgresType, Is.SameAs(intType));
                    Assert.That(intType.Name, Is.EqualTo("int4"));
                }
            }
        }

        [Test]
        public void DataTypeName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT foo,8::INTEGER FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DataTypeName, Is.EqualTo("int4"));
                    Assert.That(columns[1].DataTypeName, Is.EqualTo("int4"));
                }
            }
        }

        [Test]
        public void DefaultValue()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (with_default INTEGER DEFAULT(8), without_default INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT with_default,without_default,8 FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DefaultValue, Is.EqualTo("8"));
                    Assert.That(columns[1].DefaultValue, Is.Null);
                    Assert.That(columns[2].DefaultValue, Is.Null);
                }
            }
        }

        [Test]
        public void SameColumnName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data1 (foo INTEGER); CREATE TEMP TABLE data2 (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT data1.foo,data2.foo FROM data1,data2", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[0].BaseTableName, Is.EqualTo("data1"));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[1].BaseTableName, Is.EqualTo("data2"));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1553")]
        public void DomainTypes()
        {
            if (IsRedshift)
                Assert.Ignore("Domain types not support on Redshift");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP DOMAIN IF EXISTS mydomain; CREATE DOMAIN mydomain AS varchar(2)");
                try
                {
                    conn.ReloadTypes();
                    conn.ExecuteNonQuery("CREATE TEMP TABLE data (domain mydomain)");
                    using (var cmd = new NpgsqlCommand("SELECT domain FROM data", conn))
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                    {
                        var domainSchema = reader.GetColumnSchema().Single(c => c.ColumnName == "domain");
                        Assert.That(domainSchema.ColumnSize, Is.EqualTo(2));
                        var pgType = domainSchema.PostgresType;
                        Assert.That(pgType, Is.InstanceOf<PostgresDomainType>());
                        Assert.That(((PostgresDomainType)pgType).BaseType.Name, Is.EqualTo("varchar"));
                    }
                }
                finally
                {
                    conn.ExecuteNonQuery("DROP TABLE data; DROP DOMAIN mydomain");
                }
            }
        }

        #region Not supported

        [Test]
        public void IsAliased()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsAliased, Is.False);
            }
        }

        [Test]
        public void IsExpression()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsExpression, Is.False);
            }
        }

        [Test]
        public void IsHidden()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsHidden, Is.False);
            }
        }

        [Test]
        public void IsIdentity()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INTEGER)");

                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsIdentity, Is.False);
            }
        }

        #endregion

        class SomeComposite
        {
            public int Foo { get; set; }
        }
    }
}
