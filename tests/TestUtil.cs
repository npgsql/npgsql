using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NpgsqlTests
{
    public static class TestUtil
    {
        /// <summary>
        /// Calls Assert.Inconclusive() unless we're on the build server, in which case calls
        /// Assert.Fail(). We don't to miss any regressions just because something was misconfigured
        /// at the build server and caused a test to be inconclusive.
        /// </summary>
        public static void Inconclusive(string message)
        {
            if (Environment.GetEnvironmentVariable("TEAMCITY_VERSION") == null)
                Assert.Inconclusive(message);
            else
                Assert.Fail(message);
        }

        public static void Inconclusive(string message, params object[] args)
        {
            Inconclusive(String.Format(message, args));
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
    
    /// <summary>
    /// Causes the test to be ignored if the Postgresql backend version is less than the given one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class MinPgVersion : Attribute, ITestAction
    {
        readonly Version _minVersion;
        readonly string _ignoreText;

        public MinPgVersion(int major, int minor, int build, string ignoreText = null)
        {
            _minVersion = new Version(major, minor, build);
            _ignoreText = ignoreText;
        }

        public void BeforeTest(TestDetails testDetails)
        {
            var asTestBase = testDetails.Fixture as TestBase;
            if (asTestBase == null)
                throw new Exception("[MinPgsqlVersion] can only be used in fixtures inheriting from TestBase");

            if (asTestBase.Conn.PostgreSqlVersion < _minVersion)
            {
                var msg = String.Format("Postgresql backend version {0} is less than the required {1}",
                                        asTestBase.Conn.PostgreSqlVersion, _minVersion);
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
