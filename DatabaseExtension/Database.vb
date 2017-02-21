Public MustInherit Class Database
    Implements IDatabase

    Protected connection As System.Data.Common.DbConnection
    Protected command As System.Data.Common.DbCommand
    Protected dataadapter As System.Data.Common.DataAdapter
    Protected dataset As System.Data.DataSet
    Protected records_affected As Integer

    Sub OpenConnection() Implements IDatabase.OpenConnection
        If Not connection Is Nothing Then
            If connection.State = ConnectionState.Broken Then
                connection.Close()
                connection.Open()
            ElseIf connection.State = ConnectionState.Closed Then
                connection.Open()
            End If
        End If
    End Sub

    Public Function ExecuteReader(ByVal sql As String) As Data.IDataReader Implements IDatabase.ExecuteReader
        OpenConnection()
        command.CommandText = sql
        Return command.ExecuteReader()
    End Function

    Public Sub ExecuteCommand(ByVal sql As String) Implements IDatabase.ExecuteCommand
        OpenConnection()
        command.CommandText = sql

        'Dim writer As New IO.StreamWriter("execute_command.log")
        'writer.Write(sql)
        'writer.Close()

        Debug.Print(sql)

        records_affected = command.ExecuteNonQuery()
    End Sub

    Public Sub ExecuteCommandFromFile(ByVal fileName As String) Implements IDatabase.ExecuteCommandFromFile
        OpenConnection()
        Dim reader As New IO.StreamReader(fileName)
        command.CommandText = reader.ReadToEnd
        reader.Close()
        reader = Nothing
        records_affected = command.ExecuteNonQuery()
    End Sub

    Public Function ExecuteReaderFromFile(ByVal filename As String) As Data.IDataReader Implements IDatabase.ExecuteReaderFromFile
        OpenConnection()
        command.CommandText = IO.File.ReadAllText(filename)
        Return command.ExecuteReader()
    End Function


    Public Sub ExecuteCommand(ByVal sql As String, ByVal Transaction As Object) Implements IDatabase.ExecuteCommand
        command.CommandText = sql
        OpenConnection()
        If Not Transaction Is Nothing Then command.Transaction = Transaction
        records_affected = command.ExecuteNonQuery()
    End Sub

    Public Function ExecuteQuery(ByVal sql As String, ByVal DataTableName As String) As DataTable Implements IDatabase.ExecuteQuery
        command.CommandText = sql
        OpenConnection()

        Dim dt As New System.Data.DataTable

        If dataadapter Is Nothing Then dataadapter = New SqlClient.SqlDataAdapter
        Dim adapter As SqlClient.SqlDataAdapter = dataadapter
        adapter.SelectCommand = command
        adapter.Fill(dt)
        Return dt
    End Function


    Public ReadOnly Property RecordsAffected As Integer Implements IDatabase.RecordsAffected
        Get
            Return records_affected
        End Get
    End Property

    Public Overridable Sub RefreshDB() Implements IDatabase.RefreshDB
        If connection.State = ConnectionState.Closed Then
            Try
                connection.Open()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Try
            If connection.State = ConnectionState.Open Then connection.Close()
        Catch ex As Exception
            'die silently...
        End Try

        MyBase.Finalize()
    End Sub

    Sub Close() Implements IDatabase.Close
        Try
            connection.Close()
        Catch ex As Exception

        End Try

    End Sub


End Class

