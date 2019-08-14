#if NET461 || NETSTANDARD2_0
#pragma warning disable 1591
// ReSharper disable once CheckNamespace
using System.Data.Common;

namespace System.Data
{
    internal static class DataReaderExtensions
    {
        public static char GetChar(this DbDataReader reader, string name)
            => reader.GetChar(reader.GetOrdinal(name));

        public static string GetString(this DbDataReader reader, string name)
            => reader.GetString(reader.GetOrdinal(name));
    }
}
#endif
