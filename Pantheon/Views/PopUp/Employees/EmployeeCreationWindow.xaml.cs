using Pantheon.ViewModels.Pages;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for EmployeeCreationWindow.xaml
/// </summary>
public partial class EmployeeCreationWindow
{
    public EmployeeCreationWindow(EmployeePageVM pageVM)
    {
        InitializeComponent();
        VM.SetDataSources(pageVM);
    }
}