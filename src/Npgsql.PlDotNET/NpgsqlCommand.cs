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
    public class NpgsqlCommand : NpgsqlCommandOrig
    {
        private readonly string query;
        private readonly NpgsqlMultiHostDataSource dataSource;
        private readonly IntPtr cmdPointer;

        public NpgsqlCommand(string query, NpgsqlMultiHostDataSource dataSource)
        {
            this.query = query;
            this.dataSource = dataSource;

            pldotnet_SPIPrepare(this.query, ref this.cmdPointer);
        }

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_SPIPrepare(string command, ref IntPtr cmdPointer);

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_SPICursorOpen(IntPtr cmdPointer, ref IntPtr cursorPointer);

        public new NpgsqlDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => this.ExecuteReader();

        public new NpgsqlDataReader ExecuteReader()
        {
            IntPtr cursorPointer = IntPtr.Zero;

            pldotnet_SPICursorOpen(this.cmdPointer, ref cursorPointer);

            var r = new NpgsqlDataReader(new NpgsqlConnector(this.dataSource))
            {
                CursorPointer = cursorPointer,
            };

            return r;
        }
    }
}