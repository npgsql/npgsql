using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Npgsql.VSIX
{
    [Guid(Guid)]
    class NpgsqlProviderObjectFactory : DataProviderObjectFactory
    {
        internal const string Guid = "555cd66B-3393-4bab-84d9-3f2caa639699";

        public override object CreateObject(Type objType)
        {
            if (objType == typeof(IVsDataConnectionSupport))
                return new AdoDotNetConnectionSupport();
            if (objType == typeof(IVsDataConnectionProperties) || objType == typeof(IVsDataConnectionUIProperties))
                return new NpgsqlConnectionProperties();
            if (objType == typeof(IVsDataConnectionUIControl))
                return new NpgsqlConnectionUIControl();
            if (objType == typeof(IVsDataSourceInformation))
                return new NpgsqlSourceInformation();
            if (objType == typeof(IVsDataObjectSupport))
                return new DataObjectSupport($"{GetType().Namespace}.NpgsqlDataObjectSupport", Assembly.GetExecutingAssembly());
            if (objType == typeof(IVsDataViewSupport))
                return new DataViewSupport($"{GetType().Namespace}.NpgsqlDataViewSupport", Assembly.GetExecutingAssembly());
            if (objType == typeof(IVsDataConnectionEquivalencyComparer))
                return new NpgsqlConnectionEquivalencyComparer();
            return null;
        }
    }
}
