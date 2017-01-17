using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Framework;

namespace Npgsql.VSIX
{
    public partial class NpgsqlConnectionUIControl : DataConnectionUIControl
    {
        public NpgsqlConnectionUIControl()
        {
            InitializeComponent();
        }

        public override void LoadProperties()
        {
            try
            {
                hostTextBox.Text = (string)Site["Host"];
                portNumericUpDown.Text = ((int)Site["Port"]).ToString();
                databaseTextBox.Text = (string)Site["Database"];
                windowsAuthCheckbox.Checked = (bool)Site["Integrated Security"];
                if (!windowsAuthCheckbox.Checked)
                {
                    usernameTextBox.Text = (string)Site["Username"];
                    passwordTextBox.Text = (string)Site["Password"];
                    savePasswordCheckBox.Checked = (bool)Site["Persist Security Info"];
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        void SetHost(object sender, EventArgs e) => Site["Host"] = hostTextBox.Text;
        void SetPort(object sender, EventArgs e) => Site["Port"] = portNumericUpDown.Value;
        void SetDatabase(object sender, EventArgs e) => Site["Database"] = databaseTextBox.Text;
        void SetUsername(object sender, EventArgs e) => Site["Username"] = usernameTextBox.Text;
        void SetPassword(object sender, EventArgs e) => Site["Password"] = passwordTextBox.Text;
        void SetSavePassword(object sender, EventArgs e) => Site["Persist Security Info"] = savePasswordCheckBox.Checked;

        void SetWindowsAuth(object sender, EventArgs e)
        {
            Site["Integrated Security"] = windowsAuthCheckbox.Checked;
            if (windowsAuthCheckbox.Checked)
            {
                usernameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
                savePasswordCheckBox.Enabled = false;
            }
            else
            {
                usernameTextBox.Enabled = true;
                passwordTextBox.Enabled = true;
                savePasswordCheckBox.Enabled = true;
            }
        }
    }
}
