﻿#region License
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Npgsql.Tests
{
    public static class TestUtil
    {
        public static bool IsOnBuildServer => Environment.GetEnvironmentVariable("CI") != null;

        /// <summary>
        /// Calls Assert.Ignore() unless we're on the build server, in which case calls
        /// Assert.Fail(). We don't to miss any regressions just because something was misconfigured
        /// at the build server and caused a test to be inconclusive.
        /// </summary>
        public static void IgnoreExceptOnBuildServer(string message)
        {
            if (IsOnBuildServer)
                Assert.Fail(message);
            else
                Assert.Ignore(message);
        }

        public static void IgnoreExceptOnBuildServer(string message, params object[] args)
            => IgnoreExceptOnBuildServer(string.Format(message, args));

        public static void MinimumPgVersion(NpgsqlConnection conn, string minVersion, string ignoreText = null)
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

        static readonly Version MinCreateExtensionVersion = new Version(9, 1);
        public static void EnsureExtension(NpgsqlConnection conn, string extension, string minVersion = null)
        {
            if (minVersion != null)
                MinimumPgVersion(conn, minVersion,
                    $"The extension '{extension}' only works for PostgreSQL {minVersion} and higher.");

            if (conn.PostgreSqlVersion < MinCreateExtensionVersion)
                Assert.Ignore($"The 'CREATE EXTENSION' command only works for PostgreSQL {MinCreateExtensionVersion} and higher.");

            conn.ExecuteNonQuery($"CREATE EXTENSION IF NOT EXISTS {extension}");
            conn.ReloadTypes();
        }

        public static string GetUniqueIdentifier(string prefix)
            => prefix + Interlocked.Increment(ref _counter);

        static int _counter;

        /// <summary>
        /// Utility to generate a bytea literal in Postgresql hex format
        /// See http://www.postgresql.org/docs/current/static/datatype-binary.html
        /// </summary>
        internal static string EncodeByteaHex(ICollection<byte> buf)
        {
            var hex = new StringBuilder(@"E'\\x", buf.Count * 2 + 3);
            foreach (var b in buf)
                hex.Append($"{b:x2}");
            hex.Append("'");
            return hex.ToString();
        }

        internal static IDisposable SetEnvironmentVariable(string name, string value)
        {
            var resetter = new EnvironmentVariableResetter(name, Environment.GetEnvironmentVariable(name));
            Environment.SetEnvironmentVariable(name, value);
            return resetter;
        }

        class EnvironmentVariableResetter : IDisposable
        {
            readonly string _name, _value;

            internal EnvironmentVariableResetter(string name, string value)
            {
                _name = name;
                _value = value;
            }

            public void Dispose()
            {
                Environment.SetEnvironmentVariable(_name, _value);
            }
        }
    }

    public static class NpgsqlConnectionExtensions
    {
        public static int ExecuteNonQuery(this NpgsqlConnection conn, string sql, NpgsqlTransaction tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return cmd.ExecuteNonQuery();
        }

        [CanBeNull]
        public static object ExecuteScalar(this NpgsqlConnection conn, string sql, NpgsqlTransaction tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return cmd.ExecuteScalar();
        }

        public static async Task<int> ExecuteNonQueryAsync(this NpgsqlConnection conn, string sql, NpgsqlTransaction tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return await cmd.ExecuteNonQueryAsync();
        }

        [CanBeNull]
        public static async Task<object> ExecuteScalarAsync(this NpgsqlConnection conn, string sql, NpgsqlTransaction tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return await cmd.ExecuteScalarAsync();
        }
    }

    public static class NpgsqlCommandExtensions
    {
        public static T ExecuteScalar<T>(this NpgsqlCommand cmd)
        {
            using (var rdr = cmd.ExecuteReader())
                return rdr.Read() ? rdr.GetFieldValue<T>(0) : default;
        }

        public static NpgsqlDataReader ExecuteRecord(this NpgsqlCommand cmd)
        {
            var rdr = cmd.ExecuteReader();
            Assert.That(rdr.Read());
            return rdr;
        }
    }

    public static class CommandBehaviorExtensions
    {
        public static bool IsSequential(this CommandBehavior behavior)
            => (behavior & CommandBehavior.SequentialAccess) != 0;
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

        public void BeforeTest([NotNull] ITest test)
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                var msg = "Ignored on mono";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest([NotNull] ITest test) { }
        public ActionTargets Targets => ActionTargets.Test;
    }

    /// <summary>
    /// Causes the test to be ignored on Linux
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class LinuxIgnore : Attribute, ITestAction
    {
        readonly string _ignoreText;

        public LinuxIgnore(string ignoreText = null) { _ignoreText = ignoreText; }

        public void BeforeTest([NotNull] ITest test)
        {
            var osEnvVar = Environment.GetEnvironmentVariable("OS");
            if (osEnvVar == null || osEnvVar != "Windows_NT")
            {
                var msg = "Ignored on Linux";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest([NotNull] ITest test) { }
        public ActionTargets Targets => ActionTargets.Test;
    }

    /// <summary>
    /// Causes the test to be ignored on Windows
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class WindowsIgnore : Attribute, ITestAction
    {
        readonly string _ignoreText;

        public WindowsIgnore(string ignoreText = null) { _ignoreText = ignoreText; }

        public void BeforeTest([NotNull] ITest test)
        {
            var osEnvVar = Environment.GetEnvironmentVariable("OS");
            if (osEnvVar == "Windows_NT")
            {
                var msg = "Ignored on Windows";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest([NotNull] ITest test) { }
        public ActionTargets Targets => ActionTargets.Test;
    }

    public enum PrepareOrNot
    {
        Prepared,
        NotPrepared
    }

#if !NET452
    // When using netcoreapp, we use NUnit's portable library which doesn't include TimeoutAttribute
    // (probably because it can't enforce it). So we define it here to allow us to compile, once there's
    // proper support for netcoreapp this should be removed.
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    class TimeoutAttribute : Attribute
    {
        public TimeoutAttribute(int timeout) {}
    }
#endif
}
