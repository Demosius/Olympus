using Uranus.Staff.Models;

namespace AionClock.Views;

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