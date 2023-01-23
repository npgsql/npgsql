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
    public class NpgsqlDataReader : NpgsqlDataReaderOrig
    {
        public static TextHandler TextHandlerObject = new ();
        public static BoolHandler BoolHandlerObject = new ();
        public static IntHandler IntHandlerObject = new ();
        public static ShortHandler ShortHandlerObject = new ();

        public static Dictionary<OID, Type> TypeByOid =
                       new ()
        {
            { OID.BOOLOID, typeof(bool) },
            { OID.INT2OID, typeof(short) },
            { OID.INT4OID, typeof(int) },
            { OID.TEXTOID, typeof(string) },
        };

        public int NRows = 0;
        public int NCols = 0;
        public DataTable ReturnedTable;
        public DataRow CurrentRow;
        public new int FieldCount;
        public string[] ColumnNames;
        public int[] ColumnTypes;

        public IntPtr CursorPointer;

        public NpgsqlDataReader(NpgsqlConnector connector)
        : base(connector)
        {
        }

        // set a .NET type to the data row using reference...
        public static void SetValueInRow(ref DataRow row, string key, uint type, IntPtr datum)
        {
            row[key] = type switch
            {
                (uint)OID.BOOLOID => BoolHandlerObject.InputValue(datum),
                (uint)OID.INT2OID => ShortHandlerObject.InputValue(datum),
                (uint)OID.INT4OID => IntHandlerObject.InputValue(datum),
                (uint)OID.TEXTOID => TextHandlerObject.InputValue(datum),
                _ => throw new NotImplementedException($"Datum to {(OID)type} is not supported! Check SetValueInRow"),
            };
        }

        public static DataTable TuptableToDataTable(IntPtr[] datums, int[] columnTypes, string[] columnNames, int ncols, int nrows)
        {
            DataTable userDataTable = new ();
            for (int j = 0; j < ncols; j++)
            {
                OID type = (OID)columnTypes[j];
                if (TypeByOid.ContainsKey(type))
                {
                    userDataTable.Columns.Add(columnNames[j], TypeByOid[type]);
                }
                else
                {
                    throw new NotImplementedException($"Datum to {(OID)type} is not supported!");
                }
            }

            for (int i = 0; i < nrows; i++)
            {
                DataRow row = userDataTable.NewRow();
                for (int j = 0; j < ncols; j++)
                {
                    SetValueInRow(ref row, columnNames[j], (uint)columnTypes[j], datums[(i * ncols) + j]);
                }

                userDataTable.Rows.Add(row);
            }

            return userDataTable;
        }

        public override bool Read()
        {
            pldotnet_SPICursorFetch(this.CursorPointer);
            pldotnet_GetTableDimensions(ref this.NRows, ref this.NCols);

            if (this.NRows < 1)
            {
                this.Close();
                return false;
            }

            IntPtr[] datums = new IntPtr[this.NCols];

            if (this.ColumnNames == null && this.ColumnTypes == null)
            {
                IntPtr[] columnNamePts = new IntPtr[this.NCols];
                this.ColumnTypes = new int[this.NCols];
                pldotnet_GetColProps(this.ColumnTypes, columnNamePts);
                this.ColumnNames = columnNamePts.ToList().Select(namePts => Marshal.PtrToStringAuto(namePts)).ToArray();
            }

            pldotnet_GetTable(datums);

            this.ReturnedTable = TuptableToDataTable(datums, this.ColumnTypes, this.ColumnNames, this.NCols, this.NRows);

            this.CurrentRow = this.ReturnedTable.Rows[0];
            this.FieldCount = this.ReturnedTable.Columns.Count;

            return true;
        }

        public override void Close()
        {
            pldotnet_SPIFinish();
        }

        public override string GetName(int ordinal)
            => this.ColumnNames[ordinal];

        public override int GetOrdinal(string name)
        {
            for (int i = 0; i < this.ReturnedTable.Columns.Count; i++)
            {
                if (this.ReturnedTable.Columns[i].ColumnName == name)
                {
                    return i;
                }
            }

            throw new ArgumentException("Column not found", nameof(name));
        }

        public override T GetFieldValue<T>(int ordinal)
            => (T)this.CurrentRow[ordinal];

        public override int GetValues(object[] values)
        {
            for (int i = 0; i < this.FieldCount; i++)
            {
                values[i] = this.CurrentRow[i];
            }

            return this.FieldCount;
        }

        public string getTableLinesMd()
        {
            var sb = new System.Text.StringBuilder();

            foreach (DataRow dataRow in this.ReturnedTable.Rows)
            {
                sb.Append($"| {string.Join(" | ", dataRow.ItemArray)} |\n");
            }

            return sb.ToString();
        }

        public string getTableHeaderMd()
        {
            List<string> columnNamesDT = new ();
            var sb = new System.Text.StringBuilder();
            var divisor = new System.Text.StringBuilder();

            foreach (DataColumn column in this.ReturnedTable.Columns)
            {
                columnNamesDT.Add(column.ColumnName);
                divisor.Append("| - ");
            }

            divisor.Append("|\n");

            sb.Append($"| {string.Join(" | ", columnNamesDT.ToArray())} |\n");
            sb.Append(divisor);

            return sb.ToString();
        }

        public override string ToString()
        {
            _ = new List<string>();

            var sb = new System.Text.StringBuilder();
            sb.Append(this.getTableHeaderMd());
            sb.Append(this.getTableLinesMd());

            return sb.ToString();
        }

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_SPIFinish();

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_GetTableDimensions(ref int nrows, ref int ncols);

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_GetTable(IntPtr[] datums);

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_GetColProps(int[] columnTypes, IntPtr[] columnNames);

        [DllImport("@PKG_LIBDIR/pldotnet.so")]
        public static extern void pldotnet_SPICursorFetch(IntPtr cursorPointer);
    }
}