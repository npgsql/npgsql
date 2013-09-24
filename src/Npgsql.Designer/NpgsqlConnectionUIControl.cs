/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Drawing;
  using System.Text;
  using System.Windows.Forms;
  using Microsoft.VisualStudio.Data;
  using Microsoft.Win32;

  /// <summary>
  /// Provides a UI to edit/create SQLite database connections
  /// </summary>
  [ToolboxItem(false)]
  public partial class NpgsqlConnectionUIControl : DataConnectionUIControl
  {
    public NpgsqlConnectionUIControl()
    {
      InitializeComponent();
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.FileName = fileTextBox.Text;
      dlg.Title = "Select SQLite Database File";

      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        fileTextBox.Text = dlg.FileName;
        fileTextBox_Leave(sender, e);
      }
    }

    private void newDatabase_Click(object sender, EventArgs e)
    {
      SaveFileDialog dlg = new SaveFileDialog();
      dlg.Title = "Create SQLite Database File";
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        fileTextBox.Text = dlg.FileName;
        fileTextBox_Leave(sender, e);
      }
    }

    #region IDataConnectionUIControl Members

    public override void LoadProperties()
    {
      if (ConnectionProperties.Contains("data source"))
        fileTextBox.Text = ConnectionProperties["data source"] as string;

      if (ConnectionProperties.Contains("password"))
        passwordTextBox.Text = ConnectionProperties["password"] as string;
    }

    #endregion

    private void passwordTextBox_Leave(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(passwordTextBox.Text))
        ConnectionProperties.Remove("password");
      else
        ConnectionProperties["password"] = passwordTextBox.Text;
    }

    private void encoding_Changed(object sender, EventArgs e)
    {
      if (utf8RadioButton.Checked == true)
        ConnectionProperties.Remove("useutf16encoding");
      else
        ConnectionProperties["useutf16encoding"] = utf16RadioButton.Checked;
    }

    private void datetime_Changed(object sender, EventArgs e)
    {
      if (iso8601RadioButton.Checked == true)
        ConnectionProperties.Remove("datetimeformat");
      else if (ticksRadioButton.Checked == true)
        ConnectionProperties["datetimeformat"] = "Ticks";
      else
        ConnectionProperties["datetimeformat"] = "JulianDay";
    }

    private void sync_Changed(object sender, EventArgs e)
    {
      string sync = "Normal";
      if (fullRadioButton.Checked == true) sync = "Full";
      else if (offRadioButton.Checked == true) sync = "Off";

      if (sync == "Normal")
        ConnectionProperties.Remove("synchronous");
      else
        ConnectionProperties["synchronous"] = sync;
    }

    private void pageSizeTextBox_Leave(object sender, EventArgs e)
    {
      int n = Convert.ToInt32(pageSizeTextBox.Text);
      ConnectionProperties["page size"] = n;
    }

    private void cacheSizeTextbox_Leave(object sender, EventArgs e)
    {
      int n = Convert.ToInt32(cacheSizeTextbox.Text);
      ConnectionProperties["cache size"] = n;
    }

    private void fileTextBox_Leave(object sender, EventArgs e)
    {
      ConnectionProperties["data source"] = fileTextBox.Text;
    }
  }
}
