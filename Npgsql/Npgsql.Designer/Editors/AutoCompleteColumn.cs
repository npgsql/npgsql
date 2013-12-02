/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer.Editors
{
  using System;
  using System.Windows.Forms;
  using System.Runtime.InteropServices;
  using System.Drawing;

  public class AutoCompleteColumn : DataGridViewColumn
  {
    public AutoCompleteColumn()
      : base(new AutoCompleteCell())
    {
    }
  }

  public class AutoCompleteCell : DataGridViewTextBoxCell
  {
    public override Type EditType
    {
      get
      {
        return typeof(AutoCompleteEditingControl);
      }
    }
  }

  public class AutoCompleteEditingControl : DataGridViewComboBoxEditingControl
  {
    private bool inPrepare = false;
    private bool isDeleting = false;

    public override object EditingControlFormattedValue
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value as string;
      }
    }

    public override void PrepareEditingControlForEdit(bool selectAll)
    {
      inPrepare = true;
      base.PrepareEditingControlForEdit(selectAll);
      if (base.Items.Count == 0)
      {
        base.Items.Add("integer");
        base.Items.Add("int");
        base.Items.Add("smallint");
        base.Items.Add("tinyint");
        base.Items.Add("bit");
        base.Items.Add("varchar(50)");
        base.Items.Add("nvarchar(50)");
        base.Items.Add("text");
        base.Items.Add("ntext");
        base.Items.Add("image");
        base.Items.Add("money");
        base.Items.Add("float");
        base.Items.Add("real");
        base.Items.Add("decimal");
        base.Items.Add("numeric(18,0)");
        base.Items.Add("char(10)");
        base.Items.Add("nchar(10)");
        base.Items.Add("datetime");
        base.Items.Add("guid");
      }
      base.DropDownStyle = ComboBoxStyle.DropDown;
      base.Text = EditingControlDataGridView.CurrentCell.Value as string;

      if (selectAll)
        base.SelectAll();

      inPrepare = false;
    }

    public override bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
    {
      isDeleting = false;

      switch (keyData & Keys.KeyCode)
      {
        case Keys.Delete:
        case Keys.Back:
          isDeleting = true;
          break;
      }

      if (((keyData & Keys.KeyCode) == Keys.Left && (base.SelectionStart > 0 ||  base.SelectionLength > 0))
        || ((keyData & Keys.KeyCode) == Keys.Right && (base.SelectionStart < base.Text.Length ||  base.SelectionLength > 0))
        || (((keyData & Keys.KeyCode) == Keys.Home || (keyData & Keys.KeyCode) == Keys.End) && base.SelectionLength != base.Text.Length)
        )
      {
        return true;
      }

      return base.EditingControlWantsInputKey(keyData, dataGridViewWantsInputKey);
    }

    protected override void OnTextChanged(EventArgs e)
    {
      if (inPrepare) return;
      base.OnTextChanged(e);

      bool changed = !(EditingControlDataGridView.CurrentCell.Value as string == base.Text ||
                     (String.IsNullOrEmpty(base.Text) && String.IsNullOrEmpty(EditingControlDataGridView.CurrentCell.Value as string)));

      if ((base.SelectionLength == 0 || base.SelectionStart == base.Text.Length) && isDeleting == false)
      {
        if (base.Items.Contains(base.Text) == false)
        {
          for (int n = 0; n < base.Items.Count; n++)
          {
            if (((string)base.Items[n]).StartsWith(base.Text, StringComparison.OrdinalIgnoreCase) == true)
            {
              int start = base.SelectionStart;
              inPrepare = true;

              base.Text = base.Items[n] as string;
              base.SelectionStart = start;
              base.SelectionLength = base.Text.Length - start;

              inPrepare = false;
              break;
            }
          }
        }
      }
      EditingControlValueChanged = changed;
      EditingControlDataGridView.NotifyCurrentCellDirty(changed);
    }
  }
}
