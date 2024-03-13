//------------------------------------------------------------------------------
// <copyright file="NpgsqlVSPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data.Common;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
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
    // Start loading as soon as the VS shell is available.
    [ProvideUIContextRule(PackageUIContextRuleGuid,
    name: "Npgsql UIContext Autoload",
    expression: "(ShellInit | SolutionModal | DataSourceWindowVisible | DataSourceWindowSupported)",
    termNames: new string[] { "ShellInit", "SolutionModal", "DataSourceWindowVisible", "DataSourceWindowSupported" },
    termValues: new string[] { VSConstants.UICONTEXT.ShellInitialized_string, VSConstants.UICONTEXT.SolutionOpening_string, UIContextGuids80.DataSourceWindowAutoVisible, UIContextGuids80.DataSourceWindowSupported })]
    [ProvideAutoLoad(PackageUIContextRuleGuid, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class NpgsqlVSPackage : AsyncPackage
    {
        /// <summary>
        /// NpgsqlVSPackage static .ctor
        /// </summary>
        static NpgsqlVSPackage() => SetupNpgsqlProviderFactory();

        /// <summary>
        /// NpgsqlVSPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "ef991dc4-3119-4ed6-bdb3-c160ca562560";

        /// <summary>
        /// NpgsqlVSPackage UIContext autoload rule guid string.
        /// </summary>
        public const string PackageUIContextRuleGuid = "CFB183FF-3D31-4ACB-9475-ED31289920ED";
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            AddService(typeof(NpgsqlProviderObjectFactory), CreateServiceAsync, true);
            return base.InitializeAsync(cancellationToken, progress);
        }

        Task<object> CreateServiceAsync(IAsyncServiceContainer container, CancellationToken cancellationtoken, Type servicetype)
            => servicetype == typeof(NpgsqlProviderObjectFactory)
                ? Task.FromResult<object>(new NpgsqlProviderObjectFactory())
                : throw new ArgumentException($"Can't create service of type '{servicetype.Name}'");

        /// <summary>
        /// Directly adds a DotNet Invariant's DBProviderFactory into the local assembly
        /// register.
        /// </summary>
        private static void SetupNpgsqlProviderFactory()
        {
            /*
             * DbProviderFactories.GetFactoryClasses()
             * 		 
             * Returns a System.Data.DataTable containing System.Data.DataRow objects that contain
             * the following data of DbProviderFactories. 
             * Ordinal	Column name				Description
             * -------	-----------				-----------
             * 0		Name					Human-readable name for the data provider.
             * 1		Description				Human-readable description of the data provider.
             * 2		InvariantName			Name that can be used programmatically to refer to the data provider.
             *									eg. FirebirdSql.Data.FirebirdClient
             * 3		AssemblyQualifiedName	Fully qualified name of the factory class, which contains enough
             *									information to instantiate the object.
            */

            var table = DbProviderFactories.GetFactoryClasses();
            var row = table.Rows.Find(Constants.NpgsqlInvariantName);

            if (row != null)
                return;

            table.Rows.Add("Npgsql Data Provider", ".NET Data Provider for PostgreSQL",
                Constants.NpgsqlInvariantName, typeof(NpgsqlFactory).AssemblyQualifiedName);
            table.AcceptChanges();

            var fieldInfo = typeof(DbProviderFactories).GetField("_providerTable", BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new COMException($"Could not get FieldInfo for static field '_providerTable' in container class '{typeof(DbProviderFactories)}'.");

            try
            {
                fieldInfo.SetValue(null, table);
            }
            catch (Exception ex)
            {
                throw new COMException($"Could not set Field Value for static field '{fieldInfo.Name}' in container class '{typeof(DbProviderFactories)}'. Exception: {ex.Message}");
            }

            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                if (args.Name == typeof(NpgsqlFactory).Assembly.FullName)
                {
                    return typeof(NpgsqlFactory).Assembly;
                }

                return null;
            };
        }
    }
}
