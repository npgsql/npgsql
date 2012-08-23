#region Licence
// created on 05/06/2002 at 20:17

// frmMain.cs
// 
// Author:
//	Dave Page (dpage@postgresql.org)
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

using System;
using System.Windows.Forms;
using System.Data;
using Npgsql;


  /// <summary>
  /// The main form for the application
  /// </summary>
  public class frmMain : System.Windows.Forms.Form
  {
    private System.Windows.Forms.TabPage Connection;
    private System.Windows.Forms.TabPage ExecuteNonQuery;
    private System.Windows.Forms.TabPage ExecuteScalar;
    private System.Windows.Forms.Button cmdConnect;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.TextBox txtPort;
    private System.Windows.Forms.TextBox txtHostname;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.Label lblHostname;
    private System.Windows.Forms.TextBox txtLog;
    private System.Windows.Forms.TabControl tabset;
    private System.Windows.Forms.TextBox txtNonQuery;
    private System.Windows.Forms.Label lblNonQuery;
    private System.Windows.Forms.Button cmdNonQuery;
    private System.Windows.Forms.Button cmdScalar;
    private System.Windows.Forms.Label lblScalar;
    private System.Windows.Forms.TextBox txtScalar;
    private System.Windows.Forms.Button cmdDisconnect;
    private System.Windows.Forms.TabPage ExecuteReader;
    private System.Windows.Forms.Button cmdReader;
    private System.Windows.Forms.TextBox txtReader;
    private System.Windows.Forms.Label lblReader;
		private System.Windows.Forms.TextBox txtDatabase;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbVersion;
    private NpgsqlConnection cnDB;

    public frmMain()
    {
      // Initialise the form
      InitializeComponent();

      // Bind functions to the buttons
      cmdConnect.Click += new System.EventHandler(cmdConnect_Click);
      cmdDisconnect.Click += new System.EventHandler(cmdDisconnect_Click);
      cmdNonQuery.Click += new System.EventHandler(cmdNonQuery_Click);
      cmdScalar.Click += new System.EventHandler(cmdScalar_Click);
      cmdReader.Click += new System.EventHandler(cmdReader_Click);

      cbVersion.SelectedIndex = 0;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if (cnDB != null && cnDB.State == ConnectionState.Closed)
      {
        try
        {
          cnDB.Close();
        }
        catch (Exception)
        {
          // Do nothing...
        }
      }
      
      base.Dispose( disposing );
    }

    /// <summary>
    /// Connect to the database using the specified
    /// connection details.
    /// </summary>
    private void cmdConnect_Click(object sender, System.EventArgs e) 
    {
		
		  log("Connecting to PostgreSQL...");
		  
		  // Setup the logging
		  NpgsqlEventLog.Level = LogLevel.Normal;
		  NpgsqlEventLog.LogName = "testsuite.log";
		  NpgsqlEventLog.EchoMessages = true;
		  
		  if (cnDB != null)
		  {
        if (cnDB.State != ConnectionState.Closed)
        {
          log("Error: Already connected!");
          log("Finished connecting!\r\n"); 
          return;
        }
      }
		  
      // Check the data
      if (txtHostname.Text == "")
      {
        log("Error: No hostname was specified!");
        log("Finished connecting!\r\n"); 
        return;
      }
      if (txtPort.Text == "")
      {
        log("Error: No port was specified!");
        log("Finished connecting!\r\n"); 
        return;
      }
      if (txtUsername.Text == "")
      {
        log("Error: No username was specified!");
        log("Finished connecting!\r\n"); 
        return;
      }

      // Setup a connection string
      string szConnect = "DATABASE=" + txtDatabase.Text + ";SERVER=" + txtHostname.Text + ";PORT=" + int.Parse(txtPort.Text) + ";UID=" + txtUsername.Text + ";PWD=" + txtPassword.Text + ";";
      if (cbVersion.SelectedIndex > 0) {
        szConnect += "PROTOCOL=" + (cbVersion.SelectedIndex + 1).ToString();
      }
      log("Connection String: " + szConnect);

      // Attempt to open a connection
      cnDB = new NpgsqlConnection(szConnect);

      try 
      {
        cnDB.Open();
      } 
      catch(Exception ex)
      {
        log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
        log("Finished connecting!\r\n"); 
        return;
      } 

      // Get the PostgreSQL version number as proof
      try
      {
        NpgsqlCommand cmdVer = new NpgsqlCommand("SELECT version()", cnDB);
        Object ObjVer = cmdVer.ExecuteScalar();
        log(ObjVer.ToString());
      }
      catch(Exception ex)
      {
        log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
        log("Finished connecting!\r\n");
        return;
      } 
      log("Finished connecting!\r\n"); 
    }
    
    /// <summary>
    /// Disconnect from the database
    /// </summary>
    private void cmdDisconnect_Click(object sender, System.EventArgs e) 
    {
      log("Disconnecting from PostgreSQL...");    
      if (cnDB != null)
      {
        if (cnDB.State != ConnectionState.Closed)
        {
          try
          {
            cnDB.Close();
          }
          catch (Exception ex)
          {
            log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
            log("Finished disconnecting!\r\n");
            return;
          }
        }
        else
        {
          log("Error: The connection is already closed!"); 
        }
      } 
      else
      {
        log("Error: The connection is already closed!"); 
      }
      log("Finished disconnecting!\r\n"); 
    }

    /// <summary>
    /// Execute a non-query. This is a query that doesn't
    /// return a result. The value returned is generally
    /// the number of records affected.
    /// </summary>
    private void cmdNonQuery_Click(object sender, System.EventArgs e) 
    {
    
      log("Executing Non-Query...");
      log("Query: " + txtNonQuery.Text);
      
      // Check the connection state
      if (cnDB == null) 
      {
        log("Error: The connection has not been opened.");
        log("Finished executing Non-Query!\r\n");
        return;
      }
      else
      {
        if (cnDB.State != ConnectionState.Open)
        {
          log("Error: The connection has not been opened.");
          log("Finished executing Non-Query!\r\n"); 
          return;        
        }
      }
      
      // Attempt to execute the query
      try
      {
        NpgsqlCommand cmdNQ = new NpgsqlCommand(txtNonQuery.Text, cnDB);
        Int32 iRes = cmdNQ.ExecuteNonQuery();
        log("Records affected: " + iRes);
      }
      catch(Exception ex)
      {
        log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
        log("Finished executing Non-Query!\r\n"); 
        return;
      }
      log("Finished executing Non-Query!\r\n"); 
    }
    
    private void cmdScalar_Click(object sender, System.EventArgs e) 
    {
      log("Executing Scalar...");
      log("Query: " + txtScalar.Text);
      
      // Check the connection state
      if (cnDB == null) 
      {
        log("Error: The connection has not been opened.");
        log("Finished executing Scalar!\r\n"); 
        return;
      }
      else
      {
        if (cnDB.State != ConnectionState.Open)
        {
          log("Error: The connection has not been opened.");
          log("Finished executing Scalar!\r\n");
          return;  
        }
      }
      
      // Attempt to execute the query
      try
      {
        NpgsqlCommand cmdNQ = new NpgsqlCommand(txtScalar.Text, cnDB);
        Object objRes = cmdNQ.ExecuteScalar();
        log("Result: " + objRes.ToString());
      }
      catch(Exception ex)
      {
        log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
        log("Finished executing Scalar!\r\n"); 
        return;
      } 
      log("Finished executing Scalar!\r\n"); 
    }
    
    private void cmdReader_Click(object sender, System.EventArgs e) 
    {
      log("Executing Reader...");
      log("Query: " + txtReader.Text);
      
      // Check the connection state
      if (cnDB == null) 
      {
        log("Error: The connection has not been opened.");
        log("Finished executing Reader!\r\n"); 
        return;
      }
      else
      {
        if (cnDB.State != ConnectionState.Open)
        {
          log("Error: The connection has not been opened.");
          log("Finished executing Reader!\r\n");
          return;  
        }
      }
      
      // Attempt to execute the query
      NpgsqlDataReader objRdr = null;
      try
      {
        NpgsqlCommand cmdNQ = new NpgsqlCommand(txtReader.Text, cnDB);
        objRdr = cmdNQ.ExecuteReader();
      }
      catch(Exception ex)
      {
        log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
        log("Finished executing Reader!\r\n"); 
        return;
      } 
      
      // Output some basic info
      log("Columns: " + objRdr.FieldCount + "\r\n");
    
      // Iterate through the rows, adding them to the output
      int y = 0;
      try 
      { 
        while (objRdr.Read()) 
        {
          y++;
          log("Record: " + y); 
          for(int x=0; x<objRdr.FieldCount; x++){
            log(objRdr.GetName(x) + " (" + objRdr.GetFieldType(x) + ") = \"" + objRdr.GetValue(x) + "\""); 
          }
          log("");
        }
      }
      catch(Exception ex)
      {
        log("Error: " + ex.Message + "\r\n" + "StackTrace: \r\n" + ex.StackTrace);
        log("Finished executing Reader!\r\n"); 
        return;
      } 
      finally 
      {
        objRdr.Close();
      }
      log("Finished executing Reader!\r\n"); 
    }
      
    public void log(String szMessage)
    {
      if (szMessage == "") {
        txtLog.AppendText("\r\n");
      } else {
        txtLog.AppendText(System.DateTime.Now + " - " + szMessage + "\r\n");
      }  
      txtLog.SelectionStart = txtLog.Text.Length;
    }

		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    ///
    /// Why not?? Works for me - Dave :-)
    /// </summary>
    private void InitializeComponent()
    {
			this.tabset = new System.Windows.Forms.TabControl();
			this.Connection = new System.Windows.Forms.TabPage();
			this.txtDatabase = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cmdDisconnect = new System.Windows.Forms.Button();
			this.cmdConnect = new System.Windows.Forms.Button();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.txtHostname = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblUsername = new System.Windows.Forms.Label();
			this.lblPort = new System.Windows.Forms.Label();
			this.lblHostname = new System.Windows.Forms.Label();
			this.ExecuteNonQuery = new System.Windows.Forms.TabPage();
			this.cmdNonQuery = new System.Windows.Forms.Button();
			this.txtNonQuery = new System.Windows.Forms.TextBox();
			this.lblNonQuery = new System.Windows.Forms.Label();
			this.ExecuteScalar = new System.Windows.Forms.TabPage();
			this.cmdScalar = new System.Windows.Forms.Button();
			this.txtScalar = new System.Windows.Forms.TextBox();
			this.lblScalar = new System.Windows.Forms.Label();
			this.ExecuteReader = new System.Windows.Forms.TabPage();
			this.cmdReader = new System.Windows.Forms.Button();
			this.txtReader = new System.Windows.Forms.TextBox();
			this.lblReader = new System.Windows.Forms.Label();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.cbVersion = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tabset.SuspendLayout();
			this.Connection.SuspendLayout();
			this.ExecuteNonQuery.SuspendLayout();
			this.ExecuteScalar.SuspendLayout();
			this.ExecuteReader.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabset
			// 
			this.tabset.Controls.AddRange(new System.Windows.Forms.Control[] {
																																				 this.Connection,
																																				 this.ExecuteNonQuery,
																																				 this.ExecuteScalar,
																																				 this.ExecuteReader});
			this.tabset.Location = new System.Drawing.Point(8, 8);
			this.tabset.Name = "tabset";
			this.tabset.SelectedIndex = 0;
			this.tabset.Size = new System.Drawing.Size(600, 108);
			this.tabset.TabIndex = 10;
			// 
			// Connection
			// 
			this.Connection.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						 this.label2,
																																						 this.cbVersion,
																																						 this.txtDatabase,
																																						 this.label1,
																																						 this.cmdDisconnect,
																																						 this.cmdConnect,
																																						 this.txtPassword,
																																						 this.txtUsername,
																																						 this.txtPort,
																																						 this.txtHostname,
																																						 this.lblPassword,
																																						 this.lblUsername,
																																						 this.lblPort,
																																						 this.lblHostname});
			this.Connection.Location = new System.Drawing.Point(4, 22);
			this.Connection.Name = "Connection";
			this.Connection.Size = new System.Drawing.Size(592, 82);
			this.Connection.TabIndex = 0;
			this.Connection.Text = "Connection";
			// 
			// txtDatabase
			// 
			this.txtDatabase.Location = new System.Drawing.Point(80, 56);
			this.txtDatabase.Name = "txtDatabase";
			this.txtDatabase.Size = new System.Drawing.Size(152, 20);
			this.txtDatabase.TabIndex = 2;
			this.txtDatabase.Text = "template1";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "Database";
			// 
			// cmdDisconnect
			// 
			this.cmdDisconnect.Location = new System.Drawing.Point(488, 32);
			this.cmdDisconnect.Name = "cmdDisconnect";
			this.cmdDisconnect.Size = new System.Drawing.Size(88, 24);
			this.cmdDisconnect.TabIndex = 7;
			this.cmdDisconnect.Text = "&Disconnect";
			// 
			// cmdConnect
			// 
			this.cmdConnect.Location = new System.Drawing.Point(488, 8);
			this.cmdConnect.Name = "cmdConnect";
			this.cmdConnect.Size = new System.Drawing.Size(88, 24);
			this.cmdConnect.TabIndex = 6;
			this.cmdConnect.Text = "&Connect";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(320, 32);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(152, 20);
			this.txtPassword.TabIndex = 4;
			this.txtPassword.Text = "";
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(320, 8);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(152, 20);
			this.txtUsername.TabIndex = 3;
			this.txtUsername.Text = "postgres";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(80, 32);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(64, 20);
			this.txtPort.TabIndex = 1;
			this.txtPort.Text = "5432";
			// 
			// txtHostname
			// 
			this.txtHostname.Location = new System.Drawing.Point(80, 8);
			this.txtHostname.Name = "txtHostname";
			this.txtHostname.Size = new System.Drawing.Size(152, 20);
			this.txtHostname.TabIndex = 0;
			this.txtHostname.Text = "localhost";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(248, 40);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(54, 13);
			this.lblPassword.TabIndex = 22;
			this.lblPassword.Text = "Password";
			// 
			// lblUsername
			// 
			this.lblUsername.AutoSize = true;
			this.lblUsername.Location = new System.Drawing.Point(248, 16);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(56, 13);
			this.lblUsername.TabIndex = 21;
			this.lblUsername.Text = "Username";
			// 
			// lblPort
			// 
			this.lblPort.AutoSize = true;
			this.lblPort.Location = new System.Drawing.Point(8, 40);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new System.Drawing.Size(25, 13);
			this.lblPort.TabIndex = 20;
			this.lblPort.Text = "Port";
			// 
			// lblHostname
			// 
			this.lblHostname.AutoSize = true;
			this.lblHostname.Location = new System.Drawing.Point(8, 16);
			this.lblHostname.Name = "lblHostname";
			this.lblHostname.Size = new System.Drawing.Size(56, 13);
			this.lblHostname.TabIndex = 19;
			this.lblHostname.Text = "Hostname";
			// 
			// ExecuteNonQuery
			// 
			this.ExecuteNonQuery.Controls.AddRange(new System.Windows.Forms.Control[] {
																																									this.cmdNonQuery,
																																									this.txtNonQuery,
																																									this.lblNonQuery});
			this.ExecuteNonQuery.Location = new System.Drawing.Point(4, 22);
			this.ExecuteNonQuery.Name = "ExecuteNonQuery";
			this.ExecuteNonQuery.Size = new System.Drawing.Size(592, 82);
			this.ExecuteNonQuery.TabIndex = 1;
			this.ExecuteNonQuery.Text = "ExecuteNonQuery";
			// 
			// cmdNonQuery
			// 
			this.cmdNonQuery.Location = new System.Drawing.Point(488, 16);
			this.cmdNonQuery.Name = "cmdNonQuery";
			this.cmdNonQuery.Size = new System.Drawing.Size(88, 32);
			this.cmdNonQuery.TabIndex = 28;
			this.cmdNonQuery.Text = "&Execute Non-Query";
			// 
			// txtNonQuery
			// 
			this.txtNonQuery.Location = new System.Drawing.Point(80, 8);
			this.txtNonQuery.Multiline = true;
			this.txtNonQuery.Name = "txtNonQuery";
			this.txtNonQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtNonQuery.Size = new System.Drawing.Size(392, 67);
			this.txtNonQuery.TabIndex = 25;
			this.txtNonQuery.Text = "UPDATE pg_description SET description = \'I am a non-query\' WHERE 1 = 2";
			// 
			// lblNonQuery
			// 
			this.lblNonQuery.AutoSize = true;
			this.lblNonQuery.Location = new System.Drawing.Point(8, 16);
			this.lblNonQuery.Name = "lblNonQuery";
			this.lblNonQuery.Size = new System.Drawing.Size(60, 13);
			this.lblNonQuery.TabIndex = 24;
			this.lblNonQuery.Text = "Non-Query";
			// 
			// ExecuteScalar
			// 
			this.ExecuteScalar.Controls.AddRange(new System.Windows.Forms.Control[] {
																																								this.cmdScalar,
																																								this.txtScalar,
																																								this.lblScalar});
			this.ExecuteScalar.Location = new System.Drawing.Point(4, 22);
			this.ExecuteScalar.Name = "ExecuteScalar";
			this.ExecuteScalar.Size = new System.Drawing.Size(592, 82);
			this.ExecuteScalar.TabIndex = 2;
			this.ExecuteScalar.Text = "ExecuteScalar";
			// 
			// cmdScalar
			// 
			this.cmdScalar.Location = new System.Drawing.Point(488, 16);
			this.cmdScalar.Name = "cmdScalar";
			this.cmdScalar.Size = new System.Drawing.Size(88, 32);
			this.cmdScalar.TabIndex = 31;
			this.cmdScalar.Text = "&Execute Scalar";
			// 
			// txtScalar
			// 
			this.txtScalar.Location = new System.Drawing.Point(80, 8);
			this.txtScalar.Multiline = true;
			this.txtScalar.Name = "txtScalar";
			this.txtScalar.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtScalar.Size = new System.Drawing.Size(392, 67);
			this.txtScalar.TabIndex = 30;
			this.txtScalar.Text = "SELECT count(*) AS record_count  FROM pg_class";
			// 
			// lblScalar
			// 
			this.lblScalar.AutoSize = true;
			this.lblScalar.Location = new System.Drawing.Point(8, 16);
			this.lblScalar.Name = "lblScalar";
			this.lblScalar.Size = new System.Drawing.Size(36, 13);
			this.lblScalar.TabIndex = 29;
			this.lblScalar.Text = "Scalar";
			// 
			// ExecuteReader
			// 
			this.ExecuteReader.Controls.AddRange(new System.Windows.Forms.Control[] {
																																								this.cmdReader,
																																								this.txtReader,
																																								this.lblReader});
			this.ExecuteReader.Location = new System.Drawing.Point(4, 22);
			this.ExecuteReader.Name = "ExecuteReader";
			this.ExecuteReader.Size = new System.Drawing.Size(592, 82);
			this.ExecuteReader.TabIndex = 3;
			this.ExecuteReader.Text = "ExecuteReader";
			// 
			// cmdReader
			// 
			this.cmdReader.Location = new System.Drawing.Point(488, 16);
			this.cmdReader.Name = "cmdReader";
			this.cmdReader.Size = new System.Drawing.Size(88, 32);
			this.cmdReader.TabIndex = 34;
			this.cmdReader.Text = "&Execute Reader";
			// 
			// txtReader
			// 
			this.txtReader.Location = new System.Drawing.Point(80, 8);
			this.txtReader.Multiline = true;
			this.txtReader.Name = "txtReader";
			this.txtReader.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtReader.Size = new System.Drawing.Size(392, 67);
			this.txtReader.TabIndex = 33;
			this.txtReader.Text = "SELECT * FROM pg_database";
			// 
			// lblReader
			// 
			this.lblReader.AutoSize = true;
			this.lblReader.Location = new System.Drawing.Point(8, 16);
			this.lblReader.Name = "lblReader";
			this.lblReader.Size = new System.Drawing.Size(41, 13);
			this.lblReader.TabIndex = 32;
			this.lblReader.Text = "Reader";
			// 
			// txtLog
			// 
			this.txtLog.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.txtLog.AutoSize = false;
			this.txtLog.Location = new System.Drawing.Point(8, 119);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(600, 214);
			this.txtLog.TabIndex = 11;
			this.txtLog.Text = "";
			// 
			// cbVersion
			// 
			this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbVersion.Items.AddRange(new object[] {
																									 "Automatic",
																									 "Version 2",
																									 "Version 3",
																									 "Invalid!"});
			this.cbVersion.Location = new System.Drawing.Point(320, 56);
			this.cbVersion.Name = "cbVersion";
			this.cbVersion.Size = new System.Drawing.Size(121, 21);
			this.cbVersion.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(248, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 13);
			this.label2.TabIndex = 32;
			this.label2.Text = "Protocol";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(618, 345);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.txtLog,
																																	this.tabset});
			this.Name = "frmMain";
			this.Text = "Npgsql Test Suite";
			this.tabset.ResumeLayout(false);
			this.Connection.ResumeLayout(false);
			this.ExecuteNonQuery.ResumeLayout(false);
			this.ExecuteScalar.ResumeLayout(false);
			this.ExecuteReader.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() 
    {
      Application.Run(new frmMain());
    }
  }
