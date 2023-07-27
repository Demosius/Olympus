using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cadmus.Annotations;
using Cadmus.Interfaces;
using Cadmus.Models;
using Cadmus.Views.Labels;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace Cadmus.ViewModels.Labels;

public class RefOrgeLabelVM : ILabelVM, INotifyPropertyChanged
{
    public RefOrgeMasterLabelVM Master { get; set; }

    #region Image Dimensions

    public static int Width => 352;
    public static int Height => 231;
    public static int Row => 21;
    public static int Col1 => 54;
    public static int Col2 => 8;
    public static int Col3 => 8;
    public static int Col4 => 54;
    public static int Col5 => 114;
    public static int Col6 => 114;

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

    #region INotifyPropertyChanged Changable Members

    public int Priority => Master.Priority;

    public string BatchName => Master.BatchName;

    public string OperatorName => Master.OperatorName;

    public DateTime Date => Master.Date;

    public string TakeBin => Master.TakeBin;

    public int CaseQty => Master.CaseQty;

    public int PackQty => Master.PackQty;

    public int EachQty => Master.EachQty;

    public string PlaceBin => Master.PlaceBin;

    public string Barcode => Master.Barcode;

    public int QtyPerCase => Master.QtyPerCase;

    public int QtyPerPack => Master.QtyPerPack;

    public int ItemNumber => Master.ItemNumber;

    public string ItemDescription => Master.ItemDescription;

    public int LabelTotal => Master.LabelTotal;

    private int labelNumber;
    public int LabelNumber
    {
        get => labelNumber;
        set
        {
            labelNumber = value;
            OnPropertyChanged();
        }
    }

    public string TakeDisplayString => Master.TakeDisplayString;

    public bool PickAsPacks => Master.PickAsPacks;

    public bool Web => Master.Web;

    public string CheckDigits => Master.CheckDigits ?? string.Empty;

    public string TotalGrab => Master.TotalGrab ?? string.Empty;

    public EMoveType MoveType => Master.MoveType;

    #endregion

    public bool IsMixed => Master.MixedCarton;

    public bool IsFirstLabel => labelNumber == 1;

    public object MixedContentDisplay => Master.MixedContentDisplay;

    public RefOrgeLabelVM(RefOrgeMasterLabelVM master, int labelNo)
    {
        Master = master;
        LabelNumber = labelNo;
    }

    private static Bitmap BitmapFromSource(BitmapSource bitmapSource)
    {
        using var outStream = new MemoryStream();

        // Create a BitmapEncoder and add the RenderTargetBitmap to it
        BitmapEncoder encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        // Save the encoded image to the MemoryStream
        encoder.Save(outStream);

        // Create a new Bitmap object from the MemoryStream
        var bitmap = new Bitmap(outStream);

        return bitmap;
    }

    public Image GetLabelImage()
    {
        var dpi = Dpi;
        double resolutionFactor = dpi / 96;

        var width = (int)Math.Round((Width + 8) * resolutionFactor);
        var height = (int)Math.Round((Height + 4) * resolutionFactor);

        UserControl label = IsMixed ? new RefOrgeMCLabelView() : new RefOrgeLabelView();
        label.DataContext = this;

        label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        label.Arrange(new Rect(label.DesiredSize));
        label.UpdateLayout();
        
        var renderer = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
        renderer.Render(label);

        var bitmap = BitmapFromSource(renderer);

        return bitmap;
    }

    public void UpdateDisplay([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(propertyName);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}