using Microsoft.VisualStudio.Shell;
using System;

namespace Npgsql.VisualStudio.Provider
{
    class NpgsqlDataProviderRegistration : RegistrationAttribute
    {
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            Key providerKey = null;
            try
            {
                providerKey = context.CreateKey(@"DataProviders\{" + GuidList.guidNpgsqlDdexProviderDataProviderString + @"}");
                providerKey.SetValue(null, ".NET Framework Data Provider for PostgreSQL");
                providerKey.SetValue("AssociatedSource", "{" + GuidList.guidNpgsqlDdexProviderDataSourceString + "}");
                providerKey.SetValue("Description", "Provider_Description, " + this.GetType().Namespace + ".Resources");
                providerKey.SetValue("DisplayName", "Provider_DisplayName, " + this.GetType().Namespace + ".Resources");
                providerKey.SetValue("FactoryService", "{" + GuidList.guidNpgsqlDdexProviderObjectFactoryString + "}");
                providerKey.SetValue("InvariantName", "Npgsql");
                providerKey.SetValue("PlatformVersion", "2.0");
                providerKey.SetValue("ShortDisplayName", "Provider_ShortDisplayName, " + this.GetType().Namespace + ".Resources");
                providerKey.SetValue("Technology", "{77AB9A9D-78B9-4ba7-91AC-873F5338F1D2}");
                
                providerKey = providerKey.CreateSubkey("SupportedObjects");
                providerKey.CreateSubkey("IVsDataConnectionProperties");
                providerKey.CreateSubkey("IVsDataConnectionUIProperties");
                providerKey.CreateSubkey("IVsDataConnectionSupport");
                providerKey.CreateSubkey("IVsDataObjectSupport");
                providerKey.CreateSubkey("IVsDataViewSupport");

                providerKey = context.CreateKey(@"DataSources\{" + GuidList.guidNpgsqlDdexProviderDataSourceString + @"}");
                providerKey.SetValue(null, "PostgreSQL Database");
                providerKey.SetValue("DefaultProvider", "{" + GuidList.guidNpgsqlDdexProviderDataProviderString + "}");
                providerKey = providerKey.CreateSubkey("SupportingProviders");
                providerKey = providerKey.CreateSubkey("{" + GuidList.guidNpgsqlDdexProviderDataProviderString + "}");
                providerKey.SetValue("Description", "Provider_Description, " + this.GetType().Namespace + ".Resources");
                providerKey.SetValue("DisplayName", "Provider_DisplayName, " + this.GetType().Namespace + ".Resources");
            }
            finally
            {
                if (providerKey != null)
                    providerKey.Close();
            }
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(@"DataProviders\{" + GuidList.guidNpgsqlDdexProviderDataProviderString + @"}");
            context.RemoveKey(@"DataSources\{" + GuidList.guidNpgsqlDdexProviderDataSourceString + @"}");
        }
    }
}
