using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

// sealed class ByteaConverter : PgStreamingConverter<byte[]>
// {
//     public override byte[] Read(PgReader reader) => reader.read
//
//     public override ValueTask<byte[]> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
//
//     public override Size GetSize(SizeContext context, byte[] value, ref object? writeState) => value.Length;
//
//     public override void Write(PgWriter writer, byte[] value) => throw new System.NotImplementedException();
//
//     public override ValueTask WriteAsync(PgWriter writer, byte[] value, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
// }
