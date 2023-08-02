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
using Morpheus;
using ZXing;
using ZXing.Windows.Compatibility;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;
namespace Cadmus.ViewModels.Labels;

public class CartonLabelVM : INotifyPropertyChanged, ILabelVM
{
    public CartonLabel Label { get; set; }

    #region Image Dimensions

    public static int Width => 358;
    public static int Height => 221;
    public static int Row => 13;
    public static int Col01 => 47;
    public static int Col02 => 47;
    public static int Col03 => 12;
    public static int Col04 => 12;
    public static int Col05 => 44;
    public static int Col06 => 40;
    public static int Col07 => 26;
    public static int Col08 => 19;
    public static int Col09 => 12;
    public static int Col10 => 61;
    public static int Col11 => 19;
    public static int Col12 => 19;

    #endregion

    #region Image/Print Settings

    public float Dpi = 203;
    public CompositingQuality CompositingQuality = CompositingQuality.Default;
    public InterpolationMode InterpolationMode = InterpolationMode.NearestNeighbor;
    public SmoothingMode SmoothingMode = SmoothingMode.None;
    public PixelOffsetMode PixelOffsetMode = PixelOffsetMode.Half;

    public PaperSize PaperSize { get; } = new("CCN_Label", 450, 250);
    public Point PointPlacement { get; } = new(10, 5);

    #endregion

    #region INotifyPropertyChanged Changable Members

    public string StoreNo
    {
        get => Label.StoreNo;
        set
        {
            Label.StoreNo = value;
            OnPropertyChanged();
        }
    }

    public int Cartons
    {
        get => Label.Cartons;
        set
        {
            Label.Cartons = value;
            OnPropertyChanged();
        }
    }

    public double Weight
    {
        get => Label.Weight;
        set
        {
            Label.Weight = value;
            OnPropertyChanged();
        }
    }

    public double Cube
    {
        get => Label.Cube; 
        set
        {
            Label.Cube = value;
            OnPropertyChanged();
        }
    }

    public string CCN
    {
        get => Label.CCN;
        set
        {
            Label.CCN = value;
            Label.Barcode = BarcodeUtility.Encode128(value);
            GenerateBarcodeImage();
            OnPropertyChanged();
            OnPropertyChanged(nameof(Barcode));
        }
    }

    public string Barcode  => Label.Barcode;

    private BitmapImage barcodeImage;
    public BitmapImage BarcodeImage
    {
        get => barcodeImage;
        set
        {
            barcodeImage = value;
            OnPropertyChanged();
        }
    }

    public string CartonType
    {
        get => Label.CartonType;
        set
        {
            Label.CartonType = value;
            OnPropertyChanged();
        }
    }

    public string StartZone
    {
        get => Label.StartZone;
        set
        {
            Label.StartZone = value;
            OnPropertyChanged();
        }
    }

    public string EndZone
    {
        get => Label.EndZone;
        set
        {
            Label.EndZone = value;
            OnPropertyChanged();
        }
    }

    public string StartBin
    {
        get => Label.StartBin;
        set
        {
            Label.StartBin = value;
            OnPropertyChanged();
        }
    }

    public string EndBin
    {
        get => Label.EndBin;
        set
        {
            Label.EndBin = value;
            OnPropertyChanged();
        }
    }

    public string TOBatchNo
    {
        get => Label.TOBatchNo; 
        set
        {
            Label.TOBatchNo = value;
            OnPropertyChanged();
        }
    }

    public DateTime Date => Label.Date;

    public int TotalUnits
    {
        get => Label.TotalUnits;
        set
        {
            Label.TotalUnits = value;
            OnPropertyChanged();
        }
    }

    public string WaveNo
    {
        get => Label.WaveNo; set
        {
            Label.WaveNo = value;
            OnPropertyChanged();
        }
    }

    public string StockDescriptor
    {
        get => Label.StockDescriptor;
        set
        {
            Label.StockDescriptor = value;
            OnPropertyChanged();
        }
    }

    public string Carrier
    {
        get => Label.Carrier;
        set
        {
            Label.Carrier = value;
            OnPropertyChanged();
        }
    }


    #endregion

    public CartonLabelVM(CartonLabel label)
    {
        Label = label;
        barcodeImage = new BitmapImage();
        //GenerateBarcodeImage();
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

    public void GenerateBarcodeImage()
    {
        using var ms = new MemoryStream();

        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.CODE_128,
            Options =
            {
                Height = 90,
                Width = 190,
                PureBarcode = true
            }
        };

        Image img = writer.Write(CCN);
        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

        var image = new BitmapImage();
        image.BeginInit();
        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = null;
        image.StreamSource = ms;
        image.EndInit();

        BarcodeImage = image;
    }

    public Image GetLabelImage()
    {
        var dpi = Dpi;
        double resolutionFactor = dpi / 96;

        var width = (int)Math.Round((Width + 8) * resolutionFactor);
        var height = (int)Math.Round((Height + 4) * resolutionFactor);

        UserControl label = new CartonLabelView
        {
            DataContext = this
        };

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