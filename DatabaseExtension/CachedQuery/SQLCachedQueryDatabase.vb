Public Class SQLCachedQueryDatabase
    Inherits CachedQueryDatabase

    Sub New(ByVal Server As String, ByVal Database As String)
        MyBase.connection = New SqlClient.SqlConnection("Server=" + Server + ";Database=" + Database + ";Trusted_Connection=true;Connection Timeout=60;")
    End Sub

    Sub New(ByVal UserName As String, ByVal Password As String, ByVal Server As String, ByVal Database As String)
        MyBase.connection = New SqlClient.SqlConnection("Server=" + Server + ";Database=" + Database + ";User Id=" + UserName + ";Password=" + Password + "; Connection Timeout=60;")
    End Sub

    Protected Overrides Sub createConcreteDataAdapter()
        If MyBase.dataadapter Is Nothing Then MyBase.dataadapter = New SqlClient.SqlDataAdapter
    End Sub

    Protected Overrides Sub createConcreteConnection()
        connection = New SqlClient.SqlConnection
    End Sub

    Protected Overrides Sub setAdapterCommand(ByVal cmd As CachedQuery)
        Dim adapter As SqlClient.SqlDataAdapter = dataadapter
        Dim c As IDbCommand = cmd
        adapter.SelectCommand = c
    End Sub

    Protected Overrides Function createConcreteCachedQuery(ByVal FileName As String) As CachedQuery
        Return New SQLCachedQuery(FileName)
    End Function

    Protected Overrides Function createConcreteCachedQuery() As CachedQuery
        Return New SQLCachedQuery()
    End Function
End Class
