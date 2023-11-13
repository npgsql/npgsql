using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

class ConnectionStringBuilderTests
{
    [Test]
    public void Basic()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        Assert.That(builder.Count, Is.EqualTo(0));
        Assert.That(builder.ContainsKey("server"), Is.True);
        builder.Host = "myhost";
        Assert.That(builder["host"], Is.EqualTo("myhost"));
        Assert.That(builder.Count, Is.EqualTo(1));
        Assert.That(builder.ConnectionString, Is.EqualTo("Host=myhost"));
        builder.Remove("HOST");
        Assert.That(builder["host"], Is.EqualTo(""));
        Assert.That(builder.Count, Is.EqualTo(0));
    }

    [Test]
    public void From_string()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.ConnectionString = "Host=myhost;EF Template Database=foo";
        Assert.That(builder.Host, Is.EqualTo("myhost"));
        Assert.That(builder.EntityTemplateDatabase, Is.EqualTo("foo"));
    }

    [Test]
    public void TryGetValue()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.ConnectionString = "Host=myhost";
        Assert.That(builder.TryGetValue("Host", out var value), Is.True);
        Assert.That(value, Is.EqualTo("myhost"));
        Assert.That(builder.TryGetValue("SomethingUnknown", out value), Is.False);
    }

    [Test]
    public void Remove()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.SslMode = SslMode.Require;
        Assert.That(builder["SSL Mode"], Is.EqualTo(SslMode.Require));
        builder.Remove("SSL Mode");
        Assert.That(builder.ConnectionString, Is.EqualTo(""));
        builder.CommandTimeout = 120;
        Assert.That(builder["Command Timeout"], Is.EqualTo(120));
        builder.Remove("Command Timeout");
        Assert.That(builder.ConnectionString, Is.EqualTo(""));
    }

    [Test]
    public void Clear()
    {
        var builder = new NpgsqlConnectionStringBuilder { Host = "myhost" };
        builder.Clear();
        Assert.That(builder.Count, Is.EqualTo(0));
        Assert.That(builder["host"], Is.EqualTo(""));
        Assert.That(builder.Host, Is.Null);
    }

    [Test]
    public void Removing_resets_to_default()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        Assert.That(builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
        builder.Port = 8;
        builder.Remove("Port");
        Assert.That(builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
    }

    [Test]
    public void Setting_to_null_resets_to_default()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        Assert.That(builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
        builder.Port = 8;
        builder["Port"] = null;
        Assert.That(builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
    }

    [Test]
    public void Enum()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.ConnectionString = "SslMode=Require";
        Assert.That(builder.SslMode, Is.EqualTo(SslMode.Require));
        Assert.That(builder.Count, Is.EqualTo(1));
    }

    [Test]
    public void Enum_insensitive()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.ConnectionString = "SslMode=require";
        Assert.That(builder.SslMode, Is.EqualTo(SslMode.Require));
        Assert.That(builder.Count, Is.EqualTo(1));
    }

    [Test]
    public void Clone()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        builder.Host = "myhost";
        var builder2 = builder.Clone();
        Assert.That(builder2.Host, Is.EqualTo("myhost"));
        Assert.That(builder2["Host"], Is.EqualTo("myhost"));
        Assert.That(builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
    }

    [Test]
    public void Conversion_error_throws()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        Assert.That(() => builder["Port"] = "hello",
            Throws.Exception.TypeOf<ArgumentException>().With.Message.Contains("Port"));
    }

    [Test]
    public void Invalid_connection_string_throws()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        Assert.That(() => builder.ConnectionString = "Server=127.0.0.1;User Id=npgsql_tests;Pooling:false",
            Throws.Exception.TypeOf<ArgumentException>());
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public void PgService_ViaEnvironmentVariable()
    {
        using var pgServiceVariable = SetEnvironmentVariable("PGSERVICE", "MyService");

        PgService_With((service, host, port, reverse) =>
        {
            var builder = new NpgsqlConnectionStringBuilder();
            builder.Host = host;
            if (port is not null) builder.Port = (int)port;
            return builder;
        });
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public void PgService_ViaConnectionString()
    {
        PgService_With((service, host, port, reverse) =>
        {
            var fields = new List<string>();
            if (service is not null) fields.Add($"Service={service}");
            if (host is not null) fields.Add($"Host={host}");
            if (port is not null) fields.Add($"Port={port}");
            if (reverse) fields.Reverse();
            return new NpgsqlConnectionStringBuilder(string.Join(";", fields));
        });
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public void PgService_ViaProperty()
    {
        PgService_With((service, host, port, reverse) =>
        {
            var builder = new NpgsqlConnectionStringBuilder();
            builder.Host = host;
            if (port is not null && reverse) builder.Port = (int)port;
            builder.Service = service;
            if (port is not null && !reverse) builder.Port = (int)port;
            return builder;
        });
    }

    private void PgService_With(Func<string?, string?, int?, bool, NpgsqlConnectionStringBuilder> factory)
    {
        // Nominal case
        var builder = factory("MyService", "MyHost", null, false);
        builder.PostProcessAndValidate();
        Assert.That(builder, Is.Not.Null);
        Assert.That(builder.Count, Is.EqualTo(2));

        // Missing service file is ignored
        var tempFile = Path.GetTempFileName();
        using var pgServiceFileVariable = SetEnvironmentVariable("PGSERVICEFILE", tempFile);
        builder = factory("MyService", "MyHost", null, false);
        builder.PostProcessAndValidate();
        Assert.That(builder, Is.Not.Null);
        Assert.That(builder.Count, Is.EqualTo(2));

        try
        {
            // Comments are ignored
            File.WriteAllText(tempFile, "# test");
            builder = factory("MyService", "MyHost", null, false);
            builder.PostProcessAndValidate();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.Count, Is.EqualTo(2));

            // Other services are ignored
            File.WriteAllText(tempFile, """
                [OtherService]
                Host=test
                Port=1234
                """);
            builder = factory("MyService", "MyHost", null, false);
            builder.PostProcessAndValidate();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.Count, Is.EqualTo(2));

            // Unknown settings are ignored
            File.WriteAllText(tempFile, """
                [MyService]
                Unknown=test
                Port=1234
                """);
            builder = factory("MyService", "MyHost", null, false);
            builder.PostProcessAndValidate();
            builder.PostProcessAndValidate();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.Count, Is.EqualTo(3));
            Assert.That(builder.Host, Is.EqualTo("MyHost"));
            Assert.That(builder.Port, Is.EqualTo(1234));
            Assert.That(builder.Service, Is.EqualTo("MyService"));

            // Overridden settings are ignored
            File.WriteAllText(tempFile, """
                [MyService]
                Host=test
                Port=1234
                """);
            builder = factory("MyService", "MyHost", null, false);
            builder.PostProcessAndValidate();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.Count, Is.EqualTo(3));
            Assert.That(builder.Host, Is.EqualTo("MyHost"));
            Assert.That(builder.Port, Is.EqualTo(1234));
            Assert.That(builder.Service, Is.EqualTo("MyService"));

            builder = factory("MyService", "MyHost", 5678, false);
            builder.PostProcessAndValidate();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.Count, Is.EqualTo(3));
            Assert.That(builder.Host, Is.EqualTo("MyHost"));
            Assert.That(builder.Port, Is.EqualTo(5678));
            Assert.That(builder.Service, Is.EqualTo("MyService"));

            builder = factory("MyService", "MyHost", 5678, true);
            builder.PostProcessAndValidate();
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.Count, Is.EqualTo(3));
            Assert.That(builder.Host, Is.EqualTo("MyHost"));
            Assert.That(builder.Port, Is.EqualTo(5678));
            Assert.That(builder.Service, Is.EqualTo("MyService"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
