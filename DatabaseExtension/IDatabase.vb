Public Interface IDatabase

    Sub OpenConnection()
    Function ExecuteReader(ByVal sql As String) As Data.IDataReader
    Sub ExecuteCommand(ByVal sql As String)
    Sub ExecuteCommandFromFile(ByVal fileName As String)
    Function ExecuteReaderFromFile(ByVal filename As String) As Data.IDataReader
    Sub ExecuteCommand(ByVal sql As String, ByVal Transaction As Object)
    Function ExecuteQuery(ByVal sql As String, ByVal DataTableName As String) As DataTable
    ReadOnly Property RecordsAffected As Integer
    Sub RefreshDB()
  
    Sub Close()
End Interface
