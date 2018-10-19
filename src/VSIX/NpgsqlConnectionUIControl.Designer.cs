namespace Npgsql.VSIX
{
    partial class NpgsqlConnectionUIControl
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
            this.hostLabel = new System.Windows.Forms.Label();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.windowsAuthCheckbox = new System.Windows.Forms.CheckBox();
            this.credentialsGroupBox = new System.Windows.Forms.GroupBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.databaseTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.portNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.portLabel = new System.Windows.Forms.Label();
            this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.credentialsGroupBox.SuspendLayout();
            this.connectionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(10, 32);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(81, 32);
            this.hostLabel.TabIndex = 0;
            this.hostLabel.Text = "Host:";
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(161, 29);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(326, 38);
            this.hostTextBox.TabIndex = 1;
            this.hostTextBox.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // windowsAuthCheckbox
            // 
            this.windowsAuthCheckbox.AutoSize = true;
            this.windowsAuthCheckbox.Location = new System.Drawing.Point(14, 39);
            this.windowsAuthCheckbox.Name = "windowsAuthCheckbox";
            this.windowsAuthCheckbox.Size = new System.Drawing.Size(358, 36);
            this.windowsAuthCheckbox.TabIndex = 4;
            this.windowsAuthCheckbox.Text = "Windows Authentication";
            this.windowsAuthCheckbox.UseVisualStyleBackColor = true;
            this.windowsAuthCheckbox.CheckedChanged += new System.EventHandler(this.SetProperty);
            // 
            // credentialsGroupBox
            // 
            this.credentialsGroupBox.Controls.Add(this.savePasswordCheckBox);
            this.credentialsGroupBox.Controls.Add(this.passwordTextBox);
            this.credentialsGroupBox.Controls.Add(this.passwordLabel);
            this.credentialsGroupBox.Controls.Add(this.windowsAuthCheckbox);
            this.credentialsGroupBox.Controls.Add(this.usernameLabel);
            this.credentialsGroupBox.Controls.Add(this.usernameTextBox);
            this.credentialsGroupBox.Location = new System.Drawing.Point(0, 160);
            this.credentialsGroupBox.Name = "credentialsGroupBox";
            this.credentialsGroupBox.Size = new System.Drawing.Size(496, 190);
            this.credentialsGroupBox.TabIndex = 1;
            this.credentialsGroupBox.TabStop = false;
            this.credentialsGroupBox.Text = "Credentials";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(176, 109);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(188, 38);
            this.passwordTextBox.TabIndex = 6;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(10, 117);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(147, 32);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "Password:";
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(10, 78);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(160, 32);
            this.usernameLabel.TabIndex = 1;
            this.usernameLabel.Text = "User name:";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(176, 72);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(188, 38);
            this.usernameTextBox.TabIndex = 5;
            this.usernameTextBox.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // connectionGroupBox
            // 
            this.connectionGroupBox.Controls.Add(this.databaseTextBox);
            this.connectionGroupBox.Controls.Add(this.label1);
            this.connectionGroupBox.Controls.Add(this.portNumericUpDown);
            this.connectionGroupBox.Controls.Add(this.portLabel);
            this.connectionGroupBox.Controls.Add(this.hostLabel);
            this.connectionGroupBox.Controls.Add(this.hostTextBox);
            this.connectionGroupBox.Location = new System.Drawing.Point(0, 3);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(493, 151);
            this.connectionGroupBox.TabIndex = 0;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "Connection Details";
            // 
            // databaseTextBox
            // 
            this.databaseTextBox.Location = new System.Drawing.Point(161, 105);
            this.databaseTextBox.Name = "databaseTextBox";
            this.databaseTextBox.Size = new System.Drawing.Size(326, 38);
            this.databaseTextBox.TabIndex = 3;
            this.databaseTextBox.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 32);
            this.label1.TabIndex = 4;
            this.label1.Text = "Database:";
            // 
            // portNumericUpDown
            // 
            this.portNumericUpDown.Location = new System.Drawing.Point(161, 68);
            this.portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.portNumericUpDown.Name = "portNumericUpDown";
            this.portNumericUpDown.Size = new System.Drawing.Size(110, 38);
            this.portNumericUpDown.TabIndex = 2;
            this.portNumericUpDown.Value = new decimal(new int[] {
            5432,
            0,
            0,
            0});
            this.portNumericUpDown.ValueChanged += new System.EventHandler(this.SetProperty);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(10, 70);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(75, 32);
            this.portLabel.TabIndex = 2;
            this.portLabel.Text = "Port:";
            // 
            // savePasswordCheckBox
            // 
            this.savePasswordCheckBox.AutoSize = true;
            this.savePasswordCheckBox.Location = new System.Drawing.Point(113, 153);
            this.savePasswordCheckBox.Name = "savePasswordCheckBox";
            this.savePasswordCheckBox.Size = new System.Drawing.Size(290, 36);
            this.savePasswordCheckBox.TabIndex = 7;
            this.savePasswordCheckBox.Text = "Save my password";
            this.savePasswordCheckBox.UseVisualStyleBackColor = true;
            this.savePasswordCheckBox.CheckedChanged += new System.EventHandler(this.SetProperty);
            // 
            // NpgsqlConnectionUIControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.connectionGroupBox);
            this.Controls.Add(this.credentialsGroupBox);
            this.Name = "NpgsqlConnectionUIControl";
            this.Size = new System.Drawing.Size(496, 363);
            this.credentialsGroupBox.ResumeLayout(false);
            this.credentialsGroupBox.PerformLayout();
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.CheckBox windowsAuthCheckbox;
        private System.Windows.Forms.GroupBox credentialsGroupBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox databaseTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown portNumericUpDown;
        private System.Windows.Forms.CheckBox savePasswordCheckBox;
    }
}
