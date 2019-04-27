﻿Public Class ConfigJava

    Dim isInit As Boolean = True

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m

        ' Set the max of the slider's range to the total memory of system        
        maxmem_Validate()

        Try
            sliderMemory.Value = maxmem.Text
        Catch ex As Exception

        End Try

        javainstalltype.Text = DetectJava.FindJavaInstallType

        minmem.Text = MyUserSettings.settingsStore.Startup_MemoryMin.ToLower.Replace("m", "")
        jarpath.Text = MyUserSettings.settingsStore.Startup_JavaExec
        parameters.Text = MyUserSettings.settingsStore.JarLaunchArguments
        appendJlineArgCheckbox.IsChecked = MyUserSettings.settingsStore.AppendJlineArg

        If MyUserSettings.settingsStore.Startup_JavaExec Is "" Then
            jreauto.IsChecked = True
        Else
            jremanual.IsChecked = True
        End If

        isInit = False
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()
        superoverlay.Confirm_DoClose(Me)
    End Sub

    Public Sub HelpClicked()
        System.Diagnostics.Process.Start("http://www.minecraftwiki.net/wiki/Server/Requirements")
    End Sub

    ' Save settings on change
    Private Sub maxmem_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            If CType(maxmem.Text, Integer) > 0 Then
                'todo ignore if null
                MyUserSettings.settingsStore.Startup_Memory = maxmem.Text & "M"
            End If
        End If
    End Sub

    Private Sub TextBox_TextChanged_1(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyUserSettings.settingsStore.Startup_MemoryMin = CType(sender, TextBox).Text & "M"
        End If
    End Sub

    Private Sub TextBox_TextChanged_3(sender As Object, e As TextChangedEventArgs)
        'parameters RIGHT
        If Not isInit Then
            MyUserSettings.settingsStore.JarLaunchArguments = CType(sender, TextBox).Text
        End If
    End Sub

    Private Sub sliderMemory_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of System.Double)) Handles sliderMemory.ValueChanged
        If Not isInit Then
            maxmem.Text = CType(sliderMemory.Value, Integer)
        End If
    End Sub

#Region "Java selection"

    Private Sub jremanual_Checked(sender As Object, e As RoutedEventArgs) Handles jremanual.Unchecked
        MyUserSettings.settingsStore.Startup_JavaExec = ""
    End Sub

    Private Sub TextBox_TextChanged_JREpath(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyUserSettings.settingsStore.Startup_JavaExec = CType(sender, TextBox).Text
        End If
    End Sub

    Private Sub Hyperlink_ExploreJavaPath(sender As Object, e As RoutedEventArgs)
        Dim path As String = DetectJava.FindPath
        If Not My.Computer.FileSystem.DirectoryExists(path) Then
            path = "http://java.com/en/download/"
        End If
        System.Diagnostics.Process.Start("explorer.exe", path)
    End Sub

#End Region

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim o As Microsoft.Win32.OpenFileDialog = New Microsoft.Win32.OpenFileDialog()
        o.DefaultExt = "java.exe"
        o.Filter = "Java Executable|java.exe"
        Dim result As Boolean = o.ShowDialog()
        If result = True Then
            jarpath.Text = o.FileName
        End If
    End Sub

    Private Sub maxmem_LostFocus(sender As Object, e As RoutedEventArgs) Handles maxmem.LostFocus
        maxmem_Validate()
    End Sub

    Private Sub maxmem_Validate()
        Dim TotalMachineMemory As Integer = (ServerManager.GetTotalMemoryInBytes() / (1024 * 1024)) * 0.95 ' in MiB 
        'Times by 0.95 to prevent allocating a ridiculous maximum memory value

        sliderMemory.Maximum = TotalMachineMemory

        Dim MaxMemInMB As Integer = MyUserSettings.MaxMemoryInMB

        If MaxMemInMB <= TotalMachineMemory Then
            maxmem.Text = MaxMemInMB
        Else
            If TotalMachineMemory > 1024 Then
                Dim n As New MessageWindow(MyMainWindow, "", "You've allocated the server more memory than available on this PC. The memory will be reset to the default 1GB")
                maxmem.Text = 1024
            Else
                Dim n As New MessageWindow(MyMainWindow, "", "You've allocated the server more memory than available on this PC. Please set another value for the memory allocation.")
                maxmem.Text = 32
            End If
        End If
    End Sub

    Private Sub btnSetJavaSpecificArgs_Click(sender As Object, e As RoutedEventArgs) Handles btnSetJavaSpecificArgs.Click
        Dim m As New AdvJavaArguementsDialog
        m.Owner = MyMainWindow
        m.ShowDialog()
    End Sub

    Private Sub AppendJlineArgCheckbox_Checked(sender As Object, e As RoutedEventArgs)
        If Not isInit Then
            MyUserSettings.settingsStore.AppendJlineArg = appendJlineArgCheckbox.IsChecked
        End If
    End Sub
End Class