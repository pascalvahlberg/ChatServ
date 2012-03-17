Imports System.Net.Sockets
Imports System.IO

Public Class Server_Manager

    Dim client As TcpClient
    Dim stream As NetworkStream
    Dim streamr As StreamReader
    Dim list As New List(Of Avail)

    Private Structure Avail
        Dim name As String
        Dim ip As String
        Dim port As String
    End Structure

    Private Sub Server_Manager_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            client.Connect("chiruclan.de", 8002)
            If client.Connected Then
                Dim content As String = ""
                stream = client.GetStream
                streamr = New StreamReader(stream)
                While client.Connected
                    content = streamr.ReadLine()
                    If content = "/QUIT" Then
                        client.Close()
                        stream.Close()
                        streamr.Close()
                    Else
                        Dim c As New Avail
                        Dim cs As Array = content.Split(" ")
                        c.name = cs(0)
                        c.ip = cs(1)
                        c.port = cs(2)
                        ListBox1.Items.Add(c.name)
                        list.Add(c)
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
            TextBox1.Text = "127.0.0.1"
        End If
        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            TextBox2.Text = "8000"
        End If
        If String.IsNullOrWhiteSpace(TextBox3.Text) Then
            Dim rand As New System.Random()
            TextBox3.Text = "Guest_" & rand.Next()
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
            If c.name = ListBox1.SelectedValue() Then
                TextBox1.Text = c.ip
                TextBox2.Text = c.port
                TextBox3.Focus()
            End If
        Next
    End Sub
End Class