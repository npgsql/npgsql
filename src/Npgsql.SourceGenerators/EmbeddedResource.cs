using System;
using System.IO;
using System.Reflection;

namespace Npgsql.SourceGenerators
{
    static class EmbeddedResource
    {
        public static string GetContent(string relativePath)
        {
            var baseName = Assembly.GetExecutingAssembly().GetName().Name;
            var resourceName = relativePath
                .TrimStart('.')
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');

            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(baseName + "." + resourceName);

            if (stream == null)
                throw new NotSupportedException();

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
