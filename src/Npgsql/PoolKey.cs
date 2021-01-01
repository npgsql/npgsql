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

        public override bool Equals(object? obj)
        {
            return obj is PoolKey key && _connectionString == key._connectionString;
        }

        public static bool operator ==(PoolKey key1, PoolKey key2)
        {
            if (object.ReferenceEquals(key1, null))
            {
                return object.ReferenceEquals(key2, null);
            }

            return key1.Equals(key2);
        }

        public static bool operator !=(PoolKey key1, PoolKey key2)
        {
            return (key1 == key2) == false;
        }

        public override int GetHashCode()
        {
            return -1879859525 + EqualityComparer<string>.Default.GetHashCode(_connectionString);
        }
    }
}
