# Metrics with event counters

Npgsql supports reporting aggregated metrics which provide snapshots on its state and activities at a given point. These can be especially useful for diagnostics issues such as connection leaks, or doing general performance analysis Metrics are reported via the standard .NET event counters feature; it's recommended to read [this blog post](https://devblogs.microsoft.com/dotnet/introducing-diagnostics-improvements-in-net-core-3-0/) for a quick overview of how counters work.

To collect event counters, [install the `dotnet-counters` tool](https://docs.microsoft.com/dotnet/core/diagnostics/dotnet-counters). Then, find out your process PID, and run it as follows:

```output
dotnet counters monitor Npgsql -p <PID>
```

`dotnet-counters` will now attach to your running process and start reporting continuous counter data:

```output
[Npgsql]
Average commands per multiplexing batch                      NaN
Average write time per multiplexing batch (us) (us)          NaN
Busy Connections                                               4
Bytes Read (Count / 1 sec)                             1,874,863
Bytes Written (Count / 1 sec)                          1,546,830
Command Rate (Count / 1 sec)                              18,199
Connection Pools                                               1
Current Commands                                               5
Failed Commands                                                0
Idle Connections                                               5
Prepared Commands Ratio (%)                                    0
Total Commands                                           372,918
```
