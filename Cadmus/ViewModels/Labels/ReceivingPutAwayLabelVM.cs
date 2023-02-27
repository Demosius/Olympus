using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Interfaces;
using Cadmus.Models;
using Morpheus;
using ZXing;
using ZXing.Windows.Compatibility;

namespace Cadmus.ViewModels.Labels;

public class ReceivingPutAwayLabelVM : INotifyPropertyChanged, ILabelVM
{
    public ReceivingPutAwayLabel Label { get; set; }

    #region Image Dimensions

    public static int Width => 356;
    public static int Height => 231;
    public static int Row => 21;
    public static int Col1 => 85;
    public static int Col2 => 145;
    public static int Col3 => 63;
    public static int Col4 => 63;

    #endregion

    #region Static Fonts

    private static bool fontsSet;
    private static Font? calibriBold12;
    private static Font? calibriBoldUnder12;
    private static Font? calibriBold36;
    private static Font? courierBold10;
    private static Font? calibri16;
    private static Font? calibriBold10;
    private static Font? calibriBold9;
    private static Font? calibri11;

    #endregion

    #region Image/Print Settings

    public float Dpi = 203;
    public CompositingQuality CompositingQuality = CompositingQuality.Default;
    public InterpolationMode InterpolationMode = InterpolationMode.NearestNeighbor;
    public SmoothingMode SmoothingMode = SmoothingMode.None;
    public PixelOffsetMode PixelOffsetMode = PixelOffsetMode.Half;

    public PaperSize PaperSize { get; } = new("CCN_Label", 450, 250);
    public Point PointPlacement { get; } = new(5, 5);

    #endregion

    #region INotifyPropertyChanged Members

    public string TakeZone
    {
        get => Label.TakeZone;
        set
        {
            Label.TakeZone = value;
            OnPropertyChanged();
        }
    }

    public string TakeBin
    {
        get => Label.TakeBin;
        set
        {
            Label.TakeBin = value;
            OnPropertyChanged();
        }
    }

