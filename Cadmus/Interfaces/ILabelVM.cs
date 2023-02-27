using System.Drawing;
using System.Drawing.Printing;

namespace Cadmus.Interfaces;

public interface ILabelVM
{
    public Image GetLabelImage();
    public PaperSize PaperSize { get; }
    public Point PointPlacement { get; }
}