using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Cadmus.Interfaces;

namespace Cadmus.Helpers;

public static class PrintUtility
{
    private static readonly List<ILabelVM> printList = new();
    private static int printCount;

    public static void PrintLabels(IEnumerable<ILabelVM> baseLabels, IEnumerable<ILabelVM>? selectedLabels)
    {
        var labelVMs = baseLabels.ToList();
        var selectedVMs = selectedLabels?.ToList() ?? new List<ILabelVM>();

        if (!labelVMs.Any())
        {
            if (!selectedVMs.Any())
                return;
            labelVMs.AddRange(selectedVMs);
        }

        var printDialog = new PrintDialog
        {
            SelectedPagesEnabled = true,
            UserPageRangeEnabled = true,
            PageRange = new PageRange(1, labelVMs.Count),
            MaxPage = (uint)labelVMs.Count,
            MinPage = 1,
        };

        if (printDialog.ShowDialog() != true) return;

        var q = printDialog.PrintQueue;

        // Set items to be printed.

        var labelsToPrint = new List<ILabelVM>();

        switch (printDialog.PageRangeSelection)
        {
            case PageRangeSelection.AllPages:
                labelsToPrint.AddRange(labelVMs.Select(labelVM => labelVM));
                break;
            case PageRangeSelection.SelectedPages:
                labelsToPrint.AddRange(selectedVMs.Select(labelVM => labelVM));
                break;
            case PageRangeSelection.UserPages:
                for (var i = printDialog.PageRange.PageFrom; i <= printDialog.PageRange.PageTo; i++)
                {
                    labelsToPrint.Add(labelVMs[i - 1]);
                }
                break;
            case PageRangeSelection.CurrentPage:
            default:
                throw new ArgumentOutOfRangeException();
        }

        PrintLabels(q.FullName, labelsToPrint);
    }
    public static void PrintLabels(string fullPrinterName, IEnumerable<ILabelVM> labels)
    {
        printList.Clear();
        printList.AddRange(labels);

        if (!printList.Any()) return;

        /* Try to print label images. */
        printCount = 0;
        var pDoc = new PrintDocument();
        pDoc.PrinterSettings.PrinterName = fullPrinterName;
        pDoc.DefaultPageSettings.PaperSize = printList.First().PaperSize;
        pDoc.PrintPage += PrintImagePage;
        pDoc.Print();
    }

    private static void PrintImagePage(object sender, PrintPageEventArgs ev)
    {
        var label = printList[printCount];
        var labelImage = label.GetLabelImage();

        var pnt = label.PointPlacement;
        ev.Graphics?.DrawImage(labelImage, pnt);

        printCount++;
        // If more lines exist, print another page.
        ev.HasMorePages = printCount < printList.Count;
    }

    public static void PrintDocuments(string fullPrinterName, List<IDocumentVM> documentVMs)
    {
        throw new NotImplementedException();
    }

