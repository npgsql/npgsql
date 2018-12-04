using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;

namespace Npgsql.VSIX
{
    public class NpgsqlConnectionProperties : AdoDotNetConnectionProperties
    {
        static readonly Dictionary<string, string[]> Synonyms;

        public override bool IsComplete => 
            !string.IsNullOrEmpty((string)this["Host"]) &&
            !string.IsNullOrEmpty((string)this["Database"]) &&
            (
                (bool)this["Integrated Security"] ||
                (!string.IsNullOrEmpty((string)this["Username"]) && !string.IsNullOrEmpty((string)this["Password"]))
            );

        public override string[] GetSynonyms(string key) => Synonyms[key];

        static NpgsqlConnectionProperties()
        {
            Synonyms = typeof(NpgsqlConnectionStringBuilder)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<NpgsqlConnectionStringPropertyAttribute>() != null)
                .ToDictionary(
                    p => p.GetCustomAttribute<DisplayNameAttribute>().DisplayName,
                    p => p.GetCustomAttribute<NpgsqlConnectionStringPropertyAttribute>().Synonyms
                );
        }
    }
}
