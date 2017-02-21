Imports System.Data.SqlClient


Public MustInherit Class CachedQuery
    'Inherits Data.Common.DbCommand
    Implements IDbCommand

    Protected _cmd As SqlCommand
    Private _params As Dictionary(Of String, Object)
    Protected _filename As String
    Protected _original_command_text As String

    MustOverride Sub createConcreteCommand()

    Sub New(ByVal SqlFileName As String)
        _params = New Dictionary(Of String, Object)

        Dim reader As New IO.StreamReader(SqlFileName)
        Me.CommandText = reader.ReadToEnd()
        reader.Close()
        createConcreteCommand()
        _cmd.Connection = Connection
    End Sub

    Sub New()
        _params = New Dictionary(Of String, Object)
        createConcreteCommand()
        _cmd.Connection = Connection
    End Sub

    'instead of returning the base parameter type, we use our own parameter type
    'this is because the sql database checks the database parameter type at run time and thows an erro.
    Function GetParameter(ByVal Name As String) As KeyValuePair(Of String, Object)
        Return _params(Name)
    End Function

    Sub SetParameter(ByVal Name As String, ByVal Value As Object)
        _params(Name) = Value
    End Sub

    Private Sub Cancel() Implements IDbCommand.Cancel
        _cmd.Cancel()
    End Sub

    Public Property CommandText As String Implements IDbCommand.CommandText
        Get
            Return _original_command_text
        End Get
        Set(ByVal value As String)
            _original_command_text = value
            Dim r As New System.Text.RegularExpressions.Regex("@[A-Za-z0-9]*")
            Dim match_collection As System.Text.RegularExpressions.MatchCollection = r.Matches(_original_command_text)
            For Each match As System.Text.RegularExpressions.Match In match_collection
                Dim param_name As String = match.Value.Replace("'", "")
                If Not _params.ContainsKey(param_name) Then
                    _params.Add(param_name.Trim, Nothing)      'this is a fudge to stop the insertion of the SQL ' character
                End If
            Next
        End Set
    End Property

    Public Property CommandTimeout As Integer Implements IDbCommand.CommandTimeout
        Get
            Return _cmd.CommandTimeout
        End Get
        Set(ByVal value As Integer)
            _cmd.CommandTimeout = value
        End Set
    End Property

    Public Property CommandType As System.Data.CommandType Implements IDbCommand.CommandType
        Get
            Return _cmd.CommandType
        End Get
        Set(ByVal value As System.Data.CommandType)
            _cmd.CommandType = value
        End Set
    End Property

    Protected Function CreateDbParameter() As System.Data.Common.DbParameter
        Return _cmd.CreateParameter()
    End Function

    Protected Property DbConnection As System.Data.Common.DbConnection
        Get
            Return _cmd.Connection
        End Get
        Set(ByVal value As System.Data.Common.DbConnection)
            _cmd.Connection = value
        End Set
    End Property

    Protected ReadOnly Property DbParameterCollection As System.Data.Common.DbParameterCollection
        Get
            Return _cmd.Parameters
        End Get
    End Property

    Protected Property DbTransaction As System.Data.Common.DbTransaction
        Get
            Return _cmd.Transaction
        End Get
        Set(ByVal value As System.Data.Common.DbTransaction)
            _cmd.Transaction = value
        End Set
    End Property

    Private Sub replaceParams()
        Dim sql As String = _original_command_text
        For Each item As KeyValuePair(Of String, Object) In _params
            sql = sql.Replace(item.Key, item.Value.ToString)
        Next
        _cmd.CommandText = sql
    End Sub

    Protected Function ExecuteDbDataReader(ByVal behavior As System.Data.CommandBehavior) As System.Data.Common.DbDataReader
        replaceParams()
        Return _cmd.ExecuteReader(behavior)
    End Function

    Public Function ExecuteNonQuery() As Integer Implements System.Data.IDbCommand.ExecuteNonQuery
        replaceParams()
        Return _cmd.ExecuteNonQuery()
    End Function

    Private Function ExecuteScalar() As Object
    End Function

    Private Property UpdatedRowSource As System.Data.UpdateRowSource
        Get
            Return _cmd.UpdatedRowSource
        End Get
        Set(ByVal value As System.Data.UpdateRowSource)
            _cmd.UpdatedRowSource = value
        End Set
    End Property

    Public Property Connection As System.Data.IDbConnection Implements System.Data.IDbCommand.Connection
        Get
            Return _cmd.Connection
        End Get
        Set(ByVal value As System.Data.IDbConnection)
            _cmd.Connection = value
        End Set
    End Property

    Private Function CreateParameter() As System.Data.IDbDataParameter Implements System.Data.IDbCommand.CreateParameter
    End Function


    Function ExecuteReader() As System.Data.IDataReader Implements System.Data.IDbCommand.ExecuteReader
        replaceParams()
        Return _cmd.ExecuteReader
    End Function

    Private Function ExecuteReader(ByVal behavior As System.Data.CommandBehavior) As System.Data.IDataReader Implements System.Data.IDbCommand.ExecuteReader

    End Function

    Private ReadOnly Property Parameters As System.Data.IDataParameterCollection Implements System.Data.IDbCommand.Parameters
        Get
            Return _cmd.Parameters
        End Get
    End Property

    Private Sub Prepare1() Implements System.Data.IDbCommand.Prepare
        _cmd.Prepare()
    End Sub

    Private Property Transaction As System.Data.IDbTransaction Implements System.Data.IDbCommand.Transaction
        Get
            Return _cmd.Transaction
        End Get
        Set(ByVal value As System.Data.IDbTransaction)
            _cmd.Transaction = value
        End Set
    End Property

    Private Property UpdatedRowSource1 As System.Data.UpdateRowSource Implements System.Data.IDbCommand.UpdatedRowSource
        Get
            Return _cmd.UpdatedRowSource
        End Get
        Set(ByVal value As System.Data.UpdateRowSource)
            _cmd.UpdatedRowSource = value
        End Set
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Private Function ExecuteScalar1() As Object Implements System.Data.IDbCommand.ExecuteScalar
        Return _cmd.ExecuteScalar()
    End Function
End Class
