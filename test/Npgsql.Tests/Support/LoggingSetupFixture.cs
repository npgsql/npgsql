#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using NUnit.Framework;
using NLog.Config;
using NLog.Targets;
using NLog;
using Npgsql.Logging;
using Npgsql.Tests;
using Npgsql.Tests.Support;

// ReSharper disable once CheckNamespace

[SetUpFixture]
public class LoggingSetupFixture
{
    [OneTimeSetUp]
    public void Setup()
    {
        if (TestUtil.IsOnBuildServer)
            Console.Error.WriteLine("Running tests on: " + TestBase.ConnectionString);

        var logLevelText = Environment.GetEnvironmentVariable("NPGSQL_TEST_LOGGING");
        if (logLevelText == null)
            return;
        if (!Enum.TryParse(logLevelText, true, out NpgsqlLogLevel logLevel))
            throw new ArgumentOutOfRangeException($"Invalid loglevel in NPGSQL_TEST_LOGGING: {logLevelText}");

        var config = new LoggingConfiguration();
        var consoleTarget = new ColoredConsoleTarget
        {
            Layout = @"${message} ${exception:format=tostring}"
        };
        config.AddTarget("console", consoleTarget);
        var rule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
        config.LoggingRules.Add(rule);
        LogManager.Configuration = config;

        NpgsqlLogManager.Provider = new NLogLoggingProvider();
        NpgsqlLogManager.IsParameterLoggingEnabled = true;
    }
}
