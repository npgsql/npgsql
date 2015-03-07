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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using NLog.Config;
using NLog.Targets;
using System.Text;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{
    [TestFixture("9.4")]
    [TestFixture("9.3")]
    [TestFixture("9.2")]
    [TestFixture("9.1")]
    [TestFixture("9.0")]
    public abstract class TestBase
    {
        protected Version BackendVersion { get; private set; }

        /// <summary>
        /// Constructs the parameterized test fixture
        /// </summary>
        /// <param name="backendVersion">
        ///   The version of the Postgres backend to be used, major and minor veresions (e.g. 9.3).
        ///   Used to select the conn string environment variable to be used.
        /// </param>
        protected TestBase(string backendVersion)
        {
            BackendVersion = new Version(backendVersion);
        }

        /// <summary>
        /// A connection to the test database, set up prior to running each test.
        /// </summary>
        internal NpgsqlConnection Conn { get; set; }

        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        protected virtual string ConnectionString { get { return _connectionString; } }
        private string _connectionString;


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
        private string connectionStringEF;

        /// <summary>
        /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        private const string DEFAULT_CONNECTION_STRING = "Server=localhost;User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;syncnotification=false";

        /// <summary>
        /// Indicates whether the database schema has already been created in this unit test session.
        /// Multiple fixtures may run in the same session but we only want to create the schema once.
        /// </summary>
        private bool _schemaCreated;

        #region Setup / Teardown

        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            SetupLogging();

            var connStringEnvVar = "NPGSQL_TEST_DB_" + BackendVersion;
            _connectionString = Environment.GetEnvironmentVariable(connStringEnvVar);
            if (_connectionString == null)
            {
                if (BackendVersion == LatestBackendVersion)
                {
                    _connectionString = DEFAULT_CONNECTION_STRING;
                    Console.WriteLine("Using internal default connection string: " + _connectionString);
                }
                else
                {
                    Assert.Ignore("Skipping tests for backend version {0}, environment variable {1} isn't defined", BackendVersion, connStringEnvVar);
                    return;
                }
            }
            else
                Console.WriteLine("Using connection string provided in env var {0}: {1}", connStringEnvVar, _connectionString);

            if (!_schemaCreated)
            {
                try
                {
                    Conn = new NpgsqlConnection(ConnectionString);
                    Conn.Open();
                    CreateSchema();
                    Console.WriteLine("Schema created successfully. Backend version is " + Conn.PostgreSqlVersion);
                }
                catch (NpgsqlException e)
                {
                    try
                    {
                        if (Conn != null && Conn.State == ConnectionState.Open)
                            Conn.Close();
                    }
                    catch
                    {
                    }
                    if (e.Code == "3D000")
                        TestUtil.Inconclusive("Please create a database npgsql_tests, owned by user npgsql_tests");
                    else if (e.Code == "28P01")
                        TestUtil.Inconclusive(
                            "Please create a user npgsql_tests as follows: create user npgsql_tests with password 'npgsql_tests'");
                    else
                        throw;
                }
                catch
                {
                    try {
                        if (Conn != null && Conn.State == ConnectionState.Open)
                            Conn.Close();
                    }
                    catch {}
                    throw;
                }
            }
        }

        [SetUp]
        protected virtual void SetUp()
        {
            // If this is the first (or only) test being run, the connection has already been opened
            // in the fixture setup. Save the extra connecting time.
            if (Conn == null)
            {
                Conn = new NpgsqlConnection(ConnectionString);
                Conn.Open();
                ExecuteNonQuery("TRUNCATE data");
            }
        }

        [TearDown]
        protected virtual void TearDown()
        {
            try { Conn.Close(); }
            finally { Conn = null; }
        }

        private void CreateSchema()
        {
            Console.WriteLine("Creating test database schema");

            var sb = new StringBuilder();

            if (Conn.PostgreSqlVersion >= new Version(8, 2, 0))
            {
                sb.Append("DROP TABLE IF EXISTS DATA CASCADE;");
            }
            else
            {
                try
                {
                    ExecuteNonQuery("DROP TABLE DATA CASCADE");
                }
                catch (NpgsqlException e)
                {
                    if (! e.Message.ToLower().Contains("\"data\" does not exist"))
                    {
                        throw;
                    }
                }
            }

            var pkType = Conn.IsRedshift
                ? "INTEGER IDENTITY (1,1)"
                : "SERIAL PRIMARY KEY";

            sb.AppendFormat(@"CREATE TABLE data (
                                field_pk                      {0},
                                field_text                    TEXT,
                                field_char5                   CHAR(5),
                                field_varchar5                VARCHAR(5),
                                field_int2                    INT2,
                                field_int4                    INT4,
                                field_int8                    INT8,
                                field_numeric                 NUMERIC(11,7),
                                field_float4                  FLOAT4,
                                field_float8                  FLOAT8,
                                field_bool                    BOOL,
                                field_date                    DATE,
                                field_timestamp               TIMESTAMP", pkType);

            if (!Conn.IsRedshift)
            {
                sb.Append(",").AppendLine();
                sb.Append(@"field_serial                  SERIAL,
                            field_bit                     BIT,
                            field_time                    TIME,
                            field_timestamp_with_timezone TIMESTAMP WITH TIME ZONE,
                            field_bytea                   BYTEA,
                            field_inet                    INET,
                            field_point                   POINT,
                            field_box                     BOX,
                            field_lseg                    LSEG,
                            field_path                    PATH,
                            field_polygon                 POLYGON,
                            field_circle                  CIRCLE
                ");

                if (Conn.PostgreSqlVersion >= new Version(9, 2)) {
                    sb.Append(",").AppendLine();
                    sb.Append(@"field_json JSON");
                }

                if (Conn.PostgreSqlVersion >= new Version(9, 4)) {
                    sb.Append(",").AppendLine();
                    sb.Append(@"field_jsonb JSONB");
                }
            }

            sb.Append(") WITH OIDS;");  // Complete the CREATE TABLE command

            ExecuteNonQuery(sb.ToString());
            _schemaCreated = true;
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

        /// <summary>
        /// Uses reflection to read the [TextFixture] attributes on this class and extract the latest
        /// Postgres backend version specified within them.
        /// </summary>
        private Version LatestBackendVersion
        {
            get
            {
                return typeof(TestBase)
                    .GetCustomAttributes(typeof (TestFixtureAttribute), false)
                    .Cast<TestFixtureAttribute>()
                    .Select(a => new Version((string) a.Arguments[0]))
                    .Max();
            }
        }

        protected virtual void SetupLogging()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ConsoleTarget();
            consoleTarget.Layout = @"${message}";
            config.AddTarget("console", consoleTarget);
            var rule = new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule);
            NLog.LogManager.Configuration = config;

            LogManager.Adapter = new NLogLoggerFactoryAdapter(new NameValueCollection());
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

#if NET45
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

        protected static bool IsSequential(CommandBehavior behavior)
        {
            return (behavior & CommandBehavior.SequentialAccess) != 0;
        }
#endif

        #endregion
    }
}
