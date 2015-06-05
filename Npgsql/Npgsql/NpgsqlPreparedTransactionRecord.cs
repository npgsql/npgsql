using System;

namespace Npgsql
{
    class NpgsqlPreparedTransactionRecord
    {
        public NpgsqlPreparedTransactionRecord(string txName, string connectionString, byte[] recoveryInformation)
        {
            TxName = txName;
            ConnectionString = connectionString;
            RecoveryInformation = recoveryInformation;
        }

        public string TxName { get; private set; }
        public string ConnectionString { get; private set; }
        public byte[] RecoveryInformation { get; private set; }
    }
}