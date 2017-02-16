Imports System
Imports System.IO
Imports System.Xml

Public Class AppSetting

    Private _xdData As XmlDocument
    Private Shared _singleton As AppSetting

    Default Public ReadOnly Property Item(ByVal SearchKey As String) As String
        Get
            Return TryCast(SearchXmlForKey(SearchKey), String)
        End Get
    End Property

    Private Sub New(ByVal xdData As XmlDocument)
        _xdData = xdData
    End Sub

    Public Shared Function InstantiateSingletonInstance(ByVal xdData As XmlDocument) As AppSetting
        If _singleton Is Nothing Then
            _singleton = New AppSetting(xdData)
        End If

        Return _singleton
    End Function

    Private Function SearchXmlForKey(ByVal SearchKey As String) As String
        Dim strAttributeValue As String = ""
        Dim strXPath As String = String.Format("/configuration/appSettings/add[@key='{0}']", SearchKey)
        Dim xnNode As XmlNode = _xdData.SelectSingleNode(strXPath)

        If Not xnNode Is Nothing AndAlso Not xnNode.Attributes("value") Is Nothing Then
            strAttributeValue = xnNode.Attributes("value").Value
        Else
            Throw New Exception(String.Format("Cannot find the {0} application setting", SearchKey))
        End If

        Return strAttributeValue
    End Function

End Class
