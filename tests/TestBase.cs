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
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{
    public abstract class TestBase
    {
        /// <summary>
        /// A connection to the test database, set up prior to running each test.
        /// </summary>
        protected NpgsqlConnection Conn { get; private set; }

        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures (e.g. for protocol v2)
        /// </summary>
        protected virtual string ConnectionString { get { return CONN_STRING_V3; } }

        private const string CONN_STRING_BASE = "Server=localhost;User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;syncnotification=false";
        protected const string CONN_STRING_V3 = CONN_STRING_BASE + ";protocol=3";
        protected const string CONN_STRING_V2 = CONN_STRING_BASE + ";protocol=2";

        /// <summary>
        /// Indicates whether the database schema has already been created in this unit test session.
        /// Multiple fixtures may run in the same session but we only want to create the schema once.
        /// </summary>
        private static bool _schemaCreated;

        #region Setup / Teardown

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            if (!_schemaCreated)
            {
                try
                {
                    Conn = new NpgsqlConnection(ConnectionString);
                    Conn.Open();
                    CreateSchema();
                }
                catch (NpgsqlException e)
                {
                    if (e.Code == "3D000")
                        Assert.Inconclusive("Please create a database npgsql_tests, owned by user npgsql_tests");
                    else if (e.Code == "28P01")
                        Assert.Inconclusive("Please create a user npgsql_tests as follows: create user npgsql_tests with password 'npgsql_tests'");
                    else
                        throw;
                }
            }

            try
            {
                SuppressBinaryBackendEncoding = InitBinaryBackendSuppression();
            }
            catch
            // Throwing an exception here causes all tests to fail without running.
            // CommandTests.SuppressBinaryBackendEncodingInitTest() provides error information in event of failure.
            {}
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
            ExecuteNonQuery("DROP TABLE IF EXISTS DATA CASCADE");
            ExecuteNonQuery(@"CREATE TABLE data (
                                field_pk                      SERIAL PRIMARY KEY,
                                field_serial                  SERIAL,
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
                                field_bit                     BIT,
                                field_date                    DATE,
                                field_time                    TIME,
                                field_timestamp               TIMESTAMP,
                                field_timestamp_with_timezone TIMESTAMP WITH TIME ZONE,
                                field_bytea                   BYTEA,
                                field_inet                    INET,
                                field_point                   POINT,
                                field_box                     BOX,
                                field_lseg                    LSEG,
                                field_path                    PATH,
                                field_polygon                 POLYGON,
                                field_circle                  CIRCLE
                                ) WITH OIDS");
            _schemaCreated = true;
        }

        #endregion

        #region Utilities for use by tests

        protected int ExecuteNonQuery(string sql, NpgsqlConnection conn=null)
        {
            if (conn == null)
                conn = Conn;
            using (var cmd = new NpgsqlCommand(sql, conn))
                return cmd.ExecuteNonQuery();
        }

        protected object ExecuteScalar(string sql, NpgsqlConnection conn = null)
        {
            if (conn == null)
                conn = Conn;
            using (var cmd = new NpgsqlCommand(sql, conn))
                return cmd.ExecuteScalar();
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
