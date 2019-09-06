//------------------------------------------------------------------------------
// <copyright file="NpgsqlVSPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Npgsql.VSIX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideService(typeof(NpgsqlProviderObjectFactory), IsAsyncQueryable = true, ServiceName = "PostgreSQL Provider Object Factory")]
    [ProvideBindingPath]  // Necessary for loading Npgsql via DbProviderFactories.GetProvider()
    [NpgsqlProviderRegistration]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.DataSourceWindowAutoVisible, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.DataSourceWindowSupported, PackageAutoLoadFlags.BackgroundLoad)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class NpgsqlVSPackage : AsyncPackage
    {
        /// <summary>
        /// NpgsqlVSPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "ef991dc4-3119-4ed6-bdb3-c160ca562560";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            AddService(typeof(NpgsqlProviderObjectFactory), CreateService, true);
            SetupNpgsqlProviderFactory();
            return base.InitializeAsync(cancellationToken, progress);
        }

        Task<object> CreateService(IAsyncServiceContainer container, CancellationToken cancellationtoken, Type servicetype)
            => servicetype == typeof(NpgsqlProviderObjectFactory)
                ? Task.FromResult<object>(new NpgsqlProviderObjectFactory())
                : throw new ArgumentException($"Can't create service of type '{servicetype.Name}'");

        void SetupNpgsqlProviderFactory()
        {
            if (!(ConfigurationManager.GetSection("system.data") is DataSet systemData))
                throw new Exception("No system.data section found in configuration manager!");

            DataTable factoriesTable;
            if (systemData.Tables.IndexOf("DbProviderFactories") == -1)
                factoriesTable = systemData.Tables.Add("DbProviderFactories");
            else
            {
                factoriesTable = systemData.Tables[systemData.Tables.IndexOf("DbProviderFactories")];
                if (factoriesTable.Rows.Find(Constants.NpgsqlInvariantName) != null)
                {
                    // There's already an entry for Npgsql in the machines.config.
                    // This should mean there's also a GAC-installed Npgsql - we don't need to do anything.
                    return;
                }
            }

            // Add an entry for Npgsql
            factoriesTable.Rows.Add("Npgsql Data Provider", ".NET Data Provider for PostgreSQL",
                Constants.NpgsqlInvariantName, "Npgsql.NpgsqlFactory, Npgsql");
        }
    }
}
