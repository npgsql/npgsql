using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Tests;

/// <summary>
/// Converter-level integration tests for the measuring write protocol. Exercises the dispatch path:
/// <c>BufferRequirements</c> → <c>BindContext</c> construction → <c>Bind</c>
/// → <c>Write</c> → <c>Scope (optional)</c> → length-prefix back-patch.
/// Bypasses the connection layer by composing the options directly, simulating the parameter-level orchestration.
/// </summary>
public class MeasuringConverterTests
{
    [Test]
    public void Parameter_writes_measured_value([Values] bool unknownSize, [Values] bool optionalBind)
    {
        var param = new NpgsqlParameter<byte[]>
            { ParameterName = "p", TypedValue = [0xAA, 0xBB, 0xCC], DataTypeName = "bytea" };

        // A converter that requires measuring writes.
        // Parameter bind materializes the bytes into a side buffer, and yields the exact byte count.
        // Write then replays the bytes with that prefix.
        var (written, byteCount) = BindAndWrite(param, BuildOptions(new MeasuringByteaResolverFactory(
            // Unknown = no hint, UpperBound = arbitrary bound.
            new MeasuringByteaConverter(unknownSize ? Size.Unknown : Size.CreateUpperBound(1000), optionalBind))));

        Assert.That(byteCount, Is.EqualTo(3));
        Assert.That(written, Is.EqualTo(new byte[] { 0, 0, 0, 3, 0xAA, 0xBB, 0xCC }));
    }

    [Test]
    public void Measuring_composer_parameter_materializes_with_nested_length()
    {
        var param = new NpgsqlParameter<byte[]>
            { ParameterName = "p", TypedValue = [0xAA, 0xBB, 0xCC], DataTypeName = "bytea" };

        // A converter that requires measuring writes.
        // It internally opens another scope and its own measuring resolves in-place there.
        // Parameter bind materializes the bytes into a side buffer, and yields the exact byte count.
        // Write then replays the bytes with that prefix.
        var (written, byteCount) = BindAndWrite(param, BuildOptions(new MeasuringByteaResolverFactory(new MeasuringScopeByteaConverter())));

        Assert.That(byteCount, Is.EqualTo(7));
        // outer field prefix (7 = nested frame) + nested int4 prefix (3) + content.
        Assert.That(written, Is.EqualTo(new byte[] { 0, 0, 0, 7, 0, 0, 0, 3, 0xAA, 0xBB, 0xCC }));
    }

    static PgSerializerOptions BuildOptions(PgTypeInfoResolverFactory factory)
    {
        var dsb = new NpgsqlSlimDataSourceBuilder("Host=localhost");
        dsb.AddTypeInfoResolverFactory(factory);
        using var dataSource = dsb.Build();
        return new PgSerializerOptions(PostgresMinimalDatabaseInfo.DefaultTypeCatalog, dataSource.Configuration.ResolverChain);
    }

    // Drives Resolve → Bind → Write against an in-memory buffer; FlushMode.None keeps Write synchronous.
    // Returns the written bytes and the byte count Bind produced (what the Bind message length is summed from).
    static (byte[] Written, int ByteCount) BindAndWrite(NpgsqlParameter param, PgSerializerOptions options)
    {
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        param.Bind(out _, out var byteCount);

        var sink = new ArrayBufferWriter<byte>();
        var task = param.Write(async: false, new PgWriter(sink).Init(PostgresMinimalDatabaseInfo.DefaultTypeCatalog, 0), CancellationToken.None);
        Debug.Assert(task.IsCompletedSuccessfully, "Write should complete synchronously with FlushMode.None");
        task.GetAwaiter().GetResult();
        return (sink.WrittenSpan.ToArray(), byteCount);
    }

    sealed class MeasuringByteaResolverFactory(PgConverter<byte[]> converter) : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new Resolver(converter);
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        sealed class Resolver(PgConverter<byte[]> converter) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
                => dataTypeName == DataTypeNames.Bytea && (type == typeof(byte[]) || type is null)
                    ? new PgProviderTypeInfo(options, new MeasuringByteaProvider(options, converter), DataTypeNames.Bytea)
                    : null;
        }

        sealed class MeasuringByteaProvider(PgSerializerOptions options, PgConverter<byte[]> converter) : PgConcreteTypeInfoProvider<byte[]>
        {
            PgConcreteTypeInfo? _concreteTypeInfo;

            PgConcreteTypeInfo GetOrCreate()
                => _concreteTypeInfo ??= new(options, converter, options.GetCanonicalTypeId(DataTypeNames.Bytea));

            protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId) => GetOrCreate();
            protected override PgConcreteTypeInfo GetForValueCore(ProviderValueContext context, byte[]? value, ref object? writeState) => GetOrCreate();
        }
    }

    sealed class MeasuringByteaConverter(Size writeSize, bool optionalBind = false) : PgStreamingConverter<byte[]>
    {
        public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        {
            bufferRequirements = BufferRequirements.Create(Size.Unknown, writeSize, optionalBind);
            return format is DataFormat.Binary;
        }

        protected override Size BindValue(in BindContext context, byte[] value, ref object? writeState)
            => writeSize;

        public override void Write(PgWriter writer, byte[] value)
            => writer.WriteBytes(value);

        public override byte[] Read(PgReader reader) => throw new NotSupportedException();
        public override ValueTask<byte[]> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public override ValueTask WriteAsync(PgWriter writer, byte[] value, CancellationToken cancellationToken = default)
        {
            Write(writer, value);
            return new();
        }
    }

    sealed class MeasuringScopeByteaConverter : PgStreamingConverter<byte[]>
    {
        public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        {
            bufferRequirements = BufferRequirements.Streaming;
            return format is DataFormat.Binary;
        }

        // Add arbitrary slack to the exact byte count.
        protected override Size BindValue(in BindContext context, byte[] value, ref object? writeState)
            => Size.CreateUpperBound(value.Length + 16);

        public override void Write(PgWriter writer, byte[] value)
        {
            using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.CreateUpperBound(value.Length), state: null))
                writer.WriteBytes(value);
        }

        public override byte[] Read(PgReader reader) => throw new NotSupportedException();
        public override ValueTask<byte[]> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public override ValueTask WriteAsync(PgWriter writer, byte[] value, CancellationToken cancellationToken = default)
        {
            Write(writer, value);
            return new();
        }
    }
}
