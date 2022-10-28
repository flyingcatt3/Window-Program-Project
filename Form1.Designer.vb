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
        Me.StartLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.StartLayout.SuspendLayout()
        Me.SuspendLayout()
        '
        'Start
        '
        Me.Start.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Start.AutoSize = True
        Me.Start.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.Start.FlatAppearance.BorderSize = 2
        Me.Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Start.Font = New System.Drawing.Font("Taipei Sans TC Beta", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.Start.ForeColor = System.Drawing.Color.MediumSeaGreen
        Me.Start.Location = New System.Drawing.Point(2, 306)
        Me.Start.Margin = New System.Windows.Forms.Padding(2)
        Me.Start.Name = "Start"
        Me.Start.Size = New System.Drawing.Size(687, 83)
        Me.Start.TabIndex = 0
        Me.Start.Text = "開始遊戲"
        Me.Start.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage
        Me.Start.UseVisualStyleBackColor = False
        '
        'GameTitle
        '
        Me.GameTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GameTitle.AutoSize = True
        Me.GameTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GameTitle.Font = New System.Drawing.Font("Taipei Sans TC Beta", 54.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.GameTitle.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.GameTitle.Location = New System.Drawing.Point(2, 64)
        Me.GameTitle.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.GameTitle.Name = "GameTitle"
        Me.GameTitle.Size = New System.Drawing.Size(687, 104)
        Me.GameTitle.TabIndex = 1
        Me.GameTitle.Text = "(遊戲名稱)"
        Me.GameTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.GameTitle.UseMnemonic = False
        '
        'ver
        '
        Me.ver.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ver.AutoSize = True
        Me.ver.BackColor = System.Drawing.Color.Transparent
        Me.ver.Font = New System.Drawing.Font("Consolas", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.ver.ForeColor = System.Drawing.Color.White
        Me.ver.Location = New System.Drawing.Point(1570, 660)
        Me.ver.Margin = New System.Windows.Forms.Padding(0)
        Me.ver.Name = "ver"
        Me.ver.Size = New System.Drawing.Size(98, 23)
        Me.ver.TabIndex = 2
        Me.ver.Text = "20220922"
        Me.ver.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'StartLayout
        '
        Me.StartLayout.BackColor = System.Drawing.Color.Transparent
        Me.StartLayout.ColumnCount = 1
        Me.StartLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.StartLayout.Controls.Add(Me.Start, 0, 1)
        Me.StartLayout.Controls.Add(Me.GameTitle, 0, 0)
        Me.StartLayout.Location = New System.Drawing.Point(297, 101)
        Me.StartLayout.Margin = New System.Windows.Forms.Padding(0)
        Me.StartLayout.Name = "StartLayout"
        Me.StartLayout.RowCount = 2
        Me.StartLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.StartLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.StartLayout.Size = New System.Drawing.Size(691, 464)
        Me.StartLayout.TabIndex = 3
        '
        'Form1
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(1672, 683)
        Me.Controls.Add(Me.StartLayout)
        Me.Controls.Add(Me.ver)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Taipei Sans TC Beta", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MinimumSize = New System.Drawing.Size(1280, 720)
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "開始 - Visual Novel Engine"
        Me.StartLayout.ResumeLayout(False)
        Me.StartLayout.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Start As Button
    Friend WithEvents GameTitle As Label
	Friend WithEvents StartLayout As TableLayoutPanel
	Public WithEvents ver As Label
End Class
