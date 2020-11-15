using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using Npgsql.Util;

namespace Npgsql
{
    /// <summary>
    /// Base class for implementations which provide information about PostgreSQL and PostgreSQL-like databases
    /// (e.g. type definitions, capabilities...).
    /// </summary>
    public abstract class NpgsqlDatabaseInfo
    {
        #region Fields

        internal static readonly ConcurrentDictionary<NpgsqlDatabaseInfoCacheKey, NpgsqlDatabaseInfo> Cache
            = new ConcurrentDictionary<NpgsqlDatabaseInfoCacheKey, NpgsqlDatabaseInfo>();

        static volatile INpgsqlDatabaseInfoFactory[] Factories = new INpgsqlDatabaseInfoFactory[]
        {
            new PostgresMinimalDatabaseInfoFactory(),
            new PostgresDatabaseInfoFactory()
        };

        #endregion Fields

        #region General database info

        /// <summary>
        /// The hostname of IP address of the database.
        /// </summary>
        public string Host { get; }
        /// <summary>
        /// The TCP port of the database.
        /// </summary>
        public int Port { get; }
        /// <summary>
        /// The database name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The version of the PostgreSQL database we're connected to, as reported in the "server_version" parameter.
        /// Exposed via <see cref="NpgsqlConnection.PostgreSqlVersion"/>.
        /// </summary>
        public Version Version { get; }

        #endregion General database info

        #region Supported capabilities and features

        /// <summary>
        /// Whether the backend supports range types.
        /// </summary>
        public virtual bool SupportsRangeTypes => Version.IsGreaterOrEqual(9, 2, 0);
        /// <summary>
        /// Whether the backend supports enum types.
        /// </summary>
        public virtual bool SupportsEnumTypes => Version.IsGreaterOrEqual(8, 3, 0);
        /// <summary>
        /// Whether the backend supports the CLOSE ALL statement.
        /// </summary>
        public virtual bool SupportsCloseAll => Version.IsGreaterOrEqual(8, 3, 0);
        /// <summary>
        /// Whether the backend supports advisory locks.
        /// </summary>
        public virtual bool SupportsAdvisoryLocks => Version.IsGreaterOrEqual(8, 2, 0);
        /// <summary>
        /// Whether the backend supports the DISCARD SEQUENCES statement.
        /// </summary>
        public virtual bool SupportsDiscardSequences => Version.IsGreaterOrEqual(9, 4, 0);
        /// <summary>
        /// Whether the backend supports the UNLISTEN statement.
        /// </summary>
        public virtual bool SupportsUnlisten => Version.IsGreaterOrEqual(6, 4, 0);  // overridden by PostgresDatabase
        /// <summary>
        /// Whether the backend supports the DISCARD TEMP statement.
        /// </summary>
        public virtual bool SupportsDiscardTemp => Version.IsGreaterOrEqual(8, 3, 0);
        /// <summary>
        /// Whether the backend supports the DISCARD statement.
        /// </summary>
        public virtual bool SupportsDiscard => Version.IsGreaterOrEqual(8, 3, 0);

        /// <summary>
        /// Reports whether the backend uses the newer integer timestamp representation.
        /// </summary>
        public virtual bool HasIntegerDateTimes { get; protected set; } = true;

        /// <summary>
        /// Whether the database supports transactions.
        /// </summary>
        public virtual bool SupportsTransactions { get; protected set; } = true;

        #endregion Supported capabilities and features

        #region Types

        readonly List<PostgresBaseType>      _baseTypesMutable      = new List<PostgresBaseType>();
        readonly List<PostgresArrayType>     _arrayTypesMutable     = new List<PostgresArrayType>();
        readonly List<PostgresRangeType>     _rangeTypesMutable     = new List<PostgresRangeType>();
        readonly List<PostgresEnumType>      _enumTypesMutable      = new List<PostgresEnumType>();
        readonly List<PostgresCompositeType> _compositeTypesMutable = new List<PostgresCompositeType>();
        readonly List<PostgresDomainType>    _domainTypesMutable    = new List<PostgresDomainType>();

        internal IReadOnlyList<PostgresBaseType>      BaseTypes      => _baseTypesMutable;
        internal IReadOnlyList<PostgresArrayType>     ArrayTypes     => _arrayTypesMutable;
        internal IReadOnlyList<PostgresRangeType>     RangeTypes     => _rangeTypesMutable;
        internal IReadOnlyList<PostgresEnumType>      EnumTypes      => _enumTypesMutable;
        internal IReadOnlyList<PostgresCompositeType> CompositeTypes => _compositeTypesMutable;
        internal IReadOnlyList<PostgresDomainType>    DomainTypes    => _domainTypesMutable;

