using System;
using System.Diagnostics;
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
            _loading = true;
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
            finally
            {
                _loading = false;
            }
        }

        void SetProperty(object sender, EventArgs e)
        {
            if (_loading)
                return;  // TODO: Match with sample?

            if (sender == hostTextBox)
                Site["Host"] = hostTextBox.Text.Trim();
            else if (sender == portNumericUpDown)
                Site["Port"] = portNumericUpDown.Value;
            else if (sender == databaseTextBox)
                Site["Database"] = databaseTextBox.Text;
            else if (sender == usernameTextBox)
                Site["Username"] = usernameTextBox.Text;
            else if (sender == passwordTextBox)
                Site["Password"] = passwordTextBox.Text;
            else if (sender == savePasswordCheckBox)
                Site["Persist Security Info"] = savePasswordCheckBox.Checked;

            // TODO: Authentication!
        }
        #region Private Fields

        /// <summary>
        /// It is necessary that we keep track of whether properties are
        /// currently being loaded or not.  This is because the events
        /// fired by each control that cause the SetProperty method to
        /// be called are typically called when the text changes or the
        /// value of the control is altered.  This happens when loading
        /// the properties and when a user sets them.  In the case of
        /// loading, we do not want to update the underlying connection
        /// properties instance with the value so we set this to true
        /// during load time so that SetProperty only causes UI state
        /// changes and does not write to the connection properties.
        /// </summary>
        bool _loading = false;

        #endregion
    }
}
