using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace III_ProjectOne
{
    class ProcessExceltoDt
    {
        public static DataTable ConvertToDT (string path,string sheetName,Label labelMessage)
        {
           


            //Create a new DataTable.
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                //Open the Excel file in Read Mode using OpenXML
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(path, false))
                {
                    WorksheetPart titlesWorksheetPart = GetWorksheetPart(doc.WorkbookPart, sheetName);

                    Worksheet titlesWorksheet = titlesWorksheetPart.Worksheet;

                    //Fetch all the rows present in the worksheet
                    IEnumerable<Row> rows = titlesWorksheet.GetFirstChild<SheetData>().Descendants<Row>();

                    foreach (Cell cell in rows.ElementAt(0))
                    {
                        GlobalVariable.cancellationToken.ThrowIfCancellationRequested();


                        string headerName = GetCellValue(doc, cell);
                        Console.WriteLine(headerName);
                        dt.Columns.Add(headerName.ToString().Trim()); // this will include 2nd row a header row
                    }

                    string message = labelMessage.Text;
                    int counter = 0;
                    //Loop through the Worksheet rows
                    foreach (Row row in rows)
                    {
                        GlobalVariable.cancellationToken.ThrowIfCancellationRequested();


                        counter += 1;
                        LabelText.UpdateText(labelMessage, message + "Rows read(" + counter+")");

                        if (row.RowIndex.Value > 1) //this will exclude first two rows
                        {
                            System.Data.DataRow tempRow = dt.NewRow();
                            int columnIndex = 0;
                            foreach (Cell cell in row.Descendants<Cell>())
                            {
                                // Gets the column index of the cell with data
                                int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                                cellColumnIndex--; //zero based index
                                if (columnIndex < cellColumnIndex)
                                {
                                    do
                                    {
                                        tempRow[columnIndex] = ""; //Insert blank data here;
                                        columnIndex++;
                                    }
                                    while (columnIndex < cellColumnIndex);
                                }
                                tempRow[columnIndex] = GetCellValue(doc, cell);

                                columnIndex++;
                            }
                            dt.Rows.Add(tempRow);
                        }
                    }
                }
                LogMessage.Log("Reading excel successful.");
            }
            catch(Exception ex)
            {
               LogMessage.Log(ex.Message);
               LogMessage.Log(ex.StackTrace);
               GlobalVariable.errorStatus = true; 

            }

            return dt;
        }
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            if (cell.CellValue == null)
            {
                return "";
            }
            string value = cell.CellValue.InnerXml;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {

                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
               

                return value.ToString().Trim();
            }
        }
        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        
        /// <returns>Column Name (ie. B)</returns>
        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }
        /// <summary>
        /// Given just the column name (no row index), it will return the zero based column index.
        /// Note: This method will only handle columns with a length of up to two (ie. A to Z and AA to ZZ). 
        /// A length of three can be implemented when needed.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful; otherwise null</returns>
        public static int? GetColumnIndexFromName(string columnName)
        {
            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }

        public static WorksheetPart GetWorksheetPart(WorkbookPart workbookPart, string sheetName)
        {
            string relId = workbookPart.Workbook.Descendants<Sheet>().First(s => sheetName.Equals(s.Name)).Id;
            return (WorksheetPart)workbookPart.GetPartById(relId);
        }

    }
}

