using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests;

public static class Utility
{
    public static void ExecuteNonQuery(IDbFactoryFixture factoryFixture, string sql)
    {
        using (var connection = factoryFixture.Factory.CreateConnection()!)
        {
            connection.ConnectionString = factoryFixture.ConnectionString;
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }
    }
}