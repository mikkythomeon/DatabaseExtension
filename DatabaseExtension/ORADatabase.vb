Imports System.Data.OracleClient

Public Class ORADatabase
    Inherits Database

 Sub New(Server As String, Database As String)
        Throw New NotImplementedException
    End Sub

    Sub New(UserName As String, Password As String, Server As String, Database As String)
        Initialize(UserName, Password, Server, Database)
    End Sub

    Sub New(n As System.Xml.XmlNode)
        Initialize(n.Attributes("UserName").Value, n.Attributes("Password").Value, n.Attributes("Server").Value, n.Attributes("Database").Value)
    End Sub

    Private Sub Initialize(UserName As String, Password As String, Server As String, Database As String)
        Dim builder As New OracleConnectionStringBuilder
        builder.Password = Password
        builder.UserID = UserName
        builder.DataSource = Server

        Dim cn As New OracleClient.OracleConnection
        connection = cn
        connection.ConnectionString = builder.ConnectionString
        command = New OracleClient.OracleCommand
        command.Connection = connection

        dataadapter = New OracleClient.OracleDataAdapter
    End Sub

    Public Overrides Sub RefreshDB()
        If connection.State = ConnectionState.Open = True Then connection.Close()
        connection.Open()
    End Sub

End Class
