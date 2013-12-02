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

  public partial class TableNameDialog : Form
  {
    public TableNameDialog()
      : this("Table", null)
    {
    }

    public TableNameDialog(string type, string defaultName)
    {
      InitializeComponent();
      Text = Text.Replace("%", type);
      label1.Text = label1.Text.Replace("%", type.ToLower());
      _name.Text = defaultName;
    }

    public string TableName
    {
      get { return _name.Text; }
      set { _name.Text = value; }
    }

    private void _ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void _cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }
  }
}
