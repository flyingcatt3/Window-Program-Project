﻿Public Class Form1

    Dim Path = Application.StartupPath
    ReadOnly startBGM = Path + "start.mp3"
    ReadOnly StoryListBG = Path + "StoryListBG.jpg"
    ReadOnly Clicksound = Path + "click.mp3"
    Dim StoryListTitle As New Label()

    Public Sub timeDelay(ByVal secondsDelayedBy As Double)
        Dim stopwatch As New Stopwatch
        Dim endtimer = False
        stopwatch.Start()
        Do Until endtimer = True
            If stopwatch.ElapsedMilliseconds > (secondsDelayedBy * 1000) Then
                endtimer = True
                stopwatch.Stop()
            End If
        Loop
    End Sub

    Private Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" _
    (ByVal lpstrCommand As String, ByVal lpstrReturnString As String,
    ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer

    Sub setstartclr(r, g, b)
        Start.ForeColor = Color.FromArgb(r, g, b)
        timeDelay(0.01)
        'ver.Text = r.ToString + "," + g.ToString + "," + b.ToString
    End Sub

    'https://www.it-sideways.com/2014/04/vbnet-form-with-background-image.html
    Protected Overrides ReadOnly Property CreateParams() As Windows.Forms.CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or &H2000000
            Return cp
        End Get
    End Property

    Async Sub setStoryListTitle()
        Dim txt = "選擇故事_"
        Await Task.Run(
            Sub()

                For x = 0 To 4
                    StoryListTitle.Text &= txt(x)
                    timeDelay(0.1)
                Next
                Do While StoryListTitle.Visible
                    If Me.WindowState <> FormWindowState.Minimized Then
                        StoryListTitle.Text = StoryListTitle.Text.Remove(4)
                        timeDelay(0.2)
                        StoryListTitle.Text &= txt(4)
                        timeDelay(0.2)
                    Else
                        StoryListTitle.Hide()
                    End If
                Loop

            End Sub)
    End Sub

    Sub hideStart()
        If Me.WindowState = FormWindowState.Minimized Then
            Start.Hide()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        DoubleBuffered = True

        '初始化所需控制項的格式
        StartLayout.Location = New Point(Convert.ToInt32(Me.ClientSize.Width / 2 - Me.StartLayout.Width / 2),
                                       Convert.ToInt32(Me.ClientSize.Height / 2 - Me.StartLayout.Height / 2))
        ver.Text = "20220925"
        ver.BackColor = Color.FromArgb(100, 0, 0, 0)

        StoryListTitle.Hide()
        Me.Controls.Add(StoryListTitle)

        mciSendString("open """ & startBGM & """ alias startBGM", Nothing, 0, IntPtr.Zero)
        mciSendString("open """ & Clicksound & """ alias ClickSound", Nothing, 0, IntPtr.Zero)
        mciSendString("play startBGM repeat", String.Empty, 0, 0)
        mciSendString("setaudio startBGM volume to 200", 0, 0, 0)

    End Sub

    Private Sub buttonClickHandler() Handles MyBase.Load
        For Each ctrl In Me.StartLayout.Controls
            If (TypeOf ctrl Is Button) Then _
           AddHandler DirectCast(ctrl, Button).Click, AddressOf ButtonClick
        Next

        'AddHandler StoryList.Click, AddressOf ButtonClick

    End Sub

    'https://stackoverflow.com/questions/48710165/back-colour-of-button-not-changing-when-updated-in-for-loop
    Public Async Sub StartColorCycle() Handles MyBase.Load

        Dim r As Integer = Start.ForeColor.R
        Dim g As Integer = Start.ForeColor.G
        Dim b As Integer = Start.ForeColor.B

        Await Task.Run(
            Sub()
                Do While StartLayout.Visible And Start.Visible

                    For x = g To r Mod 255
                        g += 1
                        setstartclr(r, g, b)
                        hideStart()
                    Next

                    For x = b Mod 255 + 1 To r
                        r -= 1
                        setstartclr(r, g, b)
                        hideStart()
                    Next

                    For x = b To g Mod 255
                        b += 1
                        setstartclr(r, g, b)
                        hideStart()
                    Next

                    For x = r Mod 255 + 1 To g
                        g -= 1
                        setstartclr(r, g, b)
                        hideStart()
                    Next

                    For x = g To b Mod 255
                        r += 1
                        setstartclr(r, g, b)
                        hideStart()
                    Next

                    For x = g Mod 255 + 1 To b
                        b -= 1
                        setstartclr(r, g, b)
                        hideStart()
                    Next

                Loop

            End Sub)

    End Sub

    Sub ButtonClick()

        mciSendString("play ClickSound", String.Empty, 0, 0)
        timeDelay(0.25)
        mciSendString("seek ClickSound to start", String.Empty, 0, 0)
        'MsgBox("hello")

    End Sub
    Private Sub ChooseStory() Handles Start.Click

        StartLayout.Hide()

        Me.BackgroundImage = Image.FromFile(StoryListBG)
        Me.BackgroundImageLayout = ImageLayout.Stretch

        StoryListTitle.Location = Me.Size / 20
        StoryListTitle.Anchor = Drawing.ContentAlignment.TopLeft
        StoryListTitle.Font = New Font("Taipei Sans TC Beta", 36, FontStyle.Bold)
        StoryListTitle.ForeColor = Color.White
        StoryListTitle.BackColor = Color.FromArgb(120, 0, 0, 0)
        StoryListTitle.Width = Me.Size.Width * 0.9
        StoryListTitle.Height = Me.Size.Height * 0.9
        StoryListTitle.Text = ""
        StoryListTitle.Show()
        setStoryListTitle()

        'TODO:增加選擇故事的功能

    End Sub

    '使用者調整視窗大小後格式化畫面上的控制項以符合當前視窗
    Private Sub ControlLayout(sender As Object, e As EventArgs) Handles Me.Resize

        If StoryListTitle.Visible Then
            StoryListTitle.Location = Me.Size / 20
            StoryListTitle.Width = Me.Size.Width * 0.9
            StoryListTitle.Height = Me.Size.Height * 0.9
        End If

        If StartLayout.Visible Then
            StartLayout.Location = New Point(Convert.ToInt32(Me.ClientSize.Width / 2 - Me.StartLayout.Width / 2),
                                       Convert.ToInt32(Me.ClientSize.Height / 2 - Me.StartLayout.Height / 2))
        End If

        If Me.WindowState <> FormWindowState.Minimized And Not StoryListTitle.Visible And Not StartLayout.Visible Then
            StoryListTitle.Location = Me.Size / 20
            StoryListTitle.Width = Me.Size.Width * 0.9
            StoryListTitle.Height = Me.Size.Height * 0.9
            StoryListTitle.Text = ""
            StoryListTitle.Show()
            setStoryListTitle()
        End If

        If Me.WindowState <> FormWindowState.Minimized And Not Start.Visible Then
            Start.Show()
            StartColorCycle()
        End If
    End Sub

    Private Sub ver_Click(sender As Object, e As EventArgs) Handles ver.Click

    End Sub

    'Private Sub NextSentence(sender As Object, e As EventArgs) Handles MyBase.Click

    'End Sub

End Class
