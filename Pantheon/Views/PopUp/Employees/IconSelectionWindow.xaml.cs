using Pantheon.ViewModels.Pages;

namespace Pantheon.Views.PopUp.Employees;

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