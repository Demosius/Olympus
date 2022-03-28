using Pantheon.ViewModel.Pages;

namespace Pantheon.View;

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