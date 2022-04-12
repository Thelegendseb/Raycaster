Public Class Player

    Public x, y, dx, dy As Single
    Public theta As Single

    Public Density As Single  'radian increment
    Public FOV As Single   '0--1 'total radian span
    Public Depth As Single

    Public Rays() As Ray

    Sub New(Optional Densityin As Single = 0.005,
            Optional FOVin As Single = 0.7,
            Optional Depthin As Single = 3)
        'Defualt Values
        'Density = 0.005
        'FOV = 0.7
        'Depth = 2
        Density = Densityin
        FOV = FOVin
        Depth = Depthin
    End Sub

End Class
