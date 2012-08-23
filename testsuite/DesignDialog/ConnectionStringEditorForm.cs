//	Copyright (C) 2007 The Npgsql Development Team
//	Npgsql-devel@pgfoundry.org
//	http://npgsql.projects.postgresql.org/
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
// 
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// 
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Npgsql;

namespace DesignDialog
{
	public class ConnectionStringEditorForm : System.Windows.Forms.Form {
		private System.Windows.Forms.TabControl tc_main;
		private System.Windows.Forms.TabPage tp_connection;
		private System.Windows.Forms.Label lab_advise;
		private System.Windows.Forms.Label lab_login;
		private System.Windows.Forms.Label lab_username;
		private System.Windows.Forms.TextBox tb_username;
		private System.Windows.Forms.Label lab_select_db;
		private System.Windows.Forms.ComboBox cb_select_db;
		private System.Windows.Forms.GroupBox gb_add_parms;
		private System.Windows.Forms.Label lab_timeout;
		private System.Windows.Forms.TextBox tb_timeout;
		private System.Windows.Forms.Button btn_check_connection;
		private System.Windows.Forms.Button btn_ok;
		private System.Windows.Forms.Button btn_cancel;
		private System.Windows.Forms.Button btn_help;
		private System.Windows.Forms.Label lab_port;
		private System.Windows.Forms.TextBox tb_port;
		private System.Windows.Forms.Label lab_server;
		private System.Windows.Forms.TextBox tb_server;
		private System.Windows.Forms.TextBox tb_password;
		private System.Windows.Forms.Button btn_refresh;
		private System.Windows.Forms.Label lab_password;
		private System.Resources.ResourceManager resman;
		private Npgsql.NpgsqlConnection pgconn;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConnectionStringEditorForm()
    : this("")
    {}

