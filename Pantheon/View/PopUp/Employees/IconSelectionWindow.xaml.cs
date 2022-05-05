using Pantheon.ViewModel.Pages;

namespace Pantheon.View.PopUp.Employees;

/// <summary>
/// Interaction logic for IconSelectionWindow.xaml
/// </summary>
public partial class IconSelectionWindow
{
    public IconSelectionWindow(EmployeePageVM employeePageVM)
    {
        InitializeComponent();
        VM.SetDataSource(employeePageVM);
    }
}