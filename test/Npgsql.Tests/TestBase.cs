//
// Author:
//  Francisco Figueiredo Jr. <fxjrlists@yahoo.com>
//
//  Copyright (C) 2002-2005 The Npgsql Development Team
//  npgsql-general@gborg.postgresql.org
//  http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using System.Text;
using NLog;
using Npgsql;
using Npgsql.Logging;

using NUnit.Framework;

namespace Npgsql.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// A connection to the test database, set up prior to running each test.
        /// </summary>
        internal NpgsqlConnection Conn { get; set; }

        static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        protected virtual string ConnectionString =>
            _connectionString ?? (_connectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString);

        string _connectionString;

        static bool _loggingSetUp;

        /// <summary>
        /// New ConectionString property crafted to change the database name from original TestBase.ConnectionString to append a "_ef" suffix.
        /// i.e.: the TestBase.ConnectionString database is npgsql_tests. Entity Framework database will be npgsql_tests_ef.
        /// </summary>
        protected virtual string ConnectionStringEF
        {
            get
            {
                if (connectionStringEF == null)
                {
                    //Reuse all strings just add _ef at end of database name for
                    var connectionSB = new NpgsqlConnectionStringBuilder(ConnectionString);
                    connectionSB.Database += "_ef";
                    connectionStringEF = connectionSB.ConnectionString;
                }
                return connectionStringEF;
            }
        }
        string connectionStringEF;

        /// <summary>
        /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        const string DefaultConnectionString = "Server=localhost;User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests";

        #region Setup / Teardown

        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            SetupLogging();
            _log.Debug("Connection string is: " + ConnectionString);
        }

        [SetUp]
        protected virtual void SetUp()
        {
            Conn = new NpgsqlConnection(ConnectionString);
            try
            {
                Conn.Open();
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "3D000")
                    TestUtil.IgnoreExceptOnBuildServer("Please create a database npgsql_tests, owned by user npgsql_tests");
                else if (e.Code == "28P01")
                    TestUtil.IgnoreExceptOnBuildServer("Please create a user npgsql_tests as follows: create user npgsql_tests with password 'npgsql_tests'");
                else
                    throw;
            }
        }

        [TearDown]
        protected virtual void TearDown()
        {
            try { Conn.Close(); }
            finally { Conn = null; }
        }

        protected void CreateSchema(string schemaName)
        {
            if (Conn.PostgreSqlVersion >= new Version(9, 3))
                ExecuteNonQuery(String.Format("CREATE SCHEMA IF NOT EXISTS {0}", schemaName));
            else
            {
                try { ExecuteNonQuery(String.Format("CREATE SCHEMA {0}", schemaName)); }
                catch (NpgsqlException e) {
                    if (e.Code != "42P06")
                        throw;
                }
            }
        }

        protected virtual void SetupLogging()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ConsoleTarget();
            consoleTarget.Layout = @"${message} ${exception:format=tostring}";
            config.AddTarget("console", consoleTarget);
            var rule = new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule);
            NLog.LogManager.Configuration = config;

            if (!_loggingSetUp)
            {
                NpgsqlLogManager.Provider = new NLogLoggingProvider();
                NpgsqlLogManager.IsParameterLoggingEnabled = true;
                _loggingSetUp = true;
            }
        }

        #endregion

        #region Utilities for use by tests

        protected int ExecuteNonQuery(string sql, NpgsqlConnection conn = null, NpgsqlTransaction tx = null)
        {
            if (conn == null)
                conn = Conn;
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return cmd.ExecuteNonQuery();
        }

        protected object ExecuteScalar(string sql, NpgsqlConnection conn = null, NpgsqlTransaction tx = null)
        {
            if (conn == null)
                conn = Conn;
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return cmd.ExecuteScalar();
        }

#if !NET40
        protected async Task<int> ExecuteNonQueryAsync(string sql, NpgsqlConnection conn = null, NpgsqlTransaction tx = null)
        {
            if (conn == null)
                conn = Conn;
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return await cmd.ExecuteNonQueryAsync();
        }

        protected async Task<object> ExecuteScalarAsync(string sql, NpgsqlConnection conn = null, NpgsqlTransaction tx = null)
        {
            if (conn == null)
                conn = Conn;
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return await cmd.ExecuteScalarAsync();
        }
#endif

        protected static bool IsSequential(CommandBehavior behavior)
        {
            return (behavior & CommandBehavior.SequentialAccess) != 0;
        }


        /// <summary>
        /// In PG under 9.1 you can't do SELECT pg_sleep(2) in binary because that function returns void and PG doesn't know
        /// how to transfer that. So cast to text server-side.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        protected static NpgsqlCommand CreateSleepCommand(NpgsqlConnection conn, int seconds)
        {
            return new NpgsqlCommand(string.Format("SELECT pg_sleep({0}){1}", seconds, conn.PostgreSqlVersion < new Version(9, 1, 0) ? "::TEXT" : ""), conn);
        }

        #endregion
    }
}
