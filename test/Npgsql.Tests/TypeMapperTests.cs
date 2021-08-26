using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandlers.NumericHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests
{
    [NonParallelizable]
    public class TypeMapperTests : TestBase
    {
        [Test]
        public void GlobalMapping()
        {
            var myFactory = new MyInt32TypeHandlerResolverFactory();
            NpgsqlConnection.GlobalTypeMapper.AddTypeResolverFactory(myFactory);

            using var pool = CreateTempPool(ConnectionString, out var connectionString);
            using var conn = OpenConnection(connectionString);
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
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

         [Test]
         public void LocalMapping()
         {
             var myFactory = new MyInt32TypeHandlerResolverFactory();
             using var _ = CreateTempPool(ConnectionString, out var connectionString);

             using (var conn = OpenConnection(connectionString))
             using (var cmd = new NpgsqlCommand("SELECT @p", conn))
             {
                 conn.TypeMapper.AddTypeResolverFactory(myFactory);
                 cmd.Parameters.AddWithValue("p", 8);
                 cmd.ExecuteScalar();
                 Assert.That(myFactory.Reads, Is.EqualTo(1));
                 Assert.That(myFactory.Writes, Is.EqualTo(1));
             }

             // Make sure reopening (same physical connection) reverts the mapping
             using (var conn = OpenConnection(connectionString))
             using (var cmd = new NpgsqlCommand("SELECT @p", conn))
             {
                 cmd.Parameters.AddWithValue("p", 8);
                 cmd.ExecuteScalar();
                 Assert.That(myFactory.Reads, Is.EqualTo(1));
                 Assert.That(myFactory.Writes, Is.EqualTo(1));
             }
         }

         [Test]
         public void GlobalReset()
         {
             var myFactory = new MyInt32TypeHandlerResolverFactory();
             NpgsqlConnection.GlobalTypeMapper.AddTypeResolverFactory(myFactory);
             using var _ = CreateTempPool(ConnectionString, out var connectionString);

             using (OpenConnection(connectionString)) {}
             // We now have a connector in the pool with our custom mapping

             NpgsqlConnection.GlobalTypeMapper.Reset();
             using (var conn = OpenConnection(connectionString))
             {
                 // Should be the pooled connector from before, but it should have picked up the reset
                 conn.ExecuteScalar("SELECT 1");
                 Assert.That(myFactory.Reads, Is.Zero);

                 // Now create a second *physical* connection to make sure it picks up the new mapping as well
                 using (var conn2 = OpenConnection(connectionString))
                 {
                     conn2.ExecuteScalar("SELECT 1");
                     Assert.That(myFactory.Reads, Is.Zero);
                 }
                 NpgsqlConnection.ClearPool(conn);
             }
         }

        [Test]
        public async Task StringToCitext()
        {
            using (CreateTempPool(ConnectionString, out var connectionString))
            using (var conn = OpenConnection(connectionString))
            {
                await EnsureExtensionAsync(conn, "citext");

                conn.TypeMapper.AddTypeResolverFactory(new CitextToStringTypeHandlerResolverFactory());

                using (var cmd = new NpgsqlCommand("SELECT @p = 'hello'::citext", conn))
                {
                    cmd.Parameters.AddWithValue("p", "HeLLo");
                    Assert.That(cmd.ExecuteScalar(), Is.True);
                }
            }
        }

        #region Support

        class MyInt32TypeHandlerResolverFactory : TypeHandlerResolverFactory
        {
            internal int Reads, Writes;

            public override TypeHandlerResolver Create(NpgsqlConnector connector)
                => new MyInt32TypeHandlerResolver(connector, this);

            public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName) => throw new NotSupportedException();
            public override string? GetDataTypeNameByClrType(Type clrType) => throw new NotSupportedException();
            public override string? GetDataTypeNameByValueDependentValue(object value) => throw new NotSupportedException();
        }

        class MyInt32TypeHandlerResolver : TypeHandlerResolver
        {
            readonly NpgsqlTypeHandler _handler;

            public MyInt32TypeHandlerResolver(NpgsqlConnector connector, MyInt32TypeHandlerResolverFactory factory)
                => _handler = new MyInt32Handler(connector.DatabaseInfo.GetPostgresTypeByName("integer"), factory);

            public override NpgsqlTypeHandler? ResolveByClrType(Type type)
                => type == typeof(int) ? _handler : null;
            public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
                => typeName == "integer" ? _handler : null;

            public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName) => throw new NotSupportedException();

        }

        class MyInt32Handler : Int32Handler
        {
            readonly MyInt32TypeHandlerResolverFactory _factory;

            public MyInt32Handler(PostgresType postgresType, MyInt32TypeHandlerResolverFactory factory)
                : base(postgresType)
                => _factory = factory;

            public override int Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            {
                _factory.Reads++;
                return base.Read(buf, len, fieldDescription);
            }

            public override void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            {
                _factory.Writes++;
                base.Write(value, buf, parameter);
            }
        }

        class CitextToStringTypeHandlerResolverFactory : TypeHandlerResolverFactory
        {
            public override TypeHandlerResolver Create(NpgsqlConnector connector)
                => new CitextToStringTypeHandlerResolver(connector);

            public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName) => throw new NotSupportedException();
            public override string? GetDataTypeNameByClrType(Type clrType) => throw new NotSupportedException();
            public override string? GetDataTypeNameByValueDependentValue(object value) => throw new NotSupportedException();

            class CitextToStringTypeHandlerResolver : TypeHandlerResolver
            {
                readonly NpgsqlConnector _connector;
                readonly PostgresType _pgCitextType;

                public CitextToStringTypeHandlerResolver(NpgsqlConnector connector)
                {
                    _connector = connector;
                    _pgCitextType = connector.DatabaseInfo.GetPostgresTypeByName("citext");
                }

                public override NpgsqlTypeHandler? ResolveByClrType(Type type)
                    => type == typeof(string) ? new TextHandler(_pgCitextType, _connector.TextEncoding) : null;
                public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName) => null;

                public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName) => throw new NotSupportedException();
            }
        }

        #endregion Support

        [TearDown]
        public void TearDown() => NpgsqlConnection.GlobalTypeMapper.Reset();
    }
}
