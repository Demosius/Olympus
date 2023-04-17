using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for TempTagPage.xaml
/// </summary>
public partial class TempTagPage
{
    public TempTagPageVM VM { get; set; }

    public TempTagPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new TempTagPageVM(helios, charon);
        DataContext = VM;
    }
}