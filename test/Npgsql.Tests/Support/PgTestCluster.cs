using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests.Support;

/// <summary>
/// A throwaway PostgreSQL cluster, created with the PostgreSQL binaries in the directory that the
/// <c>NPGSQL_TEST_PG_BIN</c> environment variable points to. It is meant for tests that can't run against the shared test
/// server, e.g. because they promote a standby or shut the server down. Tests using it are ignored when the variable isn't
/// set.
/// </summary>
sealed class PgTestCluster : IAsyncDisposable
{
    const string BinDirectoryEnvironmentVariable = "NPGSQL_TEST_PG_BIN";
    const string UserName = "npgsql_tests";

    // CI kills a test after 30 seconds, so a hanging step has to fail well before that to report anything
    static readonly TimeSpan PgCtlTimeout = TimeSpan.FromSeconds(10);
    static readonly TimeSpan ProcessTimeout = TimeSpan.FromSeconds(15);

    readonly string _binDirectory;
    readonly string _rootDirectory;
    readonly PgTestCluster? _primary;
    readonly List<PgTestCluster> _standbys = [];

    internal string DataDirectory { get; }

    /// <summary>
    /// The port the cluster listens on. It is chosen anew each time the cluster starts.
    /// </summary>
    internal int Port { get; private set; }

    string LogFile => Path.Combine(_rootDirectory, Path.GetFileName(DataDirectory) + ".log");

    /// <summary>
    /// A connection string for the cluster's <c>postgres</c> database, without pooling, so that no connections stay
    /// open when the cluster is stopped.
    /// </summary>
    internal string ConnectionString => new NpgsqlConnectionStringBuilder
    {
        Host = "127.0.0.1",
        Port = Port,
        Username = UserName,
        Database = "postgres",
        Pooling = false
    }.ConnectionString;

    PgTestCluster(string binDirectory, string rootDirectory, PgTestCluster? primary, string dataDirectory)
    {
        _binDirectory = binDirectory;
        _rootDirectory = rootDirectory;
        _primary = primary;
        DataDirectory = dataDirectory;
    }

    /// <summary>
    /// Creates and starts a primary that allows physical and logical replication.
    /// </summary>
    internal static async Task<PgTestCluster> StartPrimary()
    {
        var binDirectory = Environment.GetEnvironmentVariable(BinDirectoryEnvironmentVariable);
        if (string.IsNullOrEmpty(binDirectory))
            Assert.Ignore($"Set {BinDirectoryEnvironmentVariable} to the directory with the PostgreSQL binaries to run this test");

        var rootDirectory = Path.Combine(Path.GetTempPath(), "npgsql-test-cluster-" + Guid.NewGuid().ToString("N"));
        var dataDirectory = Path.Combine(rootDirectory, "primary");
        Directory.CreateDirectory(rootDirectory);
        var cluster = new PgTestCluster(binDirectory, rootDirectory, primary: null, dataDirectory);
        try
        {
            await cluster.Run("initdb", "-D", dataDirectory, "-U", UserName, "--auth=trust", "--encoding=UTF8", "--no-locale", "--no-sync");
            await File.AppendAllTextAsync(Path.Combine(dataDirectory, "postgresql.conf"),
                """

                listen_addresses = '127.0.0.1'
                unix_socket_directories = ''
                wal_level = logical
                max_wal_senders = 10
                max_replication_slots = 10
                fsync = off
                """);
            await cluster.Start();
            return cluster;
        }
        catch
        {
            await cluster.DisposeAsync();
            throw;
        }
    }

    /// <summary>
    /// Creates a streaming standby of this cluster with pg_basebackup and starts it.
    /// </summary>
    internal async Task<PgTestCluster> StartStandby()
    {
        var dataDirectory = Path.Combine(_rootDirectory, "standby-" + Guid.NewGuid().ToString("N")[..8]);
        var standby = new PgTestCluster(_binDirectory, _rootDirectory, primary: this, dataDirectory);
        _standbys.Add(standby);
        await Run("pg_basebackup", "-h", "127.0.0.1", "-p", Port.ToString(), "-U", UserName, "-D", dataDirectory,
            "--write-recovery-conf", "--wal-method=stream", "--checkpoint=fast", "--no-sync");
        await standby.Start();
        return standby;
    }

