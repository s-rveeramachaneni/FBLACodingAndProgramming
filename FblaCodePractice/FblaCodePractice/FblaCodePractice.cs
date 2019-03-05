using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace FblaCodePractice
{
    public partial class FblaCodePractice : Form
    {
        //The following blocks of code instantiates variables we need to keep track of our data (students, books, redemptions)
        public DataTable dtStudents = new DataTable();
        public DataSet dsStudents = new DataSet();
        public static string StudentFileName = Directory.GetCurrentDirectory()+@"\\students.csv";

        public DataTable dtBooks = new DataTable();
        public DataSet dsBooks = new DataSet();
        public static string BooksFileName = Directory.GetCurrentDirectory() + @"\\Books.csv";

        public static string RedeemFileName = Directory.GetCurrentDirectory() + @"\\Redemptions.csv";

        private String currentStudentId = ""; //This instantiates an instance variable to keep track of a selected student's id 
        
        public FblaCodePractice()
        {
            InitializeComponent(); //This constructor intializes the project
        }
        
        public void LoadStudentData()
        {
            //The following blocks of code set up and load the data in the manner we want
            dsStudents.Clear();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(StudentFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter StudentsAdapter = new OleDbDataAdapter("Select * from Students.csv", connection);
            StudentsAdapter.Fill(dsStudents);
            DataColumn columnStudentId = dsStudents.Tables[0].Columns[0];

            columnStudentId.AutoIncrement = true; //this makes it so that student Ids are automatically created and incremented
            columnStudentId.ReadOnly = true; //this makes it so that student Ids cannot be edited by the user in the data grid view

            //the following block of code makes it so that null values won't be accepted
            dsStudents.Tables[0].Columns[1].AllowDBNull = false;
            dsStudents.Tables[0].Columns[2].AllowDBNull = false;
            dsStudents.Tables[0].Columns[3].AllowDBNull = false;

            dataGridViewStudents.DataSource = dsStudents.Tables[0]; //this sets up the data grid view with the data we want shown

            connection.Close();
        }
        
        public void AddStudent()
        {
            //The following block of code sets up our data so that we can use it to add new data
            dsStudents.Clear();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(StudentFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter StudentsAdapter = new OleDbDataAdapter("Select * from Students.csv", connection);
            StudentsAdapter.Fill(dsStudents);
            StudentsAdapter.InsertCommand = new OleDbCommand("INSERT INTO   Students.csv (StudentId, FirstName, LastName, Grade) VALUES (?, ?, ?, ?)", connection);

            StudentsAdapter.InsertCommand.Parameters.Add("@StudentId", OleDbType.VarChar, 255).SourceColumn = "StudentId";
            StudentsAdapter.InsertCommand.Parameters.Add("@FirstName", OleDbType.VarChar, 255).SourceColumn = "Firstname";
            StudentsAdapter.InsertCommand.Parameters.Add("@LastName", OleDbType.VarChar, 255).SourceColumn = "Lastname";
            StudentsAdapter.InsertCommand.Parameters.Add("@Grade", OleDbType.VarChar, 255).SourceColumn = "Grade";

            //The following block of code actually adds the new row with the appropriate values given (first name, last name, grade) or set (student Id) 
            dsStudents.Tables[0].Columns[0].AutoIncrement = true;
            DataRow newPersonRow = dsStudents.Tables[0].NewRow();
            newPersonRow["FirstName"] = firstNameBlank.Text;
            newPersonRow["LastName"] = lastNameBlank.Text;
            newPersonRow["Grade"] = gradeSelection.Text;
            dsStudents.Tables[0].Rows.Add(newPersonRow);

            StudentsAdapter.Update(dsStudents);

        }
        
        private void updateStudentFileFromGrid(object sender)
        {

            DataGridView dgv = sender as DataGridView;
            // The following code makes it so that if no data is returned it doesn't save
            if (dgv.Rows.Count == 0)
            {
                return;
            }

            //This string builder will build in the information we want the file to update with
            StringBuilder sb = new StringBuilder();

            //The following block of code builds the column headers again and appends it to the stringbuilder
            string columnsHeader = "";
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (dgv.Columns.Count - 1 != i)
                    columnsHeader += dgv.Columns[i].Name + ",";
                else
                    columnsHeader += dgv.Columns[i].Name;

            }
            sb.Append(columnsHeader + Environment.NewLine);

            //The following block of code goes through each cell in the datagridview and checks if its an empty row or not. If it isn't, it appends the cell data with commas to delimit and adds a new line as necessary to the string builder
            for (int j = 0; j < dgv.Rows.Count; j++)
            {
                DataGridViewRow dgvRow = dgv.Rows[j];
                if (!dgvRow.IsNewRow)
                {
                    for (int c = 0; c < dgvRow.Cells.Count; c++)
                    {
                        if (c < dgvRow.Cells.Count - 1)
                            sb.Append(dgvRow.Cells[c].Value + ",");
                        else
                            sb.Append(dgvRow.Cells[c].Value);
                    }
                    if (j < dgv.Rows.Count - 1)
                        sb.Append(Environment.NewLine);
                }
            }
            
            //The following block of code assigns the stringbuilder content to the file. Once completed, a confirmation message pops up.
            string StudentFileName2 = StudentFileName;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(StudentFileName2, false))
            {
                sw.WriteLine(sb.ToString());
            }
            MessageBox.Show("Changes have been saved.");
        }
        
        private void dataGridViewStudents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //The following code checks if the user edited a value to be null. If they did it doesn't update the data file and triggers a pop up telling the user about the error
            DataGridView dgv = dataGridViewStudents;
            try
            {
                string checking = (String)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            catch
            {
                MessageBox.Show("Please edit values to non-null inputs");
                return;
            }
            //The following line of code makes it so that if the changed value wasn't null, it adds the new value to the data set
            updateStudentFileFromGrid(sender);
        }
        
        private void dataGridViewStudents_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //The following line of code calls the update method to accomodate for the change
            updateStudentFileFromGrid(sender);
        }
        
        public void AddRedemptionCode(int studentId, string redemptionCode, string studentFirstName, string studentLastName)
        {
            //The following blocks of code set up our data for both redeem and books so that we can use them to add new data
            DataSet dsRedeem = new DataSet();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(RedeemFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter RedeemAdapter = new OleDbDataAdapter("Select * from Redemptions.CSV", connection);
            RedeemAdapter.Fill(dsRedeem);

            DataSet dsBooks = new DataSet();
            OleDbConnection ConnBooks = new OleDbConnection(connectionString);
            ConnBooks.Open();
            OleDbDataAdapter BooksAdapter = new OleDbDataAdapter("Select * from Books.CSV", connection);
            BooksAdapter.Fill(dsBooks);

            StringBuilder sb = new StringBuilder();

            //The following block of code builds the column headers again and appends it to the stringbuilder
            string columnsHeader = "";
            DataColumnCollection DC = dsBooks.Tables[0].Columns;
            for (int i = 0; i < DC.Count; i++)
            {
                if (DC.Count - 1 != i)
                    columnsHeader += DC[i].ColumnName + ",";
                else
                    columnsHeader += DC[i].ColumnName;

            }
            sb.Append(columnsHeader + Environment.NewLine);

            //The following block of code goes through each cell in the books datagridview and checks for the redemption code passed. If it is found, it checks if redeemed is true and if it is a popup comes up to tell the user the redemption code has already been used. If it isn't true, it appends all data but the redeemed cell data with commas to delimit, appends redeemed as TRUE, and adds a new line as necessary to the string builder
            DataRowCollection BookRows = dsBooks.Tables[0].Rows;
            bool RedemptioncodeFound = false;
            string bookTitle = "";
            string authorName = "";
            for (int j = 0; j < BookRows.Count; j++)
            {
                DataRow dr = BookRows[j];
                bool turnoffredemption = false;
                if (dr.ItemArray[3].ToString() == redemptionCode)
                {
                    RedemptioncodeFound = true;
                    bookTitle = (string)dr.ItemArray[1];
                    authorName = (string)dr.ItemArray[2];

                    if (dr.ItemArray[4].ToString() == "TRUE")
                    {
                        MessageBox.Show("The redemption code \" " + redemptionCode + "\" has already been redeemed");
                        return;
                    }
                    turnoffredemption = true;
                }

                for (int c = 0; c < dr.ItemArray.Length; c++)
                {
                    if (c < dr.ItemArray.Length - 1)
                        sb.Append((string)dr.ItemArray[c] + ",");
                    else
                    {
                        if (!turnoffredemption)
                            sb.Append((string)dr.ItemArray[c]);
                        else
                        {
                            turnoffredemption = false;
                            sb.Append("TRUE");
                        }
                    }
                }
                if (j < BookRows.Count - 1)
                    sb.Append(Environment.NewLine);
            }

            //The following block of code triggers a popup telling the user to enter valid, stored codes if the redemption code wasn't found in the data. 
            if (!RedemptioncodeFound)
            {
                MessageBox.Show("Please enter a valid, stored redemption code.");
                return;
            }

            //The following block of code assigns the stringbuilder content to the file. Once completed, a confirmation message pops up.
            string FileName2 = BooksFileName;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(FileName2, false))
            {
                sw.WriteLine(sb.ToString());
            }

            //The following block sets up our data in redemptions so that we can add new data. It then actually adds the new row with the appropriate values given (student id, redemption code, student first name, student last name, book title, book author) and triggers a pop up to let users know the addition was successful unless catch is triggered, in which case a pop up message lets the user know about their error
            try
            {
                dsRedeem.Tables[0].Columns[1].Unique = true;

                RedeemAdapter.InsertCommand = new OleDbCommand("INSERT INTO redemptions.csv (StudentId, RedemptionCode, StudentFirstName, StudentLastName, BookTitle, BookAuthor) VALUES (?, ?, ?, ?, ?, ?)", connection);

                RedeemAdapter.InsertCommand.Parameters.Add("@StudentId", OleDbType.VarChar, 255).SourceColumn = "StudentId";
                RedeemAdapter.InsertCommand.Parameters.Add("@RedemptionCode", OleDbType.VarChar, 255).SourceColumn = "RedemptionCode";
                RedeemAdapter.InsertCommand.Parameters.Add("@StudentFirstName", OleDbType.VarChar, 255).SourceColumn = "StudentFirstName";
                RedeemAdapter.InsertCommand.Parameters.Add("@StudentLastName", OleDbType.VarChar, 255).SourceColumn = "StudentLastName";
                RedeemAdapter.InsertCommand.Parameters.Add("@BookTitle", OleDbType.VarChar, 255).SourceColumn = "BookTitle";
                RedeemAdapter.InsertCommand.Parameters.Add("@BookAuthor", OleDbType.VarChar, 255).SourceColumn = "BookAuthor";

                DataRow newRedeemnRow = dsRedeem.Tables[0].NewRow();
                newRedeemnRow["StudentId"] = studentId;
                newRedeemnRow["RedemptionCode"] = redemptionCode;
                newRedeemnRow["StudentFirstName"] = studentFirstName;
                newRedeemnRow["StudentLastName"] = studentLastName;
                newRedeemnRow["BookTitle"] = bookTitle;
                newRedeemnRow["BookAuthor"] = authorName;

                dsRedeem.Tables[0].Rows.Add(newRedeemnRow);

                MessageBox.Show("The redemption of code \"" + redemptionCode + "\" was successful.");
            }
            catch
            {

                MessageBox.Show("The redemption code \" " + redemptionCode + "\" has already been redeemed");
            }
            RedeemAdapter.Update(dsRedeem);

            connection.Close();
        }
        
        public String getRedemptionID(int studentId)
        {
            //The following block of code sets up our data so that we can access it
            DataSet dsRedeem = new DataSet();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(RedeemFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter RedeemAdapter = new OleDbDataAdapter("Select * from Redemptions.CSV", connection);
            RedeemAdapter.Fill(dsRedeem);

            //The following block of code runs through all the data and checks for the passed studentId. Each time it is found, the redemption code associated with that student id is appended to the string builder 
            StringBuilder sbRC = new StringBuilder();
            int rowCount = dsRedeem.Tables[0].Rows.Count;
            foreach (DataRow row in dsRedeem.Tables[0].Rows)
            {
                if (row[0].Equals(""+studentId))
                {
                    sbRC.Append((string)row[1] + " ");
                }
            }
            connection.Close();

            //The following block of code checks if stringbuilder contains anything. If it doesn't, "NONE" is returned. If it does then its tostring method result is returned
            if (sbRC.ToString().Equals(""))
                return "NONE";
            else
                return sbRC.ToString();
        }
        
        public void LoadBooksData()
        {
            //The following blocks of code set up and load the data in the manner we want
            dsBooks.Clear();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(BooksFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter BooksAdapter = new OleDbDataAdapter("Select * from Books.csv", connection);
            BooksAdapter.Fill(dsBooks);
            DataColumn columnBookId = dsBooks.Tables[0].Columns[0];
            DataColumn columnBookTitle = dsBooks.Tables[0].Columns[1];
            DataColumn columnAuthorName = dsBooks.Tables[0].Columns[2];
            DataColumn columnRedeemed = dsBooks.Tables[0].Columns[4];

            //The following block of code makes it so that book Id, book title, author's last name, and redeemed status can't be edited by the user in the data grid view
            columnBookId.ReadOnly = true; 
            columnBookTitle.ReadOnly = true;
            columnAuthorName.ReadOnly = true;
            columnRedeemed.ReadOnly = true;

            //The following block of code makes it so that null values won't be accepted
            dsBooks.Tables[0].Columns[1].AllowDBNull = false;
            dsBooks.Tables[0].Columns[2].AllowDBNull = false;
            dsBooks.Tables[0].Columns[3].AllowDBNull = false;
            dsBooks.Tables[0].Columns[4].AllowDBNull = false;

            dataGridViewBooks.DataSource = dsBooks.Tables[0]; //this sets up the data grid view with the data we want shown

            connection.Close();

        }
        
        public void AddBooks()
        {
            //The following block of code sets up our data so that we can use it to add new data
            dsBooks.Clear();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(BooksFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter BooksAdapter = new OleDbDataAdapter("Select * from Books.csv", connection);
            BooksAdapter.Fill(dsBooks);
            BooksAdapter.InsertCommand = new OleDbCommand("INSERT INTO   Books.csv (BookId, BookTitle, AuthorLastName, RedemptionCode, Redeemed) VALUES (?, ?, ?, ?, ?)", connection);
            BooksAdapter.InsertCommand.Parameters.Add("@BookId", OleDbType.VarChar, 255).SourceColumn = "BookId";
            BooksAdapter.InsertCommand.Parameters.Add("@BookTitle", OleDbType.VarChar, 255).SourceColumn = "BookTitle";
            BooksAdapter.InsertCommand.Parameters.Add("@AuthorLastName", OleDbType.VarChar, 255).SourceColumn = "AuthorLastName";
            BooksAdapter.InsertCommand.Parameters.Add("@RedemptionCode", OleDbType.VarChar, 255).SourceColumn = "RedemptionCode";
            BooksAdapter.InsertCommand.Parameters.Add("@Redeemed", OleDbType.VarChar, 255).SourceColumn = "Redeemed";

            //The following block of code actually adds the new row with the appropriate values given (book Id, book title, author's last name, redemption code) or set (redeemed) for as many times as there are copies
            TextBox[] redemptionCodes = { redemptionCode1, redemptionCode2, redemptionCode3, redemptionCode4, redemptionCode5 };
            for (int i = 0; i < numberOfCopiesIncrement.Value; i++)
            {
                DataRow newBooksRow = dsBooks.Tables[0].NewRow();
                newBooksRow["BookId"] = bookIdBlank.Text;
                newBooksRow["BookTitle"] = bookTitleBlank.Text;
                newBooksRow["AuthorLastName"] = authorBlank.Text;
                newBooksRow["RedemptionCode"] = redemptionCodes[i].Text;
                newBooksRow["Redeemed"] = "FALSE";
                dsBooks.Tables[0].Rows.Add(newBooksRow);
            }
            BooksAdapter.Update(dsBooks);

        }
        
        private void updateBooksFileFromGrid(object sender)
        {
            DataGridView dgv = sender as DataGridView;
            // The following code makes it so that if no data is returned it doesn't save
            if (dgv.Rows.Count == 0)
            {
                return;
            }

            //This string builder will build in the information we want the file to update with
            StringBuilder sb = new StringBuilder();

            //The following block of code builds the column headers again and appends it to the stringbuilder
            string columnsHeader = "";
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (dgv.Columns.Count - 1 != i)
                    columnsHeader += dgv.Columns[i].Name + ",";
                else
                    columnsHeader += dgv.Columns[i].Name;
            }
            sb.Append(columnsHeader + Environment.NewLine);

            //The following block of code goes through each cell in the datagridview and checks if its an empty row or not. If it isn't, it appends the cell data with commas to delimit and adds a new line as necessary to the string builder
            for (int j = 0; j < dgv.Rows.Count; j++)
            {
                DataGridViewRow dgvRow = dgv.Rows[j];
                if (!dgvRow.IsNewRow)
                {
                    for (int c = 0; c < dgvRow.Cells.Count; c++)
                    {
                        if (c < dgvRow.Cells.Count - 1)
                            sb.Append(dgvRow.Cells[c].Value + ",");
                        else
                            sb.Append(dgvRow.Cells[c].Value);
                    }
                    if (j < dgv.Rows.Count - 1)
                        sb.Append(Environment.NewLine);
                }
            }

            //The following block of code assigns the stringbuilder content to the file. Once completed, a confirmation message pops up.
            string BooksFileName2 = BooksFileName;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(BooksFileName2, false))
            {
                sw.WriteLine(sb.ToString());
            }
            MessageBox.Show("Changes have been saved.");
        }
        
        private void dataGridViewBooks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //The following code checks if the user edited a value to be null. If they did it doesn't update the data file and triggers a pop up telling the user about the error
            DataGridView dgv = dataGridViewBooks;
            try
            {
                string checking = (String)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            catch
            {
                MessageBox.Show("Please edit values to non-null inputs");
                return;
            }

            //The following block of code makes it so that if the changed value has already been used as a redemption code, a pop up comes up that tells the user to enter a unique code. The old data is then loaded back. If the changed value is a unique redemption code, it adds the new value to the data set
            String redemptionCode = (String)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (isRedemptionCodeInvalid(sender, redemptionCode))
            {
                MessageBox.Show("Please only change redemption codes to unique ones.");
                LoadBooksData();
            }
            else
            updateBooksFileFromGrid(sender);
        }
        
        private void dataGridViewBooks_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //The following line of code calls the update method to accomodate for the change
            updateBooksFileFromGrid(sender);
        }
        
        private void addStudentButton_Click_1(object sender, EventArgs e)
        {
            String errorMessage = ""; //This string is used to build our error messages

            //The following block of code is used to check for errors (no first name entered, no last name entered, no grade chosen)
            if (firstNameBlank.Text.Trim().Equals(""))
                errorMessage += "Please enter a first name \n";
            if (lastNameBlank.Text.Trim().Equals(""))
                errorMessage += "Please enter a last name \n";
            if (gradeSelection.Text.Equals(""))
                errorMessage += "Please select a grade \n";

            //The following block of code shows a popup with the error message if there were any errors. If there weren't it calls the method to add a student and clears the information blanks for new entries.
            if (!errorMessage.Equals(""))
                MessageBox.Show(errorMessage);
            else
            {
                AddStudent();
                firstNameBlank.Clear();
                lastNameBlank.Clear();
            }
        }
        
        private void addBooksButton_Click(object sender, EventArgs e)
        {
            String errorMessage = ""; //This string is used to build our error messages
            TextBox[] redemptionCodes = { redemptionCode1, redemptionCode2, redemptionCode3, redemptionCode4, redemptionCode5 }; //This array hosts all the objects storing redemption codes so that we can use a loop to iterate through them later

            //The following block of code is used to check for errors (no book title entered, no last name entered, no book id generated, invalid redemption codes/not enough codes)
            if (bookTitleBlank.Text.Trim().Equals(""))
                errorMessage += "Please enter a book title \n";
            if (authorBlank.Text.Trim().Equals(""))
                errorMessage += "Please enter the author's last name \n";
            if (bookIdBlank.Text.Trim().Equals(""))
                errorMessage += "Please generate the book Id \n";
            for (int i = 0; i < numberOfCopiesIncrement.Value; i++)
            {
                if(isRedemptionCodeInvalid(sender, redemptionCodes[i].Text))
                    errorMessage += "Please create a valid and unique redemption code in row "+(i+1)+" \n";
            }

            //The following block of code shows a popup with the error message if there were any errors. If there weren't it calls the method to add books and clears the redemption code blanks for new entries.
            if (!errorMessage.Equals(""))
                MessageBox.Show(errorMessage);
            else
            {
                AddBooks();
                for (int i = 0; i < redemptionCodes.Length; i++)
                    redemptionCodes[i].Text = "";
            }
        }
        
        private bool isRedemptionCodeInvalid(object sender, String redemptionCodeText)
        {
            redemptionCodeText = redemptionCodeText.Trim(); //This stores a trimmed version of the redemption code for future uses

            //The following code block returns that the code is not valid if nothing is entered as the code
            if (redemptionCodeText.Equals(""))
                return true;

            // The following code instantiates and goes through data cells for books and checks if the redemption code entered matches with any already listed. If it does match with another redemption code, it returns that the redemption code is inavlid.
            DataGridView dgv = dataGridViewBooks;
            for (int j = 0; j < dgv.Rows.Count; j++)
            {
                DataGridViewRow dgvRow = dgv.Rows[j];
                if (!dgvRow.IsNewRow)
                {
                    if (redemptionCodeText== (String) dgvRow.Cells[3].Value)
                        return true;
                }
            }
            //If the redemption code hasn't triggered a return that it is invalid yet, it returns that the code is valid
            return false;
        }
        
        private void makeAllAnswersInvisible()
        {
            // The following block of code makes sure that answers to all questions are reset to be invisible.
            aAddStudent.Visible = false;
            aEditStudent.Visible = false;
            aDeleteStudent.Visible = false;
            aViewStudent.Visible = false;
            aViewRedemptionCodes.Visible = false;
            aAssignRedemptionCode.Visible = false;
            aAddEBook.Visible = false;
            aDeleteBook.Visible = false;
            aAssignStudent.Visible = false;
            aMultipleUses.Visible = false;
            aEditRedemptionCode.Visible = false;
            aGenerateReport.Visible = false;
            aPrintReport.Visible = false;
        }
        
        private void helpStudent_Click(object sender, EventArgs e)
        {
            // The following block of code makes the questions of the other sections invisible.
            qAddEBook.Visible = false;
            qEditRedemptionCode.Visible = false;
            qDeleteBook.Visible = false;
            qAssignStudent.Visible =false;
            qMultipleUses.Visible = false;
            qGenerateReport.Visible = false;
            qPrintReport.Visible = false;

            // The following block of code switches the visibility of specific help questions regarding the selected topic of students.
            qAddStudent.Visible = !qAddStudent.Visible;
            qEditStudent.Visible = !qEditStudent.Visible;
            qDeleteStudent.Visible = !qDeleteStudent.Visible;
            qViewStudent.Visible = !qViewStudent.Visible;
            qViewRedemptionCodes.Visible = !qViewRedemptionCodes.Visible;
            qAssignRedemptionCode.Visible = !qAssignRedemptionCode.Visible;

            //The following line of code calls a method to reset all answers to invisible. This is done to have a clean screen when the section is opened or closed.
            makeAllAnswersInvisible();   
        }
        
        private void helpEBooks_Click(object sender, EventArgs e)
        {
            // The following block of code makes the questions of the other sections invisible.
            qAddStudent.Visible = false;
            qEditStudent.Visible = false;
            qDeleteStudent.Visible = false;
            qViewStudent.Visible = false;
            qViewRedemptionCodes.Visible = false;
            qAssignRedemptionCode.Visible = false;
            qGenerateReport.Visible = false;
            qPrintReport.Visible = false;

            // The following block of code switches the visibility of specific help questions regarding the selected topic of E-Books.
            qAddEBook.Visible = !qAddEBook.Visible;
            qEditRedemptionCode.Visible = !qEditRedemptionCode.Visible;
            qDeleteBook.Visible = !qDeleteBook.Visible;
            qAssignStudent.Visible = !qAssignStudent.Visible;
            qMultipleUses.Visible = !qMultipleUses.Visible;

            //The following line of code calls a method to reset all answers to invisible. This is done to have a clean screen when the section is opened or closed.
            makeAllAnswersInvisible();
        }
        
        private void helpReport_Click(object sender, EventArgs e)
        {
            // The following block of code makes the questions of the other sections invisible.
            qAddStudent.Visible = false;
            qEditStudent.Visible = false;
            qDeleteStudent.Visible = false;
            qViewStudent.Visible = false;
            qViewRedemptionCodes.Visible = false;
            qAssignRedemptionCode.Visible = false;
            qAddEBook.Visible = false;
            qEditRedemptionCode.Visible = false;
            qDeleteBook.Visible = false;
            qAssignStudent.Visible = false;
            qMultipleUses.Visible = false;

            // The following block of code switches the visibility of specific help questions regarding the selected topic of E-Books.
            qGenerateReport.Visible = !qGenerateReport.Visible;
            qPrintReport.Visible = !qPrintReport.Visible;

            //The following line of code calls a method to reset all answers to invisible. This is done to have a clean screen when the section is opened or closed.
            makeAllAnswersInvisible();
        }
        
        private void qAddStudent_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aAddStudent.Visible = !aAddStudent.Visible;
        }
        
        private void qEditStudent_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aEditStudent.Visible = !aEditStudent.Visible;
        }
        
        private void qDeleteStudent_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aDeleteStudent.Visible = !aDeleteStudent.Visible;
        }
        
        private void qViewStudent_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aViewStudent.Visible = !aViewStudent.Visible;
        }
        
        private void qViewRedemptionCodes_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aViewRedemptionCodes.Visible = !aViewRedemptionCodes.Visible;
        }
        
        private void qAssignRedemptionCode_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aAssignRedemptionCode.Visible = !aAssignRedemptionCode.Visible;
        }
        
        private void qAddEBook_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aAddEBook.Visible = !aAddEBook.Visible;
        }
        
        private void qEditRedemptionCode_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aEditRedemptionCode.Visible = !aEditRedemptionCode.Visible;
        }
        
        private void qDeleteBook_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aDeleteBook.Visible = !aDeleteBook.Visible;
        }
        
        private void qAssignStudent_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aAssignStudent.Visible = !aAssignStudent.Visible;
        }
        
        private void qMultipleUses_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aMultipleUses.Visible = !aMultipleUses.Visible;
        }
        
        private void qGenerateReport_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aGenerateReport.Visible = !aGenerateReport.Visible;
        }
        
        private void qPrintReport_Click(object sender, EventArgs e)
        {
            // The following code switches the visibility of the answer to the selected question.
            aPrintReport.Visible = !aPrintReport.Visible;
        }
        
        private void bookIdBlank_Click(object sender, EventArgs e)
        {
            //The following if statement ensures that the needed author and title fields are filled properly before the id is generated. 
            if (authorBlank.Text.Trim().Equals("") || bookTitleBlank.Text.Trim().Equals(""))
            {
                bookIdBlank.Clear(); //This clears the blank for the next use.
                //The following line of code triggers a popup to let users know what they need to fix in order to generate an Id.
                MessageBox.Show("Fill out the Author and Title section properly before generating an Id");
            }
            else
            {
                //The following instantiates a random object so that we can generate a random number for the next step. 
                Random random = new System.Random();

                //The following blocks of code instantiate and build the bookId by assigning it to the first three letters of the author's last name and the first three letters on the book's title.
                //If either has less than three characters, then whatever is present is used to build the id instead
                String bookId = "";
                if (authorBlank.Text.Length > 3)
                    bookId = authorBlank.Text.Substring(0, 3);
                else
                    bookId = authorBlank.Text;

                if (bookTitleBlank.Text.Length > 3)
                    bookId += bookTitleBlank.Text.Substring(0, 3);
                else
                    bookId += bookTitleBlank.Text;

                //The following line of code adds a random 4 digit number to the end of the built book id and assigns it to the text of the textbox.  
                bookIdBlank.Text = bookId + random.Next(1000, 10000);
            }
        }
        
        private void printButton_Click(object sender, EventArgs e)
        {
            //The following code triggers the print document dialog. If the user approves the print job it then calls the actual print function.
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //The following code generates and formats the text for the printed document
            e.Graphics.DrawString(generatedReport.Text, new Font("Arial", 12), Brushes.Black, 150, 125);
        }
        
        public String redemptionCodeStudentsReport()
        {
            //The following block of code sets up our data so that we can use it
            DataSet dsRedeem = new DataSet();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(RedeemFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter RedeemAdapter = new OleDbDataAdapter("Select * from Redemptions.CSV", connection);
            RedeemAdapter.Fill(dsRedeem);

            //The following blocks of code build a string that contains the report information in the format we want and then returns it once we have all data.
            String reportText = "-------------------------\r\n\r\nBooks and Redemption Codes Assigned to Students\r\n\r\n-------------------------\r\n\r\n";

            DataRowCollection RedeemRows = dsRedeem.Tables[0].Rows;

            for (int i = 0; i < RedeemRows.Count; i++)
            {
                DataRow dr = RedeemRows[i];
                reportText += "\"" + dr.ItemArray[1] + "\" redemption code for book \"" + dr.ItemArray[4] + "\" by " + dr.ItemArray[5] + "\r\nRedeemed by student #" + dr.ItemArray[0] + ": " + dr.ItemArray[2] + " " + dr.ItemArray[3] + "\r\n\r\n";
            }
            return reportText;
        }
        
        public String unassignedRedemptionCodesReport()
        {
            //The following block of code sets up our data so that we can use it
            DataSet dsBooks = new DataSet();
            string connectionString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0}; Extended Properties = \"text;HDR=Yes;FMT=Delimited\"", Path.GetDirectoryName(BooksFileName));
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            OleDbDataAdapter BooksAdapter = new OleDbDataAdapter("Select * from Books.CSV", connection);
            BooksAdapter.Fill(dsBooks);


            //The following blocks of code build a string that contains the report information in the format we want and then returns it once we have all the data we want (books with a false value regarding redemption)
            String reportText = "-------------------------\r\n\r\nBooks and Redemption Codes Still Unassigned\r\n\r\n-------------------------\r\n\r\n";

            DataRowCollection BooksRows = dsBooks.Tables[0].Rows;

            for (int i = 0; i < BooksRows.Count; i++)
            {
                DataRow dr = BooksRows[i];
                if(dr.ItemArray[4].Equals("FALSE"))
                reportText += "\"" + dr.ItemArray[3] + "\" redemption code for book \"" + dr.ItemArray[1] + "\" by " + dr.ItemArray[2] + "\r\nIs still unredeemed"+ "\r\n\r\n";
            }
            return reportText;
        }
        
        private void generateReportButton_Click(object sender, EventArgs e)
        {
            String errorMessage = ""; //This string is used to build our error messages

            //The following block of code is used to check for errors(no content chosen, no choice made for timestamp)
            if (reportContentOptions.CheckedItems.Count==0)
                errorMessage += "Please select a box for what content you want to include in the report \n";
            if (!includeTime.Checked && !dontIncludeTime.Checked)
                errorMessage += "Please choose an option regarding inclusion of a time stamp \n";

            //The following block of code shows a popup with the error message if there were any errors. If there weren't it checks what the user selected for content and timestamp and calls corresponding methods to build a string. The string is then set to display in the generate report area.
            if (!errorMessage.Equals(""))
                MessageBox.Show(errorMessage);
            else
            {
                String fullMessage = "Weekly Report";
                if (includeTime.Checked)
                    fullMessage += " || " + DateTime.Now.ToString() + "\r\n\r\n";
                else
                    fullMessage += "\r\n\r\n";

                if (reportContentOptions.GetItemChecked(0))
                    fullMessage += redemptionCodeStudentsReport();
                if (reportContentOptions.GetItemChecked(1))
                    fullMessage += unassignedRedemptionCodesReport();

                generatedReport.Text = fullMessage;

            }
        }
        
        private void FblaCodePractice_Load(object sender, EventArgs e)
        {
            //The following block of code calls other methods to load student data and books data to their data grid views
            LoadStudentData();
            LoadBooksData();
        }
        
        private void dataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            //The following block of code checks the selected row in the student data grid view and assigns or places the information in other locations for show/use (id to instance variable, first name to blank, last name to blank, grade to dropdown)
            DataGridView gv = dataGridViewStudents;
            if (gv != null && gv.SelectedRows.Count > 0)
            {
                DataGridViewRow row = gv.SelectedRows[0];
                currentStudentId = ""+row.Cells[0].Value;
                firstNameBlank.Text= (String)row.Cells[1].Value;
                lastNameBlank.Text = (String)row.Cells[2].Value;
                gradeSelection.Text = ""+ row.Cells[3].Value;
            }
        }
        
        private void viewStudentButton_Click(object sender, EventArgs e)
        {
            //The following block of code checks if a student is selected by checking the value of the student id variable. If a student is not selected, a pop up message tells the user to select one. If a student was selected, a pop up message shows student information (full name, student id, student grade, assingned redemption codes)
            if (!currentStudentId.Equals(""))
            {
                int studentIdInt;
                int.TryParse(currentStudentId, out studentIdInt); 
                MessageBox.Show("Student's Name: " + firstNameBlank.Text + " " + lastNameBlank.Text + "\nStudent's Id: " + currentStudentId + "\nStudent's Grade: " + gradeSelection.Text + "\nRedemption Codes Assigned: " + getRedemptionID(studentIdInt));
            }
            else
                MessageBox.Show("Please select a student in the chart.");
        }

        private void firstNameBlank_Click(object sender, EventArgs e)
        {
            //The following line of code resets the student Id to an empty string
            currentStudentId = "";
        }

        private void lastNameBlank_Click(object sender, EventArgs e)
        {
            //The following line of code resets the student Id to an empty string
            currentStudentId = "";
        }

        private void gradeSelection_Click(object sender, EventArgs e)
        {
            //The following line of code resets the student Id to an empty string
            currentStudentId = "";
        }

        private void redeemCodeButton_Click(object sender, EventArgs e)
        {
            String errorMessage = ""; //This string is used to build our error messages

            //The following block of code is used to check for errors (no code entered, no row selected)
            if (redemptionCodeBlank.Text.Trim().Equals(""))
                errorMessage += "Please enter a code\n";
            if (currentStudentId.Equals(""))
                errorMessage += "Please select a student's row\n";

            //The following block of code shows a popup with the error message if there were any errors. If there weren't it calls the method to add a redemption code and refreshes the view of the book table.
            if (!errorMessage.Equals(""))
                MessageBox.Show(errorMessage);
            else
            {
                int studentIdInt;
                int.TryParse(currentStudentId, out studentIdInt);
                AddRedemptionCode(studentIdInt, redemptionCodeBlank.Text, firstNameBlank.Text, lastNameBlank.Text);
                LoadBooksData();
            }
        }

    }
}