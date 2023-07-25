using Hermes.ViewModels.Controls;
using Styx;
using Uranus;

namespace Hermes.Views.Controls;

/// <summary>
/// Interaction logic for NotificationsView.xaml
/// </summary>
public partial class NotificationsView
{
    public NotificationsView(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new NotificationsTabVM(helios, charon);
    }
}