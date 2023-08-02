using System;
using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for RoleSelectionWindow.xaml
/// </summary>
public partial class RoleSelectionWindow
{
    public RoleSelectionVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public string? DepartmentName { get; set; }

    public RoleSelectionWindow(Helios helios, Charon charon, string? departmentName = null)
    {
        Helios = helios;
        Charon = charon;
        DepartmentName = departmentName;
        InitializeComponent();
    }

    private async void RoleSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await RoleSelectionVM.CreateAsync(Helios, Charon, DepartmentName);
        DataContext = VM;
    }
}