'Copyright by Mechi Community 2009-2011
'Do not remove this Copyright!

Imports System.Net.Sockets
Imports System.IO

Public Class chat
    Private stream As NetworkStream
    Private streamw As StreamWriter
    Private streamr As StreamReader
    Private client As New TcpClient
    Private t As New Threading.Thread(AddressOf Listen)
    Private Delegate Sub DAddItem(ByVal s As String)
    Private nick As String = "unknown"
    Private ip As String = "unknown"
    Private port As String = "unknown"
    Private pwd As String = ""
    Private connected As Boolean = False

    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            client.Connect(ip, port)
            If client.Connected Then
                stream = client.GetStream
                streamw = New StreamWriter(stream)
                streamr = New StreamReader(stream)

                streamw.WriteLine(nick)
                streamw.Flush()

                connected = True
                t.Start()
            Else
                client.Close()
                stream.Close()
                streamw.Close()
                streamr.Close()
                MessageBox.Show("Unable to connect to " & ip & ":" & port)
                Application.Exit()
            End If
        Catch ex As Exception
            client.Close()
            stream.Close()
            streamw.Close()
            streamr.Close()
            MessageBox.Show("Unable to connect to " & ip & ":" & port)
            Application.Exit()
        End Try
    End Sub

    Private Sub AddItem(ByVal s As String)
        If s.StartsWith("/SHUTDOWN") Then
            If connected Then
                connected = False
                client.Close()
                stream.Close()
                streamw.Close()
                streamr.Close()
                MessageBox.Show("*** SERVER SHUTDOWN ***")
                Application.Exit()
            End If
        ElseIf s.StartsWith("/names") Then
            Dim i As Integer
            Dim names() As String = s.Split(" ")
            ListBox1.Items.Clear()
            For i = 1 To names.Length - 1
                With ListBox1
                    .Items.Add(names(i))
                End With
            Next
        Else
            If String.IsNullOrWhiteSpace(RichTextBox1.Text) Then
                RichTextBox1.AppendText(s)
            Else
                RichTextBox1.AppendText(vbNewLine & s)
            End If
            If RichTextBox1.Text.Length = RichTextBox1.MaxLength Then
                RichTextBox1.Text = s
            End If
            TextBox1.SendToBack()
        End If
    End Sub

    Private Sub Listen()
        While client.Connected
            Try
                Me.Invoke(New DAddItem(AddressOf AddItem), streamr.ReadLine)
            Catch ex As Exception
                If connected Then
                    connected = False
                    client.Close()
                    stream.Close()
                    streamw.Close()
                    streamr.Close()
                    MsgBox("Connection lost")
                    Application.Exit()
                End If
            End Try
        End While
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If String.IsNullOrWhiteSpace(TextBox1.Text) Then
            If String.IsNullOrEmpty(RichTextBox1.Text) Then
                RichTextBox1.AppendText("Cannot send an empty String!")
            Else
                RichTextBox1.AppendText(vbNewLine & "Cannot send an empty String!")
            End If
        ElseIf TextBox1.Text = "#admin" Then
            pwd = InputBox("Password: ", "Administrationpassword")
            streamw.WriteLine(TextBox1.Text)
            streamw.WriteLine(pwd)
            streamw.Flush()
            TextBox1.Clear()
        ElseIf TextBox1.Text = "#about" Then
            AboutBox1.Show()
            TextBox1.Clear()
        Else
            streamw.WriteLine(TextBox1.Text)
            streamw.Flush()
            TextBox1.Clear()
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ip = InputBox("Hostname of the Server:" & vbNewLine & "(default = chatserv.chiruclan.de) ", "Hostname:", "")
        If ip = "" Then
            ip = "chatserv.chiruclan.de"
        End If
        port = InputBox("Port:" & vbNewLine & "(default = 8000)", "Port of Server", "")
        If port = "" Then
            port = "8000"
        End If
        nick = InputBox("Nickname:" & vbNewLine & "(default = Guest_x)", "Choose nickname", "")
        If nick = "" Then
            Dim rand As New System.Random()
            nick = "Guest_" & rand.Next()
        End If
    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = 13 Then
            Call Button1_Click(Button1, e)
        End If
    End Sub

    Private Sub Form1_leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FormClosed
        If connected Then
            connected = False
            client.Close()
            stream.Close()
            streamw.Close()
            streamr.Close()
            Application.Exit()
        End If
    End Sub

    Private Sub ShowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShowToolStripMenuItem.Click
        Me.Show()
        NotifyIcon1.Visible = False
        Timer2.Enabled = True
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        NotifyIcon1.Visible = False
        Timer2.Enabled = True
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()
            NotifyIcon1.Visible = True
            NotifyIcon1.ShowBalloonTip("10")
            Timer2.Enabled = False
        End If
    End Sub

    Private Sub ToolStripButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton2.Click
        AboutBox1.Show()
    End Sub

    Private Sub HideToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HideToolStripMenuItem1.Click
        Me.Hide()
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip("10")
        Timer2.Enabled = False
    End Sub
End Class
