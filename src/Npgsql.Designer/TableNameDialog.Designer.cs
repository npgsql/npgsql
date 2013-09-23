namespace Npgsql.Designer
{
  partial class TableNameDialog
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
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.Button _ok;
      System.Windows.Forms.Button _cancel;
      this.label1 = new System.Windows.Forms.Label();
      this._name = new System.Windows.Forms.TextBox();
      _ok = new System.Windows.Forms.Button();
      _cancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      //
      // _ok
      //
      _ok.Location = new System.Drawing.Point(211, 51);
      _ok.Name = "_ok";
      _ok.Size = new System.Drawing.Size(75, 23);
      _ok.TabIndex = 2;
      _ok.Text = "OK";
      _ok.UseVisualStyleBackColor = true;
      _ok.Click += new System.EventHandler(this._ok_Click);
      //
      // _cancel
      //
      _cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      _cancel.Location = new System.Drawing.Point(292, 51);
      _cancel.Name = "_cancel";
      _cancel.Size = new System.Drawing.Size(75, 23);
      _cancel.TabIndex = 3;
      _cancel.Text = "Cancel";
      _cancel.UseVisualStyleBackColor = true;
      _cancel.Click += new System.EventHandler(this._cancel_Click);
      //
      // label1
      //
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(126, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Choose a &name for this %";
      //
      // _name
      //
      this._name.Location = new System.Drawing.Point(12, 25);
      this._name.Name = "_name";
      this._name.Size = new System.Drawing.Size(355, 20);
      this._name.TabIndex = 1;
      //
      // TableNameDialog
      //
      this.AcceptButton = _ok;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = _cancel;
      this.ClientSize = new System.Drawing.Size(379, 86);
      this.Controls.Add(this._name);
      this.Controls.Add(this.label1);
      this.Controls.Add(_cancel);
      this.Controls.Add(_ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "TableNameDialog";
      this.Font = new System.Drawing.Font("MS Shell Dlg 2", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "% Name";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox _name;

  }
}
