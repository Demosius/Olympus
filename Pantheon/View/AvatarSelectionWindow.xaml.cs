using Pantheon.ViewModel.Pages;

namespace Pantheon.View;

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