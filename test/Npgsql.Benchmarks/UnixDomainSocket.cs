using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks;

public class UnixDomainSocket
{
    readonly NpgsqlConnection _tcpipConn;
    readonly NpgsqlCommand _tcpipCmd;
    readonly NpgsqlConnection _unixConn;
    readonly NpgsqlCommand _unixCmd;

    public UnixDomainSocket()
    {
        _tcpipConn = BenchmarkEnvironment.OpenConnection();
        _tcpipCmd = new NpgsqlCommand("SELECT @p", _tcpipConn);
        _tcpipCmd.Parameters.AddWithValue("p", new string('x', 10000));

        var port = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString).Port;
        var candidateDirectories = new[] { "/var/run/postgresql", "/tmp" };
        var dir = candidateDirectories.FirstOrDefault(d => File.Exists(Path.Combine(d, $".s.PGSQL.{port}")));
        if (dir == null)
            throw new Exception("No PostgreSQL unix domain socket was found");

        var connString = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString)
        {
            Host = dir
        }.ToString();
        _unixConn = new NpgsqlConnection(connString);
        _unixConn.Open();
        _unixCmd = new NpgsqlCommand("SELECT @p", _unixConn);
        _unixCmd.Parameters.AddWithValue("p", new string('x', 10000));
    }

    [Benchmark(Baseline = true)]
    public string Tcpip() => (string)_tcpipCmd.ExecuteScalar()!;

    [Benchmark]
    public string UnixDomain() => (string)_unixCmd.ExecuteScalar()!;
}