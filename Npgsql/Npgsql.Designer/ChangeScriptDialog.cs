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
  using Npgsql.Designer.Design;

  public partial class ChangeScriptDialog : Form
  {
    private string _tableName;
    private static bool _defaultSave;

    public ChangeScriptDialog(string tableName, string script, string original)
    {
      _tableName = tableName;
      InitializeComponent();

      _script.Text = script;
      _original.Text = original;

      _saveOrig.Checked = _defaultSave;

      if (String.IsNullOrEmpty(original) || String.IsNullOrEmpty(script))
      {
        int increase = _splitter.Top - _show.Top;
        _show.Visible = false;
        _splitter.Top = _show.Top;
        _splitter.Height += increase;
        _saveOrig.Visible = false;
      }

      if (String.IsNullOrEmpty(script) == false)
        _splitter.Panel1Collapsed = true;
      else
        _splitter.Panel2Collapsed = true;
    }

    private void noButton_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void yesButton_Click(object sender, EventArgs e)
    {
      using (SaveFileDialog save = new SaveFileDialog())
      {
        save.DefaultExt = "sql";
        save.OverwritePrompt = true;
        save.Filter = "SQL Script Files (*.sql)|*.sql|All Files (*.*)|*.*";
        save.FileName = String.Format("{0}.sql", _tableName);
        save.Title = "Save SQLite Change Script";

        DialogResult = save.ShowDialog(this);

        if (DialogResult == DialogResult.OK)
        {
          _defaultSave = _saveOrig.Checked;

          using (System.IO.StreamWriter writer = new System.IO.StreamWriter(save.FileName, false, Encoding.UTF8))
          {
            if ((_show.Visible == true && _saveOrig.Checked == true) || (_show.Visible == false && _splitter.Panel2Collapsed == true))
            {
              if (_show.Visible == true) writer.WriteLine("/*");
              writer.WriteLine(_original.Text.Replace("\r", "").TrimEnd('\n').Replace("\n", "\r\n"));
              if (_show.Visible == true) writer.WriteLine("*/");
            }
            if (_show.Visible == true || _splitter.Panel2Collapsed == false)
              writer.WriteLine(_script.Text.Replace("\r", "").TrimEnd('\n').Replace("\n", "\r\n"));
          }
        }
      }
      Close();
    }

    private void _show_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      string old = _show.Text;
      _show.Text = _show.Tag.ToString();
      _show.Tag = old;

      if (_splitter.IsSplitterFixed)
      {
        _splitter.IsSplitterFixed = false;
        _splitter.Panel1Collapsed = false;
      }
      else
      {
        _splitter.IsSplitterFixed = true;
        _splitter.Panel1Collapsed = true;
      }
    }
  }
}
