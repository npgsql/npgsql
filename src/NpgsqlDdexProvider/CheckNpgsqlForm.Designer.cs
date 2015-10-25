namespace Npgsql.VisualStudio.Provider {
    partial class CheckNpgsqlForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.ToolStripContainer tsc;
            System.Windows.Forms.ToolStrip toolStrip1;
            System.Windows.Forms.ToolStripButton bEFv5;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckNpgsqlForm));
            System.Windows.Forms.ToolStripButton bEFv6;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripButton bCopy;
            this.rtb = new System.Windows.Forms.RichTextBox();
            tsc = new System.Windows.Forms.ToolStripContainer();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            bEFv5 = new System.Windows.Forms.ToolStripButton();
            bEFv6 = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            bCopy = new System.Windows.Forms.ToolStripButton();
            tsc.ContentPanel.SuspendLayout();
            tsc.TopToolStripPanel.SuspendLayout();
            tsc.SuspendLayout();
            toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tsc
            // 
            // 
            // tsc.ContentPanel
            // 
            tsc.ContentPanel.Controls.Add(this.rtb);
            tsc.ContentPanel.Size = new System.Drawing.Size(1005, 383);
            tsc.Dock = System.Windows.Forms.DockStyle.Fill;
            tsc.Location = new System.Drawing.Point(0, 0);
            tsc.Name = "tsc";
            tsc.Size = new System.Drawing.Size(1005, 410);
            tsc.TabIndex = 0;
            // 
            // tsc.TopToolStripPanel
            // 
            tsc.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // rtb
            // 
            this.rtb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rtb.Location = new System.Drawing.Point(0, 0);
            this.rtb.Name = "rtb";
            this.rtb.Size = new System.Drawing.Size(1005, 383);
            this.rtb.TabIndex = 2;
            this.rtb.Text = "";
            this.rtb.WordWrap = false;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            bEFv5,
            bEFv6,
            toolStripSeparator1,
            bCopy});
            toolStrip1.Location = new System.Drawing.Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(337, 27);
            toolStrip1.TabIndex = 0;
            // 
            // bEFv5
            // 
            bEFv5.Image = ((System.Drawing.Image)(resources.GetObject("bEFv5.Image")));
            bEFv5.ImageTransparentColor = System.Drawing.Color.Magenta;
            bEFv5.Name = "bEFv5";
            bEFv5.Size = new System.Drawing.Size(92, 24);
            bEFv5.Text = "Test EFv5";
            bEFv5.Click += new System.EventHandler(this.bEFv5_Click);
            // 
            // bEFv6
            // 
            bEFv6.Image = ((System.Drawing.Image)(resources.GetObject("bEFv6.Image")));
            bEFv6.ImageTransparentColor = System.Drawing.Color.Magenta;
            bEFv6.Name = "bEFv6";
            bEFv6.Size = new System.Drawing.Size(92, 24);
            bEFv6.Text = "Test EFv6";
            bEFv6.Click += new System.EventHandler(this.bEFv6_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bCopy
            // 
            bCopy.Image = ((System.Drawing.Image)(resources.GetObject("bCopy.Image")));
            bCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            bCopy.Name = "bCopy";
            bCopy.Size = new System.Drawing.Size(96, 24);
            bCopy.Text = "&Copy text";
            bCopy.Click += new System.EventHandler(this.bCopy_Click);
            // 
            // CheckNpgsqlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 410);
            this.Controls.Add(tsc);
            this.Name = "CheckNpgsqlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CheckNpgsqlForm";
            this.Load += new System.EventHandler(this.CheckNpgsqlForm_Load);
            tsc.ContentPanel.ResumeLayout(false);
            tsc.TopToolStripPanel.ResumeLayout(false);
            tsc.TopToolStripPanel.PerformLayout();
            tsc.ResumeLayout(false);
            tsc.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb;
    }
}