using Microsoft.Data.Entity.Relational;

namespace EntityFramework.Npgsql
{
    public interface INpgsqlConnection : IRelationalConnection
    {
        INpgsqlConnection CreateMasterConnection();
    }
}