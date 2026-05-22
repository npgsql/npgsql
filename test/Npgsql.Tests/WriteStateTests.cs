using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
    public void Nullable_array_write_state_flows([Values] bool fixedSize)
    {
        // Verifies that provider-produced write state flows through IsDbNull and Write
        // for nullable array elements, both fixed-size and variable-size.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(b => b.EnableArrays(), new WriteStateTrackingResolverFactory(fixedSize, tracker));

        var param = new NpgsqlParameter<int[]>
            { ParameterName = "p", TypedValue = new[] { 1, 2, 3 }, DataTypeName = "integer[]" };
        BindAndWriteToMemory(param, options);

        if (fixedSize)
            Assert.That(tracker.IsDbNullWriteStateReceived, Is.True, "IsDbNullValue did not receive write state");
        Assert.That(tracker.WriteWriteStateReceived, Is.True, "Write did not receive write state");
    }

    [Test]
    public void Object_array_write_state_flows_through_late_bound_element([Values] bool fixedSize)
    {
        // Verifies write state propagation through two layers with mixed element shapes:
        //   outer: ArrayTypeInfoProvider<object[], object>
        //   inner: LateBindingTypeInfoProvider (object -> int or DBNull)
        //            int path  -> WriteStateTrackingProvider<int>         (per-element wrapped WriteState)
        //            null path -> PgSerializerOptions.UnspecifiedDBNullTypeInfo (different concrete info entirely)
        // The tracking int converter's IsDbNullValue and WriteCore must see the provider-produced
        // write state after passing through the array + LateBindingConverter layers, while the DBNull
        // slots must flow through without disturbing the non-null slots.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(b => b.EnableArrays(), new WriteStateTrackingResolverFactory(fixedSize, tracker));

        var param = new NpgsqlParameter<object[]>
        {
            ParameterName = "p",
            TypedValue = new object[] { 1, DBNull.Value, 2, DBNull.Value, 3 },
            DataTypeName = "integer[]"
        };
        BindAndWriteToMemory(param, options);

        Assert.That(tracker.IsDbNullWriteStateReceived, Is.True,
            "IsDbNullValue did not receive write state after array + late-bound object layers");
        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after array + late-bound object layers");
    }

    [Test]
    public void Object_write_state_upgrades_when_late_bound_inner_produces_at_bind()
    {
        // Verifies the bind-time writeState upgrade path: late-bound resolution returns a converter
        // whose provider does not pre-populate state, so writeState arrives at LateBindingConverter.BindValue
        // as a bare PgConcreteTypeInfo (non-IDisposable). The inner converter's BindValue then produces
        // state during bind, and LateBindingConverter must upgrade the outer writeState reference from the
        // bare PgConcreteTypeInfo to a wrapped LateBindingWriteState (IDisposable).
        // Exercises the disposability-conditional lifecycle rule: replacing a non-IDisposable reference
        // with an IDisposable wrapper is allowed because the original carries no disposal obligation.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(
            new WriteStateTrackingResolverFactory(fixedSize: false, tracker, produceProviderState: false, generatesWriteStateAtBind: true));

        var param = new NpgsqlParameter<object> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" };
        BindAndWriteToMemory(param, options);

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after late-bound bind-time upgrade");
    }

    [Test]
    public void Range_write_state_flows()
    {
        // Verifies write state propagation through a range composition:
        //   RangeConverter<int> -> tracking int subtype (BindValue populates writeState)
        // The range converter must carry each bound's subtype state into BeginNestedWrite so the subtype's
        // WriteCore observes the provider-produced sentinel.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(b => b.EnableRanges(), new RangeWriteStateTrackingResolverFactory(tracker));

        var param = new NpgsqlParameter<NpgsqlRange<int>>
            { ParameterName = "p", TypedValue = new NpgsqlRange<int>(1, 10), DataTypeName = "int4range" };
        BindAndWriteToMemory(param, options);

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after range -> subtype composition");
    }

    [Test]
    public void Multirange_write_state_flows()
    {
        // Verifies write state propagation through a multirange composition:
        //   MultirangeConverter<NpgsqlRange<int>[], NpgsqlRange<int>> -> RangeConverter<int> -> tracking int subtype
        // Three layers: multirange stores per-range state, range stores per-bound state, subtype populates bound state.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(b => { b.EnableRanges(); b.EnableMultiranges(); },
            new RangeWriteStateTrackingResolverFactory(tracker));

        var param = new NpgsqlParameter<NpgsqlRange<int>[]>
        {
            ParameterName = "p",
            TypedValue = new[] { new NpgsqlRange<int>(1, 10), new NpgsqlRange<int>(20, 30) },
            DataTypeName = "int4multirange"
        };
        BindAndWriteToMemory(param, options);

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after multirange -> range -> subtype composition");
    }

    [Test]
    public void Range_array_write_state_flows()
    {
        // Verifies write state propagation through an array-over-range composition:
        //   ArrayConverter<NpgsqlRange<int>[]> -> RangeConverter<int> -> tracking int subtype
        // The per-element array slot carries the range's WriteState, which itself nests the subtype state.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(b => { b.EnableArrays(); b.EnableRanges(); },
            new RangeWriteStateTrackingResolverFactory(tracker));

        var param = new NpgsqlParameter<NpgsqlRange<int>[]>
        {
            ParameterName = "p",
            TypedValue = new[] { new NpgsqlRange<int>(1, 10), new NpgsqlRange<int>(20, 30) },
            DataTypeName = "int4range[]"
        };
        BindAndWriteToMemory(param, options);

        Assert.That(tracker.WriteWriteStateReceived, Is.True,
            "Write did not receive write state after array -> range -> subtype composition");
    }

    [Test]
    public void Array_of_multirange_write_state_flows()
    {
        // Verifies write state propagation through an array-over-multirange composition (four layers):
        //   ArrayConverter<NpgsqlRange<int>[][]> -> MultirangeConverter -> RangeConverter<int> -> tracking int subtype
        // The deepest common composition shape — if any layer loses state, the subtype's WriteCore never sees it.
        var tracker = new WriteStateTracker();
        var options = BuildOptions(b => { b.EnableArrays(); b.EnableRanges(); b.EnableMultiranges(); },
            new RangeWriteStateTrackingResolverFactory(tracker));

        var param = new NpgsqlParameter<NpgsqlRange<int>[][]>
        {
            ParameterName = "p",
            TypedValue = new[]
            {
                new[] { new NpgsqlRange<int>(1, 10), new NpgsqlRange<int>(20, 30) },
                new[] { new NpgsqlRange<int>(40, 50) }
            },
            DataTypeName = "int4multirange[]"
        };
        BindAndWriteToMemory(param, options);

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
    public void Execute_disposes_write_state()
    {
        // Verifies that write state produced during ResolveTypeInfo (and carried through Bind/Write) is disposed
        // once the normal execution path finishes, via DisposeBindingState in the Write finally block.
        var tracker = new DisposalTracker();
        var options = BuildOptions(new DisposableWriteStateResolverFactory(tracker));

        var param = new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" };
        BindAndWriteToMemory(param, options);

        Assert.That(tracker.Disposed, Is.True, "provider-produced write state was not disposed after normal execution");
    }

    [Test]
    public void SchemaOnly_disposes_resolution_write_state()
    {
        // Verifies that write state produced during ResolveTypeInfo is disposed when Bind is skipped
        // (e.g. CommandBehavior.SchemaOnly), so provider-allocated state does not leak. The willBind=false
        // path is what the SchemaOnly command behaviour threads through to ResolveTypeInfo.
        var tracker = new DisposalTracker();
        var options = BuildOptions(new DisposableWriteStateResolverFactory(tracker));

        var param = new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" };
        param.ResolveTypeInfo(options, dbTypeResolver: null, willBind: false);

        Assert.That(tracker.Disposed, Is.True, "provider-produced write state was not disposed after SchemaOnly execution");
    }

    [Test]
    public void DisposeBindingState_disposes_provider_write_state_exactly_once()
    {
        // Stronger form of Execute_disposes_write_state — pins exact-once disposal. After a successful
        // Bind the produced writeState lives in _binding.BindState; resetting via Size setter (any setter
        // that invalidates the binding works) forces DisposeBindingState, which the execute path also
        // hits in Write's finally.
        var tracker = new DisposalCountTracker();
        var options = BuildOptions(new ThrowingDisposalResolverFactory(tracker));

        var param = new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" };
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        param.Bind(out _, out _);
        // Trigger DisposeBindingState by invalidating the binding (Size setter is the cleanest no-op invalidator).
        param.Size = 0;

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.EqualTo(1), "provider should have produced one writeState");
        Assert.That(states[0].DisposeCount, Is.EqualTo(1), "provider writeState should be disposed exactly once on binding teardown");
    }

    // No-connection options for tests that only need to drive Resolve+Bind. Builds a data source so the
    // production resolver chain build (EnableArrays/EnableRanges composing) still kicks in, then pulls
    // the chain out and pairs it with the minimal type catalog.
    static PgSerializerOptions BuildOptions(params PgTypeInfoResolverFactory[] factories)
        => BuildOptions(configure: null, factories);

    static PgSerializerOptions BuildOptions(Action<NpgsqlSlimDataSourceBuilder>? configure, params PgTypeInfoResolverFactory[] factories)
    {
        // Host is only consumed by Build()'s validation; we never connect, so any non-empty value works.
        var dsb = new NpgsqlSlimDataSourceBuilder("Host=localhost");
        configure?.Invoke(dsb);
        foreach (var f in factories)
            dsb.AddTypeInfoResolverFactory(f);
        using var dataSource = dsb.Build();
        return new PgSerializerOptions(PostgresMinimalDatabaseInfo.DefaultTypeCatalog, dataSource.Configuration.ResolverChain);
    }

    // Drives the full Resolve → Bind → Write cycle against an in-memory buffer. FlushMode.None means
    // ShouldFlush returns false, so async I/O never actually kicks in and Write completes synchronously
    // — propagation tests that exercise the WriteCore path can use this without a connection.
    static void BindAndWriteToMemory(NpgsqlParameter param, PgSerializerOptions options)
    {
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        param.Bind(out _, out _);

        var pgWriter = new PgWriter(new ArrayBufferWriter<byte>()).Init(PostgresMinimalDatabaseInfo.DefaultTypeCatalog);
        var task = param.Write(async: false, pgWriter, CancellationToken.None);
        Debug.Assert(task.IsCompletedSuccessfully, "Write should complete synchronously with FlushMode.None");
        task.GetAwaiter().GetResult();
    }

    [Test]
    public void Bind_throw_disposes_provider_write_state_exactly_once()
    {
        // Provider-produced writeState held by the parameter must be disposed (exactly once) when the
        // converter's BindValue throws. The throw is wrapped by NpgsqlParameter.Bind as InvalidCastException;
        // the inner PgConverter.Bind safety net is responsible for releasing the state before the wrap.
        var tracker = new DisposalCountTracker();
        var options = BuildOptions(new ThrowingDisposalResolverFactory(tracker, throwInBindValueAt: _ => true));

        var param = new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" };
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        Assert.Throws<InvalidCastException>(() => param.Bind(out _, out _));

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.EqualTo(1), "provider should have produced one writeState");
        Assert.That(states[0].DisposeCount, Is.EqualTo(1), "writeState should be disposed exactly once on BindValue throw");
    }

    [Test]
    public void IsDbNullValue_throw_disposes_provider_write_state_exactly_once()
    {
        // Custom IsDbNullValue runs ahead of BindValue and reads writeState by value, so its throw bypasses
        // PgConverter.Bind's inner safety net. Disposal falls to the outer parameter-layer catch in
        // BindParameterValue*. Pin that the provider-produced state is disposed exactly once.
        var tracker = new DisposalCountTracker();
        var options = BuildOptions(new ThrowingDisposalResolverFactory(tracker, throwInIsDbNullValueAt: _ => true));

        var param = new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 42, DataTypeName = "integer" };
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        Assert.Throws<InvalidCastException>(() => param.Bind(out _, out _));

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.EqualTo(1), "provider should have produced one writeState");
        Assert.That(states[0].DisposeCount, Is.EqualTo(1), "writeState should be disposed exactly once on IsDbNullValue throw");
    }

    [Test]
    public void Array_element_bind_throw_disposes_all_element_write_states_exactly_once()
    {
        // Mid-iteration element throw must cascade cleanup: every element writeState already populated by
        // the array provider's element walk should be disposed exactly once via ArrayConverterWriteState.Dispose.
        // The framework wrapper carries AnyWriteState=true unconditionally for state-producing element resolves,
        // so this is a regression check against losing that invariant.
        var tracker = new DisposalCountTracker();
        // Throw on the middle element so we have populated states on either side at throw time.
        var options = BuildOptions(b => b.EnableArrays(),
            new ThrowingDisposalResolverFactory(tracker, throwInBindValueAt: v => v == 2));

        var param = new NpgsqlParameter<int[]>
            { ParameterName = "p", TypedValue = new[] { 1, 2, 3 }, DataTypeName = "integer[]" };
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        Assert.Throws<InvalidCastException>(() => param.Bind(out _, out _));

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.GreaterThanOrEqualTo(1), "provider should have produced per-element writeStates");
        foreach (var s in states)
            Assert.That(s.DisposeCount, Is.EqualTo(1),
                $"each element writeState should be disposed exactly once (found {s.DisposeCount})");
    }

    [Test]
    public void Array_element_isdbnull_throw_disposes_all_element_write_states_exactly_once()
    {
        // IsDbNullValue throw is reached before the inner converter Bind safety net, so element states
        // must survive the throw via the outer ArrayConverterWriteState.Dispose cascade. Variable-size
        // element converter forces the per-element Bind path (rather than the fixed-size optional-bind
        // skip that would never invoke IsDbNullValue).
        var tracker = new DisposalCountTracker();
        var options = BuildOptions(b => b.EnableArrays(),
            new ThrowingDisposalResolverFactory(tracker, throwInIsDbNullValueAt: v => v == 2));

        var param = new NpgsqlParameter<int[]>
            { ParameterName = "p", TypedValue = new[] { 1, 2, 3 }, DataTypeName = "integer[]" };
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        Assert.Throws<InvalidCastException>(() => param.Bind(out _, out _));

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.GreaterThanOrEqualTo(1), "provider should have produced per-element writeStates");
        foreach (var s in states)
            Assert.That(s.DisposeCount, Is.EqualTo(1),
                $"each element writeState should be disposed exactly once on IsDbNullValue throw (found {s.DisposeCount})");
    }

    [Test]
    public async Task Composite_provider_backed_field_bind_throw_disposes_write_state_exactly_once()
    {
        // Provider-backed composite field: GetWriteInfo runs MakeConcreteForValue and the provider
        // produces an IDisposable writeState, stored in slot.WriteState before the per-field Bind. If the
        // field's BindValue throws, the inner PgConverter.Bind safety net is responsible for releasing
        // that slot's writeState; the outer CompositeConverter.WriteState.Dispose cascade should not
        // double-dispose. Exercises the slow path (non-Exact composite due to variable-size field).
        var tracker = new DisposalCountTracker();
        await using var adminConnection = await OpenConnectionAsync();
        var type = await TestUtil.GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new ThrowingDisposalResolverFactory(tracker, throwInBindValueAt: _ => true));
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

        Assert.ThrowsAsync<InvalidCastException>(async () => await cmd.ExecuteNonQueryAsync());

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.GreaterThanOrEqualTo(1), "provider should have produced writeState for the composite field");
        foreach (var s in states)
            Assert.That(s.DisposeCount, Is.EqualTo(1),
                $"composite field writeState should be disposed exactly once on BindValue throw (found {s.DisposeCount})");
    }

    [Test]
    public async Task Composite_provider_backed_field_isdbnull_throw_disposes_write_state_exactly_once()
    {
        // Provider-backed composite field whose converter's IsDbNullValue throws after the provider has
        // produced state. IsDbNullValue takes writeState by value, so the inner Bind safety net never
        // engages — the throw must be absorbed by the composite wrapper's Dispose cascade. Pins that the
        // wrapper still iterates the slot (i.e. AnyWriteState reflects state stored before the throw).
        var tracker = new DisposalCountTracker();
        await using var adminConnection = await OpenConnectionAsync();
        var type = await TestUtil.GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new ThrowingDisposalResolverFactory(tracker, throwInIsDbNullValueAt: _ => true));
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

        Assert.ThrowsAsync<InvalidCastException>(async () => await cmd.ExecuteNonQueryAsync());

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.GreaterThanOrEqualTo(1), "provider should have produced writeState for the composite field");
        foreach (var s in states)
            Assert.That(s.DisposeCount, Is.EqualTo(1),
                $"composite field writeState should be disposed exactly once on IsDbNullValue throw (found {s.DisposeCount})");
    }

    [Test]
    public void Wrapping_converter_chain_no_double_dispose_on_inner_throw()
    {
        // Range over a state-producing subtype whose Bind throws — the inner safety net (subtype Bind),
        // the range converter's own Bind envelope, and the outer parameter-layer catch all see the same
        // bindState ref chain. Pinning exactly-once dispose on each produced subtype state guards against
        // the "transparent wrapper double-dispose" regression: the safety net's `current is not null` gate
        // is the load-bearing piece here.
        var tracker = new DisposalCountTracker();
        var options = BuildOptions(b => b.EnableRanges(),
            new ThrowingRangeResolverFactory(tracker, throwInBindValueAt: _ => true));

        var param = new NpgsqlParameter<NpgsqlRange<int>>
            { ParameterName = "p", TypedValue = new NpgsqlRange<int>(1, 10), DataTypeName = "int4range" };
        param.ResolveTypeInfo(options, dbTypeResolver: null);
        Assert.Throws<InvalidCastException>(() => param.Bind(out _, out _));

        var states = tracker.Snapshot();
        Assert.That(states, Has.Count.GreaterThanOrEqualTo(1), "subtype should have produced bound writeStates");
        foreach (var s in states)
            Assert.That(s.DisposeCount, Is.EqualTo(1),
                $"each subtype writeState should be disposed exactly once across the wrapping chain (found {s.DisposeCount})");
    }

    sealed class WriteStateTracker
    {
        public bool IsDbNullWriteStateReceived;
        public bool WriteWriteStateReceived;
    }

    sealed class DisposalCountState : IDisposable
    {
        public int DisposeCount;
        public void Dispose() => Interlocked.Increment(ref DisposeCount);
    }

    sealed class DisposalCountTracker
    {
        readonly List<DisposalCountState> _states = new();

        public DisposalCountState NewState()
        {
            var s = new DisposalCountState();
            lock (_states)
                _states.Add(s);
            return s;
        }

        public List<DisposalCountState> Snapshot()
        {
            lock (_states)
                return new List<DisposalCountState>(_states);
        }
    }

    /// Throwable, state-producing int converter for disposal contract tests. Non-fixed-size + HandleDbNull
    /// makes the framework safety net run (IsBindOptional=false, IsDbNullValue called).
    sealed class ThrowingDisposalConverter : PgBufferedConverter<int>
    {
        readonly Func<int, bool>? _throwInBindValueAt;
        readonly Func<int, bool>? _throwInIsDbNullValueAt;

        public ThrowingDisposalConverter(Func<int, bool>? throwInBindValueAt, Func<int, bool>? throwInIsDbNullValueAt)
        {
            _throwInBindValueAt = throwInBindValueAt;
            _throwInIsDbNullValueAt = throwInIsDbNullValueAt;
            // HandleDbNull is required for IsDbNullValue to be consulted; safe to keep on for non-throwing paths too.
            HandleDbNull = true;
        }

        public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
            // UpperBound + HandleDbNull → IsBindOptional=false, ensuring BindValue + IsDbNullValue run.
            => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.Create(Size.CreateUpperBound(sizeof(int))) };

        protected override bool IsDbNullValue(int value, object? writeState)
        {
            if (_throwInIsDbNullValueAt is { } pred && pred(value))
                throw new InvalidOperationException($"test: IsDbNullValue throw at value={value}");
            return false;
        }

        public override int Read(PgReader reader) => reader.ReadInt32();
        public override void Write(PgWriter writer, int value) => writer.WriteInt32(value);

        protected override Size BindValue(in BindContext context, int value, ref object? writeState)
        {
            if (_throwInBindValueAt is { } pred && pred(value))
                throw new InvalidOperationException($"test: BindValue throw at value={value}");
            return sizeof(int);
        }
    }

    sealed class ThrowingDisposalProvider : PgConcreteTypeInfoProvider<int>
    {
        readonly PgSerializerOptions _options;
        readonly DisposalCountTracker _tracker;
        readonly PgConverter<int> _converter;
        PgConcreteTypeInfo? _info;

        public ThrowingDisposalProvider(PgSerializerOptions options, DisposalCountTracker tracker, PgConverter<int> converter)
        {
            _options = options;
            _tracker = tracker;
            _converter = converter;
        }

        PgConcreteTypeInfo GetOrCreate()
            => _info ??= PgConcreteTypeInfo.Create(_options, _converter, _options.GetCanonicalTypeId(DataTypeNames.Int4));

        protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId) => GetOrCreate();

        protected override PgConcreteTypeInfo GetForValueCore(ProviderValueContext context, int value, ref object? writeState)
        {
            writeState = _tracker.NewState();
            return GetOrCreate();
        }
    }

    sealed class ThrowingDisposalResolverFactory : PgTypeInfoResolverFactory
    {
        readonly DisposalCountTracker _tracker;
        readonly Func<int, bool>? _throwInBindValueAt;
        readonly Func<int, bool>? _throwInIsDbNullValueAt;

        public ThrowingDisposalResolverFactory(DisposalCountTracker tracker,
            Func<int, bool>? throwInBindValueAt = null, Func<int, bool>? throwInIsDbNullValueAt = null)
        {
            _tracker = tracker;
            _throwInBindValueAt = throwInBindValueAt;
            _throwInIsDbNullValueAt = throwInIsDbNullValueAt;
        }

        public override IPgTypeInfoResolver CreateResolver() => new Resolver(_tracker, _throwInBindValueAt, _throwInIsDbNullValueAt);
        public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

        sealed class Resolver(DisposalCountTracker tracker, Func<int, bool>? throwInBindValueAt, Func<int, bool>? throwInIsDbNullValueAt) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4 && (type == typeof(int) || type is null))
                {
                    var converter = new ThrowingDisposalConverter(throwInBindValueAt, throwInIsDbNullValueAt);
                    return PgProviderTypeInfo.Create(options,
                        new ThrowingDisposalProvider(options, tracker, converter),
                        DataTypeNames.Int4);
                }
                return null;
            }
        }

        sealed class ArrayResolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName != DataTypeNames.Int4.ToArrayName())
                    return null;

                var elementInfo = options.GetTypeInfo(typeof(int), DataTypeNames.Int4);
                if (elementInfo is not PgProviderTypeInfo providerTypeInfo)
                    return null;

                return PgProviderTypeInfo.Create(options,
                    new ArrayTypeInfoProvider<int[], int>(providerTypeInfo, typeof(int[])),
                    dataTypeName);
            }
        }
    }

    sealed class ThrowingRangeResolverFactory : PgTypeInfoResolverFactory
    {
        readonly DisposalCountTracker _tracker;
        readonly Func<int, bool>? _throwInBindValueAt;

        public ThrowingRangeResolverFactory(DisposalCountTracker tracker, Func<int, bool>? throwInBindValueAt)
        {
            _tracker = tracker;
            _throwInBindValueAt = throwInBindValueAt;
        }

        public override IPgTypeInfoResolver CreateResolver() => new NoOpResolver();
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;
        public override IPgTypeInfoResolver? CreateRangeResolver() => new RangeResolver(_tracker, _throwInBindValueAt);

        sealed class NoOpResolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options) => null;
        }

        sealed class RangeResolver(DisposalCountTracker tracker, Func<int, bool>? throwInBindValueAt) : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (dataTypeName == DataTypeNames.Int4Range && (type == typeof(NpgsqlRange<int>) || type is null))
                {
                    // Subtype produces state from BindValue (writeState parameter is the tracker source);
                    // RangeConverter wraps each bound's subtype state into its own WriteState wrapper.
                    var subtype = new StateProducingSubtypeConverter(tracker, throwInBindValueAt);
                    var range = PgConverterFactory.CreateRangeConverter(subtype, options);
                    return PgConcreteTypeInfo.Create(options, range, options.GetCanonicalTypeId(DataTypeNames.Int4Range));
                }
                return null;
            }
        }

        sealed class StateProducingSubtypeConverter : PgBufferedConverter<int>
        {
            readonly DisposalCountTracker _tracker;
            readonly Func<int, bool>? _throwInBindValueAt;

            public StateProducingSubtypeConverter(DisposalCountTracker tracker, Func<int, bool>? throwInBindValueAt)
            {
                _tracker = tracker;
                _throwInBindValueAt = throwInBindValueAt;
            }

            public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
                => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.Create(Size.CreateUpperBound(sizeof(int))) };

            public override int Read(PgReader reader) => reader.ReadInt32();
            public override void Write(PgWriter writer, int value) => writer.WriteInt32(value);

            protected override Size BindValue(in BindContext context, int value, ref object? writeState)
            {
                // Produce state on entry so the outer wrapper has something to cascade-dispose on a later
                // throw — order matters: assign before any throw so the framework safety net can reach it.
                writeState = _tracker.NewState();
                if (_throwInBindValueAt is { } pred && pred(value))
                    throw new InvalidOperationException($"test: subtype BindValue throw at value={value}");
                return sizeof(int);
            }
        }
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

        public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
            => ConverterDescriptor.Invariant with { BufferRequirements = _fixedSize ? BufferRequirements.CreateFixedSize(sizeof(int)) : BufferRequirements.Create(Size.CreateUpperBound(sizeof(int))) };

        protected override bool IsDbNullValue(int value, object? writeState)
        {
            if (writeState is not null)
                _tracker.IsDbNullWriteStateReceived = true;
            return false;
        }

        public override int Read(PgReader reader) => reader.ReadInt32();

        public override void Write(PgWriter writer, int value)
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
            => _concreteTypeInfo ??= PgConcreteTypeInfo.Create(options, new WriteStateTrackingConverter(fixedSize, tracker, generatesWriteStateAtBind), options.GetCanonicalTypeId(DataTypeNames.Int4));

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
                    return PgProviderTypeInfo.Create(options, new WriteStateTrackingProvider(options, fixedSize, tracker, produceProviderState, generatesWriteStateAtBind), DataTypeNames.Int4);

                // object->int4 goes through LateBindingTypeInfoProvider which delegates back to the int resolver above,
                // letting us exercise write-state propagation across the object (late-bound) element layer.
                if (dataTypeName == DataTypeNames.Int4 && type == typeof(object))
                    return PgProviderTypeInfo.Create(options, new LateBindingTypeInfoProvider(options, options.GetCanonicalTypeId(DataTypeNames.Int4)), DataTypeNames.Int4);

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

                    return PgProviderTypeInfo.Create(options,
                        new ArrayTypeInfoProvider<object[], object>(objectElementProviderTypeInfo, typeof(object[])),
                        dataTypeName);
                }

                var elementInfo = options.GetTypeInfo(typeof(int), DataTypeNames.Int4);
                if (elementInfo is not PgProviderTypeInfo providerTypeInfo)
                    return null;

                return PgProviderTypeInfo.Create(options,
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
        public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
            => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int)) };

        public override int Read(PgReader reader) => reader.ReadInt32();
        public override void Write(PgWriter writer, int value) => writer.WriteInt32(value);
    }

    sealed class DisposableWriteStateProvider(PgSerializerOptions options, DisposalTracker tracker) : PgConcreteTypeInfoProvider<int>
    {
        PgConcreteTypeInfo? _concreteTypeInfo;

        PgConcreteTypeInfo GetOrCreate()
            => _concreteTypeInfo ??= PgConcreteTypeInfo.Create(options, new DisposableWriteStateConverter(), options.GetCanonicalTypeId(DataTypeNames.Int4));

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
                    return PgProviderTypeInfo.Create(options, new DisposableWriteStateProvider(options, tracker), DataTypeNames.Int4);

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
                    return PgConcreteTypeInfo.Create(options, range, options.GetCanonicalTypeId(DataTypeNames.Int4Range));
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
                    var rangeInfo = PgConcreteTypeInfo.Create(options, range, options.GetCanonicalTypeId(DataTypeNames.Int4Range));
                    var arrayConverter = ArrayConverter<NpgsqlRange<int>[]>.CreateArrayBased<NpgsqlRange<int>>(rangeInfo, typeof(NpgsqlRange<int>[]));
                    return PgConcreteTypeInfo.Create(options, arrayConverter, options.GetCanonicalTypeId(DataTypeNames.Int4Range.ToArrayName()));
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
                    return PgConcreteTypeInfo.Create(options, multirange, options.GetCanonicalTypeId(DataTypeNames.Int4Multirange));
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
                    var multirangeInfo = PgConcreteTypeInfo.Create(options, multirange, options.GetCanonicalTypeId(DataTypeNames.Int4Multirange));
                    var arrayConverter = ArrayConverter<NpgsqlRange<int>[][]>.CreateArrayBased<NpgsqlRange<int>[]>(multirangeInfo, typeof(NpgsqlRange<int>[][]));
                    return PgConcreteTypeInfo.Create(options, arrayConverter, options.GetCanonicalTypeId(DataTypeNames.Int4Multirange.ToArrayName()));
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
                    return PgConcreteTypeInfo.Create(options, converter, options.GetCanonicalTypeId(DataTypeNames.Int4));
                }
                return null;
            }
        }
    }
}
