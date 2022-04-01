using Pantheon.ViewModel.Pages;
using Uranus.Staff.Model;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for ShiftEmployeeWindow.xaml
/// </summary>
public partial class ShiftEmployeeWindow
{
    public ShiftEmployeeWindow(ShiftPageVM shiftPageVM, Shift shift)
    {
        InitializeComponent();
        VM.SetData(shiftPageVM, shift);
    }
}