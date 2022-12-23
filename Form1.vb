Imports SixLabors.ImageSharp
Imports System.Drawing.Text
Imports System.IO
Imports LibVLCSharp.Shared
Imports System.Threading
Imports System.IO.Compression
Imports System.Reflection
Imports System.Xml
Imports System.Text

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
            'SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
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

    ReadOnly supportedImgFormat As New List(Of String) From {".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tiff"}
    ReadOnly gamePath = Application.StartupPath
    ReadOnly storyPath = gamePath + "story\"
    ReadOnly converted_imgPath = gamePath + "converted_imgs\"
    ReadOnly StoryListBG = gamePath + "StoryListBG.jpg"
    ReadOnly Clicksound As String = gamePath + "click.mp3"
    ReadOnly addStorySound As String = gamePath + "addstory.wav"
    ReadOnly libvlc_repeat = New LibVLC("--role=music", "--input-repeat=65535", "--aout=mmedevice", "--mmdevice-backend=wasapi")
    ReadOnly libvlc = New LibVLC("--role=music", "--aout=mmedevice", "--mmdevice-backend=wasapi")
    Dim startBGM As New List(Of String)()
    Dim storyTableList As New List(Of TableLayoutPanel)()
    Dim chapterTableList As New List(Of TableLayoutPanel)()
    Dim storyTableCurrentIndex As Integer = 0
    Dim StoryListTitle As New Label()
    Dim tmpWindowSize, resizing
    Dim pfc As New PrivateFontCollection()
    Dim fadeScreen As New fadePanel
    Dim isStoryListEmpty As Boolean = False
    Dim isLoadingStory As Boolean
    Dim btnSwitchStoryL As New switchStoryBtn
    Dim btnSwitchStoryR As New switchStoryBtn
    Dim tmpStoryLs As New List(Of String)
    Dim storyLs As New List(Of String)

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

    Async Sub setStoryListTitle()
        Dim txt = "選擇故事_"
        StoryListTitle.Text = ""

        Await Task.Run(
            Sub()

                For x = 0 To txt.Length - 1
                    StoryListTitle.Text &= txt(x)
                    Thread.Sleep(50)
                Next

                Do While StoryListTitle.Visible
                    If Me.WindowState <> FormWindowState.Minimized And Not resizing Then
                        If isLoadingStory Then
                            StoryListTitle.Text = "Loading..."
                        Else
                            StoryListTitle.Text = txt
                            Thread.Sleep(100)
                            StoryListTitle.Text = StoryListTitle.Text.Remove(txt.Length - 1)
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
        'DoubleBuffered = True
        'SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)

        '開場音樂
        For Each foundFile As String In My.Computer.FileSystem.GetFiles(gamePath + "startBGM")
            startBGM.Add(foundFile)
        Next

        Dim file As String = startBGM(GetRandom(0, startBGM.Count - 1))
        Dim media = New Media(libvlc_repeat, file)
        Dim mediaPlayer = New MediaPlayer(media)

        mediaPlayer.EnableHardwareDecoding = True
        mediaPlayer.Volume = 60
        mediaPlayer.Play()

        '格式設定
        Me.ClientSize = New Drawing.Size(Screen.PrimaryScreen.Bounds.Width * 0.98, Screen.PrimaryScreen.Bounds.Width * 0.42)
        Me.StartLayout.Width = Me.ClientSize.Width / 2
        Me.StartLayout.Height = Me.ClientSize.Height / 3 * 2
        Me.ver.Text = "v" + System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyyMMdd")
        tmpWindowSize = Me.Size
        'Me.StartLayout.Show()
        'Me.BackgroundImage = Bitmap.FromFile(StoryListBG, Drawing.Imaging.PixelFormat.Format32bppPArgb)
        'Me.BackgroundImage = Drawing.Image.FromFile(StoryListBG)
        AddHandler Start.Click, AddressOf ButtonClick
        AddHandler Start.GotFocus, AddressOf ButtonCursor

        pfc.AddFontFile(gamePath + "TaipeiSansTCBeta-Regular.ttf")

        Start.Font = New Font(pfc.Families(0), 36, FontStyle.Bold)
        GameTitle.Font = New Font(pfc.Families(0), 54, FontStyle.Bold)

        Me.CenterToScreen()
        StartColorCycle()
        ChooseStory_PreLoad()

    End Sub

    Private Async Sub fadeStartLayout() Handles Start.Click

        Start.Enabled = False

        Me.Controls.Add(fadeScreen)

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
        fadeScreen.Dispose()
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
        Dim media = New Media(libvlc, Clicksound)
        Dim mediaplayer = New MediaPlayer(media)
        mediaplayer.EnableHardwareDecoding = True
        mediaplayer.Play()
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
                    AddHandler chapterBtn.GotFocus, AddressOf ButtonCursor

                    chapterTable.Controls.Add(chapterBtn)

                End If
            Next

            AddHandler storyBtn.Click, AddressOf storyBtnClick
            AddHandler storyBtn.Click, AddressOf ButtonClick
            AddHandler storyBtn.GotFocus, AddressOf ButtonCursor

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
    End Sub

    Private Sub ChooseStory()

        'If Not supportedImgFormat.Contains(Path.GetExtension(StoryListBG)) Then
        'imgConverter(StoryListBG)
        '= Drawing.Image.FromFile(converted_imgPath + Path.GetFileNameWithoutExtension(StoryListBG) + ".jpg")

        Me.Text = "選擇故事 - Visual Novel Engine"
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
                    table.Controls.Item(1).Size = New Drawing.Size(table.Width, table.Height / 3)
                    table.Controls.Item(0).Size = New Drawing.Size(table.Width, table.Controls.Item(1).Height * 2)
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
        If Me.WindowState <> FormWindowState.Minimized And storyTableList.Count And Not StoryListTitle.Visible Then
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

        Dim media = New Media(libvlc, addStorySound)
        Dim mediaplayer = New MediaPlayer(media)
        mediaplayer.EnableHardwareDecoding = True
        mediaplayer.Volume = Math.Sqrt(mediaplayer.Volume) * 10

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
            mediaplayer.Play()
        ElseIf CBool(tmpStoryLs.Count) Then '已經有story
            loadStory()
            mediaplayer.Play()
        End If
        btnSwitchStoryAdd()
        tmpStoryLs.Clear()
        isLoadingStory = False
    End Sub

    Private Sub FileDragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragEnter
        e.Effect = DragDropEffects.All
    End Sub

    'Private Sub ver_Click(sender As Object, e As EventArgs) Handles ver.Click

    'End Sub

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
            AddHandler btnSwitchStoryL.GotFocus, AddressOf ButtonCursor
            AddHandler btnSwitchStoryR.Click, AddressOf ButtonClick
            AddHandler btnSwitchStoryR.Click, AddressOf btnSwitchStoryClick
            AddHandler btnSwitchStoryR.GotFocus, AddressOf ButtonCursor

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
        If My.Computer.FileSystem.GetDirectories(storyLs(storyTableCurrentIndex)).Count > 0 Then
            storyTableList(storyTableCurrentIndex).Hide()
            btnSwitchStoryL.Hide()
            btnSwitchStoryR.Hide()
            chapterTableList(storyTableCurrentIndex).Show()
        Else
            MsgBox("在該故事的目錄中找不到任何章節(資料夾)。", vbCritical, "Error")
        End If
    End Sub

    Private Sub chapterBtnClick()
        Me.AllowDrop = False
    End Sub
End Class
