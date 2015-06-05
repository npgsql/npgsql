using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace Npgsql
{
    class NpgsqlPreparedTransactionRecordStore
    {
        private const string ConnectionStringPrefix = "ConnectionString";
        private const string RecoveryInformationPrefix = "RecoveryInformation";

        private readonly IsolatedStorageFile _store;
        private readonly object _lock = new object();

        public NpgsqlPreparedTransactionRecordStore()
        {
            _store = IsolatedStorageFile.GetUserStoreForDomain();
        }

        public Guid GetConnectionGuid(string connectionString)
        {
            lock (_lock)
            {
                var directory = GetDirectoryForConnection(connectionString);

                var rmGuidPath = Path.Combine(directory, "rmGuid");

                try
                {
                    if (_store.GetFileNames(rmGuidPath).Length > 0) // if file exists
                    {
                        using (var stream = OpenForRead(rmGuidPath))
                        using (var streamReader = new StreamReader(stream))
                        {
                            string rmGuidStr = streamReader.ReadLine();
                            return new Guid(rmGuidStr);
                        }
                    }
                }
                catch
                {
                }

                Guid rmGuid = Guid.NewGuid();
                using (var stream = OpenForWrite(rmGuidPath))
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine(rmGuid.ToString());
                }

                return rmGuid;
            }
        }

        public void AddRecord(NpgsqlPreparedTransactionRecord record)
        {
            lock (_lock)
            {
                using (var stream = OpenForWrite(GetTransactionPath(record.ConnectionString, record.TxName)))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("{0}={1}", ConnectionStringPrefix, record.ConnectionString);
                    writer.WriteLine("{0}={1}", RecoveryInformationPrefix,
                        Convert.ToBase64String(record.RecoveryInformation));
                }
            }
        }

        public void DeleteRecord(string txName, string connectionString)
        {
            lock (_lock)
            {
                _store.DeleteFile(GetTransactionPath(connectionString, txName));
            }
        }

        public IEnumerable<NpgsqlPreparedTransactionRecord> GetAllRecords(string connectionString)
        {
            lock (_lock)
            {
                var directory = GetTransactionDirectory(connectionString);

                foreach (var fileName in _store.GetFileNames(string.Format(Path.Combine(directory, "*"))))
                {
                    var record = ReadRecordFile(directory, fileName);
                    if (record != null)
                        yield return record;
                }
            }
        }

        private IsolatedStorageFileStream OpenForWrite(string path)
        {
            return new IsolatedStorageFileStream(path, FileMode.CreateNew, FileAccess.Write, _store);
        }

        private IsolatedStorageFileStream OpenForRead(string path)
        {
            return new IsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, _store);
        }

        private NpgsqlPreparedTransactionRecord ReadRecordFile(string directory, string fileName)
        {
            try
            {
                using (var stream = OpenForRead(Path.Combine(directory, fileName)))
                using (var reader = new StreamReader(stream))
                {
                    string connectionString = null;
                    byte[] recoveryInformation = null;

                    for (int i = 0; i < 2; ++i)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            break;

                        if (line.StartsWith(ConnectionStringPrefix))
                        {
                            connectionString = line.Substring(ConnectionStringPrefix.Length + 1);
                        }
                        else if (line.StartsWith(RecoveryInformationPrefix))
                        {
                            recoveryInformation = Convert.FromBase64String(line.Substring(RecoveryInformationPrefix.Length + 1));
                        }
                    }

                    if (recoveryInformation == null || connectionString == null)
                        return null;

                    return new NpgsqlPreparedTransactionRecord(fileName, connectionString, recoveryInformation);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetDirectoryForConnection(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var rootDirectory = string.Format("{0}-{1}", builder.Host, builder.Port);
            _store.CreateDirectory(rootDirectory);

            var databaseDirectory = Path.Combine(rootDirectory, builder.Database);
            _store.CreateDirectory(databaseDirectory);

            var userDirectory = Path.Combine(databaseDirectory, builder.UserName);
            _store.CreateDirectory(userDirectory);

            return userDirectory;
        }

        private string GetTransactionPath(string connectionString, string txName)
        {
            return Path.Combine(GetTransactionDirectory(connectionString), txName);
        }

        private string GetTransactionDirectory(string connectionString)
        {
            var transactionDirectory = Path.Combine(GetDirectoryForConnection(connectionString), "Transactions");
            _store.CreateDirectory(transactionDirectory);
            return transactionDirectory;
        }
    }
}
