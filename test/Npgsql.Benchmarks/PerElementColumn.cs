using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Npgsql.Benchmarks;

[AttributeUsage(AttributeTargets.Class)]
sealed class PerElementMeanAttribute() : ColumnConfigBaseAttribute(PerElementMean.Instance);

[AttributeUsage(AttributeTargets.Method)]
sealed class PerElementAttribute(int count) : Attribute
{
    public int Count { get; } = count;
}

sealed class PerElementMean : IColumn
{
    public static readonly IColumn Instance = new PerElementMean();

    public string Id => nameof(PerElementMean);
    public string ColumnName => "Mean/elem";
    public string Legend => "Mean per array element (Mean / PerElement count)";
    public UnitType UnitType => UnitType.Time;
    public ColumnCategory Category => ColumnCategory.Statistics;
    public int PriorityInCategory => 0;
    public bool IsNumeric => true;
    public bool AlwaysShow => true;
    public bool IsAvailable(Summary summary) => true;
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        => GetValue(summary, benchmarkCase, SummaryStyle.Default);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        var attr = benchmarkCase.Descriptor.WorkloadMethod.GetCustomAttribute<PerElementAttribute>();
        if (attr is null)
            return "-";
        var report = summary[benchmarkCase];
        if (report?.ResultStatistics is null)
            return "-";
        var perElem = report.ResultStatistics.Mean / attr.Count;
        return $"{perElem:F2} ns";
    }
}
