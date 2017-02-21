Public Class ObjectCollectionTest

    Sub New()
        'SimpleObjectCollectionTest()
        SSMSObjectCollectionTest()  'test query generated from SSMS
    End Sub

    Sub SimpleObjectCollectionTest()
        'Dim db As New SQLDatabase("sa", "admin", "DFEZA30", "KanbanMT")
        Dim db As New SQLDatabase(".\sqlexpress", "KanbanMT")
        Dim x As New ObjectQuery(db)
        Dim p2c As New Part2Collection()

        'associate query with collection
        p2c.Initialize(x)

        'load all parts into the collection
        p2c.loadAllObjects()

        'test enumeration of all parts
        Try
            Dim e As IEnumerator(Of IDataObject) = p2c.GetEnumerator()
            While e.MoveNext
                Dim p2 As Part2 = e.Current
                Debug.Print(p2.PartNo + "," + p2.Description + "," + p2.UOM + "," + p2.IsActive.ToString + "," + p2.ID)
            End While
        Catch ex As Exception

            Debug.Print(ex.Message)

        End Try

    End Sub

    Sub SSMSObjectCollectionTest()
        Dim db As New SQLDatabase(".\sqlexpress", "KanbanMT")
        Dim x As New SSMSObjectQuery(db)
        Dim p2c As New Part2Collection2()

        'associate query with collection
        p2c.Initialize(x)

        'load all parts into the collection
        p2c.loadAllObjects()

        'test enumeration of all parts
        Dim e As IEnumerator(Of IDataObject) = p2c.GetEnumerator()
        While e.MoveNext
            Dim p2 As Part2 = e.Current
            Debug.Print(p2.PartNo + "," + p2.Description + "," + p2.UOM + "," + p2.IsActive.ToString + "," + p2.ID)
        End While
       
        'problem: how to map properties of object to the query parameters....??????
        'did this in the Part2 class, but not ideal...still a lot of manual work.
        Dim p As Part2 = p2c.createObject()
        p.Description = "TEST"
        p.PartNo = "999999"
        p.UOM = "PCE"
        p.IsActive = False
        p2c.saveObject(p)


    End Sub
End Class


Public Class Part2
    Inherits DataObject
    Private _description As String
    Private _part_no As String
    Private _uom As String
    Private _is_Active As Boolean

    Sub New(ByVal query As ObjectQuery)
        MyBase.New(query)

    End Sub

    Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
            _query.Parameters("@Description") = value
        End Set
    End Property

    Property PartNo As String
        Get
            Return _part_no
        End Get
        Set(ByVal value As String)
            _part_no = value
            _query.Parameters("@PartNo") = value
        End Set
    End Property

    Property UOM As String
        Get
            Return _uom
        End Get
        Set(ByVal value As String)
            _uom = value
            _query.Parameters("@Uom") = value
        End Set
    End Property

    Property IsActive As Boolean
        Get
            Return _is_Active
        End Get
        Set(ByVal value As Boolean)
            _is_Active = value
            If _is_Active Then
                _query.Parameters("@IsActive") = "1"
            Else
                _query.Parameters("@IsActive") = "0"
            End If
        End Set
    End Property

End Class


Public Class Part2Collection
    Inherits DataObjectCollection

    Protected Overrides Function createConcreteObject() As IDataObject
        Dim p2 As New Part2(_query)
        Return p2
    End Function

    Protected Overrides Sub populateObject(ByVal rdr As System.Data.SqlClient.SqlDataReader, ByVal obj As IDataObject)
        Dim p2 As Part2 = obj
        p2.Description = rdr("Description")
        p2.IsActive = rdr("IsActive")
        p2.PartNo = rdr("PartNo")
        p2.UOM = rdr("Uom")
    End Sub

    Public Overrides Sub Initialize(ByVal Query As ObjectQuery)
        
        _query = Query
        _query.SelectCommand = "SELECT * FROM Part"
        _query.InsertCommand = "INSERT INTO Part(PartNo, Description,Uom,IsActive) values('@PartNo','@Description','@Uom',@IsActive)"
        _query.UpdateCommand = "UPDATE Parts set Description='@Description',Uom='@Uom',IsActive=@IsActive where PartNo='@PartNo'"
    End Sub

End Class

Public Class Part2Collection2
    Inherits DataObjectCollection

    Protected Overrides Function createConcreteObject() As IDataObject
        Dim p2 As New Part2(_query)
        Return p2
    End Function

    Protected Overrides Sub populateObject(ByVal rdr As System.Data.SqlClient.SqlDataReader, ByVal obj As IDataObject)
        Dim p2 As Part2 = obj
        p2.Description = rdr("Description")
        p2.IsActive = rdr("IsActive")
        p2.PartNo = rdr("PartNo")
        p2.UOM = rdr("Uom")
    End Sub

    Public Overrides Sub Initialize(ByVal Query As ObjectQuery)
        _query = Query
        _query.SelectCommand = "SELECT * FROM Part"
        _query.InsertCommand = "INSERT INTO [dbo].[Part] " & Environment.NewLine & _
                    "           ([PartNo] " & Environment.NewLine & _
                    "           ,[Description] " & Environment.NewLine & _
                    "           ,[Uom] " & Environment.NewLine & _
                    "           ,[IsActive]) " & Environment.NewLine & _
                    "       VALUES " & Environment.NewLine & _
                    "           (<PartNo, varchar(30),> " & Environment.NewLine & _
                    "           ,<Description, varchar(40),> " & Environment.NewLine & _
                    "           ,<Uom, varchar(10),> " & Environment.NewLine & _
                    "           ,<IsActive, bit,>) "
    End Sub
  


End Class