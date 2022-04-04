using Pantheon.ViewModel.Pages;
using Uranus.Staff.Model;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for EmployeeShiftWindow.xaml
/// </summary>
public partial class EmployeeShiftWindow
{
    public EmployeeShiftWindow(EmployeePageVM employeePageVM, Employee employee)
    {
        InitializeComponent();
        VM.SetData(employeePageVM, employee);
    }
}