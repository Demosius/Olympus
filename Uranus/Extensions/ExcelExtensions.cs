using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Uranus.Extensions;

public static class ExcelWorksheetExtension
{
    public static string[] GetHeaderColumns(this ExcelWorksheet sheet)
    {
        return sheet.Cells[sheet.Dimension.Start.Row, sheet.Dimension.Start.Column, 1, sheet.Dimension.End.Column]
            .Select(firstRowCell => firstRowCell.Text)
            .ToArray();
    }
}