		public ConnectionStringEditorForm(String ConnectionString) {
			InitializeComponent();
			// Attention: The localization-issues don't only affect the surface but also affect some
			// MessageBoxes which have to be localized too - look for resman!
			resman = new System.Resources.ResourceManager(typeof(ConnectionStringEditorForm));

			this.pgconn.ConnectionString = ConnectionString;
			this.tb_server.Text = this.pgconn.Host;
			this.tb_port.Text = this.pgconn.Port.ToString();
			this.tb_timeout.Text = this.pgconn.ConnectionTimeout.ToString();
			if (this.pgconn.Database != "") {
				this.cb_select_db.Items.Add(this.pgconn.Database);
				this.cb_select_db.SelectedIndex = 0;
			}
            /*
			this.tb_username.Text = this.pgconn.UserName;
			this.tb_password.Text = this.pgconn.Password;
            */
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionStringEditorForm));
            this.tc_main = new System.Windows.Forms.TabControl();
            this.tp_connection = new System.Windows.Forms.TabPage();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.tb_server = new System.Windows.Forms.TextBox();
            this.btn_check_connection = new System.Windows.Forms.Button();
            this.gb_add_parms = new System.Windows.Forms.GroupBox();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.lab_port = new System.Windows.Forms.Label();
            this.tb_timeout = new System.Windows.Forms.TextBox();
            this.lab_timeout = new System.Windows.Forms.Label();
            this.cb_select_db = new System.Windows.Forms.ComboBox();
            this.lab_select_db = new System.Windows.Forms.Label();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.lab_password = new System.Windows.Forms.Label();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.lab_username = new System.Windows.Forms.Label();
            this.lab_login = new System.Windows.Forms.Label();
            this.lab_server = new System.Windows.Forms.Label();
            this.lab_advise = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_help = new System.Windows.Forms.Button();
            this.pgconn = new Npgsql.NpgsqlConnection();
            this.tc_main.SuspendLayout();
            this.tp_connection.SuspendLayout();
            this.gb_add_parms.SuspendLayout();
            this.SuspendLayout();
            // 
            // tc_main
            // 
            this.tc_main.Controls.Add(this.tp_connection);
            resources.ApplyResources(this.tc_main, "tc_main");
            this.tc_main.Name = "tc_main";
            this.tc_main.SelectedIndex = 0;
            // 
            // tp_connection
            // 
            this.tp_connection.Controls.Add(this.btn_refresh);
            this.tp_connection.Controls.Add(this.tb_server);
            this.tp_connection.Controls.Add(this.btn_check_connection);
            this.tp_connection.Controls.Add(this.gb_add_parms);
            this.tp_connection.Controls.Add(this.cb_select_db);
            this.tp_connection.Controls.Add(this.lab_select_db);
            this.tp_connection.Controls.Add(this.tb_password);
            this.tp_connection.Controls.Add(this.lab_password);
            this.tp_connection.Controls.Add(this.tb_username);
            this.tp_connection.Controls.Add(this.lab_username);
            this.tp_connection.Controls.Add(this.lab_login);
            this.tp_connection.Controls.Add(this.lab_server);
            this.tp_connection.Controls.Add(this.lab_advise);
            resources.ApplyResources(this.tp_connection, "tp_connection");
            this.tp_connection.Name = "tp_connection";
            // 
            // btn_refresh
            // 
            resources.ApplyResources(this.btn_refresh, "btn_refresh");
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // tb_server
            // 
            resources.ApplyResources(this.tb_server, "tb_server");
            this.tb_server.Name = "tb_server";
            // 
            // btn_check_connection
            // 
            resources.ApplyResources(this.btn_check_connection, "btn_check_connection");
            this.btn_check_connection.Name = "btn_check_connection";
            this.btn_check_connection.Click += new System.EventHandler(this.btn_check_connection_Click);
            // 
            // gb_add_parms
            // 
            this.gb_add_parms.Controls.Add(this.tb_port);
            this.gb_add_parms.Controls.Add(this.lab_port);
            this.gb_add_parms.Controls.Add(this.tb_timeout);
            this.gb_add_parms.Controls.Add(this.lab_timeout);
            resources.ApplyResources(this.gb_add_parms, "gb_add_parms");
            this.gb_add_parms.Name = "gb_add_parms";
            this.gb_add_parms.TabStop = false;
            // 
            // tb_port
            // 
            resources.ApplyResources(this.tb_port, "tb_port");
            this.tb_port.Name = "tb_port";
            // 
            // lab_port
            // 
            resources.ApplyResources(this.lab_port, "lab_port");
            this.lab_port.Name = "lab_port";
            // 
            // tb_timeout
            // 
            resources.ApplyResources(this.tb_timeout, "tb_timeout");
            this.tb_timeout.Name = "tb_timeout";
            // 
            // lab_timeout
            // 
            resources.ApplyResources(this.lab_timeout, "lab_timeout");
            this.lab_timeout.Name = "lab_timeout";
            // 
            // cb_select_db
            // 
            resources.ApplyResources(this.cb_select_db, "cb_select_db");
            this.cb_select_db.Name = "cb_select_db";
            this.cb_select_db.DropDown += new System.EventHandler(this.cb_select_db_DropDown);
            // 
            // lab_select_db
            // 
            resources.ApplyResources(this.lab_select_db, "lab_select_db");
            this.lab_select_db.Name = "lab_select_db";
            // 
            // tb_password
            // 
            resources.ApplyResources(this.tb_password, "tb_password");
            this.tb_password.Name = "tb_password";
            // 
            // lab_password
            // 
            resources.ApplyResources(this.lab_password, "lab_password");
            this.lab_password.Name = "lab_password";
            // 
            // tb_username
            // 
            resources.ApplyResources(this.tb_username, "tb_username");
            this.tb_username.Name = "tb_username";
            // 
            // lab_username
            // 
            resources.ApplyResources(this.lab_username, "lab_username");
            this.lab_username.Name = "lab_username";
            // 
            // lab_login
            // 
            resources.ApplyResources(this.lab_login, "lab_login");
            this.lab_login.Name = "lab_login";
            // 
            // lab_server
            // 
            resources.ApplyResources(this.lab_server, "lab_server");
            this.lab_server.Name = "lab_server";
            // 
            // lab_advise
            // 
            resources.ApplyResources(this.lab_advise, "lab_advise");
            this.lab_advise.Name = "lab_advise";
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_cancel, "btn_cancel");
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_help
            // 
            resources.ApplyResources(this.btn_help, "btn_help");
            this.btn_help.Name = "btn_help";
            this.btn_help.Click += new System.EventHandler(this.btn_help_Click);
            // 
            // ConnectionStringEditorForm
            // 
            this.AcceptButton = this.btn_ok;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.btn_help);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.tc_main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionStringEditorForm";
            this.ShowInTaskbar = false;
            this.tc_main.ResumeLayout(false);
            this.tp_connection.ResumeLayout(false);
            this.tp_connection.PerformLayout();
            this.gb_add_parms.ResumeLayout(false);
            this.gb_add_parms.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void btn_cancel_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btn_ok_Click(object sender, System.EventArgs e) {
			if(connect(false) == true){
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void btn_help_Click(object sender, System.EventArgs e) {
		
		}

		private void btn_check_connection_Click(object sender, System.EventArgs e) {
			if(connect(false) == true){
				MessageBox.Show(this, resman.GetString("MsgboxText_Success"), resman.GetString("MsgboxTitle_Success"), MessageBoxButtons.OK, MessageBoxIcon.None);
			}
		}

		/// <summary>
		/// Returns the generated ConnectionString
		/// </summary>
		public string ConnectionString {
			get {
				return this.pgconn.ConnectionString;
			}
		}

		private bool connect(bool fillComboBox) {
			try{
				StringWriter sw = new StringWriter();
                
				if(this.tb_server.Text == String.Empty){
					MessageBox.Show(this, resman.GetString("MsgboxText_NoServer"), resman.GetString("MsgboxTitle_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				if(this.tb_username.Text == String.Empty){
					MessageBox.Show(this, resman.GetString("MsgboxText_NoUser"), resman.GetString("MsgboxTitle_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				sw.Write("Server={0};", this.tb_server.Text);
				if(this.tb_port.Text != String.Empty && Convert.ToInt32(this.tb_port.Text) != 5432){
					sw.Write("Port={0};", tb_port.Text);
				}
				// this happens if the user clicks Ok or Check Connection
				// before selecting a database
				if(fillComboBox == false && (String)this.cb_select_db.Text == String.Empty){
					MessageBox.Show(this, resman.GetString("MsgboxText_NoDb"), resman.GetString("MsgboxTitle_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
					// this happens if the user clicks the database-combobox
					// in order to select a database
				else if(fillComboBox == true && (String)this.cb_select_db.Text == String.Empty){
                    sw.Write("Database=template1;", pgconn.Database);
				}
				else{
					sw.Write("Database={0};", this.cb_select_db.Text);
				}
                
				try{
					if(this.tb_timeout.Text != String.Empty && Convert.ToInt32(this.tb_timeout.Text) != 15){
                        sw.Write("CommandTimeout={0};", this.tb_timeout.Text);
					}
				}
					// don't mind if the value is nonsense - just don't put it into the string
				catch(FormatException){
					MessageBox.Show(this, resman.GetString("MsgboxText_TimeoutNaN"), resman.GetString("MsgboxTitle_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				catch(OverflowException){
					MessageBox.Show(this, resman.GetString("MsgboxText_TimeoutOverflow"), resman.GetString("MsgboxTitle_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
                
                sw.Write("User Id={0};", this.tb_username.Text);
                sw.Write("Password={0};", this.tb_password.Text);

				this.pgconn.ConnectionString = sw.ToString();
                this.pgconn.Open();
				if(fillComboBox == true){
					cb_select_db.Items.Clear();
					NpgsqlCommand com = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datallowconn = 't'", this.pgconn);
					NpgsqlDataReader dr = com.ExecuteReader();
					while(dr.Read()){
						cb_select_db.Items.Add(dr["datname"]);
						if(cb_select_db.Items.Count > 0){
							cb_select_db.SelectedIndex = 0;
						}
					}
				}
				this.pgconn.Close();
			}catch(Exception ex){
				MessageBox.Show(this, ex.Message, resman.GetString("MsgboxTitle_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		private void btn_refresh_Click(object sender, System.EventArgs e) {
			connect(true);
		}



		private void cb_select_db_DropDown(object sender, System.EventArgs e) {

			if(cb_select_db.Items.Count < 1){
				connect(true);
			}
		}
	}
}
