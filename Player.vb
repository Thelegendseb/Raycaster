Public Class Player

    Public x, y, dx, dy As Single
    Public theta As Single
    Public delta As Single

    Public Density As Single  'radian increment
    Public FOV As Single    'total radian span
    Public Depth As Single

    Public Rays() As Ray

    Public TurningSpeed As Single = 0.07

    Sub New(Optional Densityin As Single = 0.005,
            Optional FOVin As Single = 0.7,
            Optional Depthin As Single = 2.5)
        'Defualt Values
        'Density = 0.005
        'FOV = 0.7
        'Depth = 3
        Density = Densityin
        FOV = FOVin
        Depth = Depthin
    End Sub

End Class
