using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for ShiftEmployeeWindow.xaml
/// </summary>
public partial class ShiftEmployeeWindow
{
    public ShiftEmployeeWindow(Helios helios, Charon charon, Shift shift)
    {
        InitializeComponent();
        VM.SetData(helios, charon, shift);
    }
}