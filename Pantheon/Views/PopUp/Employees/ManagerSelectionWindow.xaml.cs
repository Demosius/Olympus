using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees
{
    /// <summary>
    /// Interaction logic for ManagerSelectionWindow.xaml
    /// </summary>
    public partial class ManagerSelectionWindow
    {
        public ManagerSelectionVM VM { get; set; }

        public ManagerSelectionWindow(Helios helios, Charon charon)
        {
            InitializeComponent();
            VM = new ManagerSelectionVM(helios, charon);
            DataContext = VM;
        }
    }
}
