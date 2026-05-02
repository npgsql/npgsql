using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests;

/// <summary>
/// Tests that pin the write-state propagation and disposal contracts between NpgsqlParameter,
/// PgTypeInfo providers, and the converters they produce.
/// </summary>
public class WriteStateTests : TestBase
{
    [Test]
    public async Task Nullable_array_write_state_flows([Values] bool fixedSize)
    {
        // Verifies that provider-produced write state flows through IsDbNull and Write
        // for nullable array elements, both fixed-size and variable-size.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableArrays();
        dataSourceBuilder.AddTypeInfoResolverFactory(new WriteStateTrackingResolverFactory(fixedSize, tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        var input = new[] { 1, 2, 3 };

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<int[]> { ParameterName = "p", TypedValue = input, DataTypeName = "integer[]" });
        await cmd.ExecuteNonQueryAsync();

        if (fixedSize)
            Assert.That(tracker.IsDbNullWriteStateReceived, Is.True, "IsDbNullValue did not receive write state");
        Assert.That(tracker.WriteWriteStateReceived, Is.True, "Write did not receive write state");
    }

    [Test]
    public async Task Object_array_write_state_flows_through_late_bound_element([Values] bool fixedSize)
    {
        // Verifies write state propagation through two layers with mixed element shapes:
        //   outer: ArrayTypeInfoProvider<object[], object>
        //   inner: LateBoundTypeInfoProvider (object -> int or DBNull)
        //            int path  -> WriteStateTrackingProvider<int>         (per-element wrapped WriteState)
        //            null path -> PgSerializerOptions.UnspecifiedDBNullTypeInfo (different concrete info entirely)
        // The tracking int converter's IsDbNullValue and WriteCore must see the provider-produced
        // write state after passing through the array + ObjectConverter layers, while the DBNull
        // slots must flow through without disturbing the non-null slots.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableArrays();
        dataSourceBuilder.AddTypeInfoResolverFactory(new WriteStateTrackingResolverFactory(fixedSize, tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        var input = new object[] { 1, DBNull.Value, 2, DBNull.Value, 3 };

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<object[]> { ParameterName = "p", TypedValue = input, DataTypeName = "integer[]" });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.IsDbNullWriteStateReceived, Is.True,
            "IsDbNullValue did not receive write state after array + late-bound object layers");
        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after array + late-bound object layers");
    }

    [Test]
    public async Task Object_write_state_upgrades_when_late_bound_inner_produces_at_bind()
    {
        // Verifies the bind-time writeState upgrade path: late-bound resolution returns a converter
        // whose provider does not pre-populate state, so writeState arrives at ObjectConverter.BindValue
        // as a bare PgConcreteTypeInfo (non-IDisposable). The inner converter's BindValue then produces
        // state during bind, and ObjectConverter must upgrade the outer writeState reference from the
        // bare PgConcreteTypeInfo to a wrapped ObjectConverter.WriteState (IDisposable).
        // Exercises the disposability-conditional lifecycle rule: replacing a non-IDisposable reference
        // with an IDisposable wrapper is allowed because the original carries no disposal obligation.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.AddTypeInfoResolverFactory(
            new WriteStateTrackingResolverFactory(fixedSize: false, tracker, produceProviderState: false, generatesWriteStateAtBind: true));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<object> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after late-bound bind-time upgrade");
    }

