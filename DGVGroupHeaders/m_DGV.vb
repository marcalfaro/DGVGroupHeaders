Option Explicit On
Module m_DGV

    ' Creating Group Headers in your Datagridview
    ' Coded by Marc (Github.com/marcalfaro) 

    Private _GroupHeaders As List(Of DGV_GroupHeader) = Nothing
    Private _GroupHeaderHeightMultiplier As Integer

    Public Class DGV_GroupHeader
        Public Text As String
        Public Font As Font
        Public FontColor As Color
        Public BackColor As Color

        Public FirstChildIndex As Integer
        Public LastChildIndex As Integer
    End Class

    Public Function DGV_CreateGroupHeaders(ByVal dgv As DataGridView, ByVal GroupHeaders As List(Of DGV_GroupHeader), Optional ByVal columnHeightMultiplier As Integer = 2) As Tuple(Of Boolean, String)
        'I always use Tuple as functions. Not only do I pass multiple parameters, I also want to get multiple results.
        'You can use a Sub if you want.

        Dim ok As Boolean = False
        Dim erm As String = String.Empty

        Try
            _GroupHeaders = GroupHeaders
            _GroupHeaderHeightMultiplier = columnHeightMultiplier

            With dgv
                DGV_DoubleBuffer(dgv)   'remove flickering

                AddHandler .Paint, AddressOf dgv__Paint_GroupHeaders
                AddHandler .Scroll, AddressOf dgv__InvalidateHeaders
                AddHandler .ColumnWidthChanged, AddressOf dgv__InvalidateHeaders
                AddHandler .Resize, AddressOf dgv__InvalidateHeaders
                AddHandler .CellPainting, AddressOf dgv__CellPainting   'optional. Just me using custom sort icon ;)

                .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
                .ColumnHeadersHeight = 32
                .ColumnHeadersHeight = .ColumnHeadersHeight * columnHeightMultiplier
                .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter
                .AutoGenerateColumns = False
            End With

            ok = True

        Catch ex As Exception
            erm = ex.Message
        End Try

        Return New Tuple(Of Boolean, String)(ok, erm)
    End Function

    Private Sub dgv__Paint_GroupHeaders(ByVal sender As Object, ByVal e As PaintEventArgs)
        Try
            Dim _dgv = CType(sender, DataGridView)

            Dim ttlGrp As Integer = _GroupHeaders.Count        'we will iterate thru your specific column grouping
            For i As Integer = 0 To (ttlGrp - 1)

                Dim foundIndex As Boolean = False
                Dim headerRec As Rectangle = Nothing

                Dim grp As DGV_GroupHeader = _GroupHeaders(i)

                Dim colCtr As Integer = 0
                For index = grp.FirstChildIndex To grp.LastChildIndex

                    If Not foundIndex Then
                        headerRec = _dgv.GetCellDisplayRectangle(DGV_GetColumnByDisplayIndex(_dgv, index).Index, -1, True)     'this is the starting column to paint your new top header
                        If headerRec.Width > 0 Then foundIndex = True
                    Else
                        Dim r2 As Rectangle = _dgv.GetCellDisplayRectangle(DGV_GetColumnByDisplayIndex(_dgv, index).Index, -1, True)   'this holds the rest of the spanned columns
                        headerRec.Width += r2.Width
                    End If
                Next

                If foundIndex AndAlso headerRec.Width > 0 Then

                    headerRec.X += 0                            'Optional. Indent a bit to emphasize the left border
                    headerRec.Y += 0                            'Optional. Indent a bit to emphasize the top border
                    headerRec.Width -= 1                        'Optional. Make the new header's width shorter to emphasize the right border
                    headerRec.Height = (headerRec.Height / _GroupHeaderHeightMultiplier)   'The height of your new painted header

                    Using br As New SolidBrush(grp.BackColor)
                        e.Graphics.FillRectangle(br, headerRec)

                        'Below 2 lines are Optional. They add a gray border to your new painted header
                        Dim myPen As Pen = New Pen(Drawing.Color.Gray, 1)
                        e.Graphics.DrawRectangle(myPen, headerRec)
                    End Using

                    Using br As New SolidBrush(grp.FontColor)

                        Dim sf As New StringFormat()
                        sf.Alignment = StringAlignment.Center
                        sf.LineAlignment = StringAlignment.Center
                        'e.Graphics.DrawString(grp.firstHeaderText, dg.ColumnHeadersDefaultCellStyle.Font, br, headerRec, sf)   'Use this if you want to inherit the default fontStyle

                        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                        e.Graphics.CompositingQuality = Drawing2D.CompositingQuality.HighQuality

                        e.Graphics.DrawString(grp.Text, grp.Font, br, headerRec, sf)
                    End Using

                End If

            Next
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

    End Sub

    Private Sub dgv__InvalidateHeaders(ByVal sender As Object, ByVal e As EventArgs)    'repaint the custom headers
        Dim dg = CType(sender, DataGridView)
        dg.SuspendLayout()
        Dim rtHeader As Rectangle = dg.DisplayRectangle
        rtHeader.Height = dg.ColumnHeadersHeight / 2
        dg.ResumeLayout()
        dg.Invalidate(rtHeader)
    End Sub

    Private Sub dgv__CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        'Draw custom sort icon and place it at the bottom right
        Dim _dgv As DataGridView = sender
        If e.RowIndex = -1 AndAlso e.ColumnIndex > -1 AndAlso _dgv.SortedColumn IsNot Nothing Then
            e.PaintBackground(e.CellBounds, False)
            e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground)
            If _dgv.SortedColumn.Index = e.ColumnIndex Then
                Dim sortIcon As String = IIf(_dgv.SortOrder = SortOrder.Ascending, "▲", "▼")
                Dim sortIconColor As Color = Color.Black
                TextRenderer.DrawText(e.Graphics, sortIcon, e.CellStyle.Font, e.CellBounds, sortIconColor, TextFormatFlags.Bottom + TextFormatFlags.Right)
            End If
            e.Handled = True
        End If
    End Sub

#Region " Helper functions "
    Private Function DGV_GetColumnByDisplayIndex(ByVal dgv As DataGridView, ByVal displayIndex As Integer) As DataGridViewColumn    'Get the correct index of column. useful if you want to reorder the columns
        Dim ret As DataGridViewColumn = Nothing
        Dim ttlC As Integer = dgv.Columns.Count
        If ttlC > 0 Then
            For i As Integer = 0 To ttlC - 1
                If dgv.Columns(i).DisplayIndex = displayIndex Then
                    ret = dgv.Columns(i)
                    Exit For
                End If
            Next
        End If
        Return ret
    End Function

    Private Sub DGV_DoubleBuffer(ByVal DGV As DataGridView) 'by nature, DGV doesn't do doublebuffering. Use this to remove flickering.
        Try
            DGV.GetType.InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.SetProperty, Nothing, DGV, New Object() {True})
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

#End Region

End Module
