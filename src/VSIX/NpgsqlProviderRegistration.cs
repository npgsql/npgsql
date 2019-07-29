using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using Microsoft.VisualStudio.Shell;

namespace Npgsql.VSIX
{
    class NpgsqlProviderRegistration : RegistrationAttribute
    {
        const string DataSourceGuid = "7931728a-ebfb-4677-ad6b-995e29AA15c2";
        const string ProviderGuid = "70ba90f8-3027-4aF1-9b15-37abbd48744c";

        public override void Register(RegistrationContext context)
        {
            Key providerKey = null;
            try
            {
                providerKey = context.CreateKey($@"DataProviders\{{{ProviderGuid}}}");
                providerKey.SetValue(null, ".NET Framework Data Provider for PostgreSQL");
                providerKey.SetValue("AssociatedSource", $"{{{DataSourceGuid}}}");
                providerKey.SetValue("Description", $"Provider_Description, {GetType().Namespace}.Resources, Npgsql.VSIX");
                providerKey.SetValue("DisplayName", $"Provider_DisplayName, {GetType().Namespace}.Resources, Npgsql.VSIX");
                providerKey.SetValue("FactoryService", $"{{{NpgsqlProviderObjectFactory.Guid}}}");
                providerKey.SetValue("InvariantName", Constants.NpgsqlInvariantName);
                providerKey.SetValue("PlatformVersion", "2.0");
                providerKey.SetValue("ShortDisplayName", $"Provider_ShortDisplayName, {GetType().Namespace}.Resources, Npgsql.VSIX");
                providerKey.SetValue("Technology", "{77AB9A9D-78B9-4ba7-91AC-873F5338F1D2}");
                
                var supportedObjectsKey = providerKey.CreateSubkey("SupportedObjects");
                supportedObjectsKey.CreateSubkey(nameof(IVsDataConnectionSupport));
                supportedObjectsKey.CreateSubkey(nameof(IVsDataConnectionUIControl));
                supportedObjectsKey.CreateSubkey(nameof(IVsDataConnectionProperties));
                supportedObjectsKey.CreateSubkey(nameof(IVsDataConnectionEquivalencyComparer));
                supportedObjectsKey.CreateSubkey(nameof(IVsDataSourceInformation));
                supportedObjectsKey.CreateSubkey(nameof(IVsDataObjectSupport));
                supportedObjectsKey.CreateSubkey(nameof(IVsDataViewSupport));

                var dataSourceKey = context.CreateKey($@"DataSources\{{{DataSourceGuid}}}");
                dataSourceKey.SetValue(null, "PostgreSQL Database");
                dataSourceKey.SetValue("DefaultProvider", $"{{{ProviderGuid}}}");
                var supportingProviderKey = dataSourceKey
                    .CreateSubkey("SupportingProviders")
                    .CreateSubkey($"{{{ProviderGuid}}}");
                supportingProviderKey.SetValue("Description", $"Provider_Description, {GetType().Namespace}.Resources, Npgsql.VSIX");
                supportingProviderKey.SetValue("DisplayName", $"Provider_DisplayName, {GetType().Namespace}.Resources, Npgsql.VSIX");
            }
            finally
            {
                providerKey?.Close();
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey($@"DataProviders\{{{ProviderGuid}}}");
            context.RemoveKey($@"DataSources\{{{DataSourceGuid}}}");
        }
    }
}