    /// <summary>
    /// Starts the cluster on a free port.
    /// </summary>
    internal async Task Start()
    {
        for (var attempt = 1; ; attempt++)
        {
            // We choose the port right before starting so that nothing else grabs it in between. pg_basebackup writes
            // primary_conninfo to postgresql.auto.conf, so the port goes there too; the last setting wins.
            Port = GetFreePort();
            await File.AppendAllTextAsync(Path.Combine(DataDirectory, "postgresql.auto.conf"), $"\nport = {Port}\n");
            try
            {
                await RunPgCtl("start", "-l", LogFile);
                return;
            }
            catch (InvalidOperationException) when (attempt < 3 && ReadLog().Contains("could not bind"))
            {
                // Someone else took the port after all
            }
        }
    }

    /// <summary>
    /// Stops the cluster with the given shutdown mode (<c>smart</c>, <c>fast</c> or <c>immediate</c>).
    /// </summary>
    internal Task Stop(string mode = "fast") => RunPgCtl("stop", "-m", mode);

    internal Task Promote() => RunPgCtl("promote");

    async Task RunPgCtl(string command, params string[] arguments)
    {
        try
        {
            // pg_ctl start leaves the server running in the background, which inherits redirected output handles and keeps
            // them open, so we don't redirect its output (-s keeps it quiet unless something goes wrong) and report the
            // server log instead.
            await Run(redirectOutput: command != "start", "pg_ctl",
                [command, "-D", DataDirectory, "-w", "-t", ((int)PgCtlTimeout.TotalSeconds).ToString(), "-s", .. arguments]);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"{e.Message}{Environment.NewLine}Server log:{Environment.NewLine}{ReadLog()}", e);
        }
    }

    // The server may still have the log open, so we allow others to write to it
    string ReadLog()
    {
        try
        {
            using var stream = new FileStream(LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        catch (Exception e)
        {
            return $"(not available: {e.Message})";
        }
    }

    Task Run(string program, params string[] arguments) => Run(redirectOutput: true, program, arguments);

    async Task Run(bool redirectOutput, string program, string[] arguments)
    {
        var startInfo = new ProcessStartInfo(Path.Combine(_binDirectory, program))
        {
            UseShellExecute = false,
            RedirectStandardOutput = redirectOutput,
            RedirectStandardError = redirectOutput
        };
        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        using var cts = new CancellationTokenSource(ProcessTimeout);
        using var process = Process.Start(startInfo)!;
        var output = redirectOutput ? process.StandardOutput.ReadToEndAsync(cts.Token) : Task.FromResult("");
        var error = redirectOutput ? process.StandardError.ReadToEndAsync(cts.Token) : Task.FromResult("");
        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException($"{program} didn't finish within {ProcessTimeout}");
        }

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"{program} failed with exit code {process.ExitCode}: {await output}{await error}");
    }

    static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public async ValueTask DisposeAsync()
    {
        // The standbys' directories live below the primary's
        foreach (var standby in _standbys)
            await standby.DisposeAsync();
        _standbys.Clear();

        if (File.Exists(Path.Combine(DataDirectory, "postmaster.pid")))
        {
            try
            {
                await Stop("immediate");
            }
            catch
            {
                // Best effort
            }
        }

        // On Windows, the server's processes may keep their files open for a moment after it stopped
        var directory = _primary is null ? _rootDirectory : DataDirectory;
        for (var attempt = 0; attempt < 20 && Directory.Exists(directory); attempt++)
        {
            try
            {
                Directory.Delete(directory, recursive: true);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                await Task.Delay(100);
            }
        }
    }
}