    [Test]
    public async Task Range_write_state_flows()
    {
        // Verifies write state propagation through a range composition:
        //   RangeConverter<int> -> tracking int subtype (BindValue populates writeState)
        // The range converter must carry each bound's subtype state into BeginNestedWrite so the subtype's
        // WriteCore observes the provider-produced sentinel.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableRanges();
        dataSourceBuilder.AddTypeInfoResolverFactory(new RangeWriteStateTrackingResolverFactory(tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        var input = new NpgsqlRange<int>(1, 10);

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<NpgsqlRange<int>>
            { ParameterName = "p", TypedValue = input, DataTypeName = "int4range" });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after range -> subtype composition");
    }

    [Test]
    public async Task Multirange_write_state_flows()
    {
        // Verifies write state propagation through a multirange composition:
        //   MultirangeConverter<NpgsqlRange<int>[], NpgsqlRange<int>> -> RangeConverter<int> -> tracking int subtype
        // Three layers: multirange stores per-range state, range stores per-bound state, subtype populates bound state.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableRanges();
        dataSourceBuilder.EnableMultiranges();
        dataSourceBuilder.AddTypeInfoResolverFactory(new RangeWriteStateTrackingResolverFactory(tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        var input = new[] { new NpgsqlRange<int>(1, 10), new NpgsqlRange<int>(20, 30) };

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<NpgsqlRange<int>[]>
            { ParameterName = "p", TypedValue = input, DataTypeName = "int4multirange" });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after multirange -> range -> subtype composition");
    }

    [Test]
    public async Task Range_array_write_state_flows()
    {
        // Verifies write state propagation through an array-over-range composition:
        //   ArrayConverter<NpgsqlRange<int>[]> -> RangeConverter<int> -> tracking int subtype
        // The per-element array slot carries the range's WriteState, which itself nests the subtype state.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableArrays();
        dataSourceBuilder.EnableRanges();
        dataSourceBuilder.AddTypeInfoResolverFactory(new RangeWriteStateTrackingResolverFactory(tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        var input = new[] { new NpgsqlRange<int>(1, 10), new NpgsqlRange<int>(20, 30) };

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<NpgsqlRange<int>[]>
            { ParameterName = "p", TypedValue = input, DataTypeName = "int4range[]" });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after array -> range -> subtype composition");
    }

    [Test]
    public async Task Array_of_multirange_write_state_flows()
    {
        // Verifies write state propagation through an array-over-multirange composition (four layers):
        //   ArrayConverter<NpgsqlRange<int>[][]> -> MultirangeConverter -> RangeConverter<int> -> tracking int subtype
        // The deepest common composition shape — if any layer loses state, the subtype's WriteCore never sees it.
        var tracker = new WriteStateTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableArrays();
        dataSourceBuilder.EnableRanges();
        dataSourceBuilder.EnableMultiranges();
        dataSourceBuilder.AddTypeInfoResolverFactory(new RangeWriteStateTrackingResolverFactory(tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        var input = new[]
        {
            new[] { new NpgsqlRange<int>(1, 10), new NpgsqlRange<int>(20, 30) },
            new[] { new NpgsqlRange<int>(40, 50) }
        };

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<NpgsqlRange<int>[][]>
            { ParameterName = "p", TypedValue = input, DataTypeName = "int4multirange[]" });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after array -> multirange -> range -> subtype composition");
    }

    [Test]
    public async Task Composite_write_state_flows()
    {
        // Verifies write state propagation through a composite composition:
        //   CompositeConverter<CompositeWithInt> -> tracking int4 field converter
        // The composite's per-field WriteState storage must carry the subtype's sentinel into BeginNestedWrite.
        // Uses a real PG CREATE TYPE + MapComposite so the CompositeConverter is constructed by the production path.
        var tracker = new WriteStateTracker();
        await using var adminConnection = await OpenConnectionAsync();
        var type = await TestUtil.GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new CompositeFieldWriteStateTrackingResolverFactory(tracker));
        dataSourceBuilder.MapComposite<CompositeWithInt>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT @p", connection);
        cmd.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "p",
            Value = new CompositeWithInt { X = 42 },
            DataTypeName = type
        });
        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after composite -> field subtype composition");
    }

    [Test]
    public async Task Execute_disposes_write_state()
    {
        // Verifies that write state produced during ResolveTypeInfo (and carried through Bind/Write) is disposed
        // once the normal execution path finishes, via ResetBindingInfo in the Write finally block.
        var tracker = new DisposalTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.AddTypeInfoResolverFactory(new DisposableWriteStateResolverFactory(tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" });

        await cmd.ExecuteNonQueryAsync();

        Assert.That(tracker.Disposed, Is.True, "provider-produced write state was not disposed after normal execution");
    }

    [Test]
    public async Task SchemaOnly_disposes_resolution_write_state()
    {
        // Verifies that write state produced during ResolveTypeInfo is disposed when Bind is skipped
        // (e.g. CommandBehavior.SchemaOnly), so provider-allocated state does not leak.
        var tracker = new DisposalTracker();
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.AddTypeInfoResolverFactory(new DisposableWriteStateResolverFactory(tracker));
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" });

        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);

        Assert.That(tracker.Disposed, Is.True, "provider-produced write state was not disposed after SchemaOnly execution");
    }

    sealed class WriteStateTracker
    {
        public bool IsDbNullWriteStateReceived;
        public bool WriteWriteStateReceived;
    }

    sealed class WriteStateTrackingConverter : PgBufferedConverter<int>
    {
        readonly bool _fixedSize;
        readonly WriteStateTracker _tracker;
        readonly bool _generatesWriteState;

        public WriteStateTrackingConverter(bool fixedSize, WriteStateTracker tracker, bool generatesWriteState = false)
        {
            _fixedSize = fixedSize;
            _tracker = tracker;
            _generatesWriteState = generatesWriteState;
            HandleDbNull = true;
        }

        public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        {
            bufferRequirements = _fixedSize ? BufferRequirements.CreateFixedSize(sizeof(int)) : BufferRequirements.Create(Size.CreateUpperBound(sizeof(int)));
            return format is DataFormat.Binary;
        }

        protected override bool IsDbNullValue(int value, object? writeState)
        {
            if (writeState is not null)
                _tracker.IsDbNullWriteStateReceived = true;
            return false;
        }

        protected override int ReadCore(PgReader reader) => reader.ReadInt32();

        protected override void WriteCore(PgWriter writer, int value)
        {
            if (writer.Current.WriteState is not null)
                _tracker.WriteWriteStateReceived = true;
            writer.WriteInt32(value);
        }

        protected override Size BindValue(in BindContext context, int value, ref object? writeState)
        {
            // Range/Multirange call the subtype converter directly with a fresh null writeState, so for those tests the
            // subtype must produce state from BindValue. For the array tests the provider has already populated non-null
            // state and the ??= is a no-op, preserving existing behavior.
            if (_generatesWriteState)
                writeState ??= "provider-state";
            return sizeof(int);
        }
    }

    sealed class WriteStateTrackingProvider(PgSerializerOptions options, bool fixedSize, WriteStateTracker tracker, bool produceProviderState = true, bool generatesWriteStateAtBind = false) : PgConcreteTypeInfoProvider<int>
    {
        PgConcreteTypeInfo? _concreteTypeInfo;

        PgConcreteTypeInfo GetOrCreate()
            => _concreteTypeInfo ??= new(options, new WriteStateTrackingConverter(fixedSize, tracker, generatesWriteStateAtBind), options.GetCanonicalTypeId(DataTypeNames.Int4));

        protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId) => GetOrCreate();

        protected override PgConcreteTypeInfo GetForValueCore(ProviderValueContext context, int value, ref object? writeState)
        {
            if (produceProviderState)
                writeState = "provider-state";
            return GetOrCreate();
        }
    }

    sealed class WriteStateTrackingResolverFactory(bool fixedSize, WriteStateTracker tracker, bool produceProviderState = true, bool generatesWriteStateAtBind = false) : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new Resolver(fixedSize, tracker, produceProviderState, generatesWriteStateAtBind);
        public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

        sealed class Resolver(bool fixedSize, WriteStateTracker tracker, bool produceProviderState, bool generatesWriteStateAtBind) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4 && (type == typeof(int) || type is null))
                    return new PgProviderTypeInfo(options, new WriteStateTrackingProvider(options, fixedSize, tracker, produceProviderState, generatesWriteStateAtBind), DataTypeNames.Int4);

