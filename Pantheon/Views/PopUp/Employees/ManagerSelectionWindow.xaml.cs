using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees
{
    /// <summary>
    /// Interaction logic for ManagerSelectionWindow.xaml
    /// </summary>
    public partial class ManagerSelectionWindow
    {
        public ManagerSelectionVM VM { get; set; }

        public ManagerSelectionWindow(EmployeeVM employee)
        {
            InitializeComponent();
            VM = new ManagerSelectionVM(employee);
            DataContext = VM;
        }
    }
}
