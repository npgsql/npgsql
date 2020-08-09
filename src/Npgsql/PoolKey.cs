using System.Collections.Generic;

namespace Npgsql
{
    internal class PoolKey
    {
        internal string _connectionString;
        public PoolKey(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override bool Equals(object obj)
        {
            return obj is PoolKey key && _connectionString == key._connectionString;
        }

        public override int GetHashCode()
        {
            return -1879859525 + EqualityComparer<string>.Default.GetHashCode(_connectionString);
        }
    }
}
