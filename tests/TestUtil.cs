using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NpgsqlTests
{
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
}
