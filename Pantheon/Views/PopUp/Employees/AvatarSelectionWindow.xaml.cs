using System;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for AvatarSelectionWindow.xaml
/// </summary>
public partial class AvatarSelectionWindow
{
    public AvatarSelectionVM? VM { get; set; }
    public EmployeeVM EmployeeVM { get; set; }

    public AvatarSelectionWindow(EmployeeVM employeeVM)
    {
        EmployeeVM = employeeVM;
        InitializeComponent();
    }

    private async void AvatarSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await AvatarSelectionVM.CreateAsync(EmployeeVM);
        DataContext = VM;
    }
}