# OpenTelemetry Metrics

Npgsql supports reporting aggregated metrics which provide snapshots on its state and activities at a given point. These can be especially useful for diagnostics issues such as connection leaks, or doing general performance analysis Metrics are reported via the standard .NET System.Diagnostics.Metrics API; [see these docs](https://learn.microsoft.com/dotnet/core/diagnostics/metrics) for more details. The Npgsql metrics implement the experimental [OpenTelemetry semantic conventions for database metrics](https://opentelemetry.io/docs/specs/semconv/database/database-metrics/) - adding some additional useful ones - and will evolve as that specification stabilizes.

> [!NOTE]
> Npgsql 10.0 changed the metrics names to align with the OpenTelemetry standard. The names shown below reflect the Npgsql 10 counters.
>
> Npgsql versions before 8.0, as well as TFMs under net6.0, emit metrics via the older Event Counters API instead of the new OpenTelemetry ones.

Metrics are usually collected and processed via tools such as [Prometheus](https://prometheus.io), and plotted on dashboards via tools such as [Grafana](https://grafana.com). Configuring .NET to emit metrics to these tools is beyond the scope of this documentation, but you can use the command-line tool `dotnet-counters` to quickly test Npgsql's support. To collect metrics via `dotnet-counters`, [install the `dotnet-counters` tool](https://docs.microsoft.com/dotnet/core/diagnostics/dotnet-counters). Then, find out your process PID, and run it as follows:

```output
dotnet counters monitor Npgsql -p <PID>
```

`dotnet-counters` will now attach to your running process and start reporting continuous counter data:

```output
[Npgsql]
    db.client.operation.npgsql.bytes_read (By / 1 sec)
        db.client.connection.pool.name=CustomersDB                                          1,020
    db.client.operation.npgsql.bytes_written (By / 1 sec)
        db.client.connection.pool.name=CustomersDB                                            710
    db.client.operation.duration (s)
        db.client.connection.pool.name=CustomersDB,Percentile=50                                0.001
        db.client.connection.pool.name=CustomersDB,Percentile=95                                0.001
        db.client.connection.pool.name=CustomersDB,Percentile=99                                0.001
    db.client.operation.npgsql.executing ({command})
        db.client.connection.pool.name=CustomersDB                                              2
    db.client.operation.npgsql.prepared_ratio
        db.client.connection.pool.name=CustomersDB                                              0
    db.client.connection.max ({connection})
        db.client.connection.pool.name=CustomersDB                                            100
    db.client.connection.count ({connection})
        db.client.connection.pool.name=CustomersDB,state=idle                                   3
        db.client.connection.pool.name=CustomersDB,state=used                                   2
```

Note that Npgsql emits multiple *dimensions* with the metrics, e.g. the connection states (idle or used). In addition, an identifier for the connection pool - or data source - is emitted with every metric, allowing you to separately track e.g. multiple databases accessed in the same applications. By default, the `pool.name` will be the connection string, but it can be useful to give your data sources a name for easier and more consistent tracking:

```csharp
var builder = new NpgsqlDataSourceBuilder("Host=localhost;Username=test;Password=test")
{
    Name = "CustomersDB"
};
await using var dataSource = builder.Build();
```
