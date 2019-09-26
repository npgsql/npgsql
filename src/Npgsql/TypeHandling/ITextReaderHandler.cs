using System.Data.Common;
using System.IO;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Implemented by handlers which support <see cref="DbDataReader.GetTextReader"/>, returns a standard
    /// TextReader given a binary Stream.
    /// </summary>
    interface ITextReaderHandler
    {
        TextReader GetTextReader(Stream stream);
    }

#pragma warning disable CA1032
}
