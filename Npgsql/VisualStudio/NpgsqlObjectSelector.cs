using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Npgsql.VisualStudio
{
    public class NpgsqlObjectSelector : DataObjectSelector
    {
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
