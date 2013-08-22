'
' Author:
'	Hiroshi Saito.(z-saito@guitar.ocn.ne.jp)
'
'	Copyright (C) 2002-2006 The Npgsql Development Team
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
'
' This library is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
' Lesser General Public License for more details.
'
' You should have received a copy of the GNU Lesser General Public
' License along with this library; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

Option Strict Off
Option Explicit On 
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports VB = Microsoft.VisualBasic
Imports Npgsql
Imports NpgsqlTypes
' mcs
Imports Mono.Security.Protocol.Tls


Friend Class Form1
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
    Public WithEvents cmdDelete As System.Windows.Forms.Button
    Public WithEvents Check1 As System.Windows.Forms.CheckBox
    Public WithEvents cmdPicChange As System.Windows.Forms.Button
    Public WithEvents txtPicDir As System.Windows.Forms.TextBox
    Public WithEvents cmdDBGet As System.Windows.Forms.Button
    Public WithEvents cmdPeast As System.Windows.Forms.Button
    Public WithEvents txtShainNo As System.Windows.Forms.TextBox
    Public WithEvents txtName As System.Windows.Forms.TextBox
    Public WithEvents txtEMailaddr As System.Windows.Forms.TextBox
    Public WithEvents cmdEnd As System.Windows.Forms.Button
    Public WithEvents cmdSave As System.Windows.Forms.Button
    Public WithEvents txtTorokuNo As System.Windows.Forms.TextBox
    Public WithEvents Picture4 As System.Windows.Forms.PictureBox
    Public WithEvents Picture1 As System.Windows.Forms.Panel
    Public WithEvents Picture3 As System.Windows.Forms.PictureBox
    Public WithEvents Picture2 As System.Windows.Forms.PictureBox
    Public WithEvents Label6 As System.Windows.Forms.Label
    Public WithEvents Label8 As System.Windows.Forms.Label
    Public WithEvents Label7 As System.Windows.Forms.Label
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents menLogin As System.Windows.Forms.MenuItem
    Public WithEvents menEnd As System.Windows.Forms.MenuItem
    Public WithEvents menFile As System.Windows.Forms.MenuItem
    Public WithEvents menPeast As System.Windows.Forms.MenuItem
    Public WithEvents menHenshu As System.Windows.Forms.MenuItem
    Public WithEvents menCreate As System.Windows.Forms.MenuItem
    Public WithEvents menTouroku As System.Windows.Forms.MenuItem
    Public MainMenu1 As System.Windows.Forms.MainMenu

    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Public WithEvents Label9 As System.Windows.Forms.Label
    Public WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents RBBmp As System.Windows.Forms.RadioButton
    Friend WithEvents RBJpg As System.Windows.Forms.RadioButton
    Friend WithEvents RBPng As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdDelete = New System.Windows.Forms.Button
        Me.Check1 = New System.Windows.Forms.CheckBox
        Me.cmdPicChange = New System.Windows.Forms.Button
        Me.cmdDBGet = New System.Windows.Forms.Button
        Me.cmdPeast = New System.Windows.Forms.Button
        Me.txtShainNo = New System.Windows.Forms.TextBox
        Me.txtName = New System.Windows.Forms.TextBox
        Me.txtEMailaddr = New System.Windows.Forms.TextBox
        Me.cmdSave = New System.Windows.Forms.Button
        Me.Picture4 = New System.Windows.Forms.PictureBox
        Me.RBBmp = New System.Windows.Forms.RadioButton
        Me.RBJpg = New System.Windows.Forms.RadioButton
        Me.RBPng = New System.Windows.Forms.RadioButton
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.cmdEnd = New System.Windows.Forms.Button
        Me.txtPicDir = New System.Windows.Forms.TextBox
        Me.txtTorokuNo = New System.Windows.Forms.TextBox
        Me.Picture1 = New System.Windows.Forms.Panel
        Me.Picture3 = New System.Windows.Forms.PictureBox
        Me.Picture2 = New System.Windows.Forms.PictureBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.MainMenu1 = New System.Windows.Forms.MainMenu
        Me.menFile = New System.Windows.Forms.MenuItem
        Me.menLogin = New System.Windows.Forms.MenuItem
        Me.menEnd = New System.Windows.Forms.MenuItem
        Me.menHenshu = New System.Windows.Forms.MenuItem
        Me.menPeast = New System.Windows.Forms.MenuItem
        Me.menTouroku = New System.Windows.Forms.MenuItem
        Me.menCreate = New System.Windows.Forms.MenuItem
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.Picture1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdDelete
        '
        Me.cmdDelete.BackColor = System.Drawing.SystemColors.Control
        Me.cmdDelete.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdDelete.Enabled = False
        Me.cmdDelete.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDelete.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdDelete.Location = New System.Drawing.Point(224, 80)
        Me.cmdDelete.Name = "cmdDelete"
        Me.cmdDelete.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdDelete.Size = New System.Drawing.Size(72, 24)
        Me.cmdDelete.TabIndex = 7
        Me.cmdDelete.Text = "&Remove"
        Me.ToolTip1.SetToolTip(Me.cmdDelete, "Data and Picture are Remove.")
        '
        'Check1
        '
        Me.Check1.BackColor = System.Drawing.Color.FromArgb(CType(128, Byte), CType(128, Byte), CType(255, Byte))
        Me.Check1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Check1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Check1.ForeColor = System.Drawing.Color.Black
        Me.Check1.Location = New System.Drawing.Point(24, 208)
        Me.Check1.Name = "Check1"
        Me.Check1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Check1.Size = New System.Drawing.Size(192, 20)
        Me.Check1.TabIndex = 9
        Me.Check1.Text = "Picture is saved a local folder."
        Me.ToolTip1.SetToolTip(Me.Check1, "It is not concerned with a database but preservation of picture to be a local holder.")
        '
        'cmdPicChange
        '
        Me.cmdPicChange.BackColor = System.Drawing.SystemColors.Control
        Me.cmdPicChange.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdPicChange.Enabled = False
        Me.cmdPicChange.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPicChange.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdPicChange.Location = New System.Drawing.Point(208, 272)
        Me.cmdPicChange.Name = "cmdPicChange"
        Me.cmdPicChange.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdPicChange.Size = New System.Drawing.Size(81, 24)
        Me.cmdPicChange.TabIndex = 10
        Me.cmdPicChange.Text = "&Browse"
        Me.ToolTip1.SetToolTip(Me.cmdPicChange, "Select saved directry")
        '
        'cmdDBGet
        '
        Me.cmdDBGet.BackColor = System.Drawing.SystemColors.Control
        Me.cmdDBGet.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdDBGet.Enabled = False
        Me.cmdDBGet.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDBGet.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdDBGet.Location = New System.Drawing.Point(224, 48)
        Me.cmdDBGet.Name = "cmdDBGet"
        Me.cmdDBGet.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdDBGet.Size = New System.Drawing.Size(72, 24)
        Me.cmdDBGet.TabIndex = 6
        Me.cmdDBGet.Text = "&Query"
        Me.ToolTip1.SetToolTip(Me.cmdDBGet, "Query to IDNumber.")
        '
        'cmdPeast
        '
        Me.cmdPeast.BackColor = System.Drawing.SystemColors.Control
        Me.cmdPeast.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdPeast.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPeast.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdPeast.Location = New System.Drawing.Point(224, 16)
        Me.cmdPeast.Name = "cmdPeast"
        Me.cmdPeast.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdPeast.Size = New System.Drawing.Size(72, 24)
        Me.cmdPeast.TabIndex = 5
        Me.cmdPeast.Text = "&Paste"
        Me.ToolTip1.SetToolTip(Me.cmdPeast, "Please paste a picture.")
        '
        'txtShainNo
        '
        Me.txtShainNo.AcceptsReturn = True
        Me.txtShainNo.AutoSize = False
        Me.txtShainNo.BackColor = System.Drawing.SystemColors.Window
        Me.txtShainNo.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtShainNo.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtShainNo.Location = New System.Drawing.Point(56, 16)
        Me.txtShainNo.MaxLength = 0
        Me.txtShainNo.Name = "txtShainNo"
        Me.txtShainNo.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtShainNo.Size = New System.Drawing.Size(112, 20)
        Me.txtShainNo.TabIndex = 1
        Me.txtShainNo.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtShainNo, "Input IDNumber")
        '
        'txtName
        '
        Me.txtName.AcceptsReturn = True
        Me.txtName.AutoSize = False
        Me.txtName.BackColor = System.Drawing.SystemColors.Window
        Me.txtName.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.txtName.Enabled = False
        Me.txtName.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtName.Location = New System.Drawing.Point(56, 48)
        Me.txtName.MaxLength = 0
        Me.txtName.Name = "txtName"
        Me.txtName.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtName.Size = New System.Drawing.Size(160, 20)
        Me.txtName.TabIndex = 2
        Me.txtName.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtName, "FullName")
        '
        'txtEMailaddr
        '
        Me.txtEMailaddr.AcceptsReturn = True
        Me.txtEMailaddr.AutoSize = False
        Me.txtEMailaddr.BackColor = System.Drawing.SystemColors.Window
        Me.txtEMailaddr.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtEMailaddr.Enabled = False
        Me.txtEMailaddr.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtEMailaddr.ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf
        Me.txtEMailaddr.Location = New System.Drawing.Point(56, 80)
        Me.txtEMailaddr.MaxLength = 0
        Me.txtEMailaddr.Name = "txtEMailaddr"
        Me.txtEMailaddr.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtEMailaddr.Size = New System.Drawing.Size(160, 19)
        Me.txtEMailaddr.TabIndex = 3
        Me.txtEMailaddr.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtEMailaddr, "E_Mail")
        '
        'cmdSave
        '
        Me.cmdSave.BackColor = System.Drawing.SystemColors.Control
        Me.cmdSave.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdSave.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdSave.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdSave.Location = New System.Drawing.Point(224, 112)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdSave.Size = New System.Drawing.Size(72, 24)
        Me.cmdSave.TabIndex = 4
        Me.cmdSave.Text = "&Save"
        Me.ToolTip1.SetToolTip(Me.cmdSave, "Data and Picture are saved.")
        '
        'Picture4
        '
        Me.Picture4.BackColor = System.Drawing.SystemColors.Control
        Me.Picture4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Picture4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Picture4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Picture4.Location = New System.Drawing.Point(52, 63)
        Me.Picture4.Name = "Picture4"
        Me.Picture4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Picture4.Size = New System.Drawing.Size(220, 200)
        Me.Picture4.TabIndex = 19
        Me.Picture4.TabStop = False
        Me.ToolTip1.SetToolTip(Me.Picture4, "The attachement place of a picture(Left cleck:Expansion Right click:Reduction)")
        '
        'RBBmp
        '
        Me.RBBmp.Checked = True
        Me.RBBmp.Location = New System.Drawing.Point(8, 24)
        Me.RBBmp.Name = "RBBmp"
        Me.RBBmp.Size = New System.Drawing.Size(16, 16)
        Me.RBBmp.TabIndex = 24
        Me.RBBmp.TabStop = True
        Me.RBBmp.Text = "RadioButton1"
        Me.ToolTip1.SetToolTip(Me.RBBmp, "BMP")
        '
        'RBJpg
        '
        Me.RBJpg.Location = New System.Drawing.Point(56, 24)
        Me.RBJpg.Name = "RBJpg"
        Me.RBJpg.Size = New System.Drawing.Size(16, 16)
        Me.RBJpg.TabIndex = 25
        Me.RBJpg.Text = "RadioButton2"
        Me.ToolTip1.SetToolTip(Me.RBJpg, "JPG")
        '
        'RBPng
        '
        Me.RBPng.Location = New System.Drawing.Point(104, 24)
        Me.RBPng.Name = "RBPng"
        Me.RBPng.Size = New System.Drawing.Size(16, 16)
        Me.RBPng.TabIndex = 26
        Me.RBPng.Text = "RadioButton3"
        Me.ToolTip1.SetToolTip(Me.RBPng, "PNG")
        '
        'GroupBox2
        '
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(8, 312)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(272, 64)
        Me.GroupBox2.TabIndex = 31
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Status"
        Me.ToolTip1.SetToolTip(Me.GroupBox2, "ImageMake status")
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.RBBmp)
        Me.GroupBox1.Controls.Add(Me.RBJpg)
        Me.GroupBox1.Controls.Add(Me.RBPng)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(24, 144)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(160, 48)
        Me.GroupBox1.TabIndex = 30
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Picture Kind"
        Me.ToolTip1.SetToolTip(Me.GroupBox1, "Select kind picture")
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(24, 24)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(32, 16)
        Me.Label10.TabIndex = 27
        Me.Label10.Text = "BMP"
        '
        'Label11
        '
        Me.Label11.Location = New System.Drawing.Point(72, 24)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(32, 16)
        Me.Label11.TabIndex = 28
        Me.Label11.Text = "JPG"
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(120, 24)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(32, 16)
        Me.Label12.TabIndex = 29
        Me.Label12.Text = "PNG"
        '
        'cmdEnd
        '
        Me.cmdEnd.BackColor = System.Drawing.SystemColors.Control
        Me.cmdEnd.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdEnd.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdEnd.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdEnd.Location = New System.Drawing.Point(224, 144)
        Me.cmdEnd.Name = "cmdEnd"
        Me.cmdEnd.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdEnd.Size = New System.Drawing.Size(72, 24)
        Me.cmdEnd.TabIndex = 8
        Me.cmdEnd.Text = "E&xit"
        '
        'txtPicDir
        '
        Me.txtPicDir.AcceptsReturn = True
        Me.txtPicDir.AutoSize = False
        Me.txtPicDir.BackColor = System.Drawing.SystemColors.Window
        Me.txtPicDir.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtPicDir.Enabled = False
        Me.txtPicDir.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtPicDir.Location = New System.Drawing.Point(24, 248)
        Me.txtPicDir.MaxLength = 0
        Me.txtPicDir.Name = "txtPicDir"
        Me.txtPicDir.ReadOnly = True
        Me.txtPicDir.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtPicDir.Size = New System.Drawing.Size(264, 20)
        Me.txtPicDir.TabIndex = 12
        Me.txtPicDir.TabStop = False
        Me.txtPicDir.Text = ""
        '
        'txtTorokuNo
        '
        Me.txtTorokuNo.AcceptsReturn = True
        Me.txtTorokuNo.AutoSize = False
        Me.txtTorokuNo.BackColor = System.Drawing.SystemColors.Window
        Me.txtTorokuNo.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtTorokuNo.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtTorokuNo.Location = New System.Drawing.Point(56, 112)
        Me.txtTorokuNo.MaxLength = 0
        Me.txtTorokuNo.Name = "txtTorokuNo"
        Me.txtTorokuNo.ReadOnly = True
        Me.txtTorokuNo.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtTorokuNo.Size = New System.Drawing.Size(160, 20)
        Me.txtTorokuNo.TabIndex = 0
        Me.txtTorokuNo.TabStop = False
        Me.txtTorokuNo.Text = ""
        '
        'Picture1
        '
        Me.Picture1.BackColor = System.Drawing.SystemColors.Control
        Me.Picture1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Picture1.Controls.Add(Me.Picture4)
        Me.Picture1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Picture1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Picture1.Location = New System.Drawing.Point(304, 8)
        Me.Picture1.Name = "Picture1"
        Me.Picture1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Picture1.Size = New System.Drawing.Size(330, 320)
        Me.Picture1.TabIndex = 11
        '
        'Picture3
        '
        Me.Picture3.BackColor = System.Drawing.SystemColors.Control
        Me.Picture3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Picture3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Picture3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Picture3.Location = New System.Drawing.Point(304, 8)
        Me.Picture3.Name = "Picture3"
        Me.Picture3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Picture3.Size = New System.Drawing.Size(330, 320)
        Me.Picture3.TabIndex = 18
        Me.Picture3.TabStop = False
        Me.Picture3.Visible = False
        '
        'Picture2
        '
        Me.Picture2.BackColor = System.Drawing.SystemColors.Control
        Me.Picture2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Picture2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Picture2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Picture2.Location = New System.Drawing.Point(304, 8)
        Me.Picture2.Name = "Picture2"
        Me.Picture2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Picture2.Size = New System.Drawing.Size(323, 315)
        Me.Picture2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.Picture2.TabIndex = 17
        Me.Picture2.TabStop = False
        Me.Picture2.Visible = False
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label6.Enabled = False
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(24, 232)
        Me.Label6.Name = "Label6"
        Me.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label6.Size = New System.Drawing.Size(112, 16)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "Preservation plase"
        '
        'Label8
        '
        Me.Label8.BackColor = System.Drawing.Color.Transparent
        Me.Label8.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Black
        Me.Label8.Location = New System.Drawing.Point(304, 344)
        Me.Label8.Name = "Label8"
        Me.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label8.Size = New System.Drawing.Size(328, 40)
        Me.Label8.TabIndex = 22
        Me.Label8.Text = "Mouse cursor is put on an image position, and it reduces by expantion and left cl" & _
        "ick by right click"
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label7.ForeColor = System.Drawing.Color.FromArgb(CType(255, Byte), CType(255, Byte), CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(136, 392)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(360, 16)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "Copyright(C) 2002-2006 Npgsql Developer Group"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Enabled = False
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(8, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(56, 17)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Name"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label3.Enabled = False
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(8, 80)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(40, 17)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "E-Mail"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(8, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(32, 17)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = "ID"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(8, 112)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(48, 17)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Edit No."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menFile, Me.menHenshu, Me.menTouroku})
        '
        'menFile
        '
        Me.menFile.Index = 0
        Me.menFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menLogin, Me.menEnd})
        Me.menFile.Text = "&File"
        '
        'menLogin
        '
        Me.menLogin.Index = 0
        Me.menLogin.Text = "&Login"
        '
        'menEnd
        '
        Me.menEnd.Index = 1
        Me.menEnd.Text = "E&xit"
        '
        'menHenshu
        '
        Me.menHenshu.Index = 1
        Me.menHenshu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menPeast})
        Me.menHenshu.Text = "&Edit"
        '
        'menPeast
        '
        Me.menPeast.Index = 0
        Me.menPeast.Text = "&Paste"
        '
        'menTouroku
        '
        Me.menTouroku.Index = 2
        Me.menTouroku.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menCreate})
        Me.menTouroku.Text = "&InitialSetting"
        '
        'menCreate
        '
        Me.menCreate.Index = 0
        Me.menCreate.Text = "&CreateTable"
        '
        'Label9
        '
        Me.Label9.BackColor = System.Drawing.Color.Transparent
        Me.Label9.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(24, 334)
        Me.Label9.Name = "Label9"
        Me.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label9.Size = New System.Drawing.Size(233, 17)
        Me.Label9.TabIndex = 25
        Me.Label9.Text = ".Not Connected Server"
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(24, 352)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(233, 17)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = ".Picture is not saved in a local holder"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.BackColor = System.Drawing.Color.FromArgb(CType(128, Byte), CType(128, Byte), CType(255, Byte))
        Me.ClientSize = New System.Drawing.Size(642, 417)
        Me.Controls.Add(Me.cmdDelete)
        Me.Controls.Add(Me.cmdDBGet)
        Me.Controls.Add(Me.cmdPeast)
        Me.Controls.Add(Me.txtShainNo)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.txtEMailaddr)
        Me.Controls.Add(Me.txtTorokuNo)
        Me.Controls.Add(Me.txtPicDir)
        Me.Controls.Add(Me.cmdEnd)
        Me.Controls.Add(Me.cmdSave)
        Me.Controls.Add(Me.Picture1)
        Me.Controls.Add(Me.Picture3)
        Me.Controls.Add(Me.Picture2)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cmdPicChange)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Check1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Location = New System.Drawing.Point(-16, 168)
        Me.Menu = Me.MainMenu1
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "ImageMake(Npgsql) Powered by PostgreSQL"
        Me.GroupBox1.ResumeLayout(False)
        Me.Picture1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private Shared m_vb6FormDefInstance As Form1
    Private Shared m_InitializingDefInstance As Boolean
    Public Shared Property DefInstance() As Form1
        Get
            If m_vb6FormDefInstance Is Nothing OrElse m_vb6FormDefInstance.IsDisposed Then
                m_InitializingDefInstance = True
                m_vb6FormDefInstance = New Form1
                m_InitializingDefInstance = False
            End If
            DefInstance = m_vb6FormDefInstance
        End Get
        Set(ByVal Value As Form1)
            m_vb6FormDefInstance = Value
        End Set
    End Property


    Dim SaveStatus As Boolean

    Dim DbServer As String
    Dim UserName As String
    Dim UserPass As String
    Dim TableName As String
    Dim DbName As String

    Public NpgConn As Npgsql.NpgsqlConnection
    Public NpgCmd As Npgsql.NpgsqlCommand
    Public NpgDRead As Npgsql.NpgsqlDataReader
    Public NpgTrans As Npgsql.NpgsqlTransaction
    Public NpgDAdap As Npgsql.NpgsqlDataAdapter
    Public NpgCmdBld As Npgsql.NpgsqlCommandBuilder

    Public SysDset As DataSet
    Public SysDrow As DataRow

    Public TableChk As Object
    Public LoginChk As Object

    Dim Check1Chk As Short

    Public PicImg As Image
    Public PicBmap As Bitmap
    Public PicGrap As Graphics
    Public intPoint As Short

    Dim strBmpPath As Object
    Dim strBmpName As Object
    Dim que As Object
    Dim loid As Integer
    Dim delSQL As Object
    Dim Response As Object
    Dim Style As Object
    Dim Msg As Object
    Dim Mydata() As Byte
    Dim fs As FileStream


    ' Main from loading ...
    Private Sub Form1_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        Dim txtFilDir As Object
        Dim strCurDrv As String
        Dim strCurPicPath As String
        Dim strCurFilPath As String

        ' Save Status flag.
        SaveStatus = False

        strCurDrv = Mid(CurDir(), 1, 3)
        strCurPicPath = strCurDrv & ""
        strCurFilPath = strCurDrv & ""

        If Dir(strCurPicPath, FileAttribute.Directory) <> "" Then
            txtPicDir.Text = strCurPicPath
        Else
            txtPicDir.Text = VB6.GetPath
        End If

        If Dir(strCurFilPath, FileAttribute.Directory) <> "" Then
            txtFilDir = strCurFilPath
        Else
            txtFilDir = VB6.GetPath
        End If

        Form1.DefInstance.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(Form1.DefInstance.Height)) / 2)
        Form1.DefInstance.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(Form1.DefInstance.Width)) / 2)
    End Sub

    'Create Table
    Public Sub menCreate_Popup(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menCreate.Popup
        menCreate_Click(eventSender, eventArgs)
    End Sub
    Public Sub menCreate_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menCreate.Click
        Form4.DefInstance.ShowDialog()

        ' Connect infomation get
        DbServer = Form4.DefInstance.txtDbSouce.Text
        UserName = Form4.DefInstance.txtUserName.Text
        UserPass = Form4.DefInstance.txtUserPass.Text
        TableName = Form4.DefInstance.txtTableName.Text
        DbName = Form4.DefInstance.txtDbName.Text

        On Error GoTo menCreateErr

        ' Clicked on "1"
        If TableChk = 1 Then
            connection()
            NpgConn.Open()

            NpgCmd = NpgConn.CreateCommand()

            NpgCmd.CommandText = "CREATE TABLE " & TableName & " (IDNumber TEXT PRIMARY KEY,FullName TEXT,E_Mail TEXT,EditNo TEXT,Image OID, Kind TEXT)"
            NpgCmd.ExecuteNonQuery()

            NpgConn.Close()

            MsgBox("Database:" & DbName & ",TableName:" & TableName & " Created.")
            Return
menCreateErr:
            MsgBox("Error:Server connection problem!")
        End If

        Form4.DefInstance.Close()
    End Sub

    'Login
    Public Sub menLogin_Popup(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menLogin.Popup
        menLogin_Click(eventSender, eventArgs)
    End Sub
    Public Sub menLogin_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menLogin.Click
        Form3.DefInstance.ShowDialog()

        'Form3
        DbServer = Form3.DefInstance.txtDbSouce.Text
        UserName = Form3.DefInstance.txtUserName.Text
        UserPass = Form3.DefInstance.txtUserPass.Text
        TableName = Form3.DefInstance.txtTableName.Text
        DbName = Form3.DefInstance.txtDbName.Text

        'Form3 Clicked "1"
        If LoginChk = 1 Then
            connection()
            cmdDBGet.Enabled = True
            Label4.Enabled = True
            txtName.Enabled = True
            Label3.Enabled = True
            txtEMailaddr.Enabled = True
            cmdDelete.Enabled = True
            Label9.Text = ".Establish Connected Server"
        Else
            cmdDBGet.Enabled = False
            Label4.Enabled = False
            txtName.Enabled = False
            Label3.Enabled = False
            txtEMailaddr.Enabled = False
            cmdDelete.Enabled = False
            Label9.Text = ".Not Connected Server"
        End If

        Form3.DefInstance.Close()
    End Sub
    'Login

    Public Sub connection()
        'Server connect
        NpgConn = New Npgsql.NpgsqlConnection("Server=" & DbServer & ";User Id=" & UserName & ";Password=" & UserPass & ";Database=" & DbName & ";Encoding=UNICODE;")
    End Sub

    Private Sub txtShainNo_Leave(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtShainNo.Leave
        txtTorokuNo.Text = txtShainNo.Text
    End Sub

    ' Paste process
    Private Sub cmdPeast_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdPeast.Click
        proPeast()
    End Sub
    Public Sub menPeast_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menPeast.Click
        proPeast()
    End Sub
    Private Sub menPeast_Popup(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menPeast.Popup
        menPeast_Click(eventSender, eventArgs)
    End Sub
    Public Sub proPeast()
        'Clipbord to Image
        PicImg = Clipboard.GetDataObject.GetData(System.Windows.Forms.DataFormats.Bitmap)

        If Not (PicImg Is Nothing) Then
            'Picture4 Graphics
            PicGrap = Picture4.CreateGraphics()

            Picture4.Width = PicImg.Width
            Picture4.Height = PicImg.Height
            'SysDrow
            PicGrap.DrawImage(PicImg, 0, 0, Picture4.Width, Picture4.Height)
            PicGrap.Dispose()
            '
            Picture4.Left = (330 - Picture4.Width) / 2
            Picture4.Top = (320 - Picture4.Height) / 2
            SaveStatus = True
        End If

    End Sub
    Private Sub Picture4_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Picture4.Paint
        'Needs Paint event
        If Not (PicImg Is Nothing) Then
            e.Graphics.DrawImage(PicImg, 0, 0, Picture4.Width, Picture4.Height)
        End If
    End Sub

    'Picture sise replase
    Private Sub Picture1_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles Picture1.MouseDown
        Dim Button As Short = eventArgs.Button \ &H100000
        If Not (PicImg Is Nothing) Then
            If Button = 1 Then
                intPoint = 10
            Else
                intPoint = -10
            End If
            Picture_Size()
        End If
    End Sub
    Private Sub Picture4_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles Picture4.MouseDown
        Dim Button As Short = eventArgs.Button \ &H100000
        If Not (PicImg Is Nothing) Then
            If Button = 1 Then
                intPoint = 10
            Else
                intPoint = -10
            End If
            Picture_Size()
        End If
    End Sub
    Public Sub Picture_Size()
        '
        Picture4.Width = Picture4.Width + intPoint
        Picture4.Height = Picture4.Height + intPoint

        'Picture4 Image set
        Picture4.SizeMode = PictureBoxSizeMode.CenterImage
        Picture4.Size = New Size(Picture4.Width, Picture4.Height)
        Picture4.Image = CType(PicImg, Image)
        ' empty reset
        PicImg = Picture4.Image
        Picture4.Image = Nothing
        '
        Picture4.Left = (330 - Picture4.Width) / 2
        Picture4.Top = (320 - Picture4.Height) / 2
    End Sub

    'Form2(BIG)
    Private Sub cmdPicChange_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdPicChange.Click
        Form2.DefInstance.ShowDialog()

        If Form2.DefInstance.txtPath.Text <> "" Then
            txtPicDir.Text = Form2.DefInstance.txtPath.Text
        End If

        Form2.DefInstance.Close()
    End Sub

    ' Save(BIG)
    Private Sub Check1_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Check1.CheckStateChanged
        'status
        If Check1.CheckState = System.Windows.Forms.CheckState.Checked Then
            cmdPicChange.Enabled = True
            Label6.Enabled = True
            txtPicDir.Enabled = True
            Check1Chk = 1
            Label5.Text = ".Picture is saved in a local holder"
        Else
            cmdPicChange.Enabled = False
            Label6.Enabled = False
            txtPicDir.Enabled = False
            Check1Chk = 0
            Label5.Text = ".Picture is not saved in a local holder"
        End If
    End Sub
    Private Sub cmdSave_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSave.Click
        proPictureSave()
    End Sub
    Public Sub proPictureSave()
        Dim ExistFlg As Boolean

        'check
        If txtShainNo.Text = "" Then
            MsgBox("Please input IDNumber.", , "Warning")
        ElseIf (PicImg Is Nothing) Then
            MsgBox("Picture not setting", , "Warning")
        End If
        'Login
        If Check1Chk = 0 And LoginChk = 0 Then
            MsgBox("Please Login first.")
            Exit Sub
        End If
        ' Picture Name
        If txtTorokuNo.Text <> "" And Not (PicImg Is Nothing) Then
            If RBBmp.Checked Then
                strBmpName = RTrim(txtShainNo.Text) & ".bmp"
            ElseIf RBJpg.Checked Then
                strBmpName = RTrim(txtShainNo.Text) & ".jpg"
            ElseIf RBPng.Checked Then
                strBmpName = RTrim(txtShainNo.Text) & ".png"
            End If

            If VB.Right(txtPicDir.Text, 1) = "\" Then
                strBmpPath = txtPicDir.Text & strBmpName
            Else
                strBmpPath = txtPicDir.Text & "\" & strBmpName
            End If
        Else
            Exit Sub
        End If

        ExistFlg = False

        If Dir(strBmpPath, FileAttribute.Normal) = "" Then
            If Check1Chk = 0 And LoginChk = 1 Then
                Msg = "Picture " & strBmpName & " is not regstered. Is it all right?"
            Else
                Msg = "Picture " & strBmpName & " is regstered. Is it all right?"
            End If
        Else
            If Check1Chk = 0 And LoginChk = 1 Then
                Msg = "Picture " & strBmpName & " is already regstered. Does it delete?"
            Else
                Msg = "Picture " & strBmpName & " is already regstered. Does it overwrite?"
            End If
        End If

        Style = MsgBoxStyle.YesNo
        Response = MsgBox(Msg, Style, "Attention")

        If Response = MsgBoxResult.No Then Exit Sub

        If LoginChk = 1 Then
            On Error GoTo NoData
            connection()
            NpgConn.Open()
            'SELECT
            NpgCmd = New Npgsql.NpgsqlCommand("SELECT * FROM " & TableName & " where IDNumber='" & txtTorokuNo.Text & "'", NpgConn)
            NpgDRead = NpgCmd.ExecuteReader
            NpgDRead.Read()
            txtTorokuNo.Text = NpgDRead.GetString(3)
            NpgDRead.Close()
            'OID get
            NpgCmd.CommandText = "SELECT Image::oid FROM " & TableName & " where IDNumber='" & txtTorokuNo.Text & "'"
            NpgDRead = NpgCmd.ExecuteReader()
            NpgDRead.Read()
            loid = NpgDRead.GetValue(0)
            NpgDRead.Close()

            NpgConn.Close()
            '
            ExistFlg = True
            Msg = "Database " & txtTorokuNo.Text & "is already regstered. Does it overwrite?"
            GoTo data
NoData:
            ExistFlg = False
            Msg = "Database " & txtTorokuNo.Text & "is regstered. Is it all right?"

data:
            Style = MsgBoxStyle.YesNo
            Response = MsgBox(Msg, Style, "Attention")

            If Response = MsgBoxResult.No Then Exit Sub

        End If

        Dim CutSizeX As Integer
        Dim CutSizeY As Integer
        '
        CutSizeX = (Picture3.Width - Picture4.Width) / 2
        CutSizeY = (Picture3.Height - Picture4.Height) / 2

        'Picture3 size PicBmap
        PicBmap = New Bitmap(Picture3.Width, Picture3.Height)
        'PicBmp Graphics
        PicGrap = Graphics.FromImage(PicBmap)
        'PicBmp Draw
        PicGrap.DrawImage(PicImg, CutSizeX, CutSizeY, Picture4.Width, Picture4.Height)
        PicGrap.Dispose()
        '
        Picture3.Image = PicBmap
        'Picture3 Picture Kind
        If RBBmp.Checked Then
            Picture3.Image.Save(strBmpPath, System.Drawing.Imaging.ImageFormat.Bmp)
        ElseIf RBJpg.Checked Then
            Picture3.Image.Save(strBmpPath, System.Drawing.Imaging.ImageFormat.Jpeg)
        ElseIf RBPng.Checked Then
            Picture3.Image.Save(strBmpPath, System.Drawing.Imaging.ImageFormat.Png)
        End If
        'Login check
        If LoginChk = 0 Then GoTo Image

        On Error GoTo Commit

        Dim lbm As LargeObjectManager

        If ExistFlg = True Then
            '
            connection()
            NpgConn.Open()

            NpgCmd = NpgConn.CreateCommand()

            NpgTrans = NpgConn.BeginTransaction(IsolationLevel.ReadCommitted)
            NpgCmd.Transaction = NpgTrans

            NpgCmd.CommandText = "DELETE FROM " & TableName & " WHERE IDNumber='" & txtTorokuNo.Text & "'"
            NpgCmd.ExecuteNonQuery()
            'LargeObject 
            lbm = New LargeObjectManager(NpgConn)
            lbm.Delete(loid)

            NpgTrans.Commit()
            NpgConn.Close()

            MsgBox("Completion of Remove.")
        End If

        connection()
        NpgConn.Open()

        NpgDAdap = New Npgsql.NpgsqlDataAdapter("SELECT * FROM " & TableName & " WHERE IDNumber='" & txtTorokuNo.Text & "'", NpgConn)
        NpgCmdBld = New Npgsql.NpgsqlCommandBuilder(NpgDAdap)

        SysDset = New DataSet

        ' Save Picture
        Dim fs As New FileStream(strBmpPath, FileMode.OpenOrCreate, FileAccess.Read)
        Dim MyData(fs.Length) As Byte
        Dim noid As Integer
        Dim lo As LargeObject

        fs.Read(MyData, 0, fs.Length)
        fs.Close()

        NpgTrans = NpgConn.BeginTransaction(IsolationLevel.ReadCommitted)

        NpgDAdap.Fill(SysDset, TableName)

        ' New LargeObject Set
        lbm = New LargeObjectManager(NpgConn)
        noid = lbm.Create(LargeObjectManager.READWRITE)
        lo = lbm.Open(noid, LargeObjectManager.READWRITE)

        lo.Write(MyData, 0, MyData.Length)
        lo.Close()

        SysDrow = SysDset.Tables(TableName).NewRow()
        '
        SysDrow("IDNumber") = txtShainNo.Text
        SysDrow("FullName") = txtName.Text
        SysDrow("E_Mail") = txtEMailaddr.Text
        SysDrow("EditNo") = txtTorokuNo.Text
        'SysDrow("Image") = MyData
        SysDrow("Image") = noid

        If RBBmp.Checked Then
            SysDrow("Kind") = "BMP"
        ElseIf RBJpg.Checked Then
            SysDrow("Kind") = "JPG"
        ElseIf RBPng.Checked Then
            SysDrow("Kind") = "PNG"
        End If

        ' update
        SysDset.Tables(TableName).Rows.Add(SysDrow)
        NpgDAdap.Update(SysDset, TableName)

        '
        fs = Nothing
        NpgCmdBld = Nothing
        SysDset = Nothing
        NpgDAdap = Nothing

        NpgTrans.Commit()
        NpgConn.Close()

        MsgBox("Preservation was complated.")

        GoTo Image
Commit:
        MsgBox("Commit for an error!")
        NpgTrans.Rollback()
        NpgConn.Close()
Image:
        If Check1Chk = 0 Then
            'Remove Picture
            Kill(strBmpPath)
        ElseIf Check1Chk = 1 And LoginChk = 0 Then
            MsgBox("Only a Picture is saved.")
        End If

        txtTorokuNo.Text = ""
        txtShainNo.Text = ""
        txtEMailaddr.Text = ""
        txtName.Text = ""
        PicImg = Nothing
        Picture3.Image = Nothing
        Picture4.Image = Nothing
        Picture4.Width = 220
        Picture4.Height = 200
        Picture4.Left = (330 - Picture4.Width) / 2
        Picture4.Top = (320 - Picture4.Height) / 2
        PicGrap = Nothing
        SaveStatus = False
    End Sub

    ' Read to Database
    Private Sub cmdDBGet_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdDBGet.Click
        PicImg = Nothing
        Picture4.Image = Nothing
        Picture4.Width = 220
        Picture4.Height = 200
        Picture4.Left = (330 - Picture4.Width) / 2
        Picture4.Top = (320 - Picture4.Height) / 2

        On Error GoTo NoData

        ' CHeck an indispensable item
        If txtShainNo.Text = "" Then
            MsgBox("Please input IDNumber.", , "Warning")
            txtEMailaddr.Text = ""
            txtName.Text = ""
            Exit Sub
        End If

        connection()
        ' Database SELECT
        Dim noid As Integer
        Dim nkind As Char
        Dim lbm As LargeObjectManager
        Dim lo As LargeObject

        NpgConn.Open()

        NpgTrans = NpgConn.BeginTransaction(IsolationLevel.ReadCommitted)

        NpgCmd = New Npgsql.NpgsqlCommand("SELECT * FROM " & TableName & " WHERE IDNumber='" & txtTorokuNo.Text & "'", NpgConn)
        NpgDRead = NpgCmd.ExecuteReader()
        NpgDRead.Read()
        ' Display setting
        txtShainNo.Text = NpgDRead.GetString(0)
        txtName.Text = NpgDRead.GetString(1)
        txtEMailaddr.Text = NpgDRead.GetString(2)
        txtTorokuNo.Text = NpgDRead.GetString(3)
        noid = NpgDRead.GetValue(4)
        nkind = NpgDRead.GetString(5)

        ' Search Picture
        lbm = New LargeObjectManager(NpgConn)
        lo = lbm.Open(noid, LargeObjectManager.READ)

        Mydata = lo.Read(lo.Size)

        lo.Close()
        NpgTrans.Commit()
        NpgConn.Close()
        '
        strBmpName = RTrim(txtTorokuNo.Text) & "temp.bmp"
        strBmpPath = VB6.GetPath & "\" & strBmpName

        Dim K As Long
        K = UBound(Mydata)
        fs = New FileStream(strBmpPath, FileMode.OpenOrCreate, FileAccess.Write)

        fs.Write(Mydata, 0, K)
        fs.Close()

        fs = Nothing
        NpgCmdBld = Nothing
        SysDset = Nothing
        NpgDAdap = Nothing
        NpgConn = Nothing

        Picture_Set()

        GoTo Lastline

NoData:
        MsgBox("No Record set.")
        NpgDRead.Close()
        NpgConn.Close()
        txtEMailaddr.Text = ""
        txtName.Text = ""
        GoTo cmdDBGet_End
fserr:
        MsgBox("Error of FileStream.")

        NpgCmdBld = Nothing
        SysDset = Nothing
        NpgDAdap = Nothing
        NpgConn.Close()
        NpgConn = Nothing
        GoTo cmdDBGet_End
Lastline:
        On Error GoTo KillErr
        '
        Kill(strBmpPath) ' Clear temp
        ' MsgBox("File Remove successed.")
        GoTo cmdDBGet_End
KillErr:
        MsgBox("TempFile Remove failed!")
cmdDBGet_End:

    End Sub
    Public Sub Picture_Set()
        On Error GoTo restart

        '
        strBmpName = RTrim(txtTorokuNo.Text) & "temp.bmp"
        strBmpPath = VB6.GetPath & "\" & strBmpName
        '
        fs = New FileStream(strBmpPath, IO.FileMode.Open, IO.FileAccess.Read)
        'PicImg set
        PicImg = System.Drawing.Image.FromStream(fs)
        fs.Close()
        ' Grapjhics create
        PicGrap = Picture4.CreateGraphics()
        Picture4.Width = PicImg.Width
        Picture4.Height = PicImg.Height
        PicGrap.DrawImage(PicImg, 0, 0, Picture4.Width, Picture4.Height)
        PicGrap.Dispose()
        Picture4.Left = (330 - Picture4.Width) / 2
        Picture4.Top = (320 - Picture4.Height) / 2
        MsgBox("Query success.")

        Exit Sub
restart:
        MsgBox("Not Display picture!")
    End Sub

    'Remove (BIG)
    Private Sub cmdDelete_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdDelete.Click
        On Error GoTo Del_Com

        Dim noid As Integer
        Dim lbm As LargeObjectManager
        Dim lo As LargeObject

        Msg = "Remove this?"
        Style = MsgBoxStyle.YesNo
        Response = MsgBox(Msg, Style, "Warning")

        If Response = MsgBoxResult.Yes Then
            connection()
            NpgConn.Open()
            NpgCmd = NpgConn.CreateCommand()
            NpgCmd.CommandText = "SELECT Image::oid from " & TableName & " where IDNumber='" & txtTorokuNo.Text & "'"
            NpgDRead = NpgCmd.ExecuteReader()
            NpgDRead.Read()
            'OID get
            noid = NpgDRead.GetValue(0)

            NpgTrans = NpgConn.BeginTransaction()
            NpgCmd.Transaction = NpgTrans
            ' Remove
            NpgCmd.CommandText = "DELETE FROM " & TableName & " WHERE IDNumber='" & txtTorokuNo.Text & "'"
            NpgCmd.ExecuteNonQuery()
            ' LargeObject remove
            lbm = New LargeObjectManager(NpgConn)
            lbm.Delete(noid)

            NpgDRead.Close()
            NpgTrans.Commit()
            NpgConn.Close()
            MsgBox("Remove success.")
        End If

        txtTorokuNo.Text = ""
        txtShainNo.Text = ""
        txtEMailaddr.Text = ""
        txtName.Text = ""
        PicImg = Nothing
        Picture4.Image = Nothing
        Picture4.Width = 220
        Picture4.Height = 200
        Picture4.Left = (330 - Picture4.Width) / 2
        Picture4.Top = (320 - Picture4.Height) / 2
        PicGrap = Nothing
        SaveStatus = False
        Exit Sub
Del_Com:
        MsgBox("Commit error!")
    End Sub

    Private Sub cmdEnd_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdEnd.Click
        proEnd()
    End Sub
    Public Sub menEnd_Popup(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menEnd.Popup
        menEnd_Click(eventSender, eventArgs)
    End Sub
    Public Sub menEnd_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles menEnd.Click
        proEnd()
    End Sub
    Public Sub proEnd()
        'Check save flag
        If SaveStatus = True Then
            Msg = "is not regstered. Does it end?"
            Style = MsgBoxStyle.YesNo
            Response = MsgBox(Msg, Style, "Warning")
            If Response = MsgBoxResult.No Then
                Response = MsgBoxResult.Cancel
            End If
        End If

        If Response <> MsgBoxResult.Cancel Then
            End
        End If
    End Sub

    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label5.Click

    End Sub

    Private Sub Label4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label4.Click

    End Sub

    Private Sub txtShainNo_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtShainNo.TextChanged

    End Sub
End Class
