# FBLA Coding and Programming
by Rishika Veeramachaneni
# Documentation
## How to run the program
Download the ZIP file and unizip it. Then go inside FblaCodePractice, FblaCode Practice, bin, and debug. Once there open FblaCodePractice.exe to run the program
## How to view the source code
Download the ZIP file and unizip it. Then go inside FblaCodePractice. Once there open FblaCodePractice.sln with visual studio. You will need to download visual studio seperately for this. 
## How to reset the program's data
If you want to reset the data after running the program, first close the program. Then go back in to the debug file (FblaCodePractice/FblaCodePractice/bin/debug) and open Redemptions.csv, Books.csv, and students.csv as notepad files. Delete all content aside from the headers and make sure you don't leave any blank spaces. Then save the files, close them, and run the program again.
## How to use the program (Help can also be found on the help page within the program)
### How to manage students
To add a student, fill in the information requested on the students tab page and press add student. The student should then show up in the data grid view and will be added to the file. 
###
To delete a student, select their row in the data grid view and press the delete key. The student will then disappear from the grid view and from the file.
###
To edit a student's information, select a cell and type in your new value. Click away and the data grid view and data will update.
###
To view all student information, including assigned redemption codes, select the student's row and then press the view student button. Any clicks to edit blanks will require you to select the student's row again. 
### How to manage E-books
To add an E-book, fill in the information requested on the books tab page and press add book. You will need to click on the id blank to generate one before adding the book. The book should then show up in the data grid view and will be added to the file. 
###
To delete an E-book, select their row in the data grid view and press the delete key. The book will then disappear from the grid view and from the file.
###
To edit a book's redemption code, select the cell and type in your new code. Click away and the data grid view and data will update. You can not change it to a null value or a code already assigned to another book.
### How to assign redemption codes
On the students tab, select a student's row. Then type in the code in the provided blank and press the redeem button. The redemption codes you can use will be displayed on the books tab in the data grid view. If a redemption code has a FALSE value for redeemed in that data grid view, it is still valid for use. 
### How to generate report
Before generating a report, you must select the content you want to display. You can include information on books (and codes) assigned to students or/and information on books still not redeemed. You can also add in a time stamp to the report title if you want. After filling the information, press generate report and a view of it will show up in the right text box. 
### How to print a report
First, generate a report. Check the view to see if you are satisfied with your report. If you are, press print report and follow through the dialogue to finish the print job. 
### How to use the help page
Based on the topic you need to help with, select one of the three main topics: Students, E-Books and Report. Questions related to that topic will then pop up. Click on one if you want to see the answer. You can keep multiple answers from one topic open at a time. Clicking on an active question or header will make it disappear. 
### Advanced Features
If you want to sort data within the data view tables, you can click on the headers to order them by that trait. This works for both the students and books data grid view. 
###
You can delete multiple rows at a time if you select more than one. This can allow for faster management of data. 
###
Error messages will naturally pop up if the program is not used as intended. They will feature very specific instructions on all matters you need to fix. 
