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
using System.Configuration;
using System.Reflection;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{
    public abstract class BaseClassTests
    {
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

        // Connection tests will use.
        protected NpgsqlConnection _conn = null;
        protected NpgsqlConnection _connV2 = null;
        
        // Transaction to rollback tests modifications.
        protected NpgsqlTransaction _t = null;
        protected NpgsqlTransaction _tV2 = null;
        
        protected abstract NpgsqlConnection TheConnection{get;}
        protected abstract NpgsqlTransaction TheTransaction{get;set;}
        
        // Commit transaction when test finish?   
        private Boolean commitTransaction = false;
        
        // Connection string
        
        protected String _connString = ConfigurationManager.AppSettings["ConnectionString"];
        protected string _connV2String = ConfigurationManager.AppSettings["ConnectionStringV2"];

        // Some tests need to suppress binary backend formatting of parameters and result values,
        // so that both binary and text formatting can be tested.
        // Setting NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding accomplishes this, but it is intentionally internal.
        // Since it's internal, reflection is required to observe and set it.
        protected FieldInfo SuppressBinaryBackendEncoding;

        protected Boolean CommitTransaction
        {
            set
            {
                commitTransaction = value;
            }
            
            get
            {
                return commitTransaction;
            }
        }

        [TestFixtureSetUp]
        protected virtual void TestFixtureSetup()
        {
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
            //NpgsqlEventLog.Level = LogLevel.None;
            //NpgsqlEventLog.Level = LogLevel.Debug;
            //NpgsqlEventLog.LogName = "NpgsqlTests.LogFile";
            _conn = new NpgsqlConnection(_connString);
            _conn.Open();
            _t = _conn.BeginTransaction();
            
            _connV2 = new NpgsqlConnection(_connV2String);
            _connV2.Open();
            _tV2 = _connV2.BeginTransaction();
            
            CommitTransaction = false;
        }

        [TearDown]
        protected virtual void TearDown()
        {
            if (_t != null && _t.Connection != null)
                if (CommitTransaction)
                    _t.Commit();
                else
                    _t.Rollback();
                
            if (_conn.State != ConnectionState.Closed)
                _conn.Close();
            
            if (_tV2 != null && _tV2.Connection != null)
                if(CommitTransaction)
                    _tV2.Commit();
                else
                    _tV2.Rollback();
                
            if (_connV2.State != ConnectionState.Closed)
                _connV2.Close();
        }

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

                Assert.IsNotNull(fi, "GetField(\"SuppressBinaryBackendEncoding2\") returned null indicating field not found");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to bind to field NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding", e);
            }

            Assert.IsTrue(fi.FieldType == typeof(bool), "Field NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding is not boolean");

            return fi;
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
            try
            {
                SuppressBinaryBackendEncoding = InitBinaryBackendSuppression();
            }
            catch
            // Throwing an exception here causes all tests to fail without running.
            {}

            Assert.IsTrue(
                SuppressBinaryBackendEncoding != null && SuppressBinaryBackendEncoding.FieldType == typeof(bool),
                "SuppressBinaryBackendEncoding is null or not boolean; binary backend encoding cannot be suppressed. Check test CommandTests.__SuppressBinaryBackendEncodingInitTest() for more information"
            );

            return new BackendBinarySuppressor(SuppressBinaryBackendEncoding);
        }
    }
}
