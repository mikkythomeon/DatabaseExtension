
Public Class ObjectQuery
    Private _parameters As Dictionary(Of String, String)
    Private _select_command As String
    Private _insert_command As String
    Private _delete_command As String
    Private _update_command As String
    Private _db As Database

    Sub New(ByVal db As Database)
        _parameters = New Dictionary(Of String, String)
        _db = db
    End Sub

    Private Sub populateParamsFromCommandText(ByVal text As String)
        Dim r As New System.Text.RegularExpressions.Regex("@[A-Za-z0-9]+")
        Dim match_collection As System.Text.RegularExpressions.MatchCollection = r.Matches(text)
        For Each match As System.Text.RegularExpressions.Match In match_collection
            Dim param_name As String = match.Value.Replace("'", "")
            If Not _parameters.ContainsKey(param_name) Then
                _parameters.Add(param_name.Trim, Nothing)      'this is a fudge to stop the insertion of the SQL ' character
            End If
        Next
    End Sub

    Friend Function ExecuteQuery(ByVal SQL As String) As IDataReader
        Return _db.ExecuteReader(SQL)
    End Function

    Public Overridable Property SelectCommand As String
        Get
            Return _select_command
        End Get
        Set(ByVal value As String)
            _select_command = value
            populateParamsFromCommandText(value)
        End Set
    End Property

    Public Overridable Property UpdateCommand As String
        Get
            Return _update_command
        End Get
        Set(ByVal value As String)
            _update_command = value
            populateParamsFromCommandText(value)
        End Set
    End Property

    Public Overridable Property InsertCommand As String
        Get
            Return _insert_command
        End Get
        Set(ByVal value As String)
            _insert_command = value
            populateParamsFromCommandText(value)
        End Set
    End Property

    ReadOnly Property Parameters As Dictionary(Of String, String)
        Get
            Return _parameters
        End Get
    End Property

    Public Overridable Property DeleteCommand As String
        Get
            Return _delete_command
        End Get
        Set(ByVal value As String)
            _delete_command = value
            populateParamsFromCommandText(value)
        End Set
    End Property

End Class
