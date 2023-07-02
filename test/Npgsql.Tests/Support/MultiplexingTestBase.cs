using System.Collections.Concurrent;
using NUnit.Framework;

namespace Npgsql.Tests;

[TestFixture(MultiplexingMode.NonMultiplexing)]
[TestFixture(MultiplexingMode.Multiplexing)]
public abstract class MultiplexingTestBase : TestBase
{
    protected bool IsMultiplexing => MultiplexingMode == MultiplexingMode.Multiplexing;

    protected MultiplexingMode MultiplexingMode { get; }

    readonly ConcurrentDictionary<(string ConnString, bool IsMultiplexing), string> _connStringCache
        = new();

    public override string ConnectionString { get; }

    protected MultiplexingTestBase(MultiplexingMode multiplexingMode)
    {
        MultiplexingMode = multiplexingMode;

        // If the test requires multiplexing to be on or off, use a small cache to avoid reparsing and
        // regenerating the connection string every time
        ConnectionString = _connStringCache.GetOrAdd((base.ConnectionString, IsMultiplexing),
            tup => new NpgsqlConnectionStringBuilder(tup.ConnString)
            {
                Multiplexing = tup.IsMultiplexing
            }.ToString());
    }
}

public enum MultiplexingMode
{
    NonMultiplexing,
    Multiplexing
}
