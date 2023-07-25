using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Morpheus.Helpers;

public static class Excel
{
    // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
    // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
    private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
    {
        // If the part does not contain a SharedStringTable, create one.
        shareStringPart.SharedStringTable ??= new SharedStringTable();

        var i = 0;

        // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
        foreach (var item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
        {
            if (item.InnerText == text) return i;
            i++;
        }

        // The text does not exist in the part. Create the SharedStringItem and return its index.
        shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
        shareStringPart.SharedStringTable.Save();

        return i;
    }

    // Given a WorkbookPart, inserts a new worksheet.
    private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
    {
        // Add a new worksheet part to the workbook.
        var newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        newWorksheetPart.Worksheet = new Worksheet(new SheetData());
        newWorksheetPart.Worksheet.Save();

        var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
        var relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

        // Get a unique ID for the new sheet.
        uint sheetId = 1;
        if (sheets.Elements<Sheet>().Any())
        {
            sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        }

        var sheetName = "Sheet" + sheetId;

        // Append the new worksheet and associate it with the workbook.
        var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = sheetName };
        sheets.Append(sheet);
        workbookPart.Workbook.Save();

        return newWorksheetPart;
    }

    // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
    // If the cell already exists, returns it. 
    private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
    {
        var worksheet = worksheetPart.Worksheet;
        var sheetData = worksheet.GetFirstChild<SheetData>();
        var cellReference = columnName + rowIndex;

        // If the worksheet does not contain a row with the specified row index, insert one.
        Row row;
        if (sheetData.Elements<Row>().Any(r => r.RowIndex == rowIndex))
        {
            row = sheetData.Elements<Row>().First(r => r.RowIndex == rowIndex);
        }
        else
        {
            row = new Row { RowIndex = rowIndex };
            sheetData.Append(row);
        }

        // If there is not a cell with the specified column name, insert one.  
        if (row.Elements<Cell>().Any(c => c.CellReference.Value == columnName + rowIndex))
        {
            return row.Elements<Cell>().First(c => c.CellReference.Value == cellReference);
        }

        // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
        var refCell = row.Elements<Cell>()
            .Where(cell => cell.CellReference.Value.Length == cellReference.Length)
            .FirstOrDefault(cell => string.Compare(cell.CellReference.Value, cellReference, StringComparison.OrdinalIgnoreCase) > 0);

        var newCell = new Cell { CellReference = cellReference };
        row.InsertBefore(newCell, refCell);

        worksheet.Save();
        return newCell;
    }

    // Given a document name and text, 
    // inserts a new worksheet and writes the text to cell "A1" of the new worksheet.
    public static void InsertText(string docName, string text)
    {
        // Open the document for editing.
        using var spreadSheet = SpreadsheetDocument.Open(docName, true);

        // Get the SharedStringTablePart. If it does not exist, create a new one.
        var shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Any()
            ? spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First()
            : spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();

        // Insert the text into the SharedStringTablePart.
        var index = InsertSharedStringItem(text, shareStringPart);

        // Insert a new worksheet.
        var worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);

        // Insert cell A1 into the new worksheet.
        var cell = InsertCellInWorksheet("A", 1, worksheetPart);

        // Set the value of cell A1.
        cell.CellValue = new CellValue(index.ToString());
        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

        // Save the new worksheet.
        worksheetPart.Worksheet.Save();
    }

    public static void ExportDataSet(DataSet ds, string destination)
    {
        using var document = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook);

        var _ = document.AddWorkbookPart();

        document.WorkbookPart.Workbook = new Workbook
        {
            Sheets = new Sheets()
        };

        foreach (DataTable table in ds.Tables)
        {

            var sheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            sheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = document.WorkbookPart.GetIdOfPart(sheetPart);

            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Any())
            {
                sheetId =
                    sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
            sheets.Append(sheet);

            var headerRow = new Row();

            var columns = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                columns.Add(column.ColumnName);

                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column.ColumnName)
                };
                headerRow.AppendChild(cell);
            }

            sheetData.AppendChild(headerRow);

            foreach (DataRow dsRow in table.Rows)
            {
                var newRow = new Row();

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var col in columns)
                {
                    var cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(dsRow[col].ToString())
                    };
                    newRow.AppendChild(cell);
                }

                sheetData.AppendChild(newRow);
            }
        }
    }

    public static void CreateSpreadsheetWorkbook(string filepath)
    {
        // Create a spreadsheet document by supplying the filepath.
        // By default, AutoSave = true, Editable = true, and Type = xlsx.
        var spreadsheetDocument = SpreadsheetDocument.
            Create(filepath, SpreadsheetDocumentType.Workbook);

        // Add a WorkbookPart to the document.
        var workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        // Add a WorksheetPart to the WorkbookPart.
        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        // Add Sheets to the Workbook.
        var sheets = spreadsheetDocument.WorkbookPart.Workbook.
            AppendChild(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        var sheet = new Sheet()
        {
            Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = "mySheet"
        };
        sheets.Append(sheet);

        workbookPart.Workbook.Save();

        // Close the document.
        spreadsheetDocument.Close();
    }
}