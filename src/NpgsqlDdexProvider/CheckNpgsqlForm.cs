using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Npgsql.VisualStudio.Provider {
    public partial class CheckNpgsqlForm : Form {
        public CheckNpgsqlForm() {
            InitializeComponent();
        }

        public System.IServiceProvider sp { get; set; }

        private void bCopy_Click(object sender, EventArgs e) {
            rtb.SelectAll();
            rtb.Copy();

            MessageBox.Show("Copied to clipboard.");
        }

        // http://blog.mastykarz.nl/active-project-extending-visual-studio-sharepoint-development-tools-tip-1/
        internal static EnvDTE.Project GetActiveProject() {
            var dte = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
            return GetActiveProject(dte);
        }
        internal static EnvDTE.Project GetActiveProject(EnvDTE.DTE dte) {
            EnvDTE.Project activeProject = null;

            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0) {
                activeProject = activeSolutionProjects.GetValue(0) as EnvDTE.Project;
            }

            return activeProject;
        }

        private void Newl() {
            rtb.AppendText("\n");
        }

        private void Log(string p, Color color) {
            int x0 = rtb.TextLength;
            rtb.AppendText(p);
            int x1 = rtb.TextLength;
            rtb.Select(x0, x1 - x0);
            rtb.SelectionColor = color;
            rtb.Select(x1, 0);
        }

        private void Log(string p) {
            Log(p, ForeColor);
        }

        private void CheckNpgsqlForm_Load(object sender, EventArgs e) {
            llGrab.Text = llGrab.Text.Replace("%ver%", typeof(NpgsqlConnection).Assembly.GetName().Version.ToString());
            llEFv5.Text = llEFv5.Text.Replace("%ver%", typeof(NpgsqlConnection).Assembly.GetName().Version.ToString());
            llEFv6.Text = llEFv6.Text.Replace("%ver%", typeof(NpgsqlConnection).Assembly.GetName().Version.ToString());
        }

        private void bEFv5_Click(object sender, EventArgs e) {
            CheckEF(false);
        }

        private void UnsuggestAll() {
            foreach (var ll in tabPage3.Controls.OfType<LinkLabel>()) {
                UnsuggestIt(ll);
            }
        }

        private void SuggestIt(LinkLabel ll) {
            ll.Text = "⇒" + ll.Text.Substring(1);
            ll.LinkArea = new LinkArea(0, 1);
        }
        private void UnsuggestIt(LinkLabel ll) {
            ll.Text = " " + ll.Text.Substring(1);
            ll.LinkArea = new LinkArea(0, 0);
        }

        private void bEFv6_Click(object sender, EventArgs e) {
            CheckEF(true);
        }

        void CheckEF(bool v6) {
            tabControl1.SelectedTab = tabPage2;

            UnsuggestAll();

            rtb.Clear();

            try {
                {
                    Log("Check your active project... ");

                    var proj = GetActiveProject();
                    Log(proj.Name, Color.Blue);

                    Newl();

                }
                {
                    Log("Check if Npgsql is registered in host devenv's <DbProviderFactories>... ");

                    Object dbf = null;
                    try {
                        dbf = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");
                    }
                    catch (Exception) {

                    }

                    if (dbf != null) {
                        Log("Yes", Color.Blue);
                    }
                    else {
                        Log("No", Color.Red);
                    }

                    Newl();
                }
                System.Reflection.AssemblyName verNpgsql = null;
                {
                    Log("Check version of Npgsql in NpgsqlDdexProvider... ");

                    var dbf1 = Npgsql.NpgsqlFactory.Instance;

                    Log(dbf1.GetType().Assembly.FullName, Color.Blue);
                    Newl();

                    {
                        Log("Check version of Npgsql from host devenv's DbProviderFactories.GetFactory... ");

                        var dbf2 = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");

                        if (dbf2 != null) {
                            Log(dbf2.GetType().Assembly.FullName, Color.Blue);
                            Newl();

                            {
                                Log("Check location of Npgsql assembly in NpgsqlDdexProvider... ");

                                var loc1 = dbf1.GetType().Assembly.Location;
                                Log(loc1, Color.Blue);
                                Newl();

                                Log("Check location of Npgsql assembly from host devenv's DbProviderFactories.GetFactory... ");

                                var loc2 = dbf2.GetType().Assembly.Location;
                                Log(loc2, Color.Blue);
                                Newl();

                                Log("Check if 2 Npgsql assemblies have equivalent file path... ");
                                if (loc1.Equals(loc2)) {
                                    Log("Yes", Color.Blue);
                                    Newl();

                                    {
                                        Log("Check both Npgsql assembly version... ");

                                        verNpgsql = new AssemblyName(dbf2.GetType().Assembly.FullName);

                                        Log(verNpgsql.Version + "", Color.Blue);
                                        Newl();

                                        {
                                            Log("Check Npgsql assembly version in OutputPath... ");

                                            var proj = GetActiveProject();
                                            if (proj != null) {
                                                var vcs = proj.Object as VSLangProj80.VSProject2;
                                                if (vcs != null) {
                                                    try {
                                                        var FullPath = "" + PPUt.Get(proj, "FullPath", "");
                                                        var OutputPath = "" + PPUt.Get(proj, "OutputPath", "");
                                                        var fpBinNpgsql = System.IO.Path.Combine(FullPath, OutputPath, "Npgsql.dll");
                                                        if (System.IO.File.Exists(fpBinNpgsql)) {
                                                            var verBinNpgsql = AssemblyName.GetAssemblyName(fpBinNpgsql).Version;
                                                            if (verBinNpgsql == dbf1.GetType().Assembly.GetName().Version) {
                                                                Log(verBinNpgsql + "", Color.Blue);
                                                                Newl();
                                                            }
                                                            else {
                                                                Log(verBinNpgsql + ", you need to build once.", Color.Red);
                                                                Newl();

                                                                SuggestIt(llBuild);
                                                            }
                                                        }
                                                        else {
                                                            Log("No Npgsql.dll placed at " + fpBinNpgsql, Color.Blue);
                                                            Newl();

                                                            SuggestIt(llBuild);
                                                        }
                                                    }
                                                    catch (Exception err) {
                                                        Log("Failed. " + err, Color.Red);
                                                        Newl();
                                                    }
                                                }
                                                else {
                                                    Log("No C# or VB.NET active project", Color.Blue);
                                                    Newl();
                                                }
                                            }
                                            else {
                                                Log("No active project", Color.Blue);
                                                Newl();
                                            }
                                        }
                                    }
                                }
                                else {
                                    Log("No", Color.Red);
                                    Newl();

                                    SuggestIt(llRestart);
                                    SuggestIt(llGrab);
                                }
                            }
                        }
                        else {
                            Log("(Absent)", Color.Blue);
                            Newl();
                        }
                    }
                }

                {
                    Log("Check if C# or VB.NET project... ");

                    var proj = GetActiveProject();
                    if (proj != null) {
                        var vcs = proj.Object as VSLangProj80.VSProject2;
                        if (vcs != null) {
                            Log("Yes", Color.Blue);
                            Newl();

                            var llEFvx = v6 ? llEFv6 : llEFv5;

                            {
                                Version need = v6
                                    ? new Version("6.0.0.0")
                                    : new Version("5.0.0.0")
                                    ;
                                var name = "EntityFramework";

                                Log("Check if " + name + " " + need + " is referenced... ");

                                Version found = null;

                                if (vcs.References != null) {
                                    foreach (VSLangProj.Reference re in vcs.References) {
                                        if (name.Equals(re.Identity, StringComparison.InvariantCultureIgnoreCase)) {
                                            found = new Version(re.Version);
                                        }
                                    }
                                }

                                if (found == null) {
                                    Log("Not found in assembly reference", Color.Red);
                                    Newl();
                                    SuggestIt(llEFvx);
                                }
                                else if (need == found) {
                                    Log("Yes", Color.Blue);
                                    Newl();
                                }
                                else {
                                    Log("No, " + found + " is referenced", Color.Red);
                                    Newl();
                                    SuggestIt(llEFvx);
                                }
                            }

                            if (verNpgsql != null && verNpgsql.Version != null) {
                                var need = verNpgsql.Version;
                                var name = v6
                                    ? "EntityFramework6.Npgsql"
                                    : "EntityFramework5.Npgsql"
                                    ;

                                Log("Check if " + name + " " + need + " is referenced... ");

                                Version found = null;

                                if (vcs.References != null) {
                                    foreach (VSLangProj.Reference re in vcs.References) {
                                        if (name.Equals(re.Identity, StringComparison.InvariantCultureIgnoreCase)) {
                                            found = new Version(re.Version);
                                        }
                                    }
                                }

                                if (found == null) {
                                    Log("Not found in assembly reference", Color.Red);
                                    Newl();
                                    SuggestIt(llEFvx);
                                }
                                else if (need == found) {
                                    Log("Yes", Color.Blue);
                                    Newl();
                                }
                                else {
                                    Log("No, " + found + " is referenced", Color.Red);
                                    Newl();
                                    SuggestIt(llEFvx);
                                }
                            }

                            {
                                Log("Check App.config or Web.config is included... ");

                                String fpXml = null;
                                foreach (EnvDTE.ProjectItem pi in proj.ProjectItems) {
                                    if (false
                                        || "App.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                                        || "Web.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                                    ) {
                                        fpXml = pi.FileNames[1];
                                        break;
                                    }
                                }

                                if (fpXml != null) {
                                    Log("Yes", Color.Blue);
                                    Newl();

                                    Log("Check if XmlDocument can load it... ");

                                    XmlDocument xd = new XmlDocument();
                                    xd.Load(fpXml);

                                    Log("Yes", Color.Blue);
                                    Newl();

                                    {
                                        Log("Check <configuration>... ");
                                        var el1 = xd.SelectSingleNode("/configuration") as XmlElement;
                                        if (el1 != null) {
                                            Log("Yes", Color.Blue);
                                            Newl();

                                            {
                                                Log("Check <system.data>... ");
                                                var el2 = el1.SelectSingleNode("system.data") as XmlElement;
                                                if (el2 != null) {
                                                    Log("Yes", Color.Blue);
                                                    Newl();

                                                    Log("Check <DbProviderFactories>... ");
                                                    var el3 = el2.SelectSingleNode("DbProviderFactories") as XmlElement;
                                                    if (el3 != null) {
                                                        Log("Yes", Color.Blue);
                                                        Newl();

                                                        {
                                                            Log("Check <remove invariant=\"Npgsql\" />... ");
                                                            var el4 = el3.SelectSingleNode("remove[@invariant='Npgsql']") as XmlElement;
                                                            if (el4 != null) {
                                                                Log("Yes", Color.Blue);
                                                                Newl();
                                                            }
                                                            else {
                                                                Log("No", Color.Red);
                                                                Newl();
                                                            }
                                                        }
                                                        {
                                                            Log("Check <add invariant=\"Npgsql\" ... />... ");
                                                            var el4 = el3.SelectSingleNode("add[@invariant='Npgsql']") as XmlElement;
                                                            if (el4 != null) {
                                                                Log("Yes", Color.Blue);
                                                                Newl();

                                                                {
                                                                    Log("Check if type=\"Npgsql.NpgsqlFactory, Npgsql\"... ");
                                                                    var NpgsqlFactory = (el4.GetAttribute("type"));
                                                                    if (true
                                                                        && NpgsqlFactory != null
                                                                        && System.Text.RegularExpressions.Regex.Replace(NpgsqlFactory, "\\s+", "").Equals("Npgsql.NpgsqlFactory,Npgsql")
                                                                    ) {
                                                                        Log("Yes", Color.Blue);
                                                                        Newl();
                                                                    }
                                                                    else {
                                                                        Log("No", Color.Red);
                                                                        Newl();
                                                                        SuggestIt(llADONet);
                                                                    }
                                                                }
                                                            }
                                                            else {
                                                                Log("No", Color.Red);
                                                                Newl();
                                                                SuggestIt(llADONet);
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        Log("No", Color.Red);
                                                        Newl();
                                                        SuggestIt(llADONet);
                                                    }
                                                }
                                                else {
                                                    Log("No", Color.Red);
                                                    Newl();
                                                    SuggestIt(llADONet);
                                                }
                                            }
                                            {
                                                Log("Check <configSections>... ");
                                                var el2 = el1.SelectSingleNode("configSections") as XmlElement;
                                                if (el2 != null) {
                                                    Log("Yes", Color.Blue);
                                                    Newl();

                                                    {
                                                        Log("Check <section name=\"entityFramework\" ...>... ");
                                                        var el3 = el2.SelectSingleNode("section[@name='entityFramework']") as XmlElement;
                                                        if (el3 != null) {
                                                            Log("Yes", Color.Blue);
                                                            Newl();

                                                        }
                                                        else {
                                                            Log("No", Color.Red);
                                                            Newl();
                                                        }
                                                    }

                                                    {
                                                        Log("Check <configSections> is located at first element... ");
                                                        var el3 = el2.PreviousSibling as XmlElement;
                                                        if (el3 == null) {
                                                            Log("Yes", Color.Blue);
                                                            Newl();
                                                        }
                                                        else {
                                                            Log("No", Color.Red);
                                                            Newl();

                                                            SuggestIt(llConfigSections);
                                                        }
                                                    }
                                                }
                                                else {
                                                    Log("No", Color.Red);
                                                    Newl();
                                                }
                                            }
                                            if (v6) {
                                                Log("Check <entityFramework>... ");
                                                var el2 = el1.SelectSingleNode("entityFramework") as XmlElement;
                                                if (el2 != null) {
                                                    Log("Yes", Color.Blue);
                                                    Newl();

                                                    Log("Check <providers>... ");
                                                    var el3 = el2.SelectSingleNode("providers") as XmlElement;
                                                    if (el3 != null) {
                                                        Log("Yes", Color.Blue);
                                                        Newl();

                                                        Log("Check <provider invariantName=\"Npgsql\" type=\"" + EFv6Provider + "\">... ");
                                                        var el4 = el3.SelectSingleNode("provider[@invariantName='Npgsql' and @type='" + EFv6Provider + "']") as XmlElement;
                                                        if (el4 != null) {
                                                            Log("Yes", Color.Blue);
                                                            Newl();
                                                        }
                                                        else {
                                                            Log("No", Color.Red);
                                                            Newl();
                                                            SuggestIt(llProvider);
                                                        }
                                                    }
                                                    else {
                                                        Log("No", Color.Red);
                                                        Newl();
                                                        SuggestIt(llProvider);
                                                    }
                                                }
                                                else {
                                                    Log("No", Color.Red);
                                                    Newl();
                                                    SuggestIt(llProvider);
                                                }
                                            }
                                        }
                                        else {
                                            Log("No", Color.Red);
                                            Newl();
                                        }
                                    }
                                }
                                else {
                                    Log("No", Color.Red);
                                    Newl();
                                }
                            }
                        }
                        else {
                            Log("No", Color.Red);
                            Newl();
                        }
                    }
                    else {
                        Log("No active project selected", Color.Red);
                        Newl();
                    }
                }
                tabControl1.SelectedTab = tabPage3;
            }
            catch (Exception err) {
                Log("" + err, Color.Purple);
            }

        }

        class PPUt {
            public static String Get(EnvDTE.Project proj, String name, String defv) {
                if (proj != null) {
                    var cm = proj.ConfigurationManager;
                    if (cm != null) {
                        var ac = cm.ActiveConfiguration;
                        if (ac != null) {
                            var props = ac.Properties;
                            if (props != null) {
                                try {
                                    var it = props.Item(name);
                                    if (it != null)
                                        return "" + it.Value;
                                }
                                catch (ArgumentException) { } // prop not found
                            }
                        }
                    }

                    {
                        var props = proj.Properties;
                        if (props != null) {
                            try {
                                var it = props.Item(name);
                                if (it != null)
                                    return "" + it.Value;
                            }
                            catch (ArgumentException) { } // prop not found
                        }
                    }
                }
                return defv;
            }
        }

        private bool TryOpen(String url) {
            if (MessageBox.Show(this, "Open?\n\n" + url, null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                System.Diagnostics.Process.Start(url);
                return true;
            }
            return false;
        }

        private bool TryOpen2(params String[] urls) {
            if (MessageBox.Show(this, "Open them?\n\n" + String.Join("\n", urls), null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                foreach (var url in urls)
                    System.Diagnostics.Process.Start(url);
                return true;
            }
            return false;
        }

        private void llGrab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (TryOpen2("https://github.com/npgsql/npgsql/releases", "https://github.com/kenjiuno/Npgsql/releases")) {
                //UnsuggestIt((LinkLabel)sender);
            }
        }

        private void llADONet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (MessageBox.Show(this, "Are you really?", null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                var proj = GetActiveProject();
                if (proj != null) {
                    String fpXml = null;
                    foreach (EnvDTE.ProjectItem pi in proj.ProjectItems) {
                        if (false
                            || "App.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                            || "Web.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                        ) {
                            fpXml = pi.FileNames[1];
                            break;
                        }
                    }
                    if (fpXml != null) {
                        XmlDocument xd = new XmlDocument();
                        xd.Load(fpXml);

                        var el1 = xd.SelectSingleNode("/configuration") as XmlElement;
                        if (el1 != null) {
                            var el2 = el1.SelectSingleNode("system.data") as XmlElement;
                            if (el2 == null) {
                                el2 = xd.CreateElement("system.data");
                                el1.AppendChild(el2);
                            }
                            {
                                var el3 = el2.SelectSingleNode("DbProviderFactories") as XmlElement;
                                if (el3 == null) {
                                    el3 = xd.CreateElement("DbProviderFactories");
                                    el2.AppendChild(el3);
                                }
                                {
                                    {
                                        var el = xd.CreateElement("remove");
                                        el.SetAttribute("invariant", "Npgsql");
                                        el3.AppendChild(el);
                                    }
                                    {
                                        var el = xd.CreateElement("add");
                                        el.SetAttribute("invariant", "Npgsql");
                                        el.SetAttribute("name", "Npgsql Data Provider");
                                        el.SetAttribute("description", ".Net Data Provider for PostgreSQL");
                                        el.SetAttribute("type", "Npgsql.NpgsqlFactory, Npgsql");
                                        el3.AppendChild(el);
                                    }

                                    xd.Save(fpXml);
                                    MessageBox.Show(this, "Done.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    UnsuggestIt((LinkLabel)sender);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show(this, "Not changed.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private void llProvider_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (MessageBox.Show(this, "Are you really?", null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                var proj = GetActiveProject();
                if (proj != null) {
                    String fpXml = null;
                    foreach (EnvDTE.ProjectItem pi in proj.ProjectItems) {
                        if (false
                            || "App.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                            || "Web.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                        ) {
                            fpXml = pi.FileNames[1];
                            break;
                        }
                    }
                    if (fpXml != null) {
                        XmlDocument xd = new XmlDocument();
                        xd.Load(fpXml);

                        var el1 = xd.SelectSingleNode("/configuration") as XmlElement;
                        if (el1 != null) {
                            {
                                var el2 = el1.SelectSingleNode("configSections") as XmlElement;
                                if (el2 == null) {
                                    el2 = xd.CreateElement("configSections");
                                    el1.AppendChild(el2);
                                }
                                {
                                    var el3 = el2.SelectSingleNode("section[@name='entityFramework']") as XmlElement;
                                    if (el3 == null) {
                                        el3 = xd.CreateElement("section");
                                        el3.SetAttribute("name", "entityFramework");
                                        el3.SetAttribute("type", "System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                                        el3.SetAttribute("requirePermission", "false");
                                    }
                                }
                            }
                            {
                                var el2 = el1.SelectSingleNode("entityFramework") as XmlElement;
                                if (el2 == null) {
                                    el2 = xd.CreateElement("entityFramework");
                                    el1.AppendChild(el2);
                                }
                                {
                                    var el3 = el2.SelectSingleNode("providers") as XmlElement;
                                    if (el3 == null) {
                                        el3 = xd.CreateElement("providers");
                                        el2.AppendChild(el3);
                                    }
                                    {
                                        var el4 = el3.SelectSingleNode("provider[@invariantName='Npgsql']") as XmlElement;
                                        if (el4 == null) {
                                            el4 = xd.CreateElement("provider");
                                            el3.AppendChild(el4);
                                        }

                                        {
                                            el4.SetAttribute("invariantName", "Npgsql");
                                            el4.SetAttribute("type", EFv6Provider);

                                            xd.Save(fpXml);
                                            MessageBox.Show(this, "Done.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                            UnsuggestIt((LinkLabel)sender);
                                            return;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show(this, "Not changed.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        const String EFv6Provider = "Npgsql.NpgsqlServices, EntityFramework6.Npgsql";

        private void llEFv5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (MessageBox.Show(this, "Do NuGet now? \n\nTakes some minutes.", null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
                return;

            // https://docs.nuget.org/create/invoking-nuget-services-from-inside-visual-studio

            var proj = GetActiveProject();

            try {
                var componentModel = (IComponentModel)sp.GetService(typeof(SComponentModel));
                IVsPackageInstaller installer = componentModel.GetService<IVsPackageInstaller>();
                installer.InstallPackage(null, proj, "EntityFramework5.Npgsql", typeof(NpgsqlConnection).Assembly.GetName().Version, false);

                UnsuggestIt(llEFv5);
                MessageBox.Show(this, "Done.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err) {
                MessageBox.Show(this, "InstallPackage failed.\n\n" + err, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void llEFv6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (MessageBox.Show(this, "Do NuGet now? \n\nTakes some minutes.", null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
                return;

            var proj = GetActiveProject();

            try {
                var componentModel = (IComponentModel)sp.GetService(typeof(SComponentModel));
                IVsPackageInstaller installer = componentModel.GetService<IVsPackageInstaller>();
                installer.InstallPackage(null, proj, "EntityFramework6.Npgsql", typeof(NpgsqlConnection).Assembly.GetName().Version, false);

                UnsuggestIt(llEFv6);
                MessageBox.Show(this, "Done.", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err) {
                MessageBox.Show(this, "InstallPackage failed.\n\n" + err, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void llConfigSections_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (MessageBox.Show(this, "Are you really?", null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                var proj = GetActiveProject();
                if (proj != null) {
                    String fpXml = null;
                    foreach (EnvDTE.ProjectItem pi in proj.ProjectItems) {
                        if (false
                            || "App.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                            || "Web.config".Equals(pi.Name, StringComparison.InvariantCultureIgnoreCase)
                        ) {
                            fpXml = pi.FileNames[1];
                            break;
                        }
                    }
                    if (fpXml != null) {
                        XmlDocument xd = new XmlDocument();
                        xd.Load(fpXml);

                        var el2 = xd.SelectSingleNode("/configuration/configSections") as XmlElement;
                        if (el2 != null) {
                            if (el2.PreviousSibling as XmlElement != null) {
                                var el1 = el2.ParentNode as XmlElement;
                                el1.PrependChild(el2);

                                xd.Save(fpXml);
                                MessageBox.Show(this, "Done.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                UnsuggestIt((LinkLabel)sender);
                                return;
                            }
                        }
                    }
                }
            }

            MessageBox.Show(this, "Not changed.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }
    }
}