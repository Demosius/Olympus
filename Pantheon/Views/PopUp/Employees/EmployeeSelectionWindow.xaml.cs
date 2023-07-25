using System.Collections.Generic;
using System.Linq;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for ManagerSelectionWindow.xaml
/// </summary>
public partial class EmployeeSelectionWindow
{
    public EmployeeSelectionVM VM { get; set; }

    public EmployeeVM? SelectedEmployee => VM.SelectedEmployee;

    public List<EmployeeVM> SelectedEmployees => VM.FullEmployeeList.Where(e => e.IsSelected).ToList();

    public EmployeeSelectionWindow(IEnumerable<EmployeeVM> employees, bool managers = false, string? department = null, bool multiSelect = false)
    {
        InitializeComponent();
        VM = new EmployeeSelectionVM(employees, managers, department, multiSelect);
        DataContext = VM;
    }
}