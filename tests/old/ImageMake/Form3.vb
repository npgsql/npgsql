Option Strict Off
Option Explicit On
Friend Class Form3
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
    Public WithEvents cmdCancel As System.Windows.Forms.Button
    Public WithEvents cmdOk As System.Windows.Forms.Button
    Public WithEvents txtTableName As System.Windows.Forms.TextBox
    Public WithEvents txtUserPass As System.Windows.Forms.TextBox
    Public WithEvents txtUserName As System.Windows.Forms.TextBox
    Public WithEvents txtDbName As System.Windows.Forms.TextBox
    Public WithEvents txtDbSouce As System.Windows.Forms.TextBox
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents frmLogin As System.Windows.Forms.GroupBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdOk = New System.Windows.Forms.Button
        Me.frmLogin = New System.Windows.Forms.GroupBox
        Me.txtTableName = New System.Windows.Forms.TextBox
        Me.txtUserPass = New System.Windows.Forms.TextBox
        Me.txtUserName = New System.Windows.Forms.TextBox
        Me.txtDbName = New System.Windows.Forms.TextBox
        Me.txtDbSouce = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.frmLogin.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdCancel
        '
        Me.cmdCancel.BackColor = System.Drawing.SystemColors.Control
        Me.cmdCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdCancel.Location = New System.Drawing.Point(224, 166)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdCancel.Size = New System.Drawing.Size(80, 23)
        Me.cmdCancel.TabIndex = 12
        Me.cmdCancel.Text = "&Cancel"
        '
        'cmdOk
        '
        Me.cmdOk.BackColor = System.Drawing.SystemColors.Control
        Me.cmdOk.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdOk.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdOk.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdOk.Location = New System.Drawing.Point(136, 166)
        Me.cmdOk.Name = "cmdOk"
        Me.cmdOk.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdOk.Size = New System.Drawing.Size(80, 23)
        Me.cmdOk.TabIndex = 11
        Me.cmdOk.Text = "&Ok"
        '
        'frmLogin
        '
        Me.frmLogin.BackColor = System.Drawing.SystemColors.Control
        Me.frmLogin.Controls.Add(Me.txtTableName)
        Me.frmLogin.Controls.Add(Me.txtUserPass)
        Me.frmLogin.Controls.Add(Me.txtUserName)
        Me.frmLogin.Controls.Add(Me.txtDbName)
        Me.frmLogin.Controls.Add(Me.txtDbSouce)
        Me.frmLogin.Controls.Add(Me.Label5)
        Me.frmLogin.Controls.Add(Me.Label4)
        Me.frmLogin.Controls.Add(Me.Label3)
        Me.frmLogin.Controls.Add(Me.Label2)
        Me.frmLogin.Controls.Add(Me.Label1)
        Me.frmLogin.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.frmLogin.ForeColor = System.Drawing.SystemColors.ControlText
        Me.frmLogin.Location = New System.Drawing.Point(8, 8)
        Me.frmLogin.Name = "frmLogin"
        Me.frmLogin.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.frmLogin.Size = New System.Drawing.Size(297, 152)
        Me.frmLogin.TabIndex = 0
        Me.frmLogin.TabStop = False
        Me.frmLogin.Text = "Connect Setting"
        '
        'txtTableName
        '
        Me.txtTableName.AcceptsReturn = True
        Me.txtTableName.AutoSize = False
        Me.txtTableName.BackColor = System.Drawing.SystemColors.Window
        Me.txtTableName.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtTableName.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtTableName.Location = New System.Drawing.Point(96, 117)
        Me.txtTableName.MaxLength = 0
        Me.txtTableName.Name = "txtTableName"
        Me.txtTableName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtTableName.Size = New System.Drawing.Size(185, 19)
        Me.txtTableName.TabIndex = 10
        Me.txtTableName.Text = "lo_image"
        '
        'txtUserPass
        '
        Me.txtUserPass.AcceptsReturn = True
        Me.txtUserPass.AutoSize = False
        Me.txtUserPass.BackColor = System.Drawing.SystemColors.Window
        Me.txtUserPass.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtUserPass.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtUserPass.Location = New System.Drawing.Point(96, 92)
        Me.txtUserPass.MaxLength = 0
        Me.txtUserPass.Name = "txtUserPass"
        Me.txtUserPass.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtUserPass.Size = New System.Drawing.Size(185, 20)
        Me.txtUserPass.TabIndex = 9
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
        Me.txtUserName.TabIndex = 8
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
        Me.txtDbName.TabIndex = 7
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
        Me.txtDbSouce.TabIndex = 6
        Me.txtDbSouce.Text = "localhost"
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.Control
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.Location = New System.Drawing.Point(8, 120)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(80, 16)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "TableName"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.Control
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label4.Location = New System.Drawing.Point(8, 92)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(80, 20)
        Me.Label4.TabIndex = 4
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
        Me.Label3.TabIndex = 3
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
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "DataBase"
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
        Me.Label1.Size = New System.Drawing.Size(80, 14)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "HostName:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Form3
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(314, 200)
        Me.ControlBox = False
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdOk)
        Me.Controls.Add(Me.frmLogin)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Location = New System.Drawing.Point(3, 22)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form3"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Server Login"
        Me.frmLogin.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private Shared m_vb6FormDefInstance As Form3
    Private Shared m_InitializingDefInstance As Boolean
    Public Shared Property DefInstance() As Form3
        Get
            If m_vb6FormDefInstance Is Nothing OrElse m_vb6FormDefInstance.IsDisposed Then
                m_InitializingDefInstance = True
                m_vb6FormDefInstance = New Form3
                m_InitializingDefInstance = False
            End If
            DefInstance = m_vb6FormDefInstance
        End Get
        Set(ByVal Value As Form3)
            m_vb6FormDefInstance = Value
        End Set
    End Property
    Private Sub Frame1_DragDrop(ByRef Source As System.Windows.Forms.Control, ByRef X As Single, ByRef Y As Single)

    End Sub

    Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
        Form1.DefInstance.LoginChk = 0
        Me.Close()
        Hide()
    End Sub

    Private Sub cmdOk_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOk.Click
        Form1.DefInstance.LoginChk = 1
        Hide()
    End Sub

End Class