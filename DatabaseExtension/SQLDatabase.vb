Imports System.Data.SqlClient

Public Class SQLDatabase
    Inherits Database

    Sub New(Server As String, Database As String)
        MyBase.connection = New SqlClient.SqlConnection("Server=" + Server + ";Database=" + Database + ";Trusted_Connection=true;Connection Timeout=120;")
        MyBase.command = New SqlClient.SqlCommand
        command.Connection = connection
        dataadapter = New SqlClient.SqlDataAdapter
    End Sub

    Sub New(UserName As String, Password As String, Server As String, Database As String)
        MyBase.connection = New SqlClient.SqlConnection("Server=" + Server + ";Database=" + Database + ";User Id=" + UserName + ";Password=" + Password + "; Connection Timeout=20;")
        MyBase.command = New SqlClient.SqlCommand
        command.Connection = connection
        dataadapter = New SqlClient.SqlDataAdapter
    End Sub

    Sub New(n As System.Xml.XmlNode)
        'Initialize(n.Attributes("UserName").Value, n.Attributes("Password").Value, n.Attributes("ServerAddress").Value, n.Attributes("Database").Value)
        If n.Attributes("UserName") Is Nothing Then
            MyBase.connection = New SqlClient.SqlConnection("Server=" + n.Attributes("ServerAddress").Value + ";Database=" + n.Attributes("Database").Value + ";Trusted_Connection=true;")
        Else
            MyBase.connection = New SqlClient.SqlConnection("Server=" + n.Attributes("ServerAddress").Value + ";Database=" + n.Attributes("Database").Value + ";User Id=" + n.Attributes("UserName").Value + ";Password=" + n.Attributes("Password").Value + "; Connection Timeout=20;")
        End If

        MyBase.command = New SqlClient.SqlCommand
        command.Connection = connection
        dataadapter = New SqlClient.SqlDataAdapter
    End Sub

    Public Overrides Sub RefreshDB()
        If connection.State = ConnectionState.Open = True Then connection.Close()
        connection.Open()
    End Sub

End Class
