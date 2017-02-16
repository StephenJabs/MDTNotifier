Imports System
Imports System.IO
Imports System.Linq
Imports System.Net.Mail
Imports System.Text
Imports System.Text.RegularExpressions


Namespace MDTNotifier

    Module modMain
        Private _strDomain As String
        Private _strMailDomain As String
        Private _strMailServer As String
        Private _strPassword As String
        Private _strRecipient As String
        Private _strUsername As String

        Private Function GetSender(strMachineName As String) As String
            Dim strSender As String = strMachineName.Substring(0, strMachineName.IndexOf(".")) & "@" & _strMailDomain

            Return strSender
        End Function

        Private Function SendEmail(strRecipient As String, strFrom As String, strSubject As String, strBody As String) As Integer
            Dim intError As Integer
            Dim mmData As New MailMessage()

            Try
                Dim scServer As SmtpClient = New SmtpClient(_strMailServer)

                mmData.From = New MailAddress(strFrom)
                mmData.To.Add(New MailAddress(strRecipient))
                mmData.Subject = strSubject
                mmData.Body = strBody
                mmData.IsBodyHtml = True

                scServer.Send(mmData)

                mmData.Dispose()
                intError = 0
            Catch ex As Exception
                mmData.Dispose()
                intError = ex.HResult
            End Try

            Return intError
        End Function

        Private Sub SendErrorMessage(strMessage As String, strSender As String)
            Dim reData As Regex = New Regex("Error logged for computer (.*?): (.*)")
            Dim mData As Match = reData.Match(strMessage)
            Dim sbBody As StringBuilder = New StringBuilder()
            Dim strComputer As String
            Dim strError As String

            If mData.Success Then
                strComputer = mData.Groups(1).Value
                strError = mData.Groups(2).Value

                sbBody.AppendLine("<html>")
                sbBody.AppendLine("<head>")
                sbBody.AppendLine("    <style type=""text/css"">")
                sbBody.AppendLine("    p {font-family: calibri; color:black; font-size:11pt;}")
                sbBody.AppendLine("    </style>")
                sbBody.AppendLine("</head>")
                sbBody.AppendLine("<body>")
                sbBody.AppendLine(String.Format("    <p>The computer {0} encountered an error during deployment:</p>", strComputer))
                sbBody.AppendLine(String.Format("    <p>{0}</p>", strError))
                sbBody.AppendLine("</body>")
                sbBody.AppendLine("</html>")

                SendEmail(_strRecipient, strSender, String.Format("{0}: MDT Deployment Error", strComputer), sbBody.ToString)
            End If
        End Sub

        Private Sub SendStartMessage(strMessage As String, strSender As String)
            Dim reData As Regex = New Regex("Deployment started for computer (.*?)\.")
            Dim mData As Match = reData.Match(strMessage)
            Dim sbBody As StringBuilder = New StringBuilder()
            Dim strComputer As String

            If mData.Success Then
                strComputer = mData.Groups(1).Value

                sbBody.AppendLine("<html>")
                sbBody.AppendLine("<head>")
                sbBody.AppendLine("    <style type=""text/css"">")
                sbBody.AppendLine("    p {font-family: calibri; color:black; font-size:11pt;}")
                sbBody.AppendLine("    </style>")
                sbBody.AppendLine("</head>")
                sbBody.AppendLine("<body>")
                sbBody.AppendLine(String.Format("    <p>Deployment started for computer {0}.</p>", strComputer))
                sbBody.AppendLine("</body>")
                sbBody.AppendLine("</html>")

                SendEmail(_strRecipient, strSender, String.Format("{0}: MDT Deployment Started", strComputer), sbBody.ToString)
            End If
        End Sub

        Private Sub SendSuccessMessage(strMessage As String, strSender As String)
            Dim reData As Regex = New Regex("Deployment completed successfully for computer (.*?)\.")
            Dim mData As Match = reData.Match(strMessage)
            Dim sbBody As StringBuilder = New StringBuilder()
            Dim strComputer As String

            If mData.Success Then
                strComputer = mData.Groups(1).Value

                sbBody.AppendLine("<html>")
                sbBody.AppendLine("<head>")
                sbBody.AppendLine("    <style type=""text/css"">")
                sbBody.AppendLine("    p {font-family: calibri; color:black; font-size:11pt;}")
                sbBody.AppendLine("    </style>")
                sbBody.AppendLine("</head>")
                sbBody.AppendLine("<body>")
                sbBody.AppendLine(String.Format("    <p>Deployment completed successfully for computer {0}.</p>", strComputer))
                sbBody.AppendLine("</body>")
                sbBody.AppendLine("</html>")

                SendEmail(_strRecipient, strSender, String.Format("{0}: MDT Deployment Success", strComputer), sbBody.ToString)
            End If
        End Sub

        Private Sub SendWarningMessage(strMessage As String, strSender As String)
            Dim reData As Regex = New Regex("Warning logged for computer (.*?): (.*)")
            Dim mData As Match = reData.Match(strMessage)
            Dim sbBody As StringBuilder = New StringBuilder()
            Dim strComputer As String
            Dim strWarning As String

            If mData.Success Then
                strComputer = mData.Groups(1).Value
                strWarning = mData.Groups(2).Value

                sbBody.AppendLine("<html>")
                sbBody.AppendLine("<head>")
                sbBody.AppendLine("    <style type=""text/css"">")
                sbBody.AppendLine("    p {font-family: calibri; color:black; font-size:11pt;}")
                sbBody.AppendLine("    </style>")
                sbBody.AppendLine("</head>")
                sbBody.AppendLine("<body>")
                sbBody.AppendLine(String.Format("    <p>The computer {0} encountered a warning during deployment:</p>", strComputer))
                sbBody.AppendLine(String.Format("    <p>{0}</p>", strWarning))
                sbBody.AppendLine("</body>")
                sbBody.AppendLine("</html>")

                SendEmail(_strRecipient, strSender, String.Format("{0}: MDT Deployment Warning", strComputer), sbBody.ToString)
            End If
        End Sub

        Public Sub Main(ByVal Args As String())
            If Args.Length < 1 Then
                Console.Error.WriteLine("Usage:")
                Console.Error.WriteLine("MDTNotifier <path to web.config>")
                Return
            End If

            Dim strAppPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location())
            Dim csData As New ConfigurationSettings(Path.Combine(strAppPath, "web.config"))
            Dim datDate As DateTime = Today()

            _strDomain = csData.AppSettings("Domain")
            _strMailDomain = csData.AppSettings("MailDomain")
            _strMailServer = csData.AppSettings("MailServer")
            _strRecipient = csData.AppSettings("Recipient")

            Dim elApplication As EventLog = New EventLog("Application")
            Dim eleItems = From eleQuery As EventLogEntry In elApplication.Entries Where eleQuery.Source = "MDT_Monitor" And _
                           (eleQuery.InstanceId = 2 Or eleQuery.InstanceId = 3 Or eleQuery.InstanceId = 41015 Or eleQuery.InstanceId = 41016)
                           Order By eleQuery.TimeGenerated Descending Select eleQuery
            Dim eleItem As EventLogEntry

            For Each eleItem In eleItems
                Select Case eleItem.InstanceId
                    Case 2
                        SendWarningMessage(eleItem.Message, GetSender(eleItem.MachineName))
                    Case 3
                        If Not eleItem.Message.Contains("FAILURE ( 7813 ): False: Verify there are partitions defined in this Task Sequence Step.") AndAlso
                            Not eleItem.Message.Contains("returned an unexpected return code: 1641") Then
                            SendErrorMessage(eleItem.Message, GetSender(eleItem.MachineName))
                        End If
                    Case 41015
                        SendSuccessMessage(eleItem.Message, GetSender(eleItem.MachineName))
                    Case 41016
                        SendStartMessage(eleItem.Message, GetSender(eleItem.MachineName))
                End Select

                Exit For
            Next
        End Sub

    End Module

End Namespace
