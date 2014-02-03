Option Strict Off
Option Explicit On
Friend Class Form2
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
    Public WithEvents txtPath As System.Windows.Forms.TextBox
    Public WithEvents dirList As Microsoft.VisualBasic.Compatibility.VB6.DirListBox
    Public WithEvents drvList As Microsoft.VisualBasic.Compatibility.VB6.DriveListBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdOk = New System.Windows.Forms.Button
        Me.txtPath = New System.Windows.Forms.TextBox
        Me.dirList = New Microsoft.VisualBasic.Compatibility.VB6.DirListBox
        Me.drvList = New Microsoft.VisualBasic.Compatibility.VB6.DriveListBox
        Me.SuspendLayout()
        '
        'cmdCancel
        '
        Me.cmdCancel.BackColor = System.Drawing.SystemColors.Control
        Me.cmdCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCancel.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdCancel.Location = New System.Drawing.Point(240, 96)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdCancel.Size = New System.Drawing.Size(65, 24)
        Me.cmdCancel.TabIndex = 4
        Me.cmdCancel.Text = "&Cancel"
        '
        'cmdOk
        '
        Me.cmdOk.BackColor = System.Drawing.SystemColors.Control
        Me.cmdOk.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdOk.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdOk.Location = New System.Drawing.Point(240, 64)
        Me.cmdOk.Name = "cmdOk"
        Me.cmdOk.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdOk.Size = New System.Drawing.Size(65, 24)
        Me.cmdOk.TabIndex = 3
        Me.cmdOk.Text = "&Ok"
        '
        'txtPath
        '
        Me.txtPath.AcceptsReturn = True
        Me.txtPath.AutoSize = False
        Me.txtPath.BackColor = System.Drawing.SystemColors.Window
        Me.txtPath.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtPath.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtPath.Location = New System.Drawing.Point(8, 128)
        Me.txtPath.MaxLength = 0
        Me.txtPath.Name = "txtPath"
        Me.txtPath.ReadOnly = True
        Me.txtPath.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtPath.Size = New System.Drawing.Size(296, 24)
        Me.txtPath.TabIndex = 2
        Me.txtPath.TabStop = False
        Me.txtPath.Text = ""
        '
        'dirList
        '
        Me.dirList.BackColor = System.Drawing.SystemColors.Window
        Me.dirList.Cursor = System.Windows.Forms.Cursors.Default
        Me.dirList.ForeColor = System.Drawing.SystemColors.WindowText
        Me.dirList.IntegralHeight = False
        Me.dirList.Location = New System.Drawing.Point(8, 30)
        Me.dirList.Name = "dirList"
        Me.dirList.Size = New System.Drawing.Size(224, 90)
        Me.dirList.TabIndex = 1
        '
        'drvList
        '
        Me.drvList.BackColor = System.Drawing.SystemColors.Window
        Me.drvList.Cursor = System.Windows.Forms.Cursors.Default
        Me.drvList.ForeColor = System.Drawing.SystemColors.WindowText
        Me.drvList.Location = New System.Drawing.Point(8, 7)
        Me.drvList.Name = "drvList"
        Me.drvList.Size = New System.Drawing.Size(224, 22)
        Me.drvList.TabIndex = 0
        '
        'Form2
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 14)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(312, 158)
        Me.ControlBox = False
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdOk)
        Me.Controls.Add(Me.txtPath)
        Me.Controls.Add(Me.dirList)
        Me.Controls.Add(Me.drvList)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Location = New System.Drawing.Point(136, 129)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form2"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Selection of a directry"
        Me.ResumeLayout(False)

    End Sub
    Private Shared m_vb6FormDefInstance As Form2
    Private Shared m_InitializingDefInstance As Boolean
    Public Shared Property DefInstance() As Form2
        Get
            If m_vb6FormDefInstance Is Nothing OrElse m_vb6FormDefInstance.IsDisposed Then
                m_InitializingDefInstance = True
                m_vb6FormDefInstance = New Form2
                m_InitializingDefInstance = False
            End If
            DefInstance = m_vb6FormDefInstance
        End Get
        Set(ByVal Value As Form2)
            m_vb6FormDefInstance = Value
        End Set
    End Property
    Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
        txtPath.Text = ""
        Hide()
    End Sub

    Private Sub cmdOk_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOk.Click
        Hide()
    End Sub

    Private Sub dirList_Change(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles dirList.Change
        txtPath.Text = dirList.Path
    End Sub

    Private Sub drvList_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles drvList.SelectedIndexChanged
        dirList.Path = drvList.Drive
    End Sub

    Private Sub Form2_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        Form2.DefInstance.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(Form2.DefInstance.Height)) / 2)
        Form2.DefInstance.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(Form2.DefInstance.Width)) / 2)

        drvList.Drive = Form1.DefInstance.txtPicDir.Text
        dirList.Path = Form1.DefInstance.txtPicDir.Text
        txtPath.Text = dirList.Path
    End Sub
End Class