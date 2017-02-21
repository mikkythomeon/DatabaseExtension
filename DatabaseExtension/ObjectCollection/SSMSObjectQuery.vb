
''' <summary>
''' Accepts a query generated from SSMS
''' </summary>
Public Class SSMSObjectQuery
    Inherits ObjectQuery

    Sub New(ByVal db As DatabaseExtension.Database)
        MyBase.New(db)
    End Sub

    Public Overrides Property DeleteCommand As String
        Get
            Return MyBase.DeleteCommand
        End Get
        Set(ByVal value As String)
            MyBase.DeleteCommand = value
        End Set
    End Property

    Public Overrides Property InsertCommand As String
        Get
            Return MyBase.InsertCommand
        End Get
        Set(ByVal value As String)

            'parse the SSMS generated query and convert it to the format the the ObjectQuery expects
            'input: 


            'INSERT INTO [dbo].[Parts]
            '           ([PartNo]
            '           ,[Description]
            '           ,[Uom]
            '           ,[IsActive])
            '            VALUES
            '           (<PartNo, varchar(30),>
            '           ,<Description, varchar(40),>
            '           ,<Uom, varchar(10),>
            '           ,<IsActive, bit,>)
            'GO

            'output(step1) :
            'INSERT INTO [dbo].[Parts]
            '           ([PartNo]
            '           ,[Description]
            '           ,[Uom]
            '           ,[IsActive])
            '            VALUES
            '           (,@PartNo,varchar(30),
            '           ,@Description,varchar(40),
            '           ,@Uom,varchar(10),
            '           ,@IsActive,bit,)
            'GO

            'replace '(<' with '(,<'
            'replace ',>' with ''
            'replace ', ' with ' '  
            Dim sql As String = value.Replace("(<", "(,<")
            sql = sql.Replace("<", "@")
            sql = sql.Replace(",>", ",")
            sql = sql.Replace(", ", ",")


            'output(step2 :
            'INSERT INTO [dbo].[Parts]
            '           ([PartNo]
            '           ,[Description]
            '           ,[Uom]
            '           ,[IsActive])
            '            VALUES
            '           ('@PartNo'
            '           ,'@Description'
            '           ,'@Uom'
            '           ,@IsActive)
            'GO

            'now we have to parse the datatype to insert the apostrohpe in each line...
            'if line contains char, varchar, date, datetime, or guid, then use apostrophes
            Dim lines() As String = sql.Split(Environment.NewLine)
            Dim start As Boolean
            Dim _end As Boolean
            Dim line As String
            Dim nl As String = ""
            Dim fields() As String
            Dim use_apostrophe As Boolean
            For Each l As String In lines
                line = l.Trim
                If line.StartsWith("(,@") Then
                    start = True
                    nl += "("
                End If

                If start Then
                    If line.EndsWith(")") Then
                        _end = True
                    End If
                End If

                If start Then

                    fields = line.Split(",")
                    use_apostrophe = fields(2).ToLower.Contains("char") Or fields(2).Contains("date")

                    If use_apostrophe Then
                        nl += "'" + fields(1) + "'"
                    Else
                        nl += fields(1)
                    End If

                    If _end Then
                        nl += ")"
                    Else
                        nl += ","
                    End If
                Else
                    nl += line
                End If
            Next


            MyBase.InsertCommand = nl
        End Set
    End Property

    Public Overrides Property SelectCommand As String
        Get
            Return MyBase.SelectCommand
        End Get
        Set(ByVal value As String)
            MyBase.SelectCommand = value
        End Set
    End Property

    Public Overrides Property UpdateCommand As String
        Get
            Return MyBase.UpdateCommand
        End Get
        Set(ByVal value As String)
            'parse the SSMS generated query and convert it to the format the the ObjectQuery expects
            'input: 

            'UPDATE([dbo].[Parts])
            '   SET [PartNo] = <PartNo, varchar(30),>
            '      ,[Description] = <Description, varchar(40),>
            '      ,[Uom] = <Uom, varchar(10),>
            '      ,[IsActive] = <IsActive, bit,>
            ' WHERE <Search Conditions,,>
            'GO

            'output:

            'UPDATE([dbo].[Parts])
            '   SET [PartNo] = <PartNo, varchar(30),>
            '      ,[Description] = <Description, varchar(40),>
            '      ,[Uom] = <Uom, varchar(10),>
            '      ,[IsActive] = <IsActive, bit,>
            ' WHERE <Search Conditions,,>
            'GO





            MyBase.UpdateCommand = value
        End Set
    End Property


End Class
