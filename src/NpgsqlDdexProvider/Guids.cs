// Guids.cs
// MUST match guids.h
using System;

namespace Npgsql.VisualStudio.Provider
{
    static class GuidList
    {
        public const string guidNpgsqlDdexProviderPkgString = "958b9481-2712-4670-9a62-8fe65e5beea7";
        public const string guidNpgsqlDdexProviderCmdSetString = "095c8e0d-0072-4599-95b8-ade172bfd544";
        public const string guidNpgsqlDdexProviderDataProviderString = "70ba90f8-3027-4aF1-9b15-37abbd48744c";
        public const string guidNpgsqlDdexProviderDataSourceString = "7931728a-ebfb-4677-ad6b-995e29AA15c2";
        public const string guidNpgsqlDdexProviderObjectFactoryString = "555cd66B-3393-4bab-84d9-3f2caa639699";

        public static readonly Guid guidNpgsqlDdexProviderCmdSet = new Guid(guidNpgsqlDdexProviderCmdSetString);
    };
}