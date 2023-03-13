using System.Data.Common;
using DocumentFormat.OpenXml.Drawing.Charts;
using Pantheon.ViewModels.Controls.Shifts;
using Pantheon.ViewModels.Pages;
using Pantheon.ViewModels.PopUp.Shifts;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Shifts;

/// <summary>
/// Interaction logic for ShiftEmployeeWindow.xaml
/// </summary>
public partial class ShiftEmployeeWindow
{
    public ShiftEmployeeVM VM { get; set; }

    public ShiftEmployeeWindow(ShiftVM shift)
    {
        InitializeComponent();
        VM = new ShiftEmployeeVM(shift);
        DataContext = VM;
    }
}