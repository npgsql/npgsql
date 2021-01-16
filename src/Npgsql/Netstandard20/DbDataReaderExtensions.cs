#if NETSTANDARD2_0

#pragma warning disable 1591

using System.Data.Common;

// ReSharper disable once CheckNamespace
namespace System.Data
{
    static class DataReaderExtensions
    {
        public static char GetChar(this DbDataReader reader, string name)
            => reader.GetChar(reader.GetOrdinal(name));

        public static string GetString(this DbDataReader reader, string name)
            => reader.GetString(reader.GetOrdinal(name));

        public static bool IsDBNull(this DbDataReader reader, string name)
            => reader.IsDBNull(reader.GetOrdinal(name));
    }
}
#endif
