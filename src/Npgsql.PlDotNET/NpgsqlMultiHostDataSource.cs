using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using PlDotNET.Handler;

namespace PlDotNET.CustomNpgsql
{
    public class NpgsqlMultiHostDataSource : NpgsqlMultiHostDataSourceOrig
    {
	    // We don't use the connection string, so we use dummy
	    // values for when Npgsql wants to look at it.
        public static string Cs = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

        public static NpgsqlDataSourceBuilder Dsb = new NpgsqlDataSourceBuilder(Cs);
        public static NpgsqlConnectionStringBuilder Sts = new NpgsqlConnectionStringBuilder(Cs);

        internal NpgsqlMultiHostDataSource(NpgsqlConnectionStringBuilder settings, NpgsqlDataSourceConfiguration dataSourceConfig)
        : base(settings, dataSourceConfig)
        {
        }

        public static new NpgsqlMultiHostDataSource Create(string connectionString = "")
        {
            pldotnet_SPIReady();
            return new NpgsqlMultiHostDataSource(Sts, Dsb.PrepareConfiguration());
        }

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool pldotnet_SPIReady();

        public new NpgsqlCommand CreateCommand(string query)
            => new (query, this);
    }
}
