﻿Imports DWSIM.SharedClasses
Imports Microsoft.Win32
Imports DWSIM.Interfaces.Interfaces2
Imports CapeOpen

Public Class FlashAlgorithmConfig

    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    Public Property Settings As Dictionary(Of Interfaces.Enums.FlashSetting, String)

    Public Property AvailableCompounds As List(Of String)

    Public FlashAlgo As Interfaces.IFlashAlgorithm

    Dim ci As Globalization.CultureInfo

    Public _coes, _coppm As Object
    Public _selppm As CapeOpenObjInfo
    Private _coobjects As New List(Of CapeOpenObjInfo)
    Private _loaded As Boolean = False
    Public _mappings As New Dictionary(Of String, String)
    Public _esname As String = ""
    Public _phasemappings As New Dictionary(Of String, PropertyPackages.PhaseInfo)

    Public Property ExcelMode As Boolean = False

    Private Sub FlashAlgorithmConfig_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not ExcelMode Then

            Me.Text += " - " & FlashAlgo.Tag

            Select Case FlashAlgo.AlgoType
                Case Interfaces.Enums.FlashMethod.Default_Algorithm, Interfaces.Enums.FlashMethod.Nested_Loops_VLE
                    TabControl1.TabPages.Remove(TabPageGM)
                    TabControl1.TabPages.Remove(TabPageIO)
                    TabControl1.TabPages.Remove(TabPageVLLE)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.Nested_Loops_VLLE, Interfaces.Enums.FlashMethod.Nested_Loops_Immiscible_VLLE
                    TabControl1.TabPages.Remove(TabPageGM)
                    TabControl1.TabPages.Remove(TabPageIO)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.Gibbs_Minimization_VLE
                    TabControl1.TabPages.Remove(TabPageNL)
                    TabControl1.TabPages.Remove(TabPageIO)
                    TabControl1.TabPages.Remove(TabPageVLLE)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.Gibbs_Minimization_VLLE
                    TabControl1.TabPages.Remove(TabPageNL)
                    TabControl1.TabPages.Remove(TabPageIO)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.Inside_Out_VLE
                    TabControl1.TabPages.Remove(TabPageGM)
                    TabControl1.TabPages.Remove(TabPageNL)
                    TabControl1.TabPages.Remove(TabPageVLLE)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.Inside_Out_VLLE
                    TabControl1.TabPages.Remove(TabPageGM)
                    TabControl1.TabPages.Remove(TabPageNL)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.Nested_Loops_SLE_Eutectic, Interfaces.Enums.FlashMethod.Nested_Loops_SLE_SolidSolution, Interfaces.Enums.FlashMethod.Simple_LLE
                    TabControl1.TabPages.Remove(TabPageGM)
                    TabControl1.TabPages.Remove(TabPageNL)
                    TabControl1.TabPages.Remove(TabPageIO)
                    TabControl1.TabPages.Remove(TabPageVLLE)
                    TabControl1.TabPages.Remove(TabPageCOES)
                Case Interfaces.Enums.FlashMethod.CAPE_OPEN_Equilibrium_Server
                    TabControl1.TabPages.Remove(TabPageConvPars)
                    TabControl1.TabPages.Remove(TabPageGM)
                    TabControl1.TabPages.Remove(TabPageNL)
                    TabControl1.TabPages.Remove(TabPageIO)
                    TabControl1.TabPages.Remove(TabPageVLLE)
            End Select

        Else

            Me.Text = "Flash Algorithm Settings"

            TabControl1.TabPages.Remove(TabPageCOES)
            TabControl1.TabPages.Remove(TabPageGeneral)
            TabControl1.TabPages.Remove(TabPageVLLE)

        End If

        ci = Globalization.CultureInfo.InvariantCulture

        chkReplaceFlashPT.Checked = Settings(Interfaces.Enums.FlashSetting.Replace_PTFlash)
        chkValidateEqCalc.Checked = Settings(Interfaces.Enums.FlashSetting.ValidateEquilibriumCalc)
        tbFlashValidationTolerance.Text = Double.Parse(Settings(Interfaces.Enums.FlashSetting.ValidationGibbsTolerance), ci).ToString
        chkDoPhaseId.Checked = Settings(Interfaces.Enums.FlashSetting.UsePhaseIdentificationAlgorithm)
        chkCalcBubbleDew.Checked = Settings(Interfaces.Enums.FlashSetting.CalculateBubbleAndDewPoints)

        If FlashAlgo IsNot Nothing AndAlso FlashAlgo.AlgoType = Interfaces.Enums.FlashMethod.CAPE_OPEN_Equilibrium_Server Then

            If Not _coppm Is Nothing Then

                Dim myppm As CapeOpen.ICapeUtilities = TryCast(_coppm, CapeOpen.ICapeUtilities)
                If Not myppm Is Nothing Then
                    Try
                        myppm.Initialize()
                    Catch ex As Exception
                        Dim ecu As CapeOpen.ECapeUser = myppm
                        MessageBox.Show("Error Initializing Property Package Manager - " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MessageBox.Show("CAPE-OPEN Exception " & ecu.code & " at " & ecu.interfaceName & "." & ecu.scope & ". Reason: " & ecu.description)
                    End Try
                End If

                Me.lblName2.Text = _selppm.Name
                Me.lblVersion2.Text = _selppm.Version
                Me.lblAuthorURL2.Text = _selppm.VendorURL
                Me.lblDesc2.Text = _selppm.Description
                Me.lblAbout2.Text = _selppm.AboutInfo

                tbSelectedES.Text = _selppm.Name & " / " & _esname

            End If

            _loaded = True

        Else

            tbPHExtMaxIt.Text = Integer.Parse(Settings(Interfaces.Enums.FlashSetting.PHFlash_Maximum_Number_Of_External_Iterations), ci).ToString
            tbPHExtMaxTol.Text = Double.Parse(Settings(Interfaces.Enums.FlashSetting.PHFlash_External_Loop_Tolerance), ci).ToString
            tbPHIntMaxIt.Text = Integer.Parse(Settings(Interfaces.Enums.FlashSetting.PHFlash_Maximum_Number_Of_Internal_Iterations), ci).ToString
            tbPHintMaxTol.Text = Double.Parse(Settings(Interfaces.Enums.FlashSetting.PHFlash_Internal_Loop_Tolerance), ci).ToString
            tbPTExtMaxIt.Text = Integer.Parse(Settings(Interfaces.Enums.FlashSetting.PTFlash_Maximum_Number_Of_External_Iterations), ci).ToString
            tbPTExtTol.Text = Double.Parse(Settings(Interfaces.Enums.FlashSetting.PTFlash_External_Loop_Tolerance), ci).ToString
            tbPTintMaxIt.Text = Integer.Parse(Settings(Interfaces.Enums.FlashSetting.PTFlash_Maximum_Number_Of_Internal_Iterations), ci).ToString
            tbPTIntTol.Text = Double.Parse(Settings(Interfaces.Enums.FlashSetting.PTFlash_Internal_Loop_Tolerance), ci).ToString

            chkFastModeNL.Checked = Settings(Interfaces.Enums.FlashSetting.NL_FastMode)

            chkUseBroydenIO.Checked = Settings(Interfaces.Enums.FlashSetting.IO_FastMode)

            Dim minmethods As String() = [Enum].GetNames(New PropertyPackages.Auxiliary.FlashAlgorithms.GibbsMinimization3P().Solver.GetType)
            cbMinMethodGM.Items.Clear()
            cbMinMethodGM.Items.AddRange(minmethods)

            cbMinMethodGM.SelectedItem = Settings(Interfaces.Enums.FlashSetting.GM_OptimizationMethod)

            Select Case Settings(Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestSeverity)
                Case 0
                    rbLow.Checked = True
                Case 1
                    rbMedium.Checked = True
                Case 2
                    rbHigh.Checked = True
            End Select

            If Not ExcelMode Then SetupKeyCompounds()

        End If

    End Sub

    Private Sub FlashAlgorithmConfig_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        Try

            Settings(Interfaces.Enums.FlashSetting.Replace_PTFlash) = chkReplaceFlashPT.Checked
            Settings(Interfaces.Enums.FlashSetting.ValidateEquilibriumCalc) = chkValidateEqCalc.Checked
            Settings(Interfaces.Enums.FlashSetting.UsePhaseIdentificationAlgorithm) = chkDoPhaseId.Checked
            Settings(Interfaces.Enums.FlashSetting.CalculateBubbleAndDewPoints) = chkCalcBubbleDew.Checked

            Settings(Interfaces.Enums.FlashSetting.ValidationGibbsTolerance) = Double.Parse(tbFlashValidationTolerance.Text).ToString(ci)

            If ExcelMode OrElse FlashAlgo.AlgoType = Interfaces.Enums.FlashMethod.CAPE_OPEN_Equilibrium_Server Then

                Settings(Interfaces.Enums.FlashSetting.PHFlash_Maximum_Number_Of_External_Iterations) = Integer.Parse(tbPHExtMaxIt.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PHFlash_External_Loop_Tolerance) = Double.Parse(tbPHExtMaxTol.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PHFlash_Maximum_Number_Of_Internal_Iterations) = Integer.Parse(tbPHIntMaxIt.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PHFlash_Internal_Loop_Tolerance) = Double.Parse(tbPHintMaxTol.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PTFlash_Maximum_Number_Of_External_Iterations) = Integer.Parse(tbPTExtMaxIt.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PTFlash_External_Loop_Tolerance) = Double.Parse(tbPTExtTol.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PTFlash_Maximum_Number_Of_Internal_Iterations) = Integer.Parse(tbPTintMaxIt.Text).ToString(ci)
                Settings(Interfaces.Enums.FlashSetting.PTFlash_Internal_Loop_Tolerance) = Double.Parse(tbPTIntTol.Text).ToString(ci)

                Settings(Interfaces.Enums.FlashSetting.NL_FastMode) = chkFastModeNL.Checked

                Settings(Interfaces.Enums.FlashSetting.IO_FastMode) = chkUseBroydenIO.Checked

                Settings(Interfaces.Enums.FlashSetting.GM_OptimizationMethod) = cbMinMethodGM.SelectedItem

                If rbLow.Checked Then Settings(Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestSeverity) = 0
                If rbMedium.Checked Then Settings(Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestSeverity) = 1
                If rbHigh.Checked Then Settings(Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestSeverity) = 2

                Dim comps As String = ""

                For Each lvi As ListViewItem In lvKeyComp.Items
                    If lvi.Checked Then comps += lvi.Text + ","
                Next

                Settings(Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestCompIds) = comps

            End If

        Catch ex As Exception

            MessageBox.Show("Error parsing input. Some settings may not have been updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub SetupKeyCompounds()

        Dim selected As Array

        selected = Settings(Interfaces.Enums.FlashSetting.ThreePhaseFlashStabTestCompIds).ToArray(ci, Type.GetType("System.String"))

        Me.lvKeyComp.Items.Clear()

        For i As Integer = 0 To AvailableCompounds.Count - 1
            With Me.lvKeyComp.Items.Add(AvailableCompounds(i))
                For Each s As String In selected
                    If s = AvailableCompounds(i) Then
                        .Checked = True
                        Exit For
                    End If
                Next
            End With
        Next

    End Sub

    Sub SearchCO(ByVal CLSID As String)

        Dim keys As String() = My.Computer.Registry.ClassesRoot.OpenSubKey("CLSID", False).GetSubKeyNames()

        For Each k2 In keys
            Dim mykey As RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey("CLSID", False).OpenSubKey(k2, False)
            For Each s As String In mykey.GetSubKeyNames()
                If s = "Implemented Categories" Then
                    Dim arr As Array = mykey.OpenSubKey("Implemented Categories").GetSubKeyNames()
                    For Each s2 As String In arr
                        If s2.ToLower = CLSID Then
                            'this is a CAPE-OPEN UO
                            Dim myuo As New CapeOpenObjInfo
                            With myuo
                                .AboutInfo = mykey.OpenSubKey("CapeDescription").GetValue("About")
                                .CapeVersion = mykey.OpenSubKey("CapeDescription").GetValue("CapeVersion")
                                .Description = mykey.OpenSubKey("CapeDescription").GetValue("Description")
                                .HelpURL = mykey.OpenSubKey("CapeDescription").GetValue("HelpURL")
                                .Name = mykey.OpenSubKey("CapeDescription").GetValue("Name")
                                .VendorURL = mykey.OpenSubKey("CapeDescription").GetValue("VendorURL")
                                .Version = mykey.OpenSubKey("CapeDescription").GetValue("ComponentVersion")
                                .ImplementedCategory = CLSID
                                Try
                                    .TypeName = mykey.OpenSubKey("ProgID").GetValue("")
                                Catch ex As Exception
                                End Try
                                Try
                                    .Location = mykey.OpenSubKey("InProcServer32").GetValue("")
                                Catch ex As Exception
                                    .Location = mykey.OpenSubKey("LocalServer32").GetValue("")
                                End Try
                            End With
                            _coobjects.Add(myuo)
                        End If
                    Next
                End If
            Next
            mykey.Close()
        Next

    End Sub

    Private Sub btnEditThermoServer_Click(sender As Object, e As EventArgs) Handles btnEditThermoServer.Click

        Dim myuo As ICapeUtilities = TryCast(_coes, ICapeUtilities)

        If Not myuo Is Nothing Then
            Try
                myuo.Edit()
            Catch ex As Exception
                MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Object is not editable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        _coobjects.Clear()

        'CAPE-OPEN 1.1 Equilibrium Calculator {cf51e386-0110-4ed8-acb7-b50cfde6908e}
        'SearchCO("{cf51e386-0110-4ed8-acb7-b50cfde6908e}")
        'CAPE-OPEN 1.1 Property Package {cf51e384-0110-4ed8-acb7-b50cfde6908e}
        'SearchCO("{cf51e384-0110-4ed8-acb7-b50cfde6908e}")
        'CAPE-OPEN 1.1 Physical Property Package Managers {cf51e383-0110-4ed8-acb7-b50cfde6908e}
        SearchCO("{cf51e383-0110-4ed8-acb7-b50cfde6908e}")

        Dim f As New FormSelectCOPPM

        f.ListBox1.Items.AddRange(_coobjects.Select(Function(x) x.Name).ToArray)

        If f.ShowDialog() = Windows.Forms.DialogResult.OK Then

            _selppm = _coobjects(f.ListBox1.SelectedIndex)

            Dim t As Type = Type.GetTypeFromProgID(_selppm.TypeName)
            _coppm = Activator.CreateInstance(t)

            Dim myppm As CapeOpen.ICapeUtilities = TryCast(_coppm, CapeOpen.ICapeUtilities)
            If Not myppm Is Nothing Then
                Try
                    myppm.Initialize()
                Catch ex As Exception
                    Dim ecu As CapeOpen.ECapeUser = myppm
                    MessageBox.Show("Error Initializing Property Package Manager - " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MessageBox.Show("CAPE-OPEN Exception " & ecu.code & " at " & ecu.interfaceName & "." & ecu.scope & ". Reason: " & ecu.description)
                End Try
            End If

            Dim proppacks = CType(_coppm, ICapeThermoPropertyPackageManager).GetPropertyPackageList

            Dim f2 As New FormSelectCOPP

            f2.ListBox1.Items.AddRange(proppacks)

            If f2.ShowDialog() = Windows.Forms.DialogResult.OK Then

                Try
                    _coes = CType(_coppm, ICapeThermoPropertyPackageManager).GetPropertyPackage(f2.ListBox1.SelectedItem.ToString)

                    UpdateMappings()

                    Me.lblName2.Text = _coobjects(f.ListBox1.SelectedIndex).Name
                    Me.lblVersion2.Text = _coobjects(f.ListBox1.SelectedIndex).Version
                    Me.lblAuthorURL2.Text = _coobjects(f.ListBox1.SelectedIndex).VendorURL
                    Me.lblDesc2.Text = _coobjects(f.ListBox1.SelectedIndex).Description
                    Me.lblAbout2.Text = _coobjects(f.ListBox1.SelectedIndex).AboutInfo

                    tbSelectedES.Text = _coobjects(f.ListBox1.SelectedIndex).Name & " / " & f2.ListBox1.SelectedItem.ToString

                    _esname = f2.ListBox1.SelectedItem.ToString

                Catch ex As Exception

                    Dim ecu As CapeOpen.ECapeUser = myppm
                    MessageBox.Show("Error initializing CAPE-OPEN Property Package / Equilibrium Server - " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MessageBox.Show("CAPE-OPEN Exception " & ecu.code & " at " & ecu.interfaceName & "." & ecu.scope & ". Reason: " & ecu.description)

                End Try

            End If

            f2.Dispose()

        End If

        f.Dispose()

    End Sub

    Sub UpdateMappings()

        'compounds/components and phases

        If _mappings Is Nothing Then _mappings = New Dictionary(Of String, String)

        'check mappings

        Dim nc As Integer = AvailableCompounds.Count
        Dim mc As Integer = _mappings.Count

        Dim remap As Boolean = False

        If nc <> mc Then
            'remapping necessary
            remap = True
        Else
            For Each c In AvailableCompounds
                If Not _mappings.ContainsKey(c) Then
                    'remapping necessary
                    remap = True
                End If
            Next
        End If

        If remap Then
            _mappings.Clear()
            For Each c In AvailableCompounds
                _mappings.Add(c, "")
            Next
        End If

        Dim cb, cb2 As New DataGridViewComboBoxCell

        Dim complist As Object = Nothing
        Dim formulae As Object = Nothing
        Dim boiltemps As Object = Nothing
        Dim molwts As Object = Nothing
        Dim casids As Object = Nothing
        Dim names As Object = Nothing
        Dim plist As Object = Nothing
        Dim staggr As Object = Nothing
        Dim kci As Object = Nothing

        Dim i As Integer = 0

        If Not _coes Is Nothing Then

            CType(_coes, ICapeThermoCompounds).GetCompoundList(complist, formulae, names, boiltemps, molwts, casids)
            CType(_coes, ICapeThermoPhases).GetPhaseList(plist, staggr, kci)

            For Each s As String In complist
                cb.Items.Add(s)
            Next

            Dim comps = _mappings.Keys.ToArray()

            For Each s As String In comps
                i = 0
                For Each c As String In names
                    If s = c Then
                        _mappings(s) = complist(i)
                    End If
                    i += 1
                Next
            Next

            'try to find matching phases

            If _phasemappings("Vapor").PhaseLabel = "" Then
                i = 0
                For Each s In staggr
                    If s = "Vapor" Then
                        _phasemappings("Vapor").PhaseLabel = plist(i)
                        Exit For
                    End If
                    i += 1
                Next
                If _phasemappings("Vapor").PhaseLabel = "" Then _phasemappings("Vapor").PhaseLabel = "Disabled"
            End If

            If _phasemappings("Liquid1").PhaseLabel = "" Then
                i = 0
                For Each s In staggr
                    If s = "Liquid" Then
                        _phasemappings("Liquid1").PhaseLabel = plist(i)
                        Exit For
                    End If
                    i += 1
                Next
                If _phasemappings("Liquid1").PhaseLabel = "" Then _phasemappings("Liquid1").PhaseLabel = "Disabled"
            End If

            If _phasemappings("Liquid2").PhaseLabel = "" Then
                i = 0
                For Each s In plist
                    If s = "Liquid2" Then
                        _phasemappings("Liquid2").PhaseLabel = plist(i)
                        Exit For
                    End If
                    i += 1
                Next
                If _phasemappings("Liquid2").PhaseLabel = "" Then _phasemappings("Liquid2").PhaseLabel = "Disabled"
            End If

            '_phasemappings("Liquid3").PhaseLabel = "Disabled"
            '_phasemappings("Aqueous").PhaseLabel = "Disabled"

            If _phasemappings("Solid").PhaseLabel = "" Then
                i = 0
                For Each s In staggr
                    If s = "Solid" Then
                        _phasemappings("Solid").PhaseLabel = plist(i)
                        Exit For
                    End If
                    i += 1
                Next
                If _phasemappings("Solid").PhaseLabel = "" Then _phasemappings("Solid").PhaseLabel = "Disabled"
            End If

        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim txt As New Text.StringBuilder

        txt.AppendLine("DWSIM -> Equilibrium Server")
        txt.AppendLine()
        For Each item In _mappings
            txt.AppendLine(item.Key & " -> " & item.Value)
        Next

        MessageBox.Show(txt.ToString, "Compound Mapping", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

End Class