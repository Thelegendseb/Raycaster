
Public Class RayCaster

    Public P As Player

    Public Canvas As Bitmap

    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Integer) As Short

    Dim MyMap(,) As Integer = {{1, 1, 1, 1, 1, 1, 1, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 0, 2, 3, 4, 5, 0, 1},
                               {1, 0, 2, 3, 4, 5, 0, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 0, 0, 0, 0, 0, 0, 1},
                               {1, 1, 1, 1, 1, 1, 1, 1}}

    Public CellSize As Integer = 50


    Public UISize As Integer = 0 'calculated later on

    'EXTERNAL CALLS ==========================================
    Sub New(S As Size)
        Canvas = New Bitmap(S.Width, S.Height)
        UISize = S.Width / 70

        P = New Player
        Cursor.Hide()
        RayCastInit()
    End Sub
    Public Sub Update()
        'This Method is called every frame
        DrawBackground()
        Buttons()
        Cast()
        Draw()
    End Sub
    ' =============================================================
    Public Sub Draw()
        Using g As Graphics = Graphics.FromImage(Canvas)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            Projection(g)
            DrawMap(g)
            DrawPlayer(g)
        End Using
    End Sub

    '============================================================
    'SIDE CLASSES/METHODS:
    Private Sub DrawBackground()
        Using g As Graphics = Graphics.FromImage(Canvas)
            g.FillRectangle(New SolidBrush(Color.Blue), New Rectangle(0, 0, Canvas.Width, (Canvas.Height / 2) - P.delta))
            g.FillRectangle(New SolidBrush(Color.Green), New Rectangle(0, (Canvas.Height / 2) - P.delta, Canvas.Width, (Canvas.Height / 2) + P.delta))
        End Using
    End Sub

    Sub DrawPlayer(ByRef g As Graphics)
        Dim Projx, Projy As Integer
        Projx = P.x * UISize / CellSize
        Projy = P.y * UISize / CellSize
        g.FillEllipse(Brushes.Yellow, Projx - 3, Projy - 3, 6, 6)
        g.DrawLine(Pens.Yellow, Projx, Projy, Projx + (P.dx * 3), Projy + (P.dy * 3))
    End Sub
    Sub DrawMap(ByRef g As Graphics)
        Dim xo, yo As Integer
        g.FillRectangle(Brushes.DarkSlateGray, 0, 0, MyMap.GetLength(0) * UISize, MyMap.GetLength(1) * UISize)
        For y = 0 To MyMap.GetLength(0) - 1
            For x = 0 To MyMap.GetLength(1) - 1
                xo = x * UISize
                yo = y * UISize
                If MyMap(y, x) <> 0 Then
                    g.FillRectangle(Brushes.LightGray, xo, yo, UISize - 1, UISize - 1)
                Else
                    g.FillRectangle(Brushes.Black, xo, yo, UISize - 1, UISize - 1)
                End If
            Next
        Next
    End Sub
    Private Sub Cast()
        Dim Counter As Integer = 0
        For i = -P.FOV To P.FOV Step P.Density
            Dim R As New Ray(P.x, P.y, P.theta + i)

            R.Formulate(P, MyMap, CellSize)

            ReDim Preserve P.Rays(Counter)
            P.Rays(Counter) = R
            Counter += 1
        Next
    End Sub
    Private Sub Projection(ByRef g As Graphics)
        Dim ColWidth As Integer = Canvas.Width / P.Rays.Length
        Dim counter As Integer
        For i = 0 To P.Rays.Length - 1
            If P.Rays(i).Length <> 0 Then
                Dim LineHeight As Integer

                Dim proj As Single = CellSize / Math.Tan(P.FOV / 2)
                LineHeight = Int(Canvas.Height * proj / (P.Rays(i).Length) / 15)


                If LineHeight > Canvas.Height Then LineHeight = Canvas.Height
                Dim val As Integer = 255 - (255 * P.Rays(i).Length / (P.Rays.Length) * P.Depth)
                If val < 0 Then val = 0
                If val > 255 Then val = 255
                Dim c As Color
                Select Case P.Rays(i).HitValue
                    Case 1
                        '         DrawTexture(My.Resources.Wall, P.Rays(i), LineHeight, counter, ColWidth, g)
                        c = Color.FromArgb(255, val, val, val)
                        g.FillRectangle(New SolidBrush(c), counter, CSng(Canvas.Height / 2) - CSng(LineHeight / 2) - P.delta, ColWidth, LineHeight)
                    Case 2
                        c = Color.FromArgb(255, val, 0, 0)
                        g.FillRectangle(New SolidBrush(c), counter, CSng(Canvas.Height / 2) - CSng(LineHeight / 2) - P.delta, ColWidth, LineHeight)
                    Case 3
                        c = Color.FromArgb(255, 0, 0, val)
                        g.FillRectangle(New SolidBrush(c), counter, CSng(Canvas.Height / 2) - CSng(LineHeight / 2) - P.delta, ColWidth, LineHeight)
                    Case 4
                        c = Color.FromArgb(255, 0, val, val)
                        g.FillRectangle(New SolidBrush(c), counter, CSng(Canvas.Height / 2) - CSng(LineHeight / 2) - P.delta, ColWidth, LineHeight)
                    Case 5
                        c = Color.FromArgb(255, val, val, 0)
                        g.FillRectangle(New SolidBrush(c), counter, CSng(Canvas.Height / 2) - CSng(LineHeight / 2) - P.delta, ColWidth, LineHeight)
                End Select
            End If
            counter += ColWidth
        Next
    End Sub

    Sub DrawTexture(Texture As Image, R As Ray, LH As Single, C As Integer, ColWidth As Integer, ByRef g As Graphics)

        Dim x As Integer = Texture.Width / 2 ' R.y - CellSize * Math.Floor(R.y / CellSize)

        Dim bmp As New Bitmap(Texture, Texture.Size)
        Dim sec As Bitmap = bmp.Clone(New Rectangle(x, 0, ColWidth, bmp.Height), bmp.PixelFormat)
        Dim final As New Bitmap(sec, New Size(ColWidth, LH))

        g.DrawImageUnscaled(final, New Point(C, CSng(Canvas.Height / 2) - CSng(LH / 2) - P.delta))

        bmp.Dispose()
        sec.Dispose()
        final.Dispose()
    End Sub

    Private Sub Buttons()

        If GetAsyncKeyState(65) Then 'a
            P.theta -= P.TurningSpeed
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
            If CellValue(P.y + (P.dy * 3), P.x + (P.dx * 3), MyMap, CellSize) = 0 Then
                P.y += P.dy
                P.x += P.dx
            End If
        End If
        If GetAsyncKeyState(83) Then 's
            If CellValue(P.y - (P.dy * 3), P.x - (P.dx * 3), MyMap, CellSize) = 0 Then
                P.x -= P.dx
                P.y -= P.dy
            End If
        End If
        If GetAsyncKeyState(77) Then 'm
            P.delta += 25
        End If
        If GetAsyncKeyState(78) Then 'n
            P.delta -= 25
        End If
    End Sub
    Sub RayCastInit()
        P.y = CellSize * 1.5
        P.x = CellSize * 1.5
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
