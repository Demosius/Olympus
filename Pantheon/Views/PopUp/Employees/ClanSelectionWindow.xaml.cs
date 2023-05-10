using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for ClanSelectionWindow.xaml
/// </summary>
public partial class ClanSelectionWindow
{
    public ClanSelectionVM VM { get; set; }

    public ClanSelectionWindow(Helios helios, Charon charon, string? departmentName = null)
    {
        InitializeComponent();
        VM = new ClanSelectionVM(helios, charon, departmentName);
        DataContext = VM;
    }
}