Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        PopulateGrid()          'Populate your grid. Nothing special.

        CreateGroupHeaders()    'This is the magical procedure :D

    End Sub

    Private Sub PopulateGrid()
        'If you want to populate from database, then use datasource
        'dgv.DataSource = CreateBlankDataTable(8, 5)

        'but for the purpose of this example, I'll be creating the grid manually...
        With myDGV
            With .Columns
                .Add(New DataGridViewCheckBoxColumn With {.Name = "chk", .HeaderText = "", .Frozen = True, .Width = 40})            '0
                .Add(New DataGridViewTextBoxColumn With {.Name = "SN", .HeaderText = "sn", .Frozen = True, .Width = 40})            '1
                .Add(New DataGridViewTextBoxColumn With {.Name = "SR", .HeaderText = "Sales Rep", .Frozen = True, .Width = 100})    '2

                .Item("chk").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Item("SN").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Item("SR").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

                .Add(New DataGridViewTextBoxColumn With {.Name = "FYTarget", .HeaderText = "FY Target"})        '3
                .Add(New DataGridViewTextBoxColumn With {.Name = "FYActual", .HeaderText = "FY Actual"})        '4
                .Add(New DataGridViewTextBoxColumn With {.Name = "PtoGoal", .HeaderText = "%-to-Goal"})         '5
                .Add(New DataGridViewTextBoxColumn With {.Name = "Month", .HeaderText = "Month"})               '6

                .Add(New DataGridViewTextBoxColumn With {.Name = "ExpM", .HeaderText = "Expected"})     '7
                .Add(New DataGridViewTextBoxColumn With {.Name = "ActM", .HeaderText = "Actual"})       '8
                .Add(New DataGridViewTextBoxColumn With {.Name = "AchM", .HeaderText = "% Achieved"})   '9
                .Add(New DataGridViewTextBoxColumn With {.Name = "BalM", .HeaderText = "Balance"})      '10

                .Add(New DataGridViewTextBoxColumn With {.Name = "ExpQ", .HeaderText = "Expected"})     '11
                .Add(New DataGridViewTextBoxColumn With {.Name = "ActQ", .HeaderText = "Actual"})       '12
                .Add(New DataGridViewTextBoxColumn With {.Name = "AchQ", .HeaderText = "% Achieved"})   '13
                .Add(New DataGridViewTextBoxColumn With {.Name = "BalQ", .HeaderText = "Balance"})      '14

                .Add(New DataGridViewTextBoxColumn With {.Name = "ExpFY", .HeaderText = "Expected"})     '15
                .Add(New DataGridViewTextBoxColumn With {.Name = "ActFY", .HeaderText = "Actual"})       '16
                .Add(New DataGridViewTextBoxColumn With {.Name = "AchFY", .HeaderText = "% Achieved"})   '17
                .Add(New DataGridViewTextBoxColumn With {.Name = "BalFY", .HeaderText = "Balance"})      '18

                .Add(New DataGridViewTextBoxColumn With {.Name = "BO", .HeaderText = "Backorder"})            '19
                .Item("BO").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With

            For i As Integer = 1 To 5
                .Rows.Add({False, i, $"Person {i}",
                          i * 10000, (i * 10000) / 2, (((i * 10000) / 2) / 100) * 100, MonthName(Now.Month),
                          i * 10000, (i * 10000) / 2, (((i * 10000) / 2) / 100) * 100, 111,
                          i * 10000, (i * 10000) / 2, (((i * 10000) / 2) / 100) * 100, 222,
                          i * 10000, (i * 10000) / 2, (((i * 10000) / 2) / 100) * 100, 333,
                          999})
            Next

        End With

    End Sub

    Private Sub CreateGroupHeaders()
        Dim fontToUse As Font = New Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point)

        Dim ghs As New List(Of DGV_GroupHeader)
        With ghs
            .Add(New DGV_GroupHeader With {.Text = "Summary", .Font = fontToUse, .FontColor = Color.Black, .BackColor = ColorTranslator.FromHtml("#C6E0B4"), .FirstChildIndex = 3, .LastChildIndex = 6})
            .Add(New DGV_GroupHeader With {.Text = "Month Performance", .Font = fontToUse, .FontColor = Color.Black, .BackColor = ColorTranslator.FromHtml("#DDEBF7"), .FirstChildIndex = 7, .LastChildIndex = 10})
            .Add(New DGV_GroupHeader With {.Text = "Quarter Performance", .Font = fontToUse, .FontColor = Color.Black, .BackColor = ColorTranslator.FromHtml("#BDD7EE"), .FirstChildIndex = 11, .LastChildIndex = 14})
            .Add(New DGV_GroupHeader With {.Text = "FY Performance", .Font = fontToUse, .FontColor = Color.Black, .BackColor = ColorTranslator.FromHtml("#9BC2E6"), .FirstChildIndex = 15, .LastChildIndex = 18})
        End With

        DGV_CreateGroupHeaders(myDGV, ghs, 2)
    End Sub

End Class