    public int CaseQty
    {
        get => Label.CaseQty;
        set
        {
            Label.CaseQty = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int PackQty
    {
        get => Label.PackQty;
        set
        {
            Label.PackQty = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int EachQty
    {
        get => Label.EachQty;
        set
        {
            Label.EachQty = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int QtyPerCase
    {
        get => Label.QtyPerCase;
        set
        {
            Label.QtyPerCase = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int QtyPerPack
    {
        get => Label.QtyPerPack;
        set
        {
            Label.QtyPerPack = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public string Barcode
    {
        get => Label.Barcode;
        set
        {
            Label.Barcode = value;
            OnPropertyChanged();
        }
    }

    public int ItemNumber
    {
        get => Label.ItemNumber;
        set
        {
            Label.ItemNumber = value;
            Label.Barcode = BarcodeUtility.Encode128(value.ToString());
            OnPropertyChanged();
            OnPropertyChanged(nameof(Barcode));
        }
    }

    public int LabelNumber
    {
        get => Label.LabelNumber;
        set
        {
            Label.LabelNumber = value;
            labelCountDisplay = $"{Label.LabelNumber} OF {Label.LabelTotal}";
            OnPropertyChanged();
            OnPropertyChanged(nameof(LabelCountDisplay));
        }
    }

    public int LabelTotal
    {
        get => Label.LabelTotal;
        set
        {
            Label.LabelTotal = value;
            labelCountDisplay = $"{Label.LabelNumber} OF {Label.LabelTotal}";
            OnPropertyChanged();
            OnPropertyChanged(nameof(LabelCountDisplay));
        }
    }

    private string labelCountDisplay;
    public string LabelCountDisplay
    {
        get => labelCountDisplay;
        set
        {
            labelCountDisplay = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => Label.Description;
        set
        {
            Label.Description = value;
            OnPropertyChanged();
        }
    }


    private string takeDisplayString;
    public string TakeDisplayString
    {
        get => takeDisplayString;
        set
        {
            takeDisplayString = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ReceivingPutAwayLabelVM(ReceivingPutAwayLabel label)
    {
        Label = label;

        takeDisplayString = string.Empty;
        labelCountDisplay = $"{Label.LabelNumber} OF {Label.LabelTotal}";

        SetTakeDisplayString();

        if (!fontsSet) SetFonts();
    }

    private static void SetFonts()
    {
        // Set fonts.
        calibriBold12 = new Font("Calibri", 12, FontStyle.Bold);
        calibriBoldUnder12 = new Font("Calibri", 12, FontStyle.Bold | FontStyle.Underline);
        calibriBold36 = new Font("Calibri", 36, FontStyle.Bold);
        courierBold10 = new Font("Courier New", 10, FontStyle.Bold);
        calibri16 = new Font("Calibri", 16, FontStyle.Regular);
        calibriBold10 = new Font("Calibri", 10, FontStyle.Bold);
        calibriBold9 = new Font("Calibri", 9, FontStyle.Bold);
        calibri11 = new Font("Calibri", 11, FontStyle.Regular);

        fontsSet = true;
    }

    public void SetTakeDisplayString()
    {
        string? s = null;

        if (CaseQty > 0)
            s = $"{CaseQty} CASE ({CaseQty * QtyPerCase})";

        if (EachQty > 0)
            s = $"{(s is null ? "" : $"{s}/n")}{EachQty} EACH ({EachQty})";

        if (PackQty > 0)
            s = $"{(s is null ? "" : $"{s}/n")}{PackQty} PACK ({PackQty + QtyPerPack})";

        TakeDisplayString = s ?? string.Empty;
    }


    public Image GetLabelImage()
    {
        var dpi = Dpi;
        double resolutionFactor = dpi / 96;

        var width = (int)Math.Round((Width + 8) * resolutionFactor);
        var height = (int)Math.Round((Height + 4) * resolutionFactor);
        var bmp = new Bitmap(width, height);
        bmp.SetResolution(dpi, dpi);

        var graphics = Graphics.FromImage(bmp);

        graphics.InterpolationMode = InterpolationMode;
        graphics.PixelOffsetMode = PixelOffsetMode;
        graphics.SmoothingMode = SmoothingMode;
        graphics.CompositingQuality = CompositingQuality;

        // Fonts
        // Set elements of label.
        // Column / Row values.
        var col1 = (int)Math.Round(Col1 * resolutionFactor);
        var col2 = (int)Math.Round(Col2 * resolutionFactor);
        var col3 = (int)Math.Round(Col3 * resolutionFactor);
        var col4 = (int)Math.Round(Col4 * resolutionFactor);

        var row = (int)Math.Round(Row * resolutionFactor);

        // Rectangles
        var fullRectangle = new Rectangle(0, 0, col1 + col2 + col3 + col4, row * 11);
        var takeZoneRectangle = new Rectangle(0, 0, col1 + col2 + col3 + col4, row);
        var pickRectangle = new Rectangle(0, row, col1 + col2, row);
        var takeBinRectangle = new Rectangle(0, row * 2, col1 + col2, row * 2);
        var takeDisplayRectangle = new Rectangle(0, row * 4, col1 + col2, row * 3);
        var descriptionAreaRectangle = new Rectangle(0, row * 7, col1 + col2, row * 4);
        var descCaptRectangle = new Rectangle(0, row * 7, col1, row);
        var descTextRectangle = new Rectangle(0, row * 8, col1 + col2, row * 3);
        var palletQtyRectangle = new Rectangle(col1 + col2, row, col3 + col4, row);
        var emptyRectangle = new Rectangle(col1 + col2, row * 2, col3 + col4, row * 5);
        var barcodeAreaRectangle = new Rectangle(col1 + col2, row * 7, col3 + col4, row * 3);
        var barcodeImageRectangle = new Rectangle(col1 + col2 + 2, row * 7 + 6, col3 + col4 + -4, row * 3 - 10);
        var labelNoRectangle = new Rectangle(col1 + col2, row * 10, col3, row);
        var labelDisplayRectangle = new Rectangle(col1 + col2 + col3, row * 10, col4, row);

        graphics.FillRectangle(Brushes.White, fullRectangle);
        graphics.DrawRectangle(Pens.Black, fullRectangle);
        graphics.DrawRectangle(Pens.Black, takeZoneRectangle);
        if (LabelNumber == 1)
        {
            graphics.FillRectangle(Brushes.Black, takeZoneRectangle);
        }
        graphics.DrawRectangle(Pens.Black, pickRectangle);
        graphics.DrawRectangle(Pens.Black, takeBinRectangle);
        graphics.DrawRectangle(Pens.Black, takeDisplayRectangle);
        graphics.DrawRectangle(Pens.Black, descriptionAreaRectangle);
        graphics.DrawRectangle(Pens.Black, descCaptRectangle);
        graphics.DrawRectangle(Pens.Black, palletQtyRectangle);
        graphics.DrawRectangle(Pens.Black, emptyRectangle);
        graphics.DrawRectangle(Pens.Black, barcodeAreaRectangle);
        graphics.DrawRectangle(Pens.Black, labelNoRectangle);
        graphics.DrawRectangle(Pens.Black, labelDisplayRectangle);

        // Strings
        var centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        graphics.DrawString(TakeZone, calibriBold12!, LabelNumber == 1 ? Brushes.White : Brushes.Black, takeZoneRectangle, centerFormat);
        graphics.DrawString("PICK", calibriBoldUnder12!, Brushes.Black, pickRectangle, centerFormat);
        graphics.DrawString(TakeBin, calibriBold36!, Brushes.Black, takeBinRectangle, centerFormat);
        graphics.DrawString(TakeDisplayString, courierBold10!, Brushes.Black, takeDisplayRectangle, centerFormat);
        graphics.DrawString("DESCRIPTION", calibriBold10!, Brushes.Black, descCaptRectangle, centerFormat);
        graphics.DrawString(Description, calibri16!, Brushes.Black, descTextRectangle, centerFormat);
        graphics.DrawString("Pallet Qty", calibriBoldUnder12!, Brushes.Black, palletQtyRectangle, centerFormat);
        graphics.DrawString("LABEL NO:", calibriBold9!, Brushes.Black, labelNoRectangle, centerFormat);
        graphics.DrawString(LabelCountDisplay, calibri11!, Brushes.Black, labelDisplayRectangle, centerFormat);

        // Barcode image.
        var barcodeWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.CODE_128,
        };

        using var ms = new MemoryStream();
        barcodeWriter.Write(ItemNumber.ToString()).Save(ms, ImageFormat.Bmp);

        var bcImage = (Bitmap)Image.FromStream(ms);

        graphics.DrawImage(bcImage, barcodeImageRectangle);

        return bmp;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}