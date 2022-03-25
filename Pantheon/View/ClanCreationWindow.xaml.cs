using Pantheon.ViewModel.Pages;
using Styx;
using Uranus;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for ClanCreationWindow.xaml
/// </summary>
public partial class ClanCreationWindow
{
    public ClanCreationWindow(EmployeePageVM parentVM)
    {
        InitializeComponent();
        VM.SetDataSources(parentVM);
    }
}