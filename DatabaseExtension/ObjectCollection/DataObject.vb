
Public Class DataObject
    Implements IDataObject

    Private _id As String
    Private _is_dirty As Boolean
    Private _is_new As Boolean
    Protected _parent As DataObjectCollection
    Protected _query As ObjectQuery

    Sub New(ByVal Query As ObjectQuery)
        _query = Query
    End Sub

    Public Property ID As String Implements IDataObject.ID
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

    Friend Property IsDirty As Boolean Implements IDataObject.IsDirty
        Get
            Return _is_dirty
        End Get
        Set(ByVal value As Boolean)
            _is_dirty = value
        End Set
    End Property

    Friend Property IsNew As Boolean Implements IDataObject.IsNew
        Get
            Return _is_new
        End Get
        Set(ByVal value As Boolean)
            _is_new = value
        End Set
    End Property

    Public Property Parent As DataObjectCollection Implements IDataObject.Parent
        Get
            Return _parent
        End Get
        Set(ByVal value As DataObjectCollection)
            _parent = value
        End Set
    End Property
End Class
