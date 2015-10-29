using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        }

        private void bEFv5_Click(object sender, EventArgs e) {
            tabControl1.SelectedTab = tabPage2;

            rtb.Clear();

            try {
                {
                    Log("Check your active project... ");

                    var proj = GetActiveProject();
                    Log(proj.Name, Color.Blue);

                    Newl();

                }
                {
                    Log("Check if Npgsql is registered in <DbProviderFactories>... ");

                    var dbf = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");

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
                        Log("Check version of Npgsql from DbProviderFactories.GetFactory... ");

                        var dbf2 = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");

                        if (dbf2 != null) {
                            Log(dbf2.GetType().Assembly.FullName, Color.Blue);
                            Newl();

                            {
                                Log("Check location of Npgsql assembly in NpgsqlDdexProvider... ");

                                var loc1 = dbf1.GetType().Assembly.Location;
                                Log(loc1, Color.Blue);
                                Newl();

                                Log("Check location of Npgsql assembly from DbProviderFactories.GetFactory... ");

                                var loc2 = dbf2.GetType().Assembly.Location;
                                Log(loc2, Color.Blue);
                                Newl();

                                Log("Check if 2 Npgsql assemblies are equivalent... ");
                                if (loc1.Equals(loc2)) {
                                    Log("Yes", Color.Blue);
                                    Newl();

                                    {
                                        Log("Check Npgsql assembly version... ");

                                        verNpgsql = new AssemblyName(dbf2.GetType().Assembly.FullName);

                                        Log(verNpgsql.Version + "", Color.Blue);
                                    }
                                }
                                else {
                                    Log("No", Color.Red);

                                    SuggestIt(llGrab);
                                }
                                Newl();
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

                            {
                                Version need = new Version("5.0.0.0");
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
                                }
                                else if (need == found) {
                                    Log("Yes", Color.Blue);
                                    Newl();
                                }
                                else {
                                    Log("No, " + found + " is referenced", Color.Red);
                                    Newl();
                                }
                            }

                            if (verNpgsql != null && verNpgsql.Version != null) {
                                var need = verNpgsql.Version;
                                var name = "EntityFramework5.Npgsql";

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
                                }
                                else if (need == found) {
                                    Log("Yes", Color.Blue);
                                    Newl();
                                }
                                else {
                                    Log("No, " + found + " is referenced", Color.Red);
                                    Newl();
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
                                                                    && NpgsqlFactory.Equals("Npgsql.NpgsqlFactory, Npgsql")
                                                                ) {
                                                                    Log("Yes", Color.Blue);
                                                                    Newl();
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
                                                Log("No", Color.Red);
                                                Newl();
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
        private void SuggestIt(LinkLabel ll) {
            ll.Text = "⇒" + ll.Text.Substring(1);
        }
        private void UnsuggestIt(LinkLabel ll) {
            ll.Text = " " + ll.Text.Substring(1);
        }

        private void bEFv6_Click(object sender, EventArgs e) {
            tabControl1.SelectedTab = tabPage2;

            rtb.Clear();

            try {
                {
                    Log("Check your active project... ");

                    var proj = GetActiveProject();
                    Log(proj.Name, Color.Blue);

                    Newl();

                }
                {
                    Log("Check if Npgsql is registered in <DbProviderFactories>... ");

                    var dbf = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");

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
                        Log("Check version of Npgsql from DbProviderFactories.GetFactory... ");

                        var dbf2 = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");

                        if (dbf2 != null) {
                            Log(dbf2.GetType().Assembly.FullName, Color.Blue);
                            Newl();

                            {
                                Log("Check location of Npgsql assembly in NpgsqlDdexProvider... ");

                                var loc1 = dbf1.GetType().Assembly.Location;
                                Log(loc1, Color.Blue);
                                Newl();

                                Log("Check location of Npgsql assembly from DbProviderFactories.GetFactory... ");

                                var loc2 = dbf2.GetType().Assembly.Location;
                                Log(loc2, Color.Blue);
                                Newl();

                                Log("Check if 2 Npgsql assemblies are equivalent... ");
                                if (loc1.Equals(loc2)) {
                                    Log("Yes", Color.Blue);
                                    Newl();

                                    {
                                        Log("Check Npgsql assembly version... ");

                                        verNpgsql = new AssemblyName(dbf2.GetType().Assembly.FullName);

                                        Log(verNpgsql.Version + "", Color.Blue);
                                    }
                                }
                                else {
                                    Log("No", Color.Red);
                                }
                                Newl();
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

                            {
                                Version need = new Version("6.0.0.0");
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
                                }
                                else if (need == found) {
                                    Log("Yes", Color.Blue);
                                    Newl();
                                }
                                else {
                                    Log("No, " + found + " is referenced", Color.Red);
                                    Newl();
                                }
                            }

                            if (verNpgsql != null && verNpgsql.Version != null) {
                                var need = verNpgsql.Version;
                                var name = "EntityFramework6.Npgsql";

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
                                }
                                else if (need == found) {
                                    Log("Yes", Color.Blue);
                                    Newl();
                                }
                                else {
                                    Log("No, " + found + " is referenced", Color.Red);
                                    Newl();
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
                                                                        && NpgsqlFactory.Equals("Npgsql.NpgsqlFactory, Npgsql")
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
                                                else {
                                                    Log("No", Color.Red);
                                                    Newl();
                                                }
                                            }
                                            {
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

                                                        Log("Check <provider>... ");
                                                        var el4 = el3.SelectSingleNode("provider") as XmlElement;
                                                        if (el4 != null) {
                                                            Log("Yes", Color.Blue);
                                                            Newl();

                                                            Log("Check <provider invariantName=\"Npgsql\" type=\"Npgsql.NpgsqlServices, Npgsql.EntityFramework\">... ");
                                                            var el5 = el4.SelectSingleNode("provider[@invariantName='Npgsql' and @type='Npgsql.NpgsqlServices, Npgsql.EntityFramework']") as XmlElement;
                                                            if (el5 != null) {
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

        private void TryOpen(String url) {
            if (MessageBox.Show(this, "Open?\n\n" + url, null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                System.Diagnostics.Process.Start(url);
            }
        }

        private void llGrab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            TryOpen("https://github.com/npgsql/npgsql/releases");
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
                                            var el = xd.CreateElement("provider");
                                            el.SetAttribute("invariantName", "Npgsql");
                                            el.SetAttribute("type", "Npgsql.NpgsqlServices, Npgsql.EntityFramework");
                                            el3.AppendChild(el);

                                            xd.Save(fpXml);
                                            MessageBox.Show(this, "Done.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
    }
}