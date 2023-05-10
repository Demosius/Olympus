using Aion.ViewModels;
using Uranus;
using Uranus.Staff.Models;

namespace Aion.View;

/// <summary>
/// Interaction logic for EditEmployeeView.xaml
/// </summary>
public partial class EmployeeEditorWindow
{
    public EmployeeEditorVM VM { get; set; }

    public EmployeeEditorWindow(Helios helios, Employee employee, bool isNew)
    {
        VM = new EmployeeEditorVM(helios, employee, isNew);
        InitializeComponent();
        DataContext = VM;
    }
}