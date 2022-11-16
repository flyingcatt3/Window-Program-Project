Imports SixLabors.ImageSharp
Imports System.Drawing.Text
Imports System.IO
Imports LibVLCSharp.Shared

Public Class Form1

    ReadOnly supportedImgFormat As New List(Of String) From {".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tiff"}
    ReadOnly gamePath = Application.StartupPath
    ReadOnly storyPath = gamePath + "\story\"
    ReadOnly converted_imgPath = gamePath + "\converted_imgs\"
    ReadOnly StoryListBG = gamePath + "StoryListBG.jpg"
    ReadOnly Clicksound As String = gamePath + "click.mp3"
    ReadOnly libvlc_repeat = New LibVLC("--role=music", "--input-repeat=65535", "--aout=mmedevice", "--mmdevice-backend=wasapi")
    ReadOnly libvlc = New LibVLC("--mmdevice-backend=wasapi")
    Dim startBGM As New List(Of String)()
    Dim StoryListTitle As New Label()
    Dim tmpWindowSize, resizing
    Dim pfc As New PrivateFontCollection()

    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        ' by making Generator static, we preserve the same instance '
        ' (i.e., do not create new instances with the same seed over and over) '
        ' between calls '
        Static Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function

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

    Public Sub imgConverter(img As String)
        ImageExtensions.SaveAsJpeg(Image.Load(img), converted_imgPath + Path.GetFileNameWithoutExtension(img) + ".jpg")
    End Sub

    Private Sub fullscreen(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If Me.WindowState = FormWindowState.Normal And e.KeyCode = Keys.F11 Then
            FormBorderStyle = FormBorderStyle.None
            Me.WindowState = FormWindowState.Maximized
        ElseIf e.KeyCode = Keys.Escape Or e.KeyCode = Keys.F11 Then
            FormBorderStyle = FormBorderStyle.Sizable
            Me.WindowState = FormWindowState.Normal
            Me.Size = tmpWindowSize
        End If

    End Sub

    Sub setclr(control, r, g, b, op, interval)
        If op = 1 Then
            control.ForeColor = Drawing.Color.FromArgb(r, g, b)
        ElseIf op = 0 Then
            control.BackColor = Drawing.Color.FromArgb(r, g, b)
        Else
            control.FlatAppearance.BorderColor = Drawing.Color.FromArgb(r, g, b)
        End If
        If interval <> 0 Then
            timeDelay(interval)
        End If
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
                    If Me.WindowState <> FormWindowState.Minimized And Not resizing Then
                        StoryListTitle.Text = StoryListTitle.Text.Remove(4)
                        timeDelay(0.1)
                        StoryListTitle.Text &= txt(4)
                        timeDelay(0.1)
                    ElseIf Me.WindowState = FormWindowState.Minimized Then
                        StoryListTitle.Hide()
                    End If
                Loop

            End Sub)
    End Sub

    Function hideStart()
        If Me.WindowState = FormWindowState.Minimized Then
            Start.Hide()
            Return 1
        ElseIf Not Start.Enabled Then
            Return 1
        Else Return 0
        End If
    End Function

    Private Sub Form1_Load() Handles MyBase.Load

        'https://stackoverflow.com/questions/25872849/to-reduce-flicker-by-double-buffer-setstyle-vs-overriding-createparam
        'DoubleBuffered = True

        For Each foundFile As String In My.Computer.FileSystem.GetFiles(gamePath + "startBGM")
            startBGM.Add(foundFile)
        Next

        Dim file As String = startBGM(GetRandom(0, startBGM.Capacity))
        Dim media = New Media(libvlc_repeat, file)
        Dim mediaPlayer = New MediaPlayer(media)

        mediaPlayer.EnableHardwareDecoding = True
        mediaPlayer.Volume = 70
        mediaPlayer.Play()

        Me.Width = Screen.PrimaryScreen.Bounds.Width * 0.98
        Me.Height = Screen.PrimaryScreen.Bounds.Width * 0.42
        tmpWindowSize = Me.Size

        '初始化所需控制項的格式
        StartLayout.Location = New Drawing.Point(Convert.ToInt32(Me.ClientSize.Width / 2 - Me.StartLayout.Width / 2),
                                       Convert.ToInt32(Me.ClientSize.Height / 2 - Me.StartLayout.Height / 2))

        ver.Text = "20221028"
        ver.BackColor = Drawing.Color.FromArgb(100, 0, 0, 0)

        pfc.AddFontFile(gamePath + "TaipeiSansTCBeta-Regular.ttf")

        Start.Font = New Font(pfc.Families(0), 36, FontStyle.Bold)
        GameTitle.Font = New Font(pfc.Families(0), 54, FontStyle.Bold)

        StoryListTitle.Hide()
        Me.Controls.Add(StoryListTitle)

        Me.CenterToScreen()

    End Sub

    Private Async Sub fadeStartLayout() Handles Start.Click
        Dim startBorderR As Integer = Start.FlatAppearance.BorderColor.R
        Dim startBorderG As Integer = Start.FlatAppearance.BorderColor.G
        Dim startBorderB As Integer = Start.FlatAppearance.BorderColor.B
        Dim meR As Integer = Me.BackColor.R
        Dim meG As Integer = Me.BackColor.G
        Dim meB As Integer = Me.BackColor.B
        Start.Enabled = False
        Start.Text = ""
        Await Task.Run(
            Sub()
                While startBorderR Or startBorderG Or startBorderB Or meR Or meG Or meB
                    If startBorderR Then startBorderR -= 1
                    If startBorderG Then startBorderG -= 1
                    If startBorderB Then startBorderB -= 1
                    If meR Then meR -= 1
                    If meG Then meG -= 1
                    If meB Then meB -= 1
                    setclr(Start, startBorderR, startBorderG, startBorderB, 2, 0)
                    setclr(Me, meR, meG, meB, 0, 0.001)
                    'ver.Text = startR.ToString() + " / " + startG.ToString() + " / " + startB.ToString()
                End While
            End Sub
        )
        ChooseStory()
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

        Dim startR As Integer = Start.ForeColor.R
        Dim startG As Integer = Start.ForeColor.G
        Dim startB As Integer = Start.ForeColor.B

        Await Task.Run(
            Sub()
                Do While 1

                    For x = startG To startR Mod 255
                        startG += 1
                        setclr(Start, startR, startG, startB, 1, 0.01)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startB Mod 255 + 1 To startR
                        startR -= 1
                        setclr(Start, startR, startG, startB, 1, 0.01)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startB To startG Mod 255
                        startB += 1
                        setclr(Start, startR, startG, startB, 1, 0.01)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startR Mod 255 + 1 To startG
                        startG -= 1
                        setclr(Start, startR, startG, startB, 1, 0.01)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startG To startB Mod 255
                        startR += 1
                        setclr(Start, startR, startG, startB, 1, 0.01)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startG Mod 255 + 1 To startB
                        startB -= 1
                        setclr(Start, startR, startG, startB, 1, 0.01)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next
                Loop
            End Sub
            )
    End Sub

    Sub ButtonClick()
        Dim media = New Media(libvlc, Clicksound)
        Dim mediaplayer = New MediaPlayer(media)
        mediaplayer.EnableHardwareDecoding = True
        mediaplayer.Play()
    End Sub

    Sub ButtonCursor(sender As Button, e As EventArgs)
        sender.Cursor = Cursors.Hand
    End Sub

    Private Sub ChooseStory()

        StartLayout.Hide()

        'If Not supportedImgFormat.Contains(Path.GetExtension(StoryListBG)) Then
        'imgConverter(StoryListBG)
        'End If
        'img = Drawing.Image.FromFile(converted_imgPath + Path.GetFileNameWithoutExtension(StoryListBG) + ".jpg")

        Me.Text = "選擇故事 - Visual Novel Engine"

        Me.BackgroundImage = Drawing.Image.FromFile(StoryListBG)
        Me.BackgroundImageLayout = ImageLayout.Zoom

        StoryListTitle.Anchor = Drawing.ContentAlignment.MiddleLeft
        StoryListTitle.TextAlign = Drawing.ContentAlignment.MiddleLeft
        StoryListTitle.Font = New Font(pfc.Families(0), 36, FontStyle.Bold)
        StoryListTitle.ForeColor = Drawing.Color.White
        StoryListTitle.BackColor = Drawing.Color.FromArgb(120, 0, 0, 0)
        StoryListTitle.Width = Me.Size.Width
        StoryListTitle.Height = StoryListTitle.Font.Height * 1.5
        StoryListTitle.Text = ""
        StoryListTitle.Show()
        setStoryListTitle()

        'TODO:增加選擇故事的功能

    End Sub

    '使用者調整視窗大小後格式化畫面上的控制項以符合當前視窗
    Private Sub ControlLayout(sender As Object, e As EventArgs) Handles Me.Resize

        If StoryListTitle.Visible Then
            StoryListTitle.Width = Me.Size.Width
            StoryListTitle.Height = StoryListTitle.Font.Height * 1.5
        End If

        If StartLayout.Visible Then
            StartLayout.Location = New Drawing.Point(Convert.ToInt32(Me.ClientSize.Width / 2 - Me.StartLayout.Width / 2),
                                       Convert.ToInt32(Me.ClientSize.Height / 2 - Me.StartLayout.Height / 2))
        End If

        '視窗從最小化後恢復
        If Me.WindowState <> FormWindowState.Minimized And Not StoryListTitle.Visible And Not StartLayout.Visible Then
            StoryListTitle.Width = Me.Size.Width
            StoryListTitle.Height = StoryListTitle.Font.Height * 1.5
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

    Private Sub resumeScreen() Handles Me.ResizeEnd
        resizing = False
    End Sub


    'Private Sub ver_Click(sender As Object, e As EventArgs) Handles ver.Click

    'End Sub

    'Private Sub NextSentence(sender As Object, e As EventArgs) Handles MyBase.Click

    'End Sub

End Class
