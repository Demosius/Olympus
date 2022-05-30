using Pantheon.ViewModels.Pages;

namespace Pantheon.Views.PopUp.Employees;

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