using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for ClanCreationWindow.xaml
/// </summary>
public partial class ClanCreationWindow
{
    public ClanCreationVM VM { get; set; }

    public ClanCreationWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new ClanCreationVM(helios, charon);
        DataContext = VM;
    }
}