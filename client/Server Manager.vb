Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Public Class Server_Manager

    Private Sub Server_Manager_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            My.Settings.Reload()
            TextBox1.Text = My.Settings.myIP
            TextBox2.Text = My.Settings.myPort
            TextBox3.Text = My.Settings.myNick
        Catch ex As Exception
            MsgBox("Ooops! An unexpected error happened!")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        TextBox1.Focus()
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
        chat.Close()
        TextBox1.Focus()
        TextBox1.DeselectAll()
        My.Settings.Reload()

        If My.Settings.myServer_Names Is Nothing Then
            My.Settings.myServer_Names = New Specialized.StringCollection
        End If

        If My.Settings.myServer_Address Is Nothing Then
            My.Settings.myServer_Address = New Specialized.StringCollection
        End If

        If My.Settings.myServer_Ports Is Nothing Then
            My.Settings.myServer_Ports = New Specialized.StringCollection
        End If

        Dim myServer_Names(My.Settings.myServer_Names.Count - 1) As String
        My.Settings.myServer_Names.CopyTo(myServer_Names, 0)
        Dim myServer_Address(My.Settings.myServer_Address.Count - 1) As String
        My.Settings.myServer_Address.CopyTo(myServer_Address, 0)
        Dim myServer_Ports(My.Settings.myServer_Ports.Count - 1) As String
        My.Settings.myServer_Ports.CopyTo(myServer_Ports, 0)

        For Each server_name As Object In myServer_Names
            ListBox1.Items.Add(server_name)
        Next server_name

        For Each server_address As Object In myServer_Address
            ListBox2.Items.Add(server_address)
        Next server_address

        For Each server_port As Object In myServer_Ports
            ListBox3.Items.Add(server_port)
        Next server_port
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        ListBox2.SelectedIndex = ListBox1.SelectedIndex
        ListBox3.SelectedIndex = ListBox1.SelectedIndex
        TextBox1.Text = ListBox2.SelectedItem
        TextBox2.Text = ListBox3.SelectedItem
        My.Settings.myIP = ListBox2.SelectedItem
        My.Settings.myPort = ListBox3.SelectedItem
        My.Settings.Save()
        TextBox1.Focus()
        TextBox1.DeselectAll()
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

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        TextBox1.Focus()
        Dim sname As String = InputBox("Enter a server name", "Server name")
        If Not String.IsNullOrWhiteSpace(sname) Then
            ListBox1.Items.Add(sname)
            ListBox2.Items.Add(TextBox1.Text)
            ListBox3.Items.Add(TextBox2.Text)
            My.Settings.myServer_Names.Add(sname)
            My.Settings.myServer_Address.Add(TextBox1.Text)
            My.Settings.myServer_Ports.Add(TextBox2.Text)
            My.Settings.Save()
        End If
        TextBox1.Focus()
        TextBox1.DeselectAll()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim index As Integer = ListBox1.SelectedIndex
        If index >= 0 Then
            My.Settings.myServer_Names.RemoveAt(index)
            My.Settings.myServer_Address.RemoveAt(index)
            My.Settings.myServer_Ports.RemoveAt(index)
            ListBox3.Items.RemoveAt(index)
            ListBox2.Items.RemoveAt(index)
            ListBox1.Items.RemoveAt(index)
            My.Settings.Save()
        End If
        TextBox1.Focus()
        TextBox1.DeselectAll()
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged
        ListBox1.SelectedIndex = ListBox2.SelectedIndex
        ListBox3.SelectedIndex = ListBox2.SelectedIndex
        TextBox1.Text = ListBox2.SelectedItem
        TextBox2.Text = ListBox3.SelectedItem
        My.Settings.myIP = ListBox2.SelectedItem
        My.Settings.myPort = ListBox3.SelectedItem
    End Sub

    Private Sub ListBox3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox3.SelectedIndexChanged
        ListBox1.SelectedIndex = ListBox3.SelectedIndex
        ListBox2.SelectedIndex = ListBox3.SelectedIndex
        TextBox1.Text = ListBox2.SelectedItem
        TextBox2.Text = ListBox3.SelectedItem
        My.Settings.myIP = ListBox2.SelectedItem
        My.Settings.myPort = ListBox3.SelectedItem
    End Sub
End Class