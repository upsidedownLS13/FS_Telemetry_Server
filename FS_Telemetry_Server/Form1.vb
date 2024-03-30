Imports System.Net
Imports System.Data.Common
Imports System.IO
Imports System.IO.Pipes
Imports System.IO.Ports

Imports System.Text
Imports System.Threading
Imports ClientServerUsingNamedPipes.Interfaces
Imports ClientServerUsingNamedPipes.Server.PipeServer
'Imports Newtonsoft.Json
Imports System.Globalization
Imports System.Windows.Forms.VisualStyles.VisualStyleElement





Public Class Form1

    Dim headerList As New List(Of String)
    Dim bodyList As New List(Of String)

    Dim cnt As ULong
    Dim cnt_reads As ULong

    WithEvents server As ClientServerUsingNamedPipes.Server.PipeServer

    Dim _strPipeIn As String
    Dim _headers() As String


    Dim pipeServerActive As Boolean
    Dim pipeServerConnecting As Boolean

    Dim objLock As ReaderWriterLock
    Dim objLockFilters As ReaderWriterLock
    Dim boSerialUpdateThreadStop As Boolean
    Dim thrdSerialUpdate As Threading.Thread
    Dim serialsLst As New List(Of IO.Ports.SerialPort)
    Dim portFilters As New Dictionary(Of String, List(Of String))
    Dim SerialReadBuffers As New Dictionary(Of String, String)
    Dim _JSON As String



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        server = New ClientServerUsingNamedPipes.Server.PipeServer
        AddHandler server.MessageReceivedEvent, AddressOf eventHandler

        server.Start()
        pipeServerActive = True
        pipeServerConnecting = True
        Call updateConnectStatus()
        'MsgBox("started")

        lblConnectStatus.Text = "Not connected"
        _JSON = "{}"
        Call startWebServer()

        objLock = New ReaderWriterLock()
        objLockFilters = New ReaderWriterLock()

        ttSnapShot.Start()
        TimerSerialUpdate.Start()

        Call SerialStartThread()

    End Sub


    Sub eventHandler(sender As Object, e As MessageReceivedEventArgs) 'Handles server.MessageReceivedEvent

        _strPipeIn = e.Message
        Call str2label()
        Call cntPlus()
        Call convert2JSON()

        Call processPipeMessage()
    End Sub

    Sub updateConnectStatus()
        If Not pipeServerActive Then
            lblConnectStatus.Text = "not connected"
        ElseIf pipeServerConnecting Then
            lblConnectStatus.Text = "connecting..."
        Else
            lblConnectStatus.Text = "...running :)"
        End If

    End Sub

    Private Sub processPipeMessage()
        Dim items() As String

        If pipeServerConnecting Then
            pipeServerConnecting = False
            Call updateConnectStatus()
        End If
        items = _strPipeIn.Split("<")

        If items(0).Equals("HEADER") Then
            Call processHeader(items)
        ElseIf items(0).Equals("BODY") Then
            Call processBody(items)
        Else
            MsgBox("illegal string: " & System.Environment.NewLine & _strPipeIn)
        End If

    End Sub

    Sub processHeader(ByRef items() As String)
        If items.Length > 1 Then
            headerList.Clear()
            '_headers = items.Clone() 'do I need this at all?

            For cntItems = 1 To items.Length - 1
                If items(cntItems).Length > 0 Then
                    headerList.Add(items(cntItems))
                End If
            Next
        End If
    End Sub

    Sub processBody(ByRef items() As String)
        If items.Length > 1 Then
            bodyList.Clear()
            '_headers = items.Clone() 'do I need this at all?

            For cntItems = 1 To items.Length - 1
                ' If items(cntItems).Length > 0 Then
                bodyList.Add(items(cntItems))
                'End If
            Next
        End If
    End Sub



    Private Sub btnShowStr_Click(sender As Object, e As EventArgs)
        str2label()
    End Sub

    Private Sub cntPlus()
        cnt_reads = cnt_reads + 1
        lblCntReads.Text = cnt_reads

    End Sub

    Private Sub str2label()
        txtOutput.Text = _strPipeIn
        Me.Update()
    End Sub

    Private Sub btnAsyncStop_Click(sender As Object, e As EventArgs) Handles btnPipeStop.Click 'changed from stop to restart
        If pipeServerActive Then
            server.Stop()
            pipeServerActive = False
            pipeServerConnecting = True
            Call updateConnectStatus()

            server.Start()
            pipeServerActive = True
            pipeServerConnecting = True
        End If
    End Sub

    Private Sub btnAsyncStart_Click(sender As Object, e As EventArgs)

        server = New ClientServerUsingNamedPipes.Server.PipeServer
        AddHandler server.MessageReceivedEvent, AddressOf eventHandler

        server.Start()
        pipeServerActive = True
        pipeServerConnecting = True
        updateConnectStatus()
        'MsgBox("started")

    End Sub

    Private Sub UpdateSnapshot()
        lstHeaders.Items.Clear()
        If headerList.Count > 0 Then
            For cnt = 0 To headerList.Count - 1
                lstHeaders.Items.Add(headerList.Item(cnt))
            Next
        End If

        lstValues.Items.Clear()

        If bodyList.Count > 0 Then
            For cnt = 0 To bodyList.Count - 1
                lstValues.Items.Add(bodyList.Item(cnt))
            Next
        End If

        Me.Update()
    End Sub


    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        boSerialUpdateThreadStop = True
        If pipeServerActive Then
            server.Stop()
            pipeServerActive = False
            pipeServerConnecting = False
            Call updateConnectStatus()
        End If

        Call btnDisconnectSerial_Click()

        Me.Close()
    End Sub



    Private Sub btnJSON_Click(sender As Object, e As EventArgs) Handles btnJSON.Click
        txtJSON.Text = GetJSON()
    End Sub

    Private Sub convert2JSON()
        Dim newStr As String
        newStr = "{" & System.Environment.NewLine

        SyncLock headerList
            SyncLock bodyList
                If headerList.Count < 1 Or bodyList.Count < 1 Then
                    Exit Sub
                End If

                For cnt = 0 To headerList.Count - 1
                    Dim splits() As String
                    splits = bodyList.Item(cnt).Split(">")

                    newStr &= """" & headerList.Item(cnt) & """: "
                    If splits.Count = 1 Then
                        If IsNumeric(splits(0)) Then
                            newStr &= splits(0)
                        Else
                            newStr &= """" & splits(0) & """"
                        End If

                    Else 'we have a vector to handle
                        newStr &= "["
                        For cnt2 = 0 To splits.Count - 2 'last one is always empty..

                            If IsNumeric(splits(cnt2)) Then
                                newStr &= splits(cnt2)
                            Else
                                newStr &= """" & splits(cnt2) & """"
                            End If




                            If cnt2 < splits.Count - 2 Then
                                newStr &= ","
                            End If
                        Next

                        newStr &= "]"
                    End If

                    If cnt < headerList.Count - 1 Then 'normal line
                        newStr &= ","
                    End If
                    newStr &= System.Environment.NewLine
                Next

                newStr &= "}"
            End SyncLock
        End SyncLock

        _JSON = String.Copy(newStr)

    End Sub

    Private Function GetJSON() As String
        Dim returnStr As String
        SyncLock _JSON
            returnStr = String.Copy(_JSON)
        End SyncLock
        Return returnStr
    End Function

    Private Sub startWebServer()
        Dim hostName As String = Environment.MachineName

        If Not HttpListener.IsSupported Then
            MsgBox("Unable to start web server.")
        Else

            Dim listener As New HttpListener
            'netsh http add urlacl url=http://+:25556/ user=Jeder
            'netsh advfirewall firewall add rule name= "Open Port L22 25556" dir=in action=allow protocol=TCP localport=25556

            'listener.Prefixes.Add("http://localhost:25556/")
            listener.Prefixes.Add("http://+:25556/")


            Try
                listener.Start()
            Catch ex As Exception
                MsgBox("Server failed to start. Please ensure that we are trying to listen on a valid network interface.")
                MsgBox(ex.Message)
                Return
            End Try

            Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                       Do
                                                           Dim response As HttpListenerResponse = listener.GetContext.Response
                                                           'Dim textResponse As Byte() = System.Text.Encoding.UTF8.GetBytes(($"Hello from {hostName} at "))
                                                           Dim textResponse As Byte() = System.Text.Encoding.UTF8.GetBytes(GetJSON())

                                                           response.ContentLength64 = textResponse.Length
                                                           response.OutputStream.Write(textResponse, 0, textResponse.Length)
                                                           response.OutputStream.Close()
                                                       Loop
                                                   End Sub)
        End If


    End Sub



    Private Sub ttSnapShot_Tick(sender As Object, e As EventArgs) Handles ttSnapShot.Tick
        Call UpdateSnapshot()
    End Sub




    Private Sub btnRefreshCOMports_Click(sender As Object, e As EventArgs) Handles TimerSerialUpdate.Tick
        'clstCOMports.Items.Clear()
        objLock.AcquireWriterLock(Timeout.Infinite)
        For Each portName As String In IO.Ports.SerialPort.GetPortNames()
            If Not clstCOMports.Items.Contains(portName) Then
                clstCOMports.Items.Add(portName)
            End If
        Next

        Dim found As Boolean
        Dim killList As New List(Of String)
        'For Each item As String In clstCOMports.Items
        For index = 0 To clstCOMports.Items.Count - 1
            Dim item As String
            item = clstCOMports.Items(index)
            found = False
            For Each portName As String In IO.Ports.SerialPort.GetPortNames()
                If String.Equals(portName, item) Then
                    found = True
                    Exit For
                End If
            Next
            If Not found Then
                'clstCOMports.Items.Remove(item)
                killList.Add(item)
            End If
        Next

        For Each port In killList
            'MsgBox(port)
            clstCOMports.Items.Remove(port)
        Next
        objLock.ReleaseWriterLock()
    End Sub



    Private Sub SerialConnect() Handles clstCOMports.SelectedValueChanged
        objLock.AcquireWriterLock(Timeout.Infinite)
        'For Each comItem In clstCOMports.SelectedItems
        For index As Integer = 0 To clstCOMports.Items.Count - 1
            Dim comItem As String
            comItem = clstCOMports.Items(index)

            'MsgBox(comItem.ToString)
            If (clstCOMports.GetItemChecked(index)) Then
                Dim found As Boolean
                found = False
                For Each port In serialsLst
                    If port.PortName.Equals(comItem.ToString) Then
                        found = True
                        'Exit For
                    End If
                Next

                If Not found Then
                    serialsLst.Add(New System.IO.Ports.SerialPort With {.BaudRate = 115200,
                                                         .DataBits = 8,
                                                         .Parity = IO.Ports.Parity.None,
                                                         .PortName = comItem,
                                                         .StopBits = IO.Ports.StopBits.One,
                                                         .ReadTimeout = System.Threading.Timeout.Infinite,
                                                         .WriteTimeout = 10})
                    'MsgBox("port open " & comItem.ToString)
                End If
            End If
        Next

        'check for items in serialsLst that are not anymore on the checked list:
        For cnt As Integer = serialsLst.Count - 1 To 0 Step -1
            Dim found As Boolean
            found = False
            For cnt2 = 0 To clstCOMports.Items.Count - 1
                If clstCOMports.GetItemChecked(cnt2) Then
                    If String.Equals(clstCOMports.Items(cnt2), serialsLst(cnt).PortName) Then
                        found = True
                        Exit For
                    End If
                End If
            Next
            'If Not clstCOMports.SelectedItems.Contains(serialsLst(cnt).PortName) Then
            If Not found Then

                If serialsLst(cnt).IsOpen Then
                    'MsgBox("port closing " & serialsLst(cnt).PortName)
                    serialsLst(cnt).Close()
                    serialsLst(cnt).Dispose()
                    serialsLst.RemoveAt(cnt)
                End If

            End If
        Next

        objLock.ReleaseWriterLock()
        objLock.AcquireReaderLock(1000)


        For Each port In serialsLst
            If Not port.IsOpen Then
                Try
                    port.ReadBufferSize = 4096 * 16
                    port.WriteBufferSize = 4096 * 16
                    port.Open()
                    port.DiscardInBuffer()
                    port.DiscardOutBuffer()

                    If SerialReadBuffers.ContainsKey(port.PortName) Then
                        SerialReadBuffers(port.PortName) = ""
                    Else
                        SerialReadBuffers.Add(port.PortName, "")
                    End If

                    If Not portFilters.ContainsKey(port.PortName) Then
                        Dim tempLines As List(Of String) = New List(Of String)
                        portFilters.Add(port.PortName, tempLines)
                    End If


                    'MsgBox("opened port " & port.PortName)
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
            End If
        Next

        objLock.ReleaseReaderLock()

    End Sub

    Private Sub btnTestSerialRead_Click(sender As Object, e As EventArgs)
        objLock.AcquireReaderLock(1000)
        For Each port In serialsLst
            If port.IsOpen Then
                Dim str As String
                str = ""
                While port.BytesToRead > 0
                    Dim bb As Byte
                    bb = port.ReadByte
                    str &= Chr(bb)
                End While

            End If
        Next
        objLock.ReleaseReaderLock()
    End Sub



    Private Function GetKeyInStr(strJSON As String) As String
        Dim strOut() As String
        strOut = strJSON.Split("""")

        If strOut.Length > 1 Then
            Return strOut(1)
        Else
            Return ""
        End If
    End Function


    Private Sub writeJSON2port()
        Dim strJSON = GetJSON()
        objLock.AcquireReaderLock(Timeout.Infinite) 'this is not ideal, better to get rid of the popup from Microsoft.Visualbasic.core...


        For Each port In serialsLst
            If port.IsOpen Then
                For Each line In strJSON.Split(Chr(10))
                    If portFilters.ContainsKey(port.PortName) Then
                        Dim strKey As String
                        strKey = GetKeyInStr(line)
                        If portFilters(port.PortName).Contains(strKey) Or String.IsNullOrEmpty(strKey) Then
                            Try
                                port.Write(line)
                            Catch ex As Exception
                                'Call SerialConnect()
                            End Try
                        End If
                    End If
                Next
                Try
                    port.Write(System.Environment.NewLine)
                Catch ex As Exception
                    'Call SerialConnect()
                End Try

            End If
        Next

        objLock.ReleaseReaderLock()
    End Sub

    Private Sub btnWriteJSON2Serial_Click(sender As Object, e As EventArgs)
        writeJSON2port()
    End Sub

    Private Sub btnDisconnectSerial_Click()
        SerialStopThread()
        Thread.CurrentThread.Sleep(500)

        objLock.AcquireReaderLock(1000)
        For Each port In serialsLst
            If port.IsOpen Then
                port.Close()
            End If
        Next
        objLock.ReleaseReaderLock()
    End Sub




    Private Sub SerialStartThread()
        thrdSerialUpdate = New Threading.Thread(AddressOf ThreadSerialUpdate)
        boSerialUpdateThreadStop = False
        thrdSerialUpdate.Start()
        'btnSerialUpdateStart.Enabled = False
        'btnSerialUpdateStop.Enabled = True

    End Sub

    Private Sub SerialStopThread()
        boSerialUpdateThreadStop = True
        'btnSerialUpdateStart.Enabled = True
        'btnSerialUpdateStop.Enabled = False

    End Sub




    Private Sub ThreadSerialUpdate()
        Dim sleeper As New Threading.ManualResetEvent(False)

        Do
            sleeper.WaitOne(20)
            Call writeJSON2port()
            Call readSerial2Buffer()

            If (boSerialUpdateThreadStop) Then
                boSerialUpdateThreadStop = False
                Exit Sub
            End If
        Loop

    End Sub

    Private Sub readSerial2Buffer()
        objLock.AcquireReaderLock(1000)
        For Each port In serialsLst
            If port.IsOpen Then
                'Dim str As String
                'Str = ""
                While port.BytesToRead > 0
                    Dim bb As Byte
                    bb = port.ReadByte
                    If bb = 10 Then
                        Call HandleSerialBuffersReadBack(port.PortName)
                    Else
                        Try
                            SerialReadBuffers(port.PortName) &= Chr(bb)
                        Catch
                        End Try

                    End If

                End While
                'txtSerial.Text = str
            End If
        Next
        objLock.ReleaseReaderLock()
    End Sub

    ' Dim dbCnt As UInteger

    Private Sub HandleSerialBuffersReadBack(portName As String)
        'SerialReadBuffers(portName)

        objLockFilters.AcquireWriterLock(10000)
        If String.Equals(SerialReadBuffers(portName).Substring(0, 3), "clr") Then  'Serial.println(F("clr"));
            portFilters(portName).Clear()
            'dbCnt += 1
        Else                                                        'Serial.println(F("rq:IsLightTurnLeftOn"));
            If String.Equals(SerialReadBuffers(portName).Substring(0, 3), "req") Then
                Dim keyStr As String
                keyStr = SerialReadBuffers(portName).Substring(4, SerialReadBuffers(portName).Length - 5)

                If Not portFilters(portName).Contains(keyStr) Then
                    portFilters(portName).Add(keyStr)
                End If

                'TODO: make filters somehow visible on Form
            End If
        End If

        SerialReadBuffers(portName) = "" 'reset buffer
        objLockFilters.ReleaseWriterLock()

    End Sub

    Private Sub btnGetSerialBuffer_Click(sender As Object, e As EventArgs)
        Dim str As String
        str = ""
        For Each port In serialsLst
            If port.IsOpen Then
                str &= SerialReadBuffers(port.PortName)
            End If
        Next

    End Sub



    Private Sub clstCOMports_MouseEnter(sender As Object, e As EventArgs) Handles clstCOMports.MouseMove
        Dim strToolTip As String
        strToolTip = ""
        objLockFilters.AcquireReaderLock(1000)
        If Not IsNothing(clstCOMports.SelectedItem) Then
            If portFilters.ContainsKey(clstCOMports.SelectedItem) Then
                For Each line In portFilters(clstCOMports.SelectedItem)
                    strToolTip &= line & System.Environment.NewLine
                Next

            Else
                strToolTip = "not found!"
            End If
        End If
        objLockFilters.ReleaseReaderLock()

        'strToolTip &= dbCnt
        ToolTipPortFilters.SetToolTip(clstCOMports, strToolTip)
        ToolTipPortFilters.ToolTipTitle = "filter of: " & clstCOMports.SelectedItem
    End Sub

    Private Sub clstCOMports_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clstCOMports.SelectedIndexChanged

    End Sub
End Class







'https://github.com/IfatChitin/Named-Pipes
'https://www.codeproject.com/Articles/864679/Creating-a-Server-Using-Named-Pipes