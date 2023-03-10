using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees
{
    /// <summary>
    /// Interaction logic for ManagerSelectionWindow.xaml
    /// </summary>
    public partial class EmployeeSelectionWindow
    {
        public EmployeeSelectionVM VM { get; set; }

        public EmployeeSelectionWindow(Helios helios, Charon charon, bool managers = false, string? department = null)
        {
            InitializeComponent();
            VM = new EmployeeSelectionVM(helios, charon, managers, department);
            DataContext = VM;
        }
    }
}
