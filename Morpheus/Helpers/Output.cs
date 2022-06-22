using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Uranus.Inventory;
using Uranus.Inventory.Models;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using ListItem = iText.Layout.Element.ListItem;
using Path = System.IO.Path;
using Table = iText.Layout.Element.Table;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

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

            fileName = dialog.FileName;

            var dir = Path.GetDirectoryName(fileName);
            dir ??= Directory.GetCurrentDirectory();

            fileName = Path.GetFileName(fileName);

            var extension = Path.GetExtension(fileName);

            fileName = fileName[..^extension.Length];

            // Separate moves according to the take site.
            var moveGroups = moves.GroupBy(m => m.TakeSite?.Name ?? "")
                .ToDictionary(g => Path.Combine(dir, $"{fileName}_{g.Key}{extension}"), g => g.ToList());

            foreach (var (dest, moveList) in moveGroups)
            {
                var writer = new PdfWriter(dest);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf, PageSize.A4);

                document.SetMargins(20, 20, 20, 20);

                var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                var table = new Table(new float[] { 3, 9, 5, 9, 27, 5, 5, 3, 6, 9, 15 });

                // Set headers.
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("P", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("Take", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("Done", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("Item No.", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("Description", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("QTY", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("UoM", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("of", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("QTY", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("Place", headerFont, 10)));
                table.AddHeaderCell(new Cell().Add(FormattedParagraph("Comments", headerFont, 10)));

                // Set data.
                foreach (var move in moveList)
                {
                    foreach (var uom in Enum.GetValues(typeof(EUoM)).Cast<EUoM>().ToList())
                    {
                        var qty = uom switch
                        {
                            EUoM.EACH => move.TakeEaches,
                            EUoM.PACK => move.TakePacks,
                            EUoM.CASE => move.TakeCases,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        if (qty <= 0) continue;

                        table.AddCell(new Cell().Add(FormattedParagraph((move.Batch?.Priority ?? 0).ToString(), font, 12)));
                        table.AddCell(new Cell().Add(FormattedParagraph(move.TakeBin?.Code ?? "", font)));
                        table.AddCell(new Cell().Add(FormattedParagraph("", font)));
                        table.AddCell(new Cell().Add(FormattedParagraph(move.ItemNumber.ToString(), font)));
                        table.AddCell(new Cell().Add(FormattedParagraph(move.Item?.Description ?? "", font,7)));
                        table.AddCell(new Cell().Add(FormattedParagraph(qty.ToString(), font)));
                        table.AddCell(new Cell().Add(FormattedParagraph(uom.ToString(), font)));
                        table.AddCell(new Cell().Add(FormattedParagraph("of", font)));
                        table.AddCell(new Cell().Add(FormattedParagraph(uom switch
                        {
                            EUoM.EACH => "1",
                            EUoM.PACK => move.Item?.QtyPerPack.ToString(),
                            EUoM.CASE => move.Item?.QtyPerCase.ToString(),
                            _ => throw new ArgumentOutOfRangeException()
                        } ?? "", font)));
                        table.AddCell(new Cell().Add(FormattedParagraph(move.PlaceBin?.Code ?? "", font)));
                        table.AddCell(new Cell().Add(FormattedParagraph("", font)));
                    }

                }

                document.Add(table);
                document.Close();
            }

            CreateHwPdf(Path.Combine(dir, "HelloWorld.pdf"));

            CreateRickPdf(Path.Combine(dir, "Rick.pdf"));

            CreateQbfPdf(Path.Combine(dir, "QuickBrownFox.pdf"));

            /*var doc = new Document();

            var section = doc.AddSection();

            section.AddParagraph("Hello World");
            section.AddParagraph();

            var paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            paragraph.AddFormattedText("Hello, World!", TextFormat.Underline);

            var ft = paragraph.AddFormattedText("Small text", TextFormat.Bold);
            ft.Font.Size = 6;

            var pdfRenderer = new PdfDocumentRenderer(false)
            {
                Document = doc
            };

            pdfRenderer.RenderDocument();

            pdfRenderer.PdfDocument.Save(dialog.FileName);*/

            /*var pdf = new PdfDocument();

            var page = pdf.AddPage();

            var graph = XGraphics.FromPdfPage(page);

            var font = new XFont("Verdana", 20, XFontStyle.Bold);

            graph.DrawString("This is my first PDF document", font, XBrushes.Black,
                new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

            pdf.Save(dialog.FileName);*/

            // Success.
            MessageBox.Show($"Successfully Exported to file:\n\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        catch (Exception ex)
        {
            // Failure.
            MessageBox.Show($"Failed to export file.\n\n{ex}", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static Paragraph FormattedParagraph(string text, PdfFont font, int fontSize = 9,
        VerticalAlignment verticalAlignment = VerticalAlignment.MIDDLE,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.CENTER)
    {
        return new Paragraph(text).SetFont(font).SetFontSize(fontSize).SetVerticalAlignment(verticalAlignment).SetHorizontalAlignment(horizontalAlignment);
    }

    public static void CreateHwPdf(string dest)
    {
        //Initialize PDF writer
        var writer = new PdfWriter(dest);
        //Initialize PDF document
        var pdf = new PdfDocument(writer);
        // Initialize document
        var document = new Document(pdf);
        //Add paragraph to the document
        document.Add(new Paragraph("Hello World!"));
        //Close document
        document.Close();
    }

    public static void CreateRickPdf(string dest)
    {
        //Initialize PDF writer
        var writer = new PdfWriter(dest);
        //Initialize PDF document
        var pdf = new PdfDocument(writer);
        // Initialize document
        var document = new Document(pdf);
        // Create a PdfFont
        var font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
        // Add a Paragraph
        document.Add(new Paragraph("iText is:").SetFont(font));
        // Create a List
        var list = new List().SetSymbolIndent(12).SetListSymbol("\u2022").SetFont(font);
        // Add ListItem objects
        list.Add(new ListItem("Never gonna give you up")).Add(new ListItem("Never gonna let you down")).Add(new ListItem
            ("Never gonna run around and desert you")).Add(new ListItem("Never gonna make you cry")).Add(new ListItem
            ("Never gonna say goodbye")).Add(new ListItem("Never gonna tell a lie and hurt you"));
        // Add the list
        document.Add(list);
        //Close document
        document.Close();
    }

    public static void CreateQbfPdf(string dest)
    {
        const string dogJpg = "./Resources/img/dog.jpg";

        const string foxPng = "./Resources/img/fox.png";

        //Initialize PDF writer
        var writer = new PdfWriter(dest);
        //Initialize PDF document
        var pdf = new PdfDocument(writer);
        // Initialize document
        var document = new Document(pdf);
        // Compose Paragraph
        var fox = new Image(ImageDataFactory.Create(foxPng));
        var dog = new Image(ImageDataFactory.Create(dogJpg));
        var p = new Paragraph("The quick brown ").Add(fox).Add(" jumps over the lazy ").Add(dog);
        // Add Paragraph to document
        document.Add(p);
        //Close document
        document.Close();
    }
}