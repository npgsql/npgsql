using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.NumericHandlers;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [NonParallelizable]
    public class TypeMapperTests : TestBase
    {
        [Test]
        public void GlobalMapping()
        {
            var myFactory = MapMyIntGlobally();
            using (var conn = OpenLocalConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var range = new NpgsqlRange<int>(8, true, false, 0, false, true);
                var parameters = new[]
                {
                    // Base
                    new NpgsqlParameter("p", NpgsqlDbType.Integer) { Value = 8 },
                    new NpgsqlParameter("p", DbType.Int32) { Value = 8 },
                    new NpgsqlParameter { ParameterName = "p", Value = 8 },
                    // Array
                    new NpgsqlParameter { ParameterName = "p", Value = new[] { 8 } },
                    new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = new[] { 8 } },
                    // Range
                    new NpgsqlParameter { ParameterName = "p", Value = range },
                    new NpgsqlParameter("p", NpgsqlDbType.Range | NpgsqlDbType.Integer) { Value = range },
                };

                for (var i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                    cmd.ExecuteScalar();
                    Assert.That(myFactory.Reads, Is.EqualTo(i+1));
                    Assert.That(myFactory.Writes, Is.EqualTo(i+1));
                    cmd.Parameters.Clear();
                }
            }
        }

        [Test]
        public void LocalMapping()
        {
            MyInt32HandlerFactory myFactory;
            using (var conn = OpenLocalConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                myFactory = MapMyIntLocally(conn);
                cmd.Parameters.AddWithValue("p", 8);
                cmd.ExecuteScalar();
                Assert.That(myFactory.Reads, Is.EqualTo(1));
                Assert.That(myFactory.Writes, Is.EqualTo(1));
            }
            // Make sure reopening (same physical connection) reverts the mapping
            using (var conn = OpenLocalConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", 8);
                cmd.ExecuteScalar();
                Assert.That(myFactory.Reads, Is.EqualTo(1));
                Assert.That(myFactory.Writes, Is.EqualTo(1));
            }
        }

        [Test]
        public void RemoveGlobalMapping()
        {
            NpgsqlConnection.GlobalTypeMapper.RemoveMapping("integer");
            using (var conn = OpenLocalConnection())
                Assert.That(() => conn.ExecuteScalar("SELECT 8"), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void RemoveLocalMapping()
        {
            using (var conn = OpenLocalConnection())
            {
                conn.TypeMapper.RemoveMapping("integer");
                Assert.That(() => conn.ExecuteScalar("SELECT 8"), Throws.TypeOf<NotSupportedException>());
            }
            // Make sure reopening (same physical connection) reverts the mapping
            using (var conn = OpenLocalConnection())
                Assert.That(conn.ExecuteScalar("SELECT 8"), Is.EqualTo(8));
        }

        [Test]
        public void GlobalReset()
        {
            var myFactory = MapMyIntGlobally();
            using (OpenLocalConnection()) {}
            // We now have a connector in the pool with our custom mapping

            NpgsqlConnection.GlobalTypeMapper.Reset();
            using (var conn = OpenLocalConnection())
            {
                // Should be the pooled connector from before, but it should have picked up the reset
                conn.ExecuteScalar("SELECT 1");
                Assert.That(myFactory.Reads, Is.Zero);

                // Now create a second *physical* connection to make sure it picks up the new mapping as well
                using (var conn2 = OpenLocalConnection())
                {
                    conn2.ExecuteScalar("SELECT 1");
                    Assert.That(myFactory.Reads, Is.Zero);
                }
                NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test]
        public void DomainMappingNotSupported()
        {
             // PostgreSQL sends RowDescription with the OID of the base type, not the domain,
             // it's not possible to map domains
            using (var conn = OpenLocalConnection())
            {
                conn.ExecuteNonQuery(@"CREATE DOMAIN pg_temp.us_postal_code AS TEXT
CHECK
(
    VALUE ~ '^\d{5}$'
    OR VALUE ~ '^\d{5}-\d{4}$'
);
");
                conn.ReloadTypes();
                Assert.That(() => conn.TypeMapper.AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "us_postal_code",
                    TypeHandlerFactory = new DummyTypeHandlerFactory()
                }.Build()), Throws.TypeOf<NotSupportedException>());
            }
        }

        class DummyTypeHandlerFactory : NpgsqlTypeHandlerFactory<int>
        {
            protected override NpgsqlTypeHandler<int> Create(NpgsqlConnection conn)
                => throw new Exception();
        }

        [Test]
        public void MandatoryMappingFields()
        {
            Assert.That(() => new NpgsqlTypeMappingBuilder().Build(), Throws.ArgumentException);
            Assert.That(() => new NpgsqlTypeMappingBuilder{ PgTypeName = "foo" }.Build(), Throws.ArgumentException);
        }

        [Test]
        public void StringToCitext()
        {
            using (var conn = OpenLocalConnection())
            {
                conn.TypeMapper.RemoveMapping("text");
                conn.TypeMapper.AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "citext",
                    NpgsqlDbType = NpgsqlDbType.Citext,
                    DbTypes = new[] { DbType.String },
                    ClrTypes = new[] { typeof(string) },
                    TypeHandlerFactory = new TextHandlerFactory()
                }.Build());

                using (var cmd = new NpgsqlCommand("SELECT @p = 'hello'::citext", conn))
                {
                    cmd.Parameters.AddWithValue("p", "HeLLo");
                    Assert.That(cmd.ExecuteScalar(), Is.True);
                }
            }
        }

        #region Support

        MyInt32HandlerFactory MapMyIntGlobally()
        {
            var myFactory = new MyInt32HandlerFactory();
            NpgsqlConnection.GlobalTypeMapper.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "integer",
                NpgsqlDbType = NpgsqlDbType.Integer,
                DbTypes = new[] { DbType.Int32 },
                ClrTypes = new[] { typeof(int) },
                TypeHandlerFactory = myFactory
            }.Build());
            return myFactory;
        }

        MyInt32HandlerFactory MapMyIntLocally(NpgsqlConnection conn)
        {
            var myFactory = new MyInt32HandlerFactory();
            conn.TypeMapper.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "integer",
                NpgsqlDbType = NpgsqlDbType.Integer,
                DbTypes = new[] { DbType.Int32 },
                ClrTypes = new[] { typeof(int) },
                TypeHandlerFactory = myFactory
            }.Build());
            return myFactory;
        }

        class MyInt32HandlerFactory : NpgsqlTypeHandlerFactory<int>
        {
            internal int Reads, Writes;

            protected override NpgsqlTypeHandler<int> Create(NpgsqlConnection conn)
                => new MyInt32Handler(this);
        }

        class MyInt32Handler : Int32Handler
        {
            readonly MyInt32HandlerFactory _factory;

            public MyInt32Handler(MyInt32HandlerFactory factory)
            {
                _factory = factory;
            }

            public override int Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            {
                _factory.Reads++;
                return base.Read(buf, len, fieldDescription);
            }

            public override void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            {
                _factory.Writes++;
                base.Write(value, buf, parameter);
            }
        }

        NpgsqlConnection OpenLocalConnection() => OpenConnection(LocalConnString);

        static readonly string LocalConnString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(TypeMapperTests)
        }.ToString();

        #endregion Support

        [TearDown]
        public void TearDown()
        {
            NpgsqlConnection.GlobalTypeMapper.Reset();
            using (var conn = new NpgsqlConnection(LocalConnString))
                NpgsqlConnection.ClearPool(conn);
        }
    }
}
