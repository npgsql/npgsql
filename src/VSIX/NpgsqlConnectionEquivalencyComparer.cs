using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.VSIX
{
    class NpgsqlConnectionEquivalencyComparer : DataConnectionEquivalencyComparer
    {
        protected override bool AreEquivalent(IVsDataConnectionProperties connectionProperties1, IVsDataConnectionProperties connectionProperties2)
            => connectionProperties1["Host"].ToString() == connectionProperties2["Host"].ToString()
            && connectionProperties1["Port"].ToString() == connectionProperties2["Port"].ToString()
            && connectionProperties1["Database"].ToString() == connectionProperties2["Database"].ToString()
            && connectionProperties1["Username"].ToString() == connectionProperties2["Username"].ToString()
            ;
    }
}
