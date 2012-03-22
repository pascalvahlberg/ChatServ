'Copyright by Chiruclan
'Do not remove this Copyright!

Imports System.Net.Sockets
Imports System.IO
Imports System.Net

Module server
    Dim scriptslog As scripts.log = New scripts.log()
    Dim conf As scripts.conf = New scripts.conf()
    Dim time As String = Date.Now
    Dim port As String
    Private server As TcpListener
    Private client As New TcpClient
    Private config_admpwd As String = conf.load("admin", "password")
    Private config_cmd As String = conf.load("admin", "cmd")
    Private config_ip As String = conf.load("network", "ip") 'currently buggy when other ip than 0.0.0.0
    Private config_port As String = conf.load("network", "port")
    Private config_name As String = conf.load("network", "name")
    Private ipendpoint As IPEndPoint = New IPEndPoint(IPAddress.Parse(config_ip), config_port)
    Private list As New List(Of Connection)
    Private users As New List(Of String)

    Private Structure Connection
        Dim stream As NetworkStream
        Dim streamw As StreamWriter
        Dim streamr As StreamReader
        Dim nick As String
        Dim pwd As String
        Dim announce As String
    End Structure

    Private Sub SendToAllClients(ByVal s As String)
        For Each c As Connection In list
            Try
                c.streamw.WriteLine(s)
                c.streamw.Flush()
            Catch
            End Try
        Next
    End Sub

    Private Function OnlineList()
        users.Sort()
        Dim nicks As String = "/names"
        For Each c As String In users
            nicks &= " " & c
        Next
        Return nicks
    End Function

    Sub Main()
        If IO.File.Exists("config.ini") = False Then
            MsgBox("config.ini doesn't exist!", MsgBoxStyle.Critical, "config missing")
            End
        End If
        If ConsoleSpecialKey.ControlC Then
            End
        End If
        Console.ForegroundColor = ConsoleColor.Green
        Console.Title = "ChatServ Server - " & config_name
        Console.WriteLine("# <CTRL-C>")
        Console.WriteLine("# Powered by Chiruclan")
        Console.WriteLine("# Commandstring is " + config_cmd)
        Console.WriteLine("# Loaded at " & time)
        Dim reg As New Threading.Thread(AddressOf RegisterServer)
        reg.Start()
        server = New TcpListener(ipendpoint)
        server.Start()
        Console.WriteLine("# Listening on " & config_ip & ":" & config_port)
        scriptslog.LogMessage("# Listening on " & config_ip & ":" & config_port)
        My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
        Console.WriteLine("*******************************************************************************")
        scriptslog.LogMessage("*******************************************************************************")
        Console.ForegroundColor = ConsoleColor.Cyan
        While True
            client = server.AcceptTcpClient
            Dim c As New Connection
            c.stream = client.GetStream
            c.streamr = New StreamReader(c.stream)
            c.streamw = New StreamWriter(c.stream)

            c.nick = c.streamr.ReadLine
            If c.nick.Contains("@") Or c.nick.Contains("<") Or c.nick.Contains(">") Or users.Contains(c.nick) Or c.nick.Contains(" ") Then
                client.Close()
                c.stream.Close()
                c.streamr.Close()
                c.streamw.Close()
            Else
                list.Add(c)
                users.Add(c.nick)
                SendToAllClients(OnlineList())
                Console.ForegroundColor = ConsoleColor.Red
                SendToAllClients("*** " & c.nick & " has joined")
                Console.WriteLine("#" & time & " *** " & c.nick & " has joined")
                scriptslog.LogMessage("#" & time & " *** " & c.nick & " has joined")
                Console.ForegroundColor = ConsoleColor.Cyan

                c.streamw.WriteLine("/SERVERNAME " & config_name)
                c.streamw.Flush()
                Dim t As New Threading.Thread(AddressOf ListenToConnection)
                t.Start(c)
            End If
        End While
    End Sub

    Private Sub RegisterServer()
        Dim server As New TcpClient
        Try
            server.Connect("chiruclan.de", 8001)
            If server.Connected Then
                Console.WriteLine("*** Registered Server")
                Dim c As New Connection
                c.stream = server.GetStream
                c.streamr = New StreamReader(c.stream)
                c.streamw = New StreamWriter(c.stream)
                c.streamw.WriteLine(config_name & " " & config_port)
                c.streamw.Flush()
                While server.Connected

                End While
                c.stream.Close()
                c.streamw.Close()
                c.streamr.Close()
                Console.WriteLine("*** Unregistered Server")
                RegisterServer()
            Else
                server.Close()
                RegisterServer()
            End If
        Catch ex As Exception
            RegisterServer()
        End Try
    End Sub

    Private Sub ListenToConnection(ByVal con As Connection)
        Do
            Try
                Dim announce As String = con.announce
                Dim tmp As String = con.streamr.ReadLine
                If tmp.StartsWith(config_cmd & "kill ") And config_admpwd = con.pwd Then
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Dim Kickname As String = tmp.Remove(0, 6)
                    For Each Connection In list
                        If Connection.nick = Kickname Then
                            list.Remove(Connection)
                            users.Remove(Connection.nick)
                            Connection.stream.Close()
                            Connection.streamr.Close()
                            Connection.streamw.Close()
                            Console.WriteLine(time & " " & OnlineList())
                            scriptslog.LogMessage("*** " & time & " " & OnlineList())
                            SendToAllClients(OnlineList())
                            SendToAllClients("*** " & Kickname & " is killed")
                            Console.WriteLine("*" & time & " " & Kickname & " is killed!")
                            scriptslog.LogMessage("*" & time & " " & Kickname & " is killed!")
                            Exit For
                        End If
                    Next
                ElseIf tmp.StartsWith(config_cmd & "shutdown") And config_admpwd = con.pwd Then
                    Console.ForegroundColor = ConsoleColor.Yellow
                    scriptslog.LogMessage("*** Server shutdown by " + con.nick + " ****")
                    SendToAllClients("*** SERVER SHUTDOWN BY " & con.nick.ToUpper.Remove(0, 1) & " ***")
                    SendToAllClients("/SHUTDOWN")
                    End
                ElseIf tmp.StartsWith(config_cmd + "announce ") And config_admpwd = con.pwd Then
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine("!" & time & " <Announce by " & con.nick & "> " & tmp.Remove(0, 9))
                    scriptslog.LogMessage("!" & time & " <Announce by " & con.nick & "> " & tmp.Remove(0, 9))
                    SendToAllClients("<Announce by " + con.nick + "> " & tmp.Remove(0, 9))
                ElseIf tmp.StartsWith("!afk") Then
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("#" & time & " " & con.nick & " is AFK right now")
                    scriptslog.LogMessage("#" + time + " " + con.nick + " is AFK right now")
                    SendToAllClients("*** " & con.nick & " is AFK right now")
                ElseIf tmp.StartsWith("!notafk") Then
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine("#" & time & " " & con.nick & " is not longer AFK")
                    scriptslog.LogMessage("#" + time + " " + con.nick + " is not longer AFK")
                    SendToAllClients("*** " & con.nick & " is not longer AFK")
                ElseIf tmp.StartsWith("#admin") And Not tmp.Contains(" ") Then
                    con.pwd = con.streamr.ReadLine
                    If con.pwd = config_admpwd And Not con.nick.StartsWith("@") Then
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.WriteLine("#" & time & " *** " & con.nick & " is now an administrator")
                        scriptslog.LogMessage("*** " + time + " *** " & con.nick & " is now an administrator")
                        SendToAllClients("*** " & con.nick & " is now an administrator")
                        users.Remove(con.nick)
                        con.nick = "@" & con.nick
                        users.Add(con.nick)
                        Console.WriteLine(time & " " & OnlineList())
                        scriptslog.LogMessage("*** " & time & " " & OnlineList())
                        SendToAllClients(OnlineList())
                    Else

                    End If
                ElseIf tmp.Contains(config_admpwd) Then
                    list.Remove(con)
                    users.Remove(con.nick)
                    con.stream.Close()
                    con.streamr.Close()
                    con.streamw.Close()
                    Console.WriteLine(time & " " & OnlineList())
                    scriptslog.LogMessage("*** " & time & " " & OnlineList())
                    SendToAllClients(OnlineList())
                    Console.ForegroundColor = ConsoleColor.Yellow
                    SendToAllClients("*** " & con.nick & " is killed by Server")
                    Console.WriteLine("*" & time & " " & con.nick & " was killed by Server!")
                    scriptslog.LogMessage("*" & time & " " & con.nick & " was killed by Server!")
                ElseIf tmp.StartsWith(config_cmd + "names") Then
                    Sendtoperson(OnlineList(), con.nick)
                    Console.WriteLine(time & " " & OnlineList())
                    scriptslog.LogMessage("*** " & time & " " & OnlineList())
                Else
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine(time & " <" & con.nick & "> " & tmp)
                    scriptslog.LogMessage(time & " <" & con.nick & "> " & tmp)
                    SendToAllClients("<" & con.nick & "> " & tmp)
                End If
            Catch
                list.Remove(con)
                users.Remove(con.nick)
                Console.WriteLine(time & " " & OnlineList())
                scriptslog.LogMessage("*** " & time & " " & OnlineList())
                SendToAllClients(OnlineList())
                Console.ForegroundColor = ConsoleColor.Red
                SendToAllClients("*** " & con.nick & " has quit")
                Console.WriteLine("#" & time & " *** " & con.nick & " has quit")
                scriptslog.LogMessage("#" & time & " *** " & con.nick & " has quit")
                Exit Do
            End Try
        Loop
    End Sub

    Private Sub Sendtoperson(ByVal s As String, ByVal Nick As String)
        For Each Connection In list
            Try
                If Connection.nick = Nick Then
                    Connection.streamw.WriteLine(s)
                    Connection.streamw.Flush()
                End If
            Catch
            End Try
        Next
    End Sub
End Module
