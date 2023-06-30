using Cadmus.ViewModels.Controls;
using Morpheus.ViewModels.Controls;
using Uranus;

namespace Cadmus.Views.Controls;

/// <summary>
/// Interaction logic for MixedCartonSOHView.xaml
/// </summary>
public partial class MixedCartonSOHView
{
    public MixedCartonSOH_VM VM { get; set; }

    public MixedCartonSOHView(Helios helios, ProgressBarVM progressBar)
    {
        VM = new MixedCartonSOH_VM(helios, progressBar);
        InitializeComponent();
        DataContext = VM;
    }
}