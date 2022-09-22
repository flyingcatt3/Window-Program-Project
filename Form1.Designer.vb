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
        Me.Start = New System.Windows.Forms.Button()
        Me.GameTitle = New System.Windows.Forms.Label()
        Me.ver = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Start
        '
        Me.Start.AutoSize = True
        Me.Start.FlatAppearance.BorderColor = System.Drawing.Color.Orange
        Me.Start.FlatAppearance.BorderSize = 2
        Me.Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Start.Font = New System.Drawing.Font("Taipei Sans TC Beta", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.Start.ForeColor = System.Drawing.Color.MediumSeaGreen
        Me.Start.Location = New System.Drawing.Point(550, 650)
        Me.Start.Name = "Start"
        Me.Start.Size = New System.Drawing.Size(500, 100)
        Me.Start.TabIndex = 0
        Me.Start.Text = "開始遊戲"
        Me.Start.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
        Me.Start.UseVisualStyleBackColor = True
        '
        'GameTitle
        '
        Me.GameTitle.AutoSize = True
        Me.GameTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GameTitle.Font = New System.Drawing.Font("Taipei Sans TC Beta", 54.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.GameTitle.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.GameTitle.Location = New System.Drawing.Point(568, 148)
        Me.GameTitle.Name = "GameTitle"
        Me.GameTitle.Size = New System.Drawing.Size(464, 104)
        Me.GameTitle.TabIndex = 1
        Me.GameTitle.Text = "(遊戲名稱)"
        Me.GameTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.GameTitle.UseMnemonic = False
        '
        'ver
        '
        Me.ver.AutoSize = True
        Me.ver.Location = New System.Drawing.Point(1496, 833)
        Me.ver.Name = "ver"
        Me.ver.Size = New System.Drawing.Size(74, 23)
        Me.ver.TabIndex = 2
        Me.ver.Text = "Label1"
        '
        'Form1
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1582, 853)
        Me.Controls.Add(Me.ver)
        Me.Controls.Add(Me.GameTitle)
        Me.Controls.Add(Me.Start)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "Project"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Start As Button
    Friend WithEvents GameTitle As Label
    Friend WithEvents ver As Label
End Class
