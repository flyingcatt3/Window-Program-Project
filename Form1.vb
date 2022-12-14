'=================================================================

'                           GitHub

'   https://github.com/flyingcatt3/Window-Program-Project

'   這個專案使用 .NET 7.0 和 VS Studio 2022 Preview 進行開發

'=================================================================



Imports SixLabors.ImageSharp
Imports System.Drawing.Text
Imports System.IO
Imports LibVLCSharp.Shared
Imports System.Threading
Imports System.IO.Compression
Imports System.Reflection
Imports System.Xml
Imports System.Text
Imports System.Text.RegularExpressions

Public Class Form1

    Public Class StoryTableLayoutPanel

        Inherits TableLayoutPanel

        Public Sub New()
            DoubleBuffered = True
            SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            BackColor = Drawing.Color.FromArgb(100, 0, 0, 0)
            ForeColor = Drawing.Color.White
            Margin = New Padding(0, 0, 0, 0)
            Padding = New Padding(0, 30, 0, 30)
            RowCount = 2
            ColumnCount = 1
            Anchor = AnchorStyles.None
        End Sub

    End Class

    Public Class fadePanel

        Inherits Panel

        Public Sub New()
            DoubleBuffered = True
            SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            Margin = New Padding(0, 0, 0, 0)
            Padding = New Padding(0, 0, 0, 0)
            BackColor = Drawing.Color.Transparent
            Dock = DockStyle.Fill
        End Sub

    End Class

    Public Class switchStoryBtn
        Inherits Button

        Public Sub New()
            DoubleBuffered = True
            BackColor = Drawing.Color.FromArgb(100, 0, 0, 0)
            ForeColor = Drawing.Color.White
            Font = New Drawing.Font("Taipei Sans TC Beta", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            FlatStyle = FlatStyle.Flat
            FlatAppearance.BorderSize = 0
            FlatAppearance.MouseDownBackColor = Drawing.Color.FromArgb(50, 255, 255, 255)
            FlatAppearance.MouseOverBackColor = Drawing.Color.FromArgb(100, 255, 255, 255)
        End Sub
    End Class

    Public Class dbBtn
        Inherits Button

        Public Sub New()
            DoubleBuffered = True
        End Sub
    End Class

    Public Class dbLabel
        Inherits Label

        Public Sub New()
            DoubleBuffered = True
        End Sub
    End Class

    Public Class dbPictureBox
        Inherits PictureBox

        Public Sub New()
            DoubleBuffered = True
            SizeMode = PictureBoxSizeMode.Zoom
            Visible = False
            BackColor = Drawing.Color.Transparent
        End Sub
    End Class

    ReadOnly supportedImgFormat As New List(Of String) From {".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tiff"}
    ReadOnly supportedCmd As New List(Of String) From {"background", "music", "title_fullscreen", "txt", "sound", "character", "fade_white", "fade_black", "flash", "description", "stopmusic", "hide_left", "hide_right", "hide_center", "video", "exit", "location"}
    ReadOnly gamePath = Application.StartupPath
    ReadOnly storyPath = gamePath + "story\"
    ReadOnly converted_imgPath = gamePath + "converted_imgs\"
    ReadOnly StoryListBG = gamePath + "StoryListBG.jpg"
    ReadOnly btnClickSoundPath As String = gamePath + "click.mp3"
    ReadOnly addStorySound As String = gamePath + "addstory.wav"
    ReadOnly libvlc_repeat = New LibVLC("--role=music", "--input-repeat=65535", "--aout=mmedevice", "--mmdevice-backend=wasapi")
    ReadOnly libvlc = New LibVLC("--role=music", "--aout=mmedevice", "--mmdevice-backend=wasapi")
    Dim startBGM As New List(Of String)
    Dim fadeScreen As New fadePanel
    Dim storyTableList As New List(Of TableLayoutPanel)
    Dim chapterTableList As New List(Of TableLayoutPanel)
    Dim storyTableCurrentIndex As Integer = 0
    Dim StoryListTitle As New dbLabel
    Dim txt_StoryLsTitle
    Dim tmpWindowSize, resizing
    Dim pfc As New PrivateFontCollection()
    Dim isStoryListEmpty As Boolean = False
    Dim isLoadingStory As Boolean
    Dim isDisplayingStory As Boolean = False
    Dim isDisplayingEffect As Boolean = False
    Dim stopEffect As Boolean = False
    Dim isFading As Boolean = False
    Dim btnSwitchStoryL As New switchStoryBtn
    Dim btnSwitchStoryR As New switchStoryBtn
    Dim tmpStoryLs As New List(Of String)
    Dim storyLs As New List(Of String)
    Dim scriptLs As New List(Of String)
    Dim cmdQueue As New Queue(Of Func(Of Object))
    Dim skip As Integer = 0
    Dim deQCount As Integer = 0
    Dim mpls As New List(Of MediaPlayer)
    Dim sound As Media
    Dim title_fullscreen As New dbLabel
    Dim txt As New dbLabel
    Dim lct As New dbLabel
    Dim description As New dbLabel
    Dim p1 As New dbPictureBox
    Dim p2 As New dbPictureBox
    Dim p3 As New dbPictureBox
    Dim tmpWp1 As Integer
    Dim tmpWp2 As Integer
    Dim tmpWp3 As Integer
    Dim tmpHp1 As Integer
    Dim tmpHp2 As Integer
    Dim tmpHp3 As Integer
    Dim tmpQ_left As New Queue(Of Func(Of Object))
    Dim tmpQ_center As New Queue(Of Func(Of Object))
    Dim tmpQ_right As New Queue(Of Func(Of Object))

    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        ' by making Generator static, we preserve the same instance '
        ' (i.e., do not create new instances with the same seed over and over) '
        ' between calls '
        Static Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function

    Public Function hideStart()
        If Me.WindowState = FormWindowState.Minimized Then
            Start.Hide()
            Return 1
        ElseIf Not Start.Enabled Then
            Return 1
        Else Return 0
        End If
    End Function

    Public Function imgConverter(img As String)
        Dim p = converted_imgPath + Path.GetFileNameWithoutExtension(img) + ".jpg"
        ImageExtensions.SaveAsJpeg(Image.Load(img), p)
        Return p
    End Function

    Public Function center(ctrl As Control)
        Return New Drawing.Point(Convert.ToInt32(Me.ClientSize.Width / 2 - ctrl.Width / 2),
                                       Convert.ToInt32(Me.ClientSize.Height / 2 - ctrl.Height / 2))
    End Function

    Private Sub fullscreen(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If Me.WindowState = FormWindowState.Normal And e.KeyCode = Keys.F11 Then
            FormBorderStyle = FormBorderStyle.None
            Me.WindowState = FormWindowState.Maximized
        ElseIf e.KeyCode = Keys.Escape Or e.KeyCode = Keys.F11 Then
            FormBorderStyle = FormBorderStyle.Sizable
            Me.WindowState = FormWindowState.Normal
            Me.Size = tmpWindowSize
        End If

        If isDisplayingStory And e.KeyCode = Keys.Q Then
            cmdQueue(cmdQueue.Count - 1)()
            cmdQueue.Clear()
        End If
    End Sub

    Sub setclr(control, a, r, g, b, op, interval)
        If op = 1 Then
            control.ForeColor = Drawing.Color.FromArgb(a, r, g, b)
        ElseIf op = 0 Then
            control.BackColor = Drawing.Color.FromArgb(a, r, g, b)
        Else
            control.FlatAppearance.BorderColor = Drawing.Color.FromArgb(a, r, g, b)
        End If
        If interval Then
            Thread.Sleep(interval)
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

    Sub setStoryListTitle()
        txt_StoryLsTitle = "選擇視覺小說"
        StoryListTitle.Text = ""

        Task.Run(
            Sub()

                For x = 0 To txt_StoryLsTitle.Length - 1
                    If isStoryListEmpty Then
                        Exit For
                    End If
                    StoryListTitle.Text &= txt_StoryLsTitle(x)
                    Thread.Sleep(50)
                Next

                Do While StoryListTitle.Visible
                    If Me.WindowState <> FormWindowState.Minimized And Not resizing Then
                        If isLoadingStory Then
                            StoryListTitle.Text = "Loading..."
                        ElseIf isStoryListEmpty Then
                            StoryListTitle.Text = "找不到檔案。"
                            Exit Do
                        Else
                            StoryListTitle.Text = txt_StoryLsTitle
                            Thread.Sleep(100)
                            StoryListTitle.Text += "_"
                        End If
                    ElseIf Me.WindowState = FormWindowState.Minimized Then
                        StoryListTitle.Hide()
                    End If
                    Thread.Sleep(100)
                Loop
            End Sub)
    End Sub

    Private Sub GameLoad() Handles MyBase.Load

        'https://stackoverflow.com/questions/25872849/to-reduce-flicker-by-double-buffer-setstyle-vs-overriding-createparam
        DoubleBuffered = True
        SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        SetStyle(ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

        '開場音樂
        For Each foundFile As String In My.Computer.FileSystem.GetFiles(gamePath + "startBGM")
            startBGM.Add(foundFile)
        Next

        '設定3個媒體播放器
        Dim media = New Media(libvlc_repeat, startBGM(GetRandom(0, startBGM.Count)))
        sound = New Media(libvlc, btnClickSoundPath)
        Dim mp = New MediaPlayer(media)
        Dim mpCS = New MediaPlayer(sound)
        Dim mpSS = New MediaPlayer(sound)

        mp.EnableHardwareDecoding = True
        mpCS.EnableHardwareDecoding = True
        mpSS.EnableHardwareDecoding = True

        mp.FileCaching = 10
        mpSS.FileCaching = 10

        mp.Volume = 70
        mp.Play()

        mpls.Add(mp)
        mpls.Add(mpCS)
        mpls.Add(mpSS)

        '格式設定
        Me.ClientSize = New Drawing.Size(Screen.PrimaryScreen.Bounds.Width * 0.98, Screen.PrimaryScreen.Bounds.Width * 0.42)
        tmpWindowSize = Me.Size

        Me.StartLayout.Width = Me.ClientSize.Width / 2
        Me.StartLayout.Height = Me.ClientSize.Height / 3 * 2
        Me.ver.Text = "v" + System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyyMMdd")

        pfc.AddFontFile(gamePath + "TaipeiSansTCBeta-Regular.ttf")
        pfc.AddFontFile(gamePath + "ChenYuluoyan-Thin.ttf")
        Start.Font = New Font(pfc.Families(0), 36, FontStyle.Bold)
        GameTitle.Font = New Font(pfc.Families(1), 54, FontStyle.Bold)
        GameTitle.Text = "視覺化小說遊戲引擎"

        'Me.StartLayout.Show()
        'Me.BackgroundImage = Bitmap.FromFile(StoryListBG, Drawing.Imaging.PixelFormat.Format32bppPArgb)
        'Me.BackgroundImage = Drawing.Image.FromFile(StoryListBG)

        AddHandler Start.Click, AddressOf ButtonClick
        AddHandler Start.MouseHover, AddressOf ButtonCursor
        AddHandler title_fullscreen.Click, AddressOf displayStory
        AddHandler txt.Click, AddressOf displayStory
        AddHandler p1.Click, AddressOf displayStory
        AddHandler p2.Click, AddressOf displayStory
        AddHandler p3.Click, AddressOf displayStory

        Me.CenterToScreen()

        Me.Controls.Add(fadeScreen)
        fadeScreen.Show()
        Task.Run(
            Sub()
                Dim fadeColor = 255

                fadeScreen.BringToFront()
                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)

                While CBool(fadeColor)
                    fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                    fadeColor -= 1
                    Thread.Sleep(1)
                End While

                fadeScreen.Hide()
            End Sub)

        StartColorCycle()
        ChooseStory_PreLoad()

    End Sub

    Private Async Sub fadeStartLayout() Handles Start.Click
        Start.Enabled = False
        fadeScreen.Show()

        Await Task.Run(
            Sub()
                Dim fadeColor = fadeScreen.BackColor.A
                'StartLayout.BringToFront()
                fadeScreen.BringToFront()
                fadeScreen.BackColor = Drawing.Color.FromArgb(0, 0, 0, 0)
                'Me.Refresh()
                'fadeTitleScreen.Refresh()
                While CBool(255 - fadeColor)
                    fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                    fadeColor += 1
                    Thread.Sleep(1)
                End While
            End Sub)

        ver.Dispose()
        StartLayout.Dispose()
        fadeScreen.Hide()
        ChooseStory()
    End Sub

    'https://stackoverflow.com/questions/48710165/back-colour-of-button-not-changing-when-updated-in-for-loop
    Public Async Sub StartColorCycle()

        Dim startA As Integer = Start.ForeColor.A
        Dim startR As Integer = Start.ForeColor.R
        Dim startG As Integer = Start.ForeColor.G
        Dim startB As Integer = Start.ForeColor.B

        Await Task.Run(
            Sub()
                Do While 1

                    For x = startG To startR Mod 255
                        startG += 1
                        setclr(Start, startA, startR, startG, startB, 1, 10)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startB Mod 255 + 1 To startR
                        startR -= 1
                        setclr(Start, startA, startR, startG, startB, 1, 10)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startB To startG Mod 255
                        startB += 1
                        setclr(Start, startA, startR, startG, startB, 1, 10)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startR Mod 255 + 1 To startG
                        startG -= 1
                        setclr(Start, startA, startR, startG, startB, 1, 10)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startG To startB Mod 255
                        startR += 1
                        setclr(Start, startA, startR, startG, startB, 1, 10)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next

                    For x = startG Mod 255 + 1 To startB
                        startB -= 1
                        setclr(Start, startA, startR, startG, startB, 1, 10)
                        If hideStart() Then
                            Exit Do
                        End If
                    Next
                Loop
            End Sub
            )
    End Sub

    Sub ButtonClick()
        mpls(1).Media = sound
        mpls(1).Play()
        sound = New Media(libvlc, btnClickSoundPath)
    End Sub

    Sub ButtonCursor(sender As Button, e As EventArgs)
        sender.Cursor = Cursors.Hand
    End Sub

    Sub loadStory()

        Dim scope

        If tmpStoryLs.Count Then
            scope = tmpStoryLs
        Else
            scope = My.Computer.FileSystem.GetDirectories(storyPath)
        End If

        storyLs.AddRange(scope)

        For Each foundDir As String In scope
            Dim dirInfo As New System.IO.DirectoryInfo(foundDir)
            Dim storyTable As New StoryTableLayoutPanel
            Dim storyName As New Label
            Dim storyBtn As New dbBtn
            Dim chapterTable As New StoryTableLayoutPanel

            storyTable.Size = New Drawing.Size(Me.ClientSize.Width * 0.8, Me.ClientSize.Width / 10 * 3)
            storyTable.Location = center(storyTable)
            storyTable.Visible = False

            chapterTable.Size = New Drawing.Size(Me.ClientSize.Width * 0.8, Me.ClientSize.Width / 10 * 3)
            chapterTable.Location = center(chapterTable)
            chapterTable.AutoScroll = True
            chapterTable.Visible = False

            storyName.Font = New Font(pfc.Families(0), 20, FontStyle.Bold)
            storyName.Text = dirInfo.Name
            'MsgBox(storyName.Text)
            storyName.ForeColor = Drawing.Color.White
            storyName.BackColor = Drawing.Color.Transparent
            storyName.Size = New Drawing.Size(storyTable.Width, storyTable.Height / 3)
            storyName.TextAlign = Drawing.ContentAlignment.MiddleCenter
            storyName.Anchor = AnchorStyles.Bottom

            storyBtn.Anchor = AnchorStyles.Top
            storyBtn.BackColor = Drawing.Color.Transparent
            storyBtn.Dock = DockStyle.Fill
            storyBtn.FlatStyle = FlatStyle.Flat
            storyBtn.FlatAppearance.BorderSize = 0
            storyBtn.FlatAppearance.MouseOverBackColor = Drawing.Color.FromArgb(25, 0, 0, 0)
            storyBtn.FlatAppearance.MouseDownBackColor = Drawing.Color.FromArgb(50, 0, 0, 0)
            storyBtn.Size = New Drawing.Size(storyTable.Width, storyName.Height * 2)
            storyBtn.BackgroundImageLayout = ImageLayout.Zoom

            If File.Exists(foundDir + "\logo.png") Then
                storyBtn.BackgroundImage = Drawing.Image.FromFile(foundDir + "\logo.png")
            Else
                storyBtn.BackgroundImage = Drawing.Image.FromFile(gamePath + "Image_not_available.png")
            End If

            For Each ch In My.Computer.FileSystem.GetDirectories(foundDir)
                Dim chapterBtn As New dbBtn
                storyTable.Name = ""

                If File.Exists(ch + "\script.txt") Then
                    Dim dir2Info As New System.IO.DirectoryInfo(ch)

                    chapterBtn.Anchor = AnchorStyles.Top
                    chapterBtn.BackColor = Drawing.Color.Transparent
                    chapterBtn.FlatStyle = FlatStyle.Flat
                    chapterBtn.FlatAppearance.BorderSize = 0
                    chapterBtn.FlatAppearance.MouseOverBackColor = Drawing.Color.FromArgb(25, 0, 0, 0)
                    chapterBtn.FlatAppearance.MouseDownBackColor = Drawing.Color.FromArgb(50, 0, 0, 0)
                    chapterBtn.Size = New Drawing.Size(storyTable.Width - storyTable.Padding.Right - storyTable.Padding.Left - chapterBtn.Margin.All * 2, storyTable.Height / 8)
                    'chapterBtn.Text = utf8.GetString(big5.GetBytes(dir2Info.Name))
                    chapterBtn.Text = dir2Info.Name
                    chapterBtn.Font = New Font(pfc.Families(0), 20, FontStyle.Bold)
                    chapterBtn.TextAlign = Drawing.ContentAlignment.MiddleCenter

                    AddHandler chapterBtn.Click, AddressOf ButtonClick
                    AddHandler chapterBtn.Click, AddressOf chapterBtnClick
                    AddHandler chapterBtn.MouseHover, AddressOf ButtonCursor

                    chapterTable.Controls.Add(chapterBtn)

                End If
            Next

            AddHandler storyBtn.Click, AddressOf storyBtnClick
            AddHandler storyBtn.Click, AddressOf ButtonClick
            AddHandler storyBtn.MouseHover, AddressOf ButtonCursor

            storyTable.Controls.Add(storyBtn, 0, 0)
            storyTable.Controls.Add(storyName, 0, 1)
            storyTableList.Add(storyTable)
            chapterTableList.Add(chapterTable)
            Me.Controls.Add(storyTable)
            Me.Controls.Add(chapterTable)

            'MsgBox("Finish loading.")
        Next
    End Sub

    Private Sub ChooseStory_PreLoad()

        StoryListTitle.Anchor = Drawing.ContentAlignment.MiddleLeft
        StoryListTitle.TextAlign = Drawing.ContentAlignment.MiddleLeft
        StoryListTitle.Font = New Font(pfc.Families(0), 32, FontStyle.Bold)
        StoryListTitle.ForeColor = Drawing.Color.White
        StoryListTitle.BackColor = Drawing.Color.FromArgb(120, 0, 0, 0)
        StoryListTitle.Width = Me.ClientSize.Width
        StoryListTitle.Height = StoryListTitle.Font.Height * 1.5
        StoryListTitle.Text = ""

        loadStory()

        'If story don't exist in storyPath...
        If Not CBool(storyTableList.Count) Then
            Dim storyNotFound As New Label
            Dim storyTable As New StoryTableLayoutPanel

            isStoryListEmpty = True
            StoryListTitle.Text = "找不到檔案。"

            storyNotFound.Text = "若要新增故事，請將壓縮檔拖曳到這裡。"
            storyNotFound.TextAlign = ContentAlignment.MiddleCenter
            storyNotFound.Font = New Font(pfc.Families(0), 24)
            storyNotFound.ForeColor = Drawing.Color.White
            storyNotFound.BackColor = Drawing.Color.Transparent
            storyNotFound.AutoSize = True
            storyNotFound.Anchor = AnchorStyles.None

            storyTable.Visible = False
            storyTable.Size = New Drawing.Size(Me.ClientSize.Width * 0.8, Me.ClientSize.Height * 0.6)
            storyTable.Location = center(storyTable)
            storyTable.RowCount = 1

            storyTable.Controls.Add(storyNotFound, 0, 0)
            storyTableList.Add(storyTable)
        End If

        title_fullscreen.Anchor = Drawing.ContentAlignment.MiddleLeft
        title_fullscreen.TextAlign = Drawing.ContentAlignment.MiddleLeft
        title_fullscreen.Font = New Font(pfc.Families(0), 32, FontStyle.Bold)
        title_fullscreen.ForeColor = Drawing.Color.White
        title_fullscreen.BackColor = Drawing.Color.Transparent
        title_fullscreen.Hide()

        txt.Anchor = Drawing.ContentAlignment.BottomCenter
        txt.TextAlign = Drawing.ContentAlignment.TopLeft
        txt.Font = New Font(pfc.Families(0), 18, FontStyle.Bold)
        txt.ForeColor = Drawing.Color.FromArgb(47, 62, 105)
        txt.BackColor = Drawing.Color.FromArgb(230, 255, 255, 255)
        txt.Padding = New Padding(20, 20, 20, 20)
        txt.Hide()

        description.TextAlign = Drawing.ContentAlignment.MiddleCenter
        description.Font = New Font(pfc.Families(0), 20, FontStyle.Bold)
        description.ForeColor = Drawing.Color.FromArgb(255, 255, 255)
        description.BackColor = Drawing.Color.FromArgb(200, 136, 127, 153)
        description.Padding = New Padding(20, 20, 20, 20)
        description.Hide()

        lct.TextAlign = Drawing.ContentAlignment.MiddleCenter
        lct.Font = New Font(pfc.Families(0), 20, FontStyle.Bold)
        lct.ForeColor = Drawing.Color.FromArgb(255, 255, 255)
        lct.BackColor = Drawing.Color.FromArgb(66, 68, 99)
        lct.Padding = New Padding(3, 3, 3, 3)
        lct.Hide()

        Me.Controls.Add(title_fullscreen)
        Me.Controls.Add(txt)
        Me.Controls.Add(description)
        Me.Controls.Add(lct)
        Me.Controls.Add(p1)
        Me.Controls.Add(p2)
        Me.Controls.Add(p3)

    End Sub

    Private Sub ChooseStory()

        'If Not supportedImgFormat.Contains(Path.GetExtension(StoryListBG)) Then
        'imgConverter(StoryListBG)
        '= Drawing.Image.FromFile(converted_imgPath + Path.GetFileNameWithoutExtension(StoryListBG) + ".jpg")

        Me.Text = "選擇視覺小說 - Visual Novel Engine"
        Me.BackColor = Drawing.Color.Black
        Me.BackgroundImage = Drawing.Image.FromFile(StoryListBG)
        Me.BackgroundImageLayout = ImageLayout.Zoom
        Me.AllowDrop = True

        storyTableList(storyTableCurrentIndex).Visible = True

        For Each storytable In storyTableList
            Me.Controls.Add(storytable)
        Next

        Me.Controls.Add(StoryListTitle)

        If Not isStoryListEmpty Then
            setStoryListTitle()
        End If

        btnSwitchStoryAdd()

    End Sub

    '使用者調整視窗大小後格式化畫面上的控制項以符合當前視窗
    Private Sub ControlLayout(sender As Object, e As EventArgs) Handles Me.Resize

        If StoryListTitle.Visible Then
            StoryListTitle.Width = Me.ClientSize.Width
            StoryListTitle.Height = StoryListTitle.Font.Height * 1.5
            If Me.WindowState <> FormWindowState.Minimized And storyTableList.Count Then
                btnSwitchStoryL.Location = New Drawing.Point(Me.ClientSize.Width * 0.025, Convert.ToInt32(Me.ClientSize.Height / 2 - btnSwitchStoryL.Height / 2))
                btnSwitchStoryR.Location = New Drawing.Point(Me.ClientSize.Width * 0.925, Convert.ToInt32(Me.ClientSize.Height / 2 - btnSwitchStoryR.Height / 2))
                btnSwitchStoryL.Size = New Drawing.Size(Me.ClientSize.Width * 0.05, Me.ClientSize.Height * 0.2)
                btnSwitchStoryR.Size = New Drawing.Size(Me.ClientSize.Width * 0.05, Me.ClientSize.Height * 0.2)
                For Each table In storyTableList
                    table.Size = New Drawing.Size(Me.ClientSize.Width * 0.8, Me.ClientSize.Width / 10 * 3)
                    If Not isStoryListEmpty Then
                        table.Controls.Item(1).Size = New Drawing.Size(table.Width, table.Height / 3)
                        table.Controls.Item(0).Size = New Drawing.Size(table.Width, table.Controls.Item(1).Height * 2)
                    End If
                    table.Location = center(table)
                Next
                For Each table In chapterTableList
                    table.Size = New Drawing.Size(Me.ClientSize.Width * 0.8, Me.ClientSize.Width / 10 * 3)
                    table.Location = center(table)
                Next
            End If
        End If

        If StartLayout.Visible Then
            StartLayout.Width = Me.ClientSize.Width / 2
            StartLayout.Height = Me.ClientSize.Height / 3 * 2
            StartLayout.Location = center(StartLayout)
        End If

        '視窗從最小化後恢復
        If Me.WindowState <> FormWindowState.Minimized And storyTableList.Count And Not StoryListTitle.Visible And Not isDisplayingStory Then
            StoryListTitle.Width = Me.ClientSize.Width
            StoryListTitle.Height = StoryListTitle.Font.Height * 1.5
            StoryListTitle.Text = ""
            StoryListTitle.Show()
            If Not isStoryListEmpty Then
                setStoryListTitle()
            End If
        End If

        If Me.WindowState = FormWindowState.Minimized Then
            StoryListTitle.Hide()
        End If

        If Me.WindowState <> FormWindowState.Minimized And Not Start.Visible And Not Me.Controls.Contains(StoryListTitle) Then
            Start.Show()
            StartColorCycle()
        End If

        If Me.WindowState = FormWindowState.Normal Then
            tmpWindowSize = Me.Size
        End If

        If isDisplayingStory Then
            title_fullscreen.Width = Me.ClientSize.Width * 0.7
            title_fullscreen.Height = Me.ClientSize.Height * 0.7
            title_fullscreen.Location = center(title_fullscreen)

            txt.Width = Me.ClientSize.Width * 0.8
            txt.Height = Me.ClientSize.Height * 0.28
            txt.Location = New Drawing.Point(Me.ClientSize.Width * 0.1, Me.ClientSize.Height * 0.7)

            description.Height = Me.ClientSize.Height * 0.1
            description.Location = center(description)

            Dim y = Me.ClientSize.Height * 0.15
            Dim h = Me.ClientSize.Height * 0.85

            If p1.Height * 0.6 < h Then
                p1.Width = p1.Width * (1 + (h - p1.Height * 0.6) / h)
                p1.Height = p1.Height * (1 + (h - p1.Height * 0.6) / h)
            Else
                p1.Width = tmpWp1
                p1.Height = tmpHp1
            End If

            p1.Location = New Drawing.Point(Me.ClientSize.Width * 0.5 - p1.Width, y)

            If p2.Height * 0.6 < h Then
                p2.Width = p2.Width * (1 + (h - p2.Height * 0.6) / h)
                p2.Height = p2.Height * (1 + (h - p2.Height * 0.6) / h)
            Else
                p2.Width = tmpWp2
                p2.Height = tmpHp2
            End If

            p2.Location = New Drawing.Point((Me.ClientSize.Width - p2.Width) * 0.5, y)

            If p3.Height * 0.6 < h Then
                p3.Width = p3.Width * (1 + (h - p3.Height * 0.6) / h)
                p3.Height = p3.Height * (1 + (h - p3.Height * 0.6) / h)
            Else
                p3.Width = tmpWp3
                p3.Height = tmpHp3
            End If

            p3.Location = New Drawing.Point(Me.ClientSize.Width * 0.5, y)
        End If
    End Sub

    Private Sub suspendScreen() Handles Me.ResizeBegin
        resizing = True
    End Sub

    Private Sub resumeScreen() Handles Me.ResizeEnd
        resizing = False
    End Sub

    Private Async Sub FileDragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop

        isLoadingStory = True

        If isStoryListEmpty Then
            setStoryListTitle()
        End If

        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim filesList As New List(Of String)
        Dim errFilesMsg As New List(Of String)

        Dim sound = New Media(libvlc, addStorySound)
        mpls(1).Media = sound

        errFilesMsg.Add("處理下列檔案時發生錯誤：" & vbCrLf & vbCrLf)

        Await Task.Run(
            Sub()
                For Each file As String In files
                    'MsgBox(Path.GetFullPath(file))
                    If Path.GetExtension(file) <> ".zip" Then
                        errFilesMsg.Add(Path.GetFullPath(file) & vbCrLf & " - 該檔案的副檔名不是.zip。" & vbCrLf & vbCrLf)
                    Else
                        filesList.Add(Path.GetFullPath(file))
                    End If
                Next

                tmpStoryLs.AddRange(filesList)

                For Each zipPath In filesList
                    'MsgBox(zipPath)
                    'MsgBox(filesList.Count)

                    For Each foundDir As String In My.Computer.FileSystem.GetDirectories(storyPath)
                        Dim dirInfo As New System.IO.DirectoryInfo(foundDir)
                        Dim dirName = dirInfo.Name

                        If Path.GetFileNameWithoutExtension(zipPath) = dirName Then
                            tmpStoryLs.Remove(zipPath)
                            errFilesMsg.Add(zipPath & vbCrLf & " - 遊戲目錄內已經有同名的檔案。" & vbCrLf & vbCrLf)
                        End If
                    Next

                    Dim extractPath = storyPath + Path.GetFileNameWithoutExtension(zipPath)
                    My.Computer.FileSystem.CreateDirectory(extractPath)

                    Try
                        'https://marcus116.blogspot.com/2019/03/netcore-aspnet-core-using-encoding-big5.html
                        Dim big5 As System.Text.Encoding, utf8 As System.Text.Encoding
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
                        big5 = System.Text.Encoding.GetEncoding(950)
                        utf8 = System.Text.Encoding.Default
                        ZipFile.ExtractToDirectory(zipPath, extractPath, big5, True)
                    Catch ex As Exception
                        My.Computer.FileSystem.DeleteDirectory(extractPath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                        Select Case ex.GetType()
                            Case GetType(IOException)
                                tmpStoryLs.Remove(zipPath)
                                errFilesMsg.Add(zipPath & vbCrLf & " - 硬碟空間不足。" & vbCrLf & vbCrLf)
                            Case GetType(InvalidDataException)
                                tmpStoryLs.Remove(zipPath)
                                errFilesMsg.Add(zipPath & vbCrLf & " - 該檔案損毀、被加密或檔案格式與附檔名不相符。" & vbCrLf & vbCrLf)
                        End Select
                        'MsgBox(ex.ToString())
                    End Try

                Next

                If errFilesMsg.Count > 1 Then
                    Dim str As String = Nothing
                    For Each msg In errFilesMsg
                        str &= msg
                    Next
                    MsgBox(str, vbCritical, "Error")
                End If

                For i As Integer = 0 To tmpStoryLs.Count - 1
                    tmpStoryLs(i) = storyPath + Path.GetFileNameWithoutExtension(tmpStoryLs(i))
                Next
            End Sub)

        '當沒有story的時候，若要新增故事，則remove storyTableList(0)後再新增
        If CBool(tmpStoryLs.Count) And isStoryListEmpty Then
            storyTableList(0).Dispose()
            storyTableList.RemoveAt(0)
            loadStory()
            storyTableList(storyTableCurrentIndex).Show()
            isStoryListEmpty = False
            Me.Controls.Add(storyTableList(0))
            mpls(1).Play()
        ElseIf CBool(tmpStoryLs.Count) Then '已經有story
            loadStory()
            mpls(1).Play()
        End If
        btnSwitchStoryAdd()
        tmpStoryLs.Clear()
        isLoadingStory = False
    End Sub

    Private Sub FileDragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragEnter
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub ver_Click(sender As Object, e As EventArgs) Handles ver.Click
        Dim myProcess = New Process()
        myProcess.StartInfo.UseShellExecute = True
        myProcess.StartInfo.FileName = "https://github.com/flyingcatt3/Window-Program-Project"
        myProcess.Start()
    End Sub

    Private Sub btnSwitchStoryAdd()

        If storyTableList.Count > 1 And Not Me.Controls.Contains(btnSwitchStoryL) Then
            btnSwitchStoryL.Size = New Drawing.Size(Me.ClientSize.Width * 0.05, Me.ClientSize.Height * 0.2)
            btnSwitchStoryR.Size = New Drawing.Size(Me.ClientSize.Width * 0.05, Me.ClientSize.Height * 0.2)
            btnSwitchStoryL.Location = New Drawing.Point(Me.ClientSize.Width * 0.025, Convert.ToInt32(Me.ClientSize.Height / 2 - btnSwitchStoryL.Height / 2))
            btnSwitchStoryR.Location = New Drawing.Point(Me.ClientSize.Width * 0.925, Convert.ToInt32(Me.ClientSize.Height / 2 - btnSwitchStoryR.Height / 2))
            btnSwitchStoryL.Text = "<"
            btnSwitchStoryR.Text = ">"

            AddHandler btnSwitchStoryL.Click, AddressOf ButtonClick
            AddHandler btnSwitchStoryL.Click, AddressOf btnSwitchStoryClick
            AddHandler btnSwitchStoryL.MouseHover, AddressOf ButtonCursor
            AddHandler btnSwitchStoryR.Click, AddressOf ButtonClick
            AddHandler btnSwitchStoryR.Click, AddressOf btnSwitchStoryClick
            AddHandler btnSwitchStoryR.MouseHover, AddressOf ButtonCursor

            Me.Controls.Add(btnSwitchStoryL)
            Me.Controls.Add(btnSwitchStoryR)
            btnSwitchStoryL.BringToFront()
            btnSwitchStoryR.BringToFront()
        End If
    End Sub

    Private Sub btnSwitchStoryClick(sender As Button, e As EventArgs)
        storyTableList(storyTableCurrentIndex).Hide()
        'MsgBox(storyTableCurrentIndex.ToString())
        If sender.Text = "<" Then
            If CBool(storyTableCurrentIndex) Then
                storyTableCurrentIndex -= 1
            Else
                storyTableCurrentIndex = storyTableList.Count - 1
            End If
        Else
            If storyTableCurrentIndex = storyTableList.Count - 1 Then
                storyTableCurrentIndex = 0
            Else
                storyTableCurrentIndex += 1
            End If
        End If
        'MsgBox(storyTableCurrentIndex.ToString())
        storyTableList(storyTableCurrentIndex).Show()
    End Sub

    Private Sub storyBtnClick()
        Me.Text = "選擇章節 - Visual Novel Engine"
        If My.Computer.FileSystem.GetDirectories(storyLs(storyTableCurrentIndex)).Count Then
            Me.AllowDrop = False
            txt_StoryLsTitle = "選擇章節"
            storyTableList(storyTableCurrentIndex).Hide()
            btnSwitchStoryL.Hide()
            btnSwitchStoryR.Hide()
            chapterTableList(storyTableCurrentIndex).Show()

        Else
            MsgBox("在該故事的目錄中找不到任何章節(資料夾)。", vbCritical, "Error")
        End If

        For Each ctrl As Control In chapterTableList
            If ctrl.Visible Then
                ctrl.Enabled = True
            End If
        Next

        For Each ctrl As Control In chapterTableList(storyTableCurrentIndex).Controls
            If ctrl.Visible Then
                ctrl.Enabled = True
            End If
        Next
    End Sub

    Private Sub chapterBtnClick(sender As Button, e As EventArgs)
        '閱覽劇情的功能.
        isLoadingStory = True

        For Each ctrl As Control In chapterTableList(storyTableCurrentIndex).Controls
            If ctrl.Visible Then
                ctrl.Enabled = False
            End If
        Next

        Dim script As String() = File.ReadAllLines(storyLs(storyTableCurrentIndex) + "\" + sender.Text + "\script.txt")

        'Check script
        For Each cmd In script
            If Not supportedCmd.Contains(cmd.Split(" ")(0)) Then
                'MsgBox(cmd.Split(" ")(0))
                MsgBox("這個章節內的 script.txt 有錯誤，故無法載入該章節.")
                isLoadingStory = False
                For Each ctrl As Control In chapterTableList(storyTableCurrentIndex).Controls
                    If ctrl.Visible Then
                        ctrl.Enabled = True
                    End If
                Next
                Exit Sub
            End If
            scriptLs.Add(cmd)
        Next

        loadScript(storyLs(storyTableCurrentIndex) + "\" + sender.Text)
    End Sub

    Private Async Sub loadScript(chapterPath As String)
        Await Task.Run(
            Sub()
                For Each cmd As String In scriptLs
                    Dim key = cmd.Split(" ")(0)

                    Select Case key
                        Case "background"
                            Dim imgPath = chapterPath + "\" + cmd.Split(" ")(1)
                            Dim img

                            If Not supportedImgFormat.Contains(Path.GetExtension(imgPath)) Then
                                img = imgConverter(imgPath)
                            Else
                                img = imgPath
                            End If

                            cmdQueue.Enqueue(
                                Function() As Object
                                    Task.Run(
                                        Sub()
                                            Do Until Not isFading
                                                Thread.Sleep(50)
                                            Loop
                                            Me.BackgroundImage = Nothing
                                            Me.BackgroundImage = Drawing.Image.FromFile(img)
                                        End Sub)
                                    Return 0
                                End Function)
                        Case "character"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Dim imgPath = chapterPath + "\" + cmd.Split(" ")(1)
                                    Dim location = cmd.Split(" ")(2)
                                    Dim img = Drawing.Image.FromFile(imgPath)
                                    Dim y = Me.ClientSize.Height * 0.15
                                    Dim h = Me.ClientSize.Height * 0.85
                                    Dim idx = scriptLs.FindIndex(Function(value As String) value = cmd) + 1
                                    'MsgBox(cmd)
                                    Dim i = idx
                                    Dim skipCmdCount As Integer = 0

                                    Select Case location
                                        Case "left"
                                            p2.Hide()
                                            p2.Image = Nothing

                                            p1.ImageLocation = imgPath

                                            p1.Width = img.Width * (1 + (h - img.Height * 0.6) / h)
                                            p1.Height = img.Height * (1 + (h - img.Height * 0.6) / h)

                                            tmpWp1 = img.Width
                                            tmpHp1 = img.Height

                                            p1.Location = New Drawing.Point(Me.ClientSize.Width * 0.5 - p1.Width, y)
                                            p1.Show()

                                            tmpQ_left.Clear()
                                            While scriptLs(i).Split(" ")(0) = "character"
                                                If scriptLs(i).Split(" ")(2) = "left" Then
                                                    skipCmdCount += 1
                                                    tmpQ_left.Enqueue(cmdQueue(skipCmdCount))
                                                    i += 1
                                                Else
                                                    Exit While
                                                End If
                                            End While
                                        Case "center"
                                            p1.Hide()
                                            p3.Hide()
                                            p1.Image = Nothing
                                            p3.Image = Nothing

                                            p2.ImageLocation = imgPath

                                            p2.Width = img.Width * (1 + (h - img.Height * 0.6) / h)
                                            p2.Height = img.Height * (1 + (h - img.Height * 0.6) / h)

                                            tmpWp2 = img.Width
                                            tmpHp2 = img.Height

                                            p2.Location = New Drawing.Point((Me.ClientSize.Width - p2.Width) * 0.5, y)
                                            p2.Show()

                                            tmpQ_center.Clear()
                                            While scriptLs(i).Split(" ")(0) = "character"
                                                If scriptLs(i).Split(" ")(2) = "center" Then
                                                    skipCmdCount += 1
                                                    tmpQ_center.Enqueue(cmdQueue(skipCmdCount))
                                                    i += 1
                                                Else
                                                    Exit While
                                                End If
                                            End While
                                        Case "right"
                                            p2.Hide()
                                            p2.Image = Nothing

                                            p3.ImageLocation = imgPath

                                            p3.Width = img.Width * (1 + (h - img.Height * 0.6) / h)
                                            p3.Height = img.Height * (1 + (h - img.Height * 0.6) / h)

                                            tmpWp3 = img.Width
                                            tmpHp3 = img.Height

                                            p3.Location = New Drawing.Point(Me.ClientSize.Width * 0.5, y)
                                            p3.Show()

                                            tmpQ_right.Clear()
                                            While scriptLs(i).Split(" ")(0) = "character"
                                                If scriptLs(i).Split(" ")(2) = "right" Then
                                                    skipCmdCount += 1
                                                    tmpQ_right.Enqueue(cmdQueue(skipCmdCount))
                                                    i += 1
                                                Else
                                                    Exit While
                                                End If
                                            End While
                                    End Select

                                    Task.Run(
                                        Sub()
                                            While tmpQ_left.Count
                                                Thread.Sleep(2500)
                                                tmpQ_left(0)()
                                                tmpQ_left.Dequeue()
                                            End While
                                            While tmpQ_center.Count
                                                Thread.Sleep(2500)
                                                tmpQ_center(0)()
                                                tmpQ_center.Dequeue()
                                            End While
                                            While tmpQ_right.Count
                                                Thread.Sleep(2500)
                                                tmpQ_right(0)()
                                                tmpQ_right.Dequeue()
                                            End While

                                            If stopEffect Then
                                                stopEffect = False
                                            End If

                                        End Sub)

                                    skip = skipCmdCount
                                    scriptLs.Remove(cmd)
                                    Return 2
                                End Function)
                        Case "title_fullscreen"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Me.BackgroundImage = Nothing
                                    isDisplayingEffect = True

                                    title_fullscreen.Text = ""
                                    title_fullscreen.Width = Me.ClientSize.Width * 0.7
                                    title_fullscreen.Height = Me.ClientSize.Height * 0.7
                                    title_fullscreen.Location = center(title_fullscreen)
                                    title_fullscreen.Show()

                                    Task.Run(
                                        Sub()
                                            Dim t = ""
                                            Dim splcmd = cmd.Split(Chr(34)).Skip(1)

                                            For Each row As String In splcmd.SkipLast(1)
                                                If row = " " Then
                                                    t += vbCrLf
                                                Else
                                                    t += row
                                                End If
                                            Next

                                            t += splcmd.Last()

                                            For i = 0 To t.Length - 1
                                                If stopEffect Then
                                                    'MsgBox("stop")
                                                    title_fullscreen.Text = t
                                                    stopEffect = False
                                                    Exit For
                                                Else
                                                    title_fullscreen.Text += t(i)
                                                    Thread.Sleep(100)
                                                End If
                                            Next
                                            isDisplayingEffect = False
                                        End Sub)
                                    Return 1
                                End Function)
                        Case "txt"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    isDisplayingEffect = True

                                    txt.Text = ""
                                    txt.Width = Me.ClientSize.Width * 0.8
                                    txt.Height = Me.ClientSize.Height * 0.28
                                    txt.Location = New Drawing.Point(Me.ClientSize.Width * 0.1, Me.ClientSize.Height * 0.7)
                                    txt.Show()

                                    Task.Run(
                                        Sub()
                                            Dim splcmd = cmd.Split(Chr(34)).Skip(1)
                                            Dim name = "[ " + splcmd(0) + " ]" + vbCrLf + vbCrLf
                                            Dim t = ""

                                            For Each row As String In splcmd.Skip(2).SkipLast(1)
                                                If row = " " Then
                                                    t += vbCrLf
                                                Else
                                                    t += row
                                                End If
                                            Next

                                            t += splcmd.Last()

                                            txt.Text = name

                                            For i = 0 To t.Length - 1
                                                If stopEffect Then
                                                    'MsgBox("stop")
                                                    txt.Text = name + t
                                                    stopEffect = False
                                                    Exit For
                                                Else
                                                    Do Until Not isFading
                                                        Thread.Sleep(100)
                                                    Loop
                                                    txt.Text += t(i)
                                                    Thread.Sleep(50)
                                                End If
                                            Next
                                            isDisplayingEffect = False
                                        End Sub)

                                    If scriptLs(scriptLs.FindIndex(Function(value As String) value = cmd) + 1).Split(" ")(0) = "sound" Then
                                        Return 1
                                    Else
                                        If mpls(2).IsPlaying Then
                                            mpls(2).Stop()
                                        End If
                                        Return 3
                                    End If
                                End Function)
                        Case "description"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    If mpls(2).IsPlaying Then
                                        mpls(2).Stop()
                                    End If

                                    p1.Hide()
                                    p2.Hide()
                                    p3.Hide()
                                    p1.Image = Nothing
                                    p2.Image = Nothing
                                    p3.Image = Nothing

                                    isDisplayingEffect = True

                                    Dim splcmd = cmd.Split(Chr(34)).Skip(1)
                                    description.Height = Me.ClientSize.Height * 0.1
                                    description.Location = center(description)
                                    description.Text = splcmd(0)
                                    description.Show()

                                    Task.Run(
                                        Sub()
                                            For w = 0 To Me.ClientSize.Width * 0.5 Step 20
                                                description.Width = w
                                                description.Location = center(description)
                                            Next
                                            isDisplayingEffect = False
                                        End Sub)
                                    Return 3
                                End Function)
                        Case "location"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Dim splcmd = cmd.Split(Chr(34)).Skip(1)
                                    lct.Height = 50
                                    lct.Width = Me.ClientSize.Width / 5
                                    lct.Text = splcmd(0)
                                    lct.Show()

                                    Task.Run(
                                        Sub()
                                            For i = -lct.Width To 50 Step 2
                                                lct.Location = New Drawing.Point(i, 50)
                                            Next
                                            lct.Location = New Drawing.Point(50, 50)
                                            Thread.Sleep(5000)
                                            For i = 50 To -lct.Width Step -2
                                                lct.Location = New Drawing.Point(i, 50)
                                            Next
                                        End Sub)

                                    Return 0
                                End Function)
                        Case "sound"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Dim media = New Media(libvlc, chapterPath + "\" + cmd.Split(" ")(1))
                                    mpls(2).Media = media

                                    Task.Run(
                                        Sub()
                                            Do Until Not isFading
                                                Thread.Sleep(100)
                                            Loop
                                            mpls(2).Play()
                                        End Sub)

                                    'MsgBox(key(1))
                                    Return 3
                                End Function)
                        Case "music"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Dim media = New Media(libvlc_repeat, chapterPath + "\" + cmd.Split(" ")(1))
                                    mpls(0).Media = media
                                    mpls(0).Play()
                                    Return 0
                                End Function)
                        Case "stopmusic"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    mpls(0).Stop()
                                    Return 0
                                End Function)
                        Case "fade_white"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Me.Enabled = False
                                    isFading = True
                                    fadeScreen.Show()
                                    Task.Run(
                                        Sub()

                                            Dim fadeColor = 0

                                            fadeScreen.BringToFront()
                                            fadeScreen.BackColor = Drawing.Color.FromArgb(0, 255, 255, 255)

                                            While CBool(255 - fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 255, 255, 255)
                                                fadeColor += 1
                                                Thread.Sleep(1)
                                                If stopEffect Then
                                                    isFading = False
                                                    stopEffect = False
                                                    Me.Enabled = True
                                                    Exit Sub
                                                End If
                                            End While

                                            isFading = False

                                            While CBool(fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 255, 255, 255)
                                                fadeColor -= 1
                                                Thread.Sleep(1)
                                                If stopEffect Then
                                                    isFading = False
                                                    stopEffect = False
                                                    Me.Enabled = True
                                                    Exit Sub
                                                End If
                                            End While

                                            Me.Enabled = True
                                            fadeScreen.Hide()
                                        End Sub)
                                    Return 0
                                End Function)
                        Case "fade_black"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Me.Enabled = False
                                    isFading = True
                                    fadeScreen.Show()
                                    Task.Run(
                                        Sub()
                                            Dim fadeColor = 0

                                            fadeScreen.BringToFront()
                                            fadeScreen.BackColor = Drawing.Color.FromArgb(0, 0, 0, 0)

                                            While CBool(255 - fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                                                fadeColor += 1
                                                Thread.Sleep(1)
                                                If stopEffect Then
                                                    isFading = False
                                                    stopEffect = False
                                                    Me.Enabled = True
                                                    Exit Sub
                                                End If
                                            End While

                                            isFading = False

                                            While CBool(fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                                                fadeColor -= 1
                                                Thread.Sleep(1)
                                                If stopEffect Then
                                                    isFading = False
                                                    stopEffect = False
                                                    Me.Enabled = True
                                                    Exit Sub
                                                End If
                                            End While

                                            Me.Enabled = True
                                            fadeScreen.Hide()
                                        End Sub)
                                    Return 0
                                End Function)
                        Case "flash"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Me.Enabled = False
                                    isFading = True
                                    fadeScreen.Show()
                                    Task.Run(
                                        Sub()
                                            Dim fadeColor = 0

                                            fadeScreen.BringToFront()
                                            fadeScreen.BackColor = Drawing.Color.FromArgb(0, 255, 255, 255)

                                            While CBool(255 - fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 255, 255, 255)
                                                fadeColor += 3
                                                Thread.Sleep(1)
                                            End While

                                            isFading = False
                                            fadeColor = 255

                                            While CBool(fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 255, 255, 255)
                                                fadeColor -= 3
                                                Thread.Sleep(1)
                                            End While

                                            Me.Enabled = True
                                            fadeScreen.Hide()
                                        End Sub)
                                    Return 0
                                End Function)
                        Case "hide_left"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    p1.Hide()
                                    Return 0
                                End Function)
                        Case "hide_right"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    p3.Hide()
                                    Return 0
                                End Function)
                        Case "hide_center"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    p2.Hide()
                                    Return 0
                                End Function)
                        Case "video"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    'I have no time to implement this
                                    Return 0
                                End Function)
                        Case "exit"
                            cmdQueue.Enqueue(
                                Function() As Object
                                    Task.Run(
                                        Sub()
                                            stopEffect = True

                                            Me.Enabled = True
                                            fadeScreen.Show()
                                            fadeScreen.BringToFront()
                                            fadeScreen.BackColor = Drawing.Color.FromArgb(0, 0, 0, 0)

                                            Dim fadeColor = 0

                                            While CBool(255 - fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                                                fadeColor += 1
                                                Thread.Sleep(1)
                                            End While

                                            mpls(0).Stop()
                                            Me.BackgroundImage = Drawing.Image.FromFile(StoryListBG)
                                            mpls(0).Media = New Media(libvlc_repeat, startBGM(GetRandom(0, startBGM.Count)))
                                            mpls(0).Play()

                                            While CBool(fadeColor)
                                                fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                                                fadeColor -= 1
                                                Thread.Sleep(1)
                                            End While

                                            For Each ctrl As Control In Me.Controls()
                                                If ctrl.Visible Then
                                                    ctrl.Hide()
                                                End If
                                            Next

                                            storyTableList(storyTableCurrentIndex).Show()
                                            setStoryListTitle()
                                            StoryListTitle.Show()

                                            If storyTableList.Count > 1 Then
                                                btnSwitchStoryL.Show()
                                                btnSwitchStoryR.Show()
                                            End If

                                            For Each ctrl As Control In Me.Controls()
                                                If ctrl.Visible Then
                                                    ctrl.Enabled = True
                                                End If
                                            Next

                                            Me.Text = "選擇視覺小說 - Visual Novel Engine"
                                            Me.AllowDrop = True
                                        End Sub)

                                    scriptLs.Clear()
                                    deQCount = 0

                                    stopEffect = False
                                    isDisplayingStory = False
                                    Return 0
                                End Function)
                    End Select
                Next
                'MsgBox("finished loading.")
            End Sub)

        Await Task.Run(
            Sub()
                Dim fadeColor = 0
                fadeScreen.BringToFront()
                fadeScreen.BackColor = Drawing.Color.FromArgb(0, 0, 0, 0)
                fadeScreen.Show()
                While CBool(255 - fadeColor)
                    fadeScreen.BackColor = Drawing.Color.FromArgb(fadeColor, 0, 0, 0)
                    fadeColor += 1
                    Thread.Sleep(2)
                End While
            End Sub)

        Me.BackgroundImage = Nothing

        For Each ctrl As Control In Me.Controls()
            If ctrl.Visible Then
                ctrl.Hide()
                ctrl.Enabled = False
            End If
        Next

        fadeScreen.Hide()
        mpls(0).Stop()

        isLoadingStory = False
        isDisplayingStory = True

        'Thread.Sleep(300)
        Me.Text = Path.GetFileNameWithoutExtension(chapterPath) + " - Visual Novel Engine"

        displayStory()
    End Sub

    Private Sub displayStory() Handles Me.Click
        If isDisplayingStory Then
            'Avoid Race Condition
            If isDisplayingEffect Then
                stopEffect = True
                'MsgBox("stop")
                Exit Sub
            Else
                Do While cmdQueue.Count
                    If title_fullscreen.Visible Then
                        title_fullscreen.Hide()
                    End If
                    If description.Visible Then
                        description.Hide()
                    End If
                    If txt.Visible Then
                        txt.Hide()
                    End If

                    Select Case cmdQueue(0)()
                        Case 0
                            cmdQueue.Dequeue()
                            deQCount += 1
                            'MsgBox(deQCount)
                        Case 1
                            cmdQueue(1)()
                            cmdQueue.Dequeue()
                            cmdQueue.Dequeue()
                            deQCount += 2
                            'MsgBox(deQCount)
                            Exit Do
                        Case 2
                            cmdQueue.Dequeue()
                            deQCount += 1
                            While skip
                                cmdQueue.Dequeue()
                                deQCount += 1
                                skip -= 1
                            End While
                            'MsgBox(deQCount)
                        Case 3
                            cmdQueue.Dequeue()
                            deQCount += 1
                            'MsgBox(deQCount)
                            Exit Do
                    End Select
                    'MsgBox(cmdQueue.Count)
                Loop
            End If
        End If
    End Sub
End Class
