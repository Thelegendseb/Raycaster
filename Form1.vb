



Public Class Form1

    Dim WithEvents T As New Timer

    Public Screen As PictureBox

    Dim Game As RayCaster

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetInitials()

        Screen = New PictureBox With {
            .Location = New Point(0, 0),
            .Size = Me.ClientSize
        }
        Me.Controls.Add(Screen)
        Game = New RayCaster(Screen.Size)

        T.Interval = 30
        T.Start()
    End Sub

    Private Sub T_Tick(sender As Object, e As EventArgs) Handles T.Tick
        Game.Update()
        Render()
    End Sub

    Private Sub Render()
        Screen.Image = Game.Canvas
    End Sub

    Sub SetInitials()
        Me.Text = "RayCaster"
        Me.WindowState = FormWindowState.Maximized
        Me.CenterToScreen()
    End Sub

End Class