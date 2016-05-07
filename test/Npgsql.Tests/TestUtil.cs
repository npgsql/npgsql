using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public static class TestUtil
    {
        /// <summary>
        /// Calls Assert.Ignore() unless we're on the build server, in which case calls
        /// Assert.Fail(). We don't to miss any regressions just because something was misconfigured
        /// at the build server and caused a test to be inconclusive.
        /// </summary>
        public static void IgnoreExceptOnBuildServer(string message)
        {
            if (Environment.GetEnvironmentVariable("TEAMCITY_VERSION") == null)
                Assert.Ignore(message);
            else
                Assert.Fail(message);
        }

        public static void IgnoreExceptOnBuildServer(string message, params object[] args)
        {
            IgnoreExceptOnBuildServer(String.Format(message, args));
        }

        public static void MinimumPgVersion(NpgsqlConnection conn, string minVersion, string ignoreText=null)
        {
            var min = new Version(minVersion);
            if (conn.PostgreSqlVersion < min)
            {
                var msg = $"Postgresql backend version {conn.PostgreSqlVersion} is less than the required {min}";
                if (ignoreText != null)
                    msg += ": " + ignoreText;
                Assert.Ignore(msg);
            }
        }
    }

    /// <summary>
    /// Semantic attribute that points to an issue linked with this test (e.g. this
    /// test reproduces the issue)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IssueLink : Attribute
    {
        public string LinkAddress { get; private set; }
        public IssueLink(string linkAddress)
        {
            LinkAddress = linkAddress;
        }
    }

    /// <summary>
    /// Causes the test to be ignored on mono
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class MonoIgnore : Attribute, ITestAction
    {
        readonly string _ignoreText;

        public MonoIgnore(string ignoreText = null) { _ignoreText = ignoreText; }

        public void BeforeTest(TestDetails testDetails)
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                var msg = "Ignored on mono";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest(TestDetails testDetails) { }
        public ActionTargets Targets { get { return ActionTargets.Test; } }
    }

    public enum PrepareOrNot
    {
        Prepared,
        NotPrepared
    }
}
