Imports System
Imports System.IO
Imports System.Xml

Public Class ConfigurationSettings

    Private _asData As AppSetting

    Public Property AppSettings() As AppSetting
        Get
            Return _asData
        End Get
        Set(ByVal value As AppSetting)
            _asData = value
        End Set
    End Property

    Public Sub New(ByVal strFilename As String)
        If File.Exists(strFilename) Then
            Dim xdData As New XmlDocument()

            xdData.Load(strFilename)
            _asData = AppSetting.InstantiateSingletonInstance(xdData)
        Else
            Throw New Exception("The file could not be located")
        End If
    End Sub

End Class
