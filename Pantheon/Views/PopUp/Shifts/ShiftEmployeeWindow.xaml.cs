using System;
using System.Windows;
using Pantheon.ViewModels.Controls.Shifts;
using Pantheon.ViewModels.PopUp.Shifts;

namespace Pantheon.Views.PopUp.Shifts;

/// <summary>
/// Interaction logic for ShiftEmployeeWindow.xaml
/// </summary>
public partial class ShiftEmployeeWindow
{
    public ShiftEmployeeVM? VM { get; set; }
    public ShiftVM Shift { get; set; }

    public ShiftEmployeeWindow(ShiftVM shift)
    {
        Shift = shift;
        InitializeComponent();
    }

    private async void ShiftEmployeeWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ShiftEmployeeVM.CreateAsync(Shift);
        DataContext = VM;
    }
}