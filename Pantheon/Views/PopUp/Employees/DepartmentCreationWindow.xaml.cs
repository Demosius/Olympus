using Pantheon.ViewModels.Pages;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for DepartmentCreationWindow.xaml
/// </summary>
public partial class DepartmentCreationWindow
{
    public DepartmentCreationWindow(EmployeePageVM parentVM)
    {
        InitializeComponent();
        VM.SetDataSources(parentVM);
    }
}