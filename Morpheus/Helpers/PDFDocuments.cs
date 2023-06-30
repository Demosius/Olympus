using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Inventory;
using Uranus.Inventory.Models;
using Cell = iText.Layout.Element.Cell;
using Color = iText.Kernel.Colors.Color;
using Table = iText.Layout.Element.Table;

namespace Morpheus.Helpers;

public static class PDFDocuments
{
    // ReSharper disable StringLiteralTypo
    public static FontProgram CalibriFontProgram => FontProgramFactory.CreateFont("./Resources/Fonts/Calibri.ttf");
    public static FontProgram CalibriBoldFontProgram => FontProgramFactory.CreateFont("./Resources/Fonts/calibrib.ttf");
    public static FontProgram CalibriItalicFontProgram = FontProgramFactory.CreateFont("./Resources/Fonts/calibril.ttf");
    // ReSharper restore StringLiteralTypo

    public static PdfFont Calibri => PdfFontFactory.CreateFont(CalibriFontProgram, PdfEncodings.WINANSI);
    public static PdfFont CalibriBold => PdfFontFactory.CreateFont(CalibriBoldFontProgram, PdfEncodings.WINANSI);
    public static PdfFont CalibriItalic => PdfFontFactory.CreateFont(CalibriItalicFontProgram, PdfEncodings.WINANSI);
    public static PdfFont Helvetica => PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
    public static PdfFont HelveticaBold => PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

    public static void CreateMovePDF(string destinationPath, string title, List<Move> moveList)
    {
        var writer = new PdfWriter(destinationPath);
        var pdf = new PdfDocument(writer);
        // Turn off event handler for now, and manage header/footer after document creation.
        // pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new SampleEventHandler());
        var document = new Document(pdf, PageSize.A4);

        document.SetMargins(50, 20, 30, 20);

        var standardFont = Calibri;
        var headerFont = CalibriBold;
        var priorityFont = CalibriItalic;

        var table = new Table(new float[] { 7, 11, 5, 11, 29, 7, 5, 3, 8, 11, 20 });

        var headColour = ColorConstants.GRAY;

        // Set headers.
        var headers = new[] { "P", "Take", "Done", "Item No.", "Description", "QTY", "UoM", "of", "QTY", "Place", "Comments" };

        foreach (var header in headers)
            table.AddHeaderCell(CreateCell(CreateParagraph(header, headerFont, 12), TextAlignment.CENTER, headColour));

        var lineNo = 0;

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
                    _ => throw new ArgumentOutOfRangeException(nameof(uom), @"UoM value outside of expected value range.")
                };
                if (qty <= 0) continue;

                ++lineNo;

                var colour = lineNo % 2 == 0
                    ? null
                    : ColorConstants.LIGHT_GRAY;

                var priority = move.Priority;
                var font = priority is 0 or 1
                    ? priorityFont
                    : standardFont;

                var underline = priority == 0;

                var qtyPerUnit = uom switch
                {
                    EUoM.EACH => 1,
                    EUoM.PACK => move.Item?.QtyPerPack,
                    EUoM.CASE => move.Item?.QtyPerCase,
                    _ => throw new ArgumentOutOfRangeException(nameof(uom), @"UoM value outside of expected value range.")
                } ?? 1;

