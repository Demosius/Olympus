using Hermes.ViewModels.Controls;
using Styx;
using Uranus;

namespace Hermes.Views.Controls;

/// <summary>
/// Interaction logic for MessagingView.xaml
/// </summary>
public partial class MessagingView
{
    public MessagingView(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new MessagingTabVM(helios, charon);
    }
}