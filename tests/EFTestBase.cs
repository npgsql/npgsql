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
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{
    [TestFixture("9.3")]
    [TestFixture("9.2")]
    [TestFixture("9.1")]
    [TestFixture("9.0")]
    [TestFixture("8.4")]
    public abstract class EFTestBase
    {
        protected Version BackendVersion { get; private set; }
        protected ProtocolVersion BackendProtocolVersion = ProtocolVersion.Version3;

        /// <summary>
        /// Constructs the parameterized test fixture
        /// </summary>
        /// <param name="backendVersion">
        ///   The version of the Postgres backend to be used, major and minor veresions (e.g. 9.3).
        ///   Used to select the conn string environment variable to be used.
        /// </param>
        protected EFTestBase(string backendVersion)
        {
            BackendVersion = new Version(backendVersion);
        }

        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        protected virtual string ConnectionString { get { return _connectionString; } }
        private string _connectionString;

        /// <summary>
        /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        private const string DEFAULT_CONNECTION_STRING = "Server=localhost;User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;syncnotification=false";
        
        #region Setup / Teardown

        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
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

            if (_connectionString.Contains("protocol"))
                throw new Exception("Connection string base cannot contain protocol");
            _connectionString += ";protocol=" + (int)BackendProtocolVersion;

            //Reuse all strings just add _ef at end of database name for 
            var connectionSB = new NpgsqlConnectionStringBuilder(_connectionString);
            connectionSB.Database += "_ef";
            _connectionString = connectionSB.ConnectionString;

            try
            {
                SuppressBinaryBackendEncoding = InitBinaryBackendSuppression();
            }
            catch
            // Throwing an exception here causes all tests to fail without running.
            // CommandTests.SuppressBinaryBackendEncodingInitTest() provides error information in event of failure.
            { }
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
                    .GetCustomAttributes(typeof(TestFixtureAttribute), false)
                    .Cast<TestFixtureAttribute>()
                    .Select(a => new Version((string)a.Arguments[0]))
                    .Max();
            }
        }

        #endregion

        #region Binary backend suppression

        // Some tests need to suppress binary backend formatting of parameters and result values,
        // so that both binary and text formatting can be tested.
        // Setting NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding accomplishes this, but it is intentionally internal.
        // Since it's internal, reflection is required to observe and set it.
        protected FieldInfo SuppressBinaryBackendEncoding;

        // Use reflection to bind to NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding.
        protected FieldInfo InitBinaryBackendSuppression()
        {
            Assembly npgsql;
            Type typesHelper;
            FieldInfo fi;

            npgsql = Assembly.Load("Npgsql");

            // GetType() can return null.  Check for this situation and report it.
            try
            {
                typesHelper = npgsql.GetType("NpgsqlTypes.NpgsqlTypesHelper");

                Assert.IsNotNull(typesHelper, "GetType(\"NpgsqlTypes.NpgsqlTypesHelper\") returned null indicating class not found");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to bind to class NpgsqlTypes.NpgsqlTypesHelper", e);
            }

            // GetField() can return null.  Check for this situation and report it.
            try
            {
                fi = typesHelper.GetField("SuppressBinaryBackendEncoding", BindingFlags.Static | BindingFlags.NonPublic);

                Assert.IsNotNull(fi, "GetField(\"SuppressBinaryBackendEncoding\") returned null indicating field not found");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to bind to field NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding", e);
            }

            Assert.IsTrue(fi.FieldType == typeof(bool), "Field NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding is not boolean");

            return fi;
        }

        protected class BackendBinarySuppressor : IDisposable
        {
            private FieldInfo suppressionField;

            internal BackendBinarySuppressor(FieldInfo suppressionField)
            {
                this.suppressionField = suppressionField;
                suppressionField.SetValue(null, true);
            }

            public void Dispose()
            {
                suppressionField.SetValue(null, false);
            }
        }

        /// <summary>
        /// Return a BackendBinarySuppressor which has suppressed backend binary encoding.
        /// When it is disposed, suppression will be ended.  Example:
        /// using (SuppressBackendBinary())
        /// {
        ///   // Test text encoding functionality here.
        /// }
        /// </summary>
        protected BackendBinarySuppressor SuppressBackendBinary()
        {
            Assert.IsTrue(
                SuppressBinaryBackendEncoding != null && SuppressBinaryBackendEncoding.FieldType == typeof(bool),
                "SuppressBinaryBackendEncoding is null or not boolean; binary backend encoding cannot be suppressed. Check test CommandTests.__SuppressBinaryBackendEncodingInitTest() for more information"
            );

            return new BackendBinarySuppressor(SuppressBinaryBackendEncoding);
        }

        #endregion
    }
}
