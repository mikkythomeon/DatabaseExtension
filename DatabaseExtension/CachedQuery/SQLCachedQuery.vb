Imports System.Data.SqlClient

Public Class SQLCachedQuery
    Inherits CachedQuery

    Sub New(ByVal SqlFileName As String)
        MyBase.New(SqlFileName)
    End Sub

    Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub createConcreteCommand()
        _cmd = New SqlCommand()
    End Sub
End Class
