'Copyright by Mechi Community 2009-2011
'Do not remove this Copyright!

Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Public Class chat
    Private stream As NetworkStream
    Private streamw As StreamWriter
    Private streamr As StreamReader
    Private client As New TcpClient
    Private t As New Threading.Thread(AddressOf Listen)
    Private Delegate Sub DAddItem(ByVal s As String)
    Private nick As String = ""
    Private ip As String = ""
    Private port As String = ""
    Private pwd As String = ""

    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            My.Settings.Reload()
            ip = My.Settings.myIP
            port = My.Settings.myPort
            nick = My.Settings.myNick
            Server_Manager.Close()
            TextBox1.Focus()
            client.Connect(ip, Integer.Parse(port))
            If client.Connected Then
                stream = client.GetStream
                streamw = New StreamWriter(stream)
                streamr = New StreamReader(stream)

                streamw.WriteLine(nick)
                streamw.Flush()

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
        If s.StartsWith("/SERVERNAME ") Then
            Me.Text = "ChatServ Client - " & s.Remove(0, 11)
        ElseIf s.StartsWith("/SHUTDOWN") Then
            If client.Connected Then
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
            ListView1.Items.Clear()
            For i = 1 To names.Length - 1
                If names(i).StartsWith("@") Then
                    ListView1.Items.Add(names(i).Remove(0, 1))
                    Dim item As Integer = ListView1.Items.Count - 1
                    ListView1.Items(item).ForeColor = Color.DarkRed
                    ListView1.Items(item).Font = New Font(ListView1.Items(item).Font, FontStyle.Bold)
                Else
                    ListView1.Items.Add(names(i))
                End If
            Next
        Else
            Dim nick As String = s.Split(" ")(0)
            Dim fulltext As Integer = RichTextBox1.Text.Length + vbNewLine.Length + s.Length
            If fulltext >= RichTextBox1.MaxLength Then
                RichTextBox1.Clear()
            End If
            If nick.StartsWith("<") And nick.EndsWith(">") And nick.Length >= 2 Then
                Dim item As String = nick.Replace("<", "").Replace(">", "")
                With RichTextBox1
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.Font, FontStyle.Bold)
                    .AppendText(vbNewLine & "<")
                    If item.StartsWith("@") Then
                        .SelectionColor = Color.DarkRed
                        .AppendText(item.Remove(0, 1))
                        .SelectionColor = .ForeColor
                    Else
                        .AppendText(item)
                    End If
                    .AppendText(">")
                    .SelectionFont = .Font
                    .AppendText(s.Remove(0, nick.Length))
                End With
            ElseIf nick = "<Announce" Then
                Dim item As String = s.Split(" ")(2).Replace(">", "").Remove(0, 1)
                With RichTextBox1
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.Font, FontStyle.Bold)
                    .AppendText(vbNewLine & "<")
                    .SelectionColor = Color.DarkGreen
                    .AppendText("Announce by ")
                    .SelectionColor = Color.DarkRed
                    .AppendText(item)
                    .SelectionColor = .ForeColor
                    .AppendText(">")
                    .SelectionFont = .Font
                    .AppendText(s.Remove(0, 15 + item.Length))
                End With
            ElseIf nick = "***" Then
                With RichTextBox1
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.Font, FontStyle.Bold)
                    .SelectionColor = Color.DarkBlue
                    If String.IsNullOrWhiteSpace(RichTextBox1.Text) Then
                        .AppendText(s)
                    Else
                        .AppendText(vbNewLine & s)
                    End If
                    .SelectionColor = .ForeColor
                    .SelectionFont = .Font
                End With
            Else
                With RichTextBox1
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.Font, FontStyle.Bold)
                    .SelectionColor = Color.DarkViolet
                    If String.IsNullOrWhiteSpace(.Text) Then
                        .AppendText("unknown string received: ")
                    Else
                        .AppendText(vbNewLine & "unknown string received: ")
                    End If
                    .SelectionColor = .ForeColor
                    .SelectionFont = .Font
                    .AppendText(s)
                End With
            End If
            RichTextBox1.SelectionStart = RichTextBox1.TextLength
            RichTextBox1.ScrollToCaret()
        End If
    End Sub

    Private Sub Listen()
        While client.Connected
            Try
                Me.Invoke(New DAddItem(AddressOf AddItem), streamr.ReadLine)
            Catch ex As Exception
                If client.Connected Then
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

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = 13 Then
            Call Button1_Click(Button1, e)
        End If
    End Sub

    Private Sub Form1_leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FormClosed
        If client.Connected Then
            If MsgBox("Do you really want to exit?", vbYesNo) = vbYes Then

                client.Close()
                stream.Close()
                streamw.Close()
                streamr.Close()
                Application.Exit()
            End If
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

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        If client.Connected Then
            If MsgBox("Do you really want to exit?", vbYesNo) = vbYes Then

                client.Close()
                stream.Close()
                streamw.Close()
                streamr.Close()
                Application.Exit()
            End If
        End If
    End Sub

    Private Sub ToolStripButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton3.Click
        Me.Hide()
        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip("10")
        Timer2.Enabled = False
    End Sub
End Class
