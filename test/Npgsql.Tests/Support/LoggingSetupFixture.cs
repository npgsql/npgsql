using Microsoft.Extensions.Logging;
using Npgsql;
using NUnit.Framework;
using Npgsql.Tests.Support;

// ReSharper disable once CheckNamespace

[SetUpFixture]
public class LoggingSetupFixture
{
    [OneTimeSetUp]
    public void Setup()
        => NpgsqlLoggingConfiguration.InitializeLogging(LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(ListLoggerProvider.Instance);
        }), parameterLoggingEnabled: true);
}
