using System;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for IconSelectionWindow.xaml
/// </summary>
public partial class IconSelectionWindow
{
    public IconSelectionVM? VM { get; set; }
    public EmployeeVM EmployeeVM { get; set; }

    public IconSelectionWindow(EmployeeVM employeeVM)
    {
        EmployeeVM = employeeVM;
        InitializeComponent();
    }

    private async void IconSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await IconSelectionVM.CreateAsync(EmployeeVM);
        DataContext = VM;
    }
}