using Hermes.ViewModels.Controls;
using Styx;
using Uranus;

namespace Hermes.Views.Controls;

/// <summary>
/// Interaction logic for ReportingView.xaml
/// </summary>
public partial class ReportingView
{
    public ReportingView(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new ReportingTabVM(helios, charon);
    }
}