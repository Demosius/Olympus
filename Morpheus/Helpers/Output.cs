using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

            dataTable.TableName = sheetName;

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

    public static void MovesToPDF(Dictionary<string, List<Move>> moves, string fileName = "fileName")
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save PDF File",
                FileName = fileName
            };

            if (dialog.ShowDialog() != true) return;

            fileName = dialog.FileName;

            var dir = Path.GetDirectoryName(fileName);
            dir ??= Directory.GetCurrentDirectory();

            fileName = Path.GetFileName(fileName);

            var extension = Path.GetExtension(fileName);

            fileName = fileName[..^extension.Length];

            foreach (var (title, moveList) in moves)
            {
                var dest = Path.Combine(dir, $"{fileName}_{title}{extension}");
                PDFDocuments.CreateMovePDF(dest, title, moveList);
            }

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