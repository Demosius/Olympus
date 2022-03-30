using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for EmployeeShiftWindow.xaml
/// </summary>
public partial class EmployeeShiftWindow
{
    public EmployeeShiftWindow(Helios helios, Charon charon, Employee employee)
    {
        InitializeComponent();
        VM.SetData(helios, charon, employee);
    }
}