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
  using System.Windows.Forms.Design;
  using Microsoft.VisualStudio.Shell.Interop;
  using Microsoft.VisualStudio;
  using System.Data.Common;

  public partial class ChangePasswordDialog : Form
  {
    internal string Password = null;

    private NpgsqlConnectionProperties _props;

    private string GetCurrentPassword()
    {
      try
      {
        return _props["Password"] as string;
      }
      catch
      {
        return String.Empty;
      }
    }

    internal ChangePasswordDialog(NpgsqlConnectionProperties props)
    {
      _props = props;
      InitializeComponent();

      password.Text = GetCurrentPassword();
    }

    private void password_TextChanged(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(password.Text) || password.Text == GetCurrentPassword())
      {
        confirmLabel.Enabled = false;
        passwordConfirm.Enabled = false;
        passwordConfirm.Text = "";

        if (String.IsNullOrEmpty(password.Text) && String.IsNullOrEmpty(GetCurrentPassword()) == false)
          action.Text = VSPackage.Decrypt;
        else
          action.Text = "";
      }
      else
      {
        confirmLabel.Enabled = true;
        passwordConfirm.Enabled = true;

        if (String.IsNullOrEmpty(GetCurrentPassword()) == false)
          action.Text = VSPackage.ReEncrypt;
        else
          action.Text = VSPackage.Encrypt;
      }

      okButton.Enabled = (password.Text == passwordConfirm.Text);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      Password = password.Text;
      DialogResult = DialogResult.OK;
      Close();
    }
  }
}
