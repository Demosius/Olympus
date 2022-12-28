using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Management;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using Cadmus.Annotations;
using Cadmus.Models;
using Cadmus.ViewModels.Commands;
using Cadmus.Views.Labels;
using Microsoft.Win32.SafeHandles;
using Uranus;

namespace Cadmus.ViewModels.Controls;

public class ReceivingPutAwayVM : INotifyPropertyChanged, IPrintable
{
    public List<ReceivingPutAwayLabel> Labels { get; set; }

    #region INotifyPropertyChanged Members


    private ObservableCollection<ReceivingPutAwayLabelVM> labelVMs;
    public ObservableCollection<ReceivingPutAwayLabelVM> LabelVMs
    {
        get => labelVMs;
        set
        {
            labelVMs = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ReceivingPutAwayLabelVM> SelectedLabels { get; set; }

    #endregion

    #region Commands

    public PrintCommand PrintCommand { get; set; }
    public ShowPrinterCommand ShowPrinterCommand { get; set; }

    #endregion

    public ReceivingPutAwayVM()
    {
        Labels = new List<ReceivingPutAwayLabel>();
        labelVMs = new ObservableCollection<ReceivingPutAwayLabelVM>();
        SelectedLabels = new ObservableCollection<ReceivingPutAwayLabelVM>();

        PrintCommand = new PrintCommand(this);
        ShowPrinterCommand = new ShowPrinterCommand(this);

        GenerateTestData();
    }

    // TODO: Delete Test Method
    public void GenerateTestData()
    {
        const string s = @"Zone	Bin	Case	Pack	Each	QPC	QPP	Barcode	Item	LabelNo	LabelTotal	Description
OZ	G AL 1	12	0	0	54	0	Í4Å9uÎ	209725	1	1	PLUSH MINEC OCELOT 14IN
OZ	I AH 3	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AH 3	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AK 2	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AK 2	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AL 2	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AL 2	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AM 2	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AM 2	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	G AB 3	24	0	0	24	0	Í4Å;{Î	209727	1	2	PLUSH MINEC WOLF 15IN
OZ	G AB 3	24	0	0	24	0	Í4Å;{Î	209727	2	2	PLUSH MINEC WOLF 15IN
OZ	F BF 3	25	0	0	6	0	Í;Ya=Î	275765	1	2	STAT HALO MASTER CHIEF W/GRAP
OZ	F BF 3	25	0	0	6	0	Í;Ya=Î	275765	2	2	STAT HALO MASTER CHIEF W/GRAP
OZ	G AA 2	0	36	0	0	0	Í:FuRÎ	263885	1	2	REP MARV THOR STORMBREAKER
OZ	G AA 2	0	36	0	0	0	Í:FuRÎ	263885	2	2	REP MARV THOR STORMBREAKER
OZ	G AB 2	0	36	0	0	0	Í:FuRÎ	263885	1	2	REP MARV THOR STORMBREAKER
OZ	H AF 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AG 3	16	0	0	20	0	Í7?crÎ	233167	1	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AG 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AH 3	16	0	0	20	0	Í7?crÎ	233167	1	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AH 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AI 3	16	0	0	20	0	Í7?crÎ	233167	1	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AI 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AA 2	23	0	0	36	6	Í;.%hÎ	271405	1	1	POP SW MANDO CHILD BUTTERFLY
OZ	H AV 4	63	2	0	2	0	Í:zH3Î	269040	1	2	REP SW MANDALORIAN HELMET
OZ	H AV 4	63	2	0	2	0	Í:zH3Î	269040	2	2	REP SW MANDALORIAN HELMET
OZ	H AV 5	64	0	0	2	0	Í:zH3Î	269040	1	2	REP SW MANDALORIAN HELMET
OZ	H AV 5	64	0	0	2	0	Í:zH3Î	269040	2	2	REP SW MANDALORIAN HELMET
OZ	H AW 2	64	0	0	2	0	Í:zH3Î	269040	1	2	REP SW MANDALORIAN HELMET
OZ	H AW 2	64	0	0	2	0	Í:zH3Î	269040	2	2	REP SW MANDALORIAN HELMET
OZ	F AR 4	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AR 5	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AS 4	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AS 5	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AT 5	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AS 1	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	F AS 1	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	F AS 2	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	F AS 2	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	F AT 1	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	F AT 1	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	G AE 1	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	G AE 1	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	G AG 3	29	0	0	8	0	Í9<_bÎ	252863	1	1	RING FIT ADVENTURE NS
OZ	F BL 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BO 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BO 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BP 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BP 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BQ 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BQ 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BR 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BR 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	H AC 1	26	0	0	4	2	Í:}F3Î	269338	1	1	GLASS POKE 25 KANTO 4PK
OZ	ASPLEY1	180	0	0	6	0	Í9""KYÎ	250243	1	1	LAMP ZELDA MASTER SWORD
OZ	ASPLEY7	450	0	0	4	2	Í;""3zÎ	270219	1	1	GLASS SW VADER'S FREE TIME 4PK
OZ	ASPLEY6	528	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
";

        var dt = DataConversion.RawStringToTable(s);

        foreach (DataRow row in dt.Rows)
        {
            var takeZone = row["Zone"].ToString() ?? "";
            var takeBin = row["Bin"].ToString() ?? "";
            var caseQty = int.Parse(row["Case"].ToString() ?? "");
            var packQty = int.Parse(row["Pack"].ToString() ?? "");
            var eachQty = int.Parse(row["Each"].ToString() ?? "");
            var qpc = int.Parse(row["QPC"].ToString() ?? "");
            var qpp = int.Parse(row["QPP"].ToString() ?? "");
            var barcode = row["Barcode"].ToString() ?? "";
            var item = int.Parse(row["Item"].ToString() ?? "");
            var labelNo = int.Parse(row["LabelNo"].ToString() ?? "");
            var labelTotal = int.Parse(row["LabelTotal"].ToString() ?? "");
            var description = row["Description"].ToString() ?? "";

            var label = new ReceivingPutAwayLabel(
                takeZone: takeZone,
                takeBin: takeBin,
                caseQty: caseQty,
                packQty: packQty,
                eachQty: eachQty,
                qtyPerCase: qpc,
                qtyPerPack: qpp,
                barcode: barcode,
                itemNumber: item,
                labelNumber: labelNo,
                labelTotal: labelTotal,
                description: description);

            var lvm = new ReceivingPutAwayLabelVM(label);

            LabelVMs.Add(lvm);
        }

    }

    public void Print()
    {
        var printDialog = new PrintDialog
        {
            SelectedPagesEnabled = true,
            UserPageRangeEnabled = true,
            PageRange = new PageRange(1, LabelVMs.Count),
            MaxPage = (uint)LabelVMs.Count,
            MinPage = 1,
        };

        if (printDialog.ShowDialog() != true) return;

        var xpsDocWriter = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);

        var printList = new List<UIElement>();

        // Set items to be printed.
        switch (printDialog.PageRangeSelection)
        {
            case PageRangeSelection.AllPages:
                printList.AddRange(LabelVMs.Select(labelVM => new ReceivingPutAwayLabelView(labelVM)));
                break;
            case PageRangeSelection.SelectedPages:
                printList.AddRange(SelectedLabels.Select(labelVM => new ReceivingPutAwayLabelView(labelVM)));
                break;
            case PageRangeSelection.UserPages:
                for (var i = printDialog.PageRange.PageFrom; i <= printDialog.PageRange.PageTo; i++)
                {
                    var label = new ReceivingPutAwayLabelView(LabelVMs[i - 1]);
                    printList.Add(label);
                }
                break;
            case PageRangeSelection.CurrentPage:
            default:
                throw new ArgumentOutOfRangeException();
        }

        
        PrintUIElements(xpsDocWriter, printList);
    }

    public void ShowPrinter()
    {
        var dict = ShowPrinters();

        var printDialog = new PrintDialog
        {
            SelectedPagesEnabled = true,
            UserPageRangeEnabled = true,
            PageRange = new PageRange(1, LabelVMs.Count),
            MaxPage = (uint)LabelVMs.Count,
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
            /*foreach (PropertyData prop in printer.Properties)
            {
                sb.Append(string.Format("{0}: {1}", prop.Name, prop.Value));
                sb.Append("U+002CU+0020");
            }*/

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

    /// <summary>
    /// Used the XpsDocumentWriter to write a FixedDocument which get created and contains the UIElements as single pages
    /// </summary>
    /// <param name="xpsWriter">XpsDocumentWriter</param>
    /// <param name="uiElements">List of UIElement</param>
    private static void PrintUIElements(SerializerWriter xpsWriter, List<UIElement> uiElements)
    {
        var fixedDoc = new FixedDocument();

        foreach (var element in uiElements)
        {
            var fixedPage = new FixedPage();
            var pageContent = new PageContent();

            // add the UIElement object the FixedPage
            fixedPage.Children.Add(element);

            // add the FixedPage object the PageContent
            pageContent.Child = fixedPage;

            // add the PageContent object the FixedDocument
            fixedDoc.Pages.Add(pageContent);
        }
        
        xpsWriter.Write(fixedDoc);
    }

    private static void PrintToLabel(PrintQueue printQueue, List<UIElement> uiElements)
    {
        var fixedDoc = new FixedDocument();

        foreach (var element in uiElements)
        {
            var fixedPage = new FixedPage();
            var pageContent = new PageContent();

            // add the UIElement object the FixedPage
            fixedPage.Children.Add(element);

            // add the FixedPage object the PageContent
            pageContent.Child = fixedPage;

            // add the PageContent object the FixedDocument
            fixedDoc.Pages.Add(pageContent);
        }
        
        RawPrinterHelper.SendFileToPrinter(printQueue.FullName, fixedDoc.);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}