        /// <summary>
        /// Indexes backend types by their type OID.
        /// </summary>
        internal Dictionary<uint, PostgresType> ByOID { get; } = new Dictionary<uint, PostgresType>();

        /// <summary>
        /// Indexes backend types by their PostgreSQL name, including namespace (e.g. pg_catalog.int4).
        /// Only used for enums and composites.
        /// </summary>
        internal Dictionary<string, PostgresType> ByFullName { get; } = new Dictionary<string, PostgresType>();

        /// <summary>
        /// Indexes backend types by their PostgreSQL name, not including namespace.
        /// If more than one type exists with the same name (i.e. in different namespaces) this
        /// table will contain an entry with a null value.
        /// Only used for enums and composites.
        /// </summary>
        internal Dictionary<string, PostgresType?> ByName { get; } = new Dictionary<string, PostgresType?>();

        /// <summary>
        /// Initializes the instance of <see cref="NpgsqlDatabaseInfo"/>.
        /// </summary>
        protected NpgsqlDatabaseInfo(string host, int port, string databaseName, Version version)
        {
            Host = host;
            Port = port;
            Name = databaseName;
            Version = version;
        }

        internal void ProcessTypes()
        {
            foreach (var type in GetTypes())
            {
                ByOID[type.OID] = type;
                ByFullName[type.FullName] = type;
                // If more than one type exists with the same partial name, we place a null value.
                // This allows us to detect this case later and force the user to use full names only.
                ByName[type.Name] = ByName.ContainsKey(type.Name)
                    ? null
                    : type;

                switch (type)
                {
                case PostgresBaseType baseType:
                    _baseTypesMutable.Add(baseType);
                    continue;
                case PostgresArrayType arrayType:
                    _arrayTypesMutable.Add(arrayType);
                    continue;
                case PostgresRangeType rangeType:
                    _rangeTypesMutable.Add(rangeType);
                    continue;
                case PostgresEnumType enumType:
                    _enumTypesMutable.Add(enumType);
                    continue;
                case PostgresCompositeType compositeType:
                    _compositeTypesMutable.Add(compositeType);
                    continue;
                case PostgresDomainType domainType:
                    _domainTypesMutable.Add(domainType);
                    continue;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Provides all PostgreSQL types detected in this database.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<PostgresType> GetTypes();

        #endregion Types

        #region Misc

        /// <summary>
        /// Parses a PostgreSQL server version (e.g. 10.1, 9.6.3) and returns a CLR Version.
        /// </summary>
        protected static Version ParseServerVersion(string value)
        {
            var versionString = value.Trim();
            for (var idx = 0; idx != versionString.Length; ++idx)
            {
                var c = value[idx];
                if (!char.IsDigit(c) && c != '.')
                {
                    versionString = versionString.Substring(0, idx);
                    break;
                }
            }
            if (!versionString.Contains("."))
                versionString += ".0";
            return new Version(versionString);
        }

        #endregion Misc

        #region Factory management

        /// <summary>
        /// Registers a new database info factory, which is used to load information about databases.
        /// </summary>
        public static void RegisterFactory(INpgsqlDatabaseInfoFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var factories = new INpgsqlDatabaseInfoFactory[Factories.Length + 1];
            factories[0] = factory;
            Array.Copy(Factories, 0, factories, 1, Factories.Length);
            Factories = factories;

            Cache.Clear();
        }

        internal static async Task<NpgsqlDatabaseInfo> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            foreach (var factory in Factories)
            {
                var dbInfo = await factory.Load(conn, timeout, async);
                if (dbInfo != null)
                {
                    dbInfo.ProcessTypes();
                    return dbInfo;
                }
            }

            // Should never be here
            throw new NpgsqlException("No DatabaseInfoFactory could be found for this connection");
        }

        // For tests
        internal static void ResetFactories()
        {
            Factories = new INpgsqlDatabaseInfoFactory[]
            {
                new PostgresMinimalDatabaseInfoFactory(),
                new PostgresDatabaseInfoFactory()
            };
            Cache.Clear();
        }

        #endregion Factory management
    }
}
