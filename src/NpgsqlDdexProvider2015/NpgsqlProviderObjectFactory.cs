using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Runtime.InteropServices;

namespace Npgsql.VisualStudio.Provider
{
    [Guid(GuidList.guidNpgsqlDdexProviderObjectFactoryString)]
    class NpgsqlProviderObjectFactory : DataProviderObjectFactory
    {
        public override object CreateObject(Type objType)
        {
            if (objType == typeof(IVsDataConnectionProperties) || objType == typeof(IVsDataConnectionUIProperties))
                return new AdoDotNetConnectionProperties();
            else if (objType == typeof(IVsDataConnectionSupport))
                return new AdoDotNetConnectionSupport();
            else if (objType == typeof(IVsDataObjectSupport))
                return new DataObjectSupport(this.GetType().Namespace + ".NpgsqlDataObjectSupport", System.Reflection.Assembly.GetExecutingAssembly());
            else if (objType == typeof(IVsDataViewSupport))
                return new DataViewSupport(this.GetType().Namespace + ".NpgsqlDataViewSupport", System.Reflection.Assembly.GetExecutingAssembly());
            return null;
        }
    }
}
