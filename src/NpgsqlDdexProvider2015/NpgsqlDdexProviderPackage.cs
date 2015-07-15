using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Npgsql.VisualStudio.Provider
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideService(typeof(NpgsqlProviderObjectFactory), ServiceName = "PostgreSQL Provider Object Factory")]
    [NpgsqlDataProviderRegistration]
    [Guid(GuidList.guidNpgsqlDdexProviderPkgString)]
    public sealed class NpgsqlDdexProviderPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NpgsqlDdexProviderPackage()
        {

        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            ((IServiceContainer)this).AddService(typeof(NpgsqlProviderObjectFactory), new NpgsqlProviderObjectFactory(), true);
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidNpgsqlDdexProviderCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemSetupNpgsqlDdexProvider, menuCommandID);
                mcs.AddCommand(menuItem);
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (CheckNpgsqlStatus.NeedInst)
            {
                UIUt.Alert(this, "It needs setup before using .NET Framework Data Provider for PostgreSQL \n"
                    + "\n"
                    + "[Tool]→[Setup NpgsqlDdexProvider]"
                    , "NpgsqlDdexProvider");
            }

            using (new NpgsqlConnection()) { }
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            var npgsqlAssembly = typeof(NpgsqlConnection).Assembly;
            if (e.Name.Equals(npgsqlAssembly.FullName))
                return npgsqlAssembly;
            return null;
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemSetupNpgsqlDdexProvider(object sender, EventArgs e)
        {
            if (UIUt.Confirm(this, "Could we modify the host config file in order to enable NpgsqlDdexProvider? \n"
                + "\n"
                + CheckNpgsqlStatus.Ut.HostConfig + "\n"
                + "\n"
                + "The assembly version: \n"
                + "\n"
                + typeof(Npgsql.NpgsqlFactory).Assembly.FullName
                , "NpgsqlDdexProvider"))
            {
                CheckNpgsqlStatus.DoInst();

                UIUt.Alert(this, "Modification successful. \n"
                    + "\n"
                    + "Please restart this VisualStudio."
                    , "NpgsqlDdexProvider");
            }
        }

        #endregion

        class UIUt
        {
            internal static void Alert(IServiceProvider host, String text, String title)
            {
                // Show a Message Box to prove we were here
                IVsUIShell uiShell = (IVsUIShell)host.GetService(typeof(SVsUIShell));
                Guid clsid = Guid.Empty;
                int result;

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                           0,
                           ref clsid,
                           title,
                           text,
                           string.Empty,
                           0,
                           OLEMSGBUTTON.OLEMSGBUTTON_OK,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                           OLEMSGICON.OLEMSGICON_WARNING,
                           0,        // false
                           out result));
            }

            internal static bool Confirm(IServiceProvider host, String text, String title)
            {
                // Show a Message Box to prove we were here
                IVsUIShell uiShell = (IVsUIShell)host.GetService(typeof(SVsUIShell));
                Guid clsid = Guid.Empty;
                int result;

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                           0,
                           ref clsid,
                           title,
                           text,
                           string.Empty,
                           0,
                           OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                           OLEMSGICON.OLEMSGICON_WARNING,
                           0,        // false
                           out result));

                return (result == 1); // IDOK
            }
        }
    }

}