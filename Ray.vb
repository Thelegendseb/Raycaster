Public Class Ray

    Public x, y, dx, dy As Single
    Public theta As Single
    Public Length As Single
    Public HitValue As Integer
    Public VerticalHit As Boolean

    Public Shared ReadOnly PrecisionValue As Single = 0.9

    Sub New(xin As Single, yin As Single, thetain As Single)
        x = xin
        y = yin
        theta = thetain
        Length = 0
        HitValue = 0
    End Sub

    Sub Formulate(ByVal P As Player, G(,) As Integer, ScaleFactor As Integer)

        Dim R As New Ray(P.x, P.y, Me.theta)

        R.dx = Math.Cos(R.theta) * PrecisionValue
        R.dy = Math.Sin(R.theta) * PrecisionValue
        R.Length = 0
        While True
            R.x += R.dx
            R.y += R.dy
            If R.x < 0 Or R.x > ScaleFactor * G.GetLength(1) Or R.y < 0 Or R.y > ScaleFactor * G.GetLength(0) Then
                R.Length = 0
                Exit While
            End If
            Dim tempcellval As Integer = RayCaster.CellValue(R.y, R.x, G, ScaleFactor)
            If tempcellval <> 0 Then
                R.HitValue = tempcellval
                Exit While
            End If
            R.Length += 0.25
        End While

        If RayCaster.CellValue(R.y, R.x + R.dx, G, ScaleFactor) <> 0 Then
            R.VerticalHit = True
        Else
            R.VerticalHit = False
        End If

        FixFishEye()
        Match(R)
    End Sub
    Private Sub FixFishEye()
        Dim templength As Single = Me.Length * Math.Cos(Me.theta)
        Me.Length = templength
    End Sub

    Private Sub Match(R As Ray)
        Me.x = R.x
        Me.y = R.y
        Me.dx = R.dx
        Me.dy = R.dy
        Me.theta = R.theta
        Me.Length = R.Length
        Me.HitValue = R.HitValue
    End Sub

End Class
