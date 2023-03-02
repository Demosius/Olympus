using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for IconSelectionWindow.xaml
/// </summary>
public partial class IconSelectionWindow
{
    public IconSelectionVM VM { get; set; }

    public IconSelectionWindow(EmployeeVM employeeVM)
    {
        InitializeComponent();
        VM = new IconSelectionVM(employeeVM);
        DataContext = VM;
    }
}