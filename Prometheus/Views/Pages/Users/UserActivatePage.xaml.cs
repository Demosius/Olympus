using Prometheus.ViewModels.Pages.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages.Users;

/// <summary>
/// Interaction logic for UserActivatePage.xaml
/// </summary>
public partial class UserActivatePage
{
    public UserActivateVM VM { get; }

    public UserActivatePage(Helios helios, Charon charon)
    {
        VM = new UserActivateVM(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }
}