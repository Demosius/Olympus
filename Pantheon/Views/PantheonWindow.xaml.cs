using DocumentFormat.OpenXml.Drawing.Charts;
using Morpheus.ViewModels.Controls;
using Pantheon.ViewModels;
using Styx;
using Uranus;

namespace Pantheon.Views;

/// <summary>
/// Interaction logic for PantheonWindow.xaml
/// </summary>
public partial class PantheonWindow
{
    public AppVM VM { get; set; }

    public PantheonWindow()
    {
        VM = new AppVM();
        InitializeComponent();
        DataContext = VM;
    }
}