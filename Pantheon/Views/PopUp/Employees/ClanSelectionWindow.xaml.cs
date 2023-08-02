using System;
using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for ClanSelectionWindow.xaml
/// </summary>
public partial class ClanSelectionWindow
{
    public ClanSelectionVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public string? DepartmentName { get; set; }

    public ClanSelectionWindow(Helios helios, Charon charon, string? departmentName = null)
    {
        Helios = helios;
        Charon = charon;
        DepartmentName = departmentName;
        InitializeComponent();
    }

    private async void ClanSelectionWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ClanSelectionVM.CreateAsync(Helios, Charon, DepartmentName);
        DataContext = VM;
    }
}