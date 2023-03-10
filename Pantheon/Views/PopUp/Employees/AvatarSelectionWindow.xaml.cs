using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for AvatarSelectionWindow.xaml
/// </summary>
public partial class AvatarSelectionWindow
{
    public AvatarSelectionVM VM { get; }

    public AvatarSelectionWindow(EmployeeVM employeeVM)
    {
        InitializeComponent();
        VM = new AvatarSelectionVM(employeeVM);
        DataContext = VM;
    }
}