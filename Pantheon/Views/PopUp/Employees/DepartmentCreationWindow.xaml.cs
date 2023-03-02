using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for DepartmentCreationWindow.xaml
/// </summary>
public partial class DepartmentCreationWindow
{
    public DepartmentCreationVM VM { get; set; }

    public DepartmentCreationWindow(EmployeeVM parentVM)
    {
        InitializeComponent();
        VM = new DepartmentCreationVM(parentVM);
        DataContext = VM;
    }
}