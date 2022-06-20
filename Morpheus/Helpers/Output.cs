using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using Uranus.Inventory.Models;

namespace Morpheus.Helpers;

public static class Output
{
    public static void DataTableToCSV(DataTable dataTable, string fileName = "fileName")
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Save CSV File",
                FileName = fileName
            };

            if (dialog.ShowDialog() != true) return;

            dataTable.WriteToCsvFile(dialog.FileName);

            // Success.
            MessageBox.Show($"Successfully Exported to file:\n\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        catch (Exception ex)
        {
            // Failure.
            MessageBox.Show($"Failed to export file.\n\n{ex}", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void DataTableToExcel(DataTable dataTable, string fileName = "fileName", string sheetName = "sheetName")
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Save Excel File",
                FileName = fileName
            };

            if (dialog.ShowDialog() != true) return;

            var ds = new DataSet();
            ds.Tables.Add(dataTable);

            Excel.ExportDataSet(ds, dialog.FileName);

            // Success.
            MessageBox.Show($"Successfully Exported to file:\n\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        catch (Exception ex)
        {
            // Failure.
            MessageBox.Show($"Failed to export file.\n\n{ex}", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void MovesToPDF(IEnumerable<Move> moves, string fileName = "fileName")
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var dialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save PDF File",
                FileName = fileName
            };

            if (dialog.ShowDialog() != true) return;

            var pdf = new PdfDocument();

            var page = pdf.AddPage();

            var graph = XGraphics.FromPdfPage(page);

            var font = new XFont("Verdana", 20, XFontStyle.Bold);

            graph.DrawString("This is my first PDF document", font, XBrushes.Black,
                new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            pdf.Save(dialog.FileName);

            // Success.
            MessageBox.Show($"Successfully Exported to file:\n\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        catch (Exception ex)
        {
            // Failure.
            MessageBox.Show($"Failed to export file.\n\n{ex}", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}