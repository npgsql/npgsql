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
using System.Data;
using System.IO;
using System.Linq;
using Npgsql.Logging;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        public static string ConnectionString =>
            Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;

        /// <summary>
        /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        const string DefaultConnectionString = "Server=localhost;User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

        #region Utilities for use by tests

        protected virtual NpgsqlConnection OpenConnection(string connectionString = null)
        {
            if (connectionString == null)
                connectionString = ConnectionString;
            var conn = new NpgsqlConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (PostgresException e)
            {
                if (e.SqlState == "3D000")
                    TestUtil.IgnoreExceptOnBuildServer("Please create a database npgsql_tests, owned by user npgsql_tests");
                else if (e.SqlState == "28P01")
                    TestUtil.IgnoreExceptOnBuildServer("Please create a user npgsql_tests as follows: create user npgsql_tests with password 'npgsql_tests'");
                else
                    throw;
            }

            return conn;
        }

        protected NpgsqlConnection OpenConnection(NpgsqlConnectionStringBuilder csb)
            => OpenConnection(csb.ToString());

        // In PG under 9.1 you can't do SELECT pg_sleep(2) in binary because that function returns void and PG doesn't know
        // how to transfer that. So cast to text server-side.
        protected static NpgsqlCommand CreateSleepCommand(NpgsqlConnection conn, int seconds = 1000)
            => new NpgsqlCommand($"SELECT pg_sleep({seconds}){(conn.PostgreSqlVersion < new Version(9, 1, 0) ? "::TEXT" : "")}", conn);

        protected bool IsRedshift => new NpgsqlConnectionStringBuilder(ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.Redshift;

        #endregion
    }
}
