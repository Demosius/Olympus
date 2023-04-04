using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for EmployeeCreationWindow.xaml
/// </summary>
public partial class EmployeeCreationWindow
{
    public EmployeeCreationVM VM { get; }

    public EmployeeCreationWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new EmployeeCreationVM(helios, charon);
        DataContext = VM;
    }
}