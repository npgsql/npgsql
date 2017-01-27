using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;

namespace Npgsql.VSIX
{
    public class NpgsqlSourceInformation : AdoDotNetSourceInformation
    {
        public NpgsqlSourceInformation()
        {
            AddProperty(CatalogSupported, false);
            AddProperty(CatalogSupportedInDml, false);
            AddProperty(DefaultSchema, "public");
            AddProperty(IdentifierOpenQuote, "\"");
            AddProperty(IdentifierCloseQuote, "\"");
            AddProperty(IdentifierPartsCaseSensitive, false);
            //AddProperty(IdentifierPartsStorageCase, "M");
            AddProperty(ParameterPrefix, "@");
            AddProperty(ParameterPrefixInName, false);
            AddProperty(QuotedIdentifierPartsCaseSensitive, true);
            AddProperty(QuotedIdentifierPartsStorageCase, "M");
            AddProperty(SchemaSeparator, ".");
            AddProperty(SchemaSupported, true);
            AddProperty(SchemaSupportedInDml, true);
            AddProperty(SupportsAnsi92Sql, true);
            AddProperty(SupportsNestedTransactions, false);
            AddProperty(SupportsQuotedIdentifierParts, true);
            AddProperty(UserSupported, false);
            AddProperty(ViewSupported, true);
        }

        protected override object RetrieveValue(string propertyName)
        {
            if (AreEqual(propertyName, DataSourceVersion))
                return Connection.ServerVersion ?? "";
            return base.RetrieveValue(propertyName);
        }

        static bool AreEqual(string propertyName, string candidate)
            => propertyName.Equals(candidate, StringComparison.OrdinalIgnoreCase);
    }
}