    public static void ShowPrinter()
    {
        var dict = ShowPrinters();

        var printDialog = new PrintDialog
        {
            SelectedPagesEnabled = true,
            UserPageRangeEnabled = true,
            PageRange = new PageRange(1, 1),
            MaxPage = 1,
            MinPage = 1,
        };

        printDialog.ShowDialog();

        MessageBox.Show($"NAME: {printDialog.PrintQueue.FullName}\n " +
                        $"DESCRIPTION: {printDialog.PrintQueue.Description}\n\n" +
                        $"ScheduleCompletedJobsFirst:\t {printDialog.PrintQueue.ScheduleCompletedJobsFirst}\n" +
                        $"ClientPrintSchemaVersion:\t\t {printDialog.PrintQueue.ClientPrintSchemaVersion}\n" +
                        $"AveragePagesPerMinute:\t\t {printDialog.PrintQueue.AveragePagesPerMinute}\n" +
                        $"IsManualFeedRequired:\t\t {printDialog.PrintQueue.IsManualFeedRequired}\n" +
                        $"PropertiesCollection:\t\t {printDialog.PrintQueue.PropertiesCollection}\n" +
                        $"QueuePrintProcessor:\t\t {printDialog.PrintQueue.QueuePrintProcessor}\n" +
                        $"CurrentJobSettings:\t\t {printDialog.PrintQueue.CurrentJobSettings}\n" +
                        $"DefaultPrintTicket:\t\t\t {printDialog.PrintQueue.DefaultPrintTicket}\n" +
                        $"HostingPrintServer:\t\t {printDialog.PrintQueue.HostingPrintServer}\n" +
                        $"IsPendingDeletion:\t\t {printDialog.PrintQueue.IsPendingDeletion}\n" +
                        $"IsDevQueryEnabled:\t\t {printDialog.PrintQueue.IsDevQueryEnabled}\n" +
                        $"IsOutputBinFull:\t\t\t {printDialog.PrintQueue.IsOutputBinFull}\n" +
                        $"UserPrintTicket:\t\t {printDialog.PrintQueue.UserPrintTicket}\n" +
                        $"DefaultPriority:\t\t {printDialog.PrintQueue.DefaultPriority}\n" +
                        $"HasPaperProblem:\t\t {printDialog.PrintQueue.HasPaperProblem}\n" +
                        $"KeepPrintedJobs:\t\t {printDialog.PrintQueue.KeepPrintedJobs}\n" +
                        $"QueueAttributes:\t\t {printDialog.PrintQueue.QueueAttributes}\n" +
                        $"IsInitializing:\t\t {printDialog.PrintQueue.IsInitializing}\n" +
                        $"UntilTimeOfDay:\t\t {printDialog.PrintQueue.UntilTimeOfDay}\n" +
                        $"InPartialTrust:\t\t {printDialog.PrintQueue.InPartialTrust}\n" +
                        $"StartTimeOfDay:\t\t {printDialog.PrintQueue.StartTimeOfDay}\n" +
                        $"IsBidiEnabled:\t\t {printDialog.PrintQueue.IsBidiEnabled}\n" +
                        $"IsPowerSaveOn:\t\t {printDialog.PrintQueue.IsPowerSaveOn}\n" +
                        $"IsPaperJammed:\t\t {printDialog.PrintQueue.IsPaperJammed}\n" +
                        $"SeparatorFile:\t\t {printDialog.PrintQueue.SeparatorFile}\n" +
                        $"IsProcessing:\t\t {printDialog.PrintQueue.IsProcessing}\n" +
                        $"NumberOfJobs:\t\t {printDialog.PrintQueue.NumberOfJobs}\n" +
                        $"IsDoorOpened:\t\t {printDialog.PrintQueue.IsDoorOpened}\n" +
                        $"IsXpsDevice:\t\t {printDialog.PrintQueue.IsXpsDevice}\n" +
                        $"IsPublished:\t\t {printDialog.PrintQueue.IsPublished}\n" +
                        $"IsWarmingUp:\t\t {printDialog.PrintQueue.IsWarmingUp}\n" +
                        $"QueueStatus:\t\t {printDialog.PrintQueue.QueueStatus}\n" +
                        $"IsPrinting:\t\t {printDialog.PrintQueue.IsPrinting}\n" +
                        $"ShareName:\t\t {printDialog.PrintQueue.ShareName}\n" +
                        $"QueuePort:\t\t {printDialog.PrintQueue.QueuePort}\n" +
                        $"IsInError:\t\t {printDialog.PrintQueue.IsInError}\n" +
                        $"IsPaused:\t\t {printDialog.PrintQueue.IsPaused}\n" +
                        $"Location:\t\t {printDialog.PrintQueue.Location}\n" +
                        $"Priority:\t\t {printDialog.PrintQueue.Priority}\n" +
                        $"HasToner:\t\t {printDialog.PrintQueue.HasToner}\n" +
                        $"PagePunt:\t\t {printDialog.PrintQueue.PagePunt}\n" +
                        $"IsHidden:\t\t {printDialog.PrintQueue.IsHidden}\n" +
                        $"Comment:\t\t {printDialog.PrintQueue.Comment}\n" +
                        $"Parent:\t\t\t {printDialog.PrintQueue.Parent}\n" +
                        $"IsBusy:\t\t\t {printDialog.PrintQueue.IsBusy}\n" +
                        $"Name:\t\t\t {printDialog.PrintQueue.Name}\n" +
                        $"\nIsXpsDevice:\t\t\t {printDialog.PrintQueue.IsXpsDevice}\n" +
                        $"\nQueueAttributes:\t\t\t {printDialog.PrintQueue.QueueAttributes}\n" +
                        $"\nDriverBySystem:\t\t\t {dict[printDialog.PrintQueue.FullName]}\n" +
                        $"\nDriverByDescription?:\t\t\t {printDialog.PrintQueue.Description}\n" +
                        $"\nQueueDriver:\t\t {printDialog.PrintQueue.QueueDriver.Name}\n" +
                        $"\nQueueDriver:\t\t {printDialog.PrintQueue.QueueDriver.Parent}\n" +
                        $"\nQueueDriver:\t\t {printDialog.PrintQueue.QueueDriver}\n");
    }

    public static Dictionary<string, string> ShowPrinters()
    {
        const string query = "SELECT * from Win32_Printer";
        var searcher = new ManagementObjectSearcher(query);
        var coll = searcher.Get();

        var sb = new StringBuilder();

        var dict = new Dictionary<string, string>();

        foreach (var o in coll)
        {
            var printer = (ManagementObject)o;

            var driverName = printer.Properties["DriverName"];
            var printerName = printer.Properties["Name"];
            if (driverName.Value.ToString()!.ToLowerInvariant().Contains("zebra"))
            {
                sb.Append("ZEBRA: ");
            }
            else if (driverName.Value.ToString()!.ToLowerInvariant().Contains("intermec"))
            {
                sb.Append("INTERMEC: ");
            }
            else
            {
                sb.Append("Regular: ");
            }

            sb.AppendLine($"\t{printerName.Name}: {printerName.Value}");
            sb.AppendLine($"\t{driverName.Name}: {driverName.Value}\n");

            dict.Add(printerName.Value.ToString() ?? "", driverName.Value.ToString() ?? "");
        }

        MessageBox.Show(sb.ToString());

        return dict;
    }
}