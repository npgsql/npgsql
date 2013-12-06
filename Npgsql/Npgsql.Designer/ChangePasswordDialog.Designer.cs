namespace Npgsql.Designer
{
  partial class ChangePasswordDialog
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
      System.Windows.Forms.Button cancelButton;
      System.Windows.Forms.Label label1;
      this.okButton = new System.Windows.Forms.Button();
      this.confirmLabel = new System.Windows.Forms.Label();
      this.password = new System.Windows.Forms.TextBox();
      this.passwordConfirm = new System.Windows.Forms.TextBox();
      this.action = new System.Windows.Forms.Label();
      cancelButton = new System.Windows.Forms.Button();
      label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      //
      // cancelButton
      //
      cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      cancelButton.Location = new System.Drawing.Point(212, 134);
      cancelButton.Name = "cancelButton";
      cancelButton.Size = new System.Drawing.Size(75, 23);
      cancelButton.TabIndex = 0;
      cancelButton.Text = "Cancel";
      cancelButton.UseVisualStyleBackColor = true;
      //
      // label1
      //
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(28, 15);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(77, 13);
      label1.TabIndex = 2;
      label1.Text = "New &Password";
      //
      // okButton
      //
      this.okButton.Location = new System.Drawing.Point(131, 134);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 1;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      //
      // confirmLabel
      //
      this.confirmLabel.AutoSize = true;
      this.confirmLabel.Enabled = false;
      this.confirmLabel.Location = new System.Drawing.Point(12, 42);
      this.confirmLabel.Name = "confirmLabel";
      this.confirmLabel.Size = new System.Drawing.Size(93, 13);
      this.confirmLabel.TabIndex = 4;
      this.confirmLabel.Text = "&Confirm Password";
      //
      // password
      //
      this.password.Location = new System.Drawing.Point(111, 12);
      this.password.Name = "password";
      this.password.Size = new System.Drawing.Size(176, 21);
      this.password.TabIndex = 3;
      this.password.UseSystemPasswordChar = true;
      this.password.TextChanged += new System.EventHandler(this.password_TextChanged);
      //
      // passwordConfirm
      //
      this.passwordConfirm.Enabled = false;
      this.passwordConfirm.Location = new System.Drawing.Point(111, 39);
      this.passwordConfirm.Name = "passwordConfirm";
      this.passwordConfirm.Size = new System.Drawing.Size(176, 21);
      this.passwordConfirm.TabIndex = 5;
      this.passwordConfirm.UseSystemPasswordChar = true;
      this.passwordConfirm.TextChanged += new System.EventHandler(this.password_TextChanged);
      //
      // action
      //
      this.action.Location = new System.Drawing.Point(39, 74);
      this.action.Name = "action";
      this.action.Size = new System.Drawing.Size(220, 46);
      this.action.TabIndex = 6;
      //
      // ChangePasswordDialog
      //
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = cancelButton;
      this.ClientSize = new System.Drawing.Size(299, 169);
      this.Controls.Add(this.action);
      this.Controls.Add(this.passwordConfirm);
      this.Controls.Add(this.confirmLabel);
      this.Controls.Add(this.password);
      this.Controls.Add(label1);
      this.Controls.Add(this.okButton);
      this.Controls.Add(cancelButton);
      this.Font = new System.Drawing.Font("MS Shell Dlg 2", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChangePasswordDialog";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Change SQLite Database Password";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox passwordConfirm;
    private System.Windows.Forms.Label action;
    private System.Windows.Forms.TextBox password;
    private System.Windows.Forms.Label confirmLabel;
    private System.Windows.Forms.Button okButton;

  }
}
