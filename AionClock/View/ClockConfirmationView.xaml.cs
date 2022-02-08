using Uranus.Staff.Model;

namespace AionClock.View
{
    /// <summary>
    /// Interaction logic for ClockConfirmationView.xaml
    /// </summary>
    public partial class ClockConfirmationView
    {
        public ClockConfirmationView(Employee employee)
        {
            InitializeComponent();
            VM.Employee = employee;
            VM.SetDisplay();
        }
    }
}
