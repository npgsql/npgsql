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

        }

        private void bEFv5_Click(object sender, EventArgs e) {
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
            }
            catch (Exception err) {
                Log("" + err, Color.Purple);
            }
        }

        private void bEFv6_Click(object sender, EventArgs e) {
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
            }
            catch (Exception err) {
                Log("" + err, Color.Purple);
            }
        }
    }
}
