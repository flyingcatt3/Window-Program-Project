Public Class Form1

    Dim Path = Application.StartupPath
    ReadOnly startBGM = Path + "start.mp3"
    ReadOnly StoryListBG = Path + "StoryListBG.jpg"
    ReadOnly Clicksound = Path + "click.mp3"
    Dim StoryListTitle As New Label()
    Dim tmpWindowSize, resizing

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

    Private Sub fullscreen(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If Me.WindowState = FormWindowState.Normal And e.KeyCode = Keys.F11 Then
            FormBorderStyle = FormBorderStyle.None
            Me.WindowState = FormWindowState.Maximized
        ElseIf e.KeyCode = Keys.Escape Or e.KeyCode = Keys.F11 Then
            FormBorderStyle = FormBorderStyle.Sizable
            Me.WindowState = FormWindowState.Normal
            'Me.Size = New Size(1600, 900)
            Me.Size = tmpWindowSize
        End If

    End Sub

    Sub setstartclr(r, g, b)
        Start.ForeColor = Color.FromArgb(r, g, b)
        timeDelay(0.0167) '60 fps
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
                Next

                Do While StoryListTitle.Visible
                    'can use timeDelay()
                    If Me.WindowState <> FormWindowState.Minimized And Not resizing Then
                        StoryListTitle.Text = StoryListTitle.Text.Remove(4)
                        StoryListTitle.Text &= txt(4)
                    ElseIf Me.WindowState = FormWindowState.Minimized Then
                        StoryListTitle.Hide()
                        'Else timeDelay(0.25)
                    End If
                Loop

            End Sub)
    End Sub

    Function hideStart()
        If Me.WindowState = FormWindowState.Minimized Then
            Start.Hide()
            Return 1
        ElseIf Not StartLayout.Visible Then
            Return 1
        Else Return 0
        End If
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'https://stackoverflow.com/questions/25872849/to-reduce-flicker-by-double-buffer-setstyle-vs-overriding-createparam
        DoubleBuffered = True

        Me.KeyPreview = True

        '初始化所需控制項的格式
        StartLayout.Location = New Point(Convert.ToInt32(Me.ClientSize.Width / 2 - Me.StartLayout.Width / 2),
                                       Convert.ToInt32(Me.ClientSize.Height / 2 - Me.StartLayout.Height / 2))
        ver.Text = "20220929"
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
            If (TypeOf ctrl Is Button) Then
                AddHandler DirectCast(ctrl, Button).Click, AddressOf ButtonClick
                AddHandler DirectCast(ctrl, Button).GotFocus, AddressOf ButtonCursor
            End If
        Next
        '不是只有開始遊戲的畫面有按鈕而已 後續選擇故事的地方也要再另外寫
        'AddHandler StoryList.Click, AddressOf ButtonClick
    End Sub

    'https://stackoverflow.com/questions/48710165/back-colour-of-button-not-changing-when-updated-in-for-loop
    Public Async Sub StartColorCycle() Handles MyBase.Load

        Dim r As Integer = Start.ForeColor.R
        Dim g As Integer = Start.ForeColor.G
        Dim b As Integer = Start.ForeColor.B

        Await Task.Run(
            Sub()
                Do While 1

                    For x = g To r Mod 255
                        g += 1
                        setstartclr(r, g, b)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = b Mod 255 + 1 To r
                        r -= 1
                        setstartclr(r, g, b)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = b To g Mod 255
                        b += 1
                        setstartclr(r, g, b)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = r Mod 255 + 1 To g
                        g -= 1
                        setstartclr(r, g, b)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = g To b Mod 255
                        r += 1
                        setstartclr(r, g, b)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = g Mod 255 + 1 To b
                        b -= 1
                        setstartclr(r, g, b)
                        If hideStart() Then
                            Exit Do
                        End If
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

    Sub ButtonCursor(sender As Button, e As EventArgs)
        sender.Cursor = Cursors.Hand
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

        '視窗從最小化後恢復
        If Me.WindowState <> FormWindowState.Minimized And Not StoryListTitle.Visible And Not StartLayout.Visible Then
            StoryListTitle.Location = Me.Size / 20
            StoryListTitle.Width = Me.Size.Width * 0.9
            StoryListTitle.Height = Me.Size.Height * 0.9
            StoryListTitle.Text = ""
            StoryListTitle.Show()
            setStoryListTitle()
        End If

        If Me.WindowState <> FormWindowState.Minimized And Not Start.Visible And Not StoryListTitle.Visible Then
            Start.Show()
            StartColorCycle()
        End If

        If Me.WindowState = FormWindowState.Normal Then
            tmpWindowSize = Me.Size
        End If

    End Sub

    Private Sub suspendScreen() Handles Me.ResizeBegin
        resizing = True
    End Sub

    Private Sub ver_Click(sender As Object, e As EventArgs) Handles ver.Click

    End Sub

    Private Sub resumeScreen() Handles Me.ResizeEnd
        resizing = False
    End Sub

    'Private Sub ver_Click(sender As Object, e As EventArgs) Handles ver.Click

    'End Sub

    'Private Sub NextSentence(sender As Object, e As EventArgs) Handles MyBase.Click

    'End Sub

End Class
