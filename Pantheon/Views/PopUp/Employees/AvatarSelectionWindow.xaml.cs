using Pantheon.ViewModels.Pages;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for AvatarSelectionWindow.xaml
/// </summary>
public partial class AvatarSelectionWindow
{
    public AvatarSelectionWindow(EmployeePageVM employeePageVM)
    {
        InitializeComponent();
        VM.SetDataSource(employeePageVM);
    }
}