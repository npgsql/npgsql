using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;

namespace Npgsql.VisualStudio.Provider
{
    class NpgsqlDataObjectSelector : AdoDotNetRootObjectSelector
    {
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            
            if (Site != null)
            {
                IVsDataReader reader = null;
                DbConnection conn = (DbConnection)Site.GetLockedProviderObject();
                if ("".Equals(typeName))
                {
                    DbConnectionStringBuilder builder = DbProviderFactories.GetFactory("Npgsql").CreateConnectionStringBuilder();
                    builder.ConnectionString = conn.ConnectionString;
                    DataTable table = new DataTable();
                    table.Columns.AddRange(builder.Keys.Cast<String>().Select((String k) => new DataColumn(k, builder[k].GetType())).ToArray());
                    DataRow row = table.NewRow();
                    row.ItemArray = builder.Values.Cast<object>().ToArray();
                    table.Rows.Add(row);
                    reader = new AdoDotNetTableReader(table);
                }
                Site.UnlockProviderObject();
                if (reader != null)
                    return reader;
            }
            throw new NotImplementedException();
        }
    }
}
