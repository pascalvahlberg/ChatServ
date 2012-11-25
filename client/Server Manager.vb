Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Public Class Server_Manager

    Private client As New TcpClient
    Private stream As NetworkStream
    Private streamr As StreamReader
	Private streamw As StreamWriter
    Private list As New List(Of Avail)

    Private Structure Avail
        Dim name As String
        Dim ip As String
        Dim port As String
    End Structure

    Private Sub Server_Manager_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            My.Settings.Reload()
            TextBox1.Text = My.Settings.myIP
            TextBox2.Text = My.Settings.myPort
            TextBox3.Text = My.Settings.myNick
            client.Connect("hosting.chiruclan.de", 8001)
            If client.Connected Then
                Dim content As String = ""
                stream = client.GetStream
                streamr = New StreamReader(stream)
                streamw = New StreamWriter(stream)
				streamw.WriteLine("/LIST")
				streamw.Flush()
				streamw.Close()
                While client.Connected
                    content = streamr.ReadLine()
                    If Not String.IsNullOrWhiteSpace(content) Then
                        If content = "/QUIT" Then
                            client.Close()
                            stream.Close()
                            streamr.Close()
                        Else
                            Dim c As New Avail
                            Dim cs As Array = content.Split(";")
							c.opcode = cs(0)
                            c.name = cs(1)
                            c.ip = cs(2)
                            c.port = cs(3)
							if cs(0) = "0x0" Then
								ListBox1.Items.Add(c.name)
								list.Add(c)
							End If
                        End If
                    Else
                        client.Close()
                        stream.Close()
                        streamr.Close()
                    End If
                End While
                client.Close()
                stream.Close()
                streamr.Close()
            End If
        Catch ex As Exception
            MsgBox("Could not receive server list, you have to enter it yourself!")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If String.IsNullOrWhiteSpace(TextBox1.Text) Then
            My.Settings.myIP = "127.0.0.1"
            My.Settings.Save()
        End If
        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            My.Settings.myPort = "8000"
            My.Settings.Save()
        End If
        If String.IsNullOrWhiteSpace(TextBox3.Text) Then
            Dim rand As New System.Random()
            My.Settings.myNick = "Guest_" & rand.Next()
            My.Settings.Save()
        End If
        chat.Show()
        chat.Focus()
    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = 13 Then
            Call Button1_Click(Button1, e)
        End If
    End Sub

    Private Sub TextBox2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = 13 Then
            Call Button1_Click(Button1, e)
        End If
    End Sub

    Private Sub TextBox3_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = 13 Then
            Call Button1_Click(Button1, e)
        End If
    End Sub

    Private Sub Server_Manager_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        TextBox1.Focus()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        For Each c In list
            If c.name = ListBox1.SelectedItem() Then
                TextBox1.Text = c.ip
                TextBox2.Text = c.port
                My.Settings.myIP = c.ip
                My.Settings.myPort = c.port
                My.Settings.Save()
                TextBox3.Focus()
            End If
        Next
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        My.Settings.myIP = TextBox1.Text
        My.Settings.Save()
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        My.Settings.myPort = TextBox2.Text
        My.Settings.Save()
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
        My.Settings.myNick = TextBox3.Text
        My.Settings.Save()
    End Sub
End Class