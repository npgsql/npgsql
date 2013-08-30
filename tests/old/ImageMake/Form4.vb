Option Strict Off
Option Explicit On
Friend Class Form4
	Inherits System.Windows.Forms.Form
    Public Sub New()
        MyBase.New()
        If m_vb6FormDefInstance Is Nothing Then
            If m_InitializingDefInstance Then
                m_vb6FormDefInstance = Me
            Else
                Try
                    If System.Reflection.Assembly.GetExecutingAssembly.EntryPoint.DeclaringType Is Me.GetType Then
                        m_vb6FormDefInstance = Me
                    End If
                Catch
                End Try
            End If
        End If
        InitializeComponent()
    End Sub
    Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
        If Disposing Then
            If Not components Is Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(Disposing)
    End Sub
    Private components As System.ComponentModel.IContainer
    Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents txtTableName As System.Windows.Forms.TextBox
    Public WithEvents cmdCancel As System.Windows.Forms.Button
    Public WithEvents cmdOk As System.Windows.Forms.Button
    Public WithEvents txtUserPass As System.Windows.Forms.TextBox
    Public WithEvents txtUserName As System.Windows.Forms.TextBox
    Public WithEvents txtDbName As System.Windows.Forms.TextBox
    Public WithEvents txtDbSouce As System.Windows.Forms.TextBox
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents frmLogin As System.Windows.Forms.GroupBox
    Public WithEvents Label5 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtTableName = New System.Windows.Forms.TextBox
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdOk = New System.Windows.Forms.Button
        Me.frmLogin = New System.Windows.Forms.GroupBox
        Me.txtUserPass = New System.Windows.Forms.TextBox
        Me.txtUserName = New System.Windows.Forms.TextBox
        Me.txtDbName = New System.Windows.Forms.TextBox
        Me.txtDbSouce = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.frmLogin.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtTableName
        '
        Me.txtTableName.AcceptsReturn = True
        Me.txtTableName.AutoSize = False
        Me.txtTableName.BackColor = System.Drawing.SystemColors.Window
        Me.txtTableName.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtTableName.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtTableName.Location = New System.Drawing.Point(104, 140)
        Me.txtTableName.MaxLength = 0
        Me.txtTableName.Name = "txtTableName"
        Me.txtTableName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtTableName.Size = New System.Drawing.Size(185, 20)
        Me.txtTableName.TabIndex = 4
        Me.txtTableName.Text = "lo_image"
        Me.ToolTip1.SetToolTip(Me.txtTableName, "Please input table name")
        '
        'cmdCancel
        '
        Me.cmdCancel.BackColor = System.Drawing.SystemColors.Control
        Me.cmdCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdCancel.Location = New System.Drawing.Point(224, 168)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdCancel.Size = New System.Drawing.Size(80, 24)
        Me.cmdCancel.TabIndex = 6
        Me.cmdCancel.Text = "&Cancel"
        Me.ToolTip1.SetToolTip(Me.cmdCancel, "Not create table,return to main screen.")
        '
        'cmdOk
        '
        Me.cmdOk.BackColor = System.Drawing.SystemColors.Control
        Me.cmdOk.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdOk.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdOk.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdOk.Location = New System.Drawing.Point(136, 168)
        Me.cmdOk.Name = "cmdOk"
        Me.cmdOk.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdOk.Size = New System.Drawing.Size(80, 24)
        Me.cmdOk.TabIndex = 5
        Me.cmdOk.Text = "&Ok"
        Me.ToolTip1.SetToolTip(Me.cmdOk, "Create table")
        '
        'frmLogin
        '
        Me.frmLogin.BackColor = System.Drawing.SystemColors.Control
        Me.frmLogin.Controls.Add(Me.txtUserPass)
        Me.frmLogin.Controls.Add(Me.txtUserName)
        Me.frmLogin.Controls.Add(Me.txtDbName)
        Me.frmLogin.Controls.Add(Me.txtDbSouce)
        Me.frmLogin.Controls.Add(Me.Label4)
        Me.frmLogin.Controls.Add(Me.Label3)
        Me.frmLogin.Controls.Add(Me.Label2)
        Me.frmLogin.Controls.Add(Me.Label1)
        Me.frmLogin.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.frmLogin.ForeColor = System.Drawing.SystemColors.ControlText
        Me.frmLogin.Location = New System.Drawing.Point(8, 8)
        Me.frmLogin.Name = "frmLogin"
        Me.frmLogin.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.frmLogin.Size = New System.Drawing.Size(297, 128)
        Me.frmLogin.TabIndex = 7
        Me.frmLogin.TabStop = False
        Me.frmLogin.Text = "Connect Setting"
        '
        'txtUserPass
        '
        Me.txtUserPass.AcceptsReturn = True
        Me.txtUserPass.AutoSize = False
        Me.txtUserPass.BackColor = System.Drawing.SystemColors.Window
        Me.txtUserPass.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtUserPass.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtUserPass.Location = New System.Drawing.Point(96, 96)
        Me.txtUserPass.MaxLength = 0
        Me.txtUserPass.Name = "txtUserPass"
        Me.txtUserPass.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtUserPass.Size = New System.Drawing.Size(185, 18)
        Me.txtUserPass.TabIndex = 3
        Me.txtUserPass.Text = ""
        '
        'txtUserName
        '
        Me.txtUserName.AcceptsReturn = True
        Me.txtUserName.AutoSize = False
        Me.txtUserName.BackColor = System.Drawing.SystemColors.Window
        Me.txtUserName.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtUserName.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtUserName.Location = New System.Drawing.Point(96, 68)
        Me.txtUserName.MaxLength = 0
        Me.txtUserName.Name = "txtUserName"
        Me.txtUserName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtUserName.Size = New System.Drawing.Size(185, 20)
        Me.txtUserName.TabIndex = 2
        Me.txtUserName.Text = "postgres"
        '
        'txtDbName
        '
        Me.txtDbName.AcceptsReturn = True
        Me.txtDbName.AutoSize = False
        Me.txtDbName.BackColor = System.Drawing.SystemColors.Window
        Me.txtDbName.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtDbName.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtDbName.Location = New System.Drawing.Point(96, 43)
        Me.txtDbName.MaxLength = 0
        Me.txtDbName.Name = "txtDbName"
        Me.txtDbName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtDbName.Size = New System.Drawing.Size(185, 21)
        Me.txtDbName.TabIndex = 1
        Me.txtDbName.Text = "postgres"
        '
        'txtDbSouce
        '
        Me.txtDbSouce.AcceptsReturn = True
        Me.txtDbSouce.AutoSize = False
        Me.txtDbSouce.BackColor = System.Drawing.SystemColors.Window
        Me.txtDbSouce.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtDbSouce.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtDbSouce.Location = New System.Drawing.Point(96, 18)
        Me.txtDbSouce.MaxLength = 0
        Me.txtDbSouce.Name = "txtDbSouce"
        Me.txtDbSouce.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtDbSouce.Size = New System.Drawing.Size(185, 22)
        Me.txtDbSouce.TabIndex = 0
        Me.txtDbSouce.Text = "localhost"
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.Control
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label4.Location = New System.Drawing.Point(8, 96)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(80, 20)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Password"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.Control
        Me.Label3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label3.Location = New System.Drawing.Point(8, 68)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(80, 20)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "User"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.Control
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(8, 43)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(80, 21)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Database"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.Control
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.Location = New System.Drawing.Point(8, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(80, 22)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "HostName"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.Control
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.Location = New System.Drawing.Point(16, 142)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(81, 18)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Table Name"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Form4
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(314, 200)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtTableName)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdOk)
        Me.Controls.Add(Me.frmLogin)
        Me.Controls.Add(Me.Label5)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Location = New System.Drawing.Point(3, 22)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form4"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Create Table"
        Me.frmLogin.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private Shared m_vb6FormDefInstance As Form4
    Private Shared m_InitializingDefInstance As Boolean
    Public Shared Property DefInstance() As Form4
        Get
            If m_vb6FormDefInstance Is Nothing OrElse m_vb6FormDefInstance.IsDisposed Then
                m_InitializingDefInstance = True
                m_vb6FormDefInstance = New Form4
                m_InitializingDefInstance = False
            End If
            DefInstance = m_vb6FormDefInstance
        End Get
        Set(ByVal Value As Form4)
            m_vb6FormDefInstance = Value
        End Set
    End Property
    Private Sub Frame1_DragDrop(ByRef Source As System.Windows.Forms.Control, ByRef X As Single, ByRef Y As Single)

    End Sub

    Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
        Form1.DefInstance.TableChk = 0
        Hide()
    End Sub

    Private Sub cmdOk_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOk.Click
        Form1.DefInstance.TableChk = 1
        Hide()
    End Sub

End Class