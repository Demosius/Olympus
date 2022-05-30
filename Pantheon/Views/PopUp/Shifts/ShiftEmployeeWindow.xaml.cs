using Pantheon.ViewModels.Pages;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Shifts;

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