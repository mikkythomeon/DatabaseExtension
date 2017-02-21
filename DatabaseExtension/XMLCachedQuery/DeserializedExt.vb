Partial Public Class DataApplication

    Sub Initialize()


        Dim databases As New Dictionary(Of String, DataApplicationDatabase)
        For Each x As DataApplicationDatabase In Me.Database
            Dim cn As IDbConnection
            If x.UserName Is Nothing Then
                cn = New SqlClient.SqlConnection("Server=" + x.ServerAddress + ";Database=" + x.Database + ";Trusted_Connection=true;Connection Timeout=60;")
            Else
                cn = New SqlClient.SqlConnection("Server=" + x.ServerAddress + ";Database=" + x.Database + ";User Id=" + x.UserName + ";Password=" + x.Password + "; Connection Timeout=60;")
            End If

            x.Initialize(x.ServerAddress, cn)
            databases.Add(x.ID, x)
        Next


        Dim l As List(Of DataApplicationQuery) = Queries.ToList

        For Each q As DataApplicationQuery In l

            Try
                q.Initialize(q.Value.Trim, databases(q.DatabaseID))
            Catch ex As Exception
                Debug.Print("Not initd " + q.Name)
            End Try
        Next

    End Sub

    Sub Close()
        For Each x As DataApplicationDatabase In Database
            x.Close()
        Next
    End Sub

    Function GetDataTable(ByVal QueryName As String) As DataTable
        Dim q As DataApplicationQuery = Query(QueryName)
        Dim db As DataApplicationDatabase = (From x As DataApplicationDatabase In Database Where x.ID = q.DatabaseID Select x).First

        Dim rdr As SqlClient.SqlDataReader = db.ExecuteReader(q.Value)
        Dim dt As New DataTable(QueryName)
        dt.Load(rdr)
        rdr.Close()
        Return dt
    End Function

    ReadOnly Property Query(ByVal QueryName As String) As DataApplicationQuery
        Get
            Return (From x As DataApplicationQuery In Queries Where x.Name = QueryName Select x).First
        End Get
    End Property

End Class

Partial Public Class DataApplicationDatabase
    Inherits Database

    Sub Initialize(ByVal Server As String, ByVal cn As SqlClient.SqlConnection)
        MyBase.connection = cn
        MyBase.command = New System.Data.SqlClient.SqlCommand()
        MyBase.command.Connection = cn
    End Sub

End Class


Partial Public Class DataApplicationQuery
    Private _command_text As String
    Private _params As Dictionary(Of String, String)
    Private _cn As Database

    Sub Initialize(ByVal QueryText As String, ByVal connection As DataApplicationDatabase)
        _params = New Dictionary(Of String, String)
        _command_text = QueryText
        _cn = connection


        Dim r As New System.Text.RegularExpressions.Regex("@[A-Za-z0-9_]+")
        Dim match_collection As System.Text.RegularExpressions.MatchCollection = r.Matches(_command_text)
        For Each match As System.Text.RegularExpressions.Match In match_collection
            Dim param_name As String = match.Value.Replace("'", "").Trim()
            If Not _params.ContainsKey(param_name) Then
                _params.Add(param_name, "")      'this is a fudge to stop the insertion of the SQL ' character
            End If
        Next
    End Sub

    Sub SetParameter(ByVal Name As String, ByVal Value As String)
        If Not _params.ContainsKey(Name) Then Throw New Exception("Parameter '" + Name + "' not defined in query file")
        _params(Name) = Value
    End Sub

    Private Function replaceParams() As String
        Dim sql As String = _command_text
        For Each item As KeyValuePair(Of String, String) In _params
            sql = sql.Replace(item.Key, item.Value)
        Next
        Return sql
    End Function

    Function ExecuteReader() As IDataReader
        Return _cn.ExecuteReader(replaceParams)
    End Function

    Sub ExecuteNonQuery()
        _cn.ExecuteCommand(replaceParams)
    End Sub

End Class
