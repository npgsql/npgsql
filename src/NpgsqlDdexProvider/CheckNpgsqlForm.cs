using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void bEFv5_Click(object sender, EventArgs e) {
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
            {
                Log("Check version of Npgsql in NpgsqlDdexProvider... ");

                var dbf1 = Npgsql.NpgsqlFactory.Instance;

                Log(dbf1.GetType().Assembly.FullName, Color.Blue);
                Newl();

                {
                    Log("Check version of Npgsql in <DbProviderFactories>... ");

                    var dbf2 = System.Data.Common.DbProviderFactories.GetFactory("Npgsql");

                    if (dbf2 != null) {
                        Log(dbf2.GetType().Assembly.FullName, Color.Blue);
                        Newl();

                        {
                            Log("Check location of Npgsql assembly in NpgsqlDdexProvider... ");

                            var loc1 = dbf1.GetType().Assembly.Location;
                            Log(loc1, Color.Blue);
                            Newl();

                            Log("Check location of Npgsql assembly in <DbProviderFactories>... ");

                            var loc2 = dbf2.GetType().Assembly.Location;
                            Log(loc2, Color.Blue);
                            Newl();

                            Log("Check if 2 Npgsql assemblies are equivalent... ");
                            if (loc1.Equals(loc2)) {
                                Log("Yes", Color.Blue);
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
    }
}
