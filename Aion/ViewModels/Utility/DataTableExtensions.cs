using System;
using System.Data;
using System.Text;

namespace Aion.ViewModels.Utility;

public static class DataTableExtensions
{
    public static string ToCsv(this DataTable dataTable)
    {
        var sbData = new StringBuilder();

        // Only return Null if there is no structure.
        if (dataTable.Columns.Count == 0)
            return null;

        foreach (var col in dataTable.Columns)
        {
            if (col == null)
                sbData.Append(',');
            else
                sbData.Append("\"" + col.ToString()?.Replace("\"", "\"\"") + "\",");
        }

        sbData.Replace(",", Environment.NewLine, sbData.Length - 1, 1);

        foreach (DataRow dr in dataTable.Rows)
        {
            foreach (var column in dr.ItemArray)
            {
                if (column == null)
                    sbData.Append(',');
                else
                    sbData.Append("\"" + column.ToString()?.Replace("\"", "\"\"") + "\",");
            }
            sbData.Replace(",", Environment.NewLine, sbData.Length - 1, 1);
        }

        return sbData.ToString();
    }

    public static void WriteToCsvFile(this DataTable dataTable, string filePath)
    {
        System.IO.File.WriteAllText(filePath, dataTable.ToCsv());
    }
}