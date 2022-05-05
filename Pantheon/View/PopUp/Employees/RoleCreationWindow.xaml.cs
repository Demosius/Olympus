using Pantheon.ViewModel.Pages;

namespace Pantheon.View.PopUp.Employees;

/// <summary>
/// Interaction logic for RoleCreationWindow.xaml
/// </summary>
public partial class RoleCreationWindow
{
    public RoleCreationWindow(EmployeePageVM employeePageVM)
    {
        InitializeComponent();
        VM.SetDataSources(employeePageVM);
    }
}