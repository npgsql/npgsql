#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Data.Common;
using System.Reflection;

// Keep the xml comment warning quiet for this file.
#pragma warning disable 1591

namespace Npgsql
{
    /// <summary>
    /// A factory to create instances of various Npgsql objects.
    /// </summary>
#if NET45 || NET452 || DNX452
    [Serializable]
#endif
    public sealed class NpgsqlFactory : DbProviderFactory, IServiceProvider
    {
        public static NpgsqlFactory Instance = new NpgsqlFactory();

        private NpgsqlFactory()
        {
        }

        /// <summary>
        /// Creates an NpgsqlCommand object.
        /// </summary>
        public override DbCommand CreateCommand()
        {
            return new NpgsqlCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new NpgsqlConnection();
        }


        public override DbParameter CreateParameter()
        {
            return new NpgsqlParameter();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new NpgsqlConnectionStringBuilder();
        }

#if NET45 || NET452 || DNX452
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new NpgsqlCommandBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new NpgsqlDataAdapter();
        }
#endif

        #region IServiceProvider Members

        public object GetService(Type serviceType) {
            // In legacy Entity Framework, this is the entry point for obtaining Npgsql's
            // implementation of DbProviderServices. We use reflection for all types to
            // avoid any dependencies on EF stuff in this project.

            if (serviceType != null && serviceType.FullName == "System.Data.Common.DbProviderServices")
            {
                // User has requested a legacy EF DbProviderServices implementation. Check our cache first.
                if (_legacyEntityFrameworkServices != null)
                    return _legacyEntityFrameworkServices;

                // First time, attempt to find the Npgsql.EntityFrameworkLegacy assembly and load the type via reflection
                var assemblyName = typeof(NpgsqlFactory).GetTypeInfo().Assembly.GetName();
                assemblyName.Name = "Npgsql.EntityFrameworkLegacy";
                Assembly npgsqlEfAssembly;
                try {
                    npgsqlEfAssembly = Assembly.Load(new AssemblyName(assemblyName.FullName));
                } catch (Exception e) {
                    throw new Exception("Could not load Npgsql.EntityFrameworkLegacy assembly, is it installed?", e);
                }

                Type npgsqlServicesType;
                if ((npgsqlServicesType = npgsqlEfAssembly.GetType("Npgsql.NpgsqlServices")) == null ||
                    npgsqlServicesType.GetProperty("Instance") == null)
                    throw new Exception("Npgsql.EntityFrameworkLegacy assembly does not seem to contain the correct type!");

                return _legacyEntityFrameworkServices = npgsqlServicesType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetMethod.Invoke(null, new object[0]);
            }

            return null;
        }

        private static object _legacyEntityFrameworkServices;

        #endregion
    }
}

#pragma warning restore 1591