                table.AddCell(CreateCell(CreateParagraph(priority.ToString(), font, underline: underline), TextAlignment.CENTER).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph(move.TakeBin?.Code ?? "", font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph("")));
                table.AddCell(CreateCell(CreateParagraph(move.ItemNumber.ToString(), font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph(move.Item?.Description ?? "", font, underline: underline), TextAlignment.LEFT, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph(qty.ToString(), font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph(uom.ToString(), font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph("of", font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph(qtyPerUnit.ToString(), font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph(move.PlaceBin?.Code ?? "", font, underline: underline), TextAlignment.CENTER, colour).SetBorder(null));
                table.AddCell(CreateCell(CreateParagraph("")).SetBorder(null).SetBorderBottom(new SolidBorder(0.5f)));
            }
        }

        table.SetWidth(UnitValue.CreatePercentValue(100));

        document.Add(table);

        // Add footer afterwards (Page x of y) as total page number is unknown during creation.
        // Add header while we're here, because why not.
        var numberOfPages = pdf.GetNumberOfPages();
        var pageSize = pdf.GetPage(1).GetPageSize();    // Assume, as we know, that all the pages are the same size.
        var centerX = (pageSize.GetLeft() + document.GetLeftMargin() + (pageSize.GetRight() - document.GetRightMargin())) / 2;
        var footY = document.GetBottomMargin() / 2;
        var headerY = pageSize.GetTop() - document.GetTopMargin() / 2;
        var dateX = pageSize.GetWidth() * 0.9f;
        var x = document.GetLeftMargin();

        document.ShowTextAligned(CreateParagraph("Start:____________ End:____________", Calibri, 10), x, headerY, 1,
            TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);

        for (var i = 1; i <= numberOfPages; i++)
        {
            // Write aligned text to the specified by parameters point
            document.ShowTextAligned(new Paragraph($"Page {i} of {numberOfPages}").SetFontSize(8), centerX, footY, i,
                TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);

            document.ShowTextAligned(
                CreateParagraph(title, headerFont, 16, VerticalAlignment.MIDDLE, HorizontalAlignment.CENTER, true),
                centerX, headerY, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);

            document.ShowTextAligned(CreateParagraph(DateTime.Now.ToString("dd/MM/yyyy - HH:mm"), Calibri, 10), dateX, headerY, i,
                TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);
        }

        document.Close();
    }

    private static Cell CreateCell(IBlockElement element, TextAlignment? textAlignment = null, Color? backgroundColor = null)
    {
        var cell = new Cell().Add(element);

        if (textAlignment is not null) cell.SetTextAlignment(textAlignment);
        if (backgroundColor is not null) cell.SetBackgroundColor(backgroundColor);

        return cell;
    }

    private static Paragraph CreateParagraph(string text, PdfFont? font = null, int fontSize = 11,
        VerticalAlignment? verticalAlignment = null,
        HorizontalAlignment? horizontalAlignment = null,
        bool underline = false)
    {
        var p = new Paragraph(text).SetFontSize(fontSize);

        if (font is not null) p.SetFont(font);
        if (verticalAlignment is not null) p.SetVerticalAlignment(verticalAlignment);
        if (horizontalAlignment is not null) p.SetHorizontalAlignment(horizontalAlignment);
        if (underline) p.SetUnderline();

        return p;
    }
}

internal class RoundedCornersCellRenderer : CellRenderer
{
    public RoundedCornersCellRenderer(Cell modelElement) : base(modelElement)
    { }

    public override void DrawBorder(DrawContext drawContext)
    {
        var rectangle = this.GetOccupiedAreaBBox();
        var llx = rectangle.GetX() + 1;
        var lly = rectangle.GetY() + 1;
        var urx = rectangle.GetX() + this.GetOccupiedAreaBBox().GetWidth() - 1;
        var ury = rectangle.GetY() + this.GetOccupiedAreaBBox().GetHeight() - 1;
        var canvas = drawContext.GetCanvas();
        const float r = 4;
        const float b = 0.4477f;

        canvas.MoveTo(llx, lly).LineTo(urx, lly).LineTo(urx, ury - r).CurveTo(urx, ury - r * b, urx - r * b, ury,
            urx - r, ury).LineTo(llx + r, ury).CurveTo(llx + r * b, ury, llx, ury - r * b, llx, ury - r).LineTo(llx
            , lly).Stroke();
        base.DrawBorder(drawContext);
    }
}


internal class SampleEventHandler : IEventHandler
{
    public virtual void HandleEvent(Event @event)
    {
        var docEvent = (PdfDocumentEvent)@event;
        var pdfDoc = docEvent.GetDocument();
        var page = docEvent.GetPage();
        var pageNumber = pdfDoc.GetPageNumber(page);
        var pageSize = page.GetPageSize();
        var pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

        //Set background
        Color limeColor = new DeviceCmyk(0.208f, 0, 0.584f, 0);
        Color blueColor = new DeviceCmyk(0.445f, 0.0546f, 0, 0.0667f);
        Color whiteColor = new DeviceCmyk(1, 1, 1, 0);
        pdfCanvas.SaveState()
            .SetFillColor(pageNumber % 2 == 1 ? limeColor : blueColor)
            .Rectangle(pageSize.GetLeft(), pageSize.GetBottom(), pageSize.GetWidth(), pageSize.GetHeight())
            .Fill()
            .RestoreState();

        // Add header and footer
        // Each move moves from the previous position.
        pdfCanvas.BeginText()
            .SetFontAndSize(PDFDocuments.Helvetica, 9)
            .MoveText(pageSize.GetWidth() / 2 - 60, pageSize.GetTop() - 10) // -60 in width is accounting for the size of the text.
            .ShowText("THE TRUTH IS OUT THERE")
            .MoveText(60, -pageSize.GetTop() + 30)
            .ShowText($"Page {pageNumber}")
            .EndText();

        //Add watermark
        var canvas = new Canvas(pdfCanvas, page.GetPageSize());

        canvas.SetFontColor(whiteColor);
        canvas.SetFontSize(60);
        canvas.SetFont(PDFDocuments.HelveticaBold);

        canvas.ShowTextAligned(new Paragraph("CONFIDENTIAL"), 298, 421, pdfDoc.GetPageNumber(page), TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);
        pdfCanvas.Release();
    }
}
