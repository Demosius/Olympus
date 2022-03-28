using Pantheon.ViewModel.Pages;

namespace Pantheon.View;

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