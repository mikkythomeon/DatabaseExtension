
Public MustInherit Class DataObjectCollection
    Protected MustOverride Function createConcreteObject() As IDataObject
    Protected MustOverride Sub populateObject(ByVal rdr As SqlClient.SqlDataReader, ByVal obj As IDataObject)
    Public MustOverride Sub Initialize(ByVal Query As ObjectQuery)

    Protected _objects As Dictionary(Of String, IDataObject)
    Protected _query As ObjectQuery
    Protected _object_list As List(Of IDataObject)

    Sub New()
        _objects = New Dictionary(Of String, IDataObject)
    End Sub

    Public Function getObject(ByVal Key As String) As IDataObject
        Return _objects(Key)
    End Function

    Protected Sub addObject(ByVal obj As IDataObject)
        _objects.Add(obj.ID, obj)
    End Sub

    Public Function createObject() As IDataObject
        'create a new concrete object with default parameters
        Dim obj As IDataObject = createConcreteObject()  'eg: return new Part()

        'create a new guid to identify the object in the collection
        obj.ID = System.Guid.NewGuid().ToString
        obj.IsNew = True
        obj.Parent = Me
        Return obj
    End Function

    Public Sub saveObject(ByVal obj As IDataObject)
        Dim sql As String
        If obj.IsNew Then
            'save as new object
            sql = _query.InsertCommand
            For Each k As KeyValuePair(Of String, String) In _query.Parameters
                sql = sql.Replace(k.Key, k.Value)
            Next
            'if any key violations occur, then they will occur here. Let the db handle the key violations
            'and do not allow the requested save on the object
            _query.ExecuteQuery(sql)

            'error will be thrown on previous line if duplication if found in db.
            addObject(obj)
        Else
            'update object
            sql = _query.UpdateCommand
            For Each k As KeyValuePair(Of String, String) In _query.Parameters
                sql = sql.Replace(k.Key, k.Value)
            Next

            'if any key violations occur, then they will occur here. Let the db handle the key violations
            'and do not allow the requested save on the object
            _query.ExecuteQuery(sql)
        End If
        '...so now we can add the object to the collection if no errors have occurrent
        obj.IsDirty = False
        obj.IsNew = False
    End Sub

    Public Sub deleteObject(ByRef obj As IDataObject)
        Dim sql As String = _query.SelectCommand

        For Each k As KeyValuePair(Of String, String) In _query.Parameters
            sql = sql.Replace(k.Key, k.Value)
        Next

        'todo: execute command here against the database
        _query.ExecuteQuery(sql)


        _objects.Remove(obj.ID)
        obj = Nothing
    End Sub

    Public Sub loadAllObjects()
        Dim sql As String = _query.SelectCommand

        'we have to pass a datarreader to a superclass get the superclass to populate the object
        Dim rdr As SqlClient.SqlDataReader = _query.ExecuteQuery(sql)

        Dim obj As IDataObject
        While rdr.Read
            obj = createObject()
            addObject(obj)
            populateObject(rdr, obj)
        End While

        rdr.Close()
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of IDataObject)
        _object_list = Nothing
        _object_list = _objects.Values.ToList
        Return _object_list.GetEnumerator
    End Function



End Class
