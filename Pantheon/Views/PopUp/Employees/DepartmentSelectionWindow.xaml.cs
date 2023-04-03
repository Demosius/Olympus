using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for DepartmentSelectionWindow.xaml
/// </summary>
public partial class DepartmentSelectionWindow
{
    public DepartmentSelectionVM VM { get; set; }

    public DepartmentSelectionWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new DepartmentSelectionVM(helios, charon);
        DataContext = VM;
    }
}