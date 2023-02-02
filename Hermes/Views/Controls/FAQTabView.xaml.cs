using Hermes.ViewModels.Controls;
using Styx;
using Uranus;

namespace Hermes.Views.Controls;

/// <summary>
/// Interaction logic for FAQView.xaml
/// </summary>
public partial class FAQView
{
    public FAQView(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new FAQTabVM(helios, charon);
    }
}