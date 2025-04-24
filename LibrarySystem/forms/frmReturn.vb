Public Class frmReturn

    Private Sub frmReturn_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Try
            sql = "SELECT br.BorrowerId, BookTitle, DateBorrowed, Purpose, DueDate, BorrowId, br.AccessionNo " &
                  "FROM tblborrow br " &
                  "INNER JOIN tblbooks b ON br.AccessionNo = b.AccessionNo " &
                  "INNER JOIN tblborrower bw ON br.BorrowerId = bw.BorrowerId " &
                  "WHERE br.Status = 'Borrowed' AND COALESCE(br.Due, 0) = 0 " &
                  "ORDER BY BorrowId DESC"
            reloadDtg(sql, dtg_RlistReturn)
            dtg_RlistReturn.Columns(5).Visible = False
            dtg_RlistReturn.Columns(6).Visible = False

            sql = "SELECT bw.BorrowerId, Firstname, Lastname, DateBorrowed, b.AccessionNo, BookTitle, BookDesc, DateReturned " &
                  "FROM tblreturn r " &
                  "INNER JOIN tblborrow br ON r.BorrowId = br.BorrowId " &
                  "INNER JOIN tblbooks b ON br.AccessionNo = b.AccessionNo " &
                  "INNER JOIN tblborrower bw ON br.BorrowerId = bw.BorrowerId " &
                  "WHERE br.Status = 'Returned' " &
                  "ORDER BY ReturnId DESC"
            reloadDtg(sql, dtgListreturned)
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dtg_RlistReturn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles dtg_RlistReturn.Click
        Try
            If dtg_RlistReturn.CurrentRow Is Nothing Then Exit Sub

            sql = "SELECT br.*, b.BookTitle, b.BookDesc, b.Author, bw.Firstname, bw.Lastname " &
                  "FROM tblborrow br " &
                  "INNER JOIN tblbooks b ON br.AccessionNo = b.AccessionNo " &
                  "INNER JOIN tblborrower bw ON br.BorrowerId = bw.BorrowerId " &
                  "WHERE br.BorrowId = " & dtg_RlistReturn.CurrentRow.Cells(5).Value
            reloadtxt(sql)

            If dt.Rows.Count > 0 Then
                With dt.Rows(0)
                    txtRname.Text = .Item("Firstname") & " " & .Item("Lastname")
                    txtRbookTitle.Text = .Item("BookTitle")
                    txtRdescription.Text = .Item("BookDesc")
                    txtRauthor.Text = .Item("Author")
                End With
            End If
        Catch ex As Exception
            MessageBox.Show("Error retrieving book details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btn_Rsave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn_Rsave.Click
        Try
            If String.IsNullOrEmpty(txtRname.Text) Then
                MessageBox.Show("There is no borrower in the fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            sql = "INSERT INTO tblreturn (BorrowId, NoCopies, DateReturned, Remarks) VALUES (" & dtg_RlistReturn.CurrentRow.Cells(5).Value & ", 1, NOW(), 'Returned')"
            If create(sql) Then
                sql = "UPDATE tblborrow SET Status = 'Returned', Remarks = 'On Time' WHERE BorrowId = '" & dtg_RlistReturn.CurrentRow.Cells(5).Value & "'"
                updates(sql)

                sql = "UPDATE tblbooks SET Status = 'Available' WHERE AccessionNo = '" & dtg_RlistReturn.CurrentRow.Cells(6).Value & "'"
                updates(sql)

                MessageBox.Show("Book has been returned to the library.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                frmReturn_Load(sender, e)
                cleartext(grp_Rgroup)
            End If
        Catch ex As Exception
            MessageBox.Show("Error processing return: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtSearchPborrower_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSearchPborrower.TextChanged
        Try
            sql = "SELECT br.BorrowerId, b.BookTitle, br.DateBorrowed, br.Purpose, br.DueDate, br.BorrowId, br.AccessionNo " &
                  "FROM tblborrow br " &
                  "INNER JOIN tblbooks b ON br.AccessionNo = b.AccessionNo " &
                  "INNER JOIN tblborrower bw ON br.BorrowerId = bw.BorrowerId " &
                  "WHERE br.Status = 'Borrowed' AND COALESCE(br.Due, 0) = 0 " &
                  "AND br.BorrowerId LIKE '%" & txtSearchPborrower.Text & "%' " &
                  "ORDER BY br.BorrowId DESC"
            reloadDtg(sql, dtg_RlistReturn)
        Catch ex As Exception
            MessageBox.Show("Error searching borrower: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub txtSearchPborrower_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles txtSearchPborrower.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtRRqty_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles txtRRqty.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

End Class
