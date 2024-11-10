using System;
using System.Diagnostics;

namespace Npgsql;

/// <summary>
/// A builder to configure Npgsql's support for OpenTelemetry tracing.
/// </summary>
public sealed class NpgsqlTracingOptionsBuilder
{
    Func<NpgsqlCommand, bool>? _commandFilter;
    Func<NpgsqlBatch, bool>? _batchFilter;
    Action<Activity, NpgsqlCommand>? _commandEnrichmentCallback;
    Action<Activity, NpgsqlBatch>? _batchEnrichmentCallback;
    Func<NpgsqlCommand, string?>? _commandSpanNameProvider;
    Func<NpgsqlBatch, string?>? _batchSpanNameProvider;
    bool _enableFirstResponseEvent = true;
    bool _enablePhysicalOpenTracing = true;

    Func<string, CopyOperationType, bool>? _copyOperationFilter;
    Action<Activity, string, CopyOperationType>? _copyOperationEnrichmentCallback;
    Func<string, CopyOperationType, string?>? _copyOperationSpanNameProvider;

    internal NpgsqlTracingOptionsBuilder()
    {
    }

    /// <summary>
    /// Configures a filter function that determines whether to emit tracing information for an <see cref="NpgsqlCommand"/>.
    /// By default, tracing information is emitted for all commands.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureCommandFilter(Func<NpgsqlCommand, bool>? commandFilter)
    {
        _commandFilter = commandFilter;
        return this;
    }

    /// <summary>
    /// Configures a filter function that determines whether to emit tracing information for an <see cref="NpgsqlBatch"/>.
    /// By default, tracing information is emitted for all batches.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureBatchFilter(Func<NpgsqlBatch, bool>? batchFilter)
    {
        _batchFilter = batchFilter;
        return this;
    }

    /// <summary>
    /// Configures a callback that can enrich the <see cref="Activity"/> emitted for the given <see cref="NpgsqlCommand"/>.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureCommandEnrichmentCallback(Action<Activity, NpgsqlCommand>? commandEnrichmentCallback)
    {
        _commandEnrichmentCallback = commandEnrichmentCallback;
        return this;
    }

    /// <summary>
    /// Configures a callback that can enrich the <see cref="Activity"/> emitted for the given <see cref="NpgsqlBatch"/>.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureBatchEnrichmentCallback(Action<Activity, NpgsqlBatch>? batchEnrichmentCallback)
    {
        _batchEnrichmentCallback = batchEnrichmentCallback;
        return this;
    }

    /// <summary>
    /// Configures a callback that provides the tracing span's name for an <see cref="NpgsqlCommand"/>. If <c>null</c>, the default standard
    /// span name is used, which is the database name.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureCommandSpanNameProvider(Func<NpgsqlCommand, string?>? commandSpanNameProvider)
    {
        _commandSpanNameProvider = commandSpanNameProvider;
        return this;
    }

    /// <summary>
    /// Configures a callback that provides the tracing span's name for an <see cref="NpgsqlBatch"/>. If <c>null</c>, the default standard
    /// span name is used, which is the database name.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureBatchSpanNameProvider(Func<NpgsqlBatch, string?>? batchSpanNameProvider)
    {
        _batchSpanNameProvider = batchSpanNameProvider;
        return this;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable the "time-to-first-read" event.
    /// Default is true to preserve existing behavior.
    /// </summary>
    public NpgsqlTracingOptionsBuilder EnableFirstResponseEvent(bool enable = true)
    {
        _enableFirstResponseEvent = enable;
        return this;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to trace physical connection open.
    /// Default is true to preserve existing behavior.
    /// </summary>
    public NpgsqlTracingOptionsBuilder EnablePhysicalOpenTracing(bool enable = true)
    {
        _enablePhysicalOpenTracing = enable;
        return this;
    }

    /// <summary>
    /// Configures a filter function that determines whether to emit tracing information for a copy operation.
    /// By default, tracing information is emitted for all copy operations.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureCopyOperationFilter(Func<string, CopyOperationType, bool>? copyOperationFilter)
    {
        _copyOperationFilter = copyOperationFilter;
        return this;
    }

    /// <summary>
    /// Configures a callback that can enrich the <see cref="Activity"/> emitted for a given copy operation.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureCopyOperationEnrichmentCallback(Action<Activity, string, CopyOperationType>? copyOperationEnrichmentCallback)
    {
        _copyOperationEnrichmentCallback = copyOperationEnrichmentCallback;
        return this;
    }

    /// <summary>
    /// Configures a callback that provides the tracing span's name for a copy operation. If <c>null</c>, the default standard
    /// span name is used, which is the database name.
    /// </summary>
    public NpgsqlTracingOptionsBuilder ConfigureCopyOperationSpanNameProvider(Func<string, CopyOperationType, string?>? copyOperationSpanNameProvider)
    {
        _copyOperationSpanNameProvider = copyOperationSpanNameProvider;
        return this;
    }

    internal NpgsqlTracingOptions Build() => new()
    {
        CommandFilter = _commandFilter,
        BatchFilter = _batchFilter,
        CommandEnrichmentCallback = _commandEnrichmentCallback,
        BatchEnrichmentCallback = _batchEnrichmentCallback,
        CommandSpanNameProvider = _commandSpanNameProvider,
        BatchSpanNameProvider = _batchSpanNameProvider,
        EnableFirstResponseEvent = _enableFirstResponseEvent,
        EnablePhysicalOpenTracing = _enablePhysicalOpenTracing,
        CopyOperationFilter = _copyOperationFilter,
        CopyOperationEnrichmentCallback = _copyOperationEnrichmentCallback,
        CopyOperationSpanNameProvider = _copyOperationSpanNameProvider
    };
}

sealed class NpgsqlTracingOptions
{
    internal Func<NpgsqlCommand, bool>? CommandFilter { get; init; }
    internal Func<NpgsqlBatch, bool>? BatchFilter { get; init; }
    internal Action<Activity, NpgsqlCommand>? CommandEnrichmentCallback { get; init; }
    internal Action<Activity, NpgsqlBatch>? BatchEnrichmentCallback { get; init; }
    internal Func<NpgsqlCommand, string?>? CommandSpanNameProvider { get; init; }
    internal Func<NpgsqlBatch, string?>? BatchSpanNameProvider { get; init; }
    internal bool EnableFirstResponseEvent { get; init; }
    internal bool EnablePhysicalOpenTracing { get; init; }
    internal Func<string, CopyOperationType, bool>? CopyOperationFilter { get; init; }
    internal Action<Activity, string, CopyOperationType>? CopyOperationEnrichmentCallback { get; init; }
    internal Func<string, CopyOperationType, string?>? CopyOperationSpanNameProvider { get; init; }
}
