using System;

namespace Npgsql.OpenTelemetry;

sealed class NpgsqlTracingInstrumentation : IDisposable
{
    readonly NpgsqlTracingOptions _originalOptions;
    
    public NpgsqlTracingInstrumentation(NpgsqlTracingOptions options)
    {
        _originalOptions = NpgsqlActivitySource.Options;
        NpgsqlActivitySource.Options = options;
    }

    public void Dispose() => NpgsqlActivitySource.Options = _originalOptions;
}
