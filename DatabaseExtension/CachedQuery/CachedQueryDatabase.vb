Imports System.Data

Public MustInherit Class CachedQueryDatabase
    Implements IDatabase

    Protected connection As System.Data.Common.DbConnection
    Protected dataadapter As System.Data.Common.DataAdapter
    Protected dataset As System.Data.DataSet
    Protected records_affected As Integer
    Protected query_cache As Dictionary(Of String, CachedQuery)
    Protected _last_reader As IDataReader

    Protected MustOverride Sub createConcreteDataAdapter()
    Protected MustOverride Sub createConcreteConnection()
    Protected MustOverride Sub setAdapterCommand(ByVal cmd As CachedQuery)
    Protected MustOverride Function createConcreteCachedQuery(ByVal FileName As String) As CachedQuery
    Protected MustOverride Function createConcreteCachedQuery() As CachedQuery

    Public Sub Close() Implements IDatabase.Close
        If Not connection Is Nothing Then connection.Close()
    End Sub

    Private Sub ExecuteCommand(ByVal sql As String) Implements IDatabase.ExecuteCommand
    End Sub

    Private Sub ExecuteCommand(ByVal sql As String, ByVal Transaction As Object) Implements IDatabase.ExecuteCommand
    End Sub

    Private Sub ExecuteCommandFromFile(ByVal fileName As String) Implements IDatabase.ExecuteCommandFromFile
    End Sub

    Private Function ExecuteQuery(ByVal sql As String, ByVal DataTableName As String) As System.Data.DataTable Implements IDatabase.ExecuteQuery
    End Function

    Private Function ExecuteReader(ByVal sql As String) As System.Data.IDataReader Implements IDatabase.ExecuteReader
    End Function

    Private Function ExecuteReaderFromFile(ByVal filename As String) As System.Data.IDataReader Implements IDatabase.ExecuteReaderFromFile
    End Function

    ''' <summary>
    ''' Prepares for a database operation
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Prepare()
        If connection Is Nothing Then createConcreteConnection()
        If connection Is Nothing Then Throw New Exception("Specific connection type not set in superclass of CachedQueryDatabase")
        If connection.State <> ConnectionState.Open Then connection.Open()
        If Not _last_reader Is Nothing Then
            _last_reader.Close()
        End If
        If query_cache Is Nothing Then query_cache = New Dictionary(Of String, CachedQuery)
    End Sub

    ''' <summary>
    ''' Loads a saved query from a file and populates a command with command parameters from the file
    ''' </summary>
    ''' <param name="QueryFileName"></param>
    ''' <remarks></remarks>
    Sub OpenQuery(ByVal QueryFileName As String, ByVal QueryName As String)
        Prepare()
        If Not query_cache.ContainsKey(QueryName) Then
            Dim x As CachedQuery = createConcreteCachedQuery(QueryFileName)
            x.Connection = connection
            query_cache.Add(QueryName, x)
        End If
    End Sub


    ''' <summary>
    ''' Opens a file containing multiple queries and extracts the queries from the file
    ''' Queries are denoted by quasi xml barckets.
    ''' </summary>
    ''' <param name="QueryFileName"></param>
    ''' <remarks></remarks>
    Sub OpenQuery(ByVal QueryFileName As String)
        Dim reader As New IO.StreamReader(QueryFileName)
        Dim line As String
        Dim query_sql As String = ""
        Dim query_name As String = ""
        Dim name As String = ""
        Dim x As CachedQuery
        Prepare()
        While Not reader.EndOfStream
            line = reader.ReadLine.Trim
            Dim tmp As String = line
            If tmp.ToLower().StartsWith("--<query name=""") Then
                query_sql = ""

                Dim r As New System.Text.RegularExpressions.Regex("\""[a-zA-Z0-9]*\""")
                Dim matches As System.Text.RegularExpressions.MatchCollection = r.Matches(line)
                If matches.Count = 1 Then name = line.Substring(matches(0).Index + 1, matches(0).Length - 2)
                r = Nothing
            ElseIf tmp.ToLower().StartsWith("--</query>") Then
                If line.Length > 0 Then
                    If Not query_cache.ContainsKey(name) Then
                        x = createConcreteCachedQuery()
                        x.Connection = connection
                        x.CommandText = query_sql
                        query_cache.Add(name, x)
                    End If
                End If
            ElseIf (tmp.StartsWith("--")) Then
                'ignore...
            Else
                query_sql += (line + Environment.NewLine)
            End If
        End While




    End Sub


    Overridable Function GetDataTable(ByVal QueryName As String) As System.Data.DataTable
        If query_cache.ContainsKey(QueryName) Then
            Prepare()
            If dataset Is Nothing Then dataset = New DataSet()
            If dataadapter Is Nothing Then createConcreteDataAdapter()
            If dataadapter Is Nothing Then Throw New Exception("Concrete data adapter type not set in superclass of CachedQueryDatabase")
            Dim cmd As Data.IDbCommand = query_cache(QueryName)
            Dim dt As New System.Data.DataTable
            setAdapterCommand(cmd)   'have to do this so that we can have a SQLDataAdpter.Fill(dataset)
            dataadapter.Fill(dataset)
            Return dt
        Else
            Throw New Exception("Query not found")
        End If
    End Function

    Function GetDataReader(ByVal QueryName As String) As IDataReader
        If query_cache.ContainsKey(QueryName) Then
            Prepare()
            Dim icmd As IDbCommand = query_cache(QueryName)
            _last_reader = query_cache(QueryName).ExecuteReader()
            Return _last_reader
        Else
            Throw New Exception("Query '" + QueryName + "' does not exist or has not been added to the CachedQueryDatabase")
        End If
    End Function

    Sub UpdateDatabase(ByVal QueryName As String)
        If query_cache.ContainsKey(QueryName) Then
            Prepare()
            query_cache(QueryName).ExecuteNonQuery()
        End If
    End Sub

    ReadOnly Property Queries() As IDictionary(Of String, CachedQuery)
        Get
            Return query_cache
        End Get
    End Property

    Private Sub OpenConnection() Implements IDatabase.OpenConnection
    End Sub

    Public ReadOnly Property RecordsAffected As Integer Implements IDatabase.RecordsAffected
        Get
            Return records_affected
        End Get
    End Property

    Private Sub RefreshDB() Implements IDatabase.RefreshDB
    End Sub

    Protected Overrides Sub Finalize()
        Try
            If connection.State = ConnectionState.Open Then connection.Close()
            connection = Nothing

            query_cache = Nothing
        Catch ex As Exception
        End Try
        MyBase.Finalize()
    End Sub
End Class
