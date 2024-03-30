<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        txtOutput = New TextBox()
        lblOutput = New Label()
        lstHeaders = New ListBox()
        lstValues = New ListBox()
        btnExit = New Button()
        lblCntReads = New Label()
        btnPipeStop = New Button()
        Label1 = New Label()
        lblConnectStatus = New Label()
        btnJSON = New Button()
        ttSnapShot = New Timer(components)
        txtJSON = New TextBox()
        clstCOMports = New CheckedListBox()
        TimerSerialUpdate = New Timer(components)
        ToolTipPortFilters = New ToolTip(components)
        SuspendLayout()
        ' 
        ' txtOutput
        ' 
        txtOutput.Location = New Point(31, 55)
        txtOutput.Multiline = True
        txtOutput.Name = "txtOutput"
        txtOutput.Size = New Size(719, 260)
        txtOutput.TabIndex = 1
        ' 
        ' lblOutput
        ' 
        lblOutput.AutoSize = True
        lblOutput.Location = New Point(31, 37)
        lblOutput.Name = "lblOutput"
        lblOutput.Size = New Size(45, 15)
        lblOutput.TabIndex = 2
        lblOutput.Text = "Output"
        ' 
        ' lstHeaders
        ' 
        lstHeaders.FormattingEnabled = True
        lstHeaders.ItemHeight = 15
        lstHeaders.Location = New Point(793, 55)
        lstHeaders.Name = "lstHeaders"
        lstHeaders.Size = New Size(217, 889)
        lstHeaders.TabIndex = 4
        ' 
        ' lstValues
        ' 
        lstValues.FormattingEnabled = True
        lstValues.ItemHeight = 15
        lstValues.Location = New Point(1040, 55)
        lstValues.Name = "lstValues"
        lstValues.Size = New Size(267, 889)
        lstValues.TabIndex = 4
        ' 
        ' btnExit
        ' 
        btnExit.Location = New Point(630, 405)
        btnExit.Name = "btnExit"
        btnExit.Size = New Size(120, 36)
        btnExit.TabIndex = 5
        btnExit.Text = "Exit"
        btnExit.UseVisualStyleBackColor = True
        ' 
        ' lblCntReads
        ' 
        lblCntReads.AutoSize = True
        lblCntReads.Location = New Point(149, 37)
        lblCntReads.Name = "lblCntReads"
        lblCntReads.Size = New Size(13, 15)
        lblCntReads.TabIndex = 6
        lblCntReads.Text = "0"
        ' 
        ' btnPipeStop
        ' 
        btnPipeStop.Location = New Point(630, 321)
        btnPipeStop.Name = "btnPipeStop"
        btnPipeStop.Size = New Size(120, 36)
        btnPipeStop.TabIndex = 0
        btnPipeStop.Text = "Restart Connection"
        btnPipeStop.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(82, 37)
        Label1.Name = "Label1"
        Label1.Size = New Size(69, 15)
        Label1.TabIndex = 6
        Label1.Text = "read Count:"
        Label1.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' lblConnectStatus
        ' 
        lblConnectStatus.AutoSize = True
        lblConnectStatus.Location = New Point(666, 37)
        lblConnectStatus.Name = "lblConnectStatus"
        lblConnectStatus.Size = New Size(84, 15)
        lblConnectStatus.TabIndex = 7
        lblConnectStatus.Text = "not connected"
        ' 
        ' btnJSON
        ' 
        btnJSON.Location = New Point(630, 363)
        btnJSON.Name = "btnJSON"
        btnJSON.Size = New Size(120, 36)
        btnJSON.TabIndex = 5
        btnJSON.Text = "Test JSON"
        btnJSON.UseVisualStyleBackColor = True
        ' 
        ' ttSnapShot
        ' 
        ttSnapShot.Interval = 200
        ' 
        ' txtJSON
        ' 
        txtJSON.Location = New Point(1339, 55)
        txtJSON.Multiline = True
        txtJSON.Name = "txtJSON"
        txtJSON.ReadOnly = True
        txtJSON.Size = New Size(401, 889)
        txtJSON.TabIndex = 8
        ' 
        ' clstCOMports
        ' 
        clstCOMports.FormattingEnabled = True
        clstCOMports.Location = New Point(31, 321)
        clstCOMports.Name = "clstCOMports"
        clstCOMports.Size = New Size(407, 166)
        clstCOMports.TabIndex = 9
        ' 
        ' TimerSerialUpdate
        ' 
        TimerSerialUpdate.Interval = 1000
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1813, 972)
        Controls.Add(clstCOMports)
        Controls.Add(txtJSON)
        Controls.Add(lblConnectStatus)
        Controls.Add(Label1)
        Controls.Add(lblCntReads)
        Controls.Add(btnJSON)
        Controls.Add(btnExit)
        Controls.Add(lstValues)
        Controls.Add(lstHeaders)
        Controls.Add(lblOutput)
        Controls.Add(txtOutput)
        Controls.Add(btnPipeStop)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Name = "Form1"
        Text = "LS22 Telemetry Server"
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents txtOutput As TextBox
    Friend WithEvents lblOutput As Label
    Friend WithEvents lstHeaders As ListBox
    Friend WithEvents lstValues As ListBox
    Friend WithEvents btnExit As Button
    Friend WithEvents lblCntReads As Label
    Friend WithEvents btnPipeStop As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents lblConnectStatus As Label
    Friend WithEvents btnJSON As Button
    Friend WithEvents ttSnapShot As Timer
    Friend WithEvents txtJSON As TextBox
    Friend WithEvents clstCOMports As CheckedListBox
    Friend WithEvents TimerSerialUpdate As Timer
    Friend WithEvents ToolTipPortFilters As ToolTip


End Class