                // object->int4 goes through LateBoundTypeInfoProvider which delegates back to the int resolver above,
                // letting us exercise write-state propagation across the object (late-bound) element layer.
                if (dataTypeName == DataTypeNames.Int4 && type == typeof(object))
                    return new PgProviderTypeInfo(options, new LateBoundTypeInfoProvider(options, options.GetCanonicalTypeId(DataTypeNames.Int4)), DataTypeNames.Int4);

                return null;
            }
        }

        sealed class ArrayResolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName != DataTypeNames.Int4.ToArrayName())
                    return null;

                if (type == typeof(object[]))
                {
                    var objectElementInfo = options.GetTypeInfo(typeof(object), DataTypeNames.Int4);
                    if (objectElementInfo is not PgProviderTypeInfo objectElementProviderTypeInfo)
                        return null;

                    return new PgProviderTypeInfo(options,
                        new ArrayTypeInfoProvider<object[], object>(objectElementProviderTypeInfo, typeof(object[])),
                        dataTypeName);
                }

                var elementInfo = options.GetTypeInfo(typeof(int), DataTypeNames.Int4);
                if (elementInfo is not PgProviderTypeInfo providerTypeInfo)
                    return null;

                return new PgProviderTypeInfo(options,
                    new ArrayTypeInfoProvider<int[], int>(providerTypeInfo, typeof(int[])),
                    dataTypeName);
            }
        }
    }

    sealed class DisposalTracker : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }

    sealed class DisposableWriteStateConverter : PgBufferedConverter<int>
    {
        public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        {
            bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int));
            return format is DataFormat.Binary;
        }

        protected override int ReadCore(PgReader reader) => reader.ReadInt32();
        protected override void WriteCore(PgWriter writer, int value) => writer.WriteInt32(value);
    }

    sealed class DisposableWriteStateProvider(PgSerializerOptions options, DisposalTracker tracker) : PgConcreteTypeInfoProvider<int>
    {
        PgConcreteTypeInfo? _concreteTypeInfo;

        PgConcreteTypeInfo GetOrCreate()
            => _concreteTypeInfo ??= new(options, new DisposableWriteStateConverter(), options.GetCanonicalTypeId(DataTypeNames.Int4));

        protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId) => GetOrCreate();

        protected override PgConcreteTypeInfo GetForValueCore(ProviderValueContext context, int value, ref object? writeState)
        {
            writeState = tracker;
            return GetOrCreate();
        }
    }

    sealed class DisposableWriteStateResolverFactory(DisposalTracker tracker) : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new Resolver(tracker);
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        sealed class Resolver(DisposalTracker tracker) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4 && (type == typeof(int) || type is null))
                    return new PgProviderTypeInfo(options, new DisposableWriteStateProvider(options, tracker), DataTypeNames.Int4);

                return null;
            }
        }
    }

    sealed class RangeWriteStateTrackingResolverFactory(WriteStateTracker tracker) : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new NoOpResolver();
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        public override IPgTypeInfoResolver? CreateRangeResolver() => new RangeResolver(tracker);
        public override IPgTypeInfoResolver? CreateRangeArrayResolver() => new RangeArrayResolver(tracker);
        public override IPgTypeInfoResolver? CreateMultirangeResolver() => new MultirangeResolver(tracker);
        public override IPgTypeInfoResolver? CreateMultirangeArrayResolver() => new MultirangeArrayResolver(tracker);

        sealed class NoOpResolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options) => null;
        }

        sealed class RangeResolver(WriteStateTracker tracker) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4Range && (type == typeof(NpgsqlRange<int>) || type is null))
                {
                    var subtype = new WriteStateTrackingConverter(fixedSize: false, tracker, generatesWriteState: true);
                    var range = PgConverterFactory.CreateRangeConverter(subtype, options);
                    return new PgConcreteTypeInfo(options, range, options.GetCanonicalTypeId(DataTypeNames.Int4Range));
                }
                return null;
            }
        }

        sealed class RangeArrayResolver(WriteStateTracker tracker) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4Range.ToArrayName() && (type == typeof(NpgsqlRange<int>[]) || type is null))
                {
                    var subtype = new WriteStateTrackingConverter(fixedSize: false, tracker, generatesWriteState: true);
                    var range = PgConverterFactory.CreateRangeConverter(subtype, options);
                    var rangeInfo = new PgConcreteTypeInfo(options, range, options.GetCanonicalTypeId(DataTypeNames.Int4Range));
                    var arrayConverter = ArrayConverter<NpgsqlRange<int>[]>.CreateArrayBased<NpgsqlRange<int>>(rangeInfo, typeof(NpgsqlRange<int>[]));
                    return new PgConcreteTypeInfo(options, arrayConverter, options.GetCanonicalTypeId(DataTypeNames.Int4Range.ToArrayName()));
                }
                return null;
            }
        }

        sealed class MultirangeResolver(WriteStateTracker tracker) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4Multirange && (type == typeof(NpgsqlRange<int>[]) || type is null))
                {
                    var subtype = new WriteStateTrackingConverter(fixedSize: false, tracker, generatesWriteState: true);
                    var range = PgConverterFactory.CreateRangeConverter(subtype, options);
                    var multirange = PgConverterFactory.CreateArrayMultirangeConverter(range, options);
                    return new PgConcreteTypeInfo(options, multirange, options.GetCanonicalTypeId(DataTypeNames.Int4Multirange));
                }
                return null;
            }
        }

        sealed class MultirangeArrayResolver(WriteStateTracker tracker) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4Multirange.ToArrayName() && (type == typeof(NpgsqlRange<int>[][]) || type is null))
                {
                    var subtype = new WriteStateTrackingConverter(fixedSize: false, tracker, generatesWriteState: true);
                    var range = PgConverterFactory.CreateRangeConverter(subtype, options);
                    var multirange = PgConverterFactory.CreateArrayMultirangeConverter(range, options);
                    var multirangeInfo = new PgConcreteTypeInfo(options, multirange, options.GetCanonicalTypeId(DataTypeNames.Int4Multirange));
                    var arrayConverter = ArrayConverter<NpgsqlRange<int>[][]>.CreateArrayBased<NpgsqlRange<int>[]>(multirangeInfo, typeof(NpgsqlRange<int>[][]));
                    return new PgConcreteTypeInfo(options, arrayConverter, options.GetCanonicalTypeId(DataTypeNames.Int4Multirange.ToArrayName()));
                }
                return null;
            }
        }
    }

    class CompositeWithInt
    {
        public int X { get; set; }
    }

    sealed class CompositeFieldWriteStateTrackingResolverFactory(WriteStateTracker tracker) : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new Resolver(tracker);
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        sealed class Resolver(WriteStateTracker tracker) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4 && (type == typeof(int) || type is null))
                {
                    var converter = new WriteStateTrackingConverter(fixedSize: false, tracker, generatesWriteState: true);
                    return new PgConcreteTypeInfo(options, converter, options.GetCanonicalTypeId(DataTypeNames.Int4));
                }
                return null;
            }
        }
    }
}
