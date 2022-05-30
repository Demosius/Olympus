using Uranus;
using Uranus.Staff.Models;

namespace Aion.View;

/// <summary>
/// Interaction logic for EditEmployeeView.xaml
/// </summary>
public partial class EmployeeEditorWindow
{
    public EmployeeEditorWindow(Helios helios, Employee employee, bool isNew)
    {
        InitializeComponent();
        VM.SetData(helios, employee, isNew);
    }
}