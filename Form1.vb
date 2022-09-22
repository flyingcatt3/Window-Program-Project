Imports WinFormsApp1.Argus.Audio

Public Class Form1
    Dim Path = Application.StartupPath
    ReadOnly startBGM = Path + "start.mp3"
    Dim audioPlayer As New AudioFile()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PlayMusic(startBGM)
        ver.Text = "20220922"
    End Sub
    Sub PlayMusic(sender)
        audioPlayer.Filename = sender
        audioPlayer.Play()
    End Sub

    Private Sub Start_Click(sender As Object, e As EventArgs) Handles Start.Click
        audioPlayer.Stop()
        Start.Dispose()
        GameTitle.Dispose()
    End Sub

    Private Sub NextSentence(sender As Object, e As EventArgs) Handles MyBase.Click

    End Sub

End Class
