using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for LocationSelectionWindow.xaml
/// </summary>
public partial class LocationSelectionWindow
{
    public LocationSelectionVM VM { get; set; }

    public LocationSelectionWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new LocationSelectionVM(helios, charon);
        DataContext = VM;
    }
}