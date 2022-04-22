using Prometheus.ViewModel.Helpers;
using Styx;
using Uranus;

namespace Prometheus.View.Pages;

/// <summary>
/// Interaction logic for UserPage.xaml
/// </summary>
public partial class UserPage
{
    public UserPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }

    public override EDataCategory DataCategory => EDataCategory.Users;

}