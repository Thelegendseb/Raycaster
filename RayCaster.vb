
Public Class RayCaster

    Public P As Player

    Public Canvas As Bitmap

    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Integer) As Short

    Dim MyMap(,) As Integer = {{1, 1, 1, 1, 1, 1, 1, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 0, 3, 0, 0, 2, 0, 1},
                               {1, 0, 3, 0, 0, 2, 0, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 1, 1, 1, 1, 1, 1, 1}}

    Public Scaling As Integer = 60


    Public UISize As Integer = 0

    'EXTERNAL CALLS ==========================================
    Sub New(S As Size)
        Canvas = New Bitmap(S.Width, S.Height)
        UISize = S.Width / 70

        P = New Player

        RayCastInit()
    End Sub
    Public Sub Update()
        'This Method is called every frame
        DrawBackground()
        Buttons()
        Draw()
    End Sub
    ' =============================================================
    Public Sub Draw()
        Using g As Graphics = Graphics.FromImage(Canvas)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            Alls(g)
            DrawMap(g)
            DrawPlayer(g)
        End Using
    End Sub

    '============================================================
    'SIDE CLASSES/METHODS:
    Private Sub DrawBackground()
        Using g As Graphics = Graphics.FromImage(Canvas)
            g.FillRectangle(New SolidBrush(Color.Blue), New Rectangle(0, 0, Canvas.Width, Canvas.Height / 2))
            g.FillRectangle(New SolidBrush(Color.Gray), New Rectangle(0, Canvas.Height / 2, Canvas.Width, Canvas.Height / 2))
        End Using
    End Sub

    Sub DrawPlayer(ByRef g As Graphics)
        Dim Projx, Projy As Integer
        Projx = P.x * UISize / Scaling
        Projy = P.y * UISize / Scaling
        g.FillEllipse(Brushes.Red, Projx - 3, Projy - 3, 6, 6)
    End Sub
    Sub DrawMap(ByRef g As Graphics)
        Dim xo, yo As Integer
        g.FillRectangle(Brushes.DarkSlateGray, 0, 0, MyMap.GetLength(0) * UISize, MyMap.GetLength(1) * UISize)
        For y = 0 To MyMap.GetLength(0) - 1
            For x = 0 To MyMap.GetLength(1) - 1
                xo = x * UISize
                yo = y * UISize
                If MyMap(y, x) = 1 Then
                    g.FillRectangle(Brushes.LimeGreen, xo, yo, UISize - 1, UISize - 1)
                ElseIf MyMap(y, x) = 2 Then
                    g.FillRectangle(Brushes.Red, xo, yo, UISize - 1, UISize - 1)
                ElseIf MyMap(y, x) = 3 Then
                    g.FillRectangle(Brushes.DarkBlue, xo, yo, UISize - 1, UISize - 1)
                Else
                    g.FillRectangle(Brushes.Black, xo, yo, UISize - 1, UISize - 1)
                End If
            Next
        Next
    End Sub
    Private Sub Alls(ByRef g As Graphics)

        Dim Counter As Integer = 0
        For i = -P.FOV To P.FOV Step P.Density
            Dim R As New Ray(P.x, P.y, P.theta + i)

            R.Formulate(P, MyMap, Scaling)

            ' rl = FixFishEye(rl, pa - ra)
            ReDim Preserve P.Rays(Counter)
            P.Rays(Counter) = R
            Counter += 1
        Next
#Disable Warning BC42104
        Projection(g)
#Enable Warning BC42104
    End Sub

    Private Function FixFishEye(rl As Single, ad As Single) As Single
        'If ad < 0 Then ad += 2 * Math.PI
        'If ad > 2 * Math.PI Then ad -= 2 * Math.PI
        'Return rl * Math.Cos(ad)
        Return rl
    End Function
    Private Sub Projection(ByRef g As Graphics)
        Dim ColWith As Integer = Canvas.Width / P.Rays.Length
        Dim counter As Integer
        For i = 0 To P.Rays.Length - 1
            If P.Rays(i).Length <> 0 Then
                Dim LineHeight As Integer = (1 / P.Rays(i).Length * Canvas.Height) * 3.5
                If LineHeight > Canvas.Height Then LineHeight = Canvas.Height
                Dim val As Integer = 255 - (255 * P.Rays(i).Length / (P.Rays.Length) * P.Depth)
                If val < 0 Then val = 0
                If val > 255 Then val = 255
                Dim c As Color
                Select Case P.Rays(i).HitValue
                    Case 1
                        c = Color.FromArgb(255, 0, val, 0)
                    Case 2
                        c = Color.FromArgb(255, val, 0, 0)
                    Case 3
                        c = Color.FromArgb(255, 0, 0, val)
                End Select

                g.FillRectangle(New SolidBrush(c), counter, CSng(Canvas.Height / 2) - CSng(LineHeight / 2), ColWith, LineHeight)
            End If
            counter += ColWith
        Next
    End Sub
    Private Sub Buttons()
        If GetAsyncKeyState(65) Then 'a
            P.theta -= 0.05
            If P.theta < 0 Then
                P.theta += 2 * Math.PI
            End If
            P.dx = Math.Cos(P.theta) * 5
            P.dy = Math.Sin(P.theta) * 5
        End If
        If GetAsyncKeyState(68) Then 'd
            P.theta += 0.05
            If P.theta > 2 * Math.PI Then
                P.theta -= 2 * Math.PI
            End If
            P.dx = Math.Cos(P.theta) * 5
            P.dy = Math.Sin(P.theta) * 5
        End If
        If GetAsyncKeyState(87) Then 'w
            If CellValue(P.y + (P.dy * 3), P.x + (P.dx * 3), MyMap, Scaling) = 0 Then
                P.y += P.dy
                P.x += P.dx
            End If
        End If
        If GetAsyncKeyState(83) Then 's
            If CellValue(P.y - (P.dy * 3), P.x - (P.dx * 3), MyMap, Scaling) = 0 Then
                P.x -= P.dx
                P.y -= P.dy
            End If
        End If
    End Sub
    Sub RayCastInit()
        P.y = Scaling * 1.5
        P.x = Scaling * 1.5
        P.theta = Math.PI / 2
        P.dx = Math.Cos(P.theta) * 5
        P.dy = Math.Sin(P.theta) * 5
    End Sub

    '============
    Public Shared Function CellValue(y As Single, x As Single, G(,) As Integer, ScaleFactor As Integer) As Integer
        Try
            Return G(Math.Floor(y / ScaleFactor), Math.Floor(x / ScaleFactor))
        Catch ex As Exception
            Return 0
        End Try
    End Function

End Class
