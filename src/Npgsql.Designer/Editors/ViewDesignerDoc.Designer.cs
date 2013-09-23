namespace Npgsql.Designer.Editors
{
  partial class ViewDesignerDoc
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      _editingTables.Remove(GetHashCode());
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this._timer = new System.Windows.Forms.Timer(this.components);
      this._check = new System.Windows.Forms.Timer(this.components);
      this._pg = new System.Windows.Forms.PropertyGrid();
      this.SuspendLayout();
      //
      // _timer
      //
      this._timer.Tick += new System.EventHandler(this._timer_Tick);
      //
      // _check
      //
      this._check.Interval = 200;
      this._check.Tick += new System.EventHandler(this._check_Tick);
      //
      // _pg
      //
      this._pg.Location = new System.Drawing.Point(0, 3);
      this._pg.Name = "_pg";
      this._pg.PropertySort = System.Windows.Forms.PropertySort.NoSort;
      this._pg.Size = new System.Drawing.Size(130, 130);
      this._pg.TabIndex = 2;
      this._pg.ToolbarVisible = false;
      this._pg.Visible = false;
      //
      // ViewDesignerDoc
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._pg);
      this.Font = new System.Drawing.Font("MS Shell Dlg 2", 8.25F);
      this.Name = "ViewDesignerDoc";
      this.Size = new System.Drawing.Size(553, 407);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer _timer;
    private System.Windows.Forms.Timer _check;
    private System.Windows.Forms.PropertyGrid _pg;
  }
}
