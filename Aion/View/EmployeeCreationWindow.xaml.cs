using Uranus;

namespace Aion.View
{
    public partial class EmployeeCreationWindow
    {
        public EmployeeCreationWindow(Helios helios)
        {
            InitializeComponent();
            VM.SetDataSource(helios);
        }
    }
}
