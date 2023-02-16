using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Interfaces;
using Cadmus.Models;

namespace Cadmus.ViewModels.Labels;

public class RefOrgeLabelVM : ILabelVM, INotifyPropertyChanged
{
    public RefOrgeMasterLabel Label { get; set; }

    #region Image Dimensions

    public static int Width => 352;
    public static int Height => 242;
    public static int Row => 22;
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

    #region INotifyPropertyChanged Members

    public int Priority
    {
        get => Label.Priority;
        set
        {
            Label.Priority = value;
            OnPropertyChanged();
        }
    }
    


    #endregion

    public RefOrgeLabelVM(RefOrgeMasterLabel label)
    {
        Label = label;
    }

    public Image GetLabelImage()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}