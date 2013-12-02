namespace Npgsql.Designer
{
  partial class ChangeScriptDialog
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
      System.Windows.Forms.PictureBox pictureBox1;
      System.Windows.Forms.Label label1;
      System.Windows.Forms.Button noButton;
      System.Windows.Forms.Button yesButton;
      this._script = new System.Windows.Forms.RichTextBox();
      this._splitter = new System.Windows.Forms.SplitContainer();
      this._show = new System.Windows.Forms.LinkLabel();
      this._original = new System.Windows.Forms.RichTextBox();
      this._saveOrig = new System.Windows.Forms.CheckBox();
      pictureBox1 = new System.Windows.Forms.PictureBox();
      label1 = new System.Windows.Forms.Label();
      noButton = new System.Windows.Forms.Button();
      yesButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
      this._splitter.Panel1.SuspendLayout();
      this._splitter.Panel2.SuspendLayout();
      this._splitter.SuspendLayout();
      this.SuspendLayout();
      //
      // pictureBox1
      //
      pictureBox1.Image = global::Npgsql.Designer.VSPackage.info;
      pictureBox1.Location = new System.Drawing.Point(13, 13);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new System.Drawing.Size(48, 48);
      pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      pictureBox1.TabIndex = 0;
      pictureBox1.TabStop = false;
      //
      // label1
      //
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(67, 31);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(200, 13);
      label1.TabIndex = 1;
      label1.Text = "Do you want to save this script to a file?";
      //
      // noButton
      //
      noButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      noButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      noButton.Location = new System.Drawing.Point(445, 362);
      noButton.Name = "noButton";
      noButton.Size = new System.Drawing.Size(75, 25);
      noButton.TabIndex = 3;
      noButton.Text = "&No";
      noButton.UseVisualStyleBackColor = true;
      noButton.Click += new System.EventHandler(this.noButton_Click);
      //
      // yesButton
      //
      yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      yesButton.Location = new System.Drawing.Point(364, 362);
      yesButton.Name = "yesButton";
      yesButton.Size = new System.Drawing.Size(75, 25);
      yesButton.TabIndex = 4;
      yesButton.Text = "&Yes";
      yesButton.UseVisualStyleBackColor = true;
      yesButton.Click += new System.EventHandler(this.yesButton_Click);
      //
      // _script
      //
      this._script.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this._script.Dock = System.Windows.Forms.DockStyle.Fill;
      this._script.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._script.Location = new System.Drawing.Point(0, 0);
      this._script.Name = "_script";
      this._script.ReadOnly = true;
      this._script.Size = new System.Drawing.Size(508, 134);
      this._script.TabIndex = 2;
      this._script.Text = "";
      this._script.WordWrap = false;
      //
      // _splitter
      //
      this._splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._splitter.IsSplitterFixed = true;
      this._splitter.Location = new System.Drawing.Point(12, 80);
      this._splitter.Name = "_splitter";
      this._splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
      //
      // _splitter.Panel1
      //
      this._splitter.Panel1.Controls.Add(this._original);
      this._splitter.Panel1MinSize = 0;
      //
      // _splitter.Panel2
      //
      this._splitter.Panel2.Controls.Add(this._script);
      this._splitter.Panel2MinSize = 0;
      this._splitter.Size = new System.Drawing.Size(508, 276);
      this._splitter.SplitterDistance = 138;
      this._splitter.TabIndex = 5;
      //
      // _show
      //
      this._show.AutoSize = true;
      this._show.Location = new System.Drawing.Point(12, 64);
      this._show.Name = "_show";
      this._show.Size = new System.Drawing.Size(118, 13);
      this._show.TabIndex = 7;
      this._show.TabStop = true;
      this._show.Tag = "<< &Hide original script";
      this._show.Text = "&Show original script >>";
      this._show.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._show_LinkClicked);
      //
      // _original
      //
      this._original.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this._original.Dock = System.Windows.Forms.DockStyle.Fill;
      this._original.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._original.Location = new System.Drawing.Point(0, 0);
      this._original.Name = "_original";
      this._original.ReadOnly = true;
      this._original.Size = new System.Drawing.Size(508, 138);
      this._original.TabIndex = 3;
      this._original.Text = "";
      this._original.WordWrap = false;
      //
      // _saveOrig
      //
      this._saveOrig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._saveOrig.AutoSize = true;
      this._saveOrig.Location = new System.Drawing.Point(183, 367);
      this._saveOrig.Name = "_saveOrig";
      this._saveOrig.Size = new System.Drawing.Size(175, 17);
      this._saveOrig.TabIndex = 8;
      this._saveOrig.Text = "Save &original SQL with changes";
      this._saveOrig.UseVisualStyleBackColor = true;
      //
      // ChangeScriptDialog
      //
      this.AcceptButton = yesButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = noButton;
      this.ClientSize = new System.Drawing.Size(532, 399);
      this.Controls.Add(this._saveOrig);
      this.Controls.Add(this._show);
      this.Controls.Add(this._splitter);
      this.Controls.Add(yesButton);
      this.Controls.Add(noButton);
      this.Controls.Add(label1);
      this.Controls.Add(pictureBox1);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "ChangeScriptDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Save SQL Script";
      ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
      this._splitter.Panel1.ResumeLayout(false);
      this._splitter.Panel2.ResumeLayout(false);
      this._splitter.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox _script;
    private System.Windows.Forms.LinkLabel _show;
    private System.Windows.Forms.RichTextBox _original;
    private System.Windows.Forms.SplitContainer _splitter;
    private System.Windows.Forms.CheckBox _saveOrig;

  }
}
