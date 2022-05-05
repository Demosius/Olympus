using Pantheon.ViewModel.Pages;

namespace Pantheon.View.PopUp.Employees;